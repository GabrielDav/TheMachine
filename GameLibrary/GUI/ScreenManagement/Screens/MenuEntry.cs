using System;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    public class MenuEntry
    {
        public event EventHandler Selected;

        public string Text;

        public Texture2D Image;

        public Rectangle Rectangle { get; set; }

        public Color Color = new Color(255, 255, 255);

        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
            {
                Selected(this, null);
            }
        }

        public MenuEntry(string text, Texture2D image, Rectangle rectangle, Color? color)
        {
            Text = text;
            Image = image;
            Rectangle = rectangle;
            Color = color == null ? Color.White : color.Value;
        }

        public virtual void Draw(MenuScreen screen, GameTime gameTime, Color color)
        {
            if (Image != null)
            {
                EngineGlobals.Batch.Draw(Image, Rectangle, color);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                EngineGlobals.Batch.DrawString(GameGlobals.MenuGlobals.MenuFont, Text, new Vector2(Rectangle.X, Rectangle.Y), color);
            }
        }
    }
}
