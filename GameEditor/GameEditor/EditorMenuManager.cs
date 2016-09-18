using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using GameLibrary;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using TheGoo;
using Timer = System.Windows.Forms.Timer;

namespace GameEditor
{
    public class EditorMenuManager : MenuManager, IDisposable, IEditorGameEmulator
    {
        protected readonly Timer _timer;
        protected TimeSpan _totalTime;
        protected Camera2D _baseCamera;
        protected DateTime _time;
        protected readonly List<PhysicalObject> _mapGameObjects;
        protected readonly List<PhysicalObject> _mapBackgourndObjects;
        protected readonly List<IGraphicsObject> _mapForegroundObjects;
        protected readonly List<IGraphicsObject> _mapOnScreenObjects;
        protected Map _baseMap;

        public EditorMenuManager(Map map)
        {
         
            _baseMap = map;
            GameGlobals.Map = (Map)_baseMap.Clone();
            _timer = new Timer {Interval = 16};
            _timer.Tick += TimerOnTick;
            var objectsBuffers = Controller.GetObjectsBuffers();
            _mapOnScreenObjects = objectsBuffers.Item1;
            _mapForegroundObjects = objectsBuffers.Item2;
            _mapBackgourndObjects = objectsBuffers.Item3;
            _mapGameObjects = objectsBuffers.Item4;
            if (GameGlobals.Storage == null)
                GameGlobals.Storage = new StorageControl();
            if (GameGlobals.Settings == null)
            {
                Settings.Load();
            }
            Controller.ClearObjectsBuffers();
            
            GameGlobals.GameOver = false;
            EngineGlobals.GameTime = new GameTime(new TimeSpan(0), new TimeSpan(0));
            _baseCamera = EngineGlobals.Camera2D;
            EngineGlobals.Camera2D = new Camera2D();
            Initialize();
            SetUpData();
          //  LoadGameData();
            
          //  AfterLoadInit();
            _timer.Start();
            _time = DateTime.Now;
            
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            //if (GameGlobals.GameOver)
            //{
            //    Form1.Instance.StopGame(this, null);
            //    return;
            //}
            var elapsedTIme = DateTime.Now - _time;
            _totalTime = _totalTime.Add(elapsedTIme);
            var gameTime = new GameTime(_totalTime, elapsedTIme);
            Update(gameTime);
            _time = DateTime.Now;
        }

        public void SetState(bool paused)
        {
            if (paused)
            {
                _timer.Stop();
            }
            else
            {
                _timer.Start();
                _time = DateTime.Now;
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            Controller.ClearObjectsBuffers();
            foreach (var mapGameObject in _mapGameObjects)
            {
                Controller.AddGameObject(mapGameObject);
            }
            foreach (var mapBackgourndObject in _mapBackgourndObjects)
            {
                Controller.AddBackgroundObject(mapBackgourndObject);
            }
            foreach (var mapForegroundObject in _mapForegroundObjects)
            {
                Controller.AddObject(mapForegroundObject);
            }
            foreach (var mapOnScreenObject in _mapOnScreenObjects)
            {
                Controller.AddObject(mapOnScreenObject);
            }
            GameGlobals.Map = _baseMap;
            GameGlobals.Player = GameGlobals.Map.GameObjects.OfType<Player>().FirstOrDefault();          
            EngineGlobals.Camera2D = _baseCamera;
            Controller.OnCameraMove();
        }
    }
}
