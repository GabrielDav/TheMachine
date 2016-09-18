using System;
#if EDITOR
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
#endif
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Image = Engine.Graphics.Image;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using RectangleF = Engine.Graphics.RectangleF;

namespace Engine.Mechanics
{

    public enum RescaleType
    {
        Horizontal, Vertical
    }

    public enum ScaleAttachSide
    {
        TopCenter, TopRight, TopLeft, BottomCenter, BottomRight, BottomLeft, Center, CenterRight, CenterLeft
    }

#if EDITOR
    [TypeConverter(typeof (PropertySorter))]
    public abstract class PhysicalObject : ICloneable, IDisposable, IEditorObject
#else
    public abstract class PhysicalObject : IDisposable
#endif
    {
        #region protected
        
        protected Vector2 _halfPos;
        protected Vector2 _halfSize;
        protected string _name;
        protected string _resourceId;
        protected int _resourceVariation = -1;
        protected Sprite _sprite;
        protected Image _image;
        protected RectangleF _rectangle;
        protected float _rotationSpeed;
        protected bool _disposed;

        #endregion

        #region BaseProperties

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public bool GlobalUpdate { get; protected set; }

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public bool IgnoreInGameTime { get; protected set; }

#if EDITOR
        [Browsable(false)]
        [ContentSerializerIgnore]
        public bool IgnoreGridBounds = true;
#endif

#if EDITOR
        [Browsable(false)] 
#endif
        public bool Animated { get; protected set; }

        [ContentSerializerIgnore]
#if EDITOR
        [Browsable(false)] 
#endif
        public GameObject Mask { get; protected set; }

#if EDITOR
        [Browsable(false)]
#endif
        public GameObject[] SubMasks { get; protected set; }

#if EDITOR
        [Browsable(false)] 
#endif
        public virtual CollidingObjectType CollidingType { get; protected set; }

#if EDITOR
        [Browsable(false)] 
#endif
        [ContentSerializerIgnore] 
        public Vector2 Direction = Vector2.Zero;

#if EDITOR
        [Browsable(false)] 
#endif
        [ContentSerializerIgnore]
        public bool InUse
        {
            get { return IsActivated || !Mask.IsHidden || !IgnoreCollision; }
            set
            {
                IsActivated = value;
                Mask.IsHidden = !value;
                IgnoreCollision = !value;
            }
        }

        [ContentSerializerIgnore] 
        public bool IgnoreGravity;

        [ContentSerializerIgnore] 
        public bool IgnoreCulling;

#if EDITOR
        [Browsable(false)] 
#endif
        public string TypeId { get; protected set; }

#if EDITOR
        [Browsable(false)]
        [ContentSerializerIgnore]
#endif
        public virtual bool IgnoresPhysics { get { return false; } }

        #endregion

        #region PositionProperties

#if EDITOR
        [Browsable(false)] 
#endif
        [ContentSerializerIgnore]
        public virtual Vector2 HalfSize
        {
            get { return _halfSize; }
            set { SetRectangle(_rectangle.X, _rectangle.Y, value.X*2f, value.Y*2f); }
        }

#if EDITOR
        [Browsable(false)] 
#endif
        [ContentSerializerIgnore]
        public virtual Vector2 HalfPos
        {
            get { return _halfPos; }
            set
            {
                SetRectangle(value.X - _halfSize.X, value.Y - _halfSize.Y, _rectangle.Width, _rectangle.Height);
            }
        }


#if EDITOR
        [Browsable(false)] 
#endif
        public RectangleF Rectangle
        {
            get { return _rectangle; }
            set { SetRectangle(value.X, value.Y, value.Width, value.Height); }
        }

#if EDITOR
        [Browsable(false)] 
#endif
        [ContentSerializerIgnore]
        public Point Pos
        {
            get { return new Point((int) _rectangle.X, (int) _rectangle.Y); }
            set { SetRectangle(value.X, value.Y, _rectangle.Width, _rectangle.Height); }
        }

        #endregion

        #region EditorProperties

#if EDITOR

        [PropertyOrder(1)]
        public string Name
        {
            get { return _name; }
            set
            {
                FirePropertyChangingEvent("Name");
                _name = value;
                FirePropertyChangedEvent("Name");
            }
        }

        [DisplayName("Size")]
        [PropertyOrder(2)]
        [ContentSerializerIgnore]
        public virtual Size GridSize
        {
            get
            {
                var cornerRect = EngineGlobals.Grid.ToCellRectangle(_rectangle.GetRectangle());
                return new Size(cornerRect.Width, cornerRect.Height);
            }
            set
            {
                var val = value;
                var pos = GridPos;
                if (CollidingType == CollidingObjectType.Circle)
                {
                    if (Math.Abs(_rectangle.Width - (val.Width*EngineGlobals.Grid.CellWidth)) > EngineGlobals.Epsilon)
                    {
                        val.Height = val.Width;
                    }
                    else
                    {
                        val.Width = val.Height;
                    }
                }
                if (val.Width < 1)
                    val.Width = 1;
                else if (pos.X + val.Width > EngineGlobals.Grid.GridRectangle.Width)
                {
                    val.Width = EngineGlobals.Grid.GridRectangle.Width - pos.X;
                    if (CollidingType == CollidingObjectType.Circle)
                        val.Height = val.Width;
                }
                if (val.Height < 1)
                    val.Height = 1;
                else if (pos.Y + val.Height > EngineGlobals.Grid.GridRectangle.Height)
                {
                    val.Height = EngineGlobals.Grid.GridRectangle.Height - pos.Y;
                    if (CollidingType == CollidingObjectType.Circle)
                        val.Width = val.Height;
                }
                var realRect = EngineGlobals.Grid.ToRealRectangle(new Rectangle(pos.X, pos.Y, val.Width, val.Height));
                SetRectangle(realRect.X, realRect.Y, realRect.Width, realRect.Height);

            }
        }

        [PropertyOrder(3)]
        [DisplayName("Position")]
        [ContentSerializerIgnore]
        public virtual Point GridPos
        {
            get
            {
                var cornerRect = EngineGlobals.Grid.ToCellRectangle(_rectangle.GetRectangle());
                return new Point(cornerRect.X, cornerRect.Y);
            }
            set
            {
                var val = value;
                if (val.X < 0)
                    val.X = 0;
                if (val.X + GridSize.Width > EngineGlobals.Grid.GridRectangle.Width)
                    val.X = EngineGlobals.Grid.GridRectangle.Width - GridSize.Width;
                if (val.Y < 0)
                    val.Y = 0;
                if (val.Y + GridSize.Height > EngineGlobals.Grid.GridRectangle.Height)
                    val.Y = EngineGlobals.Grid.GridRectangle.Height - GridSize.Height;
                SetRectangle(val.X * EngineGlobals.Grid.CellWidth, val.Y * EngineGlobals.Grid.CellHeight, _rectangle.Width, _rectangle.Height);

            }
        }

        [Browsable(false)]
        [TypeConverter(typeof (ListPropertyConverter))]
        public virtual string ResourceId
        {
            get { return _resourceId; }
            set
            {
                FirePropertyChangingEvent("ResourceId");
                _resourceId = value;
                FirePropertyChangedEvent("ResourceId");
            }
        }

        [PropertyOrder(10)]
        public virtual SpriteEffects Flip
        {
            get { return Mask.Flip; }
            set
            {
                FirePropertyChangingEvent("Flip");
                Mask.Flip = value;
                FirePropertyChangedEvent("Flip");
            }
        }

        [PropertyOrder(11)]
        public virtual float LayerDepth
        {
            get
            {
                return Mask.LayerDepth;
            }
            set
            {
                FirePropertyChangingEvent("LayerDepth");
                if (value > 0.9f)
                    Mask.LayerDepth = 0.9f;
                else if (value < 0.1f)
                    Mask.LayerDepth = 0.1f;
                else
                    Mask.LayerDepth = value;
                FirePropertyChangedEvent("LayerDepth");
            }
        }

        [PropertyOrder(12)]
        public virtual Color Color
        {
            get { return Mask.Color; }
            set
            {
                FirePropertyChangingEvent("Color");
                Mask.Color = value;
                FirePropertyChangedEvent("Color");
            }
        }

        [ContentSerializerIgnore]
        [Browsable(false)]
        public bool IgnoreCollision { get; set; }

        [ContentSerializerIgnore]
        [Browsable(false)]
        public Vector2 Orgin
        {
            get { return Mask.Orgin; }
            set { Mask.Orgin = value; }
        }

        [PropertyOrder(7)]
        [DisplayName("Variation")]
        [TypeConverter(typeof (VariationsConverter))]
        [ContentSerializerIgnore]
        public virtual int ResourceVariationEditor
        {
            get { return _resourceVariation + 1; }
            set
            {
                FirePropertyChangingEvent("ResourceVariationEditor");
                _resourceVariation = value - 1;
                if (Animated)
                {
                    if (_resourceVariation < 0 || _resourceVariation >= EngineGlobals.Resources.Sprites[_resourceId].Count)
                    {
                        MessageBox.Show("Resource variation index incorrect", "Invalid value");
                        FirePropertyChangedEvent("ResourceVariationEditor");
                        return;
                    }
                    _sprite.LoadSpriteData(EngineGlobals.Resources.Sprites[_resourceId][_resourceVariation]);
                }
                else
                {
                    if (_resourceVariation < 0 || _resourceVariation >= EngineGlobals.Resources.Textures[_resourceId].Count)
                    {
                        MessageBox.Show("Resource variation index incorrect", "Invalid value");
                        FirePropertyChangedEvent("ResourceVariationEditor");
                        return;
                    }
                    _image.LoadTexture(EngineGlobals.Resources.Textures[_resourceId][_resourceVariation]);
                }

                FirePropertyChangedEvent("ResourceVariationEditor");

            }
        }

        [ContentSerializerIgnore]
        [Browsable(false)]
        public virtual Vector2 EditorDestination
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

#endif
        #endregion

        #region GameProperties

#if !EDITOR

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        [ContentSerializerIgnore]
        public Vector2 Orgin
        {
            get { return Mask.Orgin; }
            set { Mask.Orgin = value; }
        }

        public string ResourceId
        {
            get { return _resourceId; }
            set
            {
                _resourceId = value;
            }
        }

        [ContentSerializerIgnore]
        public bool IgnoreCollision { get; set; }

        public virtual SpriteEffects Flip
        {
            get { return Mask.Flip; }
            set
            {
                Mask.Flip = value;
            }
        }

        public virtual float LayerDepth
        {
            get { return Mask.LayerDepth; }
            set { Mask.LayerDepth = value; }
        }

        public virtual Color Color
        {
            get { return Mask.Color; }
            set { Mask.Color = value; }
        }
#endif

#if EDITOR
        [Browsable(false)]
#endif
        public virtual int ResourceVariation
        {
            get { return _resourceVariation; }
            set { _resourceVariation = value; }
        }

        public virtual bool IsActivated { get; set; }

        #endregion

        #region SharedMethods

        protected virtual void Init()
        {
            if (!Animated)
            {
                Mask = new Image(null);
            }
            else
            {
                Mask = new Sprite(null);
            }
            Mask.Color = Color.White;
            Mask.LayerDepth = 0.5f;
        }

        protected abstract void SetRectangle(float x, float y, float width, float height);

        public virtual CollisionResponce Collide(CollisionResult collisionResult)
        {
            return CollisionResponce.Stop;
        }

        protected abstract void SetOrigin();

        public virtual void Load(string resourceId, int index)
        {
            _resourceId = resourceId;
            _resourceVariation = index;
            if (!Animated)
            {
                _image = ((Image)Mask);
                _image.LoadTexture(EngineGlobals.Resources.Textures[_resourceId][index]);
            }
            else
            {
                _sprite = ((Sprite)Mask);
                _sprite.LoadSpriteData(EngineGlobals.Resources.Sprites[_resourceId][index]);
            }
            SetOrigin();
            Mask.Owner = this;
        }

        public void SetNewData(GameObject gameObject)
        {
            if (Animated)
            {
                _sprite = (Sprite)gameObject;
            }
            else
            {
                _image = (Image)gameObject;
            }
            Mask = gameObject;
            gameObject.Owner = this;
        }

        public virtual void Update()
        {
            
        }

        public virtual void Draw()
        {
            if (Animated)
                _sprite.Draw();
            else
                _image.Draw();
        }

        

        public override string ToString()
        {
            return Name + " " + TypeId;
        }

        public virtual void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException("Object '" + Name + "' already disposed.");
            if (Animated)
            {
                if (_sprite != null)
                {
                    _sprite.Dispose();
                    _sprite = null;
                }
            }
            else
            {
                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }
            }
#if EDITOR
            PropertyChanged = null;
            PropertyChanging = null;
#endif
            _disposed = true;
        }

        #endregion

        #region EditorMethods

