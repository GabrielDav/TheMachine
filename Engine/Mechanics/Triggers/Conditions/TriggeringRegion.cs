using System;

namespace Engine.Mechanics.Triggers.Conditions
{
    public class TriggeringRegion : ICondition
    {
        public string RegionName;
        public bool ConditionCheck = true;

        public int TypeId
        {
            get { return (int)ConditionType.TriggeringRegion; }
        }

        public int[] EditorGetParametersTypes()
        {
            return new[] { (int)ParameterType.Region, (int)ParameterType.Bool };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[] { RegionName, ConditionCheck };
        }

        public bool Check(EventParams eventParams)
        {
            if (eventParams.TriggeringRegion.Name != RegionName && !ConditionCheck)
                return true;
            if (eventParams.TriggeringRegion.Name == RegionName && ConditionCheck)
                return true;
            return false;
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
                    RegionName = (string)value;
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
                    "Triggering region is [02:{0}] equals [03:{1}]",
                    string.IsNullOrEmpty(RegionName) ? "NULL" : RegionName, ConditionCheck);
        }

        public void Dispose()
        {
            RegionName = null;
        }
    }
}
