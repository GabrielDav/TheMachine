using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Particles
{
    public class SeekerDotExplosionParticleManager : BaseParticleManager
    {
        protected int _particleOffset;
        protected int _releaseSpeedMin;
        protected int _releaseSpeedMax;

        public SeekerDot SeekerDotObject;

        public int ParticleSize { get; set; }

        public int ParticleOffset
        {
            get { return _particleOffset; }
            set { _particleOffset = value; }
        }

        public int ReleaseSpeedMin
        {
            get { return _releaseSpeedMin; }
            set { _releaseSpeedMin = value; }
        }

        public int ReleaseSpeedMax
        {
            get { return _releaseSpeedMax; }
            set { _releaseSpeedMax = value; }
        }

        public override Particle CreateNewParticle()
        {
            var img = new Image(EngineGlobals.Resources.Textures["SeekerParticle"][GameGlobals.Random.Next(0, 2)])
            {
                Owner = this,
                Rotation = MathHelper.ToRadians(GameGlobals.Random.Next(0, 360))
            };
            Controller.AddObject(img);

            return new Particle(img,
                                new List<IEffect>
                                    {
                                        new MoveEffect(img, Vector2.Zero, 0),
                                        new ColorEffect(img, new Color(255, 255, 255, 0), 0)
                                    });
        }

        public override void Load(PhysicalObject seekerDot)
        {
            base.Load(seekerDot);
            SeekerDotObject = seekerDot as SeekerDot;
            _particleName = "SeekerDotExplosion";
        }

        public override void LoadDefault()
        {
            ParticleSize = 10;
            ParticleOffset = 15;
            ReleaseSpeedMin = 90;
            ReleaseSpeedMax = 150;
            LifeTime = 1000;
            CreationRateMin = 100;
            CreationRateMax = 100;
            WaveAmountMin = 16;
            WaveAmountMax = 20;
            _maxWaves = 1;
        }

        protected override void InitParticle(Particle particle)
        {
            var size = ParticleSize;
            var rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(GameGlobals.Random.Next(0, 360)));
            var moveVector = Vector2.Transform(
                new Vector2(
                    0,
                    GameGlobals.Random.Next(
                        _releaseSpeedMin,
                        _releaseSpeedMax)),
                rotation);
            var startVect = Vector2.Transform(
                new Vector2(
                    0,
                    5),
                rotation);

            particle.Image.Rect = new Rectangle((int)(SeekerDotObject.HalfPos.X + startVect.X),
                                                (int)(SeekerDotObject.HalfPos.Y - startVect.Y),
                                                size,
                                                size);

            ((MoveEffect)particle.Effects[0]).Reset(
                new Vector2(particle.Image.Rect.X, particle.Image.Rect.Y),
                new Vector2(
                    particle.Image.Rect.X + moveVector.X,
                    particle.Image.Rect.Y + moveVector.Y),
                _lifeTime);
            ((ColorEffect)particle.Effects[1]).Reset(new Color(255, 255, 255, 0), _lifeTime);
        }

    }
}
