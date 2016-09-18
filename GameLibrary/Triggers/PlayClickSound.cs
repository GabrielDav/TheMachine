using System;
using Engine.Core;
using Engine.Mechanics.Triggers;

namespace GameLibrary.Triggers
{
    public class PlayClickSound : ITriggerAction
    {

        public int TypeId { get { return (int) GameActionType.PlayClickSound; } }

        public int[] EditorGetParametersTypes()
        {
            return new int[0];
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[0];
        }

        public void EditorSetValue(int index, object value)
        {
           throw new IndexOutOfRangeException();
        }

        public override string ToString()
        {
            return string.Format("Play click sound");
        }

        public void Dispose()
        {

        }

        public void DoAction(EventParams eventParams)
        {
#if !EDITOR
            EngineGlobals.SoundManager.Play("click", "click", 1f, false, 0f, 0f);
#endif
        }

        public object Clone()
        {
            var clone = MemberwiseClone();

            return clone;
        }
    }
}
