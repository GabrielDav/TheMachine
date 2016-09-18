using System;
using Engine.Core;
using Engine.Graphics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TheGoo;

namespace GameLibrary.GridBuilder
{
    public class MapTileBuilder : ITileBuilder
    {
        private GameTexture _filledStar;
        private GameTexture _emptyStar;
        private IList<LevelButton> _buttons;
        private int _lastUnlockedLevel;
        private static List<int> _raiting; 

        public void Init()
        {
            _filledStar = new GameTexture(@"Gui\Menu\Star_Filled_Small");
            _emptyStar = new GameTexture(@"Gui\Menu\Star_Empty_Small");
            _buttons = new List<LevelButton>();

            
        }

        public void BuildTile(int number, int x, int y, int width, int height, TileType? type)
        {
            if (!type.HasValue)
            {
                throw new Exception("Type is required!");
            }


            var splash = string.Empty;
            if (type.Value == TileType.Normal)
            {
                splash = "Level_completed_splash";
                _raiting = GameGlobals.SaveData.RatingNormal;
            }
            else if (type.Value == TileType.Hardcore)
            {
                splash = "Level_completed_splash_red";
                _raiting = GameGlobals.SaveData.Rating;
            }

            _lastUnlockedLevel = GetLastUnlockedLevel(type.Value);

            var mapId = number;
            var button = new LevelButton(mapId, type.Value);
            var levelComplete = 0;
            button.Rectangle = new RectangleF(x, y + 20, width, height);
            if (mapId <= _lastUnlockedLevel)
            {
                button.Text = (mapId + 1).ToString();
                if (_raiting[mapId] > -1)
                {
                    levelComplete = 1;
                    button.Splash = new Image(EngineGlobals.Resources.Textures[splash][0])
                    {
                        LayerDepth = 0.4f,
                        Rect =
                            new Rectangle((int) (button.Rectangle.X + button.Rectangle.Width - 45),
                                (int) (button.Rectangle.Y + button.Rectangle.Height - 25), 80, 40),
                        Owner = this
                    };
                    button.Load("Level_Button_completed", 0);
                    Controller.AddObject(button.Splash);

                    AddStars(
                        (int) button.Rectangle.X,
                        (int) button.Rectangle.Y,
                        (int) button.Rectangle.Height,
                        mapId);
                }
                else
                {
                    button.Load("Level_Button_completed", 0);
                }
            }
            else
            {
                button.Load("Level_Button_locked", 0);
                button.Text = "";
            }

            button.Mask.LayerDepth = 0.5f;
            button.TextRegion.Rect = button.Rectangle.GetRectangle();
            button.TextRegion.HorizontaAlign = FontHorizontalAlign.Center;
            button.TextRegion.VerticalAlign = FontVerticalAlign.Center;
            button.MenuGear = new MenuGear();
            button.MenuGear.Load("LevelBtnGear", levelComplete);
            button.MenuGear.Rectangle = new RectangleF(button.Rectangle.X - 15, button.Rectangle.Y - 18, 60, 60);
            button.MenuGear.RotationSpeed = 0.5f;
            button.MenuGear.LayerDepth = 0.55f;
            button.IsHidden = false;

            _buttons.Add(button);
            Controller.AddGameObject(button.MenuGear);
            Controller.AddGameObject(button);
        }

        private void AddStars(int x, int y, int height, int mapId)
        {
            var stars = new Image[3];
            stars[0] = new Image(_raiting[mapId] > 0 ? _filledStar : _emptyStar);
            stars[1] = new Image(_raiting[mapId] > 1 ? _filledStar : _emptyStar);
            stars[2] = new Image(_raiting[mapId] > 2 ? _filledStar : _emptyStar);
            stars[0].Rect = new Rectangle(x + 4, y + height - 20, 16, 16);
            stars[1].Rect = new Rectangle(x + 24, y + height - 20, 16, 16);
            stars[2].Rect = new Rectangle(x + 44, y + height - 20, 16, 16);

            for (int j = 0; j < 3; j++)
            {
                Controller.AddObject(stars[j]);
            }
        }

        public static int GetLastUnlockedLevel(TileType type)
        {
            var r = new List<int>();
            var lastUnlockedLevel = 4;
            if (type == TileType.Normal)
            {
                r = GameGlobals.SaveData.RatingNormal;
            }
            else if (type == TileType.Hardcore)
            {
                r = GameGlobals.SaveData.Rating;
            }
            
            for (var i = 0; i < r.Count; i += 5)
            {

                if (r[i] > -1 && r[i + 1] > -1 && r[i + 2] > -1 && r[i + 3] > -1 && r[i + 4] > -1)
                    lastUnlockedLevel += 5;
                else
                    break;
            }

            return lastUnlockedLevel;
        }
    }
}
