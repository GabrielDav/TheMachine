using System;
using System.ComponentModel;
using Engine.Core;
using Engine.Mechanics;
using Microsoft.Xna.Framework.Content;

namespace Engine.Graphics
{

    public class SpriteData : IDisposable
    {
        public float Version = 1.0f;
        public Animation[] Animations;
        protected bool _disposed;

        public event SimpleEvent Disposing;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            if (Disposing != null)
            {
                Disposing(this);
            }

            foreach (var animation in Animations)
            {
                animation.Texture.Data.Dispose();
            }
            Animations = null;
            _disposed = true;
            
        }
    }
#if EDITOR
    [TypeConverter(typeof(PropertySorter))]
    public class Animation : INotifyPropertyChanging, INotifyPropertyChanged
    {
        protected string _path;
        protected string _name;
        protected int _frames;
        protected int _frameWidth;
        protected int _frameHeight;
        protected long _speed;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        [ContentSerializerIgnore]
        [Browsable(false)]
        public GameTexture Texture;

        [PropertyOrder(1)]
        [ReadOnly(true)]
        public string Path
        {
            set
            {
                _path = value;
                if (EngineGlobals.ContentCache != null)
                    Texture = new GameTexture(_path);
            }
            get { return _path; }
        }

        [PropertyOrder(5)]
        public string Name
        {
            get { return _name; }
            set
            {
                ExecuteChanging("Name");
                _name = value;
                ExecuteChanged("Name");
            }
        }

        [PropertyOrder(10)]
        public int Frames
        {
            get { return _frames; }
            set
            {
                ExecuteChanging("Frames");
                _frames = value;
                ExecuteChanged("Frames");
            }
        }

        [PropertyOrder(15)]
        public int FrameWidth
        {
            get { return _frameWidth; }
            set
            {
                ExecuteChanging("FrameWidth");
                _frameWidth = value;
                ExecuteChanged("FrameWidth");
            }
        }


        [PropertyOrder(20)]
        public int FrameHeight
        {
            get { return _frameHeight; }
            set
            {
                ExecuteChanging("FrameHeight");
                _frameHeight = value;
                ExecuteChanged("FrameHeight");
            }
        }

        [PropertyOrder(25)]
        public long Speed
        {
            get { return _speed; }
            set
            {
                ExecuteChanging("Speed");
                _speed = value;
                ExecuteChanged("Speed");
            }
        }

        [ContentSerializer(Optional = true)]
        public bool Default;

        protected void ExecuteChanging(string name)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        protected void ExecuteChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public override string ToString()
        {
            return Name + " - Animation";
        }

        public void SetPathPassive(string path)
        {
            _path = path;
        }

        
    }
#else
    public class Animation
    {
        protected string _path;

        public string Path
        {
            set
            {
                _path = value;
                if (EngineGlobals.ContentCache != null)
                    Texture = new GameTexture(_path);
            }
            get { return _path; }
        }

        public string Name;
        public int Frames;
        public int FrameWidth;
        public int FrameHeight;
        public long Speed;
        [ContentSerializer(Optional = true)]
        public bool Default;

        [ContentSerializerIgnore]
        public GameTexture Texture;
    }

#endif
    
}
