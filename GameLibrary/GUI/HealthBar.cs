using Engine.Core;
using Engine.Graphics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Gui
{
    public class HealthBar
    {
        public Image Border;

        public Image Bar;

        public const int Width = 30;
        public const int Height = 300;
        public const int X = 10;
        public const int Y = 10;

        public const int BarWidth = 28;
        public const int BarHeight = 209;
        public const int BarX = 11;
        public const int BarY = 30;

        public HealthBar(string barTexture, string borderTexture)
        {
            Border = new Image(EngineGlobals.Resources.Textures[borderTexture][0], new Rectangle(X, Y, Width, Height))
                         {
                             StaticPosition = true,
                             LayerDepth = 0.12f,
                             Owner = this
                         };
            Bar = new Image(EngineGlobals.Resources.Textures[barTexture][0], new Rectangle(BarX, BarY, BarWidth, BarHeight))
                      {
                          StaticPosition = true,
                          LayerDepth = 0.11f,
                          Owner = this
                      };
        }

        public void UpdateBar()
        {
            var percentage = GameGlobals.Player.CurrentMovePoints / Player.MaxMovePoints;
            var shift = BarY + (int)((BarHeight)*(1 - (percentage)));
            var shrink = (int)((BarHeight)*(percentage));
            var newHeight = Bar.Texture.Height*percentage;
            Bar.Frame = new Rectangle(0, Bar.Texture.Height - (int)newHeight, Bar.Texture.Width, (int)newHeight);
            Bar.Rect = new Rectangle(
                   Bar.Rect.X,
                   shift,
                   Bar.Width,
                   shrink);   
        }

        public event SimpleEvent OnPositionTypeChanged;

        //public void Draw()
        //{
        //    Bar.Draw();
        //    //Border.Draw();
        //}

    }
}
