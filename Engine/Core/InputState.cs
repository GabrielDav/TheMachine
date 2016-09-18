using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameLibrary.Gui.ScreenManagement.ScreenManager
{
    public class InputState
    {
        public const int MaxInputs = 4;

        public TouchCollection TouchState;
        public GamePadState CurrentGamePadState;
        public GamePadState LastGamePadState;
        public PlayerIndex CurrentPlayerIndex = PlayerIndex.One;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        public void Update()
        {
            LastGamePadState = CurrentGamePadState;

            CurrentGamePadState = GamePad.GetState(CurrentPlayerIndex);

            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }

        public bool IsBackButtonPressed()
        {
            return CurrentGamePadState.IsButtonDown(Buttons.Back);
        }
    }
}
