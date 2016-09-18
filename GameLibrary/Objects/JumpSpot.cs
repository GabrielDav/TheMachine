using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.ComponentModel;
using TheGoo;

namespace GameLibrary.Objects
{
    public class JumpSpot : CirclePhysicalObject
    {
        protected bool _playerIsOn;
        protected Sprite _counter;
        protected bool _processStarted;
        public int ReleaseMode { get; protected set; }
        protected ResizeEffectP _resizeEffect;


        #if EDITOR
        [Browsable(false)]
        #endif
        [ContentSerializerIgnore]
        public bool Deadly;

        #if EDITOR
        [PropertyOrder(18)]
        #endif
        public int Duration { get; set; }

#if EDITOR
        [ReadOnly(true)]
#endif
        public override float LayerDepth
        {
            get
            {
                return base.LayerDepth;
            }
            set
            {
                base.LayerDepth = value;
            }
        }

        #if EDITOR
        [ReadOnly(true)]
        public override System.Drawing.Size GridSize
        {
            get
            {
                return base.GridSize;
            }
            set
            {
                base.GridSize = value;
            }
        }
        #endif

        public JumpSpot()
        {
            Animated = false;
            TypeId = GameObjectType.JumpSpot.ToString();
            Init();
        }

        public void PlayerJumpsOn()
        {
            if (!IsActivated)
                return;
            _playerIsOn = true;
            if (!_processStarted)
            {
                _sprite.CurrentAnimation.Speed = Duration / 6;
                _sprite.PlayAnimation("JumpSpotTimerAnimation", false);
                _processStarted = true;
                _sprite.IsHidden = false;
            }
        }

        public void PlayerJumpsOff()
        {
            _playerIsOn = false;
        }

        public override void Update()
        {
            base.Update();
            if (IsActivated)
            {
                
                if (_processStarted)
                {
                    _sprite.Update();
                    if (_sprite.Finished)
                    {
                        _processStarted = false;
                        if (_playerIsOn)
                        {
                            GameGlobals.Player.CommitJump(0, 0);
                            GameGlobals.Player.Rotation = 90;
                            PlayerJumpsOff();
                        }
                        _resizeEffect.Reset(new Vector2(Rectangle.Width+16, Rectangle.Height+16), 200);
                        
                        ReleaseMode = 1;
                    }
                }
                else if (ReleaseMode > 0)
                {
                    _resizeEffect.Update();
                    if (ReleaseMode == 1 && _resizeEffect.Finished)
                    {
                        _resizeEffect.Reset(new Vector2(Rectangle.Width - 16, Rectangle.Height - 16), 200);
                        
                        ReleaseMode = 2;
                    }
                    else if (ReleaseMode == 2 && _resizeEffect.Finished)
                    {
                        ReleaseMode = 0;
                        _sprite.StopAnimation();
                        _sprite.CurrentFrame = 0;
                        _sprite.IsHidden = true;
                        
                    }
                }
            }
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            if (GameGlobals.EditorMode)
                return;
#if EDITOR
            _sprite =
                new Sprite(
                    EngineGlobals.ContentCache.Load<SpriteData>(
                        @"GameObjects\Sprites\JumpSpotTimer\JumpSpotTimer"), Rectangle.GetRectangle()) { LayerDepth = LayerDepth - 0.1f, Owner = this };
#else
            _sprite =
                new Sprite(
                    EngineGlobals.ContentCache.Load<SpriteData>(@"GameObjects\Sprites\JumpSpotTimer\JumpSpotTimer"))
                {
                    LayerDepth = LayerDepth - 0.1f,
                    Owner = this,
                    Rect = Rectangle.GetRectangle()
                };
#endif
            _sprite.SetAnimation("JumpSpotTimerAnimation");
            _sprite.IsHidden = true;
            _resizeEffect = new ResizeEffectP(this, new Vector2(Rectangle.Width, Rectangle.Height), 100);
            Controller.AddObject(_sprite);
        }

#if EDITOR

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Diameter = 6;
            Duration = 3000;
            RotationSpeed = 0f;
            Mask.LayerDepth = 0.71f;
            IsActivated = true;
        }

        #endif
    }
}
