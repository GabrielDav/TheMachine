using System;
using System.ComponentModel;
#if EDITOR
using System.Drawing;
#endif
using Engine.Core;
using Engine.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;
using Image = Engine.Graphics.Image;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameLibrary.Objects
{
    public class HintBox : MenuObject
    {
        protected bool _drawPath = false;
        protected Image _path;
        protected Vector2 _hintPoint;
        protected Image _underline;

        public override bool IgnoresPhysics
        {
            get { return true; }
        }

        public Vector2 HintPoint
        {
            get { return _hintPoint; }
            set
            {
                _hintPoint = value;
                CalculatePath();
            }
        }

        public override Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                CalculatePath();
            }
        }

        public FontVerticalAlign TextVerticalAlign
        {
            get { return TextRegion.VerticalAlign; }
            set { TextRegion.VerticalAlign = value; }
        }

        public FontHorizontalAlign TextHorizontalAlign
        {
            get { return TextRegion.HorizontaAlign; }
            set { TextRegion.HorizontaAlign = value; }
        }

        public override int ResourceVariation
        {
            get { return 0; }
            set { base.ResourceVariation = 0; }
        }

#if EDITOR
        [ReadOnly(false)]
        public override Size GridSize
        {
            get { return base.GridSize; }
            set { base.GridSize = value; }
        }

        public override Vector2 EditorDestination
        {
            get { return HintPoint; }
            set { HintPoint = value; }
        }

        [ReadOnly(true)]
        public override int ResourceVariationEditor
        {
            get { return 1; }
            set { base.ResourceVariation = 1; }
        }
#endif

        public HintBox()
        {
            TypeId = GameObjectType.Hint.ToString();
            TextRegion.Font = GameGlobals.Font;
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            _path = new Image(EngineGlobals.Resources.Textures[TypeId][1])
            {
                LayerDepth = LayerDepth + 0.01f,
                Owner = this,
                IgnoreCulling = true
            };
            _path.Orgin = _path.OriginCenter();
            
            _underline = new Image(EngineGlobals.Resources.Textures[TypeId][1])
            {
                LayerDepth = LayerDepth - 0.01f,
                Owner = this
            };
            CalculatePath();
            Controller.AddObject(_path);
            Controller.AddObject(_underline);
            
        }

        protected virtual void CalculatePath()
        {
            if (_path == null)   
                return;
            var pos = Orientation == Orientation.Right
                ? HalfPos + HalfSize
                : HalfPos + new Vector2(-HalfSize.X, HalfSize.Y);
            var dist = Vector2.Distance(pos, HintPoint);
            if (dist < EngineGlobals.Epsilon || HintPoint == Vector2.Zero)
            {
                _path.IsHidden = true;
                _drawPath = false;
                return;
            }
            _path.IsHidden = false;
            
            var moveVector = pos + ((HintPoint - pos) / 2);
            _path.Rotation = (float)Math.Atan2(HintPoint.Y - pos.Y, HintPoint.X - pos.X);
            _path.Rect = new Rectangle((int)moveVector.X, (int)moveVector.Y, (int)dist, 2);
            _underline.Rect = new Rectangle((int)(HalfPos.X - HalfSize.X), (int)(HalfPos.Y + HalfSize.Y) - 1, (int)HalfSize.X * 2, 2);
        }

        public override void Draw()
        {
            base.Draw();
        }

        protected override void Init()
        {
            base.Init();
        }

        protected override void SetRectangle(float x, float y, float width, float height)
        {
            base.SetRectangle(x, y, width, height);
            CalculatePath();
        }


#if EDITOR
        public override bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return flag == DebugFlag.Destination;
        }

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Orientation = Orientation.Left;
            base.LoadDefault(resourceId, index, subObjectId);
            TextVerticalAlign = FontVerticalAlign.Bottom;
            TextHorizontalAlign = FontHorizontalAlign.Left;
            
        }
#endif
    }
}
