using System;
using Engine.Core;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics.Triggers.Actions
{
    public class SetCameraPosition : ITriggerAction
    {

        public int TypeId { get { return (int) ActionType.SetCameraPosition; } }

        public Point NewPos { get; set; }

        public int[] EditorGetParametersTypes()
        {
            return new[]
                       {
                           (int) ParameterType.Point
                       };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           NewPos
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    NewPos = (Point)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return string.Format("Set Camera Position to [01:{0}]",  NewPos.ToFormatedString());
        }

        public void Dispose()
        {
            
        }

        public void DoAction(EventParams eventParams)
        {
            EngineGlobals.Camera2D.Position = NewPos.ToVector();
        }

        public object Clone()
        {
            var clone = MemberwiseClone();

            return clone;
        }
    }
}
