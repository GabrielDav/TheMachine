using System;
using Engine.Core;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics.Triggers.Actions
{
    public class SetGravity : ITriggerAction
    {
        public int TypeId { get { return (int)ActionType.SetGravity; } }

        public float Gravity;

        public SetGravity()
        {
            Gravity = EngineGlobals.Gravity.Y;
        }

        public void DoAction(EventParams eventParams)
        {
            EngineGlobals.Gravity = new Vector2(EngineGlobals.Gravity.X, Gravity);
        }

        public object Clone()
        {
            var clone = MemberwiseClone();

            return clone;
        }

        public int[] EditorGetParametersTypes()
        {
            return new[]
                       {
                           (int) ParameterType.Float
                       };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           Gravity
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Gravity = (float)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return string.Format("Set gravity to [01:{0}]", Gravity.ToString("0.00"));
        }

        public void Dispose()
        {
            Gravity = 0;
        }
    }
}
