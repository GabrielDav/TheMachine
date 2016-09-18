#if EDITOR
using System.Drawing;
#endif
using GameLibrary.Gui.ScreenManagement.NewScreens;

namespace GameLibrary.Objects
{
    public class MenuGear : Circle
    {

        public MenuGear() : base()
        {
            TypeId = GameObjectType.MenuGear.ToString();
            IgnoreCulling = true;
        }

        public override void Update()
        {
            base.Update();
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(10, 10);
        }

        public override bool EditorIsDebugFlagEnabled(Engine.Mechanics.DebugFlag flag)
        {
            return false;
        }

#endif

    }
}
