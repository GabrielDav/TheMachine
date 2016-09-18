#if EDITOR
using System.Drawing;
#endif
using Engine.Core;
using Engine.Graphics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using Image = Engine.Graphics.Image;
using Rectangle = Microsoft.Xna.Framework.Rectangle;


namespace GameLibrary.Objects
{
    public class MenuBtnPointer : MenuLine, IMenuBtn
    {
        protected ColorEffect _fadeOut;
        protected ColorEffect _fadeIn;

        protected Engine.Graphics.Image _shadowImg;
        protected ResizeEffect _resizeEffect;
        protected ColorEffect _colorEffect;

        public MenuBtnPointer() : base()
        {
            TypeId = GameObjectType.MenuBtnPointer.ToString();
        }

        protected override void Init()
        {
            Animated = true;
            base.Init();
        }


#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(8, 5);
        }
#endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
           /* Mask.Color = new Color(255, 255, 255, 0);
            _fadeIn = new ColorEffect(Mask, new Color(0, 0, 0, 255), 500);
            Mask.Color = new Color(255, 255, 255, 255);
            _fadeOut = new ColorEffect(Mask, new Color(0, 0, 0, 0), 500);*/

            _sprite.PlayAnimation("Main", true);
        }

        public override void Update()
        {
            _sprite.Update();
            base.Update();
            if (_resizeEffect != null)
            {
                _resizeEffect.Update();
                _colorEffect.Update();
            }
           /* if (_fadeOut.Finished)
            {
                _fadeIn.Update();
                if (_fadeIn.Finished)
                {
                    _fadeOut.Reset(new Color(0, 0, 0, 0), 500);
                }
            }
            else
            {
                _fadeOut.Update();
                if (_fadeOut.Finished)
                    _fadeIn.Reset(new Color(0, 0, 0, 255), 500);
            }*/
        }

        public override void Draw()
        {
            base.Draw();
            if (_shadowImg != null)
                _shadowImg.Draw();
        }

        public void ShowPressAnimation()
        {
            if (_shadowImg == null)
            {
                _shadowImg = new Image(EngineGlobals.Resources.Textures["BtnPointer"][0]);
                _shadowImg.Orgin = _shadowImg.OriginCenter();
                _shadowImg.Rotation = Mask.Rotation;
            }
           
            var rect = Rectangle.GetRectangle();
            var width = rect.Width;
            var height = rect.Height;
            _shadowImg.Rect = new Rectangle((int)(rect.X + width / 2f),
                                            (int)(rect.Y + height / 2f), height,
                                            width);
            _shadowImg.Color = new Color(255, 255, 255, 255);
            if (_resizeEffect == null)
                _resizeEffect = new ResizeEffect(_shadowImg, new Vector2((int)(width * 2), (int)(height * 2)), 500);
            else
                _resizeEffect.Reset(new Vector2((int)(width * 2), (int)(height * 2)), 500);
            if (_colorEffect == null)
                _colorEffect = new ColorEffect(_shadowImg, new Color(255, 255, 255, 0), 500);
            else
                _colorEffect.Reset(new Color(255, 255, 255, 0), 500);
        }
    }
}
