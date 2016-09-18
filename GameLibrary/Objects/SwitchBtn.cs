#if EDITOR
using System.Drawing;
#endif
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using Image = Engine.Graphics.Image;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameLibrary.Objects
{
    public class SwitchBtn : Plane, IMenuBtn
    {
        protected Image _shadowImg;
        protected ResizeEffect _resizeEffect;
        protected ColorEffect _colorEffect;

        public SwitchBtn()
        {
            TypeId = GameObjectType.SwitchBtn.ToString();
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            Mask.IsHidden = false;
        }

#if EDITOR

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(3, 3);
        }

#endif


        public void ShowPressAnimation()
        {
            if (_shadowImg == null)
            {
                _shadowImg = new Image(new GameTexture(((Image) Mask).Texture));
                _shadowImg.Orgin = _shadowImg.OriginCenter();
                _shadowImg.Flip = Mask.Flip;
            }
            _shadowImg.Rect = new Rectangle((int)(Mask.Rect.X + Mask.Rect.Width/2f), (int)(Mask.Rect.Y + Mask.Rect.Height/2f), Mask.Rect.Width, Mask.Rect.Height);
            _shadowImg.Color = new Color(0, 0, 0, 255);
            if (_resizeEffect == null)
                _resizeEffect = new ResizeEffect(_shadowImg, new Vector2(60, 60), 500);
            else
                _resizeEffect.Reset(new Vector2(60, 60), 500);
            if (_colorEffect == null)
                _colorEffect = new ColorEffect(_shadowImg, new Color(0, 0, 0, 0), 500);
            else
                _colorEffect.Reset(new Color(0, 0, 0, 0), 500);
        }

        public override void Update()
        {
            base.Update();
            if (_resizeEffect != null)
            {
                _resizeEffect.Update();
                _colorEffect.Update();
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (_shadowImg != null)
                _shadowImg.Draw();
        }
    }
}
