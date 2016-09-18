using System;
using Engine.Core;
using Engine.Graphics;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class TutorialScreen : BaseFadeScreen
    {
        protected Image _shadowImg;
        protected ResizeEffect _resizeEffect;
        protected ColorEffect _colorEffect;

        protected string _imagePath;
        protected string _text;
        protected Image _continueButton;
        protected Image _tutorialImage;
        protected Image _tutorialTextBackground;
        protected TextRegion _tutorialText;
        protected ColorEffect _colorEffectFadeOut;
        protected ColorEffect _colorEffectFadeIn;
        protected Image _tutorialBackground;
        protected IScreen _levelScreen;
        protected bool _preload;

        public TutorialScreen(string imagePath, string text, IScreen levelScreen, bool preload)
        {
            _imagePath = imagePath;
            _text = text;
            _levelScreen = levelScreen;
            _preload = preload;
        }

        public TutorialScreen(string imagePath, string text, IScreen levelScreen)
            : this(imagePath, text, levelScreen, false)
        {
            
        }

        public override void Load()
        {
            base.Load();
            EngineGlobals.Camera2D = new Camera2D();
            var texture = new GameTexture(_imagePath);
            _tutorialBackground = new Image(new GameTexture("TutorialBackground")) { LayerDepth = 0.8f, Owner = this};
            _tutorialBackground.Rect = new Rectangle(0, 0, 800, 480);
            Controller.AddObject(_tutorialBackground);
            _tutorialImage = new Image(texture) { LayerDepth = 0.5f, Owner = this };
            _tutorialImage.Rect = new Rectangle(400 - (texture.Data.Width/2), 185 - (texture.Data.Height/2),
                texture.Data.Width, texture.Data.Height);
            Controller.AddObject(_tutorialImage);
            _tutorialText = new TextRegion(new Rectangle(10, 365, 660, 200), GameGlobals.Font, Color.Black, _text, true) { LayerDepth = 0.5f, Owner = this };
            Controller.AddObject(_tutorialText);
            _tutorialTextBackground = new Image(new GameTexture(@"Gui\TutorialTextBackground")) { LayerDepth = 0.6f, Owner = this };
            _tutorialTextBackground.Rect = new Rectangle(2, 359, 672, 114);
            Controller.AddObject(_tutorialTextBackground);
            _continueButton = new Image(new GameTexture(@"Gui\Menu\ScoreScreen\ButtonContinue")) { LayerDepth = 0.5f, Owner = this };
            _continueButton.Rect = new Rectangle(675, 364, 115, 104);
            Controller.AddObject(_continueButton);
            if (_preload)
                _levelScreen.Load();
        }

        public override void Initialize()
        {
            base.Initialize();
            FadeInTime = 400;
            FadeOutTime = 400;
            EngineGlobals.Camera2D.CornerPos = new Vector2(0, 0);
            _continueButton.Color = new Color(255, 255, 255, 0);
            _colorEffectFadeIn = new ColorEffect(_continueButton, Color.White, 1000);
            _continueButton.Color = Color.White;
            _colorEffectFadeOut = new ColorEffect(_continueButton, new Color(255, 255, 255, 0), 1000);
            WindowOnInput += OnWindowOnInput;
            WindowOnBack += OnWindowOnBack;
            FadeIn();
        }

        private void OnWindowOnBack(object sender)
        {
            CreateShadowImg(_continueButton);
            FadeOut();
        }

        private void OnWindowOnInput(object sender, Point location)
        {
            if (_continueButton.Rect.Contains(location))
            {
                CreateShadowImg(_continueButton);
                FadeOut();
            }
        }

        protected virtual void CreateShadowImg(Image image)
        {
            if (_shadowImg == null)
            {
                _shadowImg = new Image(new GameTexture(image.Texture));
                _shadowImg.Orgin = _shadowImg.OriginCenter();
                _shadowImg.Flip = image.Flip;
            }
            _shadowImg.Rect = new Rectangle((int)(image.Rect.X + image.Rect.Width / 2f),
                                            (int)(image.Rect.Y + image.Rect.Height / 2f), image.Rect.Width,
                                            image.Rect.Height);
            _shadowImg.Color = new Color(255, 255, 255, 255);
            if (_resizeEffect == null)
                _resizeEffect = new ResizeEffect(_shadowImg, new Vector2((int)(image.Rect.Width * 1.5), (int)(image.Rect.Height * 1.5)), 500);
            else
                _resizeEffect.Reset(new Vector2((int)(image.Rect.Width * 1.5), (int)(image.Rect.Height * 1.5)), 500);
            if (_colorEffect == null)
                _colorEffect = new ColorEffect(_shadowImg, new Color(255, 255, 255, 0), 500);
            else
                _colorEffect.Reset(new Color(255, 255, 255, 0), 500);
            Controller.AddObject(_shadowImg);
        }

        public override void Draw(GameTime gameTime)
        {
            Controller.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _colorEffectFadeIn.Update();
            if (_colorEffectFadeIn.Finished)
            {
                if (_colorEffectFadeOut.Finished)
                {
                    _colorEffectFadeOut.Reset(new Color(255, 255, 255, 0), 1000);
                    return;
                }
                _colorEffectFadeOut.Update();
                if (_colorEffectFadeOut.Finished)
                {
                    _colorEffectFadeIn.Reset(Color.White, 1000);
                }
            }
            else
            {
                _colorEffectFadeIn.Update();
            }

            if (_resizeEffect != null)
            {
                _resizeEffect.Update();
                _colorEffect.Update();
            }
        }

        protected override void FadeOutCompleted()
        {
            base.FadeOutCompleted();
            EngineGlobals.ScreenManager.QueueChangeScreen(_levelScreen, true);
        }


        public override void Dispose()
        {
            base.Dispose();
            Controller.RemoveObject(_tutorialImage);
            Controller.RemoveObject(_tutorialText);
            Controller.RemoveObject(_continueButton);
            Controller.ClearObjectsBuffers();
        }

        public static TutorialScreen GetTutorialScreen(int level, IScreen levelScreen)
        {
            switch (level)
            {
                case -1:
                    return new TutorialScreen(@"Gui\Tutorials\Arcade.png", "In the Arcade you can perform multiple jumps in mid air but this kind of jumps costs additional ink.", levelScreen, true);
                case 0:
                    return new TutorialScreen(@"Gui\Tutorials\Level4.png", "You lose the game if ink drop collides with spikes!", levelScreen);
                case 2:
                    return new TutorialScreen(@"Gui\Tutorials\Level3.png", "When on circle tap anywhere on the screen to jump into direction ink drop is facing.", levelScreen);
                case 4:
                    return new TutorialScreen(@"Gui\Tutorials\Level5.png", "You lose the game if you run out of ink!", levelScreen);
                case 5:
                    return new TutorialScreen(@"Gui\Tutorials\Level6.png", "Jumping on slide wall will cause ink dot to slide down.", levelScreen);
                case 7:
                    return new TutorialScreen(@"Gui\Tutorials\Level8.png", "You lose the game if ink drop collides with disc saw!", levelScreen);
                case 10:
                    return new TutorialScreen(@"Gui\Tutorials\Level11.png", "Spike circle works as usual circle until time indicated by timer on it runs out. Then spike circle unleashes spikes and becomes deadly until timer resets.", levelScreen);
                case 12:
                    return new TutorialScreen(@"Gui\Tutorials\Level13.png", "Spike shooter shoots spike projectiles at certain time intervals witch are indicated by timer on it. If ink drop collides with spike projectile you lose the game!", levelScreen);
                case 14:
                    return new TutorialScreen(@"Gui\Tutorials\Level15.png", "\"Double Jump\" power-up allows to perform second jump in mid-air. If ink drop stick to surface this opportunity is lost.", levelScreen);
                case 16:
                    return new TutorialScreen(@"Gui\Tutorials\Level17.png", "\"Jump Spot\" allows to jump in any direction, but if time runs out ink drop will fall down without ability to change its direction.", levelScreen);
                case 19:
                    return new TutorialScreen(@"Gui\Tutorials\Level20.png", "\"Ink Collection\" power-up allows to collect ink from vast distances. To activate it you must tap anywhere on the screen after picking up the power-up. If ink drop stick to any surface or picks up another power-up the opportunity is lost.", levelScreen);
                case 20:
                    return new TutorialScreen(@"Gui\Tutorials\Level21.png", "If you jump on \"Activation Square\" it will activate some objects that were static before.", levelScreen);
                case 22:
                    return new TutorialScreen(@"Gui\Tutorials\Level23.png", "\"Time Warp\" power-up slows down various traps, but will not slow ink dot or circles rotation speed. To activate it you must tap anywhere on the screen after picking up the power-up. If ink drop stick to any surface opportunity is lost.", levelScreen);
                case 23:
                    return new TutorialScreen(@"Gui\Tutorials\Level24.png", "\"Seeker Bug\" will start to chase you if you get close. If ink drop collides with it you will lose the game!", levelScreen);
                case 24:
                    return new TutorialScreen(@"Gui\Tutorials\Level25.png", "Pick up shield to destroy all \"Seeker Bugs\" that are chasing you.", levelScreen);
                default:
                    return null;
            }
        }

        public static TutorialScreen GetTutorialScreenNormal(int level, IScreen levelScreen)
        {
            switch (level)
            {
                case -1:
                    return new TutorialScreen(@"Gui\Tutorials\Arcade.png", "In the Arcade you can perform multiple jumps in mid air but this kind of jumps costs additional ink.", levelScreen, true);
                case 0:
                    return new TutorialScreen(@"Gui\Tutorials\Level1.png", "Tap on the screen to jump into desired direction.", levelScreen);
                case 1:
                    return new TutorialScreen(@"Gui\Tutorials\Level3.png", "When on circle tap anywhere on the screen to jump into direction ink drop is facing.", levelScreen);
                case 2:
                    return new TutorialScreen(@"Gui\Tutorials\Level5.png", "You lose the game if you run out of ink!", levelScreen);
                case 4:
                    return new TutorialScreen(@"Gui\Tutorials\Level4.png", "You lose the game if ink drop collides with spikes!", levelScreen);    
                case 5:
                    return new TutorialScreen(@"Gui\Tutorials\Level8.png", "You lose the game if ink drop collides with disc saw!", levelScreen);
                case 7:
                    return new TutorialScreen(@"Gui\Tutorials\Level6.png", "Jumping on slide wall will cause ink dot to slide down.", levelScreen);
                case 8:
                    return new TutorialScreen(@"Gui\Tutorials\Level17.png", "\"Jump Spot\" allows to jump in any direction, but if time runs out ink drop will fall down without ability to change its direction.", levelScreen);
                case 11:
                    return new TutorialScreen(@"Gui\Tutorials\Level21.png", "If you jump on \"Activation Square\" it will activate some objects that were static before.", levelScreen);
                default:
                    return null;
            }
        }

    }
}
