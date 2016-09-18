using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    public struct RectangleF
    {
        private float _width;
        private float _height;
        private float _x;
        private float _y;

        
        public float Width
        {
            [DebuggerStepThrough]
            get { return _width; }
            [DebuggerStepThrough]
            set { _width = value; }
        }

        
        public float Height
        {
            [DebuggerStepThrough]
            get { return _height; }
            [DebuggerStepThrough]
            set { _height = value; }
        }

        public float X
        {
            [DebuggerStepThrough]
            get { return _x; }
            [DebuggerStepThrough]
            set { _x = value; }
        }

        public float Y
        {
            [DebuggerStepThrough]
            get { return _y; }
            [DebuggerStepThrough]
            set { _y = value; }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public RectangleF(Rectangle rectangle)
        {
            _x = rectangle.X;
            _y = rectangle.Y;
            _width = rectangle.Width;
            _height = rectangle.Height;
        }

        public bool Contains(int x, int y)
        {
            if (_x <= x && x < _x + _width && _y <= y)
                return y < _y + _height;
            return false;
        }

        public bool Intersects(RectangleF value)
        {
            if (value.X < _x + _width && _x < value.X + value.Width && value.Y < _y + _height)
                return _y < value.Y + value.Height;
            return false;
        }

        public bool Intersects(Rectangle value)
        {
            if (value.X < _x + _width && _x < value.X + value.Width && value.Y < _y + _height)
                return _y < value.Y + value.Height;
            return false;
        }

        public bool Contains(Point p)
        {
            return Contains(p.X, p.Y);
        }

        public bool Contains(RectangleF rectangle)
        {
            if (X <= rectangle.X && rectangle.X + rectangle.Width <= X + Width && Y <= rectangle.Y)
                return rectangle.Y + rectangle.Height <= Y + Height;
            return false;
        }

        public bool Contains(Rectangle rectangle)
        {
            if (X <= rectangle.X && rectangle.X + rectangle.Width <= X + Width && Y <= rectangle.Y)
                return rectangle.Y + rectangle.Height <= Y + Height;
            return false;
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle((int)_x, (int)_y, (int)_width, (int)_height);
        }

        public Vector2 GetPos()
        {
            return new Vector2(X, Y);
        }

        public override string ToString()
        {
            return "{X:" + _x.ToString("0.##") + " Y:" + _y.ToString("0.##") + " Width:" + _width.ToString("0.##") + " Height:" + _height.ToString("0.##") + "}";
        }
    }
}
