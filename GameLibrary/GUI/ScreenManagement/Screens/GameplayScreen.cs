using System;
using Engine.Core;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    public class GameplayScreen : GameScreen
    {
        private readonly GameManager _gameManager;
        protected string _mapFile;

        public GameplayScreen(string mapFile)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            _mapFile = mapFile;
            _gameManager = new GameManager();
            
        }

        public override void LoadContent()
        {
            _gameManager.Initialize();
            EngineGlobals.Content = new ContentManager(ScreenManager.Game.Services, "GameContent");
            _gameManager.Load(_mapFile);
            Controller.CurrentGame.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _gameManager.OnExit();
            GC.Collect();
        }

        public override void  InputOnBackPressed()
{
            var pauseScreen = new PauseMenu
            {
                TitlePosition = new Vector2(350, 105)
            };
            LoadingScreen.Load(
                false,
                true,
                pauseScreen);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                _gameManager.Update(gameTime);
            }
        }

        //public override void  InputOnPress(Point point, int id)
        //{
        //}

        public override void Draw(GameTime gameTime)
        {
            _gameManager.Draw();
        }
    }
}
