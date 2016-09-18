namespace GameLibrary.Triggers
{
    public enum NativeFunctions
    {
        SettingsToggleSound = 1,
        SettingsToggleMusic = 2,
        SettingToggleVibration = 3,
        SettingsSave = 10,
        SettingsLoad = 11,
        StartArcade = 20,
        SettingsIncMusic = 30,
        SettignsDecMusi = 31
    }

    public enum NativeParameterBool
    {
        SettingsSoundOn = 1,
        SettingsMusicOn = 2,
        SettingVibrationOn = 3,
        IsTrial = 4
    }

    public enum GameActionType
    {
        ExecuteNative = 101,
        SetButtonText = 102,
        SetCameraPath = 103,
        PlayClickSound = 104,
        SetMusicText = 150,
        StartBtnAnimation = 151,
        ReturnMovingCircle = 152,
        RestartMovingCircle = 153,
        SetDeathBallState = 154,
        SetCameraBoundsTopRight = 155
    }

    public enum GameConditionType { GetNativeParameterBool = 101 }

    public enum GameParameterType { NativeFunction = 101, NativeParameterBool = 102 }

    public enum GameEventType { TrapButtonOn = 101, TrapButtonOff = 102 }
}
