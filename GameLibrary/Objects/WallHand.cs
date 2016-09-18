using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GameLibrary.Gui.ScreenManagement.NewScreens;

namespace GameLibrary.Objects
{
    public class WallHand : WallSlide
    {

        public WallHand()
        {
            Animated = false;
            TypeId = GameObjectType.Wall.ToString();
            Init();
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            SetRectangle(0, 0, 100, 240);
        }
#endif
    }
}
