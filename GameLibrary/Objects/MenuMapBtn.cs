using System.Drawing;
using GameLibrary.Gui.ScreenManagement.NewScreens;

namespace GameLibrary.Objects
{
    public class MenuMapBtn : MenuObject
    {
        public MenuMapBtn() : base()
        {
            TypeId = GameObjectType.MenuBtnMap.ToString();
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(10, 10);
        }
#endif
    }
}
