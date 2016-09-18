using System;
using Engine.Mechanics.Triggers;
using GameLibrary.Objects;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class ShowTrialDialog : ITriggerAction
    {
        public int TypeId
        {
            get { return (int)GameActionType.SetMusicText; }
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
                    if (physicalObject is MenuObject)
                    {
                        (physicalObject as MenuObject).Text = GameGlobals.Settings.MusicVolume == 0 ? "Music: OFF" : string.Format("Music: {0}%", GameGlobals.Settings.MusicVolume);
                        break;
                    }
                    throw new Exception("Object '" + physicalObject.Name + "' is not Menu Object");
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Set button [01:{0}] text to music volume information", string.IsNullOrEmpty(ObjectName) ? "NULL" : ObjectName);
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
