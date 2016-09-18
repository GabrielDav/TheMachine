using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;
#if EDITOR
using System.ComponentModel;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework.Graphics;
using Image = Engine.Graphics.Image;
using Plane = GameLibrary.Objects.Plane;
using Player = GameLibrary.Objects.Player;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
#else

#endif

namespace GameLibrary
{
   
    public enum MapType { Game = 0, Background = 1, Menu = 2}
#if EDITOR
    [TypeConverter(typeof (PropertySorter))]
    [DefaultProperty("Name")]
    public class Map : ICloneable, IDisposable
    {

        protected string _background;

        [PropertyOrder(6)]
        [ReadOnly(true)]
        public int Version { get; set; }

        [PropertyOrder(1)]
        public int Width { get; set; }

        [PropertyOrder(2)]
        public int Height { get; set; }

        [PropertyOrder(12)]
        public Color BackgroundColor { get; set; }

        [PropertyOrder(4)]
        //[EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Background
        {
            get { return _background; }
            set
            {
                _background = value;
                if (BackgroundChanged != null) BackgroundChanged(this);
            }
        }

        [PropertyOrder(5)]
        public float Gravity
        {
            get { return EngineGlobals.Gravity.Y; }
            set { EngineGlobals.Gravity.Y = value; }
        }

        [PropertyOrder(8)]
        [ReadOnly(true)]
        public MapType MapType { get; set; }

        [PropertyOrder(3)]
        public MapResources Resources { get; set; }

        [Browsable(false)]
        public List<PhysicalObject> GameObjects;

        [Browsable(false)]
        public List<PhysicalObject> BackgroundObjects;

        [Browsable(false)]
        public List<Region> Regions;

        public event SimpleEvent BackgroundChanged;

        [Browsable(false)]
        public List<Trigger> Triggers;


        public Map()
        {
            GameObjects = new List<PhysicalObject>();
            BackgroundObjects = new List<PhysicalObject>();
            Resources = new MapResources();
            Regions = new List<Region>();
            Triggers = new List<Trigger>();
            BackgroundColor = Color.White;
        }

        public void Save(Stream stream, bool temp = false)
        {
            var settings = new XmlWriterSettings {Indent = true};
            /*var texturesRes = new ResourceCollection();
            var spritesRes = new ResourceCollection();
            foreach (var obj in GameObjects)
            {
                if (obj.Animated)
                {
                    if (!spritesRes.Contains(Resources.SpriteIdentifiers[obj.ResourceId]))
                        spritesRes.Add(Resources.SpriteIdentifiers[obj.ResourceId]);
                }
                else if (!texturesRes.Contains(Resources.TextureIdentifiers[obj.ResourceId]))
                    texturesRes.Add(Resources.TextureIdentifiers[obj.ResourceId]);
            }
            Resources.Textures.Clear();
            Resources.TextureIdentifiers = texturesRes;
            Resources.Sprites.Clear();
            Resources.SpriteIdentifiers = spritesRes;*/
            if (!temp)
                Version++;
            using (var writer = XmlWriter.Create(stream, settings))
            {
                IntermediateSerializer.Serialize(writer, this, null);
            }
        }

        /*public static Map LoadEditor(string fname)
        {
            Map map;
            EngineGlobals.MapIsLoading = true;
            using (var reader = XmlReader.Create(fname))
            {
                map = IntermediateSerializer.Deserialize<Map>(reader, "map");
            }
            EngineGlobals.MapIsLoading = false;
            

            return map;
        }*/

