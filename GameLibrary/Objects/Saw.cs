using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Particles;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Objects
{
    public class Saw : MovingCircle
    {
        private Image _handImage;
        
        public bool HandAttached;

        public Rectangle HandRect;

        #if EDITOR
        [PropertyOrder(23)]
        #endif
        public SawParticleManager SawParticleManager { get; set; }

        public Saw()
        {
            TypeId = GameObjectType.Saw.ToString();
        }

        #if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            SawParticleManager = new SawParticleManager();
            SawParticleManager.LoadDefault();
            base.LoadDefault(resourceId, index, subObjectId);
            RotationSpeed = 5;
        }
        #endif

        public override void Rotate()
        {
            if (_rotationDirection == Engine.Core.Direction.Clockwise)
            {
                Mask.Rotation += (RotationSpeed / 1000 * EngineGlobals.GetElapsedInGameTime());
                if (Mask.Rotation >= MathHelper.Pi * 2f)
                {
                    Mask.Rotation = Mask.Rotation - MathHelper.Pi * 2f;
                }
            }
            else
            {
                Mask.Rotation -= (RotationSpeed / 1000 * EngineGlobals.GetElapsedInGameTime());
                if (Mask.Rotation <= -MathHelper.Pi * 2f)
                {
                    Mask.Rotation = Mask.Rotation + MathHelper.Pi * 2f;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (IsActivated)
            {
                SawParticleManager.Update();
                if (GameGlobals.Player != null)
                {
                    var distance = Vector2.Distance(Mask.Pos, GameGlobals.Player.Mask.Pos);
                    if (distance <= (float)Mask.Width / 2 + (float)GameGlobals.Player.Mask.Width / 2)
                    {
                        GameGlobals.Player.Die();
                    }
                }

                //EngineGlobals.SoundManager.Play(Name, "saw", this, 0.75f, false, -0.5f, -0.5f);
            }
        }


        #if EDITOR
        public override bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return flag == DebugFlag.Destination;
        }
        #endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            
            SawParticleManager.Load(this);
            
            if (HandAttached)
            {
                _handImage = new Image(EngineGlobals.Resources.Textures["SawHand"][0],
                                 _rectangle.GetRectangle())
                {
                    LayerDepth = Mask.LayerDepth - 0.05f,
                    Owner = this,
                    Rect = HandRect,
                    Flip = Flip
                };

                Controller.AddObject(_handImage);
            }
            
        }

        protected override void Move()
        {
            base.Move();
            if (_moving)
            {
                var delta = EngineGlobals.GetElapsedInGameTime() / 1000 * Speed;
                if (HandAttached)
                {
                    _handImage.Pos += CurrentDirection * delta;
                }
            }
        }

        public override object Clone()
        {
            var clone = base.Clone() as Saw;
            clone.SawParticleManager = (SawParticleManager)SawParticleManager.Clone();
            return clone;
        }

    }
}