#if EDITOR

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event PropertyChangingEventHandler PropertyChanging = delegate { };

        public abstract void LoadDefault(string resourceId, int index, int subObjectId);

        public abstract bool IsResizeAvailable(ResizeType resizeType);

        public string EditorGetName()
        {
            return Name;
        }

        public abstract void DoResize(ResizeType resizeDragType, Rectangle startRectangle, Rectangle newRectangle);

        public IGraphicsObject EditorGetVisualObject()
        {
            return Mask;
        }

        public Vector2 EditorGetCenter()
        {
            return HalfPos;
        }

        public virtual bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return false;
        }

        public virtual void EditorDeleteObject()
        {
            
        }

        public virtual object Clone()
        {
            var tempPropertyChanged = PropertyChanged;
            var tempPropertyChanging = PropertyChanging;
            PropertyChanged = delegate { };
            PropertyChanging = delegate { };
            var data = Mask.Clone();
            var clone = MemberwiseClone();
            ((PhysicalObject) clone).SetNewData((GameObject) data);
            PropertyChanged = tempPropertyChanged;
            PropertyChanging = tempPropertyChanging;
            return clone;
        }

        public virtual void EditorPasteObject()
        {
            
        }

        protected void FirePropertyChangingEvent(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        protected void FirePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanging != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void HideDetailImages(bool hide){}

        public virtual void RemoveDetails(){}

        public virtual void UpdatePreviewRectangle(Rectangle rectangle){}

#endif

        #endregion

        #region GameMethods

#if !EDITOR

        public virtual object Clone()
        {
            var data = Mask.Clone();
            var clone = MemberwiseClone();
            ((PhysicalObject)clone).SetNewData((GameObject)data);
            return clone;
        }

#endif
        public  virtual void Move(Vector2 shift)
        {
            HalfPos += shift;
        }

        public void RescaleByRatio(RescaleType rescaleType, ScaleAttachSide side)
        {
            //int textureWidth;
            //int textureHeight;
            //if (Animated)
            //{
            //    textureWidth = _sprite.CurrentAnimation.Texture.Data.Width;
            //    textureHeight = _sprite.CurrentAnimation.Texture.Data.Height;
            //}
            //else
            //{
            //    textureWidth = _image.Texture.Width;
            //    textureHeight = _image.Texture.Height;
            //}
            //switch (rescaleType)
            //{
            //    case RescaleType.Vertical:
            //        var ratioY = (double)textureHeight/textureWidth;
            //        Mask.Scale = new Vector2(1.0f, (float)(ratioY));
            //        Mask.Orgin = new Vector2(textureWidth / 2f, textureHeight - textureWidth / 2f);
            //        break;
            //    case RescaleType.Horizontal:
            //        var ratioX = (double)textureWidth/textureHeight;
            //        Mask.Scale = new Vector2((float)(ratioX), 1.0f);
            //        Mask.Orgin = new Vector2(textureWidth - textureHeight / 2f, textureHeight / 2f);
            //        break;
            //}
            int textureWidth;
            int textureHeight;
            if (Animated)
            {
                textureWidth = _sprite.CurrentAnimation.Texture.Data.Width;
                textureHeight = _sprite.CurrentAnimation.Texture.Data.Height;
            }
            else
            {
                textureWidth = _image.Texture.Width;
                textureHeight = _image.Texture.Height;
            }
            float xScale;
            float yScale;
            if (rescaleType == RescaleType.Vertical)
            {
                xScale = 1.0f;
                yScale = (float)textureHeight / textureWidth;
            }
            else
            {
                xScale = (float)textureWidth / textureHeight;
                yScale = 1.0f;
            }
            Mask.Scale = new Vector2(xScale, yScale);
            float yOrigin;
            float xOrigin;
            switch (side)
            {
                case ScaleAttachSide.BottomCenter:
                    yOrigin = textureHeight - textureHeight / (2f * yScale);
                    xOrigin = textureWidth / 2f;
                    break;
                case ScaleAttachSide.BottomLeft:
                    yOrigin = textureHeight - textureHeight / (2f * yScale);
                    xOrigin = textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.BottomRight:
                    yOrigin = textureHeight - textureHeight / (2f * yScale);
                    xOrigin = textureWidth - textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.TopCenter:
                    yOrigin = textureHeight / (2f * yScale);
                    xOrigin = textureWidth / 2f;
                    break;
                case ScaleAttachSide.TopLeft:
                    yOrigin = textureHeight / (2f * yScale);
                    xOrigin = textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.TopRight:
                    yOrigin = textureHeight / (2f * yScale);
                    xOrigin = textureWidth - textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.CenterLeft:
                    yOrigin = textureHeight / 2f;
                    xOrigin = textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.CenterRight:
                    yOrigin = textureHeight / 2f;
                    xOrigin = textureWidth - textureWidth / (2f * xScale);
                    break;
                default:
                    yOrigin = textureHeight / 2f;
                    xOrigin = textureWidth / 2f;
                    break;
            }
            Mask.Orgin = new Vector2(xOrigin, yOrigin);
        }

       // public abstract void Reset();

        public void RescaleBySize(int newWidth, int newHeight, ScaleAttachSide side)
        {
            int textureWidth;
            int textureHeight;
            if (Animated)
            {
                textureWidth = _sprite.CurrentAnimation.Texture.Data.Width;
                textureHeight = _sprite.CurrentAnimation.Texture.Data.Height;
            }
            else
            {
                textureWidth = _image.Texture.Width;
                textureHeight = _image.Texture.Height;
            }
            var yScale = newHeight / _rectangle.Height;
            var xScale = newWidth / _rectangle.Width;
            Mask.Scale = new Vector2(xScale, yScale);
            float yOrigin;
            float xOrigin;
            switch (side)
            {
                case ScaleAttachSide.BottomCenter:
                    yOrigin = textureHeight - textureHeight / (2f * yScale);
                    xOrigin = textureWidth / 2f;
                    break;
                case ScaleAttachSide.BottomLeft:
                    yOrigin = textureHeight - textureHeight / (2f * yScale);
                    xOrigin = textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.BottomRight:
                    yOrigin = textureHeight - textureHeight / (2f * yScale);
                    xOrigin = textureWidth - textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.TopCenter:
                    yOrigin = textureHeight / (2f * yScale);
                    xOrigin = textureWidth / 2f;
                    break;
                case ScaleAttachSide.TopLeft:
                    yOrigin = textureHeight / (2f * yScale);
                    xOrigin = textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.TopRight:
                    yOrigin = textureHeight / (2f * yScale);
                    xOrigin = textureWidth - textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.CenterLeft:
                    yOrigin = textureHeight / 2f;
                    xOrigin = textureWidth / (2f * xScale);
                    break;
                case ScaleAttachSide.CenterRight:
                    yOrigin = textureHeight / 2f;
                    xOrigin = textureWidth - textureWidth / (2f * xScale);
                    break;
                default:
                    yOrigin = textureHeight / 2f;
                    xOrigin = textureWidth / 2f;
                    break;
            }
            Mask.Orgin = new Vector2(xOrigin, yOrigin);
        }

        public void SetSizeUnsafe(float newSizeX, float newSizeY)
        {
            var changeVector = new Vector2(newSizeX/2, newSizeY/2) - _halfSize;
            _halfSize += changeVector;
            _rectangle.Width = newSizeX;
            _rectangle.Height = newSizeY;
            _rectangle.X -= changeVector.X;
            _rectangle.Y -= changeVector.Y;
            Mask.Rect = new Rectangle((int)_halfPos.X, (int)_halfPos.Y, (int)_rectangle.Width, (int)_rectangle.Height);
        }

        public void ScaleBySize(int newWidth, int newHeight)
        {
            
        }

        #endregion
    }

