using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class PauseScreen : IScreen
    {
        public IScreen Parent { get; set; }
        public List<IScreen> ChildScreens { get; set; }
        public bool IsPopup { get; set; }
        public ScreenState State { get; set; }
        protected Image _background;
        protected Rectangle _btnResume;
        protected Rectangle _btnMainMenu;
        protected Rectangle _btnRestart;

        protected Image _shadowImg;
        protected ResizeEffect _resizeEffect;
        protected ColorEffect _colorEffect;
        protected Timer _finalCountDownTimer;
        protected TextRegion _countDownTextRegion;
        protected TextRegion _shadowTextRegion;
        protected ScaleEffect _countDownRescaleEffect;
        protected ColorEffect _countDownColorEffect;
        protected Image _countDownBox;
        protected int _countDown;

        public void Initialize()
        {
            _btnRestart = new Rectangle(238, 234, 73, 73);
            _btnMainMenu = new Rectangle(368, 234, 73, 73);
            _btnResume = new Rectangle(490, 234, 73, 73);
            MusicManager.Pause();
            _finalCountDownTimer = new Timer();
            _countDownTextRegion = new TextRegion(new Rectangle(375, 225, 50, 50), GameGlobals.MenuGlobals.MenuFont, "3",
                false)
            {
                HorizontaAlign = FontHorizontalAlign.Center,
                VerticalAlign = FontVerticalAlign.Center,
                StaticPosition = true,
                Scale = new Vector2(1.5f, 1.5f),
                Color = Color.Black,
                LayerDepth = 0.4f
            };
            _shadowTextRegion = new TextRegion(_countDownTextRegion.Rect, GameGlobals.MenuGlobals.MenuFont, "3", false)
            {
                //HorizontaAlign = FontHorizontalAlign.Center,
                //VerticalAlign = FontVerticalAlign.Center,
                StaticPosition = true,
               // Scale = new Vector2(1.5f, 1.5f),
                Color = Color.Black,
                LayerDepth = 0.4f
            };
            _countDownRescaleEffect = new ScaleEffect(_shadowTextRegion, new Vector2(2.5f, 2.5f), 500);
            _countDownColorEffect = new ColorEffect(_shadowTextRegion, new Color(0, 0, 0, 0), 500);
        }

        public void Load()
        {
            _background = new Image(new GameTexture("GUI\\PauseScreen"), new Rectangle(200, 120, 400, 240));
            _countDownBox = new Image(new GameTexture("GUI\\box")) { Rect = new Rectangle(360, 210, 80, 80), LayerDepth = 0.5f};
        }

        public void Dispose()
        {
            _countDownTextRegion.Dispose();
            _background.Dispose();
        }

        public void HandleTouch(Point p, object sender)
        {
            if (_btnMainMenu.Contains(p))
            {
                EngineGlobals.ScreenManager.QueueClosePopup();
                ((BaseMainScreen) Parent).Freez = true;
                ((GameScreen) Parent).ExitToMainMenu();
            }
            else if (_btnResume.Contains(p))
            {
              //  EngineGlobals.ScreenManager.QueueClosePopup();
              //  MusicManager.Unpause();
                _countDown = 3;
                DoTick();
            }
            else if (_btnRestart.Contains(p))
            {
                EngineGlobals.ScreenManager.QueueClosePopup();
                ((BaseMainScreen)Parent).Freez = true;
                GameGlobals.Game.QueueReset();
                MusicManager.Unpause();
            }
        }

        public void HandleBack(object sender)
        {
            //MusicManager.Unpause();
            //EngineGlobals.ScreenManager.QueueClosePopup();
            if (_finalCountDownTimer.Started)
            {
                _finalCountDownTimer.Stop();
            }
            else
            {
                _countDown = 3;
                DoTick();
            }
        }

        protected void DoTick()
        {
            _shadowTextRegion.Text = _countDown.ToString();
            _shadowTextRegion.Rect = _countDownTextRegion.Rect;
            _shadowTextRegion.Scale = _countDownTextRegion.Scale;
            _shadowTextRegion.Color = new Color(0, 0, 0, 255);
            _countDownRescaleEffect.Reset(new Vector2(3.5f, 3.5f), 500, _shadowTextRegion.Rect);;
            _countDownColorEffect.Reset(new Color(0, 0, 0, 0), 500);
            EngineGlobals.SoundManager.Play("tick", "tick", 0.7f, false, 0.0f, 0.0f);
            _countDownTextRegion.Text = _countDown.ToString();
            _finalCountDownTimer.Start(1000);
        }

        public void Update(GameTime gameTime)
        {
#if WINDOWS_PHONE
            GameGlobals.AdControl.Update();
#endif
            if (_resizeEffect != null)
            {
                _resizeEffect.Update();
                _colorEffect.Update();
            }
            if (_finalCountDownTimer.Started)
            {
                _finalCountDownTimer.Update();
                if (_finalCountDownTimer.Finished)
                {
                    _countDown--;
                    
                    if (_countDown < 1)
                    {
                        EngineGlobals.SoundManager.Play("tick", "tick", 0.7f, false, 0.0f, 0.0f);
                        EngineGlobals.ScreenManager.QueueClosePopup();
                        MusicManager.Unpause();
                        return;
                    }
                    else
                    {
                        DoTick();
                    }
                }
                _countDownColorEffect.Update();
                _countDownRescaleEffect.Update();
            }
        }

        public void Draw(GameTime gameTime)
        {
            EngineGlobals.Batch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            if (!_finalCountDownTimer.Started && (!_finalCountDownTimer.Finished || _finalCountDownTimer.Stoped))
            {
                _background.Draw();
                if (_shadowImg != null)
                    _shadowImg.Draw();
            }
            else
            {
                _countDownBox.Draw();
                _countDownTextRegion.Draw();
                _shadowTextRegion.Draw();
            }
            EngineGlobals.Batch.End();
        }
    }
}
