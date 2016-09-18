using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
#if EDITOR

    public class PointTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] v = ((string)value).Split(new[] { ',' });
                return new PathPoint(int.Parse(v[0]), int.Parse(v[1]), float.Parse(v[2]));
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((PathPoint)value).X + "," + ((PathPoint)value).Y + "," + ((PathPoint)value).Speed.ToString("00");
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [Serializable]
    [TypeConverterAttribute(typeof(PointTypeConverter))]
#endif
    public struct PathPoint
    {
        private int _x;
        private int _y;
        private float _speed;
        private long _waitTime;

        public PathPoint(int x, int y)
        {
            _x = x;
            _y = y;
            _speed = 0;
            _waitTime = 0;
        }

        public PathPoint(int x, int y, float speed)
            : this(x, y)
        {
            _speed = speed;
        }

        public PathPoint(int x, int y, float speed, long waitTime)
            : this(x, y, speed)
        {
            _waitTime = waitTime;
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public long WaitTime
        {
            get { return _waitTime; }
            set { _waitTime = value; }
        }

        public Vector2 ToVector()
        {
            return new Vector2(X, Y);
        }

        public Point ToPoint()
        {
            return new Point(X, Y);
        }

    }
}
