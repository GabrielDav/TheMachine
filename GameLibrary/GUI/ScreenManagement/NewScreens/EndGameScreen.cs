using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Gui.ScreenManagement.NewScreens
{
    public class EndGameScreen : BaseMainScreen
    {
        private Image _image;
        private ColorEffect _imageFadeEffect;
        private bool _imageFading;
        protected bool _queueMainMenu;
        private bool _normalCompleted;
        private bool _hardcoreCompleted;


       public EndGameScreen(bool normalCompleted, bool hardcoreCompleted)
       {
           _normalCompleted = normalCompleted;
           _hardcoreCompleted = hardcoreCompleted;
           FadeInTime = 200;
           FadeOutTime = 200;
       }

       public override void Load()
       {
           base.Load();
           var content = Controller.CurrentGame.Content;
           GameGlobals.MenuGlobals.MenuFont = content.Load<SpriteFont>("GameFont");
           Map.Load(GameGlobals.MapList.GameEndMap, content);
           EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("End_Game_Text_Box", new[]
           {
               "GameObjects\\Images\\End_Game_Text_Box_Hardcore",
               "GameObjects\\Images\\End_Game_Text_Box_Normal",
               "GameObjects\\Images\\End_Game_Text_Box"
           }, ResourceType.Texture));

       }

       public override void Initialize()
       {
           base.Initialize();
           WindowOnBack += OnWindowOnBack;
           FadeIn();
           foreach (var physicalObject in GameGlobals.Map.GameObjects)
           {
               physicalObject.Load(physicalObject.ResourceId, physicalObject.ResourceVariation);
               Controller.AddGameObject(physicalObject);
           }
           SetUpData();
       }

        protected void SetUpData()
        {
            SetCamera();
            SetMessage();
            InitBackground();
        }

        private void SetMessage()
        {
            int endScreenType = 0;

            if (_normalCompleted && _hardcoreCompleted)
            {
                endScreenType = 2;
            }
            else if (_normalCompleted)
            {
                endScreenType = 1;
            }

            _image = new Image(EngineGlobals.Resources.Textures["End_Game_Text_Box"][endScreenType])
                {
                    Rect = new Rectangle(0, 0, 800, 480),
                    LayerDepth = 0.49f,
                    StaticPosition = true
                };

            Controller.AddObject(_image);
        }

        public void SetCamera()
        {
            EngineGlobals.Camera2D.SetCamerBounds(new Rectangle(
                                                      0,
                                                      0,
                                                      GameGlobals.Map.Width,
                                                      GameGlobals.Map.Height));
            EngineGlobals.Camera2D.BackgroundOffset = new Vector2(350f, 210f);
            EngineGlobals.Camera2D.Position = new Vector2(3050, 2800);
        }

        public void InitBackground()
        {
            if (!string.IsNullOrEmpty(GameGlobals.Map.Background))
            {
                EngineGlobals.Background = new BackgroundManager(GameGlobals.Map.Width, GameGlobals.Map.Height);
                EngineGlobals.Background.LoadBackground(new GameTexture(GameGlobals.Map.Background));
            }
        }

        public override void Update(GameTime gameTime)
        {
            EngineGlobals.Camera2D.Update();
            if (_imageFading)
            {
                _imageFadeEffect.Update();
            }
            base.Update(gameTime);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (EngineGlobals.Background != null)
            {
                EngineGlobals.Background.Dispose();
                EngineGlobals.Background = null;
            }
            WindowOnBack -= OnWindowOnBack;
            GameGlobals.Map = null;
            GameGlobals.Menu = null;
        }

        private void OnWindowOnBack(object sender)
        {
            _queueMainMenu = true;
           // CreateShadowImg(_buttonMenu);
            FadeOut();
        }

        protected override void FadeOut()
        {
            _imageFadeEffect = new ColorEffect(_image, new Color(255, 255, 255, 255), FadeOutTime);
            _imageFading = true;

            base.FadeOut();
        }

        protected override void FadeOutCompleted()
        {
            base.FadeOutCompleted();
            if (_queueMainMenu)
            {
                EngineGlobals.ScreenManager.QueueChangeScreen(new BaseLoadingScreen(new MainMenuScreen()));
            }
        }
    }
}
