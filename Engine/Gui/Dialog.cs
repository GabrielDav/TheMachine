using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Gui
{
    public class Dialog : TextRegion
    {
        public event SimpleEvent OnClick;
        public event SimpleEvent OnPressed;
        protected bool _isPressed;
        protected int _inputId;
        protected Theme _theme;


        public bool IsEnabled { get; set; }

        public bool IsPressed
        {
            get
            {
                return _isPressed;
            }
        }
#if EDITOR
        public Dialog(Rectangle rect, Theme theme, string text = "", bool multiline = false) : base(rect, theme.Font, Color.White, text, multiline)
        {
            IsEnabled = true;
            EngineGlobals.Input.OnPress += CheckClick;
            _theme = theme;
            _frame = theme.WindowBackground;
        }
#else
        public Dialog(Rectangle rect, Theme theme, string text, bool multiline)
            : base(rect, theme.Font, Color.White, text, multiline)
        {
            IsEnabled = true;
            EngineGlobals.Input.OnPress += CheckClick;
            _theme = theme;
            _frame = theme.WindowBackground;
        }
#endif

        protected virtual void CheckRelease(Point point, int id)
        {
            if (!_isPressed)
                return;
            if (id != _inputId)
                return;
            _isPressed = false;
            if (OnClick != null)
                OnClick(this);
            EngineGlobals.Input.OnRelease -= CheckRelease;
            EngineGlobals.Input.OnPress += CheckClick;
            
        }

        protected virtual void CheckClick(Point point, int id)
        {
            if (!IsEnabled) return;
            if (_isPressed) return;
            _inputId = id;
            if (!Rect.Contains(point.X, point.Y)) return;
            EngineGlobals.Input.OnPress -= CheckClick;
            EngineGlobals.Input.OnRelease += CheckRelease;
            _isPressed = true;
            if (OnPressed != null)
                OnPressed(this);
        }

        public override void Dispose()
        {
            OnClick = null;
            OnPressed = null;
            base.Dispose();
        }

        public new virtual void Draw()
        {
            EngineGlobals.Batch.Draw(_theme.Texutre, Rect, _frame, Color, Rotation, _origin, SpriteEffects.None, _layerDepth+0.0001f);
            base.Draw();
        }

    }

    
}
