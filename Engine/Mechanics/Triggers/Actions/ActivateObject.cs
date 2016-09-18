using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core;

namespace Engine.Mechanics.Triggers.Actions
{
    public class ActivateObject : ITriggerAction
    {
        public int TypeId { get { return (int)ActionType.ActivateObject; } }

        public string ObjectName;
        public bool Active;
       

        public void DoAction(EventParams eventParams)
        {
            foreach (var mapObject in EngineGlobals.TriggerManager.MapObjects)
            {
                if (mapObject.Name == ObjectName)
                {
                    mapObject.IsActivated = Active;
                }
            }
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
                           (int) ParameterType.PhysicalObject,
                           (int) ParameterType.Bool
                       };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           ObjectName,
                           Active
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
                    Active = (bool)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return string.Format("Set object [01:{0}] property 'active' to [02:{1}]",
                                 string.IsNullOrEmpty(ObjectName) ? "NULL" : ObjectName, Active);
        }

        public void Dispose()
        {
            
        }
    }
}
