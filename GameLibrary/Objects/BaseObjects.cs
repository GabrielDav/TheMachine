using System;
using System.ComponentModel;
using Engine.Core;
using Engine.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using RectangleF = Engine.Graphics.RectangleF;

namespace GameLibrary.Objects
{
    public enum MoveDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public enum Orientation {Top = 1, Left = 2, Bottom = 3, Right = 4}

    public abstract class BoxPhysicalObject : PhysicalObject
    {
        protected Orientation _orientation = Orientation.Top;

#if EDITOR
        [PropertyOrder(4)]
#endif
        public virtual Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (EngineGlobals.Device == null)
                {
                    _orientation = value;
                    return;
                }
                if (EngineGlobals.MapIsLoading)
                {
                    _orientation = value;
                    if (_orientation == Orientation.Right || _orientation == Orientation.Left)
                        Mask.Scale = new Vector2(_rectangle.Height / _rectangle.Width, _rectangle.Width / _rectangle.Height);
                    switch (_orientation)
                    {
                        case Orientation.Left:
                            Mask.Rotation = MathHelper.ToRadians(270);
                            break;
                        case Orientation.Top:
                            Mask.Rotation = MathHelper.ToRadians(0);
                            break;
                        case Orientation.Right:
                            Mask.Rotation = MathHelper.ToRadians(90);
                            break;
                        case Orientation.Bottom:
                            Mask.Rotation = MathHelper.ToRadians(180);
                            break;
                    }
                    return;
                }
#if EDITOR
                FirePropertyChangingEvent("Orientation");
#endif

                float newWidth;
                float newHeight;
                if (((_orientation == Orientation.Left || _orientation == Orientation.Right) &&
                    (value != Orientation.Left && value != Orientation.Right)) ||
                    ((_orientation == Orientation.Top || _orientation == Orientation.Bottom) &&
                    (value != Orientation.Top && value != Orientation.Bottom)))
                {
                    newWidth = _rectangle.Height;
                    newHeight = _rectangle.Width;
                    if (value == Orientation.Top || value == Orientation.Bottom)
                        Mask.Scale = new Vector2(1.0f, 1.0f);
                    else
                        Mask.Scale = new Vector2(_rectangle.Width / newWidth,
                                                 newWidth / _rectangle.Width);
                }
                else
                {
                    newWidth = _rectangle.Width;
                    newHeight = _rectangle.Height;
                    
                }
                
#if EDITOR
                var newX = _rectangle.X;
                if (newX + newWidth > EngineGlobals.Grid.WidthReal && !IgnoreGridBounds)
                {
                    newX = EngineGlobals.Grid.WidthReal - newX;
                }
                var newY = _rectangle.Y;
                if (newY + newHeight > EngineGlobals.Grid.HeightReal && !IgnoreGridBounds)
                {
                    newY = EngineGlobals.Grid.HeightReal - newHeight;
                }

                var rectangle =
                    EngineGlobals.Grid.ToAlignedRectangle(new Rectangle((int) newX, (int) newY, (int) newWidth,
                                                                        (int) newHeight));
#else
                var rectangle = new Rectangle((int)_rectangle.X, (int)_rectangle.Y, (int)newWidth, (int)newHeight);
#endif
                InnerSetRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

                _orientation = value;

                switch (_orientation)
                {
                    case Orientation.Left:
                        Mask.Rotation = MathHelper.ToRadians(270);
                        break;
                    case Orientation.Top:
                        Mask.Rotation = MathHelper.ToRadians(0);
                        break;
                    case Orientation.Right:
                        Mask.Rotation = MathHelper.ToRadians(90);
                        break;
                    case Orientation.Bottom:
                        Mask.Rotation = MathHelper.ToRadians(180);
                        break;
                }

