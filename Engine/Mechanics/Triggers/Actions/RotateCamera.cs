using System;
using Engine.Core;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics.Triggers.Actions
{
    public class RotateCamera : ITriggerAction
    {
        public int TypeId { get { return (int)ActionType.RotateCamera; } }

        public float Rotation;
        public float Speed;

        public RotateCamera()
        {
            Rotation = 180;
            Speed = 5;
        }

        public int[] EditorGetParametersTypes()
        {
            return new[]
                       {
                           (int) ParameterType.Float,
                           (int) ParameterType.Float
                       };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           Rotation,
                           Speed
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Rotation = (float)value;
                    break;
                case 1:
                    Speed = (float) value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void DoAction(EventParams eventParams)
        {
            EngineGlobals.Camera2D.Rotate(MathHelper.ToRadians(Rotation), Speed/1000f, true);
        }

        public override string ToString()
        {
            return string.Format("Rotate camera over [01:{0}] degrees with speed of [02:{1}]", Rotation.ToString("0.00"),
                Speed.ToString("0.00"));
        }

        public object Clone()
        {
            var clone = MemberwiseClone();
            return clone;
        }

        public void Dispose()
        {
            Rotation = 0;
            Speed = 0;
        }
    }
}
