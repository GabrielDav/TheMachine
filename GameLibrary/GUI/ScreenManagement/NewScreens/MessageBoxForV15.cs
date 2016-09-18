using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using TheGoo;
#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
#endif

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class MessageBoxForV15 : MessageBox
    {
        //protected Image _linkImage;
        protected TextRegion _adFreeButton;
        protected ColorEffect _adFreeFadeOut;
        protected ColorEffect _adFreeFadeIn;

        public MessageBoxForV15(string caption, string text) : base(caption, text)
        {
        }

        public MessageBoxForV15(string caption, string text, bool small) : base(caption, text, small)
        {
        }

        public MessageBoxForV15(string caption, string text, bool small, int waitTime)
            : base(caption, text, small, waitTime)
        {
        }

        public override void Create(string caption, string text, bool small, int waitTime)
        {
            base.Create(caption, text, small, waitTime);
            _height -= 30;
        }

        public override void Initialize()
        {
            base.Initialize();
            _adFreeButton.Color = Color.Black;
            _adFreeFadeOut = new ColorEffect(_adFreeButton, new Color(0, 0, 0, 0), 1000);
            _adFreeButton.Color = new Color(0, 0, 0, 0);
            _adFreeFadeIn = new ColorEffect(_adFreeButton, new Color(0, 0, 0, 255), 1000);
        }

        public override void Load()
        {
            base.Load();
            _adFreeButton = new TextRegion(
                new Rectangle((int)_background.X+20,
                    (int)_background.Y - 45 + _background.Height, 310, 25),
                GameGlobals.MenuGlobals.MenuFont,
                new Color(0, 0, 0, 0), "Get Ad Free Version", false)
            {
                Owner = this,
                LayerDepth = 0.5f,
                HorizontaAlign = FontHorizontalAlign.Left
            };

            _continuTextRegion.HorizontaAlign = FontHorizontalAlign.Left;
            _continuTextRegion.Width = 130;
            _continuTextRegion.X += _continuTextRegion.Rect.X + 400;

        }

        public override void HandleTouch(Point p, object sender)
        {
            if (_continuTextRegion.Rect.Contains(p))
            {
                EngineGlobals.ScreenManager.QueueClosePopup();
            }
            if (_adFreeButton.Rect.Contains(p))
            {
                #if WINDOWS_PHONE
                var marketplace = new MarketplaceDetailTask()
                {
                    ContentIdentifier = GameGlobals.AdFreeAppId,
                    ContentType = MarketplaceContentType.Applications
                };
                marketplace.Show();
#endif
                EngineGlobals.ScreenManager.QueueClosePopup();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_waitTime > 1 && !_timer.Finished)
            {
                return;
            }

            if (_adFreeFadeOut.Finished)
            {
                _adFreeFadeIn.Update();
                if (_adFreeFadeIn.Finished)
                {
                    _adFreeFadeOut.Reset(new Color(0, 0, 0, 0), 500);
                }
            }
            else
            {
                _adFreeFadeOut.Update();
                if (_adFreeFadeOut.Finished)
                    _adFreeFadeIn.Reset(new Color(0, 0, 0, 255), 500);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            EngineGlobals.Batch.Begin();
            _background.Draw();
            _textRegion.Draw();
            _captionRegion.Draw();
            _continuTextRegion.Draw();
            _adFreeButton.Draw();
            EngineGlobals.Batch.End();
        }
    }
}
