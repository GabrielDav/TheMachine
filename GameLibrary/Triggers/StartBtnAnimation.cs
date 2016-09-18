using System;
using Engine.Mechanics.Triggers;
using GameLibrary.Objects;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class StartBtnAnimation : ITriggerAction
    {
        public int TypeId
        {
            get { return (int)GameActionType.StartBtnAnimation; }
        }

        public string ObjectName;

        public int[] EditorGetParametersTypes()
        {
            return new[] { (int)ParameterType.MenuObject };
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
                    if (physicalObject is IMenuBtn)
                    {
                        (physicalObject as IMenuBtn).ShowPressAnimation();
                        break;
                    }
                    throw new Exception("Object '" + physicalObject.Name + "' is not IMenuBtn");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Start button [01:{0}] push animation", string.IsNullOrEmpty(ObjectName) ? "NULL" : ObjectName);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
            ObjectName = null;
        }

    }
}
