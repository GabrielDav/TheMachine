using System;
using System.Collections.Generic;
#if EDITOR
using System.Drawing;
#endif
using System.Linq;
using System.Text;
using GameLibrary.Gui.ScreenManagement.NewScreens;

namespace GameLibrary.Objects
{
    public class MenuLine : Plane
    {

        public MenuLine() : base()
        {
            TypeId = GameObjectType.MenuLine.ToString();
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(5, 1);
        }
#endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            Mask.IsHidden = false;
        }

    }
}
