using System;
using Engine.Core;
using Engine.Mechanics.Triggers;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class SetCameraBoundsTopRight : ITriggerAction
    {
        public int TypeId { get { return (int) GameActionType.SetCameraBoundsTopRight; } }

        public int X;

        public int Y;

        public int[] EditorGetParametersTypes()
        {
            return new[] { (int)ParameterType.Int, (int)ParameterType.Int };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[] { X, Y };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    X = (int)value;
                    break;
                case 1:
                    Y = (int)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();

            }
        }

        public void DoAction(EventParams eventParams)
        {
            EngineGlobals.Camera2D.SetCamerBounds(new Rectangle(X, Y, GameGlobals.Map.Width,
                   GameGlobals.Map.Height));
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public void Dispose()
        {
            X = 0;
            Y = 0;
        }

        public override string ToString()
        {
            return string.Format("Set camera bounds rectangle top right corner X = [01:{0}], Y = [02:{1}]",
                                 X, Y);
        }
    }
}
