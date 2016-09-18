using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Objects
{
    public class DeathBall : Circle, IResetable
    {
        public enum Details
        {
            CircleLayerBg = 0,
            InnerCircle = 1,
            State1 = 2,
            State2 = 3,
            State3 = 4,
            Spikes = 5
        }

        public enum State
        {
            Normal = 0,
            Expanding = 1,
            Expanded = 2,
            Collapsing = 3

        }

        public class Spikes : CirclePhysicalObject
        {
#if EDITOR
            public override void LoadDefault(string resourceId, int index, int subObjectId)
            {
                throw new NotImplementedException();
            }
#endif

        }

#if EDITOR
        [PropertyOrder(18)]
#endif
            public int ActivationTime
        {
            get { return _activationTime; }
            set
            {
#if EDITOR
                FirePropertyChangingEvent("ActivationTime");
#endif
                _activationTime = value;

#if EDITOR
                FirePropertyChangedEvent("ActivationTime");
#endif
            }
        }

#if EDITOR
        [PropertyOrder(19)]
#endif
        public int Duration
        {
            get { return _duration; }
            set
            {
#if EDITOR
                FirePropertyChangingEvent("Duration");
                if (value < SpikesActivationEffectTime * 2)
                    throw new Exception("Duration cannot be less than 2 x SpikesActivationEffectTime");
#endif
                
                _duration = value;

#if EDITOR
                FirePropertyChangedEvent("Duration");
#endif
            }
        }

#if EDITOR
        [PropertyOrder(20)]
#endif
        public int StartupTime
        {
            get { return _startupTime; }
            set
            {
#if EDITOR
                FirePropertyChangingEvent("StartupTime");
#endif

                _startupTime = value;

#if EDITOR
                FirePropertyChangedEvent("StartupTime");
#endif
            }
        }

#if EDITOR
        [PropertyOrder(20)]
#endif
        public int SpikesActivationEffectTime
        {
            get { return _spikesActivationEffectTime; }
            set
            {
#if EDITOR
                FirePropertyChangingEvent("SpikesActivationEffectTime");
#endif

                _spikesActivationEffectTime = value;

#if EDITOR
                FirePropertyChangedEvent("SpikesActivationEffectTime");
#endif
            }
        }

        protected int _duration;
        protected int _activationTime;
        protected int _startupTime;
        protected int _spikesActivationEffectTime;
        protected bool _startup;
        protected Image[] _images;
        protected RotateEffect _rotateTimerEffect;
        protected ResizeEffectP _spikesActivateEffect;
        protected Direction _detailsRotationDirection;
        protected bool _manualAction;
        //protected Timer _timer;

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public Spikes SpikesObject;

        public bool Deadly { get; set; }


#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public State CurrentState { get; protected set; }

        [ContentSerializerAttribute(Optional = true)]
        public bool AlwaysUpdate
        {
            get { return GlobalUpdate; }
            set { GlobalUpdate = value; }
        }

        public DeathBall()
        {
            TypeId = GameObjectType.DeathBall.ToString();
            //_timer = new Timer(true);
        }

        public override void Update()
        {
            base.Update();

            if (!IsActivated && !_manualAction)
                return;
            switch (CurrentState)
            {
                case State.Normal:
                    _rotateTimerEffect.Update();
                    if (_rotateTimerEffect.Finished)
                    {
                        SpikesObject.Mask.IsHidden = false;
                        _spikesActivateEffect.Reset(new Vector2(Mask.Width*1.4609f, Mask.Height*1.4609f),
                            SpikesActivationEffectTime);
                        CurrentState = State.Expanding;
                        SpikesObject.IgnoreCollision = false;
                        EngineGlobals.SoundManager.Play(
                            "explode",
                            "explode",
                            this,
                            1.0f,
                            false,
                            1.0f,
                            1.0f);
                        Deadly = true;
                    }
                    break;
                case State.Expanding:
                    _spikesActivateEffect.Update();
                    if (_spikesActivateEffect.Finished)
                    {
                      //  if (!_manualAction || (_manualAction && IsActivated))
                      //  {
                            _rotateTimerEffect.Reset(MathHelper.ToRadians(280), _duration - SpikesActivationEffectTime);
                      //  }
                        if (_manualAction)
                            _manualAction = false;
                        CurrentState = State.Expanded;
                    }
                    break;
                case State.Expanded:
                    _rotateTimerEffect.Update();
                    if (_rotateTimerEffect.Finished)
                    {
                        _spikesActivateEffect.Reset(new Vector2(Mask.Width, Mask.Height), SpikesActivationEffectTime);
                        CurrentState = State.Collapsing;
                    }
                    break;
                case State.Collapsing:
                    _spikesActivateEffect.Update();
                    if (_spikesActivateEffect.Finished)
                    {
                      //  if (!_manualAction || (_manualAction && IsActivated))
                       // {
                            _rotateTimerEffect.Reset(MathHelper.ToRadians(360), _activationTime);
                      //  }
                        if (_manualAction)
                            _manualAction = false;
                        Deadly = false;
                        SpikesObject.IgnoreCollision = true;
                        SpikesObject.Mask.IsHidden = true;
                        _detailsRotationDirection = _detailsRotationDirection == Engine.Core.Direction.Clockwise
                            ? Engine.Core.Direction.Counterclockwise
                            : Engine.Core.Direction.Clockwise;
                        CurrentState = State.Normal;
                    }
                    break;

            }
            if (CurrentState == State.Normal)
            {
                var rotationUpdate = (RotationSpeed/1000*EngineGlobals.GetElapsedInGameTime());
                if (_detailsRotationDirection == Engine.Core.Direction.Clockwise)
                {
                    _images[(int) Details.State1].Rotation += rotationUpdate;
                    _images[(int) Details.State2].Rotation -= rotationUpdate;
                    _images[(int) Details.State3].Rotation += rotationUpdate;
                }
                else
                {
                    _images[(int) Details.State1].Rotation -= rotationUpdate;
                    _images[(int) Details.State2].Rotation += rotationUpdate;
                    _images[(int) Details.State3].Rotation -= rotationUpdate;
                }
            }

        }

        public void ManualyActivate()
        {
            switch (CurrentState)
            {
                case State.Normal:
                    _manualAction = true;
                    return;
                case State.Expanding:
                    _manualAction = true;
                    return;
                case State.Expanded:
                    return;
                case State.Collapsing:
                    _manualAction = true;
                    _spikesActivateEffect.Reset(new Vector2(Mask.Width, Mask.Height), SpikesActivationEffectTime);
                    CurrentState = State.Expanding;
                    return;

            }
        }

        public void ManualyDeactivate()
        {
            switch (CurrentState)
            {
                case State.Normal:
                    return;
                case State.Expanding:
                    _manualAction = true;
                    return;
                case State.Expanded:
                    _manualAction = true;
                    return;
                case State.Collapsing:
                    _manualAction = true;
                    _spikesActivateEffect.Reset(new Vector2(Mask.Width, Mask.Height), SpikesActivationEffectTime);
                    CurrentState = State.Expanding;
                    return;

            }
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            _images = new[]
                {
                    new Image(EngineGlobals.Resources.Textures["CircleDetails"][(int) Details.CircleLayerBg],
                              _rectangle.GetRectangle()) {Scale = new Vector2(0.875f, 0.875f), LayerDepth = Mask.LayerDepth - 0.01f, Rotation = MathHelper.ToRadians(280)},
                    new Image(EngineGlobals.Resources.Textures["CircleDetails"][(int) Details.InnerCircle],
                              _rectangle.GetRectangle()) {Scale = new Vector2(0.875f, 0.875f), LayerDepth = Mask.LayerDepth - 0.02f },
                    new Image(EngineGlobals.Resources.Textures["CircleDetails"][(int) Details.State1],
                              _rectangle.GetRectangle()) {Scale = new Vector2(0.2578f, 0.2578f), LayerDepth = Mask.LayerDepth - 0.06f, Rotation = MathHelper.ToRadians(320)},
                    new Image(EngineGlobals.Resources.Textures["CircleDetails"][(int) Details.State2],
                              _rectangle.GetRectangle()) {Scale = new Vector2(0.375f, 0.375f), LayerDepth = Mask.LayerDepth - 0.05f, Rotation = MathHelper.ToRadians(270)},
                    new Image(EngineGlobals.Resources.Textures["CircleDetails"][(int) Details.State3],
                              _rectangle.GetRectangle()) {Scale = new Vector2(0.5234f, 0.5234f), LayerDepth = Mask.LayerDepth - 0.04f, Rotation = MathHelper.ToRadians(90)},
                    new Image(EngineGlobals.Resources.Textures["CircleDetails"][(int) Details.Spikes],
                              _rectangle.GetRectangle()) {IsHidden = true, LayerDepth = Mask.LayerDepth + 0.05f}
                };
            foreach (var image in _images)
            {
                
                image.Owner = this;
                image.Rect = Mask.Rect;
                image.Orgin = image.OriginCenter();
                Controller.AddObject(image);
            }

            SpikesObject = new Spikes();
            SpikesObject.SetNewData(_images[(int)Details.Spikes]);
            
            
            
            _startup = true;
            _detailsRotationDirection = Engine.Core.Direction.Clockwise;
            SpikesObject.Rectangle = Rectangle;
            SpikesObject.Mask.Orgin = ((Image)SpikesObject.Mask).OriginCenter();
            
            if (Deadly)
            {
                SpikesObject.SetSizeUnsafe((Mask.Width * 1.4609f), (Mask.Height * 1.4609f));
                _spikesActivateEffect = new ResizeEffectP(SpikesObject, new Vector2((Mask.Width * 1.4609f), (Mask.Height * 1.4609f)), 100);
                _images[(int) Details.CircleLayerBg].Rotation = MathHelper.ToRadians(360);
                _images[(int)Details.Spikes].IsHidden = false;
                SpikesObject.IgnoreCollision = false;
                _rotateTimerEffect = new RotateEffect(_images[(int)Details.CircleLayerBg], MathHelper.ToRadians(280), _duration - SpikesActivationEffectTime);
                CurrentState = State.Expanded;
              //  _timer.Start(_duration, false);
            }
            else
            {
                _rotateTimerEffect = new RotateEffect(_images[(int)Details.CircleLayerBg], MathHelper.ToRadians(360), StartupTime);
                _spikesActivateEffect = new ResizeEffectP(SpikesObject, new Vector2(Mask.Width, Mask.Height), 100);
                _images[(int)Details.Spikes].IsHidden = true;
                SpikesObject.IgnoreCollision = true;
                CurrentState = State.Normal;
             //   _timer.Start(StartupTime, false);
            }
            SpikesObject.Name = Name + "- SpikeObject";
            if (GameGlobals.EditorMode)
                return;
                GameGlobals.Physics.AddObjectToQueue(SpikesObject);
            GameGlobals.Physics.CommitQueue();
        }

        protected override void SetRectangle(float x, float y, float width, float height)
        {
            base.SetRectangle(x, y, width, height);
            if (_images != null)
            {
                foreach (var image in _images)
                {
                    image.Rect = Mask.Rect;
                }
            }
            if (SpikesObject != null)
                SpikesObject.Rectangle = Rectangle;
        }

        
        #if EDITOR

        public override void EditorDeleteObject()
        {
            foreach (var image in _images)
            {
                Controller.RemoveObject(image);
            }
        }

        public override object Clone()
        {
            var clone = (DeathBall)base.Clone();
            clone._images = new Image[_images.Length];
            for (var i = 0; i < _images.Length; i++)
            {
                clone._images[i] = (Image)_images[i].Clone();
            }
            return clone;
        }

        public override void EditorPasteObject()
        {
            foreach (var image in _images)
            {
                Controller.AddObject(image);
            }
        }

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            _duration = 1000;
            _activationTime = 3000;
            _spikesActivationEffectTime = 500;
            _startupTime = 3000;
            IsActivated = true;
        }

        

        public override void HideDetailImages(bool hide)
        {
            foreach (var image in _images)
            {
                image.IsHidden = hide;
            }
        }

        public override void RemoveDetails()
        {
            foreach (var image in _images)
            {
                Controller.RemoveObject(image);
            }
        }

#endif

        public void Reset()
        {
            SpikesObject.SetSizeUnsafe((Mask.Width), (Mask.Height));
            _images[(int) Details.CircleLayerBg].Rotation = 0;
            _rotateTimerEffect.Reset(MathHelper.ToRadians(360), StartupTime);
            _spikesActivateEffect.Reset(new Vector2(Mask.Width*1.4609f, Mask.Height*1.4609f), SpikesActivationEffectTime);
            _images[(int) Details.Spikes].IsHidden = true;
            SpikesObject.IgnoreCollision = true;
            CurrentState = State.Normal;
        }
    }
}
