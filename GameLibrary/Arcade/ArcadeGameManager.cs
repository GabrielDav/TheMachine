using System;
using Engine.Core;
using Engine.Graphics;
using GameLibrary.Gui;
//using GameLibrary.Gui.ScreenManagement.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using TheGoo;

namespace GameLibrary.Arcade
{ 
    public class ArcadeGameManager : GameManager
    {
        private LevelBuilder _levelBuilder;

        public void InitArcade()
        {
            _levelBuilder = new LevelBuilder();
            _levelBuilder.Init();
            _levelBuilder.Start();
        }

        /*public override void SetupData()
        {
            InitArcade();
        }*/

        public void UpdateArcade(GameTime gameTime)
        {
            EngineGlobals.GameTime = gameTime;
            EngineGlobals.ElapsedTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

            if (!GameGlobals.GameOver)
            {
                GameGlobals.Physics.Update();

                EngineGlobals.Camera2D.Update();

                EngineGlobals.ParticleStorageManager.Update();

                Controller.Update();

                EngineGlobals.SoundManager.Update(GameGlobals.Player.HalfPos);

                _levelBuilder.Update();
            }

            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _fps = _numOfFrames;
                _numOfFrames = 0;
            }

            #if EDITOR
            UpdateMassages();
            #endif

            
        }

        public override void SetUpData()
        {
            //EngineGlobals.SoundManager.AddMusic(EngineGlobals.Content.Load<Song>("Audio/Music/sample1"), "arcade");
            CloneMap();
            AddPhysicalObjects();
            InitSound();
            InitInput();
            EngineGlobals.Resources.LoadResources(GameGlobals.Map.Resources);
            GameGlobals.Physics.LoadMapObjects(GameGlobals.Map.GameObjects);
            SetCamera();
            //EngineGlobals.Camera2D.MoveX = false;

            #if EDITOR
            InitGameMessages();
            #endif
            InitBackground();

            InitHealthBar();

            InitScore();

            GameGlobals.Player.StartPosition = GameGlobals.Player.HalfPos;
            GameGlobals.Physics.PushObject(GameGlobals.Player, new Vector2(0, 0));
            GameGlobals.Physics.CommitQueue();

            EngineGlobals.ParticleStorageManager = new ParticleStorageManager();
            InitArcade();
            Controller.CurrentGame.ResetElapsedTime();
            MusicManager.Play("arcade", true);
        }

        public override void DispoeMap()
        {
            base.DispoeMap();
            _levelBuilder = null;
        }

        public override void InitGameMessages()
        {
            GameGlobals.GameMessages = new GameMessages(GameGlobals.Font);
            GameGlobals.GameMessages.RegisterRecord("fps", "FPS", "0", true);
            GameGlobals.GameMessages.RegisterRecord("h", "Height", "0", true);
            GameGlobals.GameMessages.RegisterRecord("lvl", "Level", "0", true);
            GameGlobals.GameMessages.RegisterRecord("speed", "Speed", "0", true);
            GameGlobals.GameMessages.InitInfoRegion();
            Controller.AddObject(GameGlobals.GameMessages);
        }

        protected override void UpdateMassages()
        {
            GameGlobals.GameMessages.UpdateInfoText(
                "fps",
                _fps.ToString());
            GameGlobals.GameMessages.UpdateInfoText(
                "h",
                _levelBuilder.Height.ToString());
            GameGlobals.GameMessages.UpdateInfoText(
                "lvl",
                _levelBuilder.Level.ToString());
            GameGlobals.GameMessages.UpdateInfoText(
                "speed",
                GameGlobals.Player.Mask.MoveSpeed.ToString());
        }

        protected override void GameOverInput()
        {
            Inactive = true;
            /*LoadingScreen.Load(
                    false,
                    true,
                    new ReplayScreen(this));*/
        }

    }
}
