using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Mechanics;
using Microsoft.Xna.Framework;

namespace Engine.Graphics
{
    public interface IEffect
    {
        void Update();
        bool Finished { get; }
        GameObject Target { get; }
        event SimpleEvent OnFinish;
    }

    public class MoveEffect : IEffect
    {
        protected Vector2 _destination;
        public bool Finished { get; protected set; }

        public GameObject Target { get; protected set; }

        public event SimpleEvent OnFinish;
        protected Vector2 _moveVector;
        protected Vector2 _posVector;
        protected int _interval;

        public MoveEffect(GameObject gameObject, Vector2 destination, int interval)
        {
            Target = gameObject;
            Reset(Target.Pos, destination, interval);
        }

        public void Reset(Vector2 posVector, Vector2 destination, int interval)
        {
            _posVector = posVector;
            _destination = destination;
            _interval = interval;
            _moveVector = _destination - _posVector;
            Finished = false;
        }

        public void Update()
        {
            if (Finished)
                return;
            var changeVector = _moveVector* EngineGlobals.GetElapsedInGameTime()/_interval;
            if (Vector2.Distance(_posVector, _destination) <= changeVector.Length())
            {
                Target.Pos = _destination;
                Finished = true;
                if (OnFinish != null)
                    OnFinish(this);
            }
            else
            {
                _posVector += changeVector;
                Target.Pos = _posVector;
            }
        }

    }

    public class MoveEffectP
    {
        protected Vector2 _destination;
        public bool Finished { get; protected set; }

        public PhysicalObject Target { get; protected set; }

        public event SimpleEvent OnFinish;
        protected Vector2 _moveVector;
        protected Vector2 _posVector;
        protected int _interval;

        public MoveEffectP(PhysicalObject physicalObject, Vector2 destination, int interval)
        {
            Target = physicalObject;
            Reset(physicalObject.Rectangle.GetPos(), destination, interval);
        }

        public void Reset(Vector2 posVector, Vector2 destination, int interval)
        {
            _posVector = posVector;
            _destination = destination;
            _interval = interval;
            _moveVector = _destination - _posVector;
            Finished = false;
        }

        public void Update()
        {
            if (Finished)
                return;
            var changeVector = _moveVector * EngineGlobals.GetElapsedInGameTime() / _interval;
            if (Vector2.Distance(_posVector, _destination) <= changeVector.Length())
            {
                Target.Rectangle = new RectangleF(_destination.X, _destination.Y, Target.Rectangle.Width, Target.Rectangle.Height);
                Finished = true;
                if (OnFinish != null)
                    OnFinish(this);
            }
            else
            {
                _posVector += changeVector;
                Target.Rectangle = new RectangleF(_posVector.X, _posVector.Y, Target.Rectangle.Width, Target.Rectangle.Height);
            }
        }
    }

    public class ResizeEffect : IEffect
    {
        protected Vector2 _endSize;
        public bool Finished { get; protected set; }

        public GameObject Target { get; protected set; }

        public event SimpleEvent OnFinish;
        protected Vector2 _resizeVector;
        protected Vector2 _sizeVector;
        protected int _interval;

        public ResizeEffect(GameObject gameObject, Vector2 endSize, int interval)
        {
            Target = gameObject;
            Reset(endSize, interval);
        }

        public void Reset(Vector2 endSize, int interval)
        {
            _sizeVector = new Vector2(Target.Width, Target.Height);
            _endSize = endSize;
            _interval = interval;
            _resizeVector = _endSize - _sizeVector;
            Finished = false;
        }

        public void Update()
        {
            if (Finished)
                return;
            var changeVector = _resizeVector * EngineGlobals.GetElapsedInGameTime() / _interval;
            if (Vector2.Distance(_sizeVector, _endSize) <= changeVector.Length())
            {
                Target.Width = (int)_endSize.X;
                Target.Height = (int) _endSize.Y;
                Finished = true;
                if (OnFinish != null)
                    OnFinish(this);
            }
            else
            {
                _sizeVector += changeVector;
                Target.Width = (int)_sizeVector.X;
                Target.Height = (int) _sizeVector.Y;
            }
        }

    }

