#if EDITOR
using System.ComponentModel;
using System.Drawing;
#endif

namespace GameLibrary.Objects
{
    public class VersionInformation : Plane
    {

        public VersionInformation()
        {
            TypeId = GameObjectType.VersionInfo.ToString();
        }

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

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(8, 2);
        }
#endif
        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            Mask.IsHidden = false;
        }
       

    }
}
