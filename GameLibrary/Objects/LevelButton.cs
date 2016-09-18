using Engine.Core;
using Engine.Graphics;
using GameLibrary.GridBuilder;
using Microsoft.Xna.Framework;

namespace GameLibrary.Objects
{
    public class LevelButton : MenuObject
    {
        public TileType Type
        {
            get
            {
                return _type;
            }
        }

        protected Image _shadowImg;
        protected ResizeEffect _resizeEffect;
        protected ColorEffect _imgColorEffect;
        protected TileType _type;

        public override bool IsHidden
        {
            get { return base.IsHidden; }
            set
            {
                base.IsHidden = value;
                MenuGear.Mask.IsHidden = value;
                if (Splash != null)
                    Splash.IsHidden = value;
            }
        }

        public int LevelId { get; set; }

        public MenuGear MenuGear { get; set; }

        public Image Splash { get; set; }

        public LevelButton(int levelId, TileType type)
        {
            LevelId = levelId;
            _type = type;
        }

        public override void ShowPressAnimation()
        {
            base.ShowPressAnimation();
            EngineGlobals.SoundManager.Play("click", "click", 0.7f, false, 0, 0);
            var image = (Image) Mask;
            if (_shadowImg == null)
            {
                _shadowImg = new Image(new GameTexture(image.Texture));
                _shadowImg.Orgin = _shadowImg.OriginCenter();
                _shadowImg.Flip = image.Flip;
            }
            _shadowImg.Rect = new Rectangle((int)(image.Rect.X + image.Rect.Width / 2f),
                                            (int)(image.Rect.Y + image.Rect.Height / 2f), image.Rect.Width,
                                            image.Rect.Height);
            _shadowImg.Color = new Color(255, 255, 255, 200);
            if (_resizeEffect == null)
                _resizeEffect = new ResizeEffect(_shadowImg, new Vector2((int)(image.Rect.Width * 2), (int)(image.Rect.Height * 2)), 500);
            else
                _resizeEffect.Reset(new Vector2((int)(image.Rect.Width * 2), (int)(image.Rect.Height * 2)), 500);
            if (_imgColorEffect == null)
                _imgColorEffect = new ColorEffect(_shadowImg, new Color(255, 255, 255, 0), 500);
            else
                _imgColorEffect.Reset(new Color(255, 255, 255, 0), 500);
        }

        public override void Draw()
        {
            base.Draw();
            if (_shadowImg != null)
                _shadowImg.Draw();
        }

        public override void Update()
        {
            base.Update();
            if (_imgColorEffect != null)
            {
                _resizeEffect.Update();
                _imgColorEffect.Update();
            }
        }

    }
}
