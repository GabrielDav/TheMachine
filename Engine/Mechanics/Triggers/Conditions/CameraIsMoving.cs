using System;
using Engine.Core;

namespace Engine.Mechanics.Triggers.Conditions
{
    public class CameraIsMoving : ICondition
    {
        public bool ConditionCheck = true;

        public int TypeId
        {
            get { return (int)ConditionType.CameraIsMoving; }
        }

        public int[] EditorGetParametersTypes()
        {
            return new[] { (int)ParameterType.Bool };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[] { ConditionCheck };
        }

        public bool Check(EventParams eventParams)
        {
            return EngineGlobals.Camera2D.IsMoving == ConditionCheck;
        }

        public object Clone()
        {
            var clone = MemberwiseClone();

            return clone;
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    ConditionCheck = (bool)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "Camera is moving equals [01:{0}]", ConditionCheck);
        }

        public void Dispose()
        {
        }
    }
}
