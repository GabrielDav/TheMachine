//using System.Collections.Generic;
//using Engine.Core;
//using Engine.Graphics;
//using GameLibrary.Objects;
//using Microsoft.Xna.Framework;
//using TheGoo;

//namespace GameLibrary
//{
    /*
    public class MapMatrix 
    {
        protected Image _shadowImg;
        protected ResizeEffect _resizeEffect;
        protected ColorEffect _colorEffect;

        protected int _heightCount;
        protected int _widthCount;
        protected List<int> _maps;
        protected int _buttonWidth;
        protected int _buttonHeight;
        protected int _spaceHorizontal;
        protected int _spaceVertical;
        protected List<MenuObject> _menuObjects;
        protected const int PagesCount = 2;
     //   protected Dictionary<int, List<LevelButton>> _menuPages;
        public Rectangle Rect { get; set; }
        protected int _currentPage;

        public MapMatrix(
            int widthCount,
            int heightCount,
            List<int> maps,
            int buttonWidth,
            int buttonHeight,
            Rectangle rectangle)
        {
            _widthCount = widthCount;
            _heightCount = heightCount;
            _maps = maps;
            _buttonWidth = buttonWidth;
            _buttonHeight = buttonHeight;
            Rect = rectangle;
            _spaceHorizontal = (rectangle.Width - (buttonWidth*widthCount))/(widthCount - 1);
            _spaceVertical = (rectangle.Height - (buttonHeight*heightCount))/(heightCount - 1);
            _menuObjects = new List<MenuObject>();
            //  _menuPages = new Dictionary<int, List<LevelButton>>();
            var lastUnlockedLevel = GetLastUnlockedLevel();

            var filledStar = new GameTexture(@"Gui\Menu\Star_Filled_Small");
            var emptyStar = new GameTexture(@"Gui\Menu\Star_Empty_Small");

            // for (var page = 0; page < PagesCount; page++)
            //  {
            //    _menuPages[page] = new List<LevelButton>();
            for (var i = 0; i < widthCount * heightCount; i++)
            {
                // var mapId = i + (page*widthCount*heightCount);
                var mapId = _maps[i];
                if (_maps.Count <= i)
                {
                    break;
                }

                var button = new LevelButton(mapId);
                var rowY = i/widthCount;
                var rowX = i%widthCount;
                button.Name = mapId.ToString();

                var x = Rect.X + (rowX*_buttonWidth + rowX*_spaceHorizontal);
                var y = Rect.Y + (rowY*_buttonWidth + rowY*_spaceVertical);
                button.Rectangle = new RectangleF(x, y + 20, _buttonWidth, _buttonHeight);
                var levelComplete = 0;
                if (mapId <= lastUnlockedLevel)
                {

                    button.Text = (mapId + 1).ToString();
                    button.IsActivated = true;
                    if (GameGlobals.SaveData.Rating[mapId] > -1)
                    {
                        levelComplete = 1;
                        button.Splash = new Image(EngineGlobals.Resources.Textures["Level_completed_splash"][0])
                        {
                            LayerDepth = 0.4f,
                            Rect =
                                new Rectangle((int) (button.Rectangle.X + button.Rectangle.Width - 45),
                                    (int) (button.Rectangle.Y + button.Rectangle.Height - 25), 80, 40),
                            Owner = this
                        };
                        button.Load("Level_Button_completed", 0);
                        Controller.AddObject(button.Splash);

#if WINDOWS_PHONE

                        var stars = new Image[3];
                        stars[0] = new Image(GameGlobals.SaveData.Rating[mapId] > 0? filledStar : emptyStar);
                        stars[1] = new Image(GameGlobals.SaveData.Rating[mapId] > 1 ? filledStar : emptyStar);
                        stars[2] = new Image(GameGlobals.SaveData.Rating[mapId] > 2 ? filledStar : emptyStar);
                        stars[0].Rect = new Rectangle(button.Pos.X + 4, button.Pos.Y + (int)button.Rectangle.Height - 20, 16, 16);
                        stars[1].Rect = new Rectangle(button.Pos.X + 24, button.Pos.Y + (int)button.Rectangle.Height - 20, 16, 16);
                        stars[2].Rect = new Rectangle(button.Pos.X + 44, button.Pos.Y + (int)button.Rectangle.Height - 20, 16, 16);
                        for (int j = 0; j < 3; j++)
                        {
                            Controller.AddObject(stars[j]);
                        }
#endif
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
                    button.IsActivated = false;
                }
                _menuObjects.Add(button);
                // _menuPages[page].Add(button);
                button.Mask.IsHidden = false;
                button.Mask.LayerDepth = 0.5f;
                //button.TextRegion.Pos = new Vector2(button.TextRegion.Pos.X + 10, button.TextRegion.Pos.Y + 7);
                button.TextRegion.Rect = button.Rectangle.GetRectangle();
                button.TextRegion.HorizontaAlign = FontHorizontalAlign.Center;
                button.TextRegion.VerticalAlign = FontVerticalAlign.Center;
                button.MenuGear = new MenuGear();
                button.MenuGear.Load("LevelBtnGear", levelComplete);
                button.MenuGear.Rectangle = new RectangleF(button.Rectangle.X -15, button.Rectangle.Y - 18, 60, 60);
                button.MenuGear.RotationSpeed = 0.5f;
                button.MenuGear.LayerDepth = 0.55f;
                //  if (page != 0) 
                //      button.IsHidden = true;
                Controller.AddGameObject(button.MenuGear);
                Controller.AddGameObject(button);

            }*/


            // }

            /*var pageSelector = new MenuObject
            {
                Name = "LevelSelector",
                Rectangle =
                    new RectangleF(Rect.X + (_buttonWidth*5 + _spaceHorizontal*5),
                        Rect.Y + (_buttonHeight + _spaceVertical), _buttonWidth, _buttonHeight)
                ,
                Text = ">",
                LayerDepth = 0.3f,
                IsActivated = true
            };
            pageSelector.Load("Level_Button_completed", 0);
            pageSelector.Mask.IsHidden = false;
            pageSelector.Mask.LayerDepth = 0.5f;


            Controller.AddGameObject(pageSelector);*/

       // }

        /*
        public static int GetLastUnlockedLevel()
        {
            var lastUnlockedLevel = 4;
            var r = GameGlobals.SaveData.Rating;
            for (var i = 0; i < GameGlobals.SaveData.Rating.Count; i += 5)
            {
                
                if (r[i] > -1 && r[i + 1] > -1 && r[i + 2] > -1 && r[i + 3] > -1 && r[i + 4] > -1)
                    lastUnlockedLevel += 5;
                else
                    break;
            }

            return lastUnlockedLevel;
        }
         */

       

        /*public void SwitchPageNext()
        {
            var prevPage = _currentPage;
            if (_currentPage + 1 >= PagesCount)
                _currentPage = 0;
            else
                _currentPage++;
            foreach (var button in _menuPages[prevPage])
            {
                button.IsHidden = true;
            }
            foreach (var button in _menuPages[_currentPage])
            {
                button.IsHidden = false;
            }
        }*/

        //public void Update()
        //{
        //    foreach (var menuObject in _menuObjects)
        //    {
        //        menuObject.Update();
        //    }
        //}
        
//    }

//}
