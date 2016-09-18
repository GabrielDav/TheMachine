using Engine.Core;
using Engine.Graphics;
using GameLibrary.Objects;
using GameLibrary.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;
using C = GameLibrary.Arcade.Consts;

namespace GameLibrary.Arcade
{
    public class LevelBuilder
    {
        public ObjectPoolManager Manager;
        private CircleBuilder _circleBuilder;
        private int _depth;
        private Saw _leftSaw;
        private Saw _rghtSaw;
        private Saw _midleSaw;
        private int _level;
        private int _height;
        private int _maxHeight;


        public int Level
        {
            get { return _level; }
        }

        public int Height
        {
            get { return _height; }
        }

        public void Init()
        {
            Manager = new ObjectPoolManager();
            _depth = GameGlobals.Map.Height - C.WallHeight;
            _circleBuilder = new CircleBuilder(Manager);
        }

        private bool RandomBool()
        {
            return GameGlobals.Random.Next(0, 2) == 1;
        }

        public bool RandomProc(int proc)
        {
            return (proc >= GameGlobals.Random.Next(0, 100));
        }

        private bool AttachSpikeShooters(int wallDepth)
        {
            if (_level <= 2)
            {
                return false;
            }

            if (RandomProc(C.LevelConst[_level].ChanceOfSpikeShooter))
            {
                var topY = wallDepth + C.SpikeShooterHalfSize;

                Manager.CreateOrGetSpikeShooter(
                    RandomBool(),
                    topY,
                    C.LevelConst[_level].BulletInterval);

                return true;
            }

            return false;
        }

        private void PlaceSpikes(int depth)
        {
            if (_level > 3 && RandomBool())
            {
                Manager.CreateOrGetCircleSpikes(C.LeftSpike, depth, true);
                Manager.CreateOrGetCircleSpikes(C.RightSpike, depth, false);
            }
            else
            {
                var right = RandomBool();
                Manager.CreateOrGetCircleSpikes(!right ? C.RightSpike : C.LeftSpike, depth, right);
            }
        }

        /// <summary>
        /// places 2 sequanced walls on each side
        /// to fill up level borders
        /// </summary>
        private void PlaceWalls()
        {
            var wallDepth = _depth - C.WallHeight;
            var upperWallDepth = _depth - C.WallHeight * 2;

            if (_level > 1 && RandomProc(C.LevelConst[_level].ChancesOfTrapsOnWall))
            {
                
                if (!AttachSpikeShooters(upperWallDepth))
                {
                    PlaceSpikes(upperWallDepth);
                }
            }

            Manager.CreateOrGetWall(0, wallDepth, false);
            Manager.CreateOrGetWall(0, upperWallDepth, false);
            Manager.CreateOrGetWall(900, wallDepth, true);
            Manager.CreateOrGetWall(900, upperWallDepth, true);
        }

        private void CreateSaws()
        {
            var depth = _depth + 200;
            _leftSaw = new Saw
                {
                    IsActivated = true,
                    Looping = false,
                    Static = false,
                    Rectangle = new RectangleF(C.Left + 5, depth, 280, 280),
                    HandAttached = true,
                    HandRect = new Rectangle(C.Left - 225, depth + 45, 400, 130),
                    LayerDepth = 0.25f
                };
            _leftSaw.EndPos = new Vector2(
                C.Left + 5 + _leftSaw.HalfPos.X,
                0);
            _leftSaw.SawParticleManager = new SawParticleManager();
            _leftSaw.Load("Saw", 0);
            _leftSaw.MoveDirection = MoveDirection.Up;
            _leftSaw.RotationDirection = Direction.Clockwise;
            _leftSaw.IgnoreCulling = true;
            _leftSaw.RotationSpeed = 5;
            _leftSaw.Speed = C.SawsSpeed;
            
            GameGlobals.Physics.AddObjectToQueue(_leftSaw);
            Controller.AddGameObject(_leftSaw);

            _rghtSaw = new Saw
                {
                    IsActivated = true,
                    Looping = false,
                    Static = false,
                    Flip = SpriteEffects.FlipHorizontally,
                    Rectangle = new RectangleF(C.Right - 295, depth, 280, 280),
                    HandAttached = true,
                    HandRect = new Rectangle(C.Right - 190, depth + 45, 400, 130)
                };
            _rghtSaw.EndPos = new Vector2(
                C.Right - 5 - _rghtSaw.HalfPos.X,
                0);
            _rghtSaw.SawParticleManager = new SawParticleManager();
            _rghtSaw.LayerDepth = 0.25f;
            _rghtSaw.Load("Saw", 0);
            _rghtSaw.MoveDirection = MoveDirection.Up;
            _rghtSaw.RotationDirection = Direction.Counterclockwise;
            _rghtSaw.IgnoreCulling = true;
            _rghtSaw.RotationSpeed = 5;
            _rghtSaw.Speed = C.SawsSpeed;
            GameGlobals.Physics.AddObjectToQueue(_rghtSaw);
            Controller.AddGameObject(_rghtSaw);

            _midleSaw = new Saw
                {
                    IsActivated = true,
                    Looping = false,
                    Static = false,
                    Rectangle = new RectangleF(360, depth + 100, 280, 280),
                    EndPos = new Vector2(360, 0),
                    SawParticleManager = new SawParticleManager(),
                    LayerDepth = 0.26f,
                };
            _midleSaw.Load("Saw", 0);
            _midleSaw.MoveDirection = MoveDirection.Up;
            _midleSaw.RotationDirection = Direction.Counterclockwise;
            _midleSaw.IgnoreCulling = true;
            _midleSaw.RotationSpeed = 5;
            _midleSaw.Speed = C.SawsSpeed;
            GameGlobals.Physics.AddObjectToQueue(_midleSaw);
            Controller.AddGameObject(_midleSaw);

            GameGlobals.Physics.CommitQueue();
        }

