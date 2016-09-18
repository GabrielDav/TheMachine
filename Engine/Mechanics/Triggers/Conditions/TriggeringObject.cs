using System;

namespace Engine.Mechanics.Triggers.Conditions
{
    public class TriggeringObject : ICondition
    {
        public string ObjectName;
        public bool ConditionCheck = true;

        public int TypeId
        {
            get { return (int)ConditionType.TriggeringObject; }
        }

        public int[] EditorGetParametersTypes()
        {
            return new[] { (int)ParameterType.PhysicalObject, (int)ParameterType.Bool };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[] { ObjectName, ConditionCheck };
        }

        public bool Check(EventParams eventParams)
        {
            if (ObjectName == eventParams.TriggeringObject.Name && ConditionCheck)
                return true;
            return ObjectName != eventParams.TriggeringObject.Name && !ConditionCheck;
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
                    ObjectName = (string)value;
                    break;
                case 1:
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
                    "Triggering object is [01:{0}] equals [02:{1}]",
                    string.IsNullOrEmpty(ObjectName) ? "NULL" : ObjectName, ConditionCheck);
        }

        public void Dispose()
        {
            ObjectName = null;
        }
    }
}
