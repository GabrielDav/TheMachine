using System;
using Engine.Core;
using Engine.Mechanics.Triggers;
using GameLibrary.Gui.ScreenManagement;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Triggers
{
    public class SetCameraPath : ITriggerAction
    {

        public int TypeId
        {
            get { return (int) GameActionType.SetCameraPath; }
        }

        public string CameraPathName;

        [ContentSerializerAttribute(Optional = true)]
        public bool DisablePlayerInput;

        public void Dispose()
        {
            CameraPathName = null;
            if (EngineGlobals.Camera2D != null)
                EngineGlobals.Camera2D.PathFinished -= Camera2DOnPathFinished;
        }

        public int[] EditorGetParametersTypes()
        {
            return new[] {(int) ParameterType.CameraPath, (int)ParameterType.Bool};
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[] {CameraPathName, DisablePlayerInput};
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    CameraPathName = (string) value;
                    break;
                case 1:
                    DisablePlayerInput = (bool) value;
                    break;
                default:
                    throw new IndexOutOfRangeException();

            }
        }

        public void DoAction(EventParams eventParams)
        {
            foreach (var physicalObject in GameGlobals.Map.GameObjects)
            {
                if (physicalObject.Name == CameraPathName && physicalObject is CameraPath)
                {
                    var obj = physicalObject as CameraPath;
                    EngineGlobals.Camera2D.StartFollow(obj.Path);
                    
                    #if !EDITOR
                    if (DisablePlayerInput)
                    {

                        ((BaseFadeScreen) EngineGlobals.ScreenManager.CurrentScreen).InputDisabled = true;
                        EngineGlobals.Camera2D.PathFinished += Camera2DOnPathFinished;
                    }
                    #endif
                    return;
                }
            }
            throw new Exception("Camera path '" + CameraPathName + "' not found.");
        }

        private void Camera2DOnPathFinished(object sender)
        {
            #if !EDITOR
            ((BaseFadeScreen)EngineGlobals.ScreenManager.CurrentScreen).InputDisabled = false;
            #endif
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("Set camera path to [01:{0}] and disable player input [02:{1}]",
                                 string.IsNullOrEmpty(CameraPathName) ? "NULL" : CameraPathName, DisablePlayerInput);
        }
    }
}