        public void ImportBackground(string fname)
        {
            Map map;
            using (var reader = XmlReader.Create(fname))
            {
                map = IntermediateSerializer.Deserialize<Map>(reader, "map");
            }
            Controller.ClearBackgroundobjects();
            BackgroundObjects.Clear();
            foreach (var backgroundObject in map.BackgroundObjects)
            {
                backgroundObject.Load(backgroundObject.ResourceId, backgroundObject.ResourceVariation);
                Controller.AddBackgroundObject(backgroundObject);
                BackgroundObjects.Add(backgroundObject);
            }
            foreach (var textureIdentifier in map.Resources.TextureIdentifiers)
            {
                if (!Resources.TextureIdentifiers.Exists(e => e.Name == textureIdentifier.Name))
                    Resources.TextureIdentifiers.Add(textureIdentifier);
            }
            foreach (var spriteIdentifier in map.Resources.SpriteIdentifiers)
            {
                if (!Resources.SpriteIdentifiers.Exists(e => e.Name == spriteIdentifier.Name))
                    Resources.SpriteIdentifiers.Add(spriteIdentifier);
            }
            Background = map.Background;
        }

        public static Map LoadEditor(Stream stream)
        {
           
            Map map;
            EngineGlobals.MapIsLoading = true;
            using (var reader = XmlReader.Create(stream))
            {
                map = IntermediateSerializer.Deserialize<Map>(reader, "map");
            }
            foreach (var physicalObject in map.GameObjects)
            {
                physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
                physicalObject.IgnoreGridBounds = false;
                Controller.AddGameObject(physicalObject);
            }
            foreach (var backgroundObject in map.BackgroundObjects)
            {
                backgroundObject.Load(backgroundObject.ResourceId, backgroundObject.ResourceVariation);
                Controller.AddBackgroundObject(backgroundObject);
            }
            foreach (var region in map.Regions)
            {
                region.Drawable = true;
            }
            if (!string.IsNullOrEmpty(map.Background))
            {
                var texture = EngineGlobals.ContentCache.Load<Texture2D>(map.Background);
                EngineGlobals.Background.LoadBackground(new GameTexture(texture));
            }
            EngineGlobals.MapIsLoading = false;

            return map;
        }

        public static Map Load(string fname, ContentManager assetsManager)
        {
            
            EngineGlobals.MapIsLoading = true;
            GameGlobals.Map = assetsManager.Load<Map>(fname);
            EngineGlobals.Grid = new Grid(new Rectangle(0, 0, GameGlobals.Map.Width / 10, GameGlobals.Map.Height / 10), 10, 10, true);
            EngineGlobals.Resources.LoadResources(GameGlobals.Map.Resources);
            /*foreach (var physicalObject in GameGlobals.Map.GameObjects)
            {
                physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
                if (physicalObject.TypeId == GameObjectType.Player.ToString())
                    GameGlobals.Player = (Player)physicalObject;
            }
            foreach (var backgroundObject in GameGlobals.Map.BackgroundObjects)
            {
                backgroundObject.Load(backgroundObject.ResourceId, backgroundObject.ResourceVariation);
            }*/
            EngineGlobals.MapIsLoading = false;
            return GameGlobals.Map;

        }