    public class ResizeEffectP
    {
        protected Vector2 _endSize;
        public bool Finished { get; protected set; }

        public PhysicalObject Target { get; protected set; }

        public event SimpleEvent OnFinish;
        protected Vector2 _resizeVector;
        protected Vector2 _sizeVector;
        protected int _interval;

        public ResizeEffectP(PhysicalObject physicalObject, Vector2 endSize, int interval)
        {
            //_destination = destination;
            Target = physicalObject;
            //_posVector = Target.Pos;
            //_interval = interval;
            //_moveVector = _destination - Target.Pos;
            Reset(endSize, interval);
        }

        public void Reset(Vector2 endSize, int interval)
        {
            _sizeVector = new Vector2(Target.Rectangle.Width, Target.Rectangle.Height);
            _endSize = endSize;
            _interval = interval;
            _resizeVector = _endSize - _sizeVector;
            Finished = false;
        }

        public void Update()
        {
            if (Finished)
                return;
            var changeVector = _resizeVector * (EngineGlobals.GetElapsedInGameTime() / (float)_interval);
            _sizeVector += changeVector;
            if (Vector2.Distance(_sizeVector, _endSize) <= changeVector.Length())
            {
                //Target.Rectangle = new RectangleF(Target.Rectangle.X, Target.Rectangle.Y, _endSize.X, _endSize.Y);
                Target.SetSizeUnsafe(_endSize.X, _endSize.Y);
                Finished = true;
                if (OnFinish != null)
                    OnFinish(this);
            }
            else
            {
                Target.SetSizeUnsafe(_sizeVector.X, _sizeVector.Y);
                //Target.HalfSize = new Vector2(_sizeVector.X, _sizeVector.Y);
                //Target.Rectangle = new RectangleF(Target.Rectangle.X, Target.Rectangle.Y, _sizeVector.X, _sizeVector.Y);

            }
        }

    }

    public class ScaleEffect : IEffect
    {
        protected Vector2 _endScale;
        public bool Finished { get; protected set; }

        public GameObject Target { get; protected set; }

        public event SimpleEvent OnFinish;
        protected Vector2 _rescaleVector;
        protected Vector2 _scaleVector;
        protected Vector2 _textSize;
        protected Rectangle _startRect;
        protected int _interval;

        public ScaleEffect(GameObject gameObject, Vector2 endSize, int interval)
        {
            Target = gameObject;
            var rect = gameObject.Rect;
            
            Reset(endSize, interval, rect);
        }

        public void Reset(Vector2 endScale, int interval, Rectangle startRect)
        {
            _scaleVector = Target.Scale;
            _endScale = endScale;
            _interval = interval;
            _rescaleVector = _endScale - _scaleVector;
            _startRect = startRect;
            _textSize = ((TextRegion)Target).GetSize();
            Finished = false;
        }

        public void Update()
        {
            if (Finished)
                return;
            var changeVector = _rescaleVector * EngineGlobals.GetElapsedInGameTime() / _interval;
            if (Vector2.Distance(_scaleVector, _endScale) <= changeVector.Length())
            {
                Target.Scale = _endScale;
                Finished = true;
                if (OnFinish != null)
                    OnFinish(this);
            }
            else
            {
                _scaleVector += changeVector;
                Target.Scale = _scaleVector;
                Target.Rect = new Rectangle((int)(_startRect.X + (_startRect.Width / 2f) - (_textSize.X * _scaleVector.X / 2f)),
                    (int)(_startRect.Y + (_startRect.Height / 2f) - (_textSize.Y * _scaleVector.Y / 2f)), (int)(_startRect.Width * _scaleVector.X), (int)(_startRect.Height * _scaleVector.Y));
            }
        }

    }

    public class ColorEffect : IEffect
    {
        protected Vector4 _updateVector;
        protected Vector4 _currentColorVector;
        protected Color _resultColor;
        protected int _duration;
        protected Color _startColor;
       
 
        public bool Finished { get; protected set; }

        public GameObject Target { get; protected set; }

        public event SimpleEvent OnFinish;

        public ColorEffect(GameObject gameObject, Color resultColor, int duration)
        {
            Target = gameObject;
            _startColor = Target.Color;
            Reset(resultColor, duration);
        }

