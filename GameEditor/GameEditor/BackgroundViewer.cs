using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary;
using Microsoft.Xna.Framework;
using TheGoo;
using Timer = System.Windows.Forms.Timer;

namespace GameEditor
{
    class BackgroundViewer : IEditorGameEmulator
    {
        protected TimeSpan _totalTime;
        protected DateTime _time;
        protected readonly Timer _timer;
        public readonly Map BaseMap;
        public Map CurrentMap;

        public
            Tuple
                <List<IGraphicsObject>, List<IGraphicsObject>, List<PhysicalObject>, List<PhysicalObject>, List<IEffect>
                    , List<IDynamic>> ObjectsBuffers;

        public BackgroundViewer(Map map)
        {
            BaseMap = map;
            CurrentMap = (Map)BaseMap.Clone();
            _timer = new Timer { Interval = 16 };
            _timer.Tick += TimerOnTick;
            GameGlobals.Map = CurrentMap;
            GameGlobals.GameOver = false;
            GameGlobals.LevelComplete = false;
            ObjectsBuffers = Controller.GetObjectsBuffers();
            Controller.ClearObjectsBuffers();
            GameGlobals.Map = CurrentMap;
            foreach (var physicalObject in CurrentMap.BackgroundObjects)
            {
                Controller.AddBackgroundObject(physicalObject);
            }
            
            _timer.Start();
            _time = DateTime.Now;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (GameGlobals.GameOver)
            {
                Form1.Instance.StopGame(this, null);
                return;
            }
            var elapsedTIme = DateTime.Now - _time;
            _totalTime = _totalTime.Add(elapsedTIme);
            var gameTime = new GameTime(_totalTime, elapsedTIme);
            EngineGlobals.GameTime = gameTime;
            foreach (var backgroundObject in CurrentMap.BackgroundObjects)
            {
                backgroundObject.Update();
            }
            EngineGlobals.Camera2D.Update();
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

        public void Draw()
        {
            Controller.Draw();
        }

        public void InputOnPress(Point p, int id)
        {
        }

        public void Dispose()
        {
            _timer.Stop();
            Controller.ClearObjectsBuffers();
            foreach (var mapBackgourndObject in ObjectsBuffers.Item3)
            {
                Controller.AddBackgroundObject(mapBackgourndObject);
            }
            foreach (var mapForegroundObject in ObjectsBuffers.Item1)
            {
                Controller.AddObject(mapForegroundObject);
            }
            foreach (var mapOnScreenObject in ObjectsBuffers.Item2)
            {
                Controller.AddObject(mapOnScreenObject);
            }
            GameGlobals.Map = BaseMap;
            CurrentMap.Dispose();
        }
    }
}
