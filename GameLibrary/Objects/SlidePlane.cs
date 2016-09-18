using System.ComponentModel;
#if EDITOR
using System.Drawing;
#endif
using GameLibrary.Gui.ScreenManagement.NewScreens;
using TheGoo;

namespace GameLibrary.Objects
{
    class SlidePlane : WallSlide
    {

        public SlidePlane()
        {
            TypeId = GameObjectType.SlidePlane.ToString();
        }

#if EDITOR
        [ReadOnly(false)]
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

        public override bool IsResizeAvailable(Engine.Mechanics.ResizeType resizeType)
        {
            return true;
        }

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(2, 10);
        }

#endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
#if !EDITOR
            if (!GameGlobals.EditorMode)
                Mask.IsHidden = true;
#endif
        }

    }
}
