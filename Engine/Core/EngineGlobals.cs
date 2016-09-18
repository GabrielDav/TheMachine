using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using Engine.ScreenManagement;
using GameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace Engine.Core
{
    public delegate void SimpleEvent(object sender);
    public delegate void PointEvent(object sender, Point location);
    public delegate void VoidEvent();
    public enum FontHorizontalAlign { Left, Center, Right }
    public enum FontVerticalAlign { Top, Center, Bottom }
    public enum Direction { Clockwise, Counterclockwise }

    public static class EngineGlobals
    {
        public static GraphicsDevice Device;
        public static Viewport Viewport;
        public static SpriteBatch Batch;
        public static GameTime GameTime;
        public static ContentManager ContentCache;
        public static Camera2D Camera2D;
        public static Input Input;
        public static string DataPath;
        public static GraphicsDeviceManager Graphics;
        public static StorageControl Storage;
        public static Grid Grid;
        public static float Epsilon = 0.00001f;
        public static ResourcesManager Resources;
        public static float PixelsPerMeter = 35;
        public static int MaxSpeed = 54;
        public static int MaximumSoundDistance = 800;
        public static SoundManager SoundManager;
        public static ParticleStorageManager ParticleStorageManager;
        public static TriggerManager TriggerManager;
        public static Random Random = new Random();
        public static GameTexture RegionTexture;
        public static ScreenManager ScreenManager;
        public static BackgroundManager Background;
        public static float TimeSpeed = 1.0f;
        


        public static bool InGame;
        public static bool MapIsLoading;

        // Physics
        public static Vector2 DefaultGravity = new Vector2(0, 12.5f);
        public static Vector2 Gravity = new Vector2(0, /*9.8f*/ 12.5f);

        // Functions
        public static Vector2 ToVector(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int) vector.X, (int) vector.Y);
        }

        public static string ToFormatedString(this Point point)
        {
            return string.Format("{0};{1}", point.X, point.Y);
        }

        public static Point ParsePoint(this string s)
        {
            if (!s.Contains(';'))
                throw new InvalidCastException("Invalid Point string");
            var x = Convert.ToInt32(s.Substring(0, s.IndexOf(';')));
            var y = Convert.ToInt32(s.Substring(s.IndexOf(';') + 1));
            return new Point(x,y);
        }

        public static string[] SplitAndKeep(this string s, char[] delims)
        {
            return InternalSplitAndKeep(s, delims).ToArray();
        }

        private static IEnumerable<string> InternalSplitAndKeep(string target, char[] delims)
        {
            var start = 0;
            int index;

            while ((index = target.IndexOfAny(delims, start)) != -1)
            {
                index++;
                index = Interlocked.Exchange(ref start, index);
                var token = target.Substring(index, start - index - 1);
                if (token != string.Empty)
                    yield return token;
                //var seperator = s.Substring(start - 1, 1);
                yield return target.Substring(start - 1, 1);
            }

            if (start < target.Length)
            {
                yield return target.Substring(start);
            }
        }

        public static Vector4 ToRgbaVector(this Color c)
        {
            return new Vector4(c.R, c.G, c.B, c.A);
        }

        public static Color ToColor(this Vector4 v)
        {
            return new Color((int)v.X, (int)v.Y, (int)v.Z, (int)v.W);
        }

        public static Point GetCenter(this Rectangle r)
        {
            return new Point(r.X + r.Width/2, r.Y + r.Height/2);
        }

        public static int Truncate(this float f)
        {
            if (f < 0)
                return (int)Math.Floor(f);
            if (f > 0)
                return (int) Math.Ceiling(f);
            return 0;
        }

        public static int Truncate(this double d)
        {
            if (d > 0)
                return (d - (int) d) > 0.01 ? (int) d + 1 : (int)d;
            if (d < 0)
                return (d + (int)d) > 0.01 ? (int)d + 1 : (int)d;
            return 0;
        }

        public static bool ItersectsCircle(this Rectangle rectangle, Rectangle circle)
        {
            var r = circle.Width/2;

            var centerX = circle.X + r;
            var centerY = circle.Y + r;

            var rectanglePoint = new Point[4];
            rectanglePoint[0] = new Point(rectangle.X, rectangle.Y);
            rectanglePoint[1] = new Point(rectangle.X + rectangle.Width, rectangle.Y);
            rectanglePoint[2] = new Point(rectangle.X, rectangle.Y + rectangle.Height);
            rectanglePoint[3] = new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

            if (rectanglePoint.Select(point => Math.Sqrt(Math.Pow((point.X - centerX), 2) + Math.Pow((point.Y - centerY), 2))).Any(distance => distance <= r))
            {
                return true;
            }

            return false;
        }

        public static Vector2 Center(this GameObject obj)
        {
            return new Vector2(obj.Pos.X + (obj.Rect.Width / 2f), obj.Pos.Y + (obj.Rect.Height / 2f));
        }

        public static float? Intersects(this Rectangle rectangle, Ray ray)
        {
            float num = 0f;
            float maxValue = float.MaxValue;
            if (Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if ((ray.Position.X < rectangle.Left) || (ray.Position.X > rectangle.Right))
                {
                    return null;
                }
            }
            else
            {
                float num11 = 1f / ray.Direction.X;
                float num8 = (rectangle.Left - ray.Position.X) * num11;
                float num7 = (rectangle.Right - ray.Position.X) * num11;
                if (num8 > num7)
                {
                    float num14 = num8;
                    num8 = num7;
                    num7 = num14;
                }
                num = MathHelper.Max(num8, num);
                maxValue = MathHelper.Min(num7, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }
            if (Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if ((ray.Position.Y < rectangle.Top) || (ray.Position.Y > rectangle.Bottom))
                {
                    return null;
                }
            }
            else
            {
                float num10 = 1f / ray.Direction.Y;
                float num6 = (rectangle.Top - ray.Position.Y) * num10;
                float num5 = (rectangle.Bottom - ray.Position.Y) * num10;
                if (num6 > num5)
                {
                    float num13 = num6;
                    num6 = num5;
                    num5 = num13;
                }
                num = MathHelper.Max(num6, num);
                maxValue = MathHelper.Min(num5, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }

            return new float?(num);
        }

        public static float GetElapsedInGameTime()
        {
            return (float)GameTime.ElapsedGameTime.TotalMilliseconds * TimeSpeed;
        }

        public static class Debug
        {
            public static int PlayerY;
            public static int TargetY;
            public static int FixedY;
            public static string TargetName;
        }
    }
}
