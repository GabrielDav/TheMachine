using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Engine.Gui
{
    public class Edit : Dialog
    {
        protected bool _editing;
        public string Caption;
        public string Description;
#if EDITOR
        public Edit(Rectangle rect, Theme theme, string text = "", bool multiline = false)
            : base(rect, theme, text, multiline)
        {
            _frame = theme.EditBackground;
        }
#else
        public Edit(Rectangle rect, Theme theme, string text, bool multiline)
            : base(rect, theme, text, multiline)
        {
            _frame = theme.EditBackground;
        }
#endif

        protected override void CheckClick(Point point, int id)
        {
            if (!IsEnabled) return;
            if (!Rect.Contains(point.X, point.Y)) return;
            if (_editing) return;
            _editing = true;
        }

        public override void Draw()
        {
            if (_editing)
            {
                if (!Guide.IsVisible)
                    Guide.BeginShowKeyboardInput(PlayerIndex.One, Caption, Description, _text,
                                                 delegate(IAsyncResult result)
                                                     {
                                                         var text = Guide.EndShowKeyboardInput(result);
                                                         if (text != null)
                                                         {
                                                             _text = text;
                                                             UpdateText();
                                                         }
                                                         _editing = false;
                                                     }, null);

            }
            base.Draw();

        }
    }
}