        public void Reset(Color resultColor, int duration)
        {
            Finished = false;
            Target.Color = _startColor;
            _resultColor = resultColor;
            _duration = duration;
            _updateVector = _resultColor.ToRgbaVector() - Target.Color.ToRgbaVector();
            _currentColorVector = Target.Color.ToRgbaVector();
        }

        public void Update()
        {
            if (Finished)
                return;
            var changeVector = _updateVector * EngineGlobals.GetElapsedInGameTime() / _duration;

            
            if (Vector4.Distance(_currentColorVector, _resultColor.ToRgbaVector()) <= changeVector.Length())
            {
                Target.Color = _resultColor;
                Finished = true;
                if (OnFinish != null)
                    OnFinish(this);
            }
            else
            {
                _currentColorVector += changeVector;
                Target.Color = _currentColorVector.ToColor();

            }
        }
    }

    public class RotateEffect : IEffect
    {
        protected float _updateValue;
        protected float _currentValue;
        protected float _resultValue;
        protected int _duration;
        protected float _startValue;
        protected float _initialValue;
        protected int _initailDuration;
        protected bool _reset;

        public bool Finished { get; protected set; }

        public GameObject Target { get; protected set; }

        public event SimpleEvent OnFinish;

        public RotateEffect(GameObject gameObject, float resultRotation, int duration)
        {
            Target = gameObject;
            _startValue = Target.Rotation;
            _initailDuration = duration;
            _initialValue = resultRotation;
            Reset(resultRotation, duration);
        }

        public void Reset(float resultRotation, int duration)
        {
            Finished = false;
            _resultValue = resultRotation;
            _duration = duration;
            _updateValue = _resultValue - Target.Rotation;
            _currentValue = Target.Rotation;
        }

        public void Restart(float resultRotation, int duration)
        {
            Reset(resultRotation, duration);
            _reset = true;
        }

        public void Update()
        {
            if (Finished && _reset)
            {
                _reset = false;
                Reset(_initialValue, _initailDuration);
            }

            if (Finished)
                return;
            var changeValue = _updateValue * EngineGlobals.GetElapsedInGameTime() / _duration;


            if (Math.Abs(_currentValue - _resultValue) <= Math.Abs(changeValue))
            {
                Target.Rotation = _resultValue;
                Finished = true;
                if (OnFinish != null)
                    OnFinish(this);
            }
            else
            {
                _currentValue += changeValue;
                Target.Rotation = _currentValue;

            }
        }
    }

    public class ApplyEffect : IEffect
    {
        protected Timer _timer;
        protected IEffect _effect;

        public ApplyEffect(IEffect effect, int waitTime)
        {
            _effect = effect;
            _timer = new Timer();
            _timer.Start(waitTime, false);
        }

        public void Update()
        {
            if (Finished)
                return;
            _timer.Update();
            if (!_timer.Finished)
                return;
            Controller.AddEffectQueue(_effect);
            Finished = true;
            if (OnFinish != null)
                OnFinish(this);
        }

        public bool Finished { get; private set; }
        public GameObject Target { get; private set; }
        public event SimpleEvent OnFinish;
    }

    public class DestroyObjectEffect : IEffect
    {
        protected Timer _timer;

        public bool Finished { get; protected set; }
        public GameObject Target { get; protected set; }
        public event SimpleEvent OnFinish;

        public DestroyObjectEffect(GameObject target, long interval)
        {
            Target = target;
            _timer = new Timer();
            _timer.Start(interval, false);
        }

        public void Update()
        {
            if (Finished)
                return;
            _timer.Update();
            if (!_timer.Finished) return;
            Controller.RemoveObject(Target);
            Target.Dispose();
            Finished = true;
            if (OnFinish != null)
                OnFinish(this);
        }
    }

    public class ApplyForceVectorEffect : IEffect
    {
        public bool Finished { get; protected set; }
        public GameObject Target { get; protected set; }
        public event SimpleEvent OnFinish;

        protected Vector2 _currentVector;
        protected Vector2[] _forceVectors;

        public ApplyForceVectorEffect(GameObject gameObject, Vector2 startingForce, Vector2[] forceVectors)
        {
            Target = gameObject;
            Reset(startingForce, forceVectors);
        }

