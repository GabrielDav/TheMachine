using System;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics.Triggers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    public class LevelCompleteScreen : MenuScreen
    {
        private  MenuEntry _btnRight;
        private  MenuEntry _btnLeft;
        protected int _currentLevel;
        protected int _nextLevel;
        protected Image[] _stars;
        protected Image[] _starShadows;
        protected ResizeEffect[] _starsResizeEffect;
        protected ColorEffect[] _starsColorEffect;
        protected GameTexture _starEmptyTexture;
        protected GameTexture _starFilledTexture;
        protected GameTexture _progressBarBorderTexture;
        protected GameTexture _progressBarTexture;
        protected GameTexture _progressBarEndTexture;
        protected Image _progressBarBorder;
        protected Image _progressBar;
        protected Image _progressBarEnd;
        protected bool _fillingProgress;
        protected bool _fillingProgressDone;
        protected Timer _timer;
        protected float _totalFill;
        protected float _currentFill;
        protected TextRegion _highScore;
        protected TextRegion _yourScore;
        protected Map _map;
        protected int _starsFilled;
        protected int _barY;

        protected virtual string BtnLeftText
        {
            get { return "Replay"; }
        }

        protected virtual string BtnRightText
        {
            get { return "Continue"; }
        }

        protected virtual int HighScore
        {
            get { return GameGlobals.SaveData.Scores[_currentLevel]; }
        }

        protected virtual bool DrawStars
        {
            get { return true; }
        }
        //protected bool _queueLoadLevel;

        public LevelCompleteScreen(int currentLevel)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
            _currentLevel = currentLevel;
            _nextLevel = _currentLevel + 1;
            
        }

        protected void SetProgress(float progress, Point point, int maxWidth)
        {
            var newWidth = (int)((maxWidth - _progressBarEnd.Rect.Height)*progress/100f);
            if (newWidth % 2 != 0)
                newWidth--;
            _progressBar.Rect = new Rectangle(point.X + newWidth/2, point.Y + _progressBar.Rect.Width / 2, _progressBar.Width, newWidth);
            _progressBarEnd.Rect = new Rectangle(point.X + newWidth + _progressBarEnd.Rect.Height/2, point.Y + _progressBar.Rect.Width / 2, _progressBarEnd.Width, _progressBarEnd.Height);
           // _yourScore.Text = "Your Score: " + (GameGlobals.Score*progress/100f).ToString("0");
        }

        protected virtual void NewHighScore(int score)
        {
            GameGlobals.SaveData.Scores[_currentLevel] = score;
            SaveData.Save();
        }

        public override void LoadContent()
        {
            GameGlobals.LevelComplete = false;
            
            _progressBarBorderTexture = new GameTexture("GameObjects\\Images\\Border");
            _progressBarTexture = new GameTexture("GameObjects\\Images\\Bar2");
            _progressBarEndTexture = new GameTexture("GameObjects\\Images\\BarEnd");

            _barY = 270;
            var totalScoreY = 175;
            var currentScoreY = 205;
            
            if (DrawStars)
            {
                _barY = 200;
                totalScoreY = 105;
                currentScoreY = 135;

                _starEmptyTexture = new GameTexture("GUI\\Menu\\Star_Empty");
                _starFilledTexture = new GameTexture("GUI\\Menu\\Star_Filled");

                _starShadows = new Image[3];
                _starShadows[0] = new Image(_starFilledTexture, new Rectangle(310, 280, 60, 60), new Color(255, 255, 255, 150)) {IsHidden = true};
                _starShadows[1] = new Image(_starFilledTexture, new Rectangle(400, 280, 60, 60), new Color(255, 255, 255, 150)) {IsHidden = true};
                _starShadows[2] = new Image(_starFilledTexture, new Rectangle(490, 280, 60, 60), new Color(255, 255, 255, 150)) {IsHidden = true};
                _starsResizeEffect = new ResizeEffect[3];
                _starsColorEffect = new ColorEffect[3];


                _stars = new Image[3];
                _stars[0] = new Image(_starEmptyTexture, new Rectangle(310, 280, 60, 60));
                _stars[1] = new Image(_starEmptyTexture, new Rectangle(400, 280, 60, 60));
                _stars[2] = new Image(_starEmptyTexture, new Rectangle(490, 280, 60, 60));
                for (var i = 0; i < 3; i++)
                {
                    _stars[i].Orgin = _stars[i].OriginCenter();
                    _objects.Add(_stars[i]);
                    _starShadows[i].Orgin = _starShadows[i].OriginCenter();
                    _objects.Add(_starShadows[i]);
                }
            }

            

            _progressBarBorder = new Image(_progressBarBorderTexture);
            _progressBarBorder.StaticPosition = true;
            _progressBarBorder.Orgin = _progressBarBorder.OriginCenter();
            _progressBarBorder.Rotation = MathHelper.ToRadians(270);
            _progressBarBorder.Rect = new Rectangle(420, _barY, 30, 360);
            _objects.Add(_progressBarBorder);

            _progressBar = new Image(_progressBarTexture);
            _progressBar.Orgin = _progressBar.Center();
            _progressBar.Rotation = MathHelper.ToRadians(270);
            _progressBar.Rect = new Rectangle(390, _barY, 26, 260);
            _objects.Add(_progressBar);

            _progressBarEnd = new Image(_progressBarEndTexture);
            _progressBarEnd.Orgin = _progressBarEnd.Center();
            _progressBarEnd.Rotation = MathHelper.ToRadians(270);
            _progressBarEnd.Rect = new Rectangle(300, _barY, 26, 10);
            _objects.Add(_progressBarEnd);

            _map = GameGlobals.Map = (Map)Map.Load("Maps\\ScoreScreen").Clone();
            foreach (var physicalObject in _map.GameObjects)
            {
                physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
            }
           EngineGlobals.Camera2D = new Camera2D();
            EngineGlobals.TriggerManager = new TriggerManager(_map.Triggers, _map.GameObjects);
            EngineGlobals.TriggerManager.ActionOccured((int)EventType.GameInitialization, new EventParams());
            _timer = new Timer();
            /*_totalFill = GameGlobals.Score * 100f / HighScore;
            if (_totalFill > 100f)*/
                _totalFill = 90f;
            _btnLeft = new MenuEntry(BtnLeftText, null, new Rectangle(30, 410, 150, 50), Color.Black);
            _btnRight = new MenuEntry(BtnRightText, null, new Rectangle(650, 410, 100, 50), Color.Black);
            _highScore = new TextRegion(new Rectangle(280, 120, 300, 20), GameGlobals.MenuGlobals.MenuFont, Color.Black, "Highscore: " + HighScore, false);
            _yourScore = new TextRegion(new Rectangle(280, 150, 300, 20), GameGlobals.MenuGlobals.MenuFont, Color.Black, "Your Score: " + GameGlobals.ScoreTotal, false);
            if (GameGlobals.ScoreTotal > HighScore)
            {
                NewHighScore(GameGlobals.ScoreTotal);
            }
            _btnRight.Selected += OnBtnRightPressed;
            _btnLeft.Selected += OnBtnLeftPressed;

            MenuEntries.Add(_btnRight);
            MenuEntries.Add(_btnLeft);
            SetProgress(0, new Point(260, _barY - 13), 254);
            base.LoadContent();
        }

        protected virtual void OnBtnRightPressed(object sender, EventArgs eventArgs)
        {
            LoadingScreen.Load(true, false, new GameplayScreen(GameGlobals.MapList.GameMaps[_nextLevel]));
            GameGlobals.GameOver = false;
            GameGlobals.Score = 0;
            GameGlobals.MaxHeight = 0;
        }

        protected virtual void OnBtnLeftPressed(object sender, EventArgs eventArgs)
        {
            LoadingScreen.Load(true, false, new GameplayScreen(GameGlobals.MapList.GameMaps[_currentLevel]));
            GameGlobals.GameOver = false;
            GameGlobals.Score = 0;
            GameGlobals.MaxHeight = 0;
        }



        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (_fillingProgress)
            {
                _currentFill += 2f;
                SetProgress(_currentFill, new Point(260, _barY - 13), 254);
                if (_currentFill >= _totalFill)
                {
                    _fillingProgressDone = true;
                    _fillingProgress = false;
                }
                if (DrawStars)
                {
                    if (_currentFill > 50 && _starsFilled < 1)
                    {
                        _stars[0].LoadTexture(_starFilledTexture);
                        _starShadows[0].IsHidden = false;
                        _starShadows[0].Rotation = _stars[0].Rotation;
                        _starsResizeEffect[0] = new ResizeEffect(_starShadows[0], new Vector2(130, 130), 1000);
                        _starsColorEffect[0] = new ColorEffect(_starShadows[0], new Color(255, 255, 255, 0), 1000);
                        _starsFilled = 1;
                    }
                    if (_currentFill > 70 && _starsFilled < 2)
                    {
                        _stars[1].LoadTexture(_starFilledTexture);
                        _starShadows[1].IsHidden = false;
                        _starShadows[1].Rotation = _stars[1].Rotation;
                        _starsResizeEffect[1] = new ResizeEffect(_starShadows[1], new Vector2(130, 130), 1000);
                        _starsColorEffect[1] = new ColorEffect(_starShadows[1], new Color(255, 255, 255, 0), 1000);
                        _starsFilled = 2;
                    }
                    if (_currentFill > 99 && _starsFilled < 3)
                    {
                        _stars[2].LoadTexture(_starFilledTexture);
                        _starShadows[2].IsHidden = false;
                        _starShadows[2].Rotation = _stars[2].Rotation;
                        _starsResizeEffect[2] = new ResizeEffect(_starShadows[2], new Vector2(130, 130), 1000);
                        _starsColorEffect[2] = new ColorEffect(_starShadows[2], new Color(255, 255, 255, 0), 1000);
                        _starsFilled = 3;
                    }
                }
                /*_timer.Update();
                if (_timer.Finished)
                {
                    _currentFill+=2f;
                    SetProgress(_currentFill, new Point(260, 187), 254);
                    if (_currentFill >= _totalFill)
                    {
                        _fillingProgressDone = true;
                        _fillingProgress = false;
                    }
                    else
                    {
                        _timer.Restart();
                    }
                }*/
                
                
            }
            else if (!_fillingProgressDone && Math.Abs(TransitionAlpha - 1f) < EngineGlobals.Epsilon)
            {
                _fillingProgress = true;
               // _timer.Start(1, false);
            }
            
            foreach (var physicalObject in _map.GameObjects)
            {
                physicalObject.Update();
            }

            if (DrawStars)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!_starShadows[i].IsHidden)
                    {
                        if (!_starsResizeEffect[i].Finished)
                            _starsResizeEffect[i].Update();
                        if (!_starsColorEffect[i].Finished)
                        {
                            _starsColorEffect[i].Update();
                        }
                        else
                        {
                            _starShadows[i].IsHidden = true;
                        }

                    }

                }
                for (int i = 0; i < 3; i++)
                {
                    _stars[i].Rotation += (1f/1000*EngineGlobals.GameTime.ElapsedGameTime.Milliseconds*
                                           EngineGlobals.GameSpeed);
                    if (_stars[i].Rotation >= MathHelper.Pi*2f)
                    {
                        _stars[i].Rotation = _stars[i].Rotation - MathHelper.Pi*2f;
                    }
                }
            }

        }

        public override void ExitScreen()
        {
            if (DrawStars)
            {
                foreach (var star in _stars)
                {
                    star.IsHidden = true;
                }
            }
            _progressBarBorder.IsHidden = true;
            _progressBar.IsHidden = true;
            _progressBarEnd.IsHidden = true;
            _highScore.IsHidden = true;
            _yourScore.IsHidden = true;
            base.ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            EngineGlobals.Batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null,
                                      EngineGlobals.Camera2D.GetTransformation());
         //   _highScore.Draw();
       //     _yourScore.Draw();
         /*   foreach (var star in _starts)
            {
                star.Draw();
            }*/
          //  _progressBarBorder.Draw();
           // _progressBar.Draw();
          //  _progressBarEnd.Draw();
            foreach (var physicalObject in _map.GameObjects)
            {
                physicalObject.Draw();
            }
            EngineGlobals.Batch.End();
            
        }
    }
}
