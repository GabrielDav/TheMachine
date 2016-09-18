using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Objects
{
    internal class Water : BoxPhysicalObject
    {
        protected MoveDirection _flowDirection;
        protected int _maxValue;
        protected bool _active;
        protected float _speed;
        protected int _deathDuration;
        protected float _value;
        protected bool _directionSwich;
        protected bool _step1Complete;
        protected bool _step2Complete;
        protected float _surfaceWidth;

        protected Image Surface;
        

        public Water()
        {
            Animated = false;
            TypeId = GameObjectType.Water.ToString();
            Init();
        }

#if EDITOR


        [PropertyOrder(20)]
        public MoveDirection FlowDirection
        {
            get { return _flowDirection; }
            set
            {
                FirePropertyChangingEvent("FlowDirection");
                _flowDirection = value;
                FirePropertyChangedEvent("FlowDirection");
            }
        }

        [PropertyOrder(21)]
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                FirePropertyChangingEvent("MaxValue");
                _maxValue = value;
                FirePropertyChangedEvent("MaxValue");
            }
        }

        [PropertyOrder(22)]
        public bool Active
        {
            get { return _active; }
            set
            {
                FirePropertyChangingEvent("Active");
                _active = value;
                FirePropertyChangedEvent("Active");
            }
        }

        [PropertyOrder(23)]
        public float Speed
        {
            get { return _speed; }
            set
            {
                FirePropertyChangingEvent("Speed");
                _speed = value;
                FirePropertyChangedEvent("Speed");
            }
        }

        [PropertyOrder(24)]
        public int DeathDuration
        {
            get { return _deathDuration; }
            set
            {
                FirePropertyChangingEvent("DeathDuration");
                _deathDuration = value;
                FirePropertyChangedEvent("DeathDuration");
            }
        }

        [PropertyOrder(25)]
        public int SurfaceWidth { get; set; }

#else
        public MoveDirection FlowDirection
        {
            get { return _flowDirection; }
            set { _flowDirection = value; }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        public int DeathDuration
        {
            get { return _deathDuration; }
            set { _deathDuration = value; }
        }

        public int SurfaceWidth { get; set; }
#endif

        //private void UpdateStep1()
        //{
        //    if (FlowDirection == Direction.Up)
        //    {
        //        var width = Rectangle.Width;
        //        var speed = width / (SurfaceWidth/Speed); //kiek pikseliu per sekunde turetu pakilti
        //        _surfaceWidth += EngineGlobals.ElapsedTime*speed;

        //        if (_directionSwich)
        //        {
        //            Surface.Rect = new Rectangle((int) (Rectangle.X + (Rectangle.Width - _surfaceWidth)), Pos.Y - SurfaceWidth, (int) _surfaceWidth, SurfaceWidth);
        //        }
        //        else
        //        {
        //            Surface.Rect = new Rectangle((int)Rectangle.X, Pos.Y - SurfaceWidth, (int) _surfaceWidth, SurfaceWidth);
        //        }

        //        if (_surfaceWidth > Rectangle.Width)
        //        {
        //            _step1Complete = true;
        //            _directionSwich = !_directionSwich;
        //            _surfaceWidth = 0;
        //        }
        //    }
        //}

        public override void Update()
        {
            base.Update();

            if (_active)
            {
               // _surfaceWidth = Speed*EngineGlobals.ElapsedTime;
                switch (FlowDirection)
                {
                    case MoveDirection.Up:
                        //if (!_step1Complete)
                        //{
                        //    UpdateStep1();
                        //}
                        //else
                        //{
                            _step1Complete = false;
                           // SetRectangle(Rectangle.X, Rectangle.Y - SurfaceWidth, Rectangle.Width, Rectangle.Height + SurfaceWidth);
                            SetRectangle(Rectangle.X, Rectangle.Y - _surfaceWidth, Rectangle.Width, Rectangle.Height + _surfaceWidth);
                      //  }
                        break;
                }
                if (Rectangle.Intersects(GameGlobals.Player.Rectangle))
                {
                    GameGlobals.GameOver = true;
                }

                if (FlowDirection == MoveDirection.Up || FlowDirection == MoveDirection.Down)
                {
                    if (Mask.Height >= MaxValue)
                    {
                        _active = false;
                    }
                }
                else
                {
                    if (Mask.Width >= MaxValue)
                    {
                        _active = false;
                    }
                }

                if (Mask.Y < GameGlobals.Player.HalfPos.Y)
                {
                    GameGlobals.GameOver = true;
                }
            }
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Mask.LayerDepth = 0.2f;
            _flowDirection = MoveDirection.Up;
            Speed = 20;
            MaxValue = 1000;
            IgnoreCollision = true;
            SetRectangle(0, 0, 100, 30);
            SurfaceWidth = 5;
        }
#endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            Surface = new Image(EngineGlobals.Resources.Textures[resourceId][index]) {LayerDepth = 0.2f};
            Controller.AddObject(Surface);
        }

    }
}