                //                else if (((_orientation == Orientation.Left || _orientation == Orientation.Right) &&
                //                         (value != Orientation.Left && value != Orientation.Right)) ||
                //                        ((_orientation == Orientation.Top || _orientation == Orientation.Bottom) &&
                //                         (value != Orientation.Top && value != Orientation.Bottom)))
                //                {
                //                    _orientation = value;
                //                    var newWidth = _rectangle.Height;
                //                    var newHeight = _rectangle.Width;
                //                    var newX = _rectangle.X;
                //                    if (newX + newWidth > EngineGlobals.Grid.WidthReal)
                //                    {
                //                        newX = EngineGlobals.Grid.WidthReal - newX;
                //                    }
                //                    var newY = _rectangle.Y;
                //                    if (newY + newHeight > EngineGlobals.Grid.HeightReal)
                //                    {
                //                        newY = EngineGlobals.Grid.HeightReal - newHeight;
                //                    }
                //                   // Mask.Rect = new Rectangle((int)newX, (int)newY, (int)_rectangle.Width, (int)_rectangle.Height);
                //#if EDITOR
                //                    rectangle = EngineGlobals.Grid.ToAlignedRectangle(new Rectangle((int)newX, (int)newY, (int)newWidth, (int)newHeight));
                //#else
                //                    var rectangle = new Rectangle((int)newX, (int)newY, (int)newWidth, (int)newHeight);
                //#endif
                //                    if (_orientation == Orientation.Left || _orientation == Orientation.Right)
                //                        Mask.Scale = new Vector2(_rectangle.Width / newWidth, newWidth / _rectangle.Width);
                //                    else
                //                    {
                //                        Mask.Scale = new Vector2(1.0f, 1.0f);
                //                    }
                //                    InnerSetRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                //                }
                //                else
                //                {
                //                   // rectangle = new Rectangle((int)_rectangle.X, (int)_rectangle.Y, (int)_rectangle.Width, (int)_rectangle.Height);
                //                    Mask.Scale = new Vector2(1.0f, 1.0f);
                //                    InnerSetRectangle(_rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height);
                //                }
                //                switch (_orientation)
                //                {
                //                    case Orientation.Left:
                //                        Mask.Rotation = MathHelper.ToRadians(270);
                //                        break;
                //                    case Orientation.Top:
                //                        Mask.Rotation = MathHelper.ToRadians(0);
                //                        break;
                //                    case Orientation.Right:
                //                        Mask.Rotation = MathHelper.ToRadians(90);
                //                        break;
                //                    case Orientation.Bottom:
                //                        Mask.Rotation = MathHelper.ToRadians(180);
                //                        break;
                //                }

                //if (_orientation != value && _orientation != 0)
                //{
                //    if (((_orientation == Orientation.Left || _orientation == Orientation.Right) &&
                //         (value != Orientation.Left && value != Orientation.Right)) ||
                //        ((_orientation == Orientation.Top || _orientation == Orientation.Bottom) &&
                //         (value != Orientation.Top && value != Orientation.Bottom)))
                //    {
                //        var newWidth = _rectangle.Height;
                //        var newHeight = _rectangle.Width;
                //        var newX = _rectangle.X;
                //        if (newX + newWidth > EngineGlobals.Grid.WidthReal)
                //        {
                //            newX = EngineGlobals.Grid.WidthReal - newX;
                //        }
                //        //else if (_rectangle.X - newX < 0)
                //        //{
                //        //    newX = 0;
                //        //}
                //        var newY = _rectangle.Y;
                //        if (newY + newHeight > EngineGlobals.Grid.HeightReal)
                //        {
                //            newY = EngineGlobals.Grid.HeightReal - newHeight;
                //        }
                //        //else if (Data.Y - _halfSize.Y < 0)
                //        //{
                //        //    newY = 0;
                //        //    _halfPos.Y = newY;
                //        //}
                //        //Data.Rect = new Rectangle((int) newX, (int) newY, (int) _halfSize.X*2, (int) _halfSize.Y*2);
                //        //_rectangle = new RectangleF(EngineGlobals.Grid.ToAlignedRectangle(_rectangle.GetRectangle()));
                //        if (value == Orientation.Right || value == Orientation.Left)
                //            Data.Scale = new Vector2(_rectangle.Width / newWidth, newWidth / _rectangle.Width);
                //        else
                //            Data.Scale = new Vector2(1.0f, 1.0f);
                //        SetRectangle(newX, newY, newWidth, newHeight);

