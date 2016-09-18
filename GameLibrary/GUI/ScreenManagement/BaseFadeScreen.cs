using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement
{
    public abstract class BaseFadeScreen : IScreen
    {

        private ColorEffect _fadeInEffect;
        private ColorEffect _fadeOutEffect;
        private GameTexture _fadeTexture;
        private Image _fadeImage;
        private bool _fadingIn;
        private bool _fadigOut;
        private int count = 0;
        protected bool _isFading { get { return _fadingIn || _fadigOut; }}
        protected event PointEvent WindowOnInput;
        protected event SimpleEvent WindowOnBack;

        public int FadeInTime{ get; protected set; }

        public int FadeOutTime { get; protected set; }

        public IScreen Parent { get; set; }
        public List<IScreen> ChildScreens { get; set; }
        public bool IsPopup { get; set; }

        public ScreenState State { get; set; }

        public bool InputDisabled { get; set; }

        public virtual void Load()
        {
            _fadeTexture = new GameTexture("FadeTexture");
            _fadeImage = new Image(_fadeTexture, new Rectangle(0,0, 800, 480)) {StaticPosition = true, LayerDepth = 0.1f, Color = new Color(255, 255, 255, 255), Owner = this};
        }

        public virtual void Initialize()
        {
            Controller.AddObject(_fadeImage);
            InputDisabled = false;
            FadeInTime = 1000;
            FadeOutTime = 1000;
        }

        public virtual void Dispose()
        {
            Controller.RemoveObject(_fadeImage);
            _fadeImage.Dispose();
            _fadeImage = null;
            _fadeTexture = null;
        }

        public void HandleTouch(Point p, Object sender)
        {
            if (InputDisabled)
                return;
            if (_fadingIn || _fadigOut)
                return;
            if (WindowOnInput != null)
                WindowOnInput(sender, p);
        }

        public void HandleBack(object sender)
        {
            if (_fadingIn || _fadigOut)
                return;
            if (WindowOnBack != null)
                WindowOnBack(sender);
        }
        
        public virtual void Update(GameTime gameTime)
        {
            if (_fadingIn)
            {
                _fadeInEffect.Update();
                count++;
                if (_fadeInEffect.Finished)
                {
                    State = ScreenState.Active;
                    _fadeInEffect = null;
                    _fadingIn = false;
                    _fadeImage.IsHidden = true;
                    FadeInCompleted();
                    
                }
            }
            else if (_fadigOut)
            {
                _fadeOutEffect.Update();
                if (_fadeOutEffect.Finished)
                {
                   // State = ScreenState.InActive;
                    _fadeOutEffect = null;
                    FadeOutCompleted();
                    _fadigOut = false;
                   // _fadeImage.IsHidden = true;
                }
            }
        }

        protected virtual void FadeInCompleted()
        {
            
        }

        protected virtual void FadeOutCompleted()
        {
            
        }

        protected virtual void FadeOut()
        {
            _fadeOutEffect = new ColorEffect(_fadeImage, new Color(255, 255, 255, 255), FadeOutTime);
            _fadigOut = true;
            _fadeImage.IsHidden = false;
        }

        protected void FadeIn()
        {
            _fadeInEffect = new ColorEffect(_fadeImage, new Color(255, 255, 255, 0), FadeInTime);
            _fadingIn = true;
            _fadeImage.IsHidden = false;
        }

        public abstract void Draw(GameTime gameTime);
    }
}
