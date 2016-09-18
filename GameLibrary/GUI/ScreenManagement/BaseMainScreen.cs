using System;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement
{
    public abstract class BaseMainScreen : BaseFadeScreen
    {

        protected int _numOfFrames;
        protected double _fps;
        protected TimeSpan _elapsedTime = TimeSpan.Zero;
        public ContentManager AssetsManager;
        public bool Freez;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Freez)
            {
                Controller.Update();
                EngineGlobals.Camera2D.Update();
            }

            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _fps = _numOfFrames;
                _numOfFrames = 0;
            }

        }

        public override void Load()
        {
           AssetsManager = new ContentManager(EngineGlobals.ContentCache.ServiceProvider, EngineGlobals.ContentCache.RootDirectory);
            base.Load();
        }

        public override void Initialize()
        {
            base.Initialize();
            EngineGlobals.Camera2D = new Camera2D();
        }

        public override void Draw(GameTime gameTime)
        {
            _numOfFrames++;
            Controller.Draw();
        }

        public override void Dispose()
        {
            base.Dispose();
            
            Controller.ClearObjectsBuffers();
            if (EngineGlobals.Background != null)
            {
                EngineGlobals.Background.Dispose();
                EngineGlobals.Background = null;
            }
            EngineGlobals.Camera2D = null;
            EngineGlobals.SoundManager.StopAllSounds();
            AssetsManager.Dispose();
        }

    }
}
