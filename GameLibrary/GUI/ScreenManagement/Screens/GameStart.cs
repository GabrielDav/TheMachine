using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.Gui.ScreenManagement.Screens
{
    public class GameStart : GameScreen
    {
        protected Timer _fadeTimer;
        protected int _loadStage = 0;
        protected Image _logoImg;
        protected Image _splashImg;
        protected const int LOGO_FADE_IN = 1000;
        protected const int LOGO_FADE_OUT = 1000;
        protected const int LOGO_SHOW_TIME = 2000;
        protected const int SPLASH_FADE_IN = 1000;
        protected const int SPLASH_FADE_OUT = 1000;
        protected const int SPLASG_SHOW_TIME = 1000;

        public GameStart()
        {
            _fadeTimer = new Timer();

        }

        public void Show()
        {
#if WINDOWS
            LoadGlobalData();
            Controller.ScreenManager.AddScreen(new MainMenuScreen());
#else
            Controller.ScreenManager.AddScreen(this);
#endif

        }

        public override void LoadContent()
        {
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("LogoImg", new[] {"Gui/CompanyLogo"}, ResourceType.Texture));
            _logoImg = new Image(EngineGlobals.Resources.Textures["LogoImg"][0], new Rectangle(0,0,800,480));
            _logoImg.Transparency = 0;
            _fadeTimer.Start(LOGO_FADE_IN, false);
            base.LoadContent();
        }

        public void LoadGlobalData()
        {
            LaodParticles();
            LoadSounds();
            LoadMusic();
        }

        public void LaodParticles()
        {
            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier("Splash", new[] { @"Particles\Splash" }, ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "SplashWall",
                    new[]
                        {
                            @"Particles\SplashWall_1",
                            @"Particles\SplashWall_2",
                            @"Particles\SplashWall_3",
                            @"Particles\SplashWall_4",
                            @"Particles\SplashWall_5",
                            @"Particles\SplashWall_6"
                        },
                    ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "Splash_Death",
                    new[] { @"Particles\Splash_Death" },
                    ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "Particle",
                    new[] { @"Particles\Particle" },
                    ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "HealthBar",
                    new[] { @"GameObjects\Images\Bar" },
                    ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "HealthBarBorder",
                    new[] { @"GameObjects\Images\Border" },
                    ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier("Back", new[] {@"Gui\ScoreBack"},
                                       ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "SawParticle",
                    new[]
                        {
                            @"Particles\SawParticle_1", @"Particles\SawParticle_2",
                            @"Particles\SawParticle_3"
                        },
                    ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier("Hit", new[] {@"Particles\Hit"},
                                       ResourceType.Texture));
        }

        protected void LoadSounds()
        {
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("stick", new[] { @"Audio\Stick" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("saw", new[] { @"Audio\Saw" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("death", new[] { @"Audio\Death" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("shot", new[] { @"Audio\Shot" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("hit", new[] { @"Audio\Hit" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("click", new[] { @"Audio\click2" }, ResourceType.Sound));
        }

        protected void LoadMusic()
        {
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("menu", new[] { @"Audio\Music\Menu" }, ResourceType.Song));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("arcade", new[] { @"Audio\Music\Arcade" }, ResourceType.Song));
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            EngineGlobals.GameTime = gameTime;
            if (_fadeTimer.Ticking)
            {
                _fadeTimer.Update();
                if (_fadeTimer.Finished)
                {
                    switch (_loadStage)
                    {
                        case 0:
                            _logoImg.Transparency = 100;
                            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("SplashImg", new[] {"Gui/GameLogo"}, ResourceType.Texture));
                            _splashImg = new Image(EngineGlobals.Resources.Textures["SplashImg"][0], new Rectangle(0, 0, 800, 480));
                            _splashImg.Transparency = 0;
                            _fadeTimer.Start(LOGO_SHOW_TIME, false);
                            _loadStage = 1;
                            break;
                        case 1:
                            _loadStage = 2;
                            _fadeTimer.Start(LOGO_FADE_OUT, false);
                            break;
                        case 2:
                            _loadStage = 3;
                            _fadeTimer.Start(SPLASH_FADE_IN, false);
                            break;
                        case 3:
                            _loadStage = 4;
                            LoadGlobalData();
                            _fadeTimer.Start(SPLASG_SHOW_TIME, false);
                            break;
                        case 4:
                            _loadStage = 5;
                            _fadeTimer.Start(SPLASH_FADE_OUT, false);
                            break;
                        case 5:
                            Controller.ScreenManager.AddScreen(new MainMenuScreen());
                            Controller.ScreenManager.RemoveScreen(this);
                            break;
                    }
                }
                else
                {
                    if (_loadStage == 0)
                    {
                        _logoImg.Transparency = (int)_fadeTimer.TimeElapsed * 100 / LOGO_FADE_IN;
                    }
                    else if (_loadStage == 2)
                    {
                        _logoImg.Transparency = (int)_fadeTimer.TimeLeft * 100 / LOGO_FADE_OUT;
                    }
                    else if (_loadStage == 3)
                    {
                        _splashImg.Transparency = (int)_fadeTimer.TimeElapsed * 100 / LOGO_FADE_IN;
                    }
                    else if (_loadStage == 5)
                    {
                        _splashImg.Transparency = (int)_fadeTimer.TimeLeft * 100 / LOGO_FADE_OUT;
                    }
                }
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            EngineGlobals.Batch.Begin();
            if (_loadStage < 3)
                _logoImg.Draw();
            else
                _splashImg.Draw();
            base.Draw(gameTime);
            EngineGlobals.Batch.End();
        }

    }
}
