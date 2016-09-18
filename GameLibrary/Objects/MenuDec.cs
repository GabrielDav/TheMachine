#if EDITOR
using System.Drawing;
#endif
using GameLibrary.Gui.ScreenManagement.NewScreens;

namespace GameLibrary.Objects
{
    public class MenuDec : Tile
    {

        public MenuDec() : base()
        {
            TypeId = GameObjectType.MenuDec.ToString();
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(3, 1);
        }
#endif
    }
}
