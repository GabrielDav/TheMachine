using System;
using Engine.Mechanics.Triggers;
using GameLibrary.Objects;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class SetButtonText : ITriggerAction
    {

        public int TypeId
        {
            get { return (int) GameActionType.SetButtonText; }
        }

        public string ObjectName;
        public string Text = "";

        public int[] EditorGetParametersTypes()
        {
            return new[] {(int)ParameterType.MenuObject, (int) ParameterType.String};
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           ObjectName,
                           Text
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    ObjectName = (string) value;
                    break;
                case 1:
                    Text = (string)value;
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
                    if (physicalObject is MenuObject)
                    {
                        (physicalObject as MenuObject).Text = Text;
                        break;
                    }
                    throw new Exception("Object '" + physicalObject.Name + "' is not Menu Object");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Set button [01:{0}] text to [02:{1}]", string.IsNullOrEmpty(ObjectName)? "NULL" : ObjectName, "'" + Text + "'");
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
            ObjectName = null;
            Text = null;
        }
    }
}
