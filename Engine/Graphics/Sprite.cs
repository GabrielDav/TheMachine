using System;
using System.ComponentModel;
using System.Linq;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
#if EDITOR
    public delegate void PropertyValueChanedEventHandler(
        object sender, string propertyName, object valueBeforeChange, object valueAfterChange);
#endif

    public class Sprite : GameObject, IDynamic
    {

#if EDITOR
        protected object _propertyValue;

        public event PropertyValueChanedEventHandler AnimationPropertyChanged;
#endif

        public SpriteData Data { get; protected set; }

        public bool Paused
        {
            get { return _timer.Paused; }
            set
            {
                if (value)
                {
                    _playing = false;
                    _timer.Pause();
                }
                else
                    _timer.Resume();
            }
        }

        public bool Finished { get; protected set; }

        public bool Playing
        {
            get { return _playing; }
        }

        public bool Looped { get; protected set; }

        protected bool _playing;
        protected int _currentFrame;
        protected Animation _currentAnimation;
        protected Timer _timer;
        public Animation CurrentAnimation { get { return _currentAnimation; }}
        public event SimpleEvent AnimationFinished;



        public int CurrentFrame
        {
            get { return _currentFrame; }
            set { _currentFrame = value;
                _frame = GetFrameRectangle();
            }
        }


#if EDITOR
        public Sprite(SpriteData data, Rectangle rect, string animation, bool looped = false)
        {
            Initialize(data, rect, Color.White, animation, looped);
        }

        public Sprite(SpriteData data, Rectangle rect, string aniamtion, Color color, bool looped = false)
        {
            Initialize(data, rect, color, aniamtion, looped);
        }

        public Sprite(SpriteData data)
        {
            if (data != null)
            {
                var defaultAnimation = data.Animations.FirstOrDefault() ?? data.Animations[0];
                Initialize(data, new Rectangle(), Color.White, defaultAnimation.Name, false);
            }
        }

        public Sprite(SpriteData data, Rectangle rectangle)
        {
            Initialize(data, rectangle, Color.White, "", false);
            _playing = false;
        }
#else
        public Sprite(SpriteData data, Rectangle rect, string animation, bool looped)
        {
            Initialize(data, rect, Color.White, animation, looped);
        }

        public Sprite(SpriteData data, Rectangle rect, string aniamtion, Color color, bool looped)
        {
            Initialize(data, rect, color, aniamtion, looped);
        }

        public Sprite(SpriteData data)
        {
            if (data != null)
            {
                var defaultAnimation = data.Animations.FirstOrDefault() ?? data.Animations[0];
                Initialize(data, new Rectangle(), Color.White, defaultAnimation.Name, false);
            }
        }
#endif

        public void LoadSpriteData(SpriteData data)
        {
            var defaultAnimation = data.Animations.FirstOrDefault() ?? data.Animations[0];
            Initialize(data, Rect, Color, defaultAnimation.Name, false);
        }

        protected void Initialize(SpriteData data, Rectangle rect, Color color, string animation, bool looped)
        {
            Data = data;
            Rect = rect;
            Color = color;
            _timer = new Timer();

#if EDITOR
            foreach (var anim in data.Animations)
            {
                anim.PropertyChanging += OnAnimationPropertyChanging;
                anim.PropertyChanged += OnAniamtionPropertyChanged;
            }
#endif
            

            if (animation != String.Empty)
                PlayAnimation(animation, looped);
        }

        public void PlayAnimation(string name, bool looped)
        {
            SetAnimation(name);
            Looped = looped;
            _playing = true;
            _timer.Start(_currentAnimation.Speed,false);
        }

        public void SetAnimation(string name)
        {
            _currentAnimation = null;
            foreach (var animation in Data.Animations)
            {
                if (animation.Name == name)
                {
                    _currentAnimation = animation;
                    break;
                }
            }
            if (_currentAnimation == null)
                throw new Exception("No such animation '" + name + "' in sprite");
            _currentFrame = 0;
            _frame = GetFrameRectangle();
            if (Orgin != Vector2.Zero)
                Orgin = new Vector2(_frame.Value.Width/2f, _frame.Value.Height/2f);
            _playing = false;
            Finished = false;
        }

        public void ResumeAnimation(bool looped)
        {
            Looped = looped;
            _playing = true;
            _timer.Start(_currentAnimation.Speed, false);
        }

        public void StopAnimation()
        {
            _playing = false;
        }

#if EDITOR
        public void RemoveAnimation(int index)
        {
            Data.Animations[index].PropertyChanged -= OnAniamtionPropertyChanged;
            Data.Animations[index].PropertyChanging -= OnAnimationPropertyChanging;
            for (int i = index; i < Data.Animations.Length - 2; i++)
            {
                Data.Animations[i] = Data.Animations[i + 1];
            }
            Data.Animations[Data.Animations.Length - 1].PropertyChanged -= OnAniamtionPropertyChanged;
            Data.Animations[Data.Animations.Length - 1].PropertyChanging -= OnAnimationPropertyChanging;
            Array.Resize(ref Data.Animations, Data.Animations.Length - 1);
        }

        public void AddAnimation(Animation animation)
        {
            Array.Resize(ref Data.Animations, Data.Animations.Length + 1);
            Data.Animations[Data.Animations.Length - 1] = animation;
            animation.PropertyChanging += OnAnimationPropertyChanging;
            animation.PropertyChanged += OnAniamtionPropertyChanged;
        }


        private void OnAnimationPropertyChanging(object sender, PropertyChangingEventArgs propertyChangingEventArgs)
        {
            var property = sender.GetType().GetProperty(propertyChangingEventArgs.PropertyName);
            _propertyValue = property.GetValue(sender, null);
        }

        private void OnAniamtionPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var property = sender.GetType().GetProperty(propertyChangedEventArgs.PropertyName);
            var value = property.GetValue(sender, null);
            if (_propertyValue == value)
                return;
            if (AnimationPropertyChanged != null)
            {
                AnimationPropertyChanged(this, propertyChangedEventArgs.PropertyName, _propertyValue, value);
            }
        }

