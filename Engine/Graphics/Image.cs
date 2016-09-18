using System;
using System.IO;
using System.Text.RegularExpressions;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{

    public class GameTexture
    {
        public Texture2D Data { get; private set; }

        public GameTexture(string path)
        {
            Load(path);
        }

        public GameTexture(Texture2D texture)
        {
            Data = texture;
        }

        public void Load(string path)
        {
#if USE_LOCAL
            var fs = File.Open(EngineGlobals.DataPath + path, FileMode.Open);
            Data = Texture2D.FromStream(EngineGlobals.Device, fs);
            fs.Close();
#else

            var reg = new Regex(@"\.jpeg$|\.jpg$|\.png$|\.bmp$");
            path = reg.Replace(path, "");

            Data = EngineGlobals.ContentCache.Load<Texture2D>(path);
#endif
        }

        public void Load(Stream stream)
        {
            Data = Texture2D.FromStream(EngineGlobals.Device, stream);
        }
    }

    public class Image : GameObject
    {
        public Texture2D Texture { get; protected set; }

        /// <summary>
        /// Specify source rectangle from texture to draw
        /// </summary>
        public Rectangle? Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }

        //public void SetOrginToCenter()
        //{
        //    Orgin = new Vector2((float)Texture.Width / 2, (float)Texture.Height / 2);
        //}

        public Image(Rectangle rectangle)
        {
            _rect = rectangle;
            DrawRectangle = rectangle;
            _x = rectangle.X;
            _y = rectangle.Y;
            _frame = null;
            Color = Color.White;
        }

        public Image(GameTexture texture)
        {
            if (texture != null)
            {
                Initialize(texture, new Rectangle(0, 0, texture.Data.Width, texture.Data.Height), Color.White);
            }
        }

        public Image(GameTexture texture, Rectangle rect)
        {
            Initialize(texture, rect, Color.White);
        }

        public Image(GameTexture texture, Rectangle rect, Color color)
        {
            Initialize(texture, rect, color);
        }

        public void LoadTexture(GameTexture texture)
        {
            Texture = texture.Data;
        }

        protected void Initialize(GameTexture texture, Rectangle rect, Color color)
        {
            Texture = texture.Data;
            _rect = rect;
            DrawRectangle = rect;
            _x = rect.X;
            _y = rect.Y;
            _frame = null;
            Color = color;
        }

        public Vector2 OriginCenter()
        {
            return new Vector2(Texture.Bounds.Width/2, Texture.Bounds.Height/2);
        }

        [ContentSerializerIgnore]
        public override bool IgnoreCulling { get; set; }

        public override void Draw()
        {
            if (IsHidden)
            {
                return;
            }
#if DEBUG
            if (Texture.IsDisposed)
                throw new Exception("Texture is disposed for object " + this);
#endif

            EngineGlobals.Batch.Draw(Texture, DrawRectangle,
                                     _frame,
                                     Color, Rotation, _origin,
                                     Flip, LayerDepth);
        }

        //protected override Rectangle CalculateCornerRectangle()
        //{
        //    if (Orgin.X < EngineGlobals.Epsilon && Orgin.Y < EngineGlobals.Epsilon)
        //    {
        //        return _rect;
        //    }
        //    var ox = Orgin.X/Texture.Width;
        //    var oy = Orgin.Y/Texture.Height;
        //    return new Rectangle(_rect.X - (int) Math.Round(_rect.Width*ox, 0),
        //                         _rect.Y - (int) Math.Round(_rect.Height*oy, 0), _rect.Width, _rect.Height);

        //}
    }
}
