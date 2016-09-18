using System.ComponentModel;
using System.Drawing;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Image = Engine.Graphics.Image;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameLibrary.Objects
{
    public class ScoreDisplayDevice : MenuObject
    {
        protected Image _cilinder;
        protected Vector2 _cilinderOffset;
        protected RotateEffect _rotateEffect;

        public float StartRotation
        {
            get
            {
                if (_cilinder == null)
                    return 0;
                return MathHelper.ToDegrees(_cilinder.Rotation);
            }
            set
            {
                if (_cilinder != null)
                    _cilinder.Rotation = MathHelper.ToRadians(value);
            }
        }

        public int Interval
        { get; set; }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            _cilinder = new Image(EngineGlobals.Resources.Textures["ScoreDisplayDeviceDetails"][0])
            {
                Owner = this,
                LayerDepth = Mask.LayerDepth - 0.01f,
                Orgin = new Vector2(10, 10),
                Rotation = StartRotation
            };

            Controller.AddObject(_cilinder);
            _cilinderOffset = new Vector2(16, 16);
        }

#if EDITOR
        [ReadOnly(true)]
        public override Size GridSize { get; set; }
#endif

        public override SpriteEffects Flip
        {
            get { return base.Flip; }
            set
            {
                if (value == SpriteEffects.FlipHorizontally && Flip != SpriteEffects.FlipHorizontally)
                    StartRotation -= 180;
                else if (value != SpriteEffects.FlipHorizontally && Flip == SpriteEffects.FlipHorizontally)
                    StartRotation += 180;
                if (value == SpriteEffects.FlipHorizontally)
                    _cilinder.Flip = SpriteEffects.FlipVertically;
                else
                    _cilinder.Flip = SpriteEffects.None;
                base.Flip = value;
                SetRectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
            }
        }

#if EDITOR

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            SetRectangle(Rectangle.X, Rectangle.Y, 60, 30);
        }

#endif

        public override void Update()
        {
            if (_rotateEffect != null)
            {
                _rotateEffect.Update();
            }
            base.Update();
        }

        public void Begin()
        {
            _rotateEffect = new RotateEffect(_cilinder, MathHelper.ToDegrees(180), Interval);
        }

        protected override void SetRectangle(float x, float y, float width, float height)
        {
            base.SetRectangle(x, y, width, height);
            _cilinder.Rect = new Rectangle((int)(x + _cilinderOffset.X + (Flip == SpriteEffects.FlipHorizontally ? 30 : 0)), (int)(y + _cilinderOffset.Y + (Flip == SpriteEffects.FlipHorizontally ? -40 : 0)), 250, 50);
        }

#if EDITOR

        public override void EditorDeleteObject()
        {
            base.EditorDeleteObject();
            Controller.RemoveObject(_cilinder);
        }

        public override void RemoveDetails()
        {
            base.RemoveDetails();
            Controller.RemoveObject(_cilinder);
        }

#endif
    }
}
