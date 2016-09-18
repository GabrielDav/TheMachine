using System.ComponentModel;

namespace GameLibrary.Objects
{
    public enum WallSlideType { Small = 1, Large = 3}

    public class WallSlide : BoxPhysicalObject
    {
#if EDITOR
        [ReadOnly(true)]
        public override System.Drawing.Size GridSize
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
            return false;
        }

#endif

        public float Friction { get; set; }

        public WallSlide()
        {
            Animated = false;
            TypeId = GameObjectType.Wall.ToString();
            Init();
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            _resourceId = resourceId;
            _resourceVariation = index;
            Load(resourceId, index);
            var wallType = (WallSlideType) subObjectId;
            switch (wallType)
            {
                case WallSlideType.Small:
                    SetRectangle(0, 0, 20, 100);
                    break;
                case WallSlideType.Large:
                    SetRectangle(0, 0, 20, 500);
                    break;
            }

            Mask.LayerDepth = 0.5f;
            Friction = 0.25f;
        }
#endif
    }
}
