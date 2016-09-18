using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
#if EDITOR
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
#elif WINDOWS_PHONE
using Engine.Graphics;
using Mangopollo;
using Mangopollo.Tiles;
using Microsoft.Phone.Shell;
#endif
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary
{

    public static class StoredData
    {

        public static void Save<T>(T data, string fileName)
        {
            var serializer = new XmlSerializer(typeof(T));
            var binaryWriter = GameGlobals.Storage.OpenFileBinaryWrite(fileName);
            serializer.Serialize(binaryWriter.BaseStream, data);
            GameGlobals.Storage.CloseStream();
        }

        public static T Load<T>(string fileName)
        {
            var serializer = new XmlSerializer(typeof(T));
            var binaryReader = GameGlobals.Storage.OpenFileBinaryRead(fileName);
            var data = (T)serializer.Deserialize(binaryReader.BaseStream);
            GameGlobals.Storage.CloseStream();
            return data;
        }
    }

    public class Settings
    {

        protected static string FileName
        {
            get
            {
                return "GameSettings.xml";
            }
        }

        public bool SoundOn { get; set; }
        public bool MusicOn { get; set; }
        public int MusicVolume { get; set; }
        public bool VibrationOn { get; set; }


        public static Settings GetDefault()
        {
            return new Settings
                       {
                           SoundOn = true,
                           MusicOn = true,
                           MusicVolume = 70
                       };
        }

        public static void Load()
        {
            GameGlobals.Settings = GameGlobals.Storage.FileExists(FileName) ? StoredData.Load<Settings>(FileName) : GetDefault();
            SoundManager.MuteSound = !GameGlobals.Settings.SoundOn;
            
            if (!GameGlobals.Settings.MusicOn && GameGlobals.Settings.MusicVolume != 0)
                GameGlobals.Settings.MusicVolume = 0;
            else if (GameGlobals.Settings.MusicOn && GameGlobals.Settings.MusicVolume == 0)
                GameGlobals.Settings.MusicVolume = 70; // set to default

            MusicManager.GlobalMusicVolume = GameGlobals.Settings.MusicVolume/100f;
            MusicManager.MuteMusic = !GameGlobals.Settings.MusicOn;
        }

        public static void Save()
        {
            StoredData.Save(GameGlobals.Settings, FileName);
        }

    }

    public class MapList
    {

        public string MenuMap { get; set; }

        public string ArcadeMap { get; set; }

        public List<string> GameMaps { get; set; }

        public List<int> LevelsTime { get; set; }

        public List<int> LevelsScore { get; set; }

        public List<int> MinLevelScore { get; set; }

        public List<bool> AdOrientationTop { get; set; }

        public List<string> NormalMaps { get; set; }

        public List<int> LevelsTimeNormal { get; set; }

        public List<int> LevelsScoreNormal { get; set; }

        public List<int> MinLevelScoreNormal { get; set; } 

        public List<bool> AdOrientationTopNornal { get; set; }

        public string GameEndMap { get; set; }
    }

    public class SaveData
    {
        public static string FileName
        {
            get
            {
                return "SaveData.xml";
            }
        }
        
        public List<int> Scores { get; set; }

        public List<int> Rating { get; set; }

        public int ArcadeScore { get; set; }

        public List<int> UnlockedAchievements { get; set; }

        public int TotoalJumps;

        public int TotalDeaths;

        public int TotalInkCollected { get; set; }

        public double TotalTimePlayed { get; set; }

        public float DistanceTraveled { get; set; }

        public int MaxLevelRetryCount { get; set; }

        public int MaxArcadeHeight { get; set; }

        public int ArcadeGamesPlayed { get; set; }

        public int HighArcadeGamesPlayed { get; set; }

        public bool InfoMessageDisplayed { get; set; }

        public bool Version15InfoMessageDisplayed { get; set; }

        public bool ArcadeWarningDisplayed { get; set; }

        public bool ArcadeTutorialScreenShowed { get; set; }

        public List<int> ScoresNormal { get; set; }

        public List<int> RatingNormal { get; set; }

 
        public static SaveData GetDefault()
        {
            var scores = InitList(GameGlobals.MapList.GameMaps.Count, 0);

            var scoresNormal = InitList(GameGlobals.MapList.NormalMaps.Count, 0);

            var rating = InitList(GameGlobals.MapList.GameMaps.Count, -1);

            var ratingNormal = InitList(GameGlobals.MapList.NormalMaps.Count, -1);
            
            return new SaveData
            {
                ArcadeScore = 0,
                Scores = scores,
                ScoresNormal = scoresNormal,
                Rating = rating,
                RatingNormal = ratingNormal,
                UnlockedAchievements = new List<int>(),
                TotalTimePlayed = 0
            };
        }

        public static void Load()
        {
            GameGlobals.SaveData = GameGlobals.Storage.FileExists(FileName) ? StoredData.Load<SaveData>(FileName) : GetDefault();

            if (GameGlobals.SaveData.RatingNormal == null || GameGlobals.SaveData.RatingNormal.Count < 1)
            {
                GameGlobals.SaveData.RatingNormal = InitList(GameGlobals.MapList.NormalMaps.Count, -1);
            }

            if (GameGlobals.SaveData.ScoresNormal == null || GameGlobals.SaveData.ScoresNormal.Count < 1)
            {
                GameGlobals.SaveData.ScoresNormal = InitList(GameGlobals.MapList.NormalMaps.Count, 0);
            }

            if (GameGlobals.SaveData.UnlockedAchievements == null)
            {
                GameGlobals.SaveData.UnlockedAchievements = new List<int>();
            }
            GameGlobals.TotalJumps = GameGlobals.SaveData.TotoalJumps;
            GameGlobals.TotalDeaths = GameGlobals.SaveData.TotalDeaths;
            GameGlobals.TotalInkCollected = GameGlobals.SaveData.TotalInkCollected;
            GameGlobals.TotalTimePlayed = TimeSpan.FromMilliseconds(GameGlobals.SaveData.TotalTimePlayed);

            /*
            for (int i = 0; i < GameGlobals.SaveData.Rating.Count; i++)
            {
                GameGlobals.SaveData.Rating[i] = 0;
            }

            GameGlobals.SaveData.Rating[13] = -1;

            for (int i = 0; i < GameGlobals.SaveData.RatingNormal.Count; i++)
            {
                GameGlobals.SaveData.RatingNormal[i] = 0;
            }
             */
            

        }

        public static void Save()
        {
            StoredData.Save(GameGlobals.SaveData, FileName);
        }

        public static void UpdateStatistics()
        {
            GameGlobals.SaveData.TotoalJumps = GameGlobals.TotalJumps;
            GameGlobals.SaveData.TotalDeaths = GameGlobals.TotalDeaths;
            GameGlobals.SaveData.TotalTimePlayed = GameGlobals.TotalTimePlayed.TotalMilliseconds;
            GameGlobals.SaveData.TotalInkCollected = GameGlobals.TotalInkCollected;
            GameGlobals.DeathsPerLevel = 0;
            GameGlobals.LevelJumps = 0;
        }

        private static List<int> InitList(int count, int value)
        {
            var raiting = new List<int>();
            for (var i = 0; i < count; i++)
            {
                raiting.Add(value);
            }

            return raiting;
        }

    }

#if WINDOWS_PHONE
    public static class PhoneUtils
    {
        private static SpriteFont _renderFont;
        private static Texture2D _tileLargeOriginal;
        private static Texture2D _tileLargeOriginalBack;
        private static Texture2D _tileWideOriginal;
        private static Texture2D _tileWideOriginalBack;

        private static Texture2D ReadTextureFromResource(string path)
        {
            var stream = Application.GetResourceStream(new Uri(path, UriKind.Relative)).Stream;
            var texture = Texture2D.FromStream(EngineGlobals.Device, stream);
            stream.Close();
            return texture;
        }

        private static void SaveTextureToStorage(string path, Texture2D texture, IsolatedStorageFile isolatedStorage)
        {
            var stream = new IsolatedStorageFileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, isolatedStorage);
            texture.SaveAsPng(stream, texture.Width, texture.Height);
            stream.Close();
        }

        public static void UpdateTiles()
        {

            if (!Utils.CanUseLiveTiles)
                return;
            var isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            const string baseIsoUri = "/Shared/ShellContent/";

            if (_renderFont == null)
                _renderFont = EngineGlobals.ContentCache.Load<SpriteFont>("TahomaFont");
            
            if (_tileLargeOriginal == null)
            {
               // if (!isolatedStorage.FileExists(baseIsoUri + "TileLarge.png"))
               // {
                    _tileLargeOriginal = ReadTextureFromResource(@"Assets/TileLarge.png");
                    SaveTextureToStorage(baseIsoUri + "TileLarge.png", _tileLargeOriginal, isolatedStorage);
              //  }
            }
            if (_tileLargeOriginalBack == null)
            {
                _tileLargeOriginalBack = ReadTextureFromResource(@"Assets/TileLargeBack.png");
            }
            if (_tileWideOriginal == null)
            {
                //if (!isolatedStorage.FileExists(baseIsoUri + "TileWide.png"))
               // {
                    _tileWideOriginal = ReadTextureFromResource(@"Assets/TileWide.png");
                    SaveTextureToStorage(baseIsoUri + "TileWide.png", _tileWideOriginal, isolatedStorage);
               // }
            }
            if (_tileWideOriginalBack == null)
            {
                _tileWideOriginalBack = ReadTextureFromResource(@"Assets/TileWideBack.png");
            }

            // Render tile large
 
            var tileLargeBackRender = new RenderTarget2D(EngineGlobals.Device, 336, 336);
            
            var regionScoreText = new TextRegion(new Rectangle(0, 70, 336, 80), _renderFont, Color.White, "SCORE", false)
                {
                    HorizontaAlign = FontHorizontalAlign.Center
                };
            var regionScoreValue = new TextRegion(new Rectangle(0, 157, 336, 80), _renderFont, Color.White,
                                                           GameGlobals.SaveData.ArcadeScore.ToString(), false)
                {
                    HorizontaAlign = FontHorizontalAlign.Center
                };
            EngineGlobals.Device.SetRenderTarget(tileLargeBackRender);

            EngineGlobals.Batch.Begin();
            EngineGlobals.Batch.Draw(_tileLargeOriginalBack, new Rectangle(0, 0, 336, 336), Color.White);
            regionScoreText.Draw();
            regionScoreValue.Draw();
            EngineGlobals.Batch.End();

            EngineGlobals.Device.SetRenderTarget(null);

            SaveTextureToStorage(baseIsoUri + "TileLargeBack.png", tileLargeBackRender, isolatedStorage);
            tileLargeBackRender.Dispose();

            // Render tile wide

            var tileWideBackRender = new RenderTarget2D(EngineGlobals.Device, 691, 336);

            EngineGlobals.Device.SetRenderTarget(tileWideBackRender);

            EngineGlobals.Batch.Begin();
            EngineGlobals.Batch.Draw(_tileWideOriginalBack, new Rectangle(0, 0, 691, 336), Color.White);
            regionScoreText.Draw();
            regionScoreValue.Draw();
            EngineGlobals.Batch.End();

            EngineGlobals.Device.SetRenderTarget(null);

            SaveTextureToStorage(baseIsoUri + "TileWideBack.png", tileWideBackRender, isolatedStorage);
            tileWideBackRender.Dispose();

            
            
            
            foreach (var tile in ShellTile.ActiveTiles)
            {
                var tileData = new FlipTileData
                {
                    Title = string.Empty,
                    BackTitle = "The Machine Free",
                    BackContent = string.Empty,
                    WideBackContent = string.Empty,
                    SmallBackgroundImage = new Uri("/Assets/TileSmall.png", UriKind.Relative),
                    BackgroundImage = new Uri("isostore:" + baseIsoUri + "TileLarge.png", UriKind.Absolute),
                    BackBackgroundImage = new Uri("isostore:" + baseIsoUri + "TileLargeBack.png", UriKind.Absolute),
                    WideBackgroundImage = new Uri("isostore:" + baseIsoUri + "TileWide.png", UriKind.Absolute),
                    WideBackBackgroundImage = new Uri("isostore:" + baseIsoUri + "TileWideBack.png", UriKind.Absolute)
                };
                tile.Update(tileData);
            }
        }
    }
#endif
}