                //    }
                //}
                //else if (_orientation == 0)
                //{
                //    if (value == Orientation.Right || value == Orientation.Left)
                //        Data.Scale = new Vector2(_rectangle.Height / _rectangle.Width, _rectangle.Width / _rectangle.Height);
                //    else
                //        Data.Scale = new Vector2(1.0f, 1.0f);
                //}
                //switch (value)
                //{
                //    case Orientation.Left:
                //        Data.Rotation = MathHelper.ToRadians(270);
                //        break;
                //    case Orientation.Top:
                //        Data.Rotation = MathHelper.ToRadians(0);
                //        break;
                //    case Orientation.Right:
                //        Data.Rotation = MathHelper.ToRadians(90);
                //        break;
                //    case Orientation.Bottom:
                //        Data.Rotation = MathHelper.ToRadians(180);
                //        break;
                //}


#if EDITOR
                FirePropertyChangedEvent("Orientation");
#endif

            }
        }

        public override Vector2 HalfSize
        {
            get
            {
                return base.HalfSize;
            }
            set
            {
                base.HalfSize = value;
                //if (Orientation == Orientation.Left || Orientation == Orientation.Right)
                //{
                //    Data.Scale = new Vector2((float)Data.Height / Data.Width, (float)Data.Width / Data.Height);
                //}
            }
        }

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public override Vector2 HalfPos
        {
            get
            {
                return base.HalfPos;
            }
            set
            {
                base.HalfPos = value;
            }
        }

#if EDITOR
        [DisplayName("Size")]
        [PropertyOrder(2)]
        [ContentSerializerIgnore]
        public virtual new System.Drawing.Size GridSize
        {
            get
            {
                return base.GridSize;
            }
            set
            {
                base.GridSize = value;
                //if (Orientation == Orientation.Left || Orientation == Orientation.Right)
                //{
                //    Data.Scale = new Vector2((float)Data.Height / Data.Width, (float)Data.Width / Data.Height);
                //}
            }
        }

        public override bool IsResizeAvailable(ResizeType resizeType)
        {
            return resizeType == ResizeType.BottomLeft || resizeType == ResizeType.Right || resizeType == ResizeType.BottomRight || resizeType == ResizeType.TopRight ||
                   resizeType == ResizeType.Top || resizeType == ResizeType.Left || resizeType == ResizeType.TopLeft || resizeType == ResizeType.Bottom;
        }

        public override void DoResize(ResizeType resizeDragType, Rectangle startRectangle, Rectangle newRectangle)
        {
            if (newRectangle.X + newRectangle.Width > EngineGlobals.Grid.WidthReal)
            {
                newRectangle.Width = EngineGlobals.Grid.WidthReal - newRectangle.X;
            }
            if (newRectangle.Y + newRectangle.Height > EngineGlobals.Grid.HeightReal)
            {
                newRectangle.Height = EngineGlobals.Grid.HeightReal - newRectangle.Y;
            }
            SetRectangle(newRectangle.X, newRectangle.Y, newRectangle.Width, newRectangle.Height);
        }

        public void EditorResetOrientationRectangle()
        {
            if (_orientation == Orientation.Top || _orientation == Orientation.Bottom)
                return;
            var newWidth = _rectangle.Height;
            var newHeight = _rectangle.Width;
            var newX = _rectangle.X;
            if (newX + newWidth > EngineGlobals.Grid.WidthReal)
            {
                newX = EngineGlobals.Grid.WidthReal - newX;
            }
            var newY = _rectangle.Y;
            if (newY + newHeight > EngineGlobals.Grid.HeightReal)
            {
                newY = EngineGlobals.Grid.HeightReal - newHeight;
            }
            SetRectangle(newX, newY, newWidth, newHeight);
        }

