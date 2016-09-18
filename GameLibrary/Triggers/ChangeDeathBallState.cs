using System;
using Engine.Mechanics.Triggers;
using GameLibrary.Objects;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class ChangeDeathBallState : ITriggerAction
    {

        public string ObjectName;

        public bool Activate;

        public int TypeId
        {
            get { return (int) GameActionType.SetDeathBallState; }
        }

        public int[] EditorGetParametersTypes()
        {
            return new[]
                       {
                           (int) ParameterType.PhysicalObject,
                           (int) ParameterType.Bool
                       };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           ObjectName,
                           Activate
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    ObjectName = (string)value;
                    break;
                case 1:
                    Activate = (bool)value;
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
                    if (physicalObject is DeathBall)
                    {
                        if (Activate)
                        (physicalObject as DeathBall).ManualyActivate();
                        else
                        {
                            (physicalObject as DeathBall).ManualyDeactivate();
                        }
                        break;
                    }
                    throw new Exception("Object '" + physicalObject.Name + "' is not DeathBall");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Change DeathBall [01:{0}] state to activated [02:{1}]",
                string.IsNullOrEmpty(ObjectName) ? "NULL" : ObjectName, Activate);
        }

        public object Clone()
        {
            var clone = MemberwiseClone();
            return clone;
        }

        public void Dispose()
        {
            ObjectName = null;
            Activate = false;
        }
    }
}