        public void Start()
        {
            CreateSaws();
            PlaceWalls();

            Manager.CreateSlideWall(90, 250000, 10, 500000);
            Manager.CreateSlideWall(900, 250000, 10, 500000);

            Manager.CreateOrGetWall(0, GameGlobals.Map.Height, false);
            Manager.CreateOrGetWall(900, GameGlobals.Map.Height, true);

            Manager.CreateOrGetWall(0, GameGlobals.Map.Height - C.WallHeight , false);
            Manager.CreateOrGetWall(900, GameGlobals.Map.Height - C.WallHeight, true);

            Manager.CreateOrGetCircle(40, 300, _depth - 80, 3, 1);
            Manager.CreateOrGetCircle(40, 500, _depth - 80, 3, 1);
            Manager.CreateOrGetCircle(40, 700, _depth - 80, 3, 1);

            Manager.CreateOrGetCircle(80, 500, _depth - 240, 3, 1);

            _circleBuilder.PrevHighestCircle = Manager.CreateOrGetCircle(80, 600, _depth - 520, 3, 1);

            Manager.FlushAddedObjects();
        }

        public void BuildLevelChunk()
        {
            PlaceWalls();
            _circleBuilder.PlaceCircles(_level, _height);
            Manager.FlushAddedObjects();
        }

        private void DisposeShrededParts()
        {
            var deletionDepth = _leftSaw.Rectangle.GetRectangle().Y + C.DrawnDistance;
            Manager.DiactivateCircles(deletionDepth);
            Manager.DiactivateWall(deletionDepth);
            Manager.DiactivateDeathBall(deletionDepth);
            Manager.DiactivateSpikes(deletionDepth);
            Manager.DiactivatePowerUp(deletionDepth);
        }

        public void UpdateLevel()
        {
            if (_level < 8)
            {
                _level = _height/C.LevelSwitchInterval;
            }
        }

        public void Update()
        {
            if (_height > _maxHeight)
            {
                _maxHeight = _height;

                GameGlobals.MaxHeight = (int)(_maxHeight/EngineGlobals.PixelsPerMeter);
                GameGlobals.UpdateScore();
            }

            if (GameGlobals.Player.Pos.Y < (_depth - 50))
            {
                _depth -= 240;
               // _water.Speed = 25 + _height/1000;
                if (_midleSaw.Speed < C.SawMaxSpeed)
                {
                    _leftSaw.Speed = C.SawsSpeed + _height/C.SawSpeedDivider;
                    _rghtSaw.Speed = C.SawsSpeed + _height/C.SawSpeedDivider;
                    _midleSaw.Speed = C.SawsSpeed + _height/C.SawSpeedDivider;
                }
                BuildLevelChunk();
                DisposeShrededParts();
                UpdateLevel();
            }

            _height = (GameGlobals.Map.Height - C.WallHeight - (int)GameGlobals.Player.HalfPos.Y);
        }
    }
}