        public PhysicalObject CreateObjectVirtual(GameObjectType typeId, string resourceId, int subObjectId = 0)
        {
            PhysicalObject obj;
            switch (typeId)
            {
                case GameObjectType.Player:
                    obj = new Player();
                    break;
                case GameObjectType.Circle:
                    obj = new Circle();
                    break;
                case GameObjectType.Plane:
                    obj = new Plane();
                    break;
                case GameObjectType.RedBall:
                    obj = new RedBall();
                    break;
                case GameObjectType.Spike:
                    obj = new Spike();
                    break;
                case GameObjectType.Water:
                    obj = new Water();
                    break;
                case GameObjectType.InkDot:
                    obj = new InkDot();
                    break;
                case GameObjectType.BackgroundGear:
                    obj = new BackgroundObject();
                    break;
                case GameObjectType.BackgroundGearSmall:
                    obj = new BackgroundGearSmall();
                    break;
                case GameObjectType.Wall:
                    obj = new WallSlide();
                    break;
                case GameObjectType.WallHand:
                    obj = new WallHand();
                    break;
                case GameObjectType.Saw:
                    obj = new Saw();
                    break;
                case GameObjectType.Tile:
                    obj = new Tile();
                    break;
                case GameObjectType.DecorativeObject:
                    obj = new DecorativeObject();
                    break;
                case GameObjectType.DeathBall:
                    obj = new DeathBall();
                    break;
                case GameObjectType.MenuObject:
                    obj = new MenuObject();
                    break;
                case GameObjectType.MenuLine:
                    obj = new MenuLine();
                    break;
                case GameObjectType.MenuDec:
                    obj = new MenuDec();
                    break;
                case GameObjectType.MenuGear:
                    obj = new MenuGear();
                    break;
                case GameObjectType.MenuBtnPointer:
                    obj = new MenuBtnPointer();
                    break;
                case GameObjectType.MenuBtnMap:
                    obj = new MenuMapBtn();
                    break;
                case GameObjectType.SpikeShooter:
                    obj = new SpikeShooter();
                    break;
                case GameObjectType.LevelEnd:
                    obj = new LevelEnd();
                    break;
                case GameObjectType.TrapBtn:
                    obj = new TrapBtn();
                    break;
                case GameObjectType.CameraPath:
                    obj = new CameraPath();
                    break;
                case GameObjectType.CircleSpikes:
                    obj = new CircleSpikes();
                    break;
                case GameObjectType.TrapDoor:
                    obj = new TrapDoors();
                    break;
                case GameObjectType.SlidePlane:
                    obj = new SlidePlane();
                    break;
                case GameObjectType.DeathPlane:
                    obj = new DeathPlane();
                    break;
                case GameObjectType.JumpSpot:
                    obj = new JumpSpot();
                    break;
                case GameObjectType.MovingCircle:
                    obj = new MovingCircle();
                    break;
                case GameObjectType.PowerUp:
                    obj = new PowerUp();
                    break;
                case GameObjectType.SeekerDot:
                    obj = new SeekerDot();
                    break;
                case GameObjectType.ScoreDisplayDevice:
                    obj = new ScoreDisplayDevice();
                    break;
                case GameObjectType.VersionInfo:
                    obj = new VersionInformation();
                    break;
                case GameObjectType.SwitchBtn:
                    obj = new SwitchBtn();
                    break;
                case GameObjectType.Hint:
                    obj = new HintBox();
                    break;
                case GameObjectType.ResizableDecorativeObject:
                    obj = new ResizableDecorativeObject();
                    break;
                case GameObjectType.SpikeSmall:
                    obj = new SpikeSmall();
                    break;
                default:
                    throw new Exception("Unknown map object type: " + typeId);
            }
            var index = 0;
            if (EngineGlobals.Resources.Sprites.ContainsKey(resourceId))
            {
                if (EngineGlobals.Resources.Sprites[resourceId].Count > 1)
                {
                    index = GameGlobals.Random.Next(0, EngineGlobals.Resources.Sprites[resourceId].Count - 1);
                }
            }
            else if (EngineGlobals.Resources.Textures.ContainsKey(resourceId))
            {
                if (EngineGlobals.Resources.Textures[resourceId].Count > 1)
                {
                    index = GameGlobals.Random.Next(0, EngineGlobals.Resources.Textures[resourceId].Count - 1);
                }
            }

                obj.LoadDefault(resourceId, index, subObjectId);
            return obj;
        }

        public void AddObject(PhysicalObject physicalObject)
        {
            if (physicalObject is BackgroundObject)
            {
                BackgroundObjects.Add(physicalObject);
                Controller.AddBackgroundObject(physicalObject);
            }
            else
            {
                GameObjects.Add(physicalObject);
                Controller.AddGameObject(physicalObject);
            }
            
        }

        public PhysicalObject CreateObject(GameObjectType typeId, string resourceId, DecorativeType decorativeType = DecorativeType.None)
        {
            var obj = CreateObjectVirtual(typeId, resourceId);
            GameObjects.Add(obj);
            Controller.AddObject(obj.Mask);
            return obj;
        }

        public override string ToString()
        {
            return "Map";
        }
#else

    public class Map : IDisposable
    {
        protected string _background;

