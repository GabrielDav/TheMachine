using System;
using System.ComponentModel;
using System.Linq;
using Engine.Core;

namespace Engine.Mechanics
{
#if EDITOR
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("Map resources")]
#endif
    public class MapResources : IDisposable
    {
        protected ResourceCollection _textureIdentifiers;
        protected ResourceCollection _spriteIdentifiers;
        protected ResourceCollection _soundIdentifiers;
        

#if EDITOR
        [Editor(typeof(DictionaryEditor),
            typeof(System.Drawing.Design.UITypeEditor)), DisplayName("Textures")]
        [Description("A collection of a textures used within map")]
#endif
        public ResourceCollection TextureIdentifiers
        {
            get { return _textureIdentifiers; }
            set { _textureIdentifiers = value; }
        }

#if EDITOR
        [Editor(typeof(DictionaryEditor), typeof(System.Drawing.Design.UITypeEditor)), DisplayName("Sprites")]
        [Description("A collection of a sprites used within map")]
#endif
        public ResourceCollection SpriteIdentifiers
        {
            get { return _spriteIdentifiers; }
            set { _spriteIdentifiers = value; }
        }

#if EDITOR
        [Editor(typeof (DictionaryEditor), typeof (System.Drawing.Design.UITypeEditor)), DisplayName("Sounds")]
        [Description("A collection of a sounds used within map")]
#endif
        public ResourceCollection SoundIdentifiers
        {
            get { return _soundIdentifiers; }
            set { _soundIdentifiers = value; }
        }


        public MapResources()
        {
            _textureIdentifiers = new ResourceCollection();
            _spriteIdentifiers = new ResourceCollection();
            _soundIdentifiers = new ResourceCollection();
        }

        public void Add(ResourceIdentifier resource)
        {
            if (resource.ResourceType == ResourceType.Texture && !_textureIdentifiers.Any(item => item.Name == resource.Name))
            {
                _textureIdentifiers.Add(resource);
                EngineGlobals.Resources.LoadTexture(resource);
            }
            else if (resource.ResourceType == ResourceType.Sprite && !_spriteIdentifiers.Any(item => item.Name == resource.Name))
            {
                _spriteIdentifiers.Add(resource);
                EngineGlobals.Resources.LoadSprite(resource);
            }
        }

        public ResourceIdentifier GetResourceIdentifierById(string resourceId)
        {
            foreach (var spriteIdentifier in SpriteIdentifiers)
            {
                if (spriteIdentifier.Name == resourceId)
                    return spriteIdentifier;
            }
            foreach (var textureIdentifier in _textureIdentifiers)
            {
                if (textureIdentifier.Name == resourceId)
                    return textureIdentifier;
            }
            return null;
        }

        public void Remove(string resourceName)
        {
            var resource = TextureIdentifiers.FirstOrDefault(item => item.Name == resourceName);
            if (resource != null)
            {
                TextureIdentifiers.Remove(resource);
                return;
            }
            resource = SpriteIdentifiers.FirstOrDefault(item => item.Name == resourceName);
            if (resource != null)
            {
                SpriteIdentifiers.Remove(resource);
                return;
            }
            resource = SoundIdentifiers.FirstOrDefault(item => item.Name == resourceName);
            if (resource != null)
            {
                SoundIdentifiers.Remove(resource);
                return;
            }
            throw new Exception("Resource '" + resourceName + "' not found.");

        }

        public override string ToString()
        {
            return "Map Resources";
        }

        public object Clone()
        {
            var clone = (MapResources) MemberwiseClone();
            clone.SoundIdentifiers = new ResourceCollection();
            foreach (var soundIdentifier in SoundIdentifiers)
            {
                clone.SoundIdentifiers.Add(soundIdentifier.Clone());
            }
            clone.TextureIdentifiers = new ResourceCollection();
            foreach (var textureIdentifier in TextureIdentifiers)
            {
                clone.TextureIdentifiers.Add(textureIdentifier);
            }
            clone.SpriteIdentifiers = new ResourceCollection();
            foreach (var spriteIdentifier in SpriteIdentifiers)
            {
                clone.SpriteIdentifiers.Add(spriteIdentifier.Clone());
            }
            return clone;
        }

        public void Dispose()
        {
            TextureIdentifiers.Clear();
            SpriteIdentifiers.Clear();
            SoundIdentifiers.Clear();
        }
    }
}