using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Arcade
{
    public class ObjectPoolManager
    {
        private readonly List<Circle> _circlesPool;
        private readonly List<DecorativeObject> _wallPool;
        private readonly List<InkDot> _inkDotsPool;
        private readonly List<Spike> _spikesPool;
        private readonly List<CircleSpikes> _circleSpikeses; 
        private readonly List<SpikeShooter> _spikeShootersPool;
        private readonly List<DeathBall> _deathBallsPool;
        private readonly List<ArcadePowerUp> _arcadePowerUpsPool; 

        public ObjectPoolManager()
        {
            _circlesPool = new List<Circle>();
            _wallPool = new List<DecorativeObject>();
            _inkDotsPool = new List<InkDot>();
            _spikesPool = new List<Spike>();
            _circleSpikeses = new List<CircleSpikes>();
            _deathBallsPool = new List<DeathBall>();
            _spikeShootersPool = new List<SpikeShooter>();
            _arcadePowerUpsPool = new List<ArcadePowerUp>();
        }

        public void AddElementToTheGame(PhysicalObject element)
        {
            GameGlobals.Physics.AddObjectToQueue(element);
            Controller.AddGameObject(element);
        }

        public void FlushAddedObjects()
        {
            GameGlobals.Physics.CommitQueue();
        }

        #region Circles
        
        public Circle CreateCircle()
        {
            var circle = new Circle();

            return circle;
        }

        public void InitCircle(Circle circle, int r, int x, int y, float rotiationSpeed, int type)
        {
            circle.Diameter = r*2;
            circle.HalfPos = new Vector2(x, y);
            circle.RotationSpeed = rotiationSpeed;
            circle.RotationDirection =/* Direction.Clockwise;*/ Random.RandomTrueFalse() ? Direction.Clockwise : Direction.Counterclockwise;
            circle.Load("Circle", /*type*/ GameGlobals.Random.Next(5));
        }

        public Rectangle CreateOrGetCircle(int r, int x, int y, float rotationSpeed, int typeId)
        {
            var isAnyFree = false;
            Rectangle circleRect = Rectangle.Empty;
            foreach (var circle in _circlesPool)
            {
                if (!circle.InUse)
                {
                    InitCircle(circle, r, x, y, rotationSpeed, typeId);
                    circle.InUse = true;
                    isAnyFree = true;
                    circleRect = circle.Rectangle.GetRectangle();
                    //AddElementToTheGame(circle);
                    break;
                }
            }

            if (!isAnyFree)
            {
                var circle = CreateCircle();
                InitCircle(circle, r, x, y, rotationSpeed, typeId);
                circle.InUse = true;
                circle.Name = "circle_" + _circlesPool.Count;
                _circlesPool.Add(circle);
                circleRect = circle.Rectangle.GetRectangle();
                AddElementToTheGame(circle);
            }

            return circleRect;
        }

        public void DiactivateCircles(int y)
        {
            foreach (var circle in _circlesPool)
            {
                if (circle.InUse)
                {
                    if (circle.HalfPos.Y > y)
                    {
                        circle.InUse = false;
                    }
                }
                
            }
        }

        #endregion

        #region InkDots

        public InkDot CreateInkDot()
        {
            var inkDot = new InkDot();
            inkDot.Load("InkDot", 0);
            
            inkDot.TriggerDistance = 175;
            inkDot.LayerDepth = 0.25f;
            inkDot.IgnoreCollision = true;

            return inkDot;
        }

        public void InitInkDot(InkDot dot, int x, int y, bool bigOne)
        {
            dot.HalfPos = new Vector2(x, y);
            dot.Follow = false;
            dot.MoveSpeed = 200;
            dot.SpeedIncrement = 100f;
            dot.SpeedIncrementTotal = 0;
            dot.Diameter = bigOne ? 20 : 15;

            dot.InkToRecover = bigOne ? 10 : 5;
        }

        public void CreateOrGetInkDot(int x, int y, bool bigOne)
        {
            var isAnyFree = false;
            foreach (var dot in _inkDotsPool)
            {
                if (!dot.IsActivated)
                {
                    InitInkDot(dot, x, y, bigOne);
                    //dot.Load("InkDot", 0);
                    dot.Mask.IsHidden = false;
                    dot.IsActivated = true;
                    isAnyFree = true;
                    break;
                }
            }

            if (!isAnyFree)
            {
                var dot = CreateInkDot();
                InitInkDot(dot, x, y, bigOne);
                dot.Mask.IsHidden = false;
                dot.IsActivated = true;
                dot.Name = "dot_" + _inkDotsPool.Count;
                _inkDotsPool.Add(dot);
                Controller.AddGameObject(dot);
            }
        }

        #endregion

        #region Walls

        public DecorativeObject CreatHandWall()
        {
            var handWall = new DecorativeObject();
            handWall.Mask.LayerDepth = 0.49f;
            handWall.Load("Wall", 0);
            return handWall;
        }

        public void InitWall(DecorativeObject wallHand, int x, int y, bool flip)
        {
            wallHand.IgnoreCollision = true;
            wallHand.Rectangle = new RectangleF(x, y, 100, 240);
            wallHand.Flip = !flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        public void CreateOrGetWall(int x, int y, bool flip)
        {
            var isAnyFree = false;
            foreach (var wall in _wallPool)
            {
                if (!wall.InUse)
                {
                    InitWall(wall, x, y, flip);
                    wall.InUse = true;
                    isAnyFree = true;
                    break;
                }
            }

            if (!isAnyFree)
            {
                var wall = CreatHandWall();
                InitWall(wall, x, y, flip);
                wall.InUse = true;
                wall.Name = "wall_" + _wallPool.Count;
                _wallPool.Add(wall);
                Controller.AddGameObject(wall);
            }
        }

        public void DiactivateWall(int y)
        {
            foreach (var wall in _wallPool)
            {
                if (wall.InUse)
                {
                    if (wall.HalfPos.Y > y)
                    {
                        wall.InUse = false;
                    }
                }

            }
        }

        #endregion

        #region Spikes

        public Spike CreateSpike()
        {
            var spike = new Spike();
            spike.Load("CircleSpikes", 0);
            spike.Rectangle = new RectangleF(0, 0, 100, 20);

            return spike;
        }

        public void InitSpike(Spike spike, int x, int y, bool flip)
        {
            spike.HalfPos = new Vector2(x, y);
            spike.Flip = !flip ? SpriteEffects.None : SpriteEffects.FlipVertically;
        }

        public void CreateOrGetSpike(int x, int y, bool flip)
        {
            var isAnyFree = false;
            foreach (var spike in _spikesPool)
            {
                if (!spike.InUse)
                {
                    InitSpike(spike, x, y, flip);
                    spike.InUse = true;
                    isAnyFree = true;
                    break;
                }
            }

            if (!isAnyFree)
            {
                var spike = CreateSpike();
                InitSpike(spike, x, y, flip);
                spike.InUse = true;
                spike.Name = "spike_" + _spikesPool.Count;
                _spikesPool.Add(spike);
                AddElementToTheGame(spike);
            }
        }

        public void DiactivateSpikes(int y)
        {
            foreach (var spike in _spikesPool)
            {
                if (spike.InUse)
                {
                    if (spike.HalfPos.Y > y)
                    {
                        spike.InUse = false;
                    }
                }

            }
        }
 
        #endregion

        #region CircleSpikes

        public CircleSpikes CreateCircleSpikes()
        {
            var spike = new CircleSpikes();
            spike.Load("CircleSpikes", 0);
            spike.Rectangle = new RectangleF(0, 0, 100, 100);

            return spike;
        }

        public void InitCircleSpikes(CircleSpikes spike, int x, int y, bool flip)
        {
            spike.HalfPos = new Vector2(x, y);
            spike.Rotation = flip ? 0 : 180;
        }

        public void CreateOrGetCircleSpikes(int x, int y, bool flip)
        {
            var isAnyFree = false;
            foreach (var spike in _circleSpikeses)
            {
                if (!spike.InUse)
                {
                    InitCircleSpikes(spike, x, y, flip);
                    spike.InUse = true;
                    isAnyFree = true;
                    break;
                }
            }

            if (!isAnyFree)
            {
                var spike = CreateCircleSpikes();
                InitCircleSpikes(spike, x, y, flip);
                spike.InUse = true;
                spike.Name = "spike_" + _circleSpikeses.Count;
                _circleSpikeses.Add(spike);
                AddElementToTheGame(spike);
            }
        }

        public void DiactivateCircleSpikes(int y)
        {
            foreach (var spike in _circleSpikeses)
            {
                if (spike.InUse)
                {
                    if (spike.HalfPos.Y > y)
                    {
                        spike.InUse = false;
                    }
                }

            }
        }

        #endregion

        #region DeathBall

        public DeathBall CreateDeathBall()
        {
            var deathBall = new DeathBall();
            deathBall.Load("DeathBall", 0);

            return deathBall;
        }

        public void InitDeathBall(DeathBall deathBall, int r, int x, int y, int rotiationSpeed, int delay)
        {
            deathBall.Diameter = r * 2;
            deathBall.HalfPos = new Vector2(x, y);
            deathBall.RotationSpeed = rotiationSpeed;
            deathBall.SpikesActivationEffectTime = 500;
            deathBall.ActivationTime = delay;
            deathBall.Duration = 1000;
            
            deathBall.Reset();
        }

        public Rectangle CreateOrGetDeathBall(int r, int x, int y, int rotationSpeed, int delay)
        {
            var isAnyFree = false;
            Rectangle deathBallRect = Rectangle.Empty;
            foreach (var deathBall in _deathBallsPool)
            {
                if (!deathBall.InUse)
                {
                    InitDeathBall(deathBall, r, x, y, rotationSpeed, delay);
                    deathBall.InUse = true;
                    isAnyFree = true;
                    deathBallRect = deathBall.Rectangle.GetRectangle();
                    break;
                }
            }

            if (!isAnyFree)
            {
                var deathBall = CreateDeathBall();
                InitDeathBall(deathBall, r, x, y, rotationSpeed, delay);
                deathBall.InUse = true;
                deathBall.Name = "deathBall_" + _deathBallsPool.Count;
                _deathBallsPool.Add(deathBall);
                deathBallRect = deathBall.Rectangle.GetRectangle();
                AddElementToTheGame(deathBall);
            }

            return deathBallRect;
        }

        public void DiactivateDeathBall(int y)
        {
            foreach (var deathBall in _deathBallsPool)
            {
                if (deathBall.InUse)
                {
                    if (deathBall.HalfPos.Y > y)
                    {
                        deathBall.InUse = false;
                        deathBall.ActivationTime = 0;
                        deathBall.Deadly = false;
                        deathBall.Duration = 0;
                        deathBall.SpikesActivationEffectTime = 0;
                        deathBall.StartupTime = 0;
                    }
                }

            }
        }

        #endregion

        public void CreateSlideWall(int x, int y, int width, int height)
        {
            var wall = new WallSlide();
            wall.Friction = 0.75f;
            wall.Load("Plane", 0);
            wall.Rectangle = new RectangleF(x, y, width, height);
            
            AddElementToTheGame(wall);
        }

        #region PowerUp

        public ArcadePowerUp CreatePowerUp()
        {
            var powerUp = new ArcadePowerUp
            {
                TriggerDistance = 100,
                MoveSpeed = 800,
            };

            return powerUp;
        }

        public void InitPowerUp(ArcadePowerUp powerUp, int x, int y, int type)
        {
            if (type == 1)
            {
                powerUp.PowerUpType = PowerUpType.InkCollect;
            }
            else if (type == 2)
            {
                powerUp.PowerUpType = PowerUpType.TimeWarp;
            }
            else
            {
                throw new Exception("Wrong powerup type in arcade!");
            }

            powerUp.Rectangle = new RectangleF(x, y, 40, 40);
            powerUp.Load("PowerUp", type);
            powerUp.IgnoreCollision = true;
        }

        public void CreateOrGetPowerUp(int x, int y, int type)
        {
            var isAnyFree = false;
            foreach (var powerUp in _arcadePowerUpsPool)
            {
                if (!powerUp.InUse)
                {
                    powerUp.InUse = true;
                    InitPowerUp(powerUp, x, y, type);
                    isAnyFree = true;
                    break;
                }
            }

            if (!isAnyFree)
            {
                var powerUp = CreatePowerUp();
                powerUp.InUse = true;
                InitPowerUp(powerUp, x, y, type);
                powerUp.Name = "powerUp_" + _arcadePowerUpsPool.Count;
                _arcadePowerUpsPool.Add(powerUp);
                AddElementToTheGame(powerUp);
            }
        }

        public void DiactivatePowerUp(int y)
        {
            foreach (var powerUp in _arcadePowerUpsPool)
            {
                if (powerUp.InUse)
                {
                    if (powerUp.HalfPos.Y > y)
                    {
                        powerUp.InUse = false;
                    }
                }

            }
        }

        #endregion

        #region SpikeShooter

        public SpikeShooter CreateSpikeShooter()
        {
            var spikeShooter = new SpikeShooter();
            spikeShooter.Rectangle = new RectangleF(0, 0, 110, 90);
            spikeShooter.IgnoreCollision = true;
            
            return spikeShooter;
        }

        public void InitSpikeShooter(SpikeShooter spikeShooter, bool left, int y, int shootingInterval)
        {
            spikeShooter.Mask.LayerDepth = 0.45f;
            spikeShooter.BulletSpeed = 7;
            spikeShooter.ShotInterval = shootingInterval;
            spikeShooter.IsActivated = true;
            spikeShooter.Orientation = left ? Orientation.Top : Orientation.Bottom;
            spikeShooter.HalfPos = new Vector2(left ? 945 : 55, y);
            spikeShooter.TargetPos = new Vector2(left ? 100 : 900, y);
            spikeShooter.Load("SpikeShooter", 0);
        }

        public void CreateOrGetSpikeShooter(bool left, int y, int shootingInterval)
        {
            var isAnyFree = false;
            foreach (var spikeShooter in _spikeShootersPool)
            {
                if (!spikeShooter.InUse)
                {
                    InitSpikeShooter(spikeShooter, left, y, shootingInterval);
                    spikeShooter.InUse = true;
                    isAnyFree = true;
                    break;
                }
            }

            if (!isAnyFree)
            {
                var spikeShooter = CreateSpikeShooter();
                InitSpikeShooter(spikeShooter, left, y, shootingInterval);
                spikeShooter.InUse = true;
                spikeShooter.Name = "spikeShooter_" + _spikeShootersPool.Count;
                _spikeShootersPool.Add(spikeShooter);

                Controller.AddGameObject(spikeShooter);
            }
        }

        public void DiactivateSpikeShooter(int y)
        {
            foreach (var spikeShooter in _spikeShootersPool)
            {
                if (spikeShooter.InUse)
                {
                    if (spikeShooter.HalfPos.Y > y)
                    {
                        spikeShooter.Diactivate();
                        
                    }
                }

            }
        }

        #endregion
    }
}