        public void Reset(Vector2 startingVector, Vector2[] forceVectors)
        {
            _currentVector = startingVector;
            _forceVectors = forceVectors;
            Finished = false;
        }

        public void Update()
        {
            throw new NotImplementedException();
            /*var changeVector = new Vector2(0, 0);
            foreach (var forceVector in _forceVectors)
            {
                changeVector += forceVector * EngineGlobals.ElapsedTime;
            }


            Target.Pos += (_currentVector * EngineGlobals.ElapsedTime) + changeVector;
            _currentVector += changeVector;*/
        }

        
    }

    public class EffectsManager
    {
        protected List<IEffect> _effects;
        protected IEffect[] _effectsArray;

        public EffectsManager()
        {
            _effects = new List<IEffect>();
            _effectsArray = new IEffect[0];
        }

        public void AddEffect(IEffect effect)
        {
            _effects.Add(effect);
            _effectsArray = _effects.ToArray();
        }

        public void Update()
        {
            if (_effectsArray.Length == 0)
                return;
            var toRemove = new List<IEffect>();
            for (var i = 0; i < _effectsArray.Length; i++)
            {
                if (_effectsArray[i].Finished)
                    toRemove.Add(_effectsArray[i]);
                else
                    _effectsArray[i].Update();
            }
            if (toRemove.Count <= 0) return;
            for (var i = 0; i < toRemove.Count; i++)
                _effects.Remove(toRemove[i]);
            _effectsArray = _effects.ToArray();
        }
    }

    public class SpiralMoveEffect : IEffect
    {
        protected Vector2 _destination;
        public bool Finished { get; protected set; }

        public GameObject Target { get; protected set; }

        public event SimpleEvent OnFinish;
        protected Vector2 _moveVector;
        protected Vector2 _posVector;
        protected float _rotation;
        protected int _moveInterval;
        protected Direction _rotationDirection;
        protected float _rotationSpeed;

        public SpiralMoveEffect(GameObject gameObject, Vector2 destinationPos, int moveInterval, float startingRotation, Direction direction, float rotationSpeed)
        {
            Target = gameObject;
            Reset(Target.Pos, destinationPos, moveInterval, startingRotation, direction, rotationSpeed);
        }

        public void Reset(Vector2 posVector, Vector2 destinationPos, int moveInterval, float startingRotation, Direction direction, float rotationSpeed)
        {
            _posVector = posVector;
            _destination = destinationPos;
            _moveInterval = moveInterval;
            _moveVector = _destination - _posVector;
            _rotationDirection = direction;
            _rotationSpeed = rotationSpeed;
            _rotation = startingRotation;
            Finished = false;
        }

        public void Update()
        {
            if (Finished)
                return;
            var changeVector = _moveVector * EngineGlobals.GetElapsedInGameTime() / _moveInterval;
            
            if (Vector2.Distance(_posVector, _destination) <= changeVector.Length())
            {
                Target.Pos = _destination;
                Finished = true;
                if (OnFinish != null)
                    OnFinish(this);
            }
            else
            {
                _posVector += changeVector;
                Target.Pos = _posVector;
            }
            var distance = Vector2.Distance(_posVector, _destination);
            Target.Pos = new Vector2(_destination.X + (float)Math.Cos(_rotation) * distance, _destination.Y + (float)Math.Sin(_rotation) * distance);
            if (_rotationDirection == Direction.Clockwise)
            {
                _rotation += (_rotationSpeed / 1000f * (float)EngineGlobals.GameTime.ElapsedGameTime.TotalMilliseconds);
                if (_rotation >= MathHelper.Pi * 2f)
                {
                    _rotation = _rotation - MathHelper.Pi * 2f;
                }
            }
            else
            {
                _rotation -= (_rotationSpeed / 1000f * (float)EngineGlobals.GameTime.ElapsedGameTime.TotalMilliseconds);
                if (_rotation <= -MathHelper.Pi * 2f)
                {
                    _rotation = _rotation + MathHelper.Pi * 2f;
                }
            }
            Target.Pos = new Vector2(_destination.X + (float)Math.Cos(_rotation) * distance, _destination.Y + (float)Math.Sin(_rotation) * distance);
        }

    }

}
