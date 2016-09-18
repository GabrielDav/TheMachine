using System;
using Engine.Core;
using Engine.Mechanics.Triggers;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class ExecuteNative : ITriggerAction
    {

        public int TypeId
        {
            get { return (int) GameActionType.ExecuteNative; }
        }

        public NativeFunctions Function = NativeFunctions.SettingsToggleSound;

        public int[] EditorGetParametersTypes()
        {
            return new[] {(int) GameParameterType.NativeFunction};
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[]
                       {
                           Function
                       };
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Function = (NativeFunctions) value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }

        }

        public void DoAction(EventParams eventParams)
        {
            switch (Function)
            {
                case NativeFunctions.SettingsToggleSound:
                    GameGlobals.Settings.SoundOn = !GameGlobals.Settings.SoundOn;
                    SoundManager.MuteSound = !GameGlobals.Settings.SoundOn;
                    break;
                case NativeFunctions.SettingsToggleMusic:
                    GameGlobals.Settings.MusicOn = !GameGlobals.Settings.MusicOn;
                    MusicManager.MuteMusic = !GameGlobals.Settings.MusicOn;
                    if (!MusicManager.MuteMusic && !MusicManager.IsPlaying)
                    {
                        MusicManager.Play("menu", true);
                    }
                    break;
                case NativeFunctions.SettingToggleVibration:
                    GameGlobals.Settings.VibrationOn = !GameGlobals.Settings.VibrationOn;
                    GameGlobals.Vibrate = GameGlobals.Settings.VibrationOn;
                    break;
                case NativeFunctions.SettingsSave:
                    Settings.Save();
                    break;
                case NativeFunctions.SettingsLoad:
                    Settings.Load();
                    break;
                case NativeFunctions.StartArcade:
                    GameGlobals.Menu.StartArcade();
                    break;
                case NativeFunctions.SettingsIncMusic:
                    var play = GameGlobals.Settings.MusicVolume == 0 && !MusicManager.IsPlaying;
                    if (GameGlobals.Settings.MusicVolume < 100)
                    {
                        GameGlobals.Settings.MusicVolume += 10;
                        MusicManager.GlobalMusicVolume = GameGlobals.Settings.MusicVolume/100f;
                        if (play)
                        {
                            GameGlobals.Settings.MusicOn = true;
                            MusicManager.Play("menu", true);
                        }
                    }
                    break;
                case NativeFunctions.SettignsDecMusi:
                    var stop = GameGlobals.Settings.MusicVolume == 10 && MusicManager.IsPlaying;
                    if (GameGlobals.Settings.MusicVolume > 0)
                    {
                        GameGlobals.Settings.MusicOn = false;
                        GameGlobals.Settings.MusicVolume -= 10;
                        MusicManager.GlobalMusicVolume = GameGlobals.Settings.MusicVolume/100f;
                        if (stop)
                            MusicManager.Stop();
                    }
                    break;
                default:
                    throw new Exception("Undefined ");
            }
        }

        public override string ToString()
        {
            return string.Format("Execute native function: [01:{0}]", Function.ToString());
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
