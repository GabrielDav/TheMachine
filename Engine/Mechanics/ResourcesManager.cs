using System;
using System.Collections.Generic;
#if EDITOR
using System;
using System.ComponentModel.Design;
#endif
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Engine.Mechanics
{

    public class ResourceCollection : List<ResourceIdentifier>
    {
        public ResourceIdentifier this[string name]
        {
            get
            {
                foreach (var item in this)
                {
                    if (item.Name == name)
                        return item;
                }
                return null;
            }
        }

    }

    public enum ResourceType
    {
        Texture,
        Sprite,
        Sound,
        Song,
        Font
    }

    public class ResourceIdentifier
    {
        public string Name { get; set; }

        public string[] Path { get; set; }

        public ResourceIdentifier[] SubResources { get; set; }

        public ResourceType ResourceType { get; set; }

        public ResourceIdentifier()
        {
        }

        public ResourceIdentifier(string name, string[] path, ResourceType resourceType)
        {
            Name = name;
            Path = path;
            ResourceType = resourceType;
        }

        public override string ToString()
        {
            return Name + "(" + ResourceType + ")";
        }

        public ResourceIdentifier Clone()
        {
            var clone = (ResourceIdentifier) MemberwiseClone();
            clone.Path = new string[Path.Length];
            Array.Copy(Path, clone.Path, Path.Length);
            return clone;
        }

    }

#if EDITOR
    public class DictionaryEditor : CollectionEditor
    {
        public DictionaryEditor(Type type)
            : base(type)
        {

        }

        public static SimpleEvent CollectionEditorClosed;

        protected override bool CanRemoveInstance(object value)
        {
            return false;
        }

        protected override CollectionForm CreateCollectionForm()
        {
            var form = base.CreateCollectionForm();
            form.Closed += (sender, args) => { if (CollectionEditorClosed != null) CollectionEditorClosed(this); };
            return form;
        }

        public object[] Items
        {
            get { return this.GetItems(this); }
        }

        public object[] GI(object a)
        {
            return this.GetItems(a);
        }

        protected override string GetDisplayText(object value)
        {
            if (((ResourceIdentifier) value).Name == null && ((ResourceIdentifier) value).Name == null)
                return "Undefined";
            if (((ResourceIdentifier) value).Name == "")
                return "Undefined";
            return ((ResourceIdentifier) value).Name;
        }
    }
#endif

    public class ResourcesManager : IDisposable
    {
        protected Dictionary<string, IList<GameTexture>> _textures;
        protected Dictionary<string, IList<SpriteData>> _sprites;
        protected Dictionary<string, IList<SoundEffect>> _sounds;
        protected Dictionary<string, Song> _songs;
        protected Dictionary<string, SpriteFont> _fonts;

        public ReadOnlyDictionary<string, IList<GameTexture>> Textures;
        public ReadOnlyDictionary<string, IList<SpriteData>> Sprites;
        public ReadOnlyDictionary<string, IList<SoundEffect>> Sounds;
        public ReadOnlyDictionary<string, Song> Songs;
        public ReadOnlyDictionary<string, SpriteFont> Fonts;

        public ResourcesManager()
        {
            _textures = new Dictionary<string, IList<GameTexture>>();
            _sprites = new Dictionary<string, IList<SpriteData>>();
            _sounds = new Dictionary<string, IList<SoundEffect>>();
            _songs = new Dictionary<string, Song>();
            _fonts = new Dictionary<string, SpriteFont>();
            Textures = new ReadOnlyDictionary<string, IList<GameTexture>>(_textures);
            Sprites = new ReadOnlyDictionary<string, IList<SpriteData>>(_sprites);
            Sounds = new ReadOnlyDictionary<string, IList<SoundEffect>>(_sounds);
            Songs = new ReadOnlyDictionary<string, Song>(_songs);
            Fonts = new ReadOnlyDictionary<string, SpriteFont>(_fonts);
        }


        public void LoadTexture(ResourceIdentifier textureIdentifier)
        {
            if (!_textures.ContainsKey(textureIdentifier.Name) || _textures[textureIdentifier.Name] == null)
            {
                var textures = new List<GameTexture>();
                foreach (var path in textureIdentifier.Path)
                    textures.Add(new GameTexture(path));
                if (!_textures.ContainsKey(textureIdentifier.Name))
                    _textures.Add(textureIdentifier.Name, textures);
                else if (_textures[textureIdentifier.Name] == null)
                    _textures[textureIdentifier.Name] = textures;
                if (textureIdentifier.SubResources != null)
                {
                    foreach (var subResource in textureIdentifier.SubResources)
                    {
                        LoadTexture(subResource);
                    }
                }
            }
        }

        public void LoadSprite(ResourceIdentifier spriteIdentifier)
        {
            if (!_sprites.ContainsKey(spriteIdentifier.Name) || _sprites[spriteIdentifier.Name] == null)
            {
                var sprites = new List<SpriteData>();
                foreach (var path in spriteIdentifier.Path)
                    sprites.Add(EngineGlobals.ContentCache.Load<SpriteData>(path));
                if (!_sprites.ContainsKey(spriteIdentifier.Name))
                    _sprites.Add(spriteIdentifier.Name, sprites);
                else if (_sprites[spriteIdentifier.Name] == null)
                    _sprites[spriteIdentifier.Name] = sprites;
            }
        }

        public void LoadSound(ResourceIdentifier soundIdentifier)
        {
            if (!_sounds.ContainsKey(soundIdentifier.Name) || _sounds[soundIdentifier.Name] == null)
            {
                var sounds = new List<SoundEffect>();
                foreach (var path in soundIdentifier.Path)
                {
                    sounds.Add(EngineGlobals.ContentCache.Load<SoundEffect>(path));
                }
                if (!_sounds.ContainsKey(soundIdentifier.Name))
                    _sounds.Add(soundIdentifier.Name, sounds);
                else if (_sounds[soundIdentifier.Name] == null)
                    _sounds[soundIdentifier.Name] = sounds;
            }
        }

        public void LoadSong(ResourceIdentifier songIdentifier)
        {
            if (!_songs.ContainsKey(songIdentifier.Name) || _songs[songIdentifier.Name] == null)
            {
                _songs[songIdentifier.Name] = EngineGlobals.ContentCache.Load<Song>(songIdentifier.Path[0]);
            }
        }

        public void LoadFont(ResourceIdentifier fontIdentifier)
        {
            if (!_fonts.ContainsKey(fontIdentifier.Name) || _fonts[fontIdentifier.Name] == null)
            {
                _fonts[fontIdentifier.Name] = EngineGlobals.ContentCache.Load<SpriteFont>(fontIdentifier.Path[0]);
            }
        }

        public void LoadResources(MapResources mapResources)
        {
            foreach (ResourceIdentifier textureIdentifier in mapResources.TextureIdentifiers)
            {
                LoadTexture(textureIdentifier);
            }
            foreach (ResourceIdentifier spriteIdentifier in mapResources.SpriteIdentifiers)
            {
                LoadSprite(spriteIdentifier);  
            }
            foreach (var soundIdentifier in mapResources.SoundIdentifiers)
            {
                LoadSound(soundIdentifier);
            }
        }

        public void LoadResource(ResourceIdentifier resource)
        {
            switch (resource.ResourceType)
            {
                case ResourceType.Texture:
                    LoadTexture(resource);
                    break;
                case ResourceType.Sprite:
                    LoadSprite(resource);
                    break;
                case ResourceType.Sound:
                    LoadSound(resource);
                    break;
                case ResourceType.Song:
                    LoadSong(resource);
                    break;
                case ResourceType.Font:
                    LoadFont(resource);
                    break;
            }
        }

        public void DisposeTexture(string name)
        {
            throw new Exception("Do Not Use");
            foreach (var texture in _textures[name])
            {
                texture.Data.Dispose();
            }
            _textures.Remove(name);
        }

        public void DisposeSprite(string name)
        {
            foreach (var sprite in _sprites[name])
            {
                sprite.Dispose();
            }
            _sprites.Remove(name);
        }

        public void DisposeSound(string name)
        {
            throw new Exception("Do Not Use");
            foreach (var sound in _sounds[name])
            {
                sound.Dispose();
            }
            _sounds.Remove(name);
        }

        public void DisposeSong(string name)
        {
            throw new Exception("Do Not Use");
            _songs[name].Dispose();
            _songs.Remove(name);
        }

        public void DisposeFont(string name)
        {
            throw new Exception("Do Not Use");
            _fonts.Remove(name);
        }

        public void DisposeResource(ResourceIdentifier resource)
        {
            throw new Exception("Do Not Use");
            switch (resource.ResourceType)
            {
                case ResourceType.Texture:
                    DisposeTexture(resource.Name);
                    break;
                case ResourceType.Sprite:
                    DisposeSprite(resource.Name);
                    break;
                case ResourceType.Sound:
                    DisposeSound(resource.Name);
                    break;
                case ResourceType.Song:
                    DisposeSong(resource.Name);
                    break;
                case ResourceType.Font:
                    DisposeFont(resource.Name);
                    break;
            }
        }

        public void DisposeResources(MapResources resources)
        {
            throw new Exception("Do Not Use");
            foreach (ResourceIdentifier textureIdentifier in resources.TextureIdentifiers)
            {
                DisposeTexture(textureIdentifier.Name);
            }
            foreach (ResourceIdentifier spriteIdentifier in resources.SpriteIdentifiers)
            {
                DisposeSprite(spriteIdentifier.Name);
            }
            foreach (var soundIdentifier in resources.SoundIdentifiers)
            {
                DisposeSound(soundIdentifier.Name);
            }
        }


        public void Dispose()
        {
            foreach (var texture in _textures)
            {
                DisposeTexture(texture.Key);
            }
            _textures.Clear();
            foreach (var sprite in _sprites)
            {
                DisposeSprite(sprite.Key);
            }
            _sprites.Clear();
            foreach (var sound in _sounds)
            {
                DisposeSound(sound.Key);
            }
            _sounds.Clear();
            foreach (var song in _songs)
            {
                DisposeSong(song.Key);
            }
            _songs.Clear();
            _fonts.Clear();
            _textures = null;
            _sprites = null;
            _sounds = null;
            _songs = null;
            _fonts = null;
            Textures = null;
            Sprites = null;
            Sounds = null;
            Songs = null;
            Fonts = null;
        }
    }



}
