using System;
using System.Collections.Generic;
using System.ComponentModel;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Particles
{
    public class LevelEndParticleManager : BaseParticleManager
    {

        protected int _particleSizeMax;
        protected int _particelSizeMin;
        protected int _particleOffset;
        protected int _releaseSpeedMin;
        protected int _releaseSpeedMax;

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public LevelEnd LevelEndObject;

        public int ParticleSizeMin { get { return _particelSizeMin; } set { _particelSizeMin = value; } }
        public int ParticleSizeMax
        {
            get { return _particleSizeMax; }
            set { _particleSizeMax = value; }
        }

        public int MaxCreationDistance
        { get; set; }

        public int MinCreationDistance { get; set; }

        public override Particle CreateNewParticle() 
        {
            var img = new Image(EngineGlobals.Resources.Textures["Particle"][0]) { Owner = this };
            Controller.AddObject(img);

            return new Particle(img,
                                new List<IEffect>
                                    {
                                        new MoveEffect(img, Vector2.Zero, 0),
                                        new ResizeEffect(img, Vector2.Zero, 0)
                                    });
        }

        public override void LoadDefault()
        {
            MinCreationDistance = 30;
            MaxCreationDistance = 30;
            ParticleSizeMin = 2;
            ParticleSizeMax = 12;
            LifeTime = 1200;
            CreationRateMin = 10;
            CreationRateMax = 10;
            WaveAmountMin = 1;
            WaveAmountMax = 1;
        }

        public override void Load(PhysicalObject levelEnd)
        {
            base.Load(levelEnd);
            LevelEndObject = levelEnd as LevelEnd;
            _particleName = "LevelEndParticle";
        }

        protected override void InitParticle(Particle particle)
        {
            var size = GameGlobals.Random.Next(_particelSizeMin, _particleSizeMax);
            var angle = EngineGlobals.Random.Next(0, 360);
            var distance = EngineGlobals.Random.Next(MinCreationDistance, MaxCreationDistance) + LevelEndObject.HalfSize.X;
            particle.Image.Rect = new Rectangle((int) (LevelEndObject.HalfPos.X + (distance*Math.Cos(angle))),
                (int) (LevelEndObject.HalfPos.Y + (distance*Math.Sin(angle))), size, size);
            var pos = particle.Image.Center();
            var moveVector = LevelEndObject.HalfPos - pos;
            ((MoveEffect)particle.Effects[0]).Reset(
                new Vector2(particle.Image.Rect.X, particle.Image.Rect.Y),
                new Vector2(
                    particle.Image.Rect.X + moveVector.X,
                    particle.Image.Rect.Y + moveVector.Y),
                _lifeTime);
            ((ResizeEffect)particle.Effects[1]).Reset(new Vector2(4, 4), _lifeTime); 
        }

        public override void Dispose()
        {
            base.Dispose();
            LevelEndObject = null;
        }
    }
}
