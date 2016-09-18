using System;
using System.ComponentModel;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    /// <summary>
    /// Main SObject from which inherits all the other graphical objects
    /// </summary>
#if WINDOWS
    public abstract class GameObject : IDisposable, ICloneable, IGraphicsObject
#else
    public abstract class GameObject : IDisposable, IGraphicsObject
#endif
    {
        protected float _x;
        protected float _y;

        /// <summary>
        /// Object rectangle
        /// </summary>
        protected Rectangle _rect;

        protected Rectangle _prevRect;

        protected Rectangle? _frame;

        protected float _rotation;

        protected bool _staticPosition;

        protected Vector2 _offset;

        /// <summary>
        /// Object depth(z position) in screen, 1 - in back, 0 - in front
        /// </summary>
        protected float _layerDepth;

        protected Vector2 _scale = new Vector2(1.0f, 1.0f);

        /// <summary>
        /// Is object hidden
        /// </summary>
        protected bool _isHidden;

        protected Vector2 _origin = new Vector2(0,0);

        public object Owner;

        [ContentSerializerIgnore]
        public float MoveSpeed;

        public Vector2 Orgin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                SetDrawRectangle();
            }
        }

        [ContentSerializerIgnore]
        public Rectangle CornerRectangle { get; protected set; }

        public Rectangle DrawRectangle { get; protected set; }

        public SpriteEffects Flip = SpriteEffects.None;

        /// <summary>
        /// Object width
        /// </summary>
        public int Width
        {
            set
            {
                _rect.Width = value;
                if (_prevRect.Width != _rect.Width)
                {
                    if (OnSizeChanged != null)
                        OnSizeChanged(this);
                }
                _prevRect = _rect;
                SetDrawRectangle();
            }
            get { return (int)_rect.Width; }
        }

        /// <summary>
        /// Object height
        /// </summary>
        public int Height
        {
            set
            {
                _rect.Height = value;
                if (_prevRect.Height != _rect.Height)
                {
                    if (OnSizeChanged != null)
                        OnSizeChanged(this);
                }
                _prevRect = _rect;
                SetDrawRectangle();
            }
            get { return (int)_rect.Height; }
        }

        /// <summary>
        /// Object position X, on set call's OnPositionChanged
        /// </summary>
        public float X
        {
            set
            {
                _x = value;
                _rect.X = (int)_x;
                SetDrawRectangle();
                if (OnPositionChanged != null)
                    OnPositionChanged(this);
            }
            get { return _x; }
        }

        /// <summary>
        /// Object position Y, on set call's OnPositionChanged
        /// </summary>
        public float Y
        {
            set
            {
                _y = value;
                _rect.Y = (int)_y;
                SetDrawRectangle();
                if (OnPositionChanged != null)
                    OnPositionChanged(this);
            }
            get { return _y; }
        }

        /// <summary>
        /// Object postion vector value, on set call's OnPositionChanged
        /// </summary>
        public virtual Vector2 Pos
        {
            get { return new Vector2(_x, _y); }
            set
            {
                _rect.X = (int) value.X;
                _rect.Y = (int) value.Y;
                _x = value.X;
                _y = value.Y;
                SetDrawRectangle();
                if (OnPositionChanged != null)
                    OnPositionChanged(this);
            }
        }

        [ContentSerializerIgnore]
        public abstract bool IgnoreCulling { get; set; }

        /// <summary>
        /// Object Recatngle value, on set call's OnPositionChanged
        /// </summary>
        public Rectangle Rect
        {
            set
            {
                _rect = value;
                _x = _rect.X;
                _y = _rect.Y;
                if (_prevRect.X != _rect.X || _prevRect.Y != _rect.Y)
                {
                    if (OnPositionChanged != null)
                        OnPositionChanged(this);
                }
                if (_rect.Width != _prevRect.Width || _rect.Height != _prevRect.Height)
                {
                    if (OnSizeChanged != null)
                        OnSizeChanged(this);
                }
                _prevRect = _rect;
                SetDrawRectangle();
            }
            get { return _rect; }
        }

        public Vector2 Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                SetDrawRectangle();
            }
        }

        /// <summary>
        /// Object Transparency(alpha value) percentage, 0 - completely invisible, 100 - not transparent
        /// </summary>
        public int Transparency
        {
            get { return (int) Math.Round(((double) Color.A)*100/255); }
            set
            {
                if ((value <= 100) && (value >= 0))
                    Color.A = (byte) Math.Round(((double) value)*255/100);
                else
                    throw new Exception(string.Format("Trying to set transparency to {0}%", value));
            }
        }

        /// <summary>
        /// Object depth(z position) in screen from 0 to 1, 1 - in back, 0 - in front
        /// </summary>
        public float LayerDepth
        {
            get { return _layerDepth; }
            set
            {
                if (value > 1.0f)
                    _layerDepth = 1.0f;
                else if (value < 0.0f)
                    _layerDepth = 0.0f;
                else
                    _layerDepth = value;
            }
        }

        /// <summary>
        /// Object scale vector value, default V(1.0f, 1.0f)(no scale)
        /// </summary>
        public Vector2 Scale
        {
            get { return _scale; }
            set { 
                _scale = value;
                SetDrawRectangle();
            }
        }

        /// <summary>
        /// Is object hidden
        /// </summary>
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                _isHidden = value;
                if (OnVisibilityChanged != null)
                    OnVisibilityChanged(this);

            }
        }

        /// <summary>
        /// Object rotation in radians
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                if (_origin == Vector2.Zero)
                {
                    _rotation = value;
                    SetDrawRectangle();
                }
                else
                {
                    _rotation = value;
                }
            }
        }

        /// <summary>
        /// Object color, default argb(255,255,255,255)
        /// </summary>
        public Color Color;

        public virtual bool StaticPosition
        {
            get { return _staticPosition; }
            set
            {
                if (value != _staticPosition)
                {
                    _staticPosition = value;
                    if (OnPositionTypeChanged != null)
                    {
                        OnPositionTypeChanged(this);
                    }
                }
            }
        }

        /// <summary>
        /// On object position changed
        /// </summary>
        public event SimpleEvent OnPositionChanged;

        public event SimpleEvent OnPositionTypeChanged;

        public event SimpleEvent OnSizeChanged;

        /// <summary>
        /// On IsHidden value changed
        /// </summary>
        public event SimpleEvent OnVisibilityChanged;

        public event SimpleEvent OnDispose;

        public abstract void Draw();

        //protected abstract Rectangle CalculateCornerRectangle();

        public virtual void Dispose()
        {
            OnPositionChanged = null;
            OnPositionChanged = null;
            if (OnDispose != null)
                OnDispose(this);
            OnDispose = null;
        }

        protected virtual void SetDrawRectangle()
        {
            if (_origin == Vector2.Zero)
            {
                var v = Vector2.Transform(new Vector2(_rect.Width * _scale.X / 2f, _rect.Height * _scale.Y / 2f), Matrix.CreateRotationZ(_rotation));
                DrawRectangle = new Rectangle((int)Math.Round(_rect.X + (_rect.Width / 2f) - v.X, 4), (int)Math.Round(_rect.Y + (_rect.Height / 2f) - v.Y, 4), (int)Math.Round(_rect.Width * _scale.X, 4),
                                          (int)Math.Round(_rect.Height * _scale.Y, 4));
                CornerRectangle = new Rectangle(_rect.X, _rect.Y, _rect.Width, _rect.Height);
            }
            else
            {
                DrawRectangle = new Rectangle(_rect.X, _rect.Y, (int)Math.Round(_rect.Width * _scale.X, 4),
                                          (int)Math.Round(_rect.Height * _scale.Y, 4));
                CornerRectangle = new Rectangle(_rect.X - _rect.Width/2, _rect.Y - _rect.Height/2, _rect.Width, _rect.Height);
            }
            //DrawRectangle = new Rectangle(_rect.X + (int)_offset.X, _rect.Y + (int)_offset.Y, (int)Math.Round(_rect.Width * _scale.X, 4),
            //                              (int)Math.Round(_rect.Height * _scale.Y, 4));
        }

        public object Clone()
        {
            var tempOnPositionChanged = OnPositionChanged;
            var tempOnSizeChanged = OnSizeChanged;
            var tempOnVisibilityChanged = OnVisibilityChanged;
            var tempOnDispose = OnDispose;
            OnVisibilityChanged = null;
            OnPositionChanged = null;
            OnSizeChanged = null;
            OnDispose = null;
            var result = MemberwiseClone();
            OnDispose = tempOnDispose;
            OnSizeChanged = tempOnSizeChanged;
            OnPositionChanged = tempOnPositionChanged;
            OnVisibilityChanged = tempOnVisibilityChanged;
            return result;
        }

    }
}
