using System.ComponentModel;
#if EDITOR
using System.Drawing;
#endif
using Engine.Core;
using Engine.Graphics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameLibrary.Objects
{
    public class MenuObject : Plane, IMenuBtn
    {
        protected bool _isHiden;

        [ContentSerializerIgnore]
        public TextRegion TextRegion;

        protected TextRegion _shadowTextRegion;
        protected ScaleEffect _rescaleEffect;
        protected ColorEffect _colorEffect;

        public string Text
        {
            get { return TextRegion.Text; }
            set { TextRegion.Text = value; }
        }

        [ContentSerializerIgnore]
        public virtual bool IsHidden
        {
            get { return _isHiden; }
            set
            {
                TextRegion.IsHidden = value;
                Mask.IsHidden = value;
                IsActivated = !value;
                _isHiden = value;
            }
        }

#if EDITOR
        public override Size GridSize
        {
            get { return base.GridSize; }
            set { base.GridSize = value; }
        }
#endif

        public Color FontColor
        {
            get { return TextRegion.Color; }
            set
            {
                TextRegion.Color = value;
            }
        }

        public MenuObject()
        {
            Animated = false;
            TypeId = GameObjectType.MenuObject.ToString();
            Init();
           /* if (FontColor == null)
                FontColor = Color.Black;
            else
            {
                var i = 0;
            }*/
            TextRegion = new TextRegion(new Rectangle(), GameGlobals.MenuGlobals.MenuFont, Color.Black, "", true)
            {
                Owner = this,
                StaticPosition = false,
                HorizontaAlign = FontHorizontalAlign.Center,
                VerticalAlign = FontVerticalAlign.Center
            };
        }

        public virtual void ShowPressAnimation()
        {
            if (_shadowTextRegion == null)
                _shadowTextRegion = new TextRegion(TextRegion.Rect, GameGlobals.MenuGlobals.MenuFont, Color.Black, Text, true);
            _shadowTextRegion.Rect = TextRegion.Rect;
            _shadowTextRegion.Scale = new Vector2(1,1);
            _shadowTextRegion.Color = new Color(0, 0, 0, 255);
            _rescaleEffect = new ScaleEffect(_shadowTextRegion, new Vector2(2, 2), 500);
            _colorEffect = new ColorEffect(_shadowTextRegion, new Color(0, 0, 0, 0), 500);
        }


        public override void Update()
        {
            base.Update();
            if (_rescaleEffect != null)
            {
                _rescaleEffect.Update();
                _colorEffect.Update();
            }
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            Controller.AddObject(TextRegion);
            if (!GameGlobals.EditorMode)
                Mask.IsHidden = true;
            //else
            //{
            //    IsHidden = true;
            //}
            TextRegion.LayerDepth = LayerDepth - 0.1f;
        }

        protected override void SetRectangle(float x, float y, float width, float height)
        {
            base.SetRectangle(x, y, width, height);
            TextRegion.Rect = Rectangle.GetRectangle();
        }

#if EDITOR

        public override void EditorDeleteObject()
        {
            base.EditorDeleteObject();
            Controller.RemoveObject(TextRegion);
            TextRegion.Dispose();
            TextRegion = null;
        }

        public override void EditorPasteObject()
        {
            base.EditorPasteObject();
            TextRegion = (TextRegion)TextRegion.Clone();
            Controller.AddObject(TextRegion);
        }

        public override bool EditorIsDebugFlagEnabled(Engine.Mechanics.DebugFlag flag)
        {
            return false;
        }

#endif

        public override void Dispose()
        {
            base.Dispose();
            if (TextRegion != null)
            {
                TextRegion.Dispose();
                TextRegion = null;
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (_shadowTextRegion != null)
                _shadowTextRegion.Draw();
        }
    }
}
