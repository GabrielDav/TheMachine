using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.ScreenManagement;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class SplashScreen : IScreen
    {
        protected Timer _fadeTimer;
        protected int _loadStage = 0;
        protected Image _logoImg;
        protected Image _splashImg;
        #if WINDOWS_PHONE
        protected const bool FAST_INTRO = false;
        #else
        protected const bool FAST_INTRO = true;
        #endif

        protected const int LOGO_FADE_IN = 1000;
        protected const int LOGO_FADE_OUT = 1000;
        protected const int LOGO_SHOW_TIME = 2000;
        protected const int SPLASH_FADE_IN = 1000;
        protected const int SPLASH_FADE_OUT = 1000;
        protected const int SPLASG_SHOW_TIME = 1000;



        public IScreen Parent { get; set; }
        public List<IScreen> ChildScreens { get; set; }
        public bool IsPopup { get; set; }

        public ScreenState State { get; set; }


        public void Initialize()
        {
            _fadeTimer = new Timer();
            if (!FAST_INTRO)
                _fadeTimer.Start(LOGO_FADE_IN, false);
        }

        public void Load()
        {
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("LogoImg", new[] { "Gui/CompanyLogo" }, ResourceType.Texture));
            _logoImg = new Image(EngineGlobals.Resources.Textures["LogoImg"][0], new Rectangle(0, 0, 800, 480)) {Transparency = 0, Owner = this};
            
        }

        public void LoadGlobalData()
        {
            LaodParticles();
            LoadSounds();
            LoadMusic();
            if (GameGlobals.Storage == null)
            {
                GameGlobals.Storage = new StorageControl();
            }
#if WINDOWS_PHONE
            GameGlobals.AdControl = new AdControl(new Vector2(640, 25), new Vector2(320, 50));
            GameGlobals.AdControl.Init();
#endif
            if (GameGlobals.Settings == null)
            {
                Settings.Load();
                MusicManager.Play("menu", true);
            }
        }

        public void LaodParticles()
        {
            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier("Splash", new[] { @"Particles\Splash" }, ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier("Splash_Red", new[] { @"Particles\Splash_Red" }, ResourceType.Texture));

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
                    "SplashWall_Red",
                    new[]
                        {
                            @"Particles\SplashWall_1_Red",
                            @"Particles\SplashWall_2_Red",
                            @"Particles\SplashWall_3_Red",
                            @"Particles\SplashWall_4_Red",
                            @"Particles\SplashWall_5_Red",
                            @"Particles\SplashWall_6_Red"
                        },
                    ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "Splash_Death",
                    new[] { @"Particles\Splash_Death" },
                    ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "Splash_Death_Red",
                    new[] { @"Particles\Splash_Death_Red" },
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
                new ResourceIdentifier("ScoreBack", new[] { @"Gui\ScoreBack" },
                                       ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier("BtnPointer", new[] { @"Gui\Menu\BtnPointer" },
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

            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("SeekerParticle",
                                                                     new[] { @"Particles\SeakerParticle_1", @"Particles\SeakerParticle_2" },
                                                                     ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier("Hit", new[] { @"Particles\Hit" },
                                       ResourceType.Texture));

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "marker", new[]
                        {
                            @"Gui\marker"
                        },
                    ResourceType.Texture));
        }

        protected void LoadSounds()
        {
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("stick", new[] { @"Audio\Stick" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("death", new[] { @"Audio\Death" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("shot", new[] { @"Audio\Shot" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("hit", new[] { @"Audio\Hit" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("click", new[] { @"Audio\click2" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("explode", new[] { @"Audio\explode" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("backtonormal", new[] { @"Audio\backtonormal" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("collectink", new[] { @"Audio\collectink" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("shatter", new[] { @"Audio\shatter" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("slowdown", new[] { @"Audio\slowdown" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("trapBtn", new[] { @"Audio\trapBtn" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("tick", new[] { @"Audio\tick" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("ticking", new[] { @"Audio\ticking" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("switchOff", new[] { @"Audio\switchOff" }, ResourceType.Sound));
        }

        protected void LoadMusic()
        {
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("arcade", new[] { @"Audio\Music\arcade" }, ResourceType.Song));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("menu", new[] { @"Audio\Music\menu" }, ResourceType.Song));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("game", new[] { @"Audio\Music\game" }, ResourceType.Song));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("score", new[] { @"Audio\Music\score" }, ResourceType.Song));
        }

        public void Dispose()
        {
            _logoImg.Dispose();
            _logoImg = null;
            _fadeTimer = null;
        }

        public void HandleTouch(Point p, object sender)
        {
        }

        public void HandleBack(object sender)
        {
        }

        public void Update(GameTime gameTime)
        {
            EngineGlobals.GameTime = gameTime;
            if (FAST_INTRO)
            {
                LoadGlobalData();
                EngineGlobals.ScreenManager.QueueChangeScreen(new MainMenuScreen());
                return;
            }
            if (_fadeTimer.Ticking)
            {
                _fadeTimer.Update();
                if (_fadeTimer.Finished)
                {

                    switch (_loadStage)
                    {
                        case 0:
                            _logoImg.Transparency = 100;
                            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("SplashImg", new[] { "Gui/GameLogo" }, ResourceType.Texture));
                            _splashImg = new Image(EngineGlobals.Resources.Textures["SplashImg"][0], new Rectangle(0, 0, 800, 480)) {Transparency = 0, Owner = this};
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
                            EngineGlobals.ScreenManager.QueueChangeScreen(new MainMenuScreen(true));
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
        }

        public void Draw(GameTime gameTime)
        {
            EngineGlobals.Batch.Begin();
            if (_loadStage < 3)
                _logoImg.Draw();
            else
                _splashImg.Draw();
            EngineGlobals.Batch.End();
        }
    }
}
