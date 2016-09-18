#if !WINDOWS_PHONE
using System.Management.Instrumentation;
using System.Resources;
using System.Windows.Forms;
#endif
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Arcade;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using TheGoo;
using Timer = Engine.Core.Timer;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class ArcadeScreen : GameScreen
    {
        protected LevelBuilder _levelBuilder;
        protected readonly Timer _gameOverScreenTimer;
        protected Sprite _timeWarpSprite;
        protected Sprite _inkCollectSprite;
        protected Sprite _powerupSprite;
        protected Timer _powerUpTimer;
        
        public ArcadeScreen()
            : base(-1, null)
        {
            GameGlobals.HardcoreMode = false;
            _gameOverScreenTimer = new Timer();
        }

        public void InitArcade()
        {
            _levelBuilder = new LevelBuilder();
            _levelBuilder.Init();
            _levelBuilder.Start();
            _powerUpTimer = new Timer(false);

            MusicManager.Play("arcade", true);
            
            EngineGlobals.Camera2D.Zoom = 0.8f;

            InitBackgroundScene();
        }

        protected virtual void InitBackgroundScene()
        {
            var scenes = new[]
            {
                new ArcadeBackgroundScene
                {
                    Objects = new[] {"Scene_1_BackgroundGear1", "Scene_1_BackgroundGear2"},
                    Offset = new Vector2(-100, 4820)
                },
                new ArcadeBackgroundScene
                {
                    Objects = new[] {"Scene_2_BackgroundGear3", "Scene_2_BackgroundGear4", "Scene_2_BackgroundGear5", "Scene_2_BackgroundGear6"},
                    Offset = new Vector2(-50, 4720)
                },
                new ArcadeBackgroundScene
                {
                    Objects = new[] {"Scene_3_BackgroundGear1", "Scene_3_BackgroundGear2"},
                    Offset = new Vector2(-90, 4620)
                },
            };
            EngineGlobals.Camera2D.BackgroundOffset = new Vector2(400, 210);
            EngineGlobals.Camera2D.BackgroundSpeedModifier = 100;
            scenes[GameGlobals.Random.Next(0, scenes.Length)].Init();
        }

        public override void Load()
        {
            GameGlobals.ArcadeMode = true;
            base.Load();
            EngineGlobals.Resources.LoadSprite(new ResourceIdentifier("TimeWarpCooldown",
                new[] {@"GameObjects\Sprites\TimeWarpCooldown\TimeWarpCooldown"}, ResourceType.Sprite));
            _timeWarpSprite = new Sprite(EngineGlobals.Resources.Sprites["TimeWarpCooldown"][0]);
            EngineGlobals.Resources.LoadSprite(new ResourceIdentifier("InkCollectCooldown",
                new[] { @"GameObjects\Sprites\InkCollectCooldown\InkCollectCooldown" }, ResourceType.Sprite));
            _inkCollectSprite = new Sprite(EngineGlobals.Resources.Sprites["InkCollectCooldown"][0]);
        }

        public override void SetupData()
        {
            base.SetupData();
            InitArcade();
        }

        public override void Initialize()
        {
            base.Initialize();
#if WINDOWS_PHONE
            GameGlobals.AdControl.Setup(new Vector2(400, 455), Orientation.Bottom);
#endif
        }

        protected override void GameOverInput()
        {
        }

        protected override void DoGameOver()
        {
            base.DoGameOver();
            _gameOverScreenTimer.Start(10);
            GameGlobals.ArcadeMode = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GameGlobals.GameOver)
            {
                if (_gameOverScreenTimer.Ticking)
                {
                    _gameOverScreenTimer.Update();
                    if (_gameOverScreenTimer.Finished /*&& _gameOverTextColorEffect.Finished && _outOfInkTextColorEffect.Finished*/)
                        EngineGlobals.ScreenManager.QueueChangeScreen(new BaseLoadingScreen(new LevelCompleteScreen(-1, null)));

                }
                return;
            }
            if (GameGlobals.Player.CurrentPowerUp != PowerUpType.None)
            {
                if (_powerUpTimer.Ticking)
                {
                    _powerUpTimer.Update();
                    if (_powerUpTimer.Finished)
                    {
                        if (GameGlobals.Player.CurrentPowerUp == PowerUpType.InkCollect)
                            GameGlobals.Player.CurrentPowerUp = PowerUpType.None;
                        else if (GameGlobals.Player.CurrentPowerUp == PowerUpType.TimeWarp)
                            GameGlobals.Player.StopTimeWarpPowerup();
                        RemoveArcadePowerup();
                    }
                    else
                    {
                        var duration = GameGlobals.Player.CurrentPowerUp == PowerUpType.InkCollect
                            ? GameGlobals.ArcadeInkCollectDuration
                            : GameGlobals.ArcadeTimeWarpDuration;
                        var step = 0;
                        if (_powerUpTimer.TimeElapsed < duration/5)
                            step = 0;
                        else if (_powerUpTimer.TimeElapsed < (duration / 5) * 2)
                            step = 1;
                        else if (_powerUpTimer.TimeElapsed < (duration / 5) * 3)
                            step = 2;
                        else if (_powerUpTimer.TimeElapsed < (duration/5)*4)
                            step = 3;
                        else
                            step = 4;
                        if (_powerupSprite.CurrentFrame != step)
                            _powerupSprite.CurrentFrame = step;
                    }
                }
            }
            _levelBuilder.Update();
        }

        public void SetArcadePowerup(PowerUpType powerUpType)
        {
            if (_powerupSprite != null)
            {
                Controller.RemoveObject(_powerupSprite);
            }
            if (powerUpType == PowerUpType.TimeWarp)
            {
                _powerupSprite = _timeWarpSprite;
                _powerUpTimer.Start(GameGlobals.ArcadeTimeWarpDuration);
            }
            else if (powerUpType == PowerUpType.InkCollect)
            {
                _powerupSprite = _inkCollectSprite;
                _powerUpTimer.Start(GameGlobals.ArcadeInkCollectDuration);
            }

            _powerupSprite.SetAnimation("main");
            _powerupSprite.CurrentFrame = 0;
            _powerupSprite.Rect = new Rectangle(2, 320, 50, 58);
            _powerupSprite.StaticPosition = true;
            Controller.AddObject(_powerupSprite);
        }

        public void RemoveArcadePowerup()
        {
            if (_powerupSprite != null)
            {
                Controller.RemoveObject(_powerupSprite);
                _powerupSprite = null;
            }
        }

    }
}
