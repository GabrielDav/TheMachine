using System;
using Engine.Core;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics.Triggers.Actions
{
    public class MoveCamera : ITriggerAction
    {

        public int TypeId { get { return (int) ActionType.MoveCamera; } }

        public Point MoveTo { get; set; }

        public float Speed { get; set; }

        public int[] EditorGetParametersTypes()
        {
            return new[]
                       {
                           (int) ParameterType.Point,
                           (int) ParameterType.Float
                       };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           MoveTo,
                           Speed
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    MoveTo = (Point)value;
                    break;
                case 1:
                    Speed = (float)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return string.Format("Move camera to point [01:{0}] with speed of [02:{1}]",  MoveTo.ToFormatedString(), Speed);
        }

        public void Dispose()
        {
            
        }

        public void DoAction(EventParams eventParams)
        {
            EngineGlobals.Camera2D.MoveTo(MoveTo.ToVector(), Speed);
        }

        public object Clone()
        {
            var clone = MemberwiseClone();

            return clone;
        }
    }
}