        public int Version { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Color BackgroundColor { get; set; }

        public string Background
        {
            get { return _background; }
            set
            {
                _background = value;
            }
        }

        public float Gravity
        {
            get { return EngineGlobals.Gravity.Y; }
            set { EngineGlobals.Gravity.Y = value; }
        }

        public MapType MapType { get; set; }

        public MapResources Resources { get; set; }

        public List<PhysicalObject> GameObjects;

        public List<PhysicalObject> BackgroundObjects;

        public List<Region> Regions;

        public List<Trigger> Triggers;

        public Map()
        {
            GameObjects = new List<PhysicalObject>();
            BackgroundObjects = new List<PhysicalObject>();
            Regions = new List<Region>();
        }

        public static Map Load(string fname, ContentManager assetsManager)
        {
            EngineGlobals.MapIsLoading = true;
            GameGlobals.Map = assetsManager.Load<Map>(fname);
            EngineGlobals.Grid = new Grid(new Rectangle(0, 0, GameGlobals.Map.Width / 10, GameGlobals.Map.Height / 10), 10, 10, true);
            EngineGlobals.Resources.LoadResources(GameGlobals.Map.Resources);
            //foreach (var physicalObject in GameGlobals.Map.GameObjects)
            //{
            //    physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
            //    if (physicalObject.TypeId == GameObjectType.Player.ToString())
            //        GameGlobals.Player = (Player)physicalObject;
            //}
            //foreach (var backgroundObject in GameGlobals.Map.BackgroundObjects)
            //{
            //    backgroundObject.Load(backgroundObject.ResourceId, backgroundObject.ResourceVariation);
            //    Controller.AddBackgroundObject(backgroundObject.Mask);
            //}
            EngineGlobals.MapIsLoading = false;
            return GameGlobals.Map;
            //EngineGlobals.MapIsLoading = true;
            //GameGlobals.Map = EngineGlobals.Content.Load<Map>(fname);
            //EngineGlobals.Resources.LoadResources(GameGlobals.Map.Resources);
            //foreach (var region in GameGlobals.Map.Regions)
            //{
            //    Controller.AddDynamicObject(region);

            //    foreach (var gameObject in map.GameObjects)
            //    {
            //        if (gameObject.Name == region.CheckedObjectName)
            //        {
            //            region.Object = gameObject;
            //        }
            //    }
            //}

            ////foreach (var physicalObject in GameGlobals.Map.GameObjects)
            ////{
            ////    physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
            ////    Controller.AddObject(physicalObject.Mask);
            ////    if (physicalObject.TypeId == GameObjectType.Player.ToString())
            ////        GameGlobals.Player = (Player) physicalObject;
            ////}
            ////foreach (var backgroundObject in GameGlobals.Map.BackgroundObjects)
            ////{
            ////    backgroundObject.Load(backgroundObject.ResourceId, backgroundObject.ResourceVariation);
            ////    Controller.AddBackgroundObject(backgroundObject.Mask);
            ////}
            //EngineGlobals.MapIsLoading = false;
            //EngineGlobals.TriggerManager = new TriggerManager(map.Triggers);

            //return GameGlobals.Map;
        }
#endif

        public object Clone()
        {
            var gameObjects = new List<PhysicalObject>();
            foreach (var physicalObject in GameObjects)
            {
                gameObjects.Add((PhysicalObject)physicalObject.Clone());
            }

            var backgroundObjects = new List<PhysicalObject>();
            foreach (var backgroundObject in BackgroundObjects)
            {
                backgroundObjects.Add((PhysicalObject)backgroundObject.Clone());
            }

            var regions = new List<Region>();
            foreach (var region in Regions)
            {
                regions.Add((Region)region.Clone());
            }

            var triggers = new List<Trigger>();
            foreach (var trigger in Triggers)
            {
                triggers.Add((Trigger)trigger.Clone());
            }

            var clone = (Map) MemberwiseClone();
            clone.GameObjects = gameObjects;
            clone.BackgroundObjects = backgroundObjects;
            clone.Regions = regions;
            clone.Triggers = triggers;
            clone.Resources = (MapResources)Resources.Clone();
            return clone;
        }

        public void Dispose()
        {
            GameObjects.Clear();

            BackgroundObjects.Clear();

            Regions.Clear();

            Resources.Dispose();
        }
    }

}
