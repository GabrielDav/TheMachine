using System;
using System.Collections.Generic;
#if EDITOR
using System.ComponentModel;
using System.ComponentModel.Design;
#endif
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Engine.Mechanics
{



 /*   public class Map
    {

        public Map()
        {
            Resources = new MapResourcesData();
            Data = new MapData();
            Objects = new List<PhysicalObject>();
        }

        public Map(Map map)
        {
            Width = map.Width;
            Height = map.Height;
            Resources = map.Resources;
            Data = map.Data;
            Objects = map.Objects;
        }

        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public virtual MapResourcesData Resources { get; set; }

        [ContentSerializerIgnore]
        public virtual MapData Data { get; set; }

        public virtual List<PhysicalObject> Objects { get; set; }
    }*/
}
