using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Mechanics;

namespace Engine.Gui
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
        public const int BarHeight = 207;
        public const int BarX = 11;
        public const int BarY = 32;

        public HealthBar(string borderPath, string barPath)
        {
            Border = new Image(new GameTexture(borderPath), new Rectangle(X, Y, Width, Height))
                         {
                             StaticPosition = true
                         };
            Bar = new Image(new GameTexture(barPath), new Rectangle(BarX, BarY, BarWidth, BarHeight))
                      {
                          StaticPosition = true
                      };
        }

        public void UpdateBar(float percentage)
        {
            var shift = BarY + (int)((BarHeight)*(1 - (percentage)));
            var shrink = (int)((BarHeight)*(percentage));
            Bar.Rect = new Rectangle(
                   Bar.Rect.X,
                   shift,
                   Bar.Width,
                   shrink
                  /* (int)(Bar.Height*percentage/2)*/);   
        }

        public event SimpleEvent OnPositionTypeChanged;

        //public void Draw()
        //{
        //    Bar.Draw();
        //    //Border.Draw();
        //}

    }
}
