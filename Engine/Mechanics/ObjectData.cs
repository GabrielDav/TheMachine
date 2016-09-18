using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics
{

/*#if EDITOR
    [TypeConverter(typeof(PropertySorter))]
    public class ObjectData : IDisposable, ICloneable, INotifyPropertyChanged, INotifyPropertyChanging
    {

        protected Rectangle _rectangle;
        protected float _scale;
        protected float _layerDepgth;
        protected string _resourceId;
        protected string _name;

#if EDITOR
        public object Owner;
#endif
        

        [PropertyOrder(2)]
        public string ResourceId
        {
            get { return _resourceId; }
            set
            {
                PropertyChanging(this, new PropertyChangingEventArgs("ResourceId"));
                _resourceId = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ResourceId"));
            }
        }

        [PropertyOrder(3)]
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set
            {
                PropertyChanging(this, new PropertyChangingEventArgs("Rectangle"));
                _rectangle = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Rectangle"));

            }
        }

        [PropertyOrder(4)]
        public float Scale
        {
            get { return _scale; }
            set
            {
                PropertyChanging(this, new PropertyChangingEventArgs("Scale"));
                _scale = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Scale"));

            }
        }

        [PropertyOrder(5)]
        public float LayerDepth
        {
            get { return _layerDepgth; }
            set
            {
                PropertyChanging(this, new PropertyChangingEventArgs("LayerDepth"));
                _layerDepgth = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LayerDepth"));

            }
        }

        [PropertyOrder(1)]
        public string Name { get { return _name; }
            set {
                PropertyChanging(this, new PropertyChangingEventArgs("Name"));
                _name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name")); } }

        [Browsable(false)]
        public bool Animated { get; set; }

        [Browsable(false)]
        public GameObjectType TypeId { get; set; }

        public override string ToString()
        {
            return Name + " - " + TypeId;
        }

        public object Clone()
        {
            var tempPropertyChanged = PropertyChanged;
            var tempPropertyChanging = PropertyChanging;
            PropertyChanged = delegate { };
            PropertyChanging = delegate { };
            var clone = MemberwiseClone();
            PropertyChanged = tempPropertyChanged;
            PropertyChanging = tempPropertyChanging;
            return clone;
        }

        public void Dispose()
        {
            PropertyChanged = null;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public event PropertyChangingEventHandler PropertyChanging = delegate { };
    }
#else

    public class ObjectData
    {
        protected Rectangle _rectangle;
        protected float _scale;
        protected float _layerDepgth;
        protected string _resourceId;
        protected string _name;

        public string ResourceId
        {
            get { return _resourceId; }
            set { _resourceId = value; }
        }

        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public float LayerDepth
        {
            get { return _layerDepgth; }
            set { _layerDepgth = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool Animated { get; set; }

        public GameObjectType TypeId { get; set; }

        public override string ToString()
        {
            return Name + " - " + TypeId;
        }

    }

#endif*/
}
