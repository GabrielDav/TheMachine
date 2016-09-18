using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Engine.Core;
using Engine.Graphics;
using GameLibrary.GridBuilder;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class LevelCompleteScreen : BaseMainScreen
    {

        protected enum TextId
        {
            ScoreDescription, ScoreValue, InkDescription, InkValue, TimeDescription, TimeValue
        }

        protected TileType _mapType;

        protected Image _shadowImg;
        protected ResizeEffect _resizeEffect;
        protected ColorEffect _colorEffect;

        protected Image _handScore;
        protected Image _handInk;
        protected Image _handTime;
        protected Image _cilinderInk;
        protected Image _cilinderTime;
        protected Image _cilinderScore;
        protected Image _buttonMenu;
        protected Image _buttonReplay;
        protected Image _buttonContinue;
        protected Image _handBotLeft;
        protected Image _handBotRight;
        protected Image _cilinderBot;

        protected ColorEffect _newHighScoreFadeOut;
        protected ColorEffect _newHighScoreFadeIn;

        protected RotateEffect _inkRotateEffect;
        protected RotateEffect _timeRotateEffect;
        protected RotateEffect _scoreRotateEffect;
        protected MoveEffect _botHandLeftMoveEffect;
        protected MoveEffect _botHandRightMoveEffect;
        protected MoveEffect _botCillinderMoveEffect;
        protected TextRegion _newHighScoreTextRegion;
        protected TextRegion _newHighScoreValueTextRegion;
        protected int _step;

        protected TextRegion[] _textRegions;

        protected Image[] _stars;
        protected Image[] _starShadows;
        protected GameTexture _starEmptyTexture;
        protected GameTexture _starFilledTexture;

        protected ResizeEffect _starsResizeEffect;
        protected ColorEffect _starsColorEffect;


        protected int _starsToFill;
        protected int _currentStarsFill;
        protected bool _fillNextStar;
        protected bool _newHighScore;
        protected ColorEffect _newHighScoreColorEffect;
        protected ColorEffect _newHighScoreValueColorEffect;

        protected Image[] _gears;

        protected int _currentLevel;
        protected bool _queueNextLevel;
        protected bool _queueReplay;
        protected bool _queueMainMenu;
        protected List<Achievement> _newAchievements;

        protected float _currentInkPercentage;
        protected bool _levelAlreadyCompleted;

        public LevelCompleteScreen(int currentLevel, TileType? mapType)
        {
            FadeInTime = 100;
            _currentLevel = currentLevel;
            if (mapType.HasValue)
            {
                _mapType = mapType.Value;
            }
        }

        protected virtual void CreateShadowImg(Image image)
        {
            if (_shadowImg == null)
            {
                _shadowImg = new Image(new GameTexture(image.Texture));
                _shadowImg.Orgin = _shadowImg.OriginCenter();
                _shadowImg.Flip = image.Flip;
            }
            _shadowImg.Rect = new Rectangle((int) (image.Rect.X + image.Rect.Width/2f),
                                            (int) (image.Rect.Y + image.Rect.Height/2f), image.Rect.Width,
                                            image.Rect.Height);
            _shadowImg.Color = new Color(255, 255, 255, 255);
            if (_resizeEffect == null)
                _resizeEffect = new ResizeEffect(_shadowImg, new Vector2((int)(image.Rect.Width * 1.5), (int)(image.Rect.Height * 1.5)), 500);
            else
                _resizeEffect.Reset(new Vector2((int)(image.Rect.Width * 1.5), (int)(image.Rect.Height * 1.5)), 500);
            if (_colorEffect == null)
                _colorEffect = new ColorEffect(_shadowImg, new Color(255, 255, 255, 0), 500);
            else
                _colorEffect.Reset(new Color(255, 255, 255, 0), 500);
            Controller.AddObject(_shadowImg);
        }

        public override void Load()
        {
            base.Load();

            EngineGlobals.Camera2D = new Camera2D();

            _handScore = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreHand")) {LayerDepth = 0.5f, Owner = this};
            _handInk = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreHand")) {LayerDepth = 0.5f, Owner = this};
            _handTime = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreHand")) {LayerDepth = 0.5f, Owner = this};
            _cilinderInk = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreCilinder"))
            {
                LayerDepth = 0.45f,
                Owner = this
            };
            _cilinderTime = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreCilinder"))
            {
                LayerDepth = 0.45f,
                Owner = this
            };
            _cilinderScore = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreHandTopCilinder"))
            {
                LayerDepth = 0.45f,
                Owner = this
            };
            _buttonMenu = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ButtonMenu"))
            {
                LayerDepth = 0.5f,
                Owner = this,
                IsHidden = true,
                Color = new Color(255, 255, 255, 0)
            };
            _buttonReplay = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ButtonReplay"))
            {
                LayerDepth = 0.5f,
                Owner = this,
                IsHidden = true,
                Color = new Color(255, 255, 255, 0)
            };
            _buttonContinue = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ButtonContinue"))
            {
                LayerDepth = 0.5f,
                Owner = this,
                IsHidden = true,
                Color = new Color(255, 255, 255, 0)
            };
            _handBotLeft = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreHandBot"))
            {
                LayerDepth = 0.55f,
                Owner = this
            };
            _handBotRight = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreHandBot"))
            {
                LayerDepth = 0.55f,
                Owner = this
            };
            _cilinderBot = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ScoreCilinderBot"))
            {
                LayerDepth = 0.5f,
                Owner = this
            };

            _textRegions = new TextRegion[6];

            _textRegions[(int)TextId.ScoreDescription] = new TextRegion(new Rectangle(302, 40, 197, 25), GameGlobals.MenuGlobals.MenuFont,
                "Score", false) { LayerDepth = 0.4f, Color = Color.Transparent, Owner = this, HorizontaAlign = FontHorizontalAlign.Center, IsHidden = false };
            _textRegions[(int)TextId.ScoreValue] = new TextRegion(new Rectangle(302, 80, 197, 25), GameGlobals.MenuGlobals.MenuFont,
                "0", false) { LayerDepth = 0.4f, Color = Color.Transparent, Owner = this, HorizontaAlign = FontHorizontalAlign.Center, IsHidden = false };

            _textRegions[(int)TextId.InkDescription] = new TextRegion(new Rectangle(205, 180, 175, 25), GameGlobals.MenuGlobals.MenuFont,
                "Ink", false) { LayerDepth = 0.4f, Color = Color.Transparent, Owner = this, HorizontaAlign = FontHorizontalAlign.Center, IsHidden = false };
            _textRegions[(int)TextId.InkValue] = new TextRegion(new Rectangle(205, 220, 175, 25), GameGlobals.MenuGlobals.MenuFont,
                "100%", false) { LayerDepth = 0.4f, Color = Color.Transparent, Owner = this, HorizontaAlign = FontHorizontalAlign.Center, IsHidden = false };

            _textRegions[(int)TextId.TimeDescription] = new TextRegion(new Rectangle(417, 180, 175, 25), GameGlobals.MenuGlobals.MenuFont,
                _currentLevel == -1? "Height" : "Time", false) { LayerDepth = 0.4f, Color = Color.Transparent, Owner = this, HorizontaAlign = FontHorizontalAlign.Center, IsHidden = false };
            _textRegions[(int)TextId.TimeValue] = new TextRegion(new Rectangle(417, 220, 175, 25), GameGlobals.MenuGlobals.MenuFont,
                "0:00", false) { LayerDepth = 0.4f, Color = Color.Transparent, Owner = this, HorizontaAlign = FontHorizontalAlign.Center, IsHidden = false };

            Controller.AddObject(_handScore);
            Controller.AddObject(_handInk);
            Controller.AddObject(_handTime);
            Controller.AddObject(_cilinderInk);
            Controller.AddObject(_cilinderTime);
            Controller.AddObject(_cilinderScore);
            Controller.AddObject(_buttonMenu);
            Controller.AddObject(_buttonReplay);
            Controller.AddObject(_buttonContinue);
            Controller.AddObject(_handBotLeft);
            Controller.AddObject(_handBotRight);
            Controller.AddObject(_cilinderBot);


            foreach (var textRegion in _textRegions)
            {
                Controller.AddObject(textRegion);
            }


            _starEmptyTexture = new GameTexture("GUI\\Menu\\Star_Empty");
            _starFilledTexture = new GameTexture("GUI\\Menu\\Star_Filled");

            _starShadows = new Image[3];
            _starShadows[0] = new Image(_starFilledTexture, new Rectangle(310, 344, 60, 60), new Color(255, 255, 255, 150)) { IsHidden = true, LayerDepth = 0.3f};
            _starShadows[1] = new Image(_starFilledTexture, new Rectangle(400, 344, 60, 60), new Color(255, 255, 255, 150)) { IsHidden = true, LayerDepth = 0.3f };
            _starShadows[2] = new Image(_starFilledTexture, new Rectangle(490, 344, 60, 60), new Color(255, 255, 255, 150)) { IsHidden = true, LayerDepth = 0.3f };


            _stars = new Image[3];
            _stars[0] = new Image(_starEmptyTexture, new Rectangle(310, 520, 60, 60)) { LayerDepth = 0.45f};
            _stars[1] = new Image(_starEmptyTexture, new Rectangle(400, 520, 60, 60)) { LayerDepth = 0.45f };
            _stars[2] = new Image(_starEmptyTexture, new Rectangle(490, 520, 60, 60)) { LayerDepth = 0.45f };
            for (var i = 0; i < 3; i++)
            {
                _stars[i].Orgin = _stars[i].OriginCenter();
                Controller.AddObject(_stars[i]);
                _starShadows[i].Orgin = _starShadows[i].OriginCenter();
                Controller.AddObject(_starShadows[i]);
            }

            if (_currentLevel == -1)
            {
                _newHighScoreTextRegion = new TextRegion(new Rectangle(280, 300, 240, 60), GameGlobals.MenuGlobals.MenuFont,
                    new Color(0, 0, 0, 0), "New High Score", false) {LayerDepth = 0.45f, Owner = this, HorizontaAlign = FontHorizontalAlign.Center};
                _newHighScoreValueTextRegion = new TextRegion(new Rectangle(280, 340, 240, 60), GameGlobals.MenuGlobals.MenuFont,
                    new Color(0, 0, 0, 0), GameGlobals.SaveData.ArcadeScore.ToString(), false) { LayerDepth = 0.45f, Owner = this, HorizontaAlign = FontHorizontalAlign.Center };
                
            }

            EngineGlobals.Background = new BackgroundManager(1000, 600);
            EngineGlobals.Background.LoadBackground(new GameTexture("5_bg"));

            _gears = new[]
            {
                new Image(new GameTexture(@"GameObjects\Images\BG_Gear3.png")) { LayerDepth = 0.6f},
                new Image(new GameTexture(@"GameObjects\Images\BG_Gear6.png")) { LayerDepth = 0.6f}
            };

            Controller.AddObject(_gears[0]);
            Controller.AddObject(_gears[1]);
        }

        private void SetRating()
        {
            var inkPercentage = 0f;
            if (GameGlobals.CreditsData.TotalInk == 0)
            {
                _textRegions[(int) TextId.InkValue].Text = "100%";
                inkPercentage = 100f;
            }
            else
            {
                inkPercentage = GameGlobals.CreditsData.InkCollected*100f/GameGlobals.CreditsData.TotalInk;
                _textRegions[(int) TextId.InkValue].Text = (int) Math.Floor(inkPercentage) + "%";
            }

            _currentInkPercentage = inkPercentage;

            var score = inkPercentage * 6 * (_currentLevel + 1);
            _textRegions[(int)TextId.ScoreValue].Text = score.ToString();
            var timeSpan = TimeSpan.FromMilliseconds(GameGlobals.CreditsData.GameTime);
            _textRegions[(int)TextId.TimeValue].Text = timeSpan.Minutes.ToString("00") + ":" +
                                                        timeSpan.Seconds.ToString("00");

            if (_mapType == TileType.Normal)
            {
                _levelAlreadyCompleted = GameGlobals.SaveData.RatingNormal[_currentLevel] > -1;

                var timeScore = (GameGlobals.MapList.LevelsTimeNormal[_currentLevel] * 100f / GameGlobals.CreditsData.GameTime) * (_currentLevel + 1) * 4;
                var totalScore = score + timeScore;

                _textRegions[(int)TextId.ScoreValue].Text = ((int)totalScore).ToString();
                var mid = (GameGlobals.MapList.LevelsScoreNormal[_currentLevel] +
                           GameGlobals.MapList.MinLevelScoreNormal[_currentLevel]) / 2;

                if (totalScore >= GameGlobals.MapList.LevelsScoreNormal[_currentLevel])
                    _starsToFill = 3;
                else if (totalScore >= mid)
                    _starsToFill = 2;
                else if (totalScore >= (GameGlobals.MapList.MinLevelScoreNormal[_currentLevel]))
                    _starsToFill = 1;
                else
                    _starsToFill = 0;

                if (totalScore >= GameGlobals.SaveData.ScoresNormal[_currentLevel])
                {
                    GameGlobals.SaveData.RatingNormal[_currentLevel] = _starsToFill;
                    GameGlobals.SaveData.ScoresNormal[_currentLevel] = (int)totalScore;
                }
            }
            else if (_mapType == TileType.Hardcore)
            {
                _levelAlreadyCompleted = GameGlobals.SaveData.Rating[_currentLevel] > -1;

                var timeScore = (GameGlobals.MapList.LevelsTime[_currentLevel] * 100f / GameGlobals.CreditsData.GameTime) * (_currentLevel + 1) * 4;
                var totalScore = score + timeScore;

                _textRegions[(int)TextId.ScoreValue].Text = ((int)totalScore).ToString();
                var mid = (GameGlobals.MapList.LevelsScore[_currentLevel] +
                           GameGlobals.MapList.MinLevelScore[_currentLevel]) / 2;

                if (totalScore >= GameGlobals.MapList.LevelsScore[_currentLevel])
                    _starsToFill = 3;
                else if (totalScore >= mid)
                    _starsToFill = 2;
                else if (totalScore >= (GameGlobals.MapList.MinLevelScore[_currentLevel]))
                    _starsToFill = 1;
                else
                    _starsToFill = 0;

                if (totalScore >= GameGlobals.SaveData.Scores[_currentLevel])
                {
                    GameGlobals.SaveData.Rating[_currentLevel] = _starsToFill;
                    GameGlobals.SaveData.Scores[_currentLevel] = (int)totalScore;
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _handScore.Rect = new Rectangle(88, -15, 159, 82);
            _handScore.Flip = SpriteEffects.FlipVertically;
            _handScore.Rotation = MathHelper.ToRadians(-90);
            _handInk.Rect = new Rectangle(-55, 120, 159, 82);
            _handInk.Flip = SpriteEffects.FlipHorizontally;
            _handTime.Rect = new Rectangle(696, 120, 159, 82);
            _cilinderInk.Rect = new Rectangle(63, 160, 334, 119);
            _cilinderInk.Flip = SpriteEffects.FlipHorizontally;
            _cilinderInk.Orgin = new Vector2(10, 10);
            _cilinderInk.Rotation = MathHelper.ToRadians(112);
            _cilinderTime.Rect = new Rectangle(737, 160, 334, 119);
            _cilinderTime.Orgin = new Vector2(324, 10);
            _cilinderTime.Rotation = MathHelper.ToRadians(-112);
            _cilinderScore.Rect = new Rectangle(168, 65, 348, 113);
            _cilinderScore.Orgin = new Vector2(10, 36);
            _cilinderScore.Rotation = MathHelper.ToRadians(-46);
            if (_currentLevel != -1)
            {
                _buttonMenu.Rect = new Rectangle(20, 245, 115, 104);
                _buttonContinue.Rect = new Rectangle(665, 362, 115, 104);
                _buttonReplay.Rect = new Rectangle(20, 362, 115, 104);
            }
            else
            {
                _buttonMenu.Rect = new Rectangle(20, 362, 115, 104);
                _buttonReplay.Rect = new Rectangle(665, 362, 115, 104);
            }

            _handBotLeft.Rect = new Rectangle(160, 476, 95, 272);
            _handBotRight.Rect = new Rectangle(545, 476, 95, 272);
            _handBotRight.Flip = SpriteEffects.FlipHorizontally;
            _cilinderBot.Rect = new Rectangle(230, 460, 340, 120);
            _gears[0].Orgin = _gears[0].OriginCenter();
            _gears[0].Rect = new Rectangle(640, 80, 600, 600);
            _gears[1].Orgin = _gears[1].OriginCenter();
            _gears[1].Rect = new Rectangle(255, 480, 600, 600);
            _gears[1].Rotation = MathHelper.ToRadians(-7);

            EngineGlobals.Camera2D.BackgroundOffset = new Vector2(400, 240);
            EngineGlobals.Camera2D.CornerPos = new Vector2(0, 0);

            if (_currentLevel >= 0)
            {
                SetRating();
            }
            else
            {
                _textRegions[(int)TextId.TimeValue].Text = GameGlobals.MaxHeight.ToString();
                _textRegions[(int) TextId.ScoreValue].Text = GameGlobals.ScoreTotal.ToString();
                _textRegions[(int)TextId.InkValue].Text = GameGlobals.Score.ToString();
                if (GameGlobals.SaveData.ArcadeScore < GameGlobals.ScoreTotal)
                {
                    _newHighScore = true;
                    GameGlobals.SaveData.ArcadeScore = GameGlobals.ScoreTotal;
                    _newHighScoreValueTextRegion.Text = GameGlobals.ScoreTotal.ToString();
                    _newHighScoreTextRegion.Color = new Color(0, 0, 0, 0);
                    _newHighScoreFadeIn = new ColorEffect(_newHighScoreTextRegion, new Color(0, 0, 0, 255), 500);
                    _newHighScoreTextRegion.Color = Color.Black;
                    _newHighScoreFadeOut = new ColorEffect(_newHighScoreTextRegion, new Color(0, 0, 0, 0), 500);
                    _newHighScoreTextRegion.Color = new Color(0, 0, 0, 0);
                }
                else
                {
                    _newHighScoreTextRegion.Text = "High Score";
                    _newHighScoreValueTextRegion.Text = GameGlobals.SaveData.ArcadeScore.ToString();
                }
                Controller.AddObject(_newHighScoreTextRegion);
                Controller.AddObject(_newHighScoreValueTextRegion);
            }

            UpdateAchievements();

            SaveData.Save();

            GameGlobals.Score = 0;
            GameGlobals.MaxHeight = 0;

            MusicManager.Play("score", false);
            
            WindowOnInput += OnWindowOnInput;
            WindowOnBack += OnWindowOnBack;
            FadeIn();
        }

        private void UpdateAchievements()
        {
            _newAchievements = new List<Achievement>();

            // Ink Collector
            if (GameGlobals.TotalInkCollected > 3000 &&
                !GameGlobals.SaveData.UnlockedAchievements.Contains((int)Achievement.InkCollector))
            {
                _newAchievements.Add(Achievement.InkCollector);
                GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.InkCollector);
            }

            // First blood
            if (GameGlobals.TotalDeaths > 0 &&
                !GameGlobals.SaveData.UnlockedAchievements.Contains((int)Achievement.FirstBlood))
            {
                _newAchievements.Add(Achievement.FirstBlood);
                GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.FirstBlood);
            }

            if (_currentLevel >= 0)
            {
                // Fast learner
                GameGlobals.TotalJumps += GameGlobals.LevelJumps;
                if (_currentLevel == 0 && GameGlobals.LevelJumps < 10 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int) Achievement.FastLearner) && 
                    _mapType == TileType.Normal)
                {
                    _newAchievements.Add(Achievement.FastLearner);
                    GameGlobals.SaveData.UnlockedAchievements.Add((int) Achievement.FastLearner);
                }

                // Efficient
                if (_mapType == TileType.Normal && 
                    _currentLevel == 2 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int) Achievement.Efficient) &&
                    GameGlobals.Player.CurrentMovePoints >= 50)
                {
                    _newAchievements.Add(Achievement.Efficient);
                    GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.Efficient);
                }

                // LabyrinthMaster
                if (_mapType == TileType.Normal &&
                    _currentLevel == 13 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int)Achievement.LabyrinthMaster) &&
                    _currentInkPercentage > 99)
                {
                    _newAchievements.Add(Achievement.LabyrinthMaster);
                    GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.LabyrinthMaster);
                }

                // Professional
                if (_mapType == TileType.Normal && 
                    _starsToFill == 3 && 
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int)Achievement.Professional) &&
                    _currentLevel < 15)
                {
                    var starsCount = 0;
                    for (var i = 0; i < 15; i++)
                    {
                        if (GameGlobals.SaveData.RatingNormal[i] == 3 || i == _currentLevel)
                        {
                            starsCount++;
                        }
                    }

                    if (starsCount == 15)
                    {
                        _newAchievements.Add(Achievement.Professional);
                        GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.Professional);
                    }
                }

                // Hard core play
                if (GameGlobals.DeathsPerLevel < 1 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int) Achievement.HardCorePlay) &&
                    _mapType == TileType.Hardcore)
                {
                    if (_currentLevel == 8)
                    {
                        GameGlobals.HardCorePlayLevel = true;
                    }
                    else if (_currentLevel == 9 && GameGlobals.HardCorePlayLevel)
                    {
                        GameGlobals.HardCorePlayLevel = false;
                        _newAchievements.Add(Achievement.HardCorePlay);
                        GameGlobals.SaveData.UnlockedAchievements.Add((int) Achievement.HardCorePlay);
                    }
                }
                else if (GameGlobals.DeathsPerLevel > 0)
                    GameGlobals.HardCorePlayLevel = false;

                // Close call
                if (GameGlobals.InkLeft < Player.MaxMovePoints*5/100 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int) Achievement.CloseCall))
                {
                    _newAchievements.Add(Achievement.CloseCall);
                    GameGlobals.SaveData.UnlockedAchievements.Add((int) Achievement.CloseCall);
                }

                // Player Bronze/Silver/Gold
                if (_mapType == TileType.Hardcore && _currentLevel >= 0 && _starsToFill == 3)
                {
                    var starsCount = 0;
                    for (var i = 0; i < GameGlobals.SaveData.Rating.Count; i++)
                    {
                        if (GameGlobals.SaveData.Rating[i] == 3 || i == _currentLevel)
                            starsCount++;
                    }
                    if (starsCount == 10 &&
                        !GameGlobals.SaveData.UnlockedAchievements.Contains((int) Achievement.PlayerBronze))
                    {
                        _newAchievements.Add(Achievement.PlayerBronze);
                        GameGlobals.SaveData.UnlockedAchievements.Add((int) Achievement.PlayerBronze);
                    }
                    else if (starsCount == 20 &&
                             !GameGlobals.SaveData.UnlockedAchievements.Contains((int) Achievement.PlayerSilver))
                    {
                        _newAchievements.Add(Achievement.PlayerSilver);
                        GameGlobals.SaveData.UnlockedAchievements.Add((int) Achievement.PlayerSilver);
                    }
                    else if (starsCount == 30 &&
                             !GameGlobals.SaveData.UnlockedAchievements.Contains((int) Achievement.PlayerGold))
                    {
                        _newAchievements.Add(Achievement.PlayerGold);
                        GameGlobals.SaveData.UnlockedAchievements.Add((int) Achievement.PlayerGold);
                    }
                }

                
                if (_currentLevel >= 0 && GameGlobals.SaveData.MaxLevelRetryCount < GameGlobals.DeathsPerLevel)
                    GameGlobals.SaveData.MaxLevelRetryCount = GameGlobals.DeathsPerLevel;
            }
            else
            {
                // Climber Bronze
                if (GameGlobals.MaxHeight > 500 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int)Achievement.ClimberBronze))
                {
                    _newAchievements.Add(Achievement.ClimberBronze);
                    GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.ClimberBronze);
                }
                // Climber Silver
                if (GameGlobals.MaxHeight > 700 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int)Achievement.ClimberSilver))
                {
                    _newAchievements.Add(Achievement.ClimberSilver);
                    GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.ClimberSilver);
                }
                // Climber Gold
                if (GameGlobals.MaxHeight > 900 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int)Achievement.ClimberGold))
                {
                    _newAchievements.Add(Achievement.ClimberGold);
                    GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.ClimberGold);
                }

                if (_currentLevel < 0 && GameGlobals.MaxHeight > GameGlobals.SaveData.MaxArcadeHeight)
                    GameGlobals.SaveData.MaxArcadeHeight = GameGlobals.MaxHeight;
                if (_currentLevel < 0)
                    GameGlobals.SaveData.ArcadeGamesPlayed++;

                if (GameGlobals.MaxHeight > 300)
                    GameGlobals.SaveData.HighArcadeGamesPlayed++;

                // Determined
                if (GameGlobals.SaveData.HighArcadeGamesPlayed > 100 &&
                    !GameGlobals.SaveData.UnlockedAchievements.Contains((int)Achievement.Determined))
                {
                    _newAchievements.Add(Achievement.Determined);
                    GameGlobals.SaveData.UnlockedAchievements.Add((int)Achievement.Determined);
                }
                
            }
            
            SaveData.UpdateStatistics();
        }

        private void OnWindowOnBack(object sender)
        {
            _queueMainMenu = true;
            CreateShadowImg(_buttonMenu);
            FadeOut();
        }

        private void OnWindowOnInput(object sender, Point location)
        {
            if (_buttonContinue.Rect.Contains(location) && _currentLevel != -1 /*&& _currentLevel != 29*/)
            {
                _queueNextLevel = true;
                CreateShadowImg(_buttonContinue);
                FadeOut();
            }
            else if (_buttonMenu.Rect.Contains(location))
            {
                _queueMainMenu = true;
                CreateShadowImg(_buttonMenu);
                FadeOut();
            }
            else if (_buttonReplay.Rect.Contains(location))
            {
                _queueReplay = true;
                CreateShadowImg(_buttonReplay);
                FadeOut();
            }
        }

        protected override void FadeInCompleted()
        {
            base.FadeInCompleted();
            _step = 1;
        }

        private bool ModeCompleted(TileType mapType)
        {
            var rating = mapType == TileType.Hardcore
                       ? GameGlobals.SaveData.Rating
                       : GameGlobals.SaveData.RatingNormal;

            for (int i = 0; i < rating.Count; i++)
            {
                if (rating[i] < 0)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void FadeOutCompleted()
        {
            base.FadeOutCompleted();

            var normalCompleted = false;
            var hardcoreCompleted = false;

            if (_queueMainMenu)
            {
                EngineGlobals.ScreenManager.QueueChangeScreen(new BaseLoadingScreen(new MainMenuScreen()));
            }
            else if (_queueReplay)
            {
                if (_currentLevel >= 0)
                {
                    MusicManager.Play("game", true);
                }

                EngineGlobals.ScreenManager.QueueChangeScreen(
                    new BaseLoadingScreen(_currentLevel == -1
                        ? new ArcadeScreen()
                        : new GameScreen(_currentLevel, _mapType)));
            }
            else if (_queueNextLevel && _currentLevel != -1)
            {
                var rating = _mapType == TileType.Hardcore
                    ? GameGlobals.SaveData.Rating
                    : GameGlobals.SaveData.RatingNormal;


                var lastUnlockedLevel = MapTileBuilder.GetLastUnlockedLevel(_mapType);
                var nextLevel = _currentLevel + 1;
                if (nextLevel > lastUnlockedLevel)
                {


                    for (int i = 0; i < rating.Count; i++)
                    {
                        if (rating[i] < 0)
                        {
                            nextLevel = i;
                            break;
                        }
                    }
                }
                else if (!_levelAlreadyCompleted)
                {
                    normalCompleted = ModeCompleted(TileType.Normal);
                    hardcoreCompleted = ModeCompleted(TileType.Hardcore);

                    if ((_mapType == TileType.Normal && normalCompleted) ||
                        (_mapType == TileType.Hardcore && hardcoreCompleted))
                    {
                        nextLevel = rating.Count;
                    }
                }


                if (_mapType == TileType.Normal)
                {
                    if (nextLevel > 14)
                    {
                        MusicManager.Play("score", false);
                        EngineGlobals.ScreenManager.QueueChangeScreen(new EndGameScreen(normalCompleted, hardcoreCompleted));
                    }
                    else
                    {
                        MusicManager.Play("game", true);
                        EngineGlobals.ScreenManager.QueueChangeScreen(new LevelLoadingScreen(nextLevel, _mapType));
                    }
                }

                if (_mapType == TileType.Hardcore)
                {
                    if (nextLevel > 29)
                    {
                        MusicManager.Play("score", false);
                        EngineGlobals.ScreenManager.QueueChangeScreen(new EndGameScreen(normalCompleted, hardcoreCompleted));
                    }
                    else
                    {
                        MusicManager.Play("game", true);
                        EngineGlobals.ScreenManager.QueueChangeScreen(new LevelLoadingScreen(nextLevel, _mapType));
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var star in _stars)
            {
                star.Rotation += ((float)3 / 1000 * EngineGlobals.GetElapsedInGameTime() *
                                  EngineGlobals.TimeSpeed);
                if (star.Rotation >= MathHelper.Pi * 2f)
                {
                    star.Rotation = star.Rotation - MathHelper.Pi * 2f;
                }
            }

                _gears[0].Rotation += ((float)1 / 1200 * EngineGlobals.GetElapsedInGameTime() *
                                  EngineGlobals.TimeSpeed);
                if (_gears[0].Rotation >= MathHelper.Pi * 2f)
                {
                    _gears[0].Rotation = _gears[0].Rotation - MathHelper.Pi * 2f;
                }

                _gears[1].Rotation -= ((float)1 / 1200 * EngineGlobals.GetElapsedInGameTime() *
                                      EngineGlobals.TimeSpeed);
                if (_gears[1].Rotation <= -MathHelper.Pi * 2f)
                {
                    _gears[1].Rotation = _gears[1].Rotation + MathHelper.Pi * 2f;
                }

            switch (_step)
            {
                case 1:
                    _inkRotateEffect = new RotateEffect(_cilinderInk, 0, 500);
                    _step++;
                    break;
                case 2:
                    _inkRotateEffect.Update();
                    if (_inkRotateEffect.Finished)
                    {
                        Controller.AddEffectQueue(new ColorEffect(_textRegions[(int)TextId.InkDescription], Color.Black, 200));
                        Controller.AddEffectQueue(new ColorEffect(_textRegions[(int)TextId.InkValue], Color.Black, 200));
                        _timeRotateEffect = new RotateEffect(_cilinderTime, 0, 500);
                        _step++;
                    }
                    break;
                case 3:
                    _timeRotateEffect.Update();
                    if (_timeRotateEffect.Finished)
                    {
                        Controller.AddEffectQueue(new ColorEffect(_textRegions[(int)TextId.TimeDescription], Color.Black, 200));
                        Controller.AddEffectQueue(new ColorEffect(_textRegions[(int)TextId.TimeValue], Color.Black, 200));
                        _scoreRotateEffect = new RotateEffect(_cilinderScore, 0, 500);
                        _step++;
                    }
                    break;
                case 4:
                    _scoreRotateEffect.Update();
                    if (_scoreRotateEffect.Finished)
                    {
                        Controller.AddEffectQueue(new ColorEffect(_textRegions[(int) TextId.ScoreDescription],
                            Color.Black, 200));
                        Controller.AddEffectQueue(new ColorEffect(_textRegions[(int) TextId.ScoreValue], Color.Black,
                            200));
                        _botCillinderMoveEffect = new MoveEffect(_cilinderBot, new Vector2(230, 284), 500);
                        _botHandLeftMoveEffect = new MoveEffect(_handBotLeft, new Vector2(160, 300), 500);
                        _botHandRightMoveEffect = new MoveEffect(_handBotRight, new Vector2(545, 300), 500);


                        if (_currentLevel == -1)
                        {
                            _step++;
                            break;
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            Controller.AddEffectQueue(new MoveEffect(_stars[i], new Vector2(_stars[i].Rect.X, 344), 500));
                        }
                        _step++;
                    }
                    break;
                case 5:
                    
                    _botHandLeftMoveEffect.Update();
                    _botHandRightMoveEffect.Update();
                    _botCillinderMoveEffect.Update();
                    if (_botCillinderMoveEffect.Finished && _botHandLeftMoveEffect.Finished &&
                        _botHandRightMoveEffect.Finished)
                    {
                        _step++;
                        if (_currentLevel == -1)
                        {
                            _newHighScoreColorEffect = new ColorEffect(_newHighScoreTextRegion, new Color(0, 0, 0, 255),
                                200);
                            _newHighScoreValueColorEffect = new ColorEffect(_newHighScoreValueTextRegion,
                                new Color(0, 0, 0, 255), 200);
                        }
                    }
                    break;
                case 6:
                    if (_currentLevel == -1)
                    {
                        _newHighScoreColorEffect.Update();
                        _newHighScoreValueColorEffect.Update();
                        if (_newHighScoreColorEffect.Finished)
                            _step++;
                        break;
                    }
                    if (_starsToFill > _currentStarsFill && !_fillNextStar)
                    {
                        _starsColorEffect = new ColorEffect(_starShadows[_currentStarsFill], Color.Transparent, 500);
                        _starsResizeEffect = new ResizeEffect(_starShadows[_currentStarsFill], new Vector2(120, 120), 500);
                        _stars[_currentStarsFill].LoadTexture(_starFilledTexture);
                        _starShadows[_currentStarsFill].IsHidden = false;
                        _fillNextStar = true;
                    }
                    else if (!_fillNextStar)
                    {
                        _step++;
                    }
                    else
                    {
                        _starsColorEffect.Update();
                        _starsResizeEffect.Update();
                        if (_starsColorEffect.Finished)
                        {
                            _currentStarsFill++;
                            _fillNextStar = false;
                        }
                    }
                    break;
                case 7:
                    if (_newAchievements.Count > 0)
                    {
                        var achievement = _newAchievements[0];
                        _newAchievements.Remove(achievement);
                        EngineGlobals.ScreenManager.QueueShowPopup(new AchievementScreen(achievement));
                        return;
                    }
                    _buttonReplay.IsHidden = false;
                    _buttonMenu.IsHidden = false;
                    if (_currentLevel != -1 /*&& _currentLevel != 29*/)
                    {
                        Controller.AddEffectQueue(new ColorEffect(_buttonContinue, Color.White, 300));
                        _buttonContinue.IsHidden = false;
                    }
                    Controller.AddEffectQueue(new ColorEffect(_buttonMenu, Color.White, 300));
                    Controller.AddEffectQueue(new ColorEffect(_buttonReplay, Color.White, 300));
                    _step++;
                    break;
            }

            if (_resizeEffect != null)
            {
                _resizeEffect.Update();
                _colorEffect.Update();
            }

            if (_newHighScore && _step > 6)
            {
                if (_newHighScoreFadeOut.Finished)
                {
                    _newHighScoreFadeIn.Update();
                    if (_newHighScoreFadeIn.Finished)
                    {
                        _newHighScoreFadeOut.Reset(new Color(0, 0, 0, 0), 500);
                    }
                }
                else
                {
                    _newHighScoreFadeOut.Update();
                    if (_newHighScoreFadeOut.Finished)
                        _newHighScoreFadeIn.Reset(new Color(0, 0, 0, 255), 500);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();
            WindowOnInput -= OnWindowOnInput;
            WindowOnBack -= OnWindowOnBack;
            Controller.ClearObjectsBuffers();
           // Controller.RemoveObject(_shadowImg);
        }
    }
}
