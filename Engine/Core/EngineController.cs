using System;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Core
{
    public static class EngineController
    {
        private static Game _game;

        public static  void Init(Game game)
        {
            EngineGlobals.Graphics = new GraphicsDeviceManager(game);
            game.Content.RootDirectory = "Content";
            game.IsFixedTimeStep = false;
            game.TargetElapsedTime = TimeSpan.FromTicks(333333);
            game.InactiveSleepTime = TimeSpan.FromSeconds(1);
            _game = game;
        }

        public static void SetContent()
        {
            EngineGlobals.Device = _game.GraphicsDevice;
            EngineGlobals.Batch = new SpriteBatch(_game.GraphicsDevice);
            EngineGlobals.Content = _game.Content;
            //EngineGlobals.Camera = new Camera(new Point(0, 0));
            EngineGlobals.Camera2D = new Camera2D();
            EngineGlobals.Input = new Input();
            EngineGlobals.Storage = new StorageControl();

#if WINDOWS
            _game.IsMouseVisible = true;
            EngineGlobals.Graphics.PreferredBackBufferWidth = 800;
            EngineGlobals.Graphics.PreferredBackBufferHeight = 480;
            EngineGlobals.Graphics.ApplyChanges();
#endif

        }

    }
}
