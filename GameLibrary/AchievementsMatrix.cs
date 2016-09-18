using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary
{
    /*
    public class AchievementsMatrix
    {
        //public List<AchievementButton> Matrix { get; protected set; }
        //protected GameTexture _backgroundTexture;

        public AchievementsMatrix(int widthCount, int heightCount, int buttonWidth, int buttonHeight,
            Rectangle rectangle)
        {
            var spaceHorizontal = (rectangle.Width - (buttonWidth * widthCount)) / (widthCount - 1);
            var spaceVertical = (rectangle.Height - (buttonHeight * heightCount)) / (heightCount - 1);
            _backgroundTexture = new GameTexture("FadeTexture");

            Matrix = new List<AchievementButton>();

            for (var i = 0; i < widthCount * heightCount; i++)
            {
                if (i >= GameGlobals.AchievementsCount)
                    break;
                var rowY = i / widthCount;
                var rowX = i % widthCount;

                var x = rectangle.X + (rowX * buttonWidth + rowX * spaceHorizontal);
                var y = rectangle.Y + (rowY * buttonWidth + rowY * spaceVertical);
                var iconType = (Achievement)i;
                var iconRect = new RectangleF(x, y, buttonWidth, buttonHeight);
                var achievement = new AchievementButton(i) {Rectangle = iconRect};
                achievement.Load("Achievement", i);
                Matrix.Add(achievement);
                achievement.LayerDepth = 0.5f;
                achievement.Mask.IsHidden = false;
                if (!GameGlobals.SaveData.UnlockedAchievements.Contains(i))
                    achievement.Mask.Transparency = 33;
                Controller.AddGameObject(achievement);
                var backgroundImage = new Image(_backgroundTexture);
                backgroundImage.Rect = achievement.Rectangle.GetRectangle();
                backgroundImage.LayerDepth = 0.6f;
                Controller.AddObject(backgroundImage);

            }
        }
    }
     */
}
