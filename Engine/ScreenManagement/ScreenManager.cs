using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.ScreenManagement
{
    public class ScreenManager : DrawableGameComponent
    {
        protected Game _game;
        protected IScreen _rootScreen;
        public List<IScreen> Screens { get; protected set; }
        protected IScreen _pendingScreen;
        protected IScreen _pendingPopup;
        public IScreen CurrentScreen { get; protected set; }
        protected bool _pendingClosePopup;
        protected bool _preloadedScreen;

        public ScreenManager(Game game) : base(game)
        {
            _game = game;
            Screens = new List<IScreen>();
        }
        
        /// <remarks>Initlize base global game data</remarks>
        public override void Initialize()
        {
            EngineGlobals.ContentCache = Game.Content;
            EngineGlobals.ContentCache.RootDirectory = "GameContent";
            


            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("blank", new[] {"blank"}, ResourceType.Texture));
            EngineGlobals.Resources.LoadFont(new ResourceIdentifier("GameFont", new[] {"GameFont"}, ResourceType.Font));

            EngineGlobals.Input.OnPress += InputOnOnPress;
            EngineGlobals.Input.BackButtonPress += InputOnBackButtonPress;
            EngineGlobals.SoundManager = new SoundManager();
        }

        private void InputOnBackButtonPress()
        {
            foreach (var screen in Screens)
            {
                if (screen.State == ScreenState.Active)
                    screen.HandleBack(this);
            }
        }

        protected void InputOnOnPress(Point point, int inputId)
        {
            foreach (var screen in Screens)
            {
                if (screen.State == ScreenState.Active)
                    screen.HandleTouch(point, this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            EngineGlobals.Device.Clear(Color.White);
            foreach (var screen in Screens)
            {
                if (screen.State != ScreenState.Hidden)
                    screen.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            EngineGlobals.GameTime = gameTime;
            EngineGlobals.Input.Update();
            foreach (var screen in Screens)
            {
                if (screen.State == ScreenState.Active)
                    screen.Update(gameTime);
            }
            if (_pendingScreen != null)
            {
                if (_rootScreen != null)
                {
                    foreach (var childScreen in _rootScreen.ChildScreens)
                    {
                        childScreen.Dispose();
                        Screens.Remove(childScreen);
                    }
                    _rootScreen.ChildScreens.Clear();
                    _rootScreen.Dispose();
                    Screens.Remove(_rootScreen);
                }
                _rootScreen = _pendingScreen;
                _rootScreen.ChildScreens = new List<IScreen>();
                Screens.Add(_pendingScreen);
                _pendingScreen.IsPopup = false;
                if (!_preloadedScreen)
                    _pendingScreen.Load();
                else
                    _preloadedScreen = false;
                CurrentScreen = _rootScreen;
                _pendingScreen = null;
                _rootScreen.Initialize();
                
                
            }
            if (_pendingPopup != null)
            {
                foreach (var childScreen in _rootScreen.ChildScreens)
                {
                    childScreen.State = ScreenState.InActive;
                }
                _rootScreen.State = ScreenState.InActive;
                _rootScreen.ChildScreens.Add(_pendingPopup);
                Screens.Add(_pendingPopup);
                _pendingPopup.ChildScreens = new List<IScreen>();
                _pendingPopup.State = ScreenState.Active;
                _pendingPopup.IsPopup = true;
                _pendingPopup.Parent = _rootScreen;
                _pendingPopup.Load();
                _pendingPopup.Initialize();
                CurrentScreen = _pendingPopup;
                _pendingPopup = null;
                

            }
            if (_pendingClosePopup)
            {
                _rootScreen.State = ScreenState.Active;
                
                if (!CurrentScreen.IsPopup)
                    throw new Exception("Current active window is not popup");
                CurrentScreen.Dispose();
                _rootScreen.ChildScreens.Remove(CurrentScreen);
                Screens.Remove(CurrentScreen);
                CurrentScreen = _rootScreen;
                _pendingClosePopup = false;
            }
            EngineGlobals.SoundManager.Update(EngineGlobals.Camera2D != null ? EngineGlobals.Camera2D.Position : new Vector2(0, 0));
        }

        protected void CheckScreenOperaionQueue()
        {
            if (_pendingScreen != null)
                throw new Exception("Another screen alredy queued (" + _pendingScreen.GetType() + ")");
            if (_pendingPopup != null)
                throw new Exception("Another screen popup operation already queued (" + _pendingPopup.GetType() + ")");
            if (_pendingClosePopup)
                throw new Exception("Close popup operation already queued");
        }

        public void QueueChangeScreen(IScreen screen, bool preloaded)
        {
            CheckScreenOperaionQueue();
            _pendingScreen = screen;
            _preloadedScreen = preloaded;
        }

        public void QueueChangeScreen(IScreen screen)
        {
            QueueChangeScreen(screen, false);
        }

        /// <summary>
        /// Only last popup screen is active
        /// </summary>
        public void QueueShowPopup(IScreen screen)
        {
            CheckScreenOperaionQueue();
            _pendingPopup = screen;
        }

        public void QueueClosePopup()
        {
            CheckScreenOperaionQueue();
            _pendingClosePopup = true;
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            foreach (var screen in Screens)
            {
                screen.Dispose();
            }
            EngineGlobals.SoundManager.Dispose();
            MusicManager.Stop();
        }
    }
}
