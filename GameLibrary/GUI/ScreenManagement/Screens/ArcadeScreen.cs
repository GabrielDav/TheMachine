using System;
using Engine.Core;
using Engine.Graphics;
using Engine.ScreenManagement;
using GameLibrary.Arcade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    class ArcadeScreen : GameScreen
    {
        private ArcadeGameManager _gameManager;

        public ArcadeScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }



        public override void LoadContent()
        {
            EngineGlobals.Content = new ContentManager(ScreenManager.Game.Services, "GameContent");
            _gameManager = new ArcadeGameManager();
            _gameManager.LoadContent(GameGlobals.MapList.ArcadeMap);
            _gameManager.Initialize();
            _gameManager.SetUpData();
        }

        public override void UnloadContent()
        {
            EngineGlobals.SoundManager.StopMusic();
            EngineGlobals.SoundManager.StopAllSounds();
            base.UnloadContent();
            //EngineGlobals.Resources.DisposeResources(GameGlobals.Map.Resources);
            GameGlobals.Map.Dispose();
            Controller.ClearObjectsBuffers();
            GameGlobals.Physics.Dispose();
            ////EngineGlobals.Content.Dispose();
            GC.Collect();
        }

        public override void InputOnBackPressed()
        {
            var pauseScreen = new PauseMenu
            {
               // BackgroundRectangle = new Rectangle(175, 127, 450, 225),
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
                _gameManager.UpdateArcade(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _gameManager.Draw();
        }
    }
}
