using System;
using System.Collections.Generic;
using System.Xml;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using Engine.ScreenManagement;
using GameLibrary.Gui;
//using GameLibrary.Gui.ScreenManagement.Screens;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary
{
    public class MenuManager : IGameManager, IDisposable
    {
        protected int _numOfFrames;
        protected double _fps;
        protected TimeSpan _elapsedTime = TimeSpan.Zero;
        protected bool _queueLoadArcade;
        protected bool _queueLoadMap;
        protected int _mapToLoad;
        //protected MapMatrix _mapMatrix;
        protected ContentManager _assetsManager;

        public event SimpleEvent Exit;

        public void Initialize()
        {

        }

        public void Load()
        {
            
            LoadContent();

            SetUpData();
        }

        public virtual void LoadContent()
        {
            if (GameGlobals.Storage == null)
            {
                GameGlobals.Storage = new StorageControl();
            }

            if (GameGlobals.MapList == null)
            {
                GameGlobals.MapList = _assetsManager.Load<MapList>("MapList");
            }

            if (GameGlobals.Settings == null)
            {
                Settings.Load();
            }
            if (GameGlobals.SaveData == null)
            {
                SaveData.Load();
            }
            GameGlobals.Map = Map.Load(GameGlobals.MapList.MenuMap, _assetsManager);
            var maps = new List<int>();
            for (int i = 0; i < 15; i++)
            {
                maps.Add(i);
            }
            // EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_Button_next", new[] { "GameObjects\\Images\\Level_button_next" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_Button_locked", new[] { "GameObjects\\Images\\Level_button_locked" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_Button_completed", new[] { "GameObjects\\Images\\Level_button_completed" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_completed_splash", new[] { "GameObjects\\Images\\Level_completed_splash" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_completed_splash_red", new[] { "GameObjects\\Images\\Level_completed_splash_red" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("LevelBtnGear", new[] { "GUI/Menu/LevelBtnGearWhite", "GUI/Menu/LevelBtnGearBlack" }, ResourceType.Texture));
            //_mapMatrix = new MapMatrix(5, 3, maps, 80, 74, new Rectangle(17640,8150, 500, 260));
        }

        public void InitSound()
        {
            SoundManager.GlobalSoundVolume = 0.7f;
            MusicManager.GlobalMusicVolume = 0.0f;
        }

        public void InitInput()
        {
            EngineGlobals.Input.OnPress += InputOnPress;
        }

        public void InitBackground()
        {
            if (!string.IsNullOrEmpty(GameGlobals.Map.Background))
            {
                EngineGlobals.Background = new BackgroundManager(GameGlobals.Map.Width, GameGlobals.Map.Height);
                EngineGlobals.Background.LoadBackground(new GameTexture(GameGlobals.Map.Background));
            }
        }

        protected void SetUpData()
        {
            InitInput();
            InitSound();

            EngineGlobals.Resources.LoadResources(GameGlobals.Map.Resources);
            EngineGlobals.TriggerManager = new TriggerManager(GameGlobals.Map.Triggers, GameGlobals.Map.GameObjects);
            
            foreach (var physicalObject in GameGlobals.Map.GameObjects)
            {
                physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
                Controller.AddGameObject(physicalObject);
            }
            EngineGlobals.Camera2D = new Camera2D();
            EngineGlobals.SoundManager = new SoundManager();

            SetCamera();

            EngineGlobals.TriggerManager.ActionOccured((int)EventType.GameInitialization, null);

            InitBackground();
        }

/*        public virtual void DispoeMap()
        {
            GameGlobals.Physics.Dispose();
            GameGlobals.Map.Dispose();
            GameGlobals.Map = null;
            GameGlobals.Score = 0;
            GameGlobals.ScoreRegion = null;
            Controller.ClearObjectsBuffers();
            EngineGlobals.SoundManager.Dispose();
        }*/


        public void StartArcade()
        {
            if (GameGlobals.EditorMode)
                return;
            _queueLoadArcade = true;
        }

        public void SetCamera()
        {
            EngineGlobals.Camera2D.SetCamerBounds(new Rectangle(
                   0,
                   0,
                   GameGlobals.Map.Width,
                   GameGlobals.Map.Height));
            EngineGlobals.Camera2D.BackgroundOffset = new Vector2(350f, 210f);
        }

        //public virtual void AddObjectsToDelete(PhysicalObject gameObject)
        //{
        //    //_objectToDelete.Add(gameObject);
        //}


        public void InputOnPress(Point point, int inputId)
        {
            var p = EngineGlobals.Camera2D.ViewToWorld(point.ToVector()).ToPoint();
            foreach (var physicalObject in Controller.GetGameObjects())
            {
                if (physicalObject.Rectangle.Contains(p))
                {
                    EngineGlobals.TriggerManager.ActionOccured((int) EventType.ObjectClicked,
                                                               new EventParams {Location = point, TriggeringObject = physicalObject});
#if WINDOWS
                    EngineGlobals.SoundManager.Play("click", "click", physicalObject, 1f, false, 0f, 0f);
#endif
                    if (physicalObject is LevelButton && physicalObject.IsActivated)
                    {
                        _queueLoadMap = true;
                        _mapToLoad = (physicalObject as LevelButton).LevelId;
                    }
                }
               
            }
        }

        public void Update(GameTime gameTime)
        {
            EngineGlobals.Camera2D.Update();


            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _fps = _numOfFrames;
                _numOfFrames = 0;
            }

            EngineGlobals.SoundManager.Update(EngineGlobals.Camera2D.Position);
            //if (_queueLoadArcade)
            //    LoadingScreen.Load(true, false, new ArcadeScreen());
            //if (_queueLoadMap)
            //    LoadingScreen.Load(true, false, new GameplayScreen(GameGlobals.MapList.GameMaps[_mapToLoad]));

        }

        public void Draw()
        {
            _numOfFrames++;
            Controller.Draw();
        }

        public void Dispose()
        {
            EngineGlobals.Input.OnPress -= InputOnPress;
            Controller.ClearObjectsBuffers();
            //EngineGlobals.Resources.DisposeResources(GameGlobals.Map.Resources);
            
            GameGlobals.Physics = null;
            EngineGlobals.TriggerManager.Dispose();
            EngineGlobals.TriggerManager = null;
            EngineGlobals.SoundManager.Dispose();
            MusicManager.Stop();
            EngineGlobals.SoundManager = null;
            EngineGlobals.Camera2D = null;
            GameGlobals.Map = null;
            _assetsManager.Dispose();
        }
    }
}
