using System;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    public class PauseMenu : MenuScreen
    {
        public Rectangle BackgroundRectangle = new Rectangle(175, 127, 450, 225);
        private Texture2D _gradientTexture;
        private readonly MenuEntry _continue;
        private readonly MenuEntry _mainMenu;

        public PauseMenu()
        {
            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            _continue = new MenuEntry("", null, new Rectangle(470, 270, 150, 50), null);
            _mainMenu = new MenuEntry("", null, new Rectangle(215, 172, 100, 50), null);

            _continue.Selected += Continue;
            _mainMenu.Selected += MainMenu;

            MenuEntries.Add(_continue);
            MenuEntries.Add(_mainMenu);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            _gradientTexture = content.Load<Texture2D>("GUI\\PauseScreen");
        }

         void Continue(object sender, EventArgs e)
         {
             ExitScreen();
         }

        void MainMenu(object sender, EventArgs e)
        {
            LoadingScreen.Load(false, false, new MainMenuScreen());
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Color color = Color.White * TransitionAlpha;

            EngineGlobals.Batch.Begin();

            EngineGlobals.Batch.Draw(_gradientTexture, BackgroundRectangle, color);

            EngineGlobals.Batch.End();

            base.Draw(gameTime);
        }
    }
}
