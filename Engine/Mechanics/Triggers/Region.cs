using System;
using System.Collections.Generic;
using System.ComponentModel;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Mechanics.Triggers
{
    public delegate void RegioneEnterEventHandler(object sender, PhysicalObject enteringObject);

    public delegate void RegionLeaveEventHandler(object sender, PhysicalObject leavingObject);

#if EDITOR
    public class Region : IGraphicsObject, INotifyPropertyChanging, INotifyPropertyChanged, IEditorObject, IDynamic
#else
    public class Region : IGraphicsObject, IDynamic
#endif
    {
        protected List<PhysicalObject> _items;
        protected bool _drawable;
        protected RectangleF _rectangle;
        protected Color _color;
        protected string _name;
        protected bool _resizeMode;
        protected bool _inRegion;

        [Browsable(false)]
        [ContentSerializerIgnore] 
        public PhysicalObject Object;

        public string CheckedObjectName { get; set; }

        [Browsable(false)]
        [ContentSerializerIgnore]
        public float LayerDepth { get; set; }

        [Browsable(false)]
        [ContentSerializerIgnore]
        public string ResourceId { get; set; }

        [Browsable(false)]
        [ContentSerializerIgnore]
        public bool Animated { get; private set; }

        [Browsable(false)]
        [ContentSerializerIgnore]
        public bool Drawable
        {
            get { return _drawable; }
            set { _drawable = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                ExecuteNotify("Name", false);
                _name = value;
                ExecuteNotify("Name", true);
            }
        }

        [Browsable(false)]
        public RectangleF Rectangle
        {
            get { return _rectangle; }
            set
            {
                if (_resizeMode)
                {
                    _rectangle = value;
                    return;
                }
                ExecuteNotify("Rectangle", false);
                _rectangle = value;
                var items = new List<PhysicalObject>();
                foreach (var physicalObject in _items)
                {
                    if (_rectangle.Contains(physicalObject.Rectangle))
                        items.Add(physicalObject);
                }
                _items = items;
                ExecuteNotify("Rectangle", true);
            }
        }

        [Browsable(false)]
        public Color Color
        {
            get { return _color; }
            set
            {
                ExecuteNotify("Color", false);
                _color = value;
                ExecuteNotify("Color", true);
            }
        }

#if EDITOR

        protected RegionColor _editorColor;

        [DisplayName("Rectangle")]
        [ContentSerializerIgnore]
        public Rectangle EditorRectangle
        {
            get { return _rectangle.GetRectangle(); }
            set
            {
                ExecuteNotify("EditorRectangle", false);
                _rectangle = new RectangleF(value);
                ExecuteNotify("EditorRectangle", true);
            }
        }

        public enum RegionColor{ Red, Green, Blue }

        [DisplayName("Color")]
        [ContentSerializerIgnore]
        public RegionColor EditorColor
        {
            get
            {
                if (Color.R > 100)
                    return RegionColor.Red;
                if (Color.G > 100)
                    return RegionColor.Green;
                return RegionColor.Blue;
            }
            set
            {
                ExecuteNotify("EditorColor", false);
                var a = _color.A;
                switch (value)
                {
                    case RegionColor.Red:
                        _color = Color.Red;
                        break;
                    case RegionColor.Green:
                        _color = Color.Green;
                        break;
                    case RegionColor.Blue:
                        _color = Color.Blue;
                        break;
                }
                _color.A = a;
                ExecuteNotify("EditorColor", true);
            }
        }

        [ContentSerializerIgnore]
        [Browsable(false)]
        public virtual Vector2 EditorDestination { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

#endif
        [Browsable(false)]
        [ContentSerializerIgnore]
        public bool StaticPosition { get { return false; } set { } }
        [ContentSerializerIgnore]
        public bool IgnoreCulling { get { return false; } set { } }
        [ContentSerializerIgnore]
        public Rectangle Rect { get { return Rectangle.GetRectangle(); } set { Rectangle = new RectangleF(value);} }
        [ContentSerializerIgnore]
        public Rectangle CornerRectangle { get { return Rect; } }
        public event SimpleEvent OnPositionTypeChanged;
        public event RegioneEnterEventHandler ObjectEnter;
        public event RegionLeaveEventHandler ObjectLeave;
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public Region(RectangleF rectangle, Color color, bool drawable)
        {
            _rectangle = rectangle;
            _color = color;
            _drawable = drawable;
            _items = new List<PhysicalObject>();
        }

        public Region()
        {
            _drawable = false;
            _items = new List<PhysicalObject>();
        }

#if EDITOR
        public bool IsResizeAvailable(ResizeType resizeType)
        {
            return resizeType == ResizeType.Top || resizeType == ResizeType.TopRight || resizeType == ResizeType.Right || resizeType == ResizeType.BottomRight ||
                resizeType == ResizeType.Bottom || resizeType == ResizeType.BottomLeft || resizeType == ResizeType.Left || resizeType == ResizeType.TopLeft;
        }

        public string EditorGetName()
        {
            return Name;
        }

        public void DoResize(ResizeType resizeDragType, Rectangle startRectangle, Rectangle newRectangle)
        {
            if (newRectangle.X + newRectangle.Width > EngineGlobals.Grid.WidthReal)
            {
                newRectangle.Width = EngineGlobals.Grid.WidthReal - newRectangle.X;
            }
            if (newRectangle.Y + newRectangle.Height > EngineGlobals.Grid.HeightReal)
            {
                newRectangle.Height = EngineGlobals.Grid.HeightReal - newRectangle.Y;
            }
            Rectangle = new RectangleF(newRectangle.X, newRectangle.Y, newRectangle.Width, newRectangle.Height);
        }
#endif

        public IGraphicsObject EditorGetVisualObject()
        {
            return this;
        }

        public Vector2 EditorGetCenter()
        {
            return new Vector2(_rectangle.X + _rectangle.Width/2f, _rectangle.Y + _rectangle.Height/2f);
        }

        public bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return false;
        }

        public void EditorDeleteObject()
        {
            
        }

        protected void ExecuteNotify(string name, bool changed)
        {
            if (changed)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
            else
            {
                if (PropertyChanging != null)
                    PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        public void CheckObject(PhysicalObject obj)
        {
            if (_items.Contains(obj))
            {
                if (!_rectangle.Contains(obj.Rectangle))
                {
                    _items.Remove(obj);
                    if (ObjectLeave != null)
                        ObjectLeave(this, obj);
                }
            }
            else
            {
                if (!_rectangle.Contains(obj.Rectangle))
                {
                    _items.Add(obj);
                    if (ObjectEnter != null)
                        ObjectEnter(this, obj);
                }
            }
        }

        public void BeginResize()
        {
            _resizeMode = true;
        }

        public void EndResize()
        {
            _resizeMode = false;
            Rectangle = _rectangle;
        }

        public void Draw()
        {
            if (!_drawable)
                return;
            EngineGlobals.Batch.Draw(EngineGlobals.RegionTexture.Data, _rectangle.GetRectangle(), null, _color, 0f, Vector2.Zero, SpriteEffects.None, 0.95f);
        }

        public void Draw(Color clr)
        {
            if (!_drawable)
                return;
            EngineGlobals.Batch.Draw(EngineGlobals.RegionTexture.Data, _rectangle.GetRectangle(), null, clr, 0f, Vector2.Zero, SpriteEffects.None, 0.95f);
        }

        public object Clone()
        {
            var clone = (Region) MemberwiseClone();
            clone._items = new List<PhysicalObject>();

            return clone;
        }

        public void Dispose()
        {
            _items.Clear();
        }

        public override string ToString()
        {
            return Name;
        }

        public void Update()
        {
            if (!_inRegion)
            {
                if (this.Rectangle.Contains(Object.Rectangle))
                {
                    _inRegion = true;
                    EngineGlobals.TriggerManager.ActionOccured((int)EventType.ObjectEntersRegion,
                                                               new EventParams()
                                                                   {
                                                                       TriggeringObject = Object,
                                                                       TriggeringRegion = this
                                                                   });
                }
            }
            else
            {
                if (!this.Rectangle.Contains(Object.Rectangle))
                {
                    _inRegion = false;
                    EngineGlobals.TriggerManager.ActionOccured((int)EventType.ObjectLeavesRegion,
                                                               new EventParams()
                                                               {
                                                                   TriggeringObject = Object,
                                                                   TriggeringRegion = this
                                                               });
                }
            }
        }
    }
}
