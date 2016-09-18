using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Objects
{
    public class SpikeSmall : Spike
    {
        public SpikeSmall()
        {
            TypeId = GameObjectType.SpikeSmall.ToString();
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            SetRectangle(0, 0, 60, 20);
        }
#endif
    }
}
