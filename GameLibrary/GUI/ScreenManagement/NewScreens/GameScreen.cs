using System;
#if EDITOR
using System.Windows.Forms.VisualStyles;
#endif
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using GameLibrary.GridBuilder;
using GameLibrary.Objects;
#if WINDOWS_PHONE
using Microsoft.Devices;
#endif
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class GameScreen : BaseMainScreen
    {
        protected string _mapName;
        protected Map _baseMap;
        protected Image _scoreBackground;
        protected bool _queueNextLevel;
        protected bool _queueMainMenu;
        protected bool _queueReset;
        protected bool _queueGameOver;
       // protected TextRegion _gameOverText;
        public bool OutOfInk;
        protected Timer _outOfInkTimer;
       // protected TextRegion _outOfInkText;
        protected RotateEffect _playerRotateEffect;
        protected MoveEffect _playerMoveEffect;
        protected ResizeEffect _playerResizeEffect;
        protected int _currentLevel;
        protected TileType _mapType;
       // protected Image _gameOver;
       // protected Image _gameOverOutOfInk;
        

        public GameScreen(int currentLevel, TileType? type)
        {
            if (type.HasValue)
            {
                _mapType = type.Value;
            }

            if (currentLevel == -1)
            {
                _mapName = GameGlobals.MapList.ArcadeMap;
            }
            else if (type == TileType.Normal)
            {
                _mapName = GameGlobals.MapList.NormalMaps[currentLevel];
            }
            else if (type == TileType.Hardcore)
            {
                _mapName = GameGlobals.MapList.GameMaps[currentLevel];
            }
            else
            {
                throw new Exception("Unknown map type!");
            }

            _currentLevel = currentLevel;
        }

        #region Init
        
        public override void Load()
        {
            base.Load();
           // _gameOver = new Image(new GameTexture("Gui\\GameOver")) { StaticPosition = true };
           // _gameOverOutOfInk = new Image(new GameTexture("Gui\\GameOverOutOfInk")) { StaticPosition = true };
            /*_gameOverText = new TextRegion(new Rectangle(250, 224, 300, 25), GameGlobals.MenuGlobals.MenuFont,
                Color.Black, "GAME OVER", false) { LayerDepth = 0.05f, Owner = this, HorizontaAlign = FontHorizontalAlign.Center };
            _outOfInkText = new TextRegion(new Rectangle(250, 230, 300, 25), GameGlobals.MenuGlobals.MenuFont, Color.Black, "Out of ink", false) { LayerDepth = 0.05f, Owner = this, HorizontaAlign = FontHorizontalAlign.Center };*/
            GameGlobals.CreditsData.Reset();
            _baseMap = Map.Load(_mapName, AssetsManager);

        }

        public override void Initialize()
        {
            base.Initialize();

            FadeInTime = 300;
            FadeOutTime = 300;
            GameGlobals.Game = this;
            GameGlobals.LevelComplete = false;

            OutOfInk = false;
            _queueGameOver = false;
            _queueMainMenu = false;
            _queueNextLevel = false;
            _queueReset = false;
            
            _outOfInkTimer = new Timer(true);
            SetupData();
            InitBackground(); 

            #if WINDOWS_PHONE
            var orientation = false;
            if (_currentLevel == -1)
            {
                orientation = false;
            }
            else if (_mapType == TileType.Normal)
            {
                orientation = GameGlobals.MapList.AdOrientationTopNornal[_currentLevel];
            }
            else if (_mapType == TileType.Hardcore)
            {
                orientation = GameGlobals.MapList.AdOrientationTop[_currentLevel];
            }

            if (_currentLevel != -1 && orientation)
            {
                GameGlobals.AdControl.Setup(new Vector2(400, 25), Orientation.Top);
            }
            else
            {
                GameGlobals.AdControl.Setup(new Vector2(400, 455), Orientation.Bottom);
            }
            Controller.AddObject(GameGlobals.AdControl);
            #endif

            FadeIn();
        }

        public virtual void SetupData()
        {
            CloneMap();
            InitSound();
            GameGlobals.GameOver = false;

            EngineGlobals.Resources.LoadResources(GameGlobals.Map.Resources);
            EngineGlobals.TriggerManager = new TriggerManager(GameGlobals.Map.Triggers, GameGlobals.Map.GameObjects);
            GameGlobals.Physics = new PhysicsManager();
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


            GameGlobals.Physics.CommitQueue();

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

            WindowOnInput += OnWindowOnInput;
            WindowOnBack += OnWindowOnBack;
        }

        private void OnWindowOnBack(object sender)
        {
            if (!GameGlobals.GameOver)
                EngineGlobals.ScreenManager.QueueShowPopup(new PauseScreen());
            else
            {
                GameOverInput();
            }
        }

        public void CloneMap()
        {
            GameGlobals.Map = (Map)(_baseMap.Clone());
        }

        public void InitSound()
        {
            SoundManager.GlobalSoundVolume = 1.0f;
            MusicManager.GlobalMusicVolume = GameGlobals.Settings.MusicVolume / 100f;
        }

        public void SetCamera()
        {
            EngineGlobals.Camera2D.SetCamerBounds(new Rectangle(
                   0,
                   0,
                   GameGlobals.Map.Width,
                   GameGlobals.Map.Height));
            EngineGlobals.Camera2D.BackgroundOffset = new Vector2(350f, 210f);
            EngineGlobals.Camera2D.Zoom = 0.9f;
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
             _scoreBackground= new Image(EngineGlobals.Resources.Textures["ScoreBack"][0], new Rectangle(55, 5, 85, 40)) {StaticPosition = true, LayerDepth = 0.12f, Owner = this};
             Controller.AddObject(_scoreBackground);

            GameGlobals.ScoreRegion = new TextRegion(
                new Rectangle(70, 5, 100, 40),
                GameGlobals.MenuGlobals.MenuFont,
                Color.Black,
                "0",
                false) {LayerDepth = 0.11f};

            Controller.AddObject(GameGlobals.ScoreRegion);
        }

        public void AddPhysicalObjects()
        {
            GameGlobals.CreditsData.TotalInk = 0;
            foreach (var physicalObject in GameGlobals.Map.GameObjects)
            {
                physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
                if (physicalObject.TypeId == GameObjectType.Player.ToString())
                {
                    GameGlobals.Player = (Player)physicalObject;
                }
                else if (physicalObject.TypeId == GameObjectType.InkDot.ToString())
                {
                    GameGlobals.CreditsData.TotalInk++;
                }
                Controller.AddGameObject(physicalObject);
            }
        }

        public void InitBackground()
        {
            if (!string.IsNullOrEmpty(GameGlobals.Map.Background))
            {
                EngineGlobals.Background = new BackgroundManager(GameGlobals.Map.Width, GameGlobals.Map.Height);
                EngineGlobals.Background.LoadBackground(new GameTexture(GameGlobals.Map.Background));
            }
        }

        #endregion

        protected virtual void GameOverInput()
        {
            if (OutOfInk)
                return;
            Reset();
            FadeIn();
        }

        private void OnWindowOnInput(object sender, Point point)
        {
#if WINDOWS_PHONE
            if (GameGlobals.AdControl.OnPress(point))
            {
                return;
            }
#endif

            if (GameGlobals.Player == null)
                return;
           /* if (GameGlobals.GameOver)
            {
                GameOverInput();
            }*/
            if (!GameGlobals.GameOver)
            {
                if (GameGlobals.Player.CurrentPowerUp != PowerUpType.None &&
                    GameGlobals.Player.CurrentPowerUp != PowerUpType.DoubleJump && !GameGlobals.ArcadeMode)
                {
                    GameGlobals.Player.UsePowerUp(point);
                }
                else
                {
                    GameGlobals.Player.Jump(point);
                }
            }
        }

        public virtual void Reset()
        {
            CleanData();
            EngineGlobals.Camera2D.Reset();
            EngineGlobals.TimeSpeed = 1f;
            GameGlobals.Score = 0;
            GameGlobals.MaxHeight = 0;
            EngineGlobals.Gravity = EngineGlobals.DefaultGravity;
            Initialize();
            GameGlobals.GameOver = false;
            GameGlobals.LevelComplete = false;
            Freez = false;
            GameGlobals.CreditsData.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Freez)
                return;
            EngineGlobals.ParticleStorageManager.Update();
            if (GameGlobals.GameOver)
            {
                if (OutOfInk && !_queueGameOver)
                {
                    if (_outOfInkTimer.Ticking)
                    {
                        _outOfInkTimer.Update();
                        if (_outOfInkTimer.Finished)
                            QueueGameOver();
                    }
                    else
                        _outOfInkTimer.Start(1000);
                }
                else if (GameGlobals.LevelComplete)
                {
                    _playerMoveEffect.Update();
                    _playerResizeEffect.Update();
                    _playerRotateEffect.Update();
                }
                return;
            }
            GameGlobals.CreditsData.GameTime += (long)gameTime.ElapsedGameTime.TotalMilliseconds;
            GameGlobals.TotalTimePlayed =
                GameGlobals.TotalTimePlayed.Add(TimeSpan.FromMilliseconds(gameTime.ElapsedGameTime.TotalMilliseconds));
            GameGlobals.Physics.Update();
        }

        public void CleanData()
        {
            Controller.RemoveObject(GameGlobals.HealthBar.Border);
            Controller.RemoveObject(GameGlobals.HealthBar.Bar);
            Controller.RemoveObject(GameGlobals.ScoreRegion);
            Controller.RemoveObject(_scoreBackground);
            GameGlobals.Physics.Dispose();
            GameGlobals.Physics.Dispose();
            EngineGlobals.TriggerManager.Dispose();
            EngineGlobals.TriggerManager = null;
            WindowOnInput -= OnWindowOnInput;
            WindowOnBack -= OnWindowOnBack;
            
            EngineGlobals.TimeSpeed = 1f;
            GameGlobals.ScoreRegion = null;
            Controller.ClearObjectsBuffers();
            GameGlobals.Map.Dispose();
            GameGlobals.Map = null;
        }

        public override void Dispose()
        {
            base.Dispose();
            GameGlobals.Game = null;
            EngineGlobals.TimeSpeed = 1.0f;
            CleanData();
            if (EngineGlobals.Background != null)
            {
                EngineGlobals.Background.Dispose();
                EngineGlobals.Background = null;
            }

        }

        protected virtual void DoGameOver()
        {
            if (OutOfInk)
            {
                //_gameOverOutOfInk.Rect = new Rectangle(0, 0, 800, 480);
                //Controller.AddObject(_gameOverOutOfInk);
                //_gameOverText.Rect = new Rectangle(_gameOverText.Rect.X, 200, _gameOverText.Rect.Width, _gameOverText.Rect.Height);
               // Controller.AddObject(_outOfInkText);
            }
            else
            {
               // _gameOver.Rect = new Rectangle(0, 0, 800, 480);
               // Controller.AddObject(_gameOver);
               // _gameOverText.Rect = new Rectangle(_gameOverText.Rect.X, 230, _gameOverText.Rect.Width, _gameOverText.Rect.Height);
            }
            OutOfInk = false;
           // Controller.AddObject(_gameOverText);
        }

        protected override void FadeOutCompleted()
        {
            base.FadeOutCompleted();
            if (_queueNextLevel)
            {
                if (_mapType == TileType.Normal)
                {
                    EngineGlobals.ScreenManager.QueueChangeScreen(
                    new LevelCompleteScreen(GameGlobals.MapList.NormalMaps.IndexOf(_mapName), _mapType));
                }
                else
                {
                    EngineGlobals.ScreenManager.QueueChangeScreen(
                       new LevelCompleteScreen(GameGlobals.MapList.GameMaps.IndexOf(_mapName), _mapType));   
                }
            }
            else if (_queueMainMenu)
            {
                EngineGlobals.ScreenManager.QueueChangeScreen(new BaseLoadingScreen(new MainMenuScreen()));
            }
            else if (_queueReset)
            {
                Reset();
                FadeIn();
            }
            else if (_queueGameOver)
            {
                if (!GameGlobals.ArcadeMode)
                {
                    Reset();
                    FadeIn();
                }
                else
                 DoGameOver();
            }
            else
            {
                throw new Exception("Random fadeout?");
            }
        }

        public void NextLevel(LevelEnd levelEnd)
        {
            _queueNextLevel = true;
            GameGlobals.InkLeft = GameGlobals.Player.CurrentMovePoints;
            if (levelEnd != null)
            {
                _playerMoveEffect = new MoveEffect(GameGlobals.Player.Mask, levelEnd.HalfPos, 1000);
                _playerResizeEffect = new ResizeEffect(GameGlobals.Player.Mask, new Vector2(20, 16), 1000);
                var dist = levelEnd.HalfPos - GameGlobals.Player.HalfPos;
                _playerRotateEffect = new RotateEffect(GameGlobals.Player.Mask,
                    (float) Math.Atan2(dist.Y, dist.X) - MathHelper.PiOver2, 500);
            }
            FadeOut();
        }

        public void QueueReset()
        {
            _queueReset = true;
            FadeOut();
        }

        public void QueueGameOver()
        {
            _queueGameOver = true;
            Freez = true;
            GameGlobals.GameOver = true;
            FadeOut();
        }

        public void ExitToMainMenu()
        {
            GameGlobals.HardCorePlayLevel = false;
            GameGlobals.Score = 0;
            GameGlobals.MaxHeight = 0;
            SaveData.UpdateStatistics();
            SaveData.Save();

            _queueMainMenu = true;
            FadeOut();
        }

    }
}
