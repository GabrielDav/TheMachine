using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Mechanics.Triggers;
using GameLibrary.Objects;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class MovingCircleRestart : ITriggerAction
    {
        public string ObjectName;

        public int TypeId
        {
            get { return (int) GameActionType.RestartMovingCircle; }
        }

        public int[] EditorGetParametersTypes()
        {
            return new[] { (int)ParameterType.PhysicalObject };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           ObjectName
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    ObjectName = (string)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void DoAction(EventParams eventParams)
        {
            foreach (var physicalObject in GameGlobals.Map.GameObjects)
            {
                if (physicalObject.Name == ObjectName)
                {
                    if (physicalObject is MovingCircle)
                    {
                        (physicalObject as MovingCircle).RestartMoving();
                        break;
                    }
                    throw new Exception("Object '" + physicalObject.Name + "' is not MovingCircle");
                }
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("Restart moving circle [01:{0}].", ObjectName);
        }

        public void Dispose()
        {

        }
    }
}
