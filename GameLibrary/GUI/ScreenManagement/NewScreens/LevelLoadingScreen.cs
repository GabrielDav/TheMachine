using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using GameLibrary.GridBuilder;
using Microsoft.Xna.Framework;
using TheGoo;
using Image = Engine.Graphics.Image;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class LevelLoadingScreen : BaseLoadingScreen
    {
        protected enum IconType
        {
            Circle = 0,
            Spike = 1,
            SlideWall = 2,
            Saw = 3,
            DeathBall = 4,
            CircleSpikes = 5,
            PowerUpCollect = 6,
            JumpSpot = 7,
            SpikeShooter = 8,
            PowerUpDoubleJump = 9,
            TimeWarp = 10,
            Seeker = 11,
            PowerUpProtect = 12,
            Button = 13
        }

        protected static int IconCount = 14;

        protected TextRegion _levelCount;
        protected TextRegion _levelName;
        protected Image[] _levelIcons;
        protected int _nextLevel;
        protected Timer _waitTimer;
        protected readonly int _waitTime;
        protected Image _loadingImage;
        private TileType _type;

        protected static IconType[] AllIconTypes =
        {
            IconType.Circle, IconType.CircleSpikes, IconType.DeathBall, IconType.JumpSpot,
            IconType.PowerUpCollect, IconType.PowerUpDoubleJump, IconType.PowerUpProtect, IconType.Saw, IconType.Seeker,
            IconType.SlideWall, IconType.Spike, IconType.SpikeShooter, IconType.TimeWarp
        };

        public LevelLoadingScreen(int nextLevel, TileType type) : base(new GameScreen(nextLevel, type))
        {
            _nextLevel = nextLevel;
            _waitTimer = new Timer();
            _waitTime = 1000;
            _type = type;

            if (type == TileType.Normal)
            {
                GameGlobals.HardcoreMode = false;
            }
            else if (type == TileType.Hardcore)
            {
                GameGlobals.HardcoreMode = true;
            }
        }

        public override void Load()
        {
            base.Load();
            EngineGlobals.Camera2D = new Camera2D();
            var count = 0;
            if (_type == TileType.Normal)
            {
                count = 15;
            }
            else if (_type == TileType.Hardcore)
            {
                count = GameGlobals.MapList.GameMaps.Count;
            }
            
            _levelCount = new TextRegion(new Rectangle(300, 10, 200, 25), GameGlobals.MenuGlobals.MenuFont, Color.Black,
                "Level " + (_nextLevel + 1) + "/" + count, false){ HorizontaAlign = FontHorizontalAlign.Center, LayerDepth = 0.5f};
            Controller.AddObject(_levelCount);
            _levelName = new TextRegion(new Rectangle(300, 45, 200, 25), GameGlobals.MenuGlobals.MenuFont, Color.Black,
                GetLevelName(_nextLevel), false) { HorizontaAlign = FontHorizontalAlign.Center, LayerDepth = 0.5f };
            Controller.AddObject(_levelName);

            List<IconType> iconTypes = null;

            if (_type == TileType.Hardcore)
            {
                iconTypes = GetLevelIconTypes(_nextLevel);
            }
            else if (_type == TileType.Normal)
            {
                iconTypes = GetLevelIconTypesNormal(_nextLevel);
            }

            CreateIconMatrix(iconTypes, 70, 70, new Rectangle(200, 120, 400, 240));

            _loadingImage = new Image(new GameTexture("Loading")) { LayerDepth = 0.5f };
            _loadingImage.Rect = new Rectangle(574, 385, 206, 75);
            Controller.AddObject(_loadingImage);
        }

        protected virtual string GetLevelName(int level)
        {
            /*
            switch (level)
            {
                case 0:
                    return "Hodor";
                default:
                    return "Hodor!";
                    //throw new Exception("Undefined level number: " + level);
            }*/

            return string.Empty;
        }

        protected List<IconType> GetLevelIconTypesNormal(int level)
        {
            switch (level)
            {
                case 1:
                case 2:
                    return new List<IconType> { IconType.Circle };
                case 3:
                case 4:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike
                        };
                case 5:
                case 6:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.Saw
                        };
                case 7:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.Saw,
                            IconType.SlideWall
                        };
                case 8:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.JumpSpot,
                            IconType.Saw
                        };
                case 9:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Saw,
                            IconType.JumpSpot
                        };
                case 10:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.JumpSpot,
                            IconType.Saw
                        };
                case 11:
                case 12:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw,
                            IconType.Button
                        };
                case 13:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw
                        };
                case 14:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Saw,
                            IconType.Spike,
                            IconType.SlideWall,
                            IconType.Button
                        };

                default: 
                    return new List<IconType> ();
            }
        }

        protected List<IconType> GetLevelIconTypes(int level)
        {
            switch (level)
            {
                case 0:
                case 1:
                    return new List<IconType> { IconType.Spike };
                case 2:
                case 3:
                case 4:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike
                        };
                case 5:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.SlideWall,
                            IconType.CircleSpikes
                        };
                case 6:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.SlideWall,
                            IconType.CircleSpikes
                        };
                case 7:
                case 8:
                case 9:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.SlideWall,
                            IconType.Saw
                        };
                case 10:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.Saw,
                            IconType.DeathBall
                        };
                case 11:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.SlideWall,
                            IconType.Saw,
                            IconType.DeathBall
                        };
                case 12:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.SlideWall,
                            IconType.Saw,
                            IconType.DeathBall,
                            IconType.SpikeShooter,
                            IconType.CircleSpikes
                        };
                case 13:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.SlideWall,
                            IconType.Saw,
                            IconType.DeathBall,
                            IconType.SpikeShooter,
                        };
                case 14:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.SlideWall,
                            IconType.Saw,
                            IconType.PowerUpDoubleJump
                        };
                case 15:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.DeathBall,
                            IconType.SpikeShooter,
                            IconType.PowerUpDoubleJump
                        };
                case 16:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.DeathBall,
                            IconType.SpikeShooter,
                            IconType.PowerUpDoubleJump,
                            IconType.JumpSpot
                        };
                case 17:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Spike,
                            IconType.DeathBall,
                            IconType.SpikeShooter,
                            IconType.PowerUpDoubleJump,
                            IconType.JumpSpot
                        };
                case 18:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw,
                            IconType.DeathBall,
                            IconType.SpikeShooter,
                            IconType.PowerUpDoubleJump,
                            IconType.JumpSpot
                        };
                case 19:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw,
                            IconType.PowerUpDoubleJump,
                            IconType.DeathBall
                        };
                case 20:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw,
                            IconType.PowerUpDoubleJump,
                            IconType.DeathBall,
                            IconType.Button
                        };
                case 21:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw,
                            IconType.JumpSpot,
                            IconType.PowerUpDoubleJump,
                            IconType.Button
                        };
                case 22:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw,
                            IconType.JumpSpot,
                            IconType.PowerUpDoubleJump,
                        };
                case 23:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw,
                            IconType.JumpSpot,
                            IconType.PowerUpDoubleJump,
                            IconType.PowerUpCollect,
                            IconType.TimeWarp,
                            IconType.Seeker
                        };
                case 24:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.Saw,
                            IconType.JumpSpot,
                            IconType.PowerUpDoubleJump,
                            IconType.PowerUpCollect,
                            IconType.TimeWarp,
                            IconType.Seeker,
                            IconType.PowerUpProtect
                        };
                case 25:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Seeker,
                            IconType.JumpSpot,
                            IconType.PowerUpProtect,
                            IconType.PowerUpDoubleJump,
                            IconType.Spike,
                            IconType.DeathBall,
                            IconType.Saw
                        };
                case 26:
                    return new List<IconType>
                        {
                            IconType.Circle,
                            IconType.Seeker,
                            IconType.JumpSpot,
                            IconType.PowerUpProtect,
                            IconType.PowerUpDoubleJump,
                            IconType.Spike,
                            IconType.DeathBall,
                            IconType.Saw,
                        };
                case 27:
                    return new List<IconType>
                        {
                            IconType.SlideWall,
                            IconType.Saw,
                            IconType.JumpSpot,
                            IconType.DeathBall,
                            IconType.Spike,
                            IconType.PowerUpDoubleJump,
                            IconType.PowerUpProtect,
                            IconType.CircleSpikes,
                            IconType.Circle,
                            IconType.Seeker
                        };
                case 28:
                    return new List<IconType>
                        {
                            IconType.Spike,
                            IconType.Circle,
                            IconType.PowerUpDoubleJump,
                            IconType.PowerUpProtect,
                            IconType.PowerUpCollect,
                            IconType.Saw,
                            IconType.Seeker,
                            IconType.SpikeShooter,
                            IconType.Button
                        };
                case 29:
                    return new List<IconType>
                        {
                            IconType.Saw,
                            IconType.JumpSpot,
                            IconType.PowerUpDoubleJump,
                            IconType.PowerUpProtect,
                            IconType.PowerUpCollect,
                            IconType.Spike,
                            IconType.Seeker,
                            IconType.Circle,
                            IconType.Button
                        };
                default:
                    return new List<IconType>();
                //throw new Exception("Undefined level number: " + level);
            }
        }

        protected override void LoadNextScreenInstantly()
        {
            
        }

        protected void CreateIconMatrix(List<IconType> levelObjects,  int buttonWidth, int buttonHeight,
            Rectangle rectangle)
        {
            const int widthCount = 5;
            const int heightCount = 3;
            var spaceHorizontal = (rectangle.Width - (buttonWidth * widthCount)) / (widthCount - 1);
            var spaceVertical = (rectangle.Height - (buttonHeight * heightCount)) / (heightCount - 1);

            var matrix = new List<Image>();

            for (var i = 0; i < widthCount * heightCount; i++)
            {
                if (i >= IconCount)
                    break;
                var rowY = i/widthCount;
                var rowX = i%widthCount;

                var x = rectangle.X + (rowX * buttonWidth + rowX * spaceHorizontal);
                var y = rectangle.Y + (rowY * buttonWidth + rowY * spaceVertical);
                var iconType = (IconType) i;
                var iconRect = new RectangleF(x, y, buttonWidth, buttonHeight);
                var img = new Image(GetIconTexture(iconType)) { Owner = this, LayerDepth = 0.2f };
                img.Rect = iconRect.GetRectangle();
                if (!levelObjects.Contains(iconType))
                    img.Transparency = 35;
                matrix.Add(img);
                Controller.AddObject(img);
            }

            _levelIcons = matrix.ToArray();
        }

        protected virtual GameTexture GetIconTexture(IconType iconType)
        {
            switch (iconType)
            {
                case IconType.Circle:
                    return new GameTexture(@"GUI\Icons\Icon_Circle");
                case IconType.CircleSpikes:
                    return new GameTexture(@"GUI\Icons\Icon_HalfCircleSpike");
                case IconType.DeathBall:
                    return new GameTexture(@"GUI\Icons\Icon_DeathBall");
                case IconType.JumpSpot:
                    return new GameTexture(@"GUI\Icons\Icon_JumpSpot");
                case IconType.PowerUpCollect:
                    return new GameTexture(@"GUI\Icons\Icon_Collect");
                case IconType.PowerUpDoubleJump:
                    return new GameTexture(@"GUI\Icons\Icon_DoubleJump");
                case IconType.PowerUpProtect:
                    return new GameTexture(@"GUI\Icons\Icon_Protect");
                case IconType.Saw:
                    return new GameTexture(@"GUI\Icons\Icon_Saw");
                case IconType.Seeker:
                    return new GameTexture(@"GUI\Icons\Icon_Seeker");
                case IconType.SlideWall:
                    return new GameTexture(@"GUI\Icons\Icon_SlideWall");
                case IconType.Spike:
                    return new GameTexture(@"GUI\Icons\Icon_Spike");
                case IconType.SpikeShooter:
                    return new GameTexture(@"GUI\Icons\Icon_SpikeShooter");
                case IconType.TimeWarp:
                    return new GameTexture(@"GUI\Icons\Icon_TimeWarp");
                case IconType.Button:
                    return new GameTexture(@"GUI\Icons\Icon_Button");
                default:
                    throw new Exception("Undefined icon type: " + iconType);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            EngineGlobals.Camera2D.CornerPos = new Vector2(0, 0);
            FadeInTime = 1000;
            FadeOutTime = 1000;
            FadeIn();
        }

        protected override void FadeInCompleted()
        {
            base.FadeInCompleted();
            _redirect.Load();
            _waitTimer.Start(_waitTime);
        }

        protected override void FadeOutCompleted()
        {
            base.FadeOutCompleted();

            TutorialScreen tutorialScreen = null;
            if (_type == TileType.Hardcore)
            {
                tutorialScreen = TutorialScreen.GetTutorialScreen(_nextLevel, _redirect);
            }
            else if (_type == TileType.Normal)
            {
                tutorialScreen = TutorialScreen.GetTutorialScreenNormal(_nextLevel, _redirect);
            }

            if (tutorialScreen == null)
                EngineGlobals.ScreenManager.QueueChangeScreen(_redirect, true);
            else
            {
                EngineGlobals.ScreenManager.QueueChangeScreen(tutorialScreen);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_waitTimer.Ticking)
            {
                _waitTimer.Update();
                if (_waitTimer.Finished)
                {
                    FadeOut();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Controller.Draw();
        }

        public override void Dispose()
        {
            base.Dispose();
            Controller.RemoveObject(_levelCount);
            Controller.RemoveObject(_levelName);
            foreach (var levelIcon in _levelIcons)
            {
                Controller.RemoveObject(levelIcon);
            }
            Controller.RemoveObject(_loadingImage);
        }
    }
}
