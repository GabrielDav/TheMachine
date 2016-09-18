using System;
using Engine.Core;

namespace Engine.Mechanics.Triggers.Actions
{
    public class ExitGame : ITriggerAction
    {

        public int TypeId { get { return (int)ActionType.ExitGame; } }

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

        public void DoAction(EventParams eventParams)
        {
            Controller.CurrentGame.Exit();
        }

        public override string ToString()
        {
            return string.Format("Exit game");
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
