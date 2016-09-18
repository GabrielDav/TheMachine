using System;
using System.Collections.Generic;
using GameLibrary.Objects;
#if WINDOWS
using System.Security.Policy;
#endif
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary;
using GameLibrary.Gui;
using GameLibrary.Gui.ScreenManagement.NewScreens;
#if WINDOWS_PHONE
using Microsoft.Devices;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Player = GameLibrary.Objects.Player;

namespace TheGoo
{

    public enum Achievement
    {
        FastLearner = 0,
        CloseCall = 1,
        InkCollector = 2,
        ClimberBronze = 3,
        PlayerBronze = 4,
        Efficient = 5,
        LabyrinthMaster = 6,
        Determined = 7,
        ClimberSilver = 8,
        PlayerSilver = 9,
        FirstBlood = 10,
        Professional = 11,
        HardCorePlay = 12,
        ClimberGold = 13,
        PlayerGold = 14
    }

    

   //FastLerner, Closecall,       InkCollector, ClimberBronze, PlayerBronze,
   //                 Efficient,  LabyrinthMaster, Determined,   ClimberSilver, PlayeSilver,
   //                 FirstBlood, Professional,    HardCorePlay, ClimverGold,   PlayerGold

    public static class GameGlobals
    {
        //public static Dictionary<int, int> AchievemetsPositions;

        public static string AdFreeAppId = "89e71a19-939d-42d5-bb7f-4de77c92cc6b";

        private static bool _gameOver;

        private static PhysicsManager _physics;

        public const float Factor = 1;

        public const long ArcadeTimeWarpDuration = 10000;

        public const long ArcadeInkCollectDuration = 10000;

        public static bool GameOver
        {
            get { return _gameOver; }
            set
            {
                if (value)
                {
                    if (!_gameOver)
                    {
                        _gameOver = true;
                        BackgroundColor = Color.White;
                    }
                }
                else
                {
                    _gameOver = false;
                    BackgroundColor = Color.CornflowerBlue;
                }
            }
        }

        public static bool LevelComplete;

        public const float MaxSpeed = 54/Factor;

        public const int Grid = 10;

        public static Player Player;

        private static Map _map;

        public static Map Map
        {
            get { return _map; }
            set { _map = value; }
        }

        public static PrimitiveBatch PrimitiveBatch;

        public static PhysicsManager Physics
        {
            get { return _physics; }
            set { _physics = value; }
        }

        public static Color BackgroundColor = Color.CornflowerBlue;

        public static GameMessages GameMessages;

        public static Random Random = new Random();

        public static bool ArcadeMode;

        //public static ArcadePowerUp CurrentArcadePowerUp;

        public static Settings Settings;

        public static SaveData SaveData;

        public static StorageControl Storage;

        public static int RandomSign
        {
            get { return Random.Next(0, 1) == 1 ? 1 : -1; }
        }

        public static bool EditorMode;

        public static MapList MapList;

        public static GameScreen Game;

        public static void RemoveObject(PhysicalObject gameObject)
        {
            Map.GameObjects.Remove(gameObject);
            Controller.RemoveGameObject(gameObject);
            gameObject.Dispose();
        }

        public static bool IsTrial = true;

        public static MainMenuScreen Menu;

        public static bool Vibrate = false;

#if WINDOWS_PHONE
        internal static AdControl AdControl;
#endif

        #if WINDOWS_PHONE
        public static VibrateController VibrateController = VibrateController.Default;
        #endif

        public static class CreditsData
        {
            public static long GameTime;
            public static int InkCollected;
            public static int TotalInk;

            public static void Reset()
            {
                GameTime = 0;
                InkCollected = 0;
            }
        }

        public static int TotalJumps;
        public static int TotalDeaths;
        public static int DeathsPerLevel;
        public static int LevelJumps;
        public static bool HardCorePlayLevel;
        public static float InkLeft;
        public static int TotalInkCollected;
        public static TimeSpan TotalTimePlayed;
        

        public static bool HardcoreMode;

        #region GUI

        public static HealthBar HealthBar;

        public static SpriteFont Font;

        public static class MenuGlobals
        {
            public static SpriteFont MenuFont;
        }

        public static void UpdateScore()
        {
            ScoreRegion.Text = ScoreTotal.ToString();
        }

