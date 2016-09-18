using System;
using Engine.Core;

namespace Engine.Mechanics.Triggers.Actions
{
    public class DisableTrigger : ITriggerAction
    {

        public int TypeId { get { return (int) ActionType.DisableTrigger; } }

        public string TriggerName { get; set; }

        public bool Enabled { get; set; }

        public int[] EditorGetParametersTypes()
        {
            return new[] {(int) ParameterType.Trigger, (int) ParameterType.Bool };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           TriggerName,
                           Enabled
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    TriggerName = (string)value;
                    break;
                case 1:
                    Enabled = (bool) value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public void DoAction(EventParams eventParams)
        {
            foreach (var trigger in EngineGlobals.TriggerManager.Triggers)
            {
                if (trigger.Name == TriggerName)
                {
                    trigger.Enabled = Enabled;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Set trigger [01:{0}] is enabled equals [02:{1}]", string.IsNullOrEmpty(TriggerName)? "NULL" : TriggerName, Enabled);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
            TriggerName = null;
        }
    }
}
