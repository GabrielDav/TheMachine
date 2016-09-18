
using System;
using Engine.Core;
using Microsoft.Xna.Framework;

namespace GameLibrary.Objects
{
    public class CircleSpikes : Circle
    {
        protected Timer _timer;
        protected bool _rotating;
        protected float _endRotation;
        protected float _currentRotation;

        public int StartTime { get; set; }
        public int IntervalTime { get; set; }
        public float IntervalRotation { get; set; }


        public CircleSpikes()
        {
            Animated = false;
            TypeId = GameObjectType.CircleSpikes.ToString();
            Init();
        }

        public override void RotateByGameTime()
        {
            if (!IsActivated)
                return;
            if (IntervalTime == 0)
            {
                base.Rotate();
                return;
            }
            if (_rotating)
            {
                if (_rotationDirection == Engine.Core.Direction.Clockwise)
                {
                    var rotation = (RotationSpeed / 1000 * EngineGlobals.GetElapsedInGameTime() *
                                      EngineGlobals.TimeSpeed);
                    
                    if (_currentRotation + rotation >= _endRotation)
                    {
                        rotation = _endRotation - _currentRotation;
                        _rotating = false;
                        _currentRotation = 0;
                        _timer.Start(IntervalTime, false);
                    }
                    else
                    {
                        _currentRotation += rotation;
                    }
                    Mask.Rotation += rotation;
                    if (Mask.Rotation >= MathHelper.Pi * 2f)
                    {
                        Mask.Rotation = Mask.Rotation - MathHelper.Pi * 2f;
                    }
                }
                else
                {
                    var rotation = (RotationSpeed / 1000 * EngineGlobals.GetElapsedInGameTime() *
                                      EngineGlobals.TimeSpeed);
                    
                    if (_currentRotation - rotation <= _endRotation)
                    {
                        rotation = _currentRotation - _endRotation;
                        _rotating = false;
                        _currentRotation = 0;
                        _timer.Start(IntervalTime, false);
                    }
                    else
                    {
                        _currentRotation -= rotation;
                    }
                    Mask.Rotation -= rotation;
                    if (Mask.Rotation <= -MathHelper.Pi * 2f)
                    {
                        Mask.Rotation = Mask.Rotation + MathHelper.Pi * 2f;
                    }
                }
            }
            if (!_timer.Started)
            {
                _timer.Start(StartTime, false);
                return;
            }
            _timer.Update();
            if (_timer.Finished)
            {
                _timer.Start(IntervalTime, false);
                if (_rotationDirection == Engine.Core.Direction.Clockwise)
                {
                    _endRotation = MathHelper.ToRadians(IntervalRotation);
                    _rotating = true;
                }
                else
                {
                    _endRotation = -MathHelper.ToRadians(IntervalRotation);
                    _rotating = true;
                }

            }

        }

        public override void Update()
        {
            RotateByGameTime();
        }

        public virtual bool CheckCollision(float targetRotation)
        {
            // Normalize angle
            targetRotation = MathHelper.PiOver2 + targetRotation;
            if (targetRotation < 0)
            {
                targetRotation = MathHelper.TwoPi + targetRotation;
            }

            var fixedStartRotation = Mask.Rotation < 0 ? MathHelper.TwoPi + Mask.Rotation : Mask.Rotation;
            return ((targetRotation >= fixedStartRotation && targetRotation - fixedStartRotation <= MathHelper.Pi) ||
                       (targetRotation <= fixedStartRotation && fixedStartRotation - targetRotation >= MathHelper.Pi));
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            _timer = new Timer(true);
            if (IsActivated)
                _timer.Start(StartTime, false);

        }


        #if EDITOR

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Diameter = 10;
            Mask.LayerDepth = 0.5f;
            IntervalRotation = 90;
            IntervalTime = 1000;
            StartTime = 1000;
            RotationSpeed = 2;
            IsActivated = true;
        }

        #endif
    }
}
