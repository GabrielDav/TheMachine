using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Particles
{
    public class PlayerOutOfInkParticleManager : BaseParticleManager
    {

        protected int _particleSizeMax;
        protected int _particelSizeMin;
        protected int _particleOffset;
        protected int _releaseSpeedMin;
        protected int _releaseSpeedMax;

        public Player PlayerObject;

        public int ParticleSizeMin { get { return _particelSizeMin; } set { _particelSizeMin = value; } }
        public int ParticleSizeMax
        {
            get { return _particleSizeMax; }
            set { _particleSizeMax = value; }
        }

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
            var img = new Image(EngineGlobals.Resources.Textures["Particle"][0]) { Owner = this };
            Controller.AddObject(img);

            return new Particle(img,
                                new List<IEffect>
                                    {
                                        new MoveEffect(img, Vector2.Zero, 0),
                                        new ResizeEffect(img, Vector2.Zero, 0)
                                    });
        }

        public override void Load(PhysicalObject player)
        {
            base.Load(player);
            PlayerObject = player as Player;
            _particleName = "OutOfInk";
        }

        public override void LoadDefault()
        {
            ParticleSizeMin = 8;
            ParticleSizeMax = 24;
            ParticleOffset = 15;
            ReleaseSpeedMin = 40;
            ReleaseSpeedMax = 90;
            LifeTime = 1000;
            CreationRateMin = 100;
            CreationRateMax = 100;
            WaveAmountMin = 21;
            WaveAmountMax = 32;
            _maxWaves = 1;
        }

        protected override void InitParticle(Particle particle)
        {
            var size = GameGlobals.Random.Next(_particelSizeMin, _particleSizeMax);
            var vector = new Vector2(0, PlayerObject.HalfSize.X + _particleOffset / 2f);
            var moveVector = Vector2.Transform(
                new Vector2(
                    0,
                    GameGlobals.Random.Next(
                        _releaseSpeedMin,
                        _releaseSpeedMax)),
                Matrix.CreateRotationZ(MathHelper.ToRadians(GameGlobals.Random.Next(0, 360))));
            moveVector += (PlayerObject.Direction * 10);
            moveVector *= 2;

            var result = Vector2.Transform(
                vector,
                Matrix.CreateRotationZ(PlayerObject.Mask.Rotation));
            
            particle.Image.Rect = new Rectangle((int)(PlayerObject.HalfPos.X - result.X),
                                                (int)(PlayerObject.HalfPos.Y - result.Y),
                                                size,
                                                size);

            ((MoveEffect)particle.Effects[0]).Reset(
                new Vector2(particle.Image.Rect.X, particle.Image.Rect.Y),
                new Vector2(
                    particle.Image.Rect.X + moveVector.X,
                    particle.Image.Rect.Y + moveVector.Y),
                _lifeTime);
            ((ResizeEffect)particle.Effects[1]).Reset(Vector2.Zero, _lifeTime); 
        }
    }
}
