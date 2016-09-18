using System;
using System.Collections.Generic;
using System.ComponentModel;
#if EDITOR
using System.Drawing;
#endif
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;
using Image = Engine.Graphics.Image;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GameLibrary.Objects
{
    public class SpikeShooter : BoxPhysicalObject
    {
        //protected enum State { Charging, Recharging, Pending }
        //protected Timer _timer;
       // protected Timer _rechargeTimer;
        protected List<SpikeBullet> _bullets;
        protected Image _timerImage;
        protected long _shotInterval;
        protected long _rechargeTime;
        protected bool _isCharging;
        protected float _endRotation;
        protected float _startRotation;
        protected RotateEffect _rotateEffect;
        ////private int _horyzontalSign = 1;
        ////private int _verticalSign = -1;
        ////private int _imgRotation = 180;

        //protected RotateEffect _rotateTimerEffect;

#if EDITOR
        [Browsable(false)]
#endif
        public int CurrentBulletCount { get; set; }

        public long ShotInterval
        {
            get { return _shotInterval; }
            set { if (value < _rechargeTime)
                throw new Exception("Shot interval cannot be shorter than recharge time");
                _shotInterval = value;
            }
        }

        public long RechargeTime
        {
            get { return _rechargeTime; }
            set
            {
                if (value > _shotInterval)
                    throw new Exception("Recharge time cannot be longer than shot interval");
                _rechargeTime = value;
            }
        }

        public int BulletSpeed { get; set; }

        public const int MaxBulletCount = 20;

        public Vector2 TargetPos { get; set; }

        [ReadOnly(true)]
        public override SpriteEffects Flip
        {
            get
            {
                return base.Flip;
            }
            set
            {
                base.Flip = value;
            }
        }

        public override Orientation Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                base.Orientation = value;
                UpdateOrientation();
            }
        }

        public override bool IsActivated { get { return base.IsActivated; } set { base.IsActivated = value; GlobalUpdate = value; } }

        //private Image _timeImg;

        #if EDITOR

        public override Vector2 EditorDestination
        {
            get
            {
                return TargetPos;
            }
            set
            {
                TargetPos = value;
            }
        }

        #endif

        public SpikeShooter()
        {
            Animated = false;
            Init();
            TypeId = GameObjectType.SpikeShooter.ToString();
        }

        public override void Load(string resourceId, int index)
        {
            
            //_timer = new Timer(true);
            base.Load(resourceId, index);
            //_timer.Start(ShotInterval, false);
            _bullets = new List<SpikeBullet>();
            PhysicalObject objectAttachedTo = null;

            foreach (var mapObject in GameGlobals.Map.GameObjects)
            {
                if (Rectangle.GetRectangle().Intersects(mapObject.Rectangle.GetRectangle()))
                {
                    objectAttachedTo = mapObject;
                }
            }

            for (int i = 0; i < MaxBulletCount; i++)
            {
                var newBullet = new SpikeBullet(this);
                newBullet.Load("SpikeShooterParts", 0);
                newBullet.InitBullet(HalfPos, TargetPos, BulletSpeed, objectAttachedTo);
                Controller.AddGameObject(newBullet);
                _bullets.Add(newBullet);
            }
            //var timerImgX = (int)HalfPos.X;
            //var timerImgY = (int)HalfPos.Y;
            _isCharging = false;
            //_rechargeTimer = new Timer(true);
           
            _timerImage = new Image(EngineGlobals.Resources.Textures["SpikeShooterParts"][1]){LayerDepth = LayerDepth + 0.1f};
            _timerImage.Orgin = _timerImage.OriginCenter();
            _timerImage.Owner = this;
            Controller.AddObject(_timerImage);
            UpdateOrientation();
            /*if (Orientation == Orientation.Right)
            {
                timerImgX = (int)HalfPos.X + 9;
                timerImgY = (int)HalfPos.Y + 14;
                _imgRotation = 90;
            }*/
            

            /*if (Flip == SpriteEffects.FlipHorizontally)
            {
                _horyzontalSign = -1;
                _imgRotation = -_imgRotation;
                timerImgX -= 13;
                timerImgY += 8;
            }*/
            
            //_timeImg = new Image(EngineGlobals.Resources.Textures["SpikeShooterParts"][1],
            //                     _rectangle.GetRectangle())
            //    {
            //        LayerDepth = Mask.LayerDepth - 0.01f,
            //        Owner = this,
            //        Scale = new Vector2(0.6f, 0.6f),
            //        Rect = new Rectangle(
            //            timerImgX,
            //            timerImgY,
            //            (int) Rectangle.Width,
            //            (int) Rectangle.Height)
            //    };

            //_timeImg.Orgin = _timeImg.OriginCenter();
            
            //Controller.AddObject(_timeImg);

            //_rotateTimerEffect = new RotateEffect(
            //    _timeImg,
            //    MathHelper.ToRadians(/*-_horyzontalSign*180*/_imgRotation),
            //    (int) ShotInterval);
        }

        protected override void SetRectangle(float x, float y, float width, float height)
        {
            base.SetRectangle(x, y, width, height);
#if EDITOR
            UpdatePreviewRectangle(Rectangle.GetRectangle());
#endif
            
        }
        
        public void Shoot()
        {
            CurrentBulletCount++;

            foreach (var bullet in _bullets)
            {
                if (!bullet.InFlight)
                {
                    switch (Orientation)
                    {
                            case Orientation.Top:
                            bullet.Rotation = 0;
                            break;
                            case Orientation.Bottom:
                            bullet.Rotation = 180;
                            break;
                            case Orientation.Right:
                            bullet.Rotation = 90;
                            break;
                            case Orientation.Left:
                            bullet.Rotation = 270;
                            break;
                    }
                    bullet.Activate();
                    EngineGlobals.SoundManager.Play("spikeShooter_shot", "shot", this, 1f, false, 1f, 1f);
                    return;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (IsActivated)
            {
                _rotateEffect.Update();
                if (_rotateEffect.Finished)
                {
                    if (!_isCharging)
                    {
                        Shoot();
                        _rotateEffect.Reset(_startRotation, (int) RechargeTime);
                        _isCharging = true;
                    }
                    else
                    {
                        _rotateEffect.Reset(_endRotation, (int)(ShotInterval - RechargeTime));
                        _isCharging = false;
                    }
                }
                //if (!_isCharging)
                //{
                //    _timer.Update();

                //    if (_timer.Finished && (CurrentBulletCount < MaxBulletCount))
                //    {
                //        Shoot();
                //        _timer.Restart();
                //        //_rotateTimerEffect.Restart(MathHelper.ToRadians(0), 500);
                //    }
                //}
                //else
                //{
                //    _timer.Update();
                //    if (_timer.Finished)
                //    {
                //        _timerImage.Rotation = _startRotation;
                //        _timer.Start(_shotInterval - _rechargeTime, false);
                //    }
                //}
            }
        }

        protected void UpdateOrientation()
        {
            if (_timerImage == null)
                return;
            switch (Orientation)
            {
                case Orientation.Top:
                    _startRotation = MathHelper.PiOver2;
                    _endRotation = 0;
                    _timerImage.Rotation = _startRotation;
                    _timerImage.Rect = new Rectangle((int)(Rectangle.X + Rectangle.Width - 45), (int)Rectangle.Y + 45, 88, 88);
                    Flip = SpriteEffects.None;
                    
                    break;
                case Orientation.Bottom:
                    _startRotation = MathHelper.PiOver2;
                    _endRotation = MathHelper.Pi;
                    _timerImage.Rotation = _startRotation;
                    _timerImage.Rect = new Rectangle((int)(Rectangle.X + 45), (int)Rectangle.Y + 45, 88, 88);
                    Flip = SpriteEffects.FlipVertically;
                    break;
                case Orientation.Left:
                    _startRotation = -MathHelper.Pi;
                    _endRotation = -MathHelper.PiOver2;
                    _timerImage.Rotation = _startRotation;
                    _timerImage.Rect = new Rectangle((int)(Rectangle.X + 45), (int)Rectangle.Y + 45, 88, 88);
                    Flip = SpriteEffects.FlipVertically;
                    break;
                case Orientation.Right:
                    _startRotation = MathHelper.Pi;
                    _endRotation = MathHelper.PiOver2;
                    _timerImage.Rotation = _startRotation;
                    _timerImage.Rect = new Rectangle((int)(Rectangle.X + 45), (int)(Rectangle.Y + Rectangle.Height  - 45), 88, 88);
                    Flip = SpriteEffects.None;
                    break;
                
            }
            if (!GameGlobals.EditorMode)
                _rotateEffect = new RotateEffect(_timerImage, _endRotation, (int)ShotInterval);
        }

        public void Diactivate()
        {
            foreach (var bullet in _bullets)
            {
                bullet.RemoveBullet();
            }

            InUse = false;
        }


#if EDITOR

        public override void UpdatePreviewRectangle(Rectangle rectangle)
        {
            base.UpdatePreviewRectangle(rectangle);
            if (_timerImage == null)
                return;
            UpdateOrientation();
        }

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            GridSize = new Size(11, 9);
            Mask.LayerDepth = 0.45f;
            BulletSpeed = 15;
            ShotInterval = 2000;
            IsActivated = true;
        }

        public override bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return flag == DebugFlag.Destination;
        }

        public override void EditorDeleteObject()
        {
            base.EditorDeleteObject();
            Controller.RemoveObject(_timerImage);
        }

        #endif
    }
}
