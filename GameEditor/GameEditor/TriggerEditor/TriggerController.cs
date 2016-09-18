using System;
using System.Collections.Generic;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using Engine.Mechanics.Triggers.Conditions;
using GameLibrary.Triggers;

namespace GameEditor.TriggerEditor
{
    public static class TriggerController
    {
        private static bool _isInitialized;
        
        public static Dictionary<int, string> EventsStrings;
        public static Dictionary<int, string> EventImageKeys;
        public static Dictionary<int, string> ConditionImageKeys;
        public static Dictionary<int, Type> ConditionTypes;
        public static Dictionary<int, string> ConditionStrings;
        public static Dictionary<int, string> ParameterTypeStrings;
        public static Dictionary<int, string> ActionImageKeys;
        public static Dictionary<int, string> ActionStrings;

        public static void Init()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            EventsStrings = new Dictionary<int, string>
                {
                    {(int) EventType.ObjectEntersRegion, "Object Enters Region"},
                    {(int) EventType.ObjectLeavesRegion, "Object Leaves Region"},
                    {(int) EventType.GameInitialization, "Game Initialization"},
                    {(int) EventType.ObjectClicked, "Object Clicked"},
                    {(int) GameEventType.TrapButtonOn, "Trap Button On"},
                    {(int) GameEventType.TrapButtonOff, "Trap Button Off"},
                    {(int) EventType.BackPressed, "Back Pressed"},
                };
            EventImageKeys = new Dictionary<int, string>
                {
                    {(int) EventType.ObjectEntersRegion, "Regions.png"},
                    {(int) EventType.ObjectLeavesRegion, "Regions.png"},
                    {(int) EventType.GameInitialization, "Mechanics.png"},
                    {(int) EventType.ObjectClicked, "Pointer.png"},
                    {(int) GameEventType.TrapButtonOn, "Mechanics.png"},
                    {(int) GameEventType.TrapButtonOff, "Mechanics.png"},
                    {(int) EventType.BackPressed, "Back.png"}
                };
            ConditionImageKeys = new Dictionary<int, string>
                {
                    {(int) ConditionType.TriggeringRegion, "Regions.png"},
                    {(int) ConditionType.TriggeringObject, "Object.png"},
                    {(int) GameConditionType.GetNativeParameterBool, "Mechanics.png"},
                    {(int) ConditionType.CameraIsMoving, "Camera.png"}
                };
            ConditionTypes = new Dictionary<int, Type>
                {
                    {(int) ConditionType.TriggeringRegion, typeof (TriggeringRegion)},
                    {(int) ConditionType.TriggeringObject, typeof (TriggeringObject)},
                    {(int) GameConditionType.GetNativeParameterBool, typeof (GetNativeParameterBool)}
                };
            ConditionStrings = new Dictionary<int, string>
                {
                    {(int) ConditionType.TriggeringRegion, "Region condition check"},
                    {(int) ConditionType.TriggeringObject, "Object condition check"},
                    {(int) GameConditionType.GetNativeParameterBool, "Check native parameter value - bool"},
                    {(int) ConditionType.CameraIsMoving, "Camera is moving"}
                };
            ActionImageKeys = new Dictionary<int, string>
                {
                    {(int) ActionType.SetCameraZoom, "Camera.png"},
                    {(int) ActionType.MoveCamera, "Camera.png"},
                    {(int) ActionType.SetCameraPosition, "Camera.png"},
                    {(int) ActionType.ActivateObject, "Property.png"},
                    {(int) ActionType.DisableTrigger, "Mechanics.png"},
                    {(int) GameActionType.ExecuteNative, "Mechanics.png"},
                    {(int) GameActionType.SetButtonText, "Property.png"},
                    {(int) GameActionType.SetMusicText, "Property.png"},
                    {(int) GameActionType.SetCameraPath, "Camera.png"},
                    {(int) GameActionType.StartBtnAnimation, "Camera.png"},
                    {(int) GameActionType.PlayClickSound, "Sound.png"},
                    {(int) ActionType.ExitGame, "Mechanics.png"},
                    {(int) GameActionType.ReturnMovingCircle, "Property.png"},
                    {(int) GameActionType.RestartMovingCircle, "Property.png"},
                    {(int) ActionType.SetGravity, "Mechanics.png"},
                    {(int) ActionType.RotateCamera, "Camera.png"},
                    {(int) GameActionType.SetDeathBallState, "Property.png"},
                    {(int) GameActionType.SetCameraBoundsTopRight, "Camera.png"}
                };
            ActionStrings = new Dictionary<int, string>
                {
                    {(int) ActionType.SetCameraZoom, "Set camera zoom"},
                    {(int) ActionType.SetCameraPosition, "Set camera position"},
                    {(int) ActionType.MoveCamera, "Move camera"},
                    {(int) ActionType.ActivateObject, "Activate/deactivate object"},
                    {(int) ActionType.DisableTrigger, "Enable/disable trigger"},
                    {(int) GameActionType.ExecuteNative, "Execute native function"},
                    {(int) GameActionType.SetButtonText, "Set button text"},
                    {(int) GameActionType.SetMusicText, "Set music text"},
                    {(int) GameActionType.StartBtnAnimation, "Start button push animation"},
                    {(int) GameActionType.SetCameraPath, "Set camera path"},
                    {(int) GameActionType.PlayClickSound, "Play click sound"},
                    {(int) ActionType.ExitGame, "Exit game"},
                    {(int) ActionType.SetGravity, "Set gravity"},
                    {(int) GameActionType.ReturnMovingCircle, "Return moving circle to starting position and stop it"},
                    {(int) GameActionType.RestartMovingCircle, "Restart moving circle"},
                    {(int) ActionType.RotateCamera, "Rotate camera"},
                    {(int) GameActionType.SetDeathBallState, "Change DeathBall state"},
                    {(int) GameActionType.SetCameraBoundsTopRight, "Set camera bounds top right point"}
                };

        }

        public static string ParameterGetImageKey(int parameterType)
        {
            switch (parameterType)
            {
                case (int)ParameterType.Bool:
                    return "Check.png";
                case (int)ParameterType.Region:
                    return "RegionType.png";
                case (int)ParameterType.Float:
                    return "FloatValue.png";
                case (int)ParameterType.Int:
                    return "IntegerValue.png";
                case (int)ParameterType.Point:
                    return "Point.png";
                case (int)ParameterType.PhysicalObject:
                    return "Object.png";
                case (int)ParameterType.Trigger:
                    return "Trigger.png";
                case (int)GameParameterType.NativeFunction:
                    return "Trigger.png";
                case (int)GameParameterType.NativeParameterBool:
                    return "Trigger.png";
                case (int)ParameterType.String:
                    return "String.png";
                case (int)ParameterType.MenuObject:
                    return "Object.png";
                case (int)ParameterType.CameraPath:
                    return "Camera.png";

            }
            throw new Exception("Undefined parameter type: " + parameterType);
        }

    }
}
