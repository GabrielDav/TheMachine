using System;
using System.IO;
using System.Xml;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using GameLibrary.GridBuilder;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class MainMenuScreen : BaseMainScreen
    {
        //protected MapMatrix _mapMatrix;
        //protected MapMatrix _mapMatrix2;
        protected LinksList _linkList;
        protected bool _loadArcade;
        protected bool _loadMap;
        protected int _mapIndex;
        protected bool _firstLoad;

        private readonly MatrixBuilder _achivementsGrid;
        private readonly MatrixBuilder _hardcoreGrid;
        private readonly MatrixBuilder _normalGrid;

        private TileType _mapType;

        private const string FastLerner = "GUI\\Achievements\\FastLearner";
        private const string FirstBlood = "GUI\\Achievements\\FirstBlood";
        private const string Closecall = "GUI\\Achievements\\CloseCall";
        private const string InkCollector = "GUI\\Achievements\\InkCollector";
        private const string ClimberBronze = "GUI\\Achievements\\ClimberBronze";
        private const string ClimberSilver = "GUI\\Achievements\\ClimberSilver";
        private const string ClimverGold = "GUI\\Achievements\\ClimberGold";
        private const string Determined = "GUI\\Achievements\\Determined";
        private const string PlayerBronze = "GUI\\Achievements\\PlayerBronze";
        private const string PlayeSilver = "GUI\\Achievements\\PlayerSilver";
        private const string PlayerGold = "GUI\\Achievements\\PlayerGold";
        private const string HardCorePlay = "GUI\\Achievements\\HardCorePlay";
        private const string Efficient = "GUI\\Achievements\\Efficient";
        private const string LabyrinthMaster = "GUI\\Achievements\\LabyrinthMaster";
        private const string Professional = "GUI\\Achievements\\Professional";

        public MainMenuScreen(bool firstLoad)
        {
            FadeInTime = 200;
            FadeOutTime = 1000;
            _firstLoad = firstLoad;

            _achivementsGrid = new MatrixBuilder(new AchievementsTileBuilder(), 18470, 8800, null);
            _hardcoreGrid = new MatrixBuilder(new MapTileBuilder(), 17730, 7440, TileType.Hardcore);
            _normalGrid = new MatrixBuilder(new MapTileBuilder(), 18600, 8190, TileType.Normal);
        }

        public MainMenuScreen() : this(false)
        {

        }

        #region Initialization
        
        public override void Load()
        {
            base.Load();
            var content = Controller.CurrentGame.Content;
            GameGlobals.MenuGlobals.MenuFont = content.Load<SpriteFont>("MenuFont");
            LoadContent();
////#if WINDOWS_PHONE
////            GameGlobals.AdControl.Setup(new Vector2(640, 240), Orientation.Top);
////            Controller.AddObject(GameGlobals.AdControl);
////#endif
        }

        public override void Initialize()
        {
            base.Initialize();
            GameGlobals.Menu = this;
            SetUpData();
            FadeIn();
            WindowOnInput += OnWindowOnInput;
            WindowOnBack += OnWindowOnBack;
            GameGlobals.ArcadeMode = false;
        }

        private void OnWindowOnBack(object sender)
        {
            EngineGlobals.TriggerManager.ActionOccured((int)EventType.BackPressed, new EventParams());
        }

        private void OnWindowOnInput(object sender, Point point)
        {
            var p = EngineGlobals.Camera2D.ViewToWorld(point.ToVector()).ToPoint();
/*#if WINDOWS_PHONE
            if (GameGlobals.AdControl.OnPress(point))
                return;
#endif*/
            if (_linkList.CheckClick(p))
            {
                return;
            }
            foreach (var physicalObject in Controller.GetGameObjects())
            {
                if (physicalObject.Rectangle.Contains(p))
                {
                    EngineGlobals.TriggerManager.ActionOccured((int)EventType.ObjectClicked,
                                                               new EventParams { Location = point, TriggeringObject = physicalObject });
                    if (physicalObject is LevelButton && physicalObject.IsActivated)
                    {
                        var button = physicalObject as LevelButton;
                        _mapType = button.Type;
                        _loadMap = true;
                        _mapIndex = button.LevelId;
                        button.ShowPressAnimation();
                        FadeOut();
                    }
                    else if (physicalObject is LevelButton)
                    {
                        var button = physicalObject as LevelButton;

                        var uncompletedLevels = GetNotCompletedLevels(button.Type);
                        for (var i = 0; i < uncompletedLevels.Length; i++)
                        {
                            uncompletedLevels[i]++;
                        }
                        var str = (uncompletedLevels.Length > 1? "levels " : "level ") + string.Join(", ", uncompletedLevels);
                        if (uncompletedLevels.Length < 1)
                        {
                            EngineGlobals.ScreenManager.QueueShowPopup(new MessageBox("Level locked",
                                "You must complete no levels to unlock next five levels. Yup, your save data is broken, please contact us. Sorry for the inconvenience.",
                                true, 0));
                        }

                        EngineGlobals.ScreenManager.QueueShowPopup(new MessageBox("Level locked", string.Format("You must complete {0} to unlock next five levels.", str), true, 0));
                    }
                    else if (physicalObject is AchievementButton)
                    {
                        EngineGlobals.ScreenManager.QueueShowPopup(new AchievementScreen((Achievement)((AchievementButton)physicalObject).AchievementId));
                    }
                }

            }
        }

        private static int[] GetNotCompletedLevels(TileType type)
        {
            var levels = new List<int>();
            var lastUnlockedLevel = MapTileBuilder.GetLastUnlockedLevel(type);

            var r = new List<int>();

            if (type == TileType.Normal)
            {
                r = GameGlobals.SaveData.RatingNormal;
            }
            else if (type == TileType.Normal)
            {
                r = GameGlobals.SaveData.Rating;
            }

            for (var i = 0; i <= lastUnlockedLevel; i++)
            {
                if (r[i] == -1)
                    levels.Add(i);
            }
            return levels.ToArray();
        }

        public virtual void LoadContent()
        {
            if (GameGlobals.MapList == null)
            {
                GameGlobals.MapList = AssetsManager.Load<MapList>("MapList");
            }

            
            if (GameGlobals.SaveData == null)
            {
                SaveData.Load();
                #if WINDOWS_PHONE
                PhoneUtils.UpdateTiles();
                #endif
            }
            Map.Load(GameGlobals.MapList.MenuMap, AssetsManager);
            var maps = new List<int>();
            for (int i = 0; i < 15; i++)
            {
                maps.Add(i);
            }
            
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_Button_locked", new[] { "GameObjects\\Images\\Level_button_locked" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_Button_completed", new[] { "GameObjects\\Images\\Level_button_completed" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_completed_splash", new[] { "GameObjects\\Images\\Level_completed_splash" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Level_completed_splash_red", new[] { "GameObjects\\Images\\Level_completed_splash_red" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("LevelBtnGear", new[] { "GUI/Menu/LevelBtnGearWhite", "GUI/Menu/LevelBtnGearBlack" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Achievement",
                new[]
                {
                    FastLerner, Closecall,       InkCollector, ClimberBronze, PlayerBronze,
                    Efficient,  LabyrinthMaster, Determined,   ClimberSilver, PlayeSilver,
                    FirstBlood, Professional,    HardCorePlay, ClimverGold,   PlayerGold
                }, ResourceType.Texture));

            /*
            GameGlobals.AchievemetsPositions  = new Dictionary<int, int>(); 
            GameGlobals.AchievemetsPositions.Add(0, 0);
            GameGlobals.AchievemetsPositions.Add(1, 1);
            GameGlobals.AchievemetsPositions.Add(2, 2);
            GameGlobals.AchievemetsPositions.Add(3, 3);
            GameGlobals.AchievemetsPositions.Add(4, 4);
            GameGlobals.AchievemetsPositions.Add(5, 5);
            GameGlobals.AchievemetsPositions.Add(7, 7);
            GameGlobals.AchievemetsPositions.Add(8, 8);
            GameGlobals.AchievemetsPositions.Add(9, 9);
            GameGlobals.AchievemetsPositions.Add(10, 10);
            GameGlobals.AchievemetsPositions.Add(11, 11);
            GameGlobals.AchievemetsPositions.Add(12, 12);
            GameGlobals.AchievemetsPositions.Add(13, 13);
            GameGlobals.AchievemetsPositions.Add(14, 14);
             */


            _normalGrid.BuildGrid(5, 3, 64, 64, 40, 40, 15, 500);
            _hardcoreGrid.BuildGrid(5, 3, 64, 64, 40, 40, 30, 875);
            _achivementsGrid.BuildGrid(5, 3, 70, 70, 10, 10, 15, 500);

            CreateStatistics(new Rectangle(18890, 8790, 360, 500), false);
            _linkList = new LinksList(new Point(16830, 8770));
        }

        protected void SetUpData()
        {
            InitSound();

            EngineGlobals.Resources.LoadResources(GameGlobals.Map.Resources);
            EngineGlobals.TriggerManager = new TriggerManager(GameGlobals.Map.Triggers, GameGlobals.Map.GameObjects);

            foreach (var physicalObject in GameGlobals.Map.GameObjects)
            {
                physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
                Controller.AddGameObject(physicalObject);
            }

            SetCamera();

            EngineGlobals.TriggerManager.ActionOccured((int)EventType.GameInitialization, null);

            InitBackground();
        }

        public  static void CreateStatistics(Rectangle rectangle, bool staticPos)
        {
            var text = string.Format(@"Time played: {0}
Distance traveled: {1}
Ink collected: {2}
Jumps: {3}
Deaths: {4}
Max level retry count: {5}
Arcade max height: {6}
Arcade games played: {7}
Arcade score: {8}",
                                     GameGlobals.TotalTimePlayed.ToString(@"hh\:mm\:ss"),
                                     (int) (GameGlobals.SaveData.DistanceTraveled/EngineGlobals.PixelsPerMeter),
                                     GameGlobals.SaveData.TotalInkCollected,
                                     GameGlobals.SaveData.TotoalJumps,
                                     GameGlobals.SaveData.TotalDeaths,
                                     GameGlobals.SaveData.MaxLevelRetryCount,
                                     GameGlobals.SaveData.MaxArcadeHeight,
                                     GameGlobals.SaveData.ArcadeGamesPlayed,
                                     GameGlobals.SaveData.ArcadeScore);

            var statistics = new TextRegion(rectangle, GameGlobals.Font, Color.Black, text, true)
            {
                //Owner = this,
                LayerDepth = 0.2f,
                StaticPosition = staticPos
            };
            
            Controller.AddObject(statistics);
        }

        public void InitSound()
        {
            SoundManager.GlobalSoundVolume = 0.7f;
           // MusicManager.MuteMusic = !GameGlobals.Settings.MusicOn;
            if (!_firstLoad)
                MusicManager.Play("menu", true);
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

        public void StartArcade()
        {
            
            if (GameGlobals.EditorMode)
                return;
            if (!GameGlobals.SaveData.ArcadeWarningDisplayed)
            {
                var levelsCompleted = false;
                for (var i = 0; i < 5; i++)
                {
                    if (GameGlobals.SaveData.RatingNormal[i] > -1)
                    {
                        levelsCompleted = true;
                        break;
                    }
                }
                if (!levelsCompleted)
                {
                    for (var i = 0; i < 5; i++)
                    {
                        if (GameGlobals.SaveData.Rating[i] > -1)
                        {
                            levelsCompleted = true;
                            break;
                        }
                    }
                }

                if (!levelsCompleted)
                {
                    EngineGlobals.ScreenManager.QueueShowPopup(new MessageBox("Warning",
                        "It's highly recommended that You play a few levels first to learn game mechanics before playing Arcade."));
                    GameGlobals.SaveData.ArcadeWarningDisplayed = true;
                    SaveData.Save();
                    return;
                }

                GameGlobals.SaveData.ArcadeWarningDisplayed = true;
                SaveData.Save();
                
            }
            _loadArcade = true;
            FadeOut();
        }

        protected override void FadeOutCompleted()
        {
            base.FadeOutCompleted();
            if (_loadArcade)
            {
                if (!GameGlobals.SaveData.ArcadeTutorialScreenShowed)
                {
                    //EngineGlobals.ScreenManager.QueueChangeScreen(new BaseLoadingScreen(new ArcadeScreen()));
                    EngineGlobals.ScreenManager.QueueChangeScreen(
                        new BaseLoadingScreen(TutorialScreen.GetTutorialScreen(-1, new ArcadeScreen())));
                    GameGlobals.SaveData.ArcadeTutorialScreenShowed = true;
                    SaveData.Save();
                }
                else
                {
                    EngineGlobals.ScreenManager.QueueChangeScreen(new ArcadeScreen());
                }

                //MusicManager.Play("arcade", true);
            }
            if (_loadMap)
            {
                MusicManager.Play("game", true);
                EngineGlobals.ScreenManager.QueueChangeScreen(new LevelLoadingScreen(_mapIndex, _mapType));
            }
        }

        protected override void FadeInCompleted()
        {
            if (!GameGlobals.SaveData.Version15InfoMessageDisplayed || GameGlobals.Random.Next(1, 101) <= 20)
            {
                EngineGlobals.ScreenManager.QueueShowPopup(new MessageBoxForV15("Information",
                    "Ad free version if finally out! If you want to enjoy an ad free experience or simply support us please check it out!"));
                if (!GameGlobals.SaveData.Version15InfoMessageDisplayed)
                {
                    GameGlobals.SaveData.Version15InfoMessageDisplayed = true;
                    SaveData.Save();
                }
            }
            base.FadeInCompleted();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (EngineGlobals.Background != null)
            {
                EngineGlobals.Background.Dispose();
                EngineGlobals.Background = null;
            }
            EngineGlobals.TriggerManager.Dispose();
            EngineGlobals.TriggerManager = null;
            WindowOnInput -= OnWindowOnInput;
            GameGlobals.Map = null;
            GameGlobals.Menu = null;
        }
    }
}
