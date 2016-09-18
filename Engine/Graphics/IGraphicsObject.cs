using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Graphics
{
    public interface IGraphicsObject
    {
        bool StaticPosition { get; set; }
        [ContentSerializerIgnore]
        bool IgnoreCulling { get; }
        Rectangle Rect { get; set; }
        Rectangle CornerRectangle { get; }
        event SimpleEvent OnPositionTypeChanged;
        void Draw();
    }
}
