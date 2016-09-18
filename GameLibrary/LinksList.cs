using System;
using Engine.Core;
using Engine.Graphics;
#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
#endif
using Microsoft.Xna.Framework;
using TheGoo;
using Image = Engine.Graphics.Image;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameLibrary
{
    public class LinksList
    {
        protected const int VerticalDistance = 10;
        protected const int HorizontalDistanceImg = -4;
        protected const int HorizontalDistanceText= 0;
        protected static Vector2 ImgSize = new Vector2(60, 60);
        protected static Vector2 _largeTextBoxSize = new Vector2(600, 60);
        protected static Vector2 _smallTextBoxSize = new Vector2(146, 60);
        protected Rectangle[] _buttons;
        protected GameTexture[] _textures;
        protected GameTexture _borderSmall;
        protected GameTexture _borderLarge;
        

        public LinksList(Point pos)
        {
            _textures = new[]
            {
                new GameTexture(@"Gui\Menu\twitter"),
                new GameTexture(@"Gui\Menu\facebook"), 
                new GameTexture(@"Gui\Menu\youtube"), 
                new GameTexture(@"Gui\Menu\rate"),
                new GameTexture(@"Gui\Menu\email"),
                new GameTexture(@"Gui\Menu\AdFree")
            };
            _borderSmall = new GameTexture(@"Gui\Menu\link_borders_small");
            _borderLarge = new GameTexture(@"Gui\Menu\link_borders_large");
            _buttons = new Rectangle[_textures.Length];
            var posY = pos.Y;
            //posY += 60 + VerticalDistance;
            AddButton(3, 3, new Rectangle(pos.X, posY, (int)ImgSize.X, (int)ImgSize.Y), 3, false);

            posY += 60 + VerticalDistance;
            AddButton(4, 4, new Rectangle(pos.X, posY, (int)ImgSize.X, (int)ImgSize.Y), 4, false);

            posY += 60 + VerticalDistance;
            AddButton(5, 5, new Rectangle(pos.X, posY, (int)ImgSize.X, (int)ImgSize.Y), 5, false);

            posY += 60 + VerticalDistance;
            AddButton(0, 0, new Rectangle(pos.X, posY, (int)ImgSize.X, (int)ImgSize.Y), 0, true);
            AddButton(1, 1, new Rectangle(pos.X + (int)ImgSize.X + (int)_smallTextBoxSize.X + 20, posY, (int)ImgSize.X, (int)ImgSize.Y), 1, true);
            AddButton(2, 2, new Rectangle(pos.X + (int)ImgSize.X*2 + (int)_smallTextBoxSize.X*2 + 40, posY, (int)ImgSize.X, (int)ImgSize.Y), 2, true);

        }

        protected void AddButton(int btnId, int textureId, Rectangle rectangle, int descriptionId, bool small)
        {
            var descriptions = GetDescriptions();

            var btnImg = new Image(_textures[textureId])
            {
                LayerDepth = 0.11f,
                Owner = this,
                Rect = rectangle
            };
            _buttons[btnId] = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + (int)(small ? _smallTextBoxSize.X : _largeTextBoxSize.X), rectangle.Height);
            var bordersImg = new Image(small ? _borderSmall : _borderLarge)
            {
                LayerDepth = 0.11f,
                Owner = this,
                Rect =
                    new Rectangle(rectangle.X + rectangle.Width + HorizontalDistanceImg, rectangle.Y,
                        (int) (small ? _smallTextBoxSize.X : _largeTextBoxSize.X),
                        (int) (small ? _smallTextBoxSize.Y : _largeTextBoxSize.Y))
            };
            var tb = new TextRegion(new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, (int)(small ? _smallTextBoxSize.X : _largeTextBoxSize.X), (int)(small ? _smallTextBoxSize.Y : _largeTextBoxSize.Y)),
                GameGlobals.Font, descriptions[descriptionId], true)
            {
                Owner = this,
                LayerDepth = 0.1f,
                Color = Color.Black,
                HorizontaAlign = FontHorizontalAlign.Center,
                VerticalAlign = FontVerticalAlign.Center,
                StaticPosition = false
            };
            Controller.AddObject(tb);
            Controller.AddObject(btnImg);
            Controller.AddObject(bordersImg);
        }

        public static string[] GetDescriptions()
        {
            return new[]
            {
                "Twitter", "Facebook", "YouTube", "Enjoyed the game? Rate us!",
                "Send us your feedback.", "Get Ad free version."
            };

        }

        public bool CheckClick(Point pos)
        {
#if WINDOWS_PHONE

            for (var i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i].Contains(pos))
                {
                    switch (i)
                    {
                        case 0:
                            var twitterTask = new WebBrowserTask
                            {
                                Uri =
                                    new Uri(
                                        "https://twitter.com/FarstallGames")
                            };
                            twitterTask.Show();
                            return true;
                        case 1:
                            var facebookTask = new WebBrowserTask
                            {
                                Uri =
                                    new Uri(
                                        "https://www.facebook.com/pages/Farstall-games/1394426237472332")
                            };
                            facebookTask.Show();
                            return true;
                        case 2:
                            var youtubeTask = new WebBrowserTask
                            {
                                Uri =
                                    new Uri("http://www.youtube.com/user/FarstallGames")
                            };
                            youtubeTask.Show();
                            return true;
                        case 3:
                            var marketplace = new MarketplaceDetailTask()
                            {
                                ContentIdentifier = "23b0924f-763f-4626-a657-681200971d32",//GameGlobals.AdFreeAppId,
                                ContentType = MarketplaceContentType.Applications
                            };
                            marketplace.Show();
                            return true;
                       /* case 2:
                            var bloggerTask = new WebBrowserTask
                            {
                                Uri = new Uri("http://farstallgames.blogspot.com/")
                            };
                            bloggerTask.Show();
                            return true;*/
                        case 4:
                            var emailComposeTask = new EmailComposeTask
                            {
                                Subject = "TheMachine feedback",
                                Body = "",
                                To = "farstallgames@gmail.com"
                            };
                            emailComposeTask.Show();
                            return true;
                        case 5:
                            var adFreeTask = new MarketplaceDetailTask()
                            {
                                ContentIdentifier = GameGlobals.AdFreeAppId,
                                ContentType = MarketplaceContentType.Applications
                            };
                            adFreeTask.Show();
                            return true;
                    }
                }
            }
            return false;
#else
            return false;
#endif
        }

    }
}
