using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class MessageBox : IScreen
    {
        public IScreen Parent { get; set; }
        public List<IScreen> ChildScreens { get; set; }
        public bool IsPopup { get; set; }
        public ScreenState State { get; set; }

        protected Image _background;
        protected ColorEffect _continueFadeOut;
        protected ColorEffect _continueFadeIn;
        protected Timer _timer;
        protected TextRegion _continuTextRegion;
        protected TextRegion _textRegion;
        protected TextRegion _captionRegion;
        protected string _text;
        protected string _caption;
        protected int _width;
        protected int _height;
        protected int _waitTime;

        public MessageBox(string caption, string text)
        {
            Create(caption, text, false, 1000);
        }

        public MessageBox(string caption, string text, bool small)
        {
            Create(caption, text, small, 1000);
        }

        public MessageBox(string caption, string text, bool small, int waitTime)
        {
            Create(caption, text, small, waitTime);
        }

        public virtual void Create(string caption, string text, bool small, int waitTime)
        {
            _text = text;
            _caption = caption;
            if (!small)
            {
                _width = 550;
                _height = 220;
            }
            else
            {
                _width = 400;
                _height = 200;
            }
            _waitTime = waitTime;
        }

        public virtual void Initialize()
        {
            _continuTextRegion.Color = Color.Black;
            _continueFadeOut = new ColorEffect(_continuTextRegion, new Color(0, 0, 0, 0), 1000);
            _continuTextRegion.Color = new Color(0, 0, 0, 0);
            _continueFadeIn = new ColorEffect(_continuTextRegion, new Color(0, 0, 0, 255), 1000);
            if (_waitTime > 1)
                _timer.Start(_waitTime);
        }

        public virtual void Load()
        {
            _background = new Image(new GameTexture("GUI\\AchievementScreen"), new Rectangle(EngineGlobals.Viewport.Width / 2 - _width / 2, EngineGlobals.Viewport.Height / 2 - _height / 2, _width, _height)) { LayerDepth = 0.8f, Owner = this };
            _captionRegion =
                new TextRegion(new Rectangle((int)_background.X, (int)_background.Y + 5, _background.Width, 25),
                    GameGlobals.MenuGlobals.MenuFont, Color.Black, _caption, false)
                {
                    Owner = this,
                    LayerDepth = 0.5f,
                    HorizontaAlign = FontHorizontalAlign.Center
                };
            _textRegion =
                new TextRegion(new Rectangle((int)_background.X + 5, (int)_background.Y + 50, _background.Width - 20, _background.Height),
                    GameGlobals.Font, Color.Black, _text, true)
                {
                    LayerDepth = 0.5f,
                    Owner = this
                };
            _continuTextRegion = new TextRegion(
                    new Rectangle((int)_background.X,
                        (int)_background.Y - 45 + _background.Height, _background.Width, 25), GameGlobals.MenuGlobals.MenuFont,
                    new Color(0, 0, 0, 0), "Continue", false) { Owner = this, LayerDepth = 0.5f, HorizontaAlign = FontHorizontalAlign.Center };
            _timer = new Timer();
        }

        public void Dispose()
        {
            
        }

        public  virtual void HandleTouch(Point p, object sender)
        {
            if (_waitTime > 1 && !_timer.Finished)
                return;
            EngineGlobals.ScreenManager.QueueClosePopup();
        }

        public void HandleBack(object sender)
        {
            if (_waitTime > 1 && !_timer.Finished)
                return;
            EngineGlobals.ScreenManager.QueueClosePopup();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_waitTime > 1 && !_timer.Finished)
            {
                _timer.Update();
                return;
            }
            
            if (_continueFadeOut.Finished)
            {
                _continueFadeIn.Update();
                if (_continueFadeIn.Finished)
                {
                    _continueFadeOut.Reset(new Color(0, 0, 0, 0), 500);
                }
            }
            else
            {
                _continueFadeOut.Update();
                if (_continueFadeOut.Finished)
                    _continueFadeIn.Reset(new Color(0, 0, 0, 255), 500);
            }
            
        }

        public virtual void Draw(GameTime gameTime)
        {
            EngineGlobals.Batch.Begin();
            _background.Draw();
            _textRegion.Draw();
            _captionRegion.Draw();
            _continuTextRegion.Draw();
            EngineGlobals.Batch.End();
        }
    }
}
