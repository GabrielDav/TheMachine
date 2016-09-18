using System;
using System.Collections.Generic;
#if EDITOR
using System.ComponentModel;
#endif
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Particles
{

#if EDITOR
    [TypeConverter(typeof(ExpandableObjectConverter))]
#endif
    public class SawParticleManager : BaseParticleManager
    {
#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public Saw SawObject;

        private Vector2 _gravityModifier;
        private Vector2 _initialVelocity;

        public Vector2 GravityModifier
        {
            get { return _gravityModifier; }
            set { _gravityModifier = value; }
        }

        public Vector2 InitialVelocity
        {
            get { return _initialVelocity; }
            set { _initialVelocity = value; }
        }

        public override void Load(PhysicalObject owner)
        {
            base.Load(owner);
            SawObject = owner as Saw;
            _particleName = "SawParticle";
        }

        public override Particle CreateNewParticle()
        {
            var img = new Image(EngineGlobals.Resources.Textures["SawParticle"][0]) {Owner = this};
            Controller.AddObject(img);

            return new Particle(img,
                                new List<IEffect>
                                    {
                                        new MoveEffect(img, Vector2.Zero, 0),
                                        //new ColorEffect(img, Vector2.Zero, 0)
                                    });
        }

        public override void LoadDefault()
        {
            CreationRateMin = 100;
            CreationRateMax = 100;
            GravityModifier = new Vector2(0, 200);
            InitialVelocity = new Vector2(50, -300);
            LifeTime = 1000;
            WaveAmountMin = 1;
            WaveAmountMax = 1;
        }

        public override void Dispose()
        {
            base.Dispose();
            SawObject = null;
        }


        protected override void InitParticle(Particle particle)
        {
            //particle.Image.LoadTexture(EngineGlobals.Resources.Textures["SawParticle"][GameGlobals.Random.Next(0, 2)]);
            //particle.Image.Orgin = particle.Image.OriginCenter();
            
            //particle.Image.Rotation = (float)(GameGlobals.Random.NextDouble()*2*MathHelper.Pi);

            //var amount = GameGlobals.Random.Next(150, 175);

            //var x = amount;
            //var y = amount - GameGlobals.Random.Next(25, 75);

            //var move = new Vector2(SawObject.CurrentDirection.X * x, -y);

            //particle.Image.Rect = new Rectangle(
            //    (int)(SawObject.HalfPos.X + (SawObject.CurrentDirection.X*((SawObject.Diameter/2) - 5))),
            //    (int)SawObject.HalfPos.Y,
            //    8,
            //    8);

            //((MoveEffect)particle.Effects[0]).Reset(particle.Image.Pos, particle.Image.Pos + move, GameGlobals.Random.Next(600, 1000));

            //particle.Image.LoadTexture(EngineGlobals.Resources.Textures["SawParticle"][GameGlobals.Random.Next(0, 2)]);
            //particle.Image.Orgin = particle.Image.OriginCenter();
            //particle.Image.LayerDepth = SawObject.LayerDepth + 0.01f;
            //particle.Image.Rotation = MathHelper.ToRadians(GameGlobals.Random.Next(0, 360));
            //var particlePos = new Vector2((SawObject.HalfPos.X + ((SawObject.HalfSize.X/2) * (SawObject.CurrentDirection.X < 0? -1: 1))
            //    ), SawObject.HalfPos.Y);
            //particle.Image.Rect = new Rectangle(
            //    (SawObject.CurrentDirection.X < 0 ? (int)particlePos.X - 8 : (int)particlePos.X + 8),
            //    (int)particlePos.Y,
            //    8,
            //    8);
            //var throwAngle = MathHelper.ToRadians(GameGlobals.Random.Next(40, 70));
            //var throwVector = new Vector2(
            //    SawObject.CurrentDirection.X < 0 ? (-(float)Math.Cos(throwAngle) * 150 - SawObject.Speed)
            //    : ((float)Math.Cos(throwAngle) * 150 + SawObject.Speed), (-(float)Math.Sin(throwAngle) * 300) - SawObject.Speed);

           // ((ApplyForceVectorEffect)particle.Effects[0]).Reset(_initialVelocity, new[] {_gravityModifier });

        }

        
    }
}
