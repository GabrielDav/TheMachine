using System;
using System.Collections.Generic;
using System.ComponentModel;
#if EDITOR
using System.Drawing;
#endif
using System.Linq;
using System.Text;
using Engine.Mechanics;

namespace GameLibrary.Objects
{
    public class ResizableDecorativeObject : DecorativeObject
    {
        public ResizableDecorativeObject()
        {
            TypeId = GameObjectType.ResizableDecorativeObject.ToString();
        }

        #if EDITOR
        [ReadOnly(false)]
        public override Size GridSize
        {
            get { return base.GridSize; }
            set { base.GridSize = value; }
        }

        public override bool IsResizeAvailable(ResizeType resizeType)
        {
            return true;
        }
        #endif
    }
}
