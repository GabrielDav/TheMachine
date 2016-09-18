using System;
using System.Globalization;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using GameLibrary.Gui;
//using GameLibrary.Gui.ScreenManagement.Screens;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary
{
    public enum GameObjectType
    {
        Player,
        Plane,
        Circle,
        Spike,
        SpikeShooter,
        RedBall,
        Water,
        InkDot,
        JumpSpot,
        BackgroundGear,
        BackgroundGearSmall,
        Wall,
        WallHand,
        Saw,
        Tile,
        DeathBall,
        DecorativeObject,
        MenuObject,
        MenuLine,
        MenuDec,
        MenuGear,
        MenuBtnPointer,
        MenuBtnMap,
        LevelEnd,
        TrapBtn,
        CircleSpikes,
        CameraPath,
        TrapDoor,
        SlidePlane,
        DeathPlane,
        MovingCircle,
        PowerUp,
        SeekerDot,
        ScoreDisplayDevice,
        VersionInfo,
        SwitchBtn,
        Hint,
        ResizableDecorativeObject,
        SpikeSmall
    }

    public class GameManager
    {
        protected int _numOfFrames;
        protected double _fps;
        protected TimeSpan _elapsedTime = TimeSpan.Zero;
        protected Map _baseMap;
        public bool QueueShowEndScreen;
        protected string _mapName;
        public bool Inactive;
        protected Image _scoreBackground;
        protected ContentManager _assetsManager;
        //private static List<PhysicalObject> _objectToDelete;

        public event SimpleEvent Exit;

        public void Load(string mapName)
        {
            LoadContent(mapName);
            _mapName = mapName;
            SetUpData();
        }

        public virtual void LoadContent(string mapName)
        {
            _assetsManager = new ContentManager(EngineGlobals.ContentCache.ServiceProvider, EngineGlobals.ContentCache.RootDirectory);
            _baseMap = Map.Load(mapName, _assetsManager);
        }

        public void Initialize()
        {
            GameGlobals.Physics = new PhysicsManager();
            EngineGlobals.Camera2D = new Camera2D();
            EngineGlobals.SoundManager = new SoundManager();
        }

        public void InitSound()
        {
            SoundManager.GlobalSoundVolume = 0.7f;
            MusicManager.GlobalMusicVolume = 0.0f;
        }

        public void AddPhysicalObjects()
        {
            foreach (var physicalObject in GameGlobals.Map.GameObjects)
            {
                physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
                if (physicalObject.TypeId == GameObjectType.Player.ToString())
                {
                    GameGlobals.Player = (Player)physicalObject;
                }
                Controller.AddGameObject(physicalObject);
            }
        }

        public void InitInput()
        {
            EngineGlobals.Input.OnPress += InputOnPress;
        }

        public void InitBackground()
        {
            /*if (!string.IsNullOrEmpty(GameGlobals.Map.Background))
            {
                GameGlobals.Background = new BackgroundManager();
                GameGlobals.Background.LoadBackground(new GameTexture(GameGlobals.Map.Background));
                Controller.Background = GameGlobals.Background;
            }*/
        }

        public void CloneMap()
        {
            GameGlobals.Map = (Map)(_baseMap.Clone());
        }

        public virtual void SetUpData()
        {
            CloneMap();
            InitInput();
            InitSound();

            //_objectToDelete = new List<PhysicalObject>();

            EngineGlobals.Resources.LoadResources(GameGlobals.Map.Resources);
            EngineGlobals.TriggerManager = new TriggerManager(GameGlobals.Map.Triggers, GameGlobals.Map.GameObjects);
            GameGlobals.Physics.LoadMapObjects(GameGlobals.Map.GameObjects);
            
            AddPhysicalObjects();

            foreach (var backgroundObject in GameGlobals.Map.BackgroundObjects)
            {
                backgroundObject.Load(backgroundObject.ResourceId, backgroundObject.ResourceVariation);
                Controller.AddBackgroundObject(backgroundObject);
            }

            foreach (var region in GameGlobals.Map.Regions)
            {
                Controller.AddDynamicObject(region);
                foreach (var gameObject in GameGlobals.Map.GameObjects)
                {
                    if (gameObject.Name == region.CheckedObjectName)
                    {
                        region.Object = gameObject;
                    }
                }
            }

            InitBackground();
            
            GameGlobals.Physics.CommitQueue();

            #if EDITOR
            InitGameMessages();
            #endif

            SetCamera();

            InitHealthBar();

            InitScore();
            if (GameGlobals.Map.MapType == MapType.Game)
            {
                GameGlobals.Player.StartPosition = GameGlobals.Player.HalfPos;
                GameGlobals.Physics.PushObject(GameGlobals.Player, new Vector2(0, 0));
            }

            EngineGlobals.ParticleStorageManager = new ParticleStorageManager();

            EngineGlobals.TriggerManager.ActionOccured((int)EventType.GameInitialization, null);
        }

        public virtual void Reset()
        {
            DispoeMap();
            EngineGlobals.Camera2D.Reset();
            EngineGlobals.TimeSpeed = 1f;
            SetUpData();
            GameGlobals.GameOver = false;
        }

        public virtual void DispoeMap()
        {
            GameGlobals.Physics.Dispose();
            GameGlobals.Map.Dispose();
            GameGlobals.Map = null;
            GameGlobals.Score = 0;
            GameGlobals.MaxHeight = 0;
            GameGlobals.ScoreRegion = null;
            Controller.ClearObjectsBuffers();
            EngineGlobals.SoundManager.Dispose();
            MusicManager.Stop();
        }

        

        public virtual void InitGameMessages()
        {
            /*
            GameGlobals.GameMessages = new GameMessages(GameGlobals.Font);
            GameGlobals.GameMessages.RegisterRecord("fps", "FPS", "0", true);
            GameGlobals.GameMessages.RegisterRecord("zoom", "Zoom", "0", true);
            //GameGlobals.GameMessages.RegisterRecord("rotation", "Rotation", "0", false);
            GameGlobals.GameMessages.RegisterRecord("pos", "Pos", "0", true);
            GameGlobals.GameMessages.RegisterRecord("camPos", "Cam", "0", true);
            GameGlobals.GameMessages.RegisterRecord("py", "PY", "0", true);
            GameGlobals.GameMessages.RegisterRecord("ty", "TY", "0", true);
            GameGlobals.GameMessages.RegisterRecord("fy", "FY", "0", true);
            GameGlobals.GameMessages.RegisterRecord("tgName", "Name", "-", true);
            //GameGlobals.GameMessages.RegisterRecord("direction", "Dir", "0", false);
            //GameGlobals.GameMessages.RegisterRecord("particles", "Particles", "0", true);
            //GameGlobals.GameMessages.RegisterRecord("draw", "Drawn objects", "0", true);
            //GameGlobals.GameMessages.RegisterRecord("update", "Updated objects", "0", true);
            //GameGlobals.GameMessages.RegisterRecord("drawBg", "Drawn bg objects", "0", true);
            GameGlobals.GameMessages.RegisterRecord("diagonal", "Diagonal", "false", true);
            GameGlobals.GameMessages.RegisterRecord("v", "V", "-", true);
            GameGlobals.GameMessages.RegisterRecord("h", "H", "-", true);
            GameGlobals.GameMessages.InitInfoRegion();
            Controller.AddObject(GameGlobals.GameMessages);
             */
        }

        public void SetCamera()
        {
            EngineGlobals.Camera2D.SetCamerBounds(new Rectangle(
                   0,
                   0,
                   GameGlobals.Map.Width,
                   GameGlobals.Map.Height));
            EngineGlobals.Camera2D.BackgroundOffset = new Vector2(350f, 210f);
            EngineGlobals.Camera2D.Zoom = 1;
            if (GameGlobals.Map.MapType == MapType.Game)
            {
                EngineGlobals.Camera2D.FollowObject(GameGlobals.Player);
                EngineGlobals.Camera2D.Position = GameGlobals.Player.HalfPos;
            }
        }

        public void InitHealthBar()
        {
            GameGlobals.HealthBar = new HealthBar("HealthBar", "HealthBarBorder");
            Controller.AddObject(GameGlobals.HealthBar.Border);
            Controller.AddObject(GameGlobals.HealthBar.Bar);
        }

        public void InitScore()
        {
            _scoreBackground = new Image(EngineGlobals.Resources.Textures["ScoreBack"][0], new Rectangle(55, 5, 85, 40)) {StaticPosition = true, Owner = this};
            Controller.AddObject(_scoreBackground);

            GameGlobals.ScoreRegion = new TextRegion(
                new Rectangle(70, 5, 100, 40),
                GameGlobals.MenuGlobals.MenuFont,
                Color.Black,
                "0",
                false);

            Controller.AddObject(GameGlobals.ScoreRegion);
        }

        //public virtual void AddObjectsToDelete(PhysicalObject gameObject)
        //{
        //    //_objectToDelete.Add(gameObject);
        //}

        public void DisposeGameData()
        {
            GameGlobals.ScoreRegion = null;
            Controller.ClearObjectsBuffers();
#if !EDITOR
          //  EngineGlobals.Resources.DisposeResources(GameGlobals.Map.Resources);
#endif
            GameGlobals.Map.Dispose();
            GameGlobals.Map = null;
            if (_assetsManager != null)
                _assetsManager.Dispose();
            GameGlobals.Physics.Dispose();
            GameGlobals.Physics = null;
            EngineGlobals.Input.OnPress -= InputOnPress;
            EngineGlobals.ParticleStorageManager.Dispose();
            EngineGlobals.ParticleStorageManager = null;
            EngineGlobals.TriggerManager.Dispose();
            EngineGlobals.TriggerManager = null;
            EngineGlobals.SoundManager.Dispose();
            MusicManager.Stop();
            EngineGlobals.SoundManager = null;
            EngineGlobals.Camera2D = null;
            GameGlobals.Player = null;
        }

        protected virtual void GameOverInput()
        {
            if (!GameGlobals.LevelComplete)
            {
                Reset();
            }
        }

        public void InputOnPress(Point point, int inputId)
        {
            if (Inactive)
                return;
            if (GameGlobals.Player == null)
                return;
            if (GameGlobals.GameOver)
            {
                GameOverInput();
            }
            else
            {
                //if (Vector2.Distance(GameGlobals.Player.HalfPos, EngineGlobals.Camera2D.ViewToWorld(point.ToVector())) < GameGlobals.Player.HalfSize.X * 2)
                if (GameGlobals.Player.CurrentPowerUp != PowerUpType.None && GameGlobals.Player.CurrentPowerUp != PowerUpType.DoubleJump && GameGlobals.Player.CurrentPowerUp != PowerUpType.Shield)
                {
                    GameGlobals.Player.UsePowerUp(point);
                }
                else
                    GameGlobals.Player.Jump(point);
            }
        }

        public void OnExit()
        {
            DisposeGameData();
            SimpleEvent handler = Exit;
            if (handler != null) handler(this);
        }

        public void Update(GameTime gameTime)
        {
            EngineGlobals.GameTime = gameTime;

            //foreach (var physicalObject in _objectToDelete)
            //{
            //    GameGlobals.RemoveObject(physicalObject);
            //}

            //_objectToDelete.Clear();


            if (!GameGlobals.GameOver)
            {
                GameGlobals.Physics.Update();

                
            }

            EngineGlobals.Camera2D.Update();

            EngineGlobals.ParticleStorageManager.Update();

            Controller.Update();

            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _fps = _numOfFrames;
                _numOfFrames = 0;
            }

            #if EDITOR
            //UpdateMassages();
            #endif

            EngineGlobals.SoundManager.Update(EngineGlobals.Camera2D.Position);
            if (QueueShowEndScreen)
            {
                QueueShowEndScreen = false;
                //LoadingScreen.Load(true, false, new LevelCompleteScreen(GameGlobals.MapList.GameMaps.IndexOf(_mapName)));
            }

        }

        protected virtual void UpdateMassages()
        {
            GameGlobals.GameMessages.UpdateInfoText(
                "fps",
                _fps.ToString(CultureInfo.InvariantCulture));
            GameGlobals.GameMessages.UpdateInfoText(
                "zoom",
                EngineGlobals.Camera2D.Zoom.ToString(CultureInfo.InvariantCulture));
            //GameGlobals.GameMessages.UpdateInfoText("rotation",
            //                                        MathHelper.ToDegrees(GameGlobals.Player.Mask.Rotation).ToString(
            //                                            "0.##"));
            //GameGlobals.GameMessages.UpdateInfoText("pos", GameGlobals.Player.HalfPos.Y.ToString("0.#"));
            GameGlobals.GameMessages.UpdateInfoText("camPos", EngineGlobals.Camera2D.CameraRectangle.Y.ToString("0.#"));
            //GameGlobals.GameMessages.UpdateInfoText("py", EngineGlobals.Debug.PlayerY.ToString());
            //GameGlobals.GameMessages.UpdateInfoText("ty", EngineGlobals.Debug.TargetY.ToString());
            //GameGlobals.GameMessages.UpdateInfoText("fy", EngineGlobals.Debug.FixedY.ToString());
            //GameGlobals.GameMessages.UpdateInfoText("tgName", EngineGlobals.Debug.TargetName);
            //GameGlobals.GameMessages.UpdateInfoText("direction", GameGlobals.Player.Direction.ToString());
            //GameGlobals.GameMessages.UpdateInfoText(
            //    "draw",
            //    Controller.DrawnObjectsCount.ToString(CultureInfo.InvariantCulture));
            //GameGlobals.GameMessages.UpdateInfoText(
            //    "update",
            //    Controller.UpdatedObjectCount.ToString(CultureInfo.InvariantCulture));
            //GameGlobals.GameMessages.UpdateInfoText(
            //    "particles",
            //    EngineGlobals.ParticleStorageManager
            //        .ParticlesPool.Count.ToString(CultureInfo.InvariantCulture));
           /* GameGlobals.GameMessages.UpdateInfoText(
                "diagonal",
                Player.Diagonal.ToString());
            GameGlobals.GameMessages.UpdateInfoText(
                "h",
                Player.H.ToString(CultureInfo.InvariantCulture));
            GameGlobals.GameMessages.UpdateInfoText(
                "v",
                Player.V.ToString(CultureInfo.InvariantCulture));*/
        }

        public virtual void Draw()
        {
            if (Inactive)
                return;
            _numOfFrames++;

            Controller.Draw();
        }
    }
}
