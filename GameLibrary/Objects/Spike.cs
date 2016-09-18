#if EDITOR
using System.ComponentModel;
using System.Drawing;
#endif
using GameLibrary.Gui.ScreenManagement.NewScreens;

namespace GameLibrary.Objects
{
    public class Spike : Plane
    {

#if EDITOR

        [ReadOnly(true)]
        public override Size GridSize
        {
            get
            {
                return base.GridSize;
            }
            set
            {
                base.GridSize = value;
            }
        }

#endif

        public Spike()
        {
            TypeId = GameObjectType.Spike.ToString();
        }
#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            Orientation = Orientation.Top;
            SetRectangle(0, 0, 150, 20);
        }
#endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            Mask.IsHidden = false;
        }
    }
}