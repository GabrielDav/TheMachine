using Engine.Core;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class BaseLoadingScreen : BaseFadeScreen
    {
        protected IScreen _redirect;

        public BaseLoadingScreen(IScreen nextScreen)
        {
            FadeInTime = 100;
            FadeOutTime = 100;
            _redirect = nextScreen;
        }

        public override void Initialize()
        {
            base.Initialize();
            LoadNextScreenInstantly();
        }

        protected virtual void LoadNextScreenInstantly()
        {
            _redirect.Load();
            EngineGlobals.ScreenManager.QueueChangeScreen(_redirect, true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
        }
    }
}
