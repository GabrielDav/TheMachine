using System;
using Engine.Mechanics.Triggers;
using Engine.Mechanics.Triggers.Conditions;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class GetNativeParameterBool : ICondition
    {
        public int TypeId
        {
            get { return (int)GameConditionType.GetNativeParameterBool; }
        }

        public NativeParameterBool Parameter = NativeParameterBool.SettingsSoundOn;
        public bool ConditionCheck = true;

        public int[] EditorGetParametersTypes()
        {
            return new[] { (int)GameParameterType.NativeParameterBool, (int)ParameterType.Bool };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[] { Parameter , ConditionCheck };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Parameter = (NativeParameterBool)value;
                    break;
                case 1:
                    ConditionCheck = (bool)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public bool Check(EventParams eventParams)
        {
            switch (Parameter)
            {
                case NativeParameterBool.SettingsSoundOn:
                    return GameGlobals.Settings.SoundOn == ConditionCheck;
                case NativeParameterBool.SettingsMusicOn:
                    return GameGlobals.Settings.MusicOn == ConditionCheck;
                case NativeParameterBool.SettingVibrationOn:
                    return GameGlobals.Settings.VibrationOn == ConditionCheck; 
                case NativeParameterBool.IsTrial:
                    return GameGlobals.IsTrial;
                default:
                    throw new Exception("Undefined NativeParameterBool type '" + Parameter + "'");
            }
        }

        public override string ToString()
        {
            return string.Format("Check if native parameter [01:{0}] bool value equals to [02:{1}]", Parameter, ConditionCheck);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Dispose()
        {
            
        }
    }
}
