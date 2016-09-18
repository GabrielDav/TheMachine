using System;
using Engine.Core;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics.Triggers.Actions
{
    public class ZoomCamera : ITriggerAction
    {
        public int TypeId { get { return (int)ActionType.SetCameraZoom; } }

        public float Zoom;
        public int Time;

        public ZoomCamera()
        {
            Zoom = 1.0f;
        }

        public void DoAction(EventParams eventParams)
        {
            EngineGlobals.Camera2D.SlowZoom(Zoom, Time);
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
                           (int) ParameterType.Float,
                           (int) ParameterType.Int
                       };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           Zoom,
                           Time
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Zoom = (float)value;
                    break;
                case 1:
                    Time = (int)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return string.Format("Set camera zoom to [01:{0}]", Zoom.ToString("0.00"));
        }

        public void Dispose()
        {
            
        }
    }
}
