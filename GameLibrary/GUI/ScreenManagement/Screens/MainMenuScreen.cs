using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    public class MainMenuScreen : MenuScreen
    {

        public MainMenuScreen()
        {
            MenuTitle = "Main Menu";
            TitlePosition = new Vector2(335, 50);
            GameGlobals.Menu = new MenuManager();
            GameGlobals.Menu.Initialize();
        }

        public override void LoadContent()
        {
            var content = Controller.CurrentGame.Content;
            
            GameGlobals.MenuGlobals.MenuFont = content.Load<SpriteFont>("MenuFont");
            GameGlobals.Menu.Load();
            //var arcadeMenuEntry = new MenuEntry(null, content.Load<Texture2D>(@"Gui/Menu/Buttons/Arcade"), new Rectangle(300, 150, 200, 50));
            //var playGameMenuEntry = new MenuEntry(null, content.Load<Texture2D>(@"Gui/Menu/Buttons/PlayGame"), new Rectangle(300, 250, 200, 50));
            //var optionsMenuEntry = new MenuEntry(null, content.Load<Texture2D>(@"Gui/Menu/Buttons/Options"), new Rectangle(300, 350, 200, 50));

            //arcadeMenuEntry.Selected += ArcadeSelected;
            //playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
           // optionsMenuEntry.Selected += OptionsMenuEntrySelected;

           // MenuEntries.Add(arcadeMenuEntry);
           // MenuEntries.Add(playGameMenuEntry);
           // MenuEntries.Add(optionsMenuEntry);

            EngineGlobals.SoundManager.PlayMusic("menu", true);
        }

        /// <summary>
        /// Event handler for when the Arcade menu entry is selected.
        /// </summary>
        void ArcadeSelected(object sender, object e)
        {
            LoadingScreen.Load(true, false, new ArcadeScreen());
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, object e)
        {
            LoadingScreen.Load(true, false, new GameplayScreen(GameGlobals.MapList.GameMaps[0]));
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, object e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen());
        }

        public override void UnloadContent()
        {
            EngineGlobals.SoundManager.StopMusic();
            base.UnloadContent();
            GameGlobals.Menu.Dispose();
            //EngineGlobals.SoundManager.Dispose();
            GameGlobals.Menu = null;
        }

        public override void Draw(GameTime gameTime)
        {
            GameGlobals.Menu.Draw();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            EngineGlobals.SoundManager.Update(EngineGlobals.Camera2D.Position);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (!IsExiting)
                GameGlobals.Menu.Update(gameTime);
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
        }
    }
}