#endif

        protected override void SetRectangle(float x, float y, float width, float height)
        {
#if EDITOR
            FirePropertyChangingEvent("Rectangle");
#endif
            InnerSetRectangle(x, y, width, height);
#if EDITOR
            FirePropertyChangedEvent("Rectangle");
#endif
        }

        protected void InnerSetRectangle(float x, float y, float width, float height)
        {
            _rectangle.X = x;
            _rectangle.Y = y;
            _rectangle.Width = width;
            _rectangle.Height = height;
            _halfSize.X = width / 2;
            _halfSize.Y = height / 2;
            _halfPos.X = x + _halfSize.X;
            _halfPos.Y = y + _halfSize.Y;
            var rectangle = _rectangle.GetRectangle();

            Mask.Rect = rectangle;
        }

        public override CollidingObjectType CollidingType
        {
            get
            {
                return CollidingObjectType.Rectangle;
            }
            protected set
            {
                throw new Exception("Cannot change colliding type of Bounding Box.");
            }
        }

        protected override void SetOrigin()
        {
            Mask.Orgin = new Vector2(0,0);
        }

    }

    
    public abstract class CirclePhysicalObject : PhysicalObject
    {

        protected Direction _rotationDirection;
        protected int _rotation;

        public override CollidingObjectType CollidingType
        {
            get
            {
                return CollidingObjectType.Circle;
            }
            protected set
            {
                throw new Exception("Cannot change colliding type of Bounding Circle.");
            }
        }

#if EDITOR
        [DisplayName("Diameter")]
        [PropertyOrder(3)]
        [ContentSerializerIgnore]
        public virtual int Diameter
        {
            get { return GridSize.Width; }
            set
            {
                if (value < 1)
                    value = 1;
                else if (GridPos.X + value > EngineGlobals.Grid.GridRectangle.Width)
                {
                    value = EngineGlobals.Grid.GridRectangle.Width - GridPos.X;
                }
                if (value < 1)
                    value = 1;
                else if (GridPos.Y + value > EngineGlobals.Grid.GridRectangle.Height)
                {
                    value = EngineGlobals.Grid.GridRectangle.Height - GridPos.Y;
                }
                var realRect = EngineGlobals.Grid.ToRealRectangle(new Rectangle(GridPos.X, GridPos.Y, value, value));

                SetRectangle(realRect.X, realRect.Y, realRect.Width, realRect.Height);
            }
        }
#endif

#if EDITOR
        [PropertyOrder(9)]
#endif
        public virtual int Rotation
        {
            get { return _rotation; }
            set
            {
#if EDITOR
                FirePropertyChangingEvent("Rotation");
#endif
                _rotation = value;
                if (_rotation > 360) _rotation = value - 360;
                else if (_rotation < 0) _rotation = 360 + value;
                if (Mask != null)
                {
                    Mask.Rotation = MathHelper.ToRadians(_rotation);
                }

#if EDITOR
                FirePropertyChangedEvent("Rotation");
#endif
            }
        }

#if EDITOR
        [PropertyOrder(10)]
#endif
        public virtual float RotationSpeed
        {
            get { return _rotationSpeed; }
            set
            {
#if EDITOR
                FirePropertyChangingEvent("RotationSpeed");
#endif

                _rotationSpeed = value;
#if EDITOR
                FirePropertyChangedEvent("RotationSpeed");
#endif
            }
        }

#if EDITOR
        [PropertyOrder(11)]
#endif
        public virtual Direction RotationDirection
        {
            get { return _rotationDirection; }
            set
            {
#if EDITOR
                FirePropertyChangingEvent("RotationDirection");
#endif
                _rotationDirection = value;
#if EDITOR
                FirePropertyChangedEvent("RotationDirection");
#endif
            }
        }

#if EDITOR
        [Browsable(false)]
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

        public override bool IsResizeAvailable(ResizeType resizeType)
        {
            return resizeType == ResizeType.BottomLeft || resizeType == ResizeType.BottomRight || resizeType == ResizeType.TopRight ||
                  resizeType == ResizeType.TopLeft;
        }

        public override void DoResize(ResizeType resizeDragType, Rectangle startRectangle, Rectangle newRectangle)
        {
            switch (resizeDragType)
            {
                case ResizeType.TopLeft:
                    if (startRectangle.X - newRectangle.X > startRectangle.Y - newRectangle.Y)
                    {
                        newRectangle.Y = startRectangle.Y - (startRectangle.X - newRectangle.X);
                        newRectangle.Height = newRectangle.Width;
                    }
                    else if (startRectangle.Y - newRectangle.Y > startRectangle.X - newRectangle.X)
                    {
                        newRectangle.X = startRectangle.X - (startRectangle.Y - newRectangle.Y);
                        newRectangle.Width = newRectangle.Height;
                    }
                    if (newRectangle.Y < 0)
                    {
                        newRectangle.Y = 0;
                        newRectangle.Height = startRectangle.Y + startRectangle.Height;
                        newRectangle.X = startRectangle.X - startRectangle.Y;
                        newRectangle.Width = newRectangle.Height;
                    }
                    else if (newRectangle.X < 0)
                    {
                        newRectangle.X = 0;
                        newRectangle.Width = startRectangle.X + startRectangle.Width;
                        newRectangle.Y = startRectangle.Y - startRectangle.X;
                        newRectangle.Height = newRectangle.Width;
                    }
                    break;
                case ResizeType.TopRight:
                    if (newRectangle.Width > newRectangle.Height)
                    {
                        newRectangle.Y = startRectangle.Y + startRectangle.Height - newRectangle.Width;
                        newRectangle.Height = newRectangle.Width;
                    }
                    else if (newRectangle.Width < newRectangle.Height)
                    {
                        newRectangle.Width = newRectangle.Height;
                    }
                    if (newRectangle.Y < 0)
                    {
                        newRectangle.Y = 0;
                        newRectangle.Height = startRectangle.Y + startRectangle.Height;
                        newRectangle.Width = newRectangle.Height;
                    }
                    if (newRectangle.X + newRectangle.Width > EngineGlobals.Grid.WidthReal)
                    {
                        newRectangle.Width = EngineGlobals.Grid.WidthReal - newRectangle.X;
                        newRectangle.Y = startRectangle.Y + startRectangle.Height - newRectangle.Width;
                        newRectangle.Height = newRectangle.Width;
                    }
                    break;
                case ResizeType.BottomLeft:
                    if (newRectangle.Height > newRectangle.Width)
                    {
                        newRectangle.X = startRectangle.X + startRectangle.Width - newRectangle.Height;
                        newRectangle.Width = newRectangle.Height;
                    }
                    else if (newRectangle.Height < newRectangle.Width)
                    {
                        newRectangle.Height = newRectangle.Width;
                    }
                    if (newRectangle.X < 0)
                    {
                        newRectangle.X = 0;
                        newRectangle.Width = startRectangle.X + startRectangle.Width;
                        newRectangle.Height = newRectangle.Width;
                    }
                    if (newRectangle.Y + newRectangle.Height > EngineGlobals.Grid.HeightReal)
                    {
                        newRectangle.Height = EngineGlobals.Grid.HeightReal - newRectangle.Y;
                        newRectangle.X = startRectangle.X + startRectangle.Width - newRectangle.Height;
                        newRectangle.Width = newRectangle.Height;
                    }
                    break;
                case ResizeType.BottomRight:
                    if (newRectangle.Width > newRectangle.Height)
                    {
                        newRectangle.Height = newRectangle.Width;
                    }
                    else if (newRectangle.Width < newRectangle.Height)
                    {
                        newRectangle.Width = newRectangle.Height;
                    }
                    break;
                default:
                    throw new Exception("Unsuported resize type: " + resizeDragType);
            }
            if (newRectangle.X + newRectangle.Width > EngineGlobals.Grid.WidthReal)
            {
                newRectangle.Width = EngineGlobals.Grid.WidthReal - newRectangle.X;
                newRectangle.Height = newRectangle.Width;
            }
            if (newRectangle.Y + newRectangle.Height > EngineGlobals.Grid.HeightReal)
            {
                newRectangle.Height = EngineGlobals.Grid.HeightReal - newRectangle.Y;
                newRectangle.Width = newRectangle.Height;
            }
            SetRectangle(newRectangle.X, newRectangle.Y, newRectangle.Width, newRectangle.Height);
        }

#else
        [ContentSerializerIgnore]
        public virtual float R
        {
            get { return _halfSize.X; }
            set
            {
                SetRectangle(_rectangle.X, _rectangle.Y, value*2, value*2);
            }
        }

        [ContentSerializerIgnore]
        public virtual  int Diameter
        {
            get { return (int)_rectangle.Width; }
            set { SetRectangle(_rectangle.X, _rectangle.Y, value, value); }
        }

#endif

        protected override void SetRectangle(float x, float y, float width, float height)
        {
#if EDITOR
            FirePropertyChangingEvent("Rectangle");
#endif
            _rectangle.X = x;
            _rectangle.Y = y;
            _rectangle.Width = width;
            _rectangle.Height = height;
            _halfSize.X = width / 2;
            _halfSize.Y = height / 2;
            _halfPos.X = x + _halfSize.X;
            _halfPos.Y = y + _halfSize.Y;
            Mask.Rect = new Rectangle((int)_halfPos.X, (int)_halfPos.Y, (int)_rectangle.Width, (int)_rectangle.Height);
            if (SubMasks != null)
            {
                foreach (var gameObject in SubMasks)
                {
                    gameObject.Pos = Mask.Pos;
                }
            }
#if EDITOR
            FirePropertyChangedEvent("Rectangle");
#endif
        }

        

        protected override void SetOrigin()
        {
            Mask.Orgin = Animated ? _sprite.OriginCenter() : _image.OriginCenter();
        }
    }

}
