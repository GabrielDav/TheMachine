using Engine.Graphics;
using GameLibrary.Objects;
using TheGoo;
using Engine.Core;
using System.Collections.Generic;

namespace GameLibrary.GridBuilder
{
    public class AchievementsTileBuilder : ITileBuilder
    {
        private GameTexture _backgroundTexture;
        private IList<AchievementButton> _buttons;

        public void Init()
        {
            _buttons = new List<AchievementButton>();
            _backgroundTexture = new GameTexture("FadeTexture");
        }

        public void BuildTile(int number, int x, int y, int width, int height, TileType? type)
        {
            var iconRect = new RectangleF(x, y, width, height);
            var achievement = new AchievementButton(number) { Rectangle = iconRect };
            achievement.Load("Achievement", number);
            achievement.LayerDepth = 0.5f;
            achievement.IsHidden = false;

            if (!GameGlobals.SaveData.UnlockedAchievements.Contains(number))
            {
                achievement.Mask.Transparency = 33;
            }

            Controller.AddGameObject(achievement);
            var backgroundImage = new Image(_backgroundTexture)
            {
                Rect = achievement.Rectangle.GetRectangle(),
                LayerDepth = 0.6f
            };

            _buttons.Add(achievement);
            Controller.AddObject(backgroundImage);
        }

    }
}
