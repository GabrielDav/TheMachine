using Engine.Core;
using Microsoft.Xna.Framework;

namespace Engine.Gui
{
    internal class Label : Dialog
    {
#if EDITOR
        public Label(Rectangle rect, Theme theme, string text = "", bool multiline = false) : base(rect, theme, text)
        {
            Multiline = multiline;
            //_frame = theme.LabelBackgound;
        }
#else
        public Label(Rectangle rect, Theme theme, string text, bool multiline)
            : base(rect, theme, text, multiline)
        {
            Multiline = multiline;
            //_frame = theme.LabelBackgound;
        }
#endif

        public new virtual void Draw()
        {
            if (IsHidden)
                return;
            //if (!EngineGlobals.Camera.IsObjectVisible(CalculateCornerRectangle()))
            //    return;
            // EngineGlobals.Batch.Draw(_backgound, EngineGlobals.Camera.Transform(Rect), _frame, Color, Rotation, _origin, SpriteEffects.None, LayerDepth);
            base.Draw();
        }
    }
}
