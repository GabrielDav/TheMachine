using System;
using System.Collections.Generic;
using System.ComponentModel;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics.Triggers;
using GameLibrary.Particles;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Objects
{
    public class SeekerDot : InkDot
    {
        //protected List<Region> _regionsToCheck;
        protected SeekerDotExplosionParticleManager _particleManager;
        protected bool _dead;


#if EDITOR
        [ReadOnly(true)]
        public override int Diameter { get { return base.Diameter; } set { base.Diameter = value; } }
#endif

        public SeekerDot()
        {
            TypeId = GameObjectType.SeekerDot.ToString();
        }

        protected override void Init()
        {
            Animated = true;
            base.Init();
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            GlobalUpdate = true;
            SetRectangle(_rectangle.X, _rectangle.Y, 60, 60);
            //_regionsToCheck = new List<Region>();
           /* foreach (var region in GameGlobals.Map.Regions)
            {
                if (region.CheckedObjectName == Name)
                {
                    _regionsToCheck.Add(region);
                }
            }*/
            _particleManager = new SeekerDotExplosionParticleManager();
            _particleManager.LoadDefault();
            _particleManager.Load(this);
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            Diameter = 6;
            InkToRecover = 0;
            MoveSpeed = 200;
            SpeedIncrement = 0;
            TriggerDistance = 300;
        }
#endif


        public void Die()
        {
            IsActivated = false;
            Follow = false;
            _dead = true;
            Mask.IsHidden = true;
            EngineGlobals.SoundManager.Play(
                    "seeker_dot_shatter",
                    "shatter",
                    this,
                    1.0f,
                    false,
                    1.0f,
                    1.0f);
        }

        public override void Merge()
        {
            IsActivated = false;
            Follow = false;
            Mask.IsHidden = true;
            GameGlobals.Player.Die();
        }

        public override void StartFollow()
        {
            base.StartFollow();
            ((Sprite)Mask).PlayAnimation("Seek", true);
        }

        public override void Update()
        {
            if (IsActivated)
            {
                var distance = Vector2.Distance(HalfPos, GameGlobals.Player.HalfPos);
                if (distance <= HalfSize.X + GameGlobals.Player.HalfSize.X)
                {
                    Merge();
                }
                if (Follow)
                {
                    SpeedIncrementTotal += (SpeedIncrement * EngineGlobals.GetElapsedInGameTime() / 1000f);
                    var direction = Vector2.Normalize(GameGlobals.Player.HalfPos - HalfPos);

                    var delta = EngineGlobals.GetElapsedInGameTime() / 1000 *
                                (MoveSpeed + SpeedIncrementTotal);
                    HalfPos += direction * delta;
                }
                else
                {
                    var brderDistance = distance + HalfSize.X + GameGlobals.Player.HalfSize.X;

                    if (brderDistance <= TriggerDistance)
                    {
                        StartFollow();
                    }
                }


            }
            if (_dead)
            {
                _particleManager.Update();
                return;
            }
            if (!IsActivated || !Follow)
                return;
            ((Sprite)Mask).Update();
            var lineVector = GameGlobals.Player.HalfPos - HalfPos;
            Mask.Rotation = (float)Math.Atan2(lineVector.Y, lineVector.X) + MathHelper.PiOver2;
            /*foreach (var region in _regionsToCheck)
            {
                if (region.Rectangle.Contains(Rectangle))
                    Die();
            }*/
        }
    }
}
