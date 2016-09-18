using System;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Objects
{
    public class MovingCircle : Circle
    {
        protected MoveDirection _moveDirection;
        protected Vector2 _startPos;
        protected bool _movingForward;
        protected bool _moving;
        protected float _distance;
        protected bool _paused;
        protected Timer _timer;
        protected Image _pathStartPoint;
        protected Image _pathEndPoint;
        protected Image _path;
        protected bool _drawPath;
        protected Vector2 _endPos;
        protected bool _originalLooping;

        [ContentSerializerIgnore] public Vector2 CurrentDirection;

        public Vector2 EndPos
        {
            get { return _endPos; }
            set
            {
                _endPos = value;
                if (_pathStartPoint != null)
                {
                    CalculatePath();
                }
            }
        }

        public bool Looping { get; set; }
        public bool Static { get; set; }
        public int Speed { get; set; }
        public int DelayAtStart { get; set; }
        public int DelayAtEnd { get; set; }

        

#if EDITOR
        [PropertyOrder(20)]
#endif
            public MoveDirection MoveDirection
        {
            get { return _moveDirection; }
            set
            {
#if EDITOR
                FirePropertyChangingEvent("MoveDirection");
#endif

                _moveDirection = value;

#if EDITOR
                FirePropertyChangedEvent("MoveDirection");
#endif
            }
        }

#if EDITOR
        // [ContentSerializerAttribute(Optional = true)]
        public override Vector2 EditorDestination
        {
            get { return EndPos; }
            set { EndPos = value; }
        }

#endif

        public override bool IsActivated
        {
            get { return base.IsActivated; }
            set
            {
                base.IsActivated = value;
                GlobalUpdate = value;
            }
        }

        public MovingCircle()
        {
            TypeId = GameObjectType.MovingCircle.ToString();
        }

        public override void Update()
        {
            base.Update();
            if (IsActivated)
            {
                if (!Static)
                {
                    Move();
                }
            }
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            _timer = new Timer(true);
            _startPos = HalfPos;
            if (Static)
            {
                _distance = 0;
                CurrentDirection = Vector2.Zero;
                _movingForward = false;

            }
            else
            {
                _distance = 0;
                _distance = Vector2.Distance(_startPos, EndPos);
                CurrentDirection = Vector2.Normalize(EndPos - _startPos);
                _movingForward = true;
            }
            _moving = true;
            _paused = false;
            if (!GameGlobals.ArcadeMode)
            {
                _pathStartPoint = new Image(EngineGlobals.Resources.Textures["Path"][1])
                {
                    LayerDepth = 0.81f,
                    Owner = this,
                    IgnoreCulling = true
                };
                _pathStartPoint.Orgin = _pathStartPoint.OriginCenter();
                _path = new Image(EngineGlobals.Resources.Textures["Path"][0])
                {
                    LayerDepth = 0.81f,
                    Owner = this,
                    IgnoreCulling = true
                };
                _path.Orgin = _path.OriginCenter();
                _pathEndPoint = new Image(EngineGlobals.Resources.Textures["Path"][1])
                {
                    LayerDepth = 0.81f,
                    Owner = this,
                    IgnoreCulling = true
                };
                _pathEndPoint.Orgin = _pathEndPoint.OriginCenter();
                Controller.AddObject(_path);
                Controller.AddObject(_pathEndPoint);
                Controller.AddObject(_pathStartPoint);
                CalculatePath();
                if (DelayAtStart > 0)
                {
                    _paused = true;
                    _timer.Start(DelayAtStart, false);
                }
            }
            _originalLooping = Looping;
        }

        protected virtual void Move()
        {
            if (_paused)
            {
                _timer.Update();
                if (_timer.Finished)
                {
                    _paused = false;
                    _moving = true;
                    _paused = false;
                }
            }
            else
            {
                if (_moving)
                {
                    var delta = EngineGlobals.GetElapsedInGameTime() / 1000 * Speed;

                    HalfPos += CurrentDirection * delta;

                    if (_movingForward)
                    {
                        if (Vector2.Distance(HalfPos, _startPos) >= _distance)
                        {
                            HalfPos = EndPos;
                            _movingForward = false;
                            CurrentDirection = -CurrentDirection;
                            if (!Looping)
                            {
                                Static = true;
                                _moving = false;
                            }
                            else if (DelayAtEnd > 0)
                            {
                                _timer.Start(DelayAtEnd, false);
                                _paused = true;
                                _moving = false;
                            }
                        }
                    }
                    else
                    {
                        if (Vector2.Distance(HalfPos, EndPos) >= _distance)
                        {
                            HalfPos = _startPos;
                            _movingForward = true;
                            CurrentDirection = -CurrentDirection;
                            if (!Looping)
                            {
                                Static = true;
                                _moving = false;
                            }
                            else if (DelayAtStart > 0)
                            {
                                _timer.Start(DelayAtStart, false);
                                _paused = true;
                                _moving = false;
                            }
                        }
                    }
                }
            }
        }

        public virtual void MoveToStartAndStop()
        {
            if (!_movingForward)
            {
                Static = false;
                _moving = true;
            }
            else
            {
                _movingForward = false;
                CurrentDirection = -CurrentDirection;
            }
            Looping = false;
        }

        public virtual void RestartMoving()
        {
            if (!IsActivated)
            {
                IsActivated = true;
                return;
            }
            Looping = _originalLooping;
            if (!_moving)
            {
                Static = false;
                _moving = true;
            }
            else
            {
                CurrentDirection = -CurrentDirection;
                _movingForward = !_movingForward;
            }
        }

        protected virtual void CalculatePath()
        {
            if (GameGlobals.ArcadeMode)
                return;
            var dist = Vector2.Distance(HalfPos, EndPos);
            if (dist < EngineGlobals.Epsilon || EndPos == Vector2.Zero)
            {
                _drawPath = false;
                return;
            }
            _drawPath = true;
            var lineVector = EndPos - HalfPos;
            lineVector.Normalize();
            var rotation = (float) Math.Atan2(lineVector.Y, lineVector.X);
            var endPointVector = new Vector2(200, 12)*lineVector;
            _pathStartPoint.Rect = new Rectangle((int)HalfPos.X, (int)HalfPos.Y , 12, 12);
            _pathStartPoint.Flip = SpriteEffects.FlipHorizontally;
            _pathEndPoint.Rect = new Rectangle((int)EndPos.X, (int)EndPos.Y, 12, 12);
            _pathEndPoint.Flip = SpriteEffects.FlipHorizontally;
            _pathStartPoint.Rotation = rotation - MathHelper.Pi;
            _pathEndPoint.Rotation = rotation;
            var moveVectorStart = HalfPos + endPointVector;
            var moveVectorEnd = EndPos - endPointVector;
            var moveVector = moveVectorStart + ((moveVectorEnd - moveVectorStart) / 2);
            var originValue = _path.Texture.Width/dist*_pathStartPoint.Width;
           // _path.Orgin = new Vector2(-originValue, _path.Texture.Height/2f);
             _path.Rotation = (float)Math.Atan2(EndPos.Y - HalfPos.Y, EndPos.X - HalfPos.X);
             _path.Rect = new Rectangle((int)moveVector.X, (int)moveVector.Y, (int)dist- 10, 12);


        }


        //public override void Draw()
        //{
        //    base.Draw();
        //    if (_drawPath)
        //    {
        //        _pathStartPoint.Draw();
        //        _pathEndPoint.Draw();
        //        _path.Draw();
        //    }
        //}

#if EDITOR

        public override void EditorDeleteObject()
        {
            base.EditorDeleteObject();
            Controller.RemoveObject(_path);
            Controller.RemoveObject(_pathEndPoint);
            Controller.RemoveObject(_pathStartPoint);
        }

        public override void HideDetailImages(bool hide)
        {
            _path.IsHidden = hide;
            _pathEndPoint.IsHidden = hide;
            _pathStartPoint.IsHidden = hide;
        }

        public override bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return flag != DebugFlag.Circle || flag == DebugFlag.Destination;
        }

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            Looping = true;
            Static = true;
        }

        public override void UpdatePreviewRectangle(Rectangle rectangle)
        {
            base.UpdatePreviewRectangle(rectangle);
            CalculatePath();
        }

#endif

    }

}