#region Converters

#if EDITOR

    internal class ListPropertyConverter : StringConverter
    {
        public override bool
            GetStandardValuesSupported(
            ITypeDescriptorContext context)
        {
            //True - means show a Combobox
            //and False for show a Modal 
            return true;
        }

        public override bool
            GetStandardValuesExclusive(
            ITypeDescriptorContext context)
        {
            //False - a option to edit values 
            //and True - set values to state readonly
            return true;
        }

        public override StandardValuesCollection
            GetStandardValues(
            ITypeDescriptorContext context)
        {
            if (((PhysicalObject) context.Instance).Animated)
            {
                var collection = new List<string>();
                foreach (var sprite in EngineGlobals.Resources.Sprites)
                {
                    collection.Add(sprite.Key);
                }
                return new StandardValuesCollection(collection);
            }
            else
            {
                var collection = new List<string>();
                foreach (var texture in EngineGlobals.Resources.Textures)
                {
                    collection.Add(texture.Key);
                }
                return new StandardValuesCollection(collection);
            }
        }
    }

    public class VariationsConverter : Int32Converter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext
                                                            context)
        {
            if (!(context.Instance is PhysicalObject))
                return false;
            var obj = (PhysicalObject) context.Instance;
            if (obj.Animated)
            {
                if (EngineGlobals.Resources.Sprites[obj.ResourceId].Count < 2)
                    return false;
                return true;
            }
            if (EngineGlobals.Resources.Textures[obj.ResourceId].Count < 2)
                return false;
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext
                                                            context)
        {
            return true;
        }

        public override StandardValuesCollection
            GetStandardValues(ITypeDescriptorContext context)
        {
            var obj = (PhysicalObject) context.Instance;
            var values = new List<int>();
            var i = 0;
            values.AddRange(obj.Animated
                                ? EngineGlobals.Resources.Sprites[obj.ResourceId].Select(sprite => ++i)
                                : EngineGlobals.Resources.Textures[obj.ResourceId].Select(gameTexture => ++i));

            return new StandardValuesCollection(values);
        }

    }

#endif

#endregion

}