        public static int ScoreTotal
        {
            get { return (MaxHeight + Score); }
        }

        public static int Score;

        public static int MaxHeight;

        public static TextRegion ScoreRegion;

        #endregion

        #region Achievements

        public static Texture2D GetAchievementTexture(Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.FastLearner:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\FastLearner");
                case Achievement.FirstBlood:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\FirstBlood");
                case Achievement.CloseCall:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\CloseCall");
                case Achievement.PlayerBronze:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\PlayerBronze");
                case Achievement.PlayerSilver:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\PlayerSilver");
                case Achievement.PlayerGold:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\PlayerGold");
                case Achievement.InkCollector:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\InkCollector");
                case Achievement.ClimberBronze:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\ClimberBronze");
                case Achievement.ClimberSilver:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\ClimberSilver");
                case Achievement.ClimberGold:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\ClimberGold");
                case Achievement.Determined:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\Icon_Collect");
                case Achievement.HardCorePlay:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\HardCorePlay");
                case Achievement.Efficient:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\Efficient");
                case Achievement.LabyrinthMaster:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\LabyrinthMaster");
                case Achievement.Professional:
                    return EngineGlobals.ContentCache.Load<Texture2D>(@"Gui\Achievements\Professional");
                default:
                    throw new Exception("Undefined achivement type: " + achievement);
            }
        }

        public static string GetAchievementDescription(Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.FastLearner:
                    return @"Complete first level in normal mode performing less then 10 jumps.";
                case Achievement.FirstBlood:
                    return @"Die first time in the game.";
                case Achievement.CloseCall:
                    return @"Complete level with less than 5% ink remaining.";
                case Achievement.PlayerBronze:
                    return @"Complete 10 levels in hardcore mode getting all 3 gears.";
                case Achievement.PlayerSilver:
                    return @"Complete 20 levels in hardcore mode getting all 3 gears.";
                case Achievement.PlayerGold:
                    return @"Complete 30 levels in hardcore mode getting all 3 gears.";
                case Achievement.InkCollector:
                    return @"Collect more than 3000 ink.";
                case Achievement.ClimberBronze:
                    return @"Reach 500 height in Arcade.";
                case Achievement.ClimberSilver:
                    return @"Reach 700 height in Arcade.";
                case Achievement.ClimberGold:
                    return @"Reach 900 height in Arcade.";
                case Achievement.Determined:
                    return @"Play Arcade over 100 times reaching at least 300 height.";
                case Achievement.HardCorePlay:
                    return @"Complete levels 9 and 10 in hardcore mode consecutively without dying once.";
                case Achievement.Efficient:
                    return @"Finish level 3 in normal mode with more than 25% ink remaining.";
                case Achievement.Professional:
                    return @"Finish 15 normal mode levels getting all \n3 gears in each level.";
                case Achievement.LabyrinthMaster:
                    return @"Collect all ink in level 14 in normal mode.";
                default:
                    throw new Exception("Undefined achivement type: " + achievement);
            }
        }

        public static string GetAchievementName(Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.FastLearner:
                    return @"Fast Learner";
                case Achievement.FirstBlood:
                    return @"First Blood";
                case Achievement.CloseCall:
                    return @"Close Call";
                case Achievement.PlayerBronze:
                    return @"Best Play: Bronze";
                case Achievement.PlayerSilver:
                    return @"Best Play: Silver";
                case Achievement.PlayerGold:
                    return @"Best Play: Gold";
                case Achievement.InkCollector:
                    return @"Ink Collector";
                case Achievement.ClimberBronze:
                    return @"Climber: Bronze";
                case Achievement.ClimberSilver:
                    return @"Climber: Silver";
                case Achievement.ClimberGold:
                    return @"Climber: Gold ";
                case Achievement.Determined:
                    return @"Veteran";
                case Achievement.HardCorePlay:
                    return @"Ace";
                case Achievement.Professional:
                    return @"Professional";
                case Achievement.LabyrinthMaster:
                    return @"Labyrinth Master";
                case Achievement.Efficient:
                    return @"Efficient";
                default:
                    throw new Exception("Undefined achivement type: " + achievement);
            }
        }

        public const int AchievementsCount = 12;

        #endregion
    }
}
