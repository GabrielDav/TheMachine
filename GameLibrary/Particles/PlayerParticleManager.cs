using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Particles
{
    public class PlayerParticleManager : BaseParticleManager
    {
        public Player PlayerObject;

        public PlayerParticleManager(PhysicalObject player)
        {
            PlayerObject = player as Player;
        }

        public override Particle CreateNewParticle()
        {
            var img = new Image(EngineGlobals.Resources.Textures["Particle"][0]);
            Controller.AddObject(img);

            return new Particle(img,
                                new List<IEffect>
                                    {
                                        new MoveEffect(img, Vector2.Zero, 0)
                                    });
        }

        public void InitParticle(Particle particle)
        {
            var size = GameGlobals.Random.Next(2, 10);
            var vector = new Vector2(0, PlayerObject.Mask.DrawRectangle.Height / 2f);
            var result = Vector2.Transform(vector, Matrix.CreateRotationZ(PlayerObject.Mask.Rotation));
            particle.Image.Rect = new Rectangle((int) (PlayerObject.HalfPos.X - result.X),
                                                (int) (PlayerObject.HalfPos.Y - result.Y), 
                                                size,
                                                size);

            ((MoveEffect)particle.Effects[0]).Reset(PlayerObject.Mask.Pos, new Vector2(PlayerObject.HalfPos.X, PlayerObject.HalfPos.Y + 1000), 10000);            
        }

        public override void Update()
        {
            if (Active)
            {
                base.Update();
                if (_timer.Finished)
                {
                    var particle = EngineGlobals.ParticleStorageManager.GetParticle(this, "Player");
                    particle.Activate(5000);
                    InitParticle(particle);
                    _timer.Start(GameGlobals.Random.Next(100, 400), false);
                }
            }
        }
    }
}
