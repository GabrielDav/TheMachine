using System;
using Engine.Core;
using Engine.Mechanics;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
#if EDITOR
    public class 
        Camera2D : ICloneable
#else
    public class Camera2D
#endif
    {
        private Vector2 _posToMoveTo;
        private bool _transformRequired;
        protected Vector2 _backgroundOffset;
        protected Matrix _transform;
        protected Matrix _backgroundTransform;

        protected PhysicalObject _objectToFollow;
        protected float _zoom;
        protected Rectangle _bounds;
        protected float _rotation;
        protected bool _rotateRight;
        protected float _rotationSpeed;
        protected float _destinationRotation;
        protected Vector2 _position;
        protected bool _isZooming;
        protected bool _isRotating;
        protected bool _isMoving;
        protected bool _isFollowing;
        protected float _zoomSpeed;
        protected float _destinationZoom;
        protected float _currentSpeed;
        protected bool _zoomIn;

        protected Timer _pathWaitTimer;
        protected PathPoint[] _pathToFollow;
        protected int _pathIndex;
        protected bool _isFollowingPath;

        protected Vector2 _direction;

        public bool IsCamerBorderEnabled;

        public static int DefaultZoom = 1;
        public int BackgroundSpeedModifier = 8;
        public int FollowSpeedAmplifier = 5;
        public int DefaultSpeed = 300;
        public event SimpleEvent PathFinished;

        public Vector2 BackgroundOffset
        {
            get { return _backgroundOffset; }
            set
            {
                _backgroundOffset = value;
                _transformRequired = true;
            }
        }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < 0.1f)
                {
                    _zoom = 0.1f;
                }
                _transformRequired = true;
            }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
        }

        public float ViewPortHorizontal
        {
            get { return (EngineGlobals.Device.Viewport.Width/_zoom)/2f; }
        }

        public float ViewPortVertical
        {
            get { return (EngineGlobals.Device.Viewport.Height/_zoom)/2f; }                                                                             
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _transformRequired = true;
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value != _position)
                {
                    _position = value;

                    _transformRequired = true;
                    Controller.OnCameraMove();
                }
            }
        }

        public Rectangle CameraRectangle
        {
            get
            {
                return new Rectangle(
                    (int)(Position.X - ViewPortHorizontal),
                    (int)(Position.Y - ViewPortVertical),
                    (int)(EngineGlobals.Device.Viewport.Width / _zoom),
                    (int)(EngineGlobals.Device.Viewport.Height / _zoom));
            }
        }

        public Rectangle CameraBackgroundRectangle
        {
            get
            {
                var rect = CameraRectangle;
                return new Rectangle(
                    rect.X / BackgroundSpeedModifier,
                    rect.Y / BackgroundSpeedModifier,
                    (int) (EngineGlobals.Device.Viewport.Width/_zoom),
                    (int) (EngineGlobals.Device.Viewport.Height/_zoom));
            }
        }

        public Vector2 CornerPos
        {
            get
            {
                return new Vector2(Position.X - EngineGlobals.Device.Viewport.Width / 2f / _zoom, Position.Y - EngineGlobals.Device.Viewport.Height / 2f / _zoom);
            }
            set
            {
                Position = new Vector2(value.X + EngineGlobals.Device.Viewport.Width / 2f / _zoom, value.Y + EngineGlobals.Device.Viewport.Height / 2f / _zoom);
            }
        }

        public bool MoveX;

        public bool MoveY;

        public Camera2D()
        {
            Reset();
        }

        public void Reset()
        {
            Zoom = DefaultZoom;
            Rotation = 0.0f;
            Position = Vector2.Zero;
            _currentSpeed = DefaultSpeed;
            MoveX = true;
            MoveY = true;
            _objectToFollow = null;
           _bounds = new Rectangle();
            _isZooming = false;
            _isRotating = false;
            _isMoving = false;
            _isFollowing = false;
            _zoomSpeed = 0;
            _destinationZoom = 0;
            _currentSpeed = 0;
            _zoomIn = false;
        }

        private void StartMove(Vector2 position, float speed)
        {
            if (_isFollowingPath)
                return;
            _isMoving = true;
            _posToMoveTo = position;
            _currentSpeed = speed;
        }

        public void MoveTo(Vector2 position, float speed)
        {
            StopFollow();
            StartMove(position, speed);
        }

        public void MoveTo(Vector2 position)
        {
            StopFollow();
            _isMoving = true;
            _posToMoveTo = position;
        }

        public void SetCamerBounds(Rectangle rectangle)
        {
            IsCamerBorderEnabled = true;
            _bounds = rectangle;
        }

        public void FollowObject(PhysicalObject objectToFollow)
        {
            _isFollowing = true;
            _isMoving = false;
            _objectToFollow = objectToFollow;
            //_objectToFollow.OnPositionChanged += ObjectToFollowOnPositionChanged;
        }

        public void StopFollow()
        {
            //_objectToFollow.OnPositionChanged -= ObjectToFollowOnPositionChanged;
            _isFollowing = false;
            _objectToFollow = null;
        }

        protected void Follow()
        {
            var objectPos = _objectToFollow.Pos.ToVector();
            var distance = Vector2.Distance(objectPos, Position);

            if (distance > FollowSpeedAmplifier)
            {
                _direction = Vector2.Normalize(objectPos - _position);
                var delta = distance * FollowSpeedAmplifier * (float)EngineGlobals.GameTime.ElapsedGameTime.TotalSeconds;
                var pos = _position + _direction * delta;
                Position = AdjustToBounds(pos);
            }
        }

        private Vector2 AdjustToBounds(Vector2 pos)
        {
            if (!IsCamerBorderEnabled)
            {
                return pos;
            }

            if (pos.X - ViewPortHorizontal < _bounds.Left)
            {
                pos.X = _bounds.Left + ViewPortHorizontal;
            }
            else if (pos.X + ViewPortHorizontal > _bounds.Right)
            {
                pos.X = _bounds.Right - ViewPortHorizontal;
            }

            if (pos.Y - ViewPortVertical < _bounds.Top)
            {
                pos.Y = _bounds.Top + ViewPortVertical;
            }
            else if (pos.Y + ViewPortVertical > _bounds.Bottom)
            {
                pos.Y = _bounds.Bottom - ViewPortVertical;
            }

            _transformRequired = true;

            return pos;
        }

        protected void FollowPath()
        {
            if (_pathWaitTimer.Ticking)
            {
                _pathWaitTimer.Update();
                if (_pathWaitTimer.Finished)
                {
                    _pathIndex++;
                    if (_pathIndex >= _pathToFollow.Length)
                    {
                        _isFollowingPath = false;
                        if (PathFinished != null)
                            PathFinished(this);
                        return;
                    }
                    _currentSpeed = _pathToFollow[_pathIndex + 1].Speed;
                }
                else
                {
                    return;
                }
                
            }
            var nextPoint = _pathToFollow[_pathIndex + 1];
            var dist = Vector2.Distance(Position, nextPoint.ToVector());
            var speedDelta = _currentSpeed * (float)EngineGlobals.GameTime.ElapsedGameTime.TotalSeconds;
            if (dist < speedDelta)
            {
                if (nextPoint.WaitTime > 0f || _pathToFollow.Length - 1 <= _pathIndex + 1)
                {
                    Position = nextPoint.ToVector();
                    if (nextPoint.WaitTime > 0f)
                    {
                        _pathWaitTimer.Start(nextPoint.WaitTime, false);
                        return;
                    }
                    _isFollowingPath = false;
                    _isMoving = false;
                    if (PathFinished != null)
                        PathFinished(this);
                    return;
                }
                _pathIndex++;
                nextPoint = _pathToFollow[_pathIndex + 1];
                _currentSpeed = nextPoint.Speed;
            }
            _direction = Vector2.Normalize(nextPoint.ToVector() - _position);
            Position += _direction * speedDelta;
        }

        public void Update()
        {
            if (_isFollowingPath)
            {
                FollowPath();
            }
            else if (_isMoving)
            {
                _direction = Vector2.Normalize(_posToMoveTo - _position);
                var delta = _currentSpeed*(float) EngineGlobals.GameTime.ElapsedGameTime.TotalSeconds;
                var pos = _position + _direction*delta;

                Position = AdjustToBounds(pos);

                if (Vector2.Distance(Position, _posToMoveTo) <= delta)
                    // bugas - nedaeina iki galutinio tasko, jei atstumas mazesnis uz delta
                {
                    _isMoving = false;
                }
            }
            else if (_isFollowing)
            {
                Follow();
            }

            if (_isZooming)
            {
                _zoom -= _zoomSpeed*EngineGlobals.GameTime.ElapsedGameTime.Milliseconds;
                Position = AdjustToBounds(_position);
                if (_zoomIn)
                {
                    if (Zoom <= _destinationZoom)
                    {
                        Zoom = _destinationZoom;
                        _isZooming = false;
                    }
                }
                else
                {
                    if (Zoom >= _destinationZoom)
                    {
                        Zoom = _destinationZoom;
                        _isZooming = false;
                    }
                }
            }

            if (_isRotating)
            {
                var delta = _rotationSpeed*EngineGlobals.GameTime.ElapsedGameTime.Milliseconds;
                _transformRequired = true;
               if (_rotateRight)
                {
                    if (_rotation + delta >= _destinationRotation)
                    {
                        _rotation = _destinationRotation;
                        _isRotating = false;
                    }
                    else
                    {
                        _rotation += delta;
                    }
                }
                else
                {
                    if (_rotation - delta <= _destinationRotation)
                    {
                        _rotation = _destinationRotation;
                        _isRotating = false;
                    }
                    else
                    {
                        _rotation += delta;
                    }
                }
            }
        }

        public Matrix GetTransformation()
        {
            if (_transformRequired)
            {
                _transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0))*
                             Matrix.CreateRotationZ(_rotation)*
                             Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))*
                             Matrix.CreateTranslation(
                                 new Vector3(
                                     EngineGlobals.Device.Viewport.Width*0.5f,
                                     EngineGlobals.Device.Viewport.Height*0.5f, 0));
                _transformRequired = false;
            }
            return _transform;
        }

        public Matrix GetBackgroundTransformation()
        {
            if (_transformRequired)
            {
                _backgroundTransform = Matrix.CreateTranslation(new Vector3(-Position.X/BackgroundSpeedModifier - BackgroundOffset.X/_zoom, -Position.Y/BackgroundSpeedModifier - BackgroundOffset.Y/_zoom, 0))*
                                       Matrix.CreateRotationZ(_rotation)*
                                       Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))*
                                       Matrix.CreateTranslation(new Vector3(
                                                                    EngineGlobals.Device.Viewport.Width*0.5f,
                                                                    EngineGlobals.Device.Viewport.Height*0.5f, 0));
            }

            return _backgroundTransform;
        }

        /// <summary>
        /// Transform screen coordinates to world coordinates
        /// </summary>
        public Vector2 ViewToWorld(Vector2 pos)
        {
            return Vector2.Transform(pos, Matrix.Invert(_transform));
        }

        public Vector2 ViewToBackgroundWorld(Vector2 pos)
        {
            return Vector2.Transform(pos, Matrix.Invert(_backgroundTransform));
        }

        /// <summary>
        /// Stops camera, also stops following object
        /// </summary>
        public void Stop()
        {
            _isMoving = false;
            _objectToFollow = null;
        }

        public bool IsObjectVisible(Rectangle rectangle)
        {
            var cameraRectabgle = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                EngineGlobals.Graphics.GraphicsDevice.Viewport.Width,
                EngineGlobals.Graphics.GraphicsDevice.Viewport.Height);

            return cameraRectabgle.Intersects(rectangle);
        }

        public void SlowZoom(float zoom, int time)
        {
            if (time == 0)
            {
                time = 100;
            }
            _isZooming = true;
            _destinationZoom = zoom;
            _zoomIn = _destinationZoom < Zoom;
            var distance = _zoom - zoom;
            _zoomSpeed = distance/time;
        }

        public void Rotate(float destination, float speed, bool rotateRight)
        {
            _isRotating = true;
            _destinationRotation = MathExt.NormalizeRotation(destination);
            _rotation = MathExt.NormalizeRotation(_rotation);
            if (rotateRight && _destinationRotation < _rotation)
                _destinationRotation = MathHelper.TwoPi - _destinationRotation;
            else if (!rotateRight && _destinationRotation > _rotation)
                _destinationRotation = MathHelper.TwoPi - _destinationRotation;
            
            _rotationSpeed = speed;
            _rotateRight = rotateRight;
        }

        public void StopZoom()
        {
            _isZooming = false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void StartFollow(PathPoint[] path)
        {
            if (_pathWaitTimer == null)
                _pathWaitTimer = new Timer();
            _pathToFollow = path;
            _pathIndex = -1;
            _isFollowingPath = true;
            _isMoving = true;
            _currentSpeed = path[0].Speed;

        }
    } 
}