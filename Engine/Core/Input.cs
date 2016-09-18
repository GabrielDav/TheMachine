using GameLibrary.Gui.ScreenManagement.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif


namespace Engine.Core
{

    public delegate void InputEvent(Point point, int inputId);

    public delegate void PhoneInput();

    public class Input
    {
        public const int MaxInputs = 4;

        #if WINDOWS || EDITOR
        protected MouseState _prevMouseState;
        protected KeyboardState _currentKeyboardState;
        protected KeyboardState _lastKeyboardState;
        public Point MousePos;
#else
        protected TouchCollection _touchState;
        protected GamePadState _currentGamePadState;
        protected GamePadState _lastGamePadState;
#endif

        public event InputEvent OnPress;
        public event InputEvent OnRelease;
        public event PhoneInput BackButtonPress;



        public Input()
        {
#if WINDOWS_PHONE
            _currentGamePadState = new GamePadState();
            _lastGamePadState = _currentGamePadState;
#else
            _currentKeyboardState = new KeyboardState();
            _lastKeyboardState = _currentKeyboardState;
#endif

        }

        public void Update()
        {
#if WINDOWS_PHONE
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
            if (_currentGamePadState.Buttons.Back == ButtonState.Pressed && _lastGamePadState.Buttons.Back == ButtonState.Released)
            {
                if (BackButtonPress != null)
                    BackButtonPress();
            }
            _touchState = TouchPanel.GetState();
            foreach (var touch in _touchState)
            {
                if (touch.State == TouchLocationState.Pressed)
                {
                    if (OnPress != null)
                    {
                        OnPress(touch.Position.ToPoint(), touch.Id);
                    }
                }
                else if (touch.State == TouchLocationState.Released)
                {
                    if (OnRelease != null)
                    {
                        OnRelease(touch.Position.ToPoint(), touch.Id);
                    }
                }
            }
            _lastGamePadState = _currentGamePadState;
            //var touchCollection = TouchPanel.GetState();
            //foreach (var tl in touchCollection)
            //{
            //    if (tl.State == TouchLocationState.Pressed)
            //    {
            //        if (OnPress != null)
            //            OnPress(tl.Position.ToPoint(), tl.Id);
            //    }
            //    else if (tl.State == TouchLocationState.Released)
            //    {
            //        if (OnRelease != null)
            //            OnRelease(tl.Position.ToPoint(), tl.Id);
            //    }
            //}
#elif WINDOWS
            _currentKeyboardState = Keyboard.GetState();
            if (_currentKeyboardState.IsKeyDown(Keys.Escape) && !_lastKeyboardState.IsKeyDown(Keys.Escape))
            {
                if (BackButtonPress != null)
                    BackButtonPress();
            }
            _lastKeyboardState = _currentKeyboardState;
            var mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
            {
                if (OnPress != null)
                    OnPress(new Point(mouse.X, mouse.Y), 1);
                
            }
            else if (_prevMouseState.LeftButton == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released)
            {
                if (OnRelease != null)
                    OnRelease(new Point(mouse.X, mouse.Y), 1);
            }
            _prevMouseState = mouse;
            MousePos = new Point(mouse.X, mouse.Y);
#endif
        }

        /*public void HandleInput()
        {
            // if the user pressed the back button, we return to the main menu
            if (State.IsBackButtonPressed())
            {
                BackButtonPress();
                //LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
            }
            else
            {
                foreach (var touch in State.TouchState)
                {
                    if (touch.State == TouchLocationState.Pressed)
                    {
                        if (OnPress != null)
                        {
                            OnPress(touch.Position.ToPoint(), touch.Id);
                        }
                    }
                    else if (touch.State == TouchLocationState.Released)
                    {
                        if (OnRelease != null)
                        {
                            OnRelease(touch.Position.ToPoint(), touch.Id);
                        }
                    }
                }
            }
         */
    }
}
