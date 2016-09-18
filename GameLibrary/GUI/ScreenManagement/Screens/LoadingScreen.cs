using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core;
using Engine.ScreenManagement;
using GameLibrary.Gui.ScreenManagement.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Gui
{
    public class LoadingScreen : GameScreen
    {
        private bool _loadingIsSlow;
        private bool _otherScreensAreGone;
        private static bool _isPopup;

        private GameScreen[] _screensToLoad;

        public LoadingScreen(bool loadingIsSlow, GameScreen[] screensToLoad)
        {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad;

            //Do not serialize this screen
            //because will never get to loading with back button
            IsSerializable = false;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public static void Load(
            bool loadingIsSlow,
            bool popup,
            params GameScreen[] screensToLoad)
        {
            _isPopup = popup;
            // Tell all the current screens to transition off unless it's menu popup
            if (!popup)
            {
                foreach (GameScreen screen in Controller.ScreenManager.GetScreens())
                {
                    screen.ExitScreen();
                }
            }

            // Create and activate the loading screen.
            var loadingScreen = new LoadingScreen(loadingIsSlow, screensToLoad);

            Controller.ScreenManager.AddScreen(loadingScreen);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                     bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (_otherScreensAreGone || _isPopup)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in _screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                _otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (_loadingIsSlow)
            {
                //const string message = "Loading...";

                //// Center the text in the viewport.
                //Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                //var viewportSize = new Vector2(viewport.Width, viewport.Height);
                //Vector2 textSize = GameGlobals.MenuGlobals.MenuFont.MeasureString(message);
                //Vector2 textPosition = (viewportSize - textSize)/2;

                //Color color = Color.White*TransitionAlpha;

                //// Draw the text.
                //EngineGlobals.Batch.Begin();
                //EngineGlobals.Batch.DrawString(GameGlobals.MenuGlobals.MenuFont, message, textPosition, color);
                //EngineGlobals.Batch.End();
                EngineGlobals.Batch.Begin();
                EngineGlobals.Batch.DrawString(EngineGlobals.Resources.Fonts["GameFont"], "Loading...", new Vector2(2, 2),
                                               Color.Black);
                EngineGlobals.Batch.End();
            }
        }
        
    }
}
