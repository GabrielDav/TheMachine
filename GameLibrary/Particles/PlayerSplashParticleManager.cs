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
    public enum SpashType
    {
        CircleSplash,
        WallSplash
    }

#if EDITOR
    [TypeConverter(typeof(ExpandableObjectConverter))]
#endif
    public class PlayerSplashParticleManager : BaseParticleManager
    {
        //public const int SplashOffset = 10;

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public Player PlayerObject;

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public SpashType Type;

        private string _splash;
        private string _splashWall;

        public PlayerSplashParticleManager(PhysicalObject player)
        {
            PlayerObject = player as Player;
            if (GameGlobals.HardcoreMode)
            {
                _splash = "Splash_Red";
                _splashWall = "SplashWall_Red";
            }
            else
            {
                _splash = "Splash";
                _splashWall = "SplashWall";
            }
        }

        public override Particle CreateNewParticle()
        {
            Image img = null;
            if (Type == SpashType.CircleSplash)
            {
                img = new Image(EngineGlobals.Resources.Textures[_splash][0]);
            }
            else if (Type == SpashType.WallSplash)
            {
                img = new Image(EngineGlobals.Resources.Textures[_splashWall][0]);
            }
            if (img != null)
            {
                img.Owner = this;
                Controller.AddObject(img);

                return new Particle(img,
                                    new List<IEffect>
                                        {
                                            new ColorEffect(img, Color.White, 0)
                                        });
            }

            throw  new Exception("image is null");
        }

        public override void LoadDefault()
        {
            throw new NotImplementedException();
        }

        protected override void InitParticle(Particle particle)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            PlayerObject = null;
        }

        public void InitParticle(Particle particle, FixPosition spalshDirection, int offset)
        {
            if (Type == SpashType.CircleSplash)
            {
                particle.Image.Rect = new Rectangle((int) PlayerObject.HalfPos.X, (int) PlayerObject.HalfPos.Y, 100, 100);
                particle.Image.Orgin = particle.Image.OriginCenter();
                particle.Image.LayerDepth = 0.75f;
                particle.Image.Rotation = MathHelper.ToRadians(GameGlobals.Random.Next(0, 360));
                particle.Image.Color = Color.Wheat;
                ((ColorEffect) particle.Effects[0]).Reset(new Color(255, 255, 255, 0), 3200);
            }
            else if (Type == SpashType.WallSplash)
            {
                particle.Image.LoadTexture(EngineGlobals.Resources.Textures[_splashWall][GameGlobals.Random.Next(0, 2)]);
                particle.Image.Orgin = particle.Image.OriginCenter();


                switch (spalshDirection)
                {
                    case FixPosition.Left:
                        particle.Image.Rect = new Rectangle((int)PlayerObject.HalfPos.X - offset, (int)PlayerObject.HalfPos.Y, 112, 56);
                        particle.Image.Rotation = MathHelper.ToRadians(270f);
                        break;
                    case FixPosition.Right:
                        particle.Image.Rect = new Rectangle((int)PlayerObject.HalfPos.X + offset, (int)PlayerObject.HalfPos.Y, 112, 56);
                        particle.Image.Rotation = MathHelper.ToRadians(90f);
                        break;
                    case FixPosition.Top:
                        particle.Image.Rect = new Rectangle((int)PlayerObject.HalfPos.X, (int)PlayerObject.HalfPos.Y - offset, 112, 56);
                        particle.Image.Rotation = MathHelper.ToRadians(0f);
                        break;
                    case FixPosition.Bottom:
                        particle.Image.Rect = new Rectangle((int)PlayerObject.HalfPos.X, (int)PlayerObject.HalfPos.Y + offset, 112, 56);
                        particle.Image.Rotation = MathHelper.ToRadians(180f);
                        break;
                }
                particle.Image.Orgin = particle.Image.OriginCenter();
                particle.Image.LayerDepth = 0.75f;

                particle.Image.Color = Color.Wheat;
                ((ColorEffect)particle.Effects[0]).Reset(new Color(255, 255, 255, 0), 3200);
            }
        }

        public void CreateSpash(SpashType type, FixPosition splashDirection, int offset)
        {
            if (Enabled)
            {
                Type = type;
                Particle particle = null;
                if (Type == SpashType.CircleSplash)
                {
                    particle = EngineGlobals.ParticleStorageManager.GetParticle(this, "PlayerCircleSplash");
                }
                else if (Type == SpashType.WallSplash)
                {
                    particle = EngineGlobals.ParticleStorageManager.GetParticle(this, "PlayerWallSplash");
                }

                if (particle != null)
                {
                    particle.Activate(3200);
                    InitParticle(particle, splashDirection, offset);
                }
                else
                {
                    Enabled = false;
                }
            }
        }
    }
}