#endif

        public void Update()
        {
            if (!Playing)
                return;
            _timer.Update();
            if (!_timer.Finished)
            {
                return;
            }
            _timer.Restart();
            _currentFrame++;
            if (_currentFrame == _currentAnimation.Frames)
            {
                if (Looped)
                    _currentFrame = 0;
                else
                {
                    _currentFrame--;
                    Finished = true;
                    _playing = false;
                    if (AnimationFinished != null)
                    {
                        AnimationFinished(this);
                    }
                    return;
                }
            }
            _frame = GetFrameRectangle();
        }

        public Image GetFirstFrame()
        {
            return new Image(_currentAnimation.Texture) {Frame = _frame, Rect = Rect, Color = Color};
        }

        protected Rectangle GetFrameRectangle()
        {
            var y = _currentFrame/(_currentAnimation.Texture.Data.Width/_currentAnimation.FrameWidth);
            y *= _currentAnimation.FrameHeight;
            var x = _currentFrame%(_currentAnimation.Texture.Data.Width/_currentAnimation.FrameWidth);
            x *= _currentAnimation.FrameWidth;
            return new Rectangle(x, y, _currentAnimation.FrameWidth, _currentAnimation.FrameHeight);
        }

        public Vector2 OriginCenter()
        {
            return new Vector2(_frame.Value.Width/2, _frame.Value.Height/2);
        }

        //protected override Rectangle CalculateCornerRectangle()
        //{
        //    if (Orgin.X < EngineGlobals.Epsilon && Orgin.Y < EngineGlobals.Epsilon)
        //    {
        //        return _rect;
        //    }
        //    var ox = Orgin.X/_currentAnimation.Texture.Data.Width;
        //    var oy = Orgin.Y/_currentAnimation.Texture.Data.Height;
        //    return new Rectangle(_rect.X - (int) Math.Round(_rect.Width*ox, 0),
        //                         _rect.Y - (int) Math.Round(_rect.Height*oy, 0), _rect.Width, _rect.Height);

        //}

        [ContentSerializerIgnore]
        public override bool IgnoreCulling { get { return false; } set { throw new NotImplementedException();} }

        public override void Draw()
        {
//#if DEBUG

            if (_isHidden)
            {
                return;
            }
#if DEBUG
            if (_currentAnimation.Texture.Data.IsDisposed)
            {
                throw new Exception("Texture is disposed for current sprite");
            }
#endif
            
            EngineGlobals.Batch.Draw(_currentAnimation.Texture.Data, DrawRectangle, _frame,
                                     Color,
                                     Rotation, _origin, Flip, _layerDepth);
//#else
//            if (_isHidden)
//                return;
//            if (IsTransformedByCamera)
//            {
//                if (!EngineGlobals.Camera.IsObjectVisible(Rect))
//                    return;
//                EngineGlobals.Batch.Draw(_currentAnimation.Texture, EngineGlobals.Camera.Transform(Rect), _frame,
//                                         Color,
//                                         Rotation, _origin, SpriteEffects.None, _layerDepth);
//            }
//            else
//                EngineGlobals.Batch.Draw(_currentAnimation.Texture, Rect, _frame,
//                                         Color,
//                                         Rotation, _origin, SpriteEffects.None, _layerDepth);
//#endif
        }

        public override void Dispose()
        {
            AnimationFinished = null;
#if EDITOR
            AnimationPropertyChanged = null;
#endif
            base.Dispose();
        }

    }
}
