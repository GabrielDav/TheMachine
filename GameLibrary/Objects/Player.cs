using System;
using System.Diagnostics;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using GameLibrary.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

#if EDITOR
using System.ComponentModel;
#endif

namespace GameLibrary.Objects
{
    public class Player : CirclePhysicalObject
    {
        #region Public
        #if EDITOR
        [Browsable(false)]
        #endif
        [ContentSerializerIgnore]
        public FixPosition FixPosition;
        
        //[ContentSerializerIgnore]
        //public Image _debugImg;

        /*
        [ContentSerializerIgnore]
        private Image _markImage;

        [ContentSerializerIgnore] 
        private Timer _markTimer;
         */
        

        [ContentSerializerIgnore]
        public const float JumpAngle = 60;

        [ContentSerializerIgnore]
        public static float MaxMovePoints = 200;

        [ContentSerializerIgnore]
        public PowerUpType CurrentPowerUp = PowerUpType.None;

        protected PlayerOutOfInkParticleManager _outOfInkParticleManager;

        protected DoubleJumpParticle _doubleJumpParticle;

        //[ContentSerializerIgnore]
        #if EDITOR
        //[Browsable(false)]
        #endif
        public float CurrentMovePoints
        {
            get { return _currentMovePoints; }

            set
            {
                _currentMovePoints = value;

                if (_currentMovePoints > MaxMovePoints)
                {
                    _currentMovePoints = MaxMovePoints;
                }
                else if (_currentMovePoints <= 0)
                {
                    _currentMovePoints = 0;
                    GameGlobals.GameOver = true;
                    EngineGlobals.TimeSpeed = 1f;
                    Mask.IsHidden = true;
                    GameGlobals.DeathsPerLevel++;
                    GameGlobals.TotalDeaths++;
#if !EDITOR
                    ((GameScreen) EngineGlobals.ScreenManager.CurrentScreen).OutOfInk = true;
#endif
                    _outOfInkParticleManager.Update();
                }
            }
        }

        [ContentSerializerIgnore]
#if EDITOR
        [Browsable(false)]
#endif
        public float TotalDistanceTravelled;

        [ContentSerializerIgnore]
#if EDITOR
        [Browsable(false)]
#endif
        public bool IsInAir
        {
            get { return _inAir; }
        }

        #if EDITOR
        [PropertyOrder(20)]
        #endif
        public float JumpSpeed { get; set; }

        [ContentSerializerIgnore]
        public Vector2 StartPosition;

        #if EDITOR
        [PropertyOrder(21)]
        #endif
        public float SlideSpeed { get; set; }

#if EDITOR
        [PropertyOrder(22)]
#endif
        public PlayerTrailParticleManager TrailParticleManager { get; set; }

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public override float RotationSpeed
        {
            get { return base.RotationSpeed; }
            set { base.RotationSpeed = value; }
        }

#if EDITOR
        [Browsable(false)]
#endif
        [ContentSerializerIgnore]
        public override Direction RotationDirection
        {
            get { return base.RotationDirection; }
            set { base.RotationDirection = value; }
        }

        #endregion

        #region Flags

        protected bool _inAir;
        protected bool _onCircle;
        protected bool _onJumpSpot;
        protected bool _onPlane;
        protected bool _onWall;
        protected bool _onSlideWall;
        protected bool _diagonalCollision;

        #endregion

        #region Debug

        public static bool Diagonal;
        public static int V;
        public static int H;

        #endregion

        #region Private

        public PhysicalObject CurrentObject { get; protected set; }
        protected float _circleStartRotation;
        protected float _currentMovePoints;
        protected float _worldRotation;
        protected float _currentWallFriction;
        protected float _angle;
        protected PlayerSplashParticleManager SplashParticleManager { get; set; }
        protected const int TimeWarpTime = 20000;
        protected const int InkDotCollectionPowerupDistance = 300;
        protected Timer _timeWarpTimer;
        private int _cameraSpeed;
        private int _cameraOffset;

        private string _deadthSplash;

        #endregion

        public Player()
        {
            Animated = true;
            _inAir = true;
            
            TypeId = GameObjectType.Player.ToString();
            Init();

            if (GameGlobals.HardcoreMode)
            {
                _deadthSplash = "Splash_Death_Red";
            }
            else
            {
                _deadthSplash = "Splash_Death";
            }
            //_debugImg = new Image(new GameTexture("selection"));
        }

        #region Jump Logic

        private bool AbovePlayer(int y)
        {
            return y <= HalfPos.Y;
        }

        private bool RightOfPLayer(int x)
        {
            if (x >= HalfPos.X)
            {
                return true;
            }

            return false;
        }

        private bool JumpFromRight(int angle, int x)
        {
            if (RightOfPLayer(x))
            {
                if (angle > 80)
                {
                    angle = 80;
                }
                else if (angle < -80)
                {
                    angle = -80;
                }

                _angle = angle;
                Rotation = (int)_angle;

                return true;
            }

            return false;
        }

        private bool JumpFromLeft(int angle, int x)
        {
            if (!RightOfPLayer(x))
            {
                if (angle < 100 && angle > 0)
                {
                    angle = 100;
                }
                else if(angle > -100 && angle < 0)
                {
                    angle = -100;
                }

                _angle = angle;
                Rotation = (int) _angle;

                return true;
            }

            return false;
        }

        private bool JumpFromTop(int angle, int y)
        {
            if (!AbovePlayer(y))
            {
                if (angle < -170)
                {
                    angle = -170;
                }
                else if (angle > -10)
                {
                    angle = -10;
                }
                _angle = angle;
                Rotation = (int)_angle;

                return true;
            }

            return false;
        }

        private bool JumpFromBottom(int angle, int y)
        {
            if (AbovePlayer(y))
            {
                if (angle < 10)
                {
                    angle = 10;
                }
                else if (angle > 170)
                {
                    angle = 170;
                }
                _angle = angle;
                Rotation = (int) _angle;

                return true;
            }

            return false;
        }

        private bool JumpFromWall(int x, int y)
        {
            var deltaX = -(HalfPos.X - x);
            var deltaY = HalfPos.Y - y;

            var angle = (int)MathHelper.ToDegrees((float)Math.Atan2(deltaY, deltaX));

            switch (FixPosition)
            {
                case FixPosition.Left:
                    return JumpFromLeft(angle, x);
                case FixPosition.Right:
                    return JumpFromRight(angle, x);
                    
            }

            return false;
        }

        private bool DiagonalJump(int x, int y, int angle)
        {
            switch (FixPosition)
            {
                case FixPosition.TopLeft:
                    if (JumpFromLeft(angle, x) || JumpFromBottom(angle, y))
                    {
                        HalfPos = new Vector2(HalfPos.X - HalfSize.X, HalfPos.Y - HalfSize.Y);
                        return true;
                    }
                    break;
                case FixPosition.TopRight:
                    if (JumpFromRight(angle, x) || JumpFromBottom(angle, y))
                    {
                        HalfPos = new Vector2(HalfPos.X + HalfSize.X, HalfPos.Y - HalfSize.Y);
                        return true;
                    }
                    break;
                case FixPosition.BottomRight:
                    if (JumpFromRight(angle, x) || JumpFromBottom(angle, y))
                    {
                        HalfPos = new Vector2(HalfPos.X + HalfSize.X, HalfPos.Y + HalfSize.Y);
                        return true;
                    }
                    break;
                case FixPosition.BottomLeft:
                    if (JumpFromLeft(angle, x) || JumpFromBottom(angle, y))
                    {
                        HalfPos = new Vector2(HalfPos.X - HalfSize.X, HalfPos.Y + HalfSize.Y);
                        return true;
                    }
                    break;
            }

            return false;
        }

        private bool HorizontalVerticalJump(int x, int y, int angle)
        {
            switch (FixPosition)
            {
                case FixPosition.Left:
                    if (JumpFromLeft(angle, x))
                    {
                        HalfPos = new Vector2(HalfPos.X - HalfSize.X, HalfPos.Y);
                        return true;
                    }
                    break;
                case FixPosition.Right:
                    if (JumpFromRight(angle, x))
                    {
                        HalfPos = new Vector2(HalfPos.X + HalfSize.X, HalfPos.Y);
                        return true;
                    }
                    break;
                case FixPosition.Top:
                    if (JumpFromBottom(angle, y))
                    {
                        HalfPos = new Vector2(HalfPos.X, HalfPos.Y - HalfSize.Y);
                        return true;
                    }
                    break;
                case FixPosition.Bottom:
                    if (JumpFromTop(angle, y))
                    {
                        HalfPos = new Vector2(HalfPos.X, HalfPos.Y + HalfSize.Y);
                        return true;
                    }
                    break;
            }

            return false;
        }

        private bool JumpFromTile(int x, int y)
        {
            var deltaX = -(HalfPos.X - x);
            var deltaY = HalfPos.Y - y;

            var angle = (int)MathHelper.ToDegrees((float)Math.Atan2(deltaY, deltaX));

            bool commitJump = _diagonalCollision
                                  ? DiagonalJump(x, y, angle)
                                  : HorizontalVerticalJump(x, y, angle);

            return commitJump;
        }

        private void UnrestrictedJump(int x, int y)
        {
            var deltaX = -(HalfPos.X - x);
            var deltaY = HalfPos.Y - y;

            var angle = (int)MathHelper.ToDegrees((float)Math.Atan2(deltaY, deltaX));

            _angle = angle;
            Rotation = (int)_angle;
        }

        private void JumpFromJumpSpot(int x, int y)
        {
            UnrestrictedJump(x, y);

            ((JumpSpot)CurrentObject).PlayerJumpsOff();
        }

        private float JumpFromCircle()
        {
            _angle = -MathHelper.ToDegrees(_worldRotation);

            Rotation = -((int)_angle + 90);
            HalfPos =
                new Vector2(
                    (float)(CurrentObject.HalfPos.X + CurrentObject.HalfSize.X * Math.Cos(_worldRotation)),
                    (float)(CurrentObject.HalfPos.Y + CurrentObject.HalfSize.X * Math.Sin(_worldRotation)));
            EngineGlobals.Camera2D.FollowObject(GameGlobals.Player);
            var circle = CurrentObject as Circle;
            Debug.Assert(circle != null, "circle != null");

            //kolkas suolis nuo rutulio nepadidina greicio
            return 0; // circle.RotationSpeed / 3; 
        }

        public void CommitJump(float jumpSpeed, float angle)
        {
            _inAir = true;
            TrailParticleManager.Enabled = true;
            if (FixPosition == FixPosition.Top)
            {
                _sprite.SetAnimation(AnimationNames.Air);
                _sprite.Flip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                _sprite.SetAnimation(AnimationNames.Air);
                _sprite.Flip = SpriteEffects.None;
            }

            RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);

            GameGlobals.Physics.PushObject(this, jumpSpeed, angle);
            _onCircle = false;
            _onPlane = false;
            _onWall = false;
        }

        /*
        private void UpdateMarker()
        {
            if (_markTimer.Finished)
            {
                _markImage.IsHidden = true;
            }
            else
            {
                _markTimer.Update();
            }
        }
         */
         
        
        public void Jump(Point pressPoint)
        {
            /*
            _markImage.Pos = new Vector2(pressPoint.X - 20, pressPoint.Y - 20);
            _markImage.IsHidden = false;
            _markTimer.Start(350);
             */
             

            var pressPointInWorld = EngineGlobals.Camera2D.ViewToWorld(pressPoint.ToVector());
            var jumpSpeed = JumpSpeed;

            if (!_inAir)
            {
                bool commitJump;
                if (_onWall)
                {
                    commitJump = JumpFromWall((int) pressPointInWorld.X, (int) pressPointInWorld.Y);
                }
                else if (_onCircle)
                {
                    jumpSpeed += JumpFromCircle();
                    commitJump = true;
                }
                else if (_onJumpSpot)
                {
                    _onJumpSpot = false;
                    JumpFromJumpSpot((int) pressPointInWorld.X, (int) pressPointInWorld.Y);
                    commitJump = true;
                }
                else if (_onPlane)
                {
                    commitJump = JumpFromTile((int) pressPointInWorld.X, (int) pressPointInWorld.Y);
                }
                else
                {
                    throw new Exception("Unrecognized jump surface!");
                }

                if (commitJump)
                {
                    GameGlobals.LevelJumps++;
                    CommitJump(jumpSpeed, _angle);
                }
            }
            else
            {
                switch (CurrentPowerUp)
                {
                        case PowerUpType.DoubleJump:
                        DoubleJump((int)pressPointInWorld.X, (int)pressPointInWorld.Y, jumpSpeed);
                        break;
                }

                if (GameGlobals.ArcadeMode)
                {
                    DoubleJumpSimple((int) pressPointInWorld.X, (int) pressPointInWorld.Y, jumpSpeed);
                }
            }
        }

        #endregion

        #region PowerUps Logic
        
        private void DoubleJump(int x, int y, float jumpSpeed)
        {

                CurrentPowerUp = PowerUpType.None;
                UnrestrictedJump(x, y);
                GameGlobals.Physics.PushObject(this, jumpSpeed, _angle);
        }

        private void DoubleJumpSimple(int x, int y, float jumpSpeed)
        {
            UnrestrictedJump(x, y);
            GameGlobals.Physics.PushObject(this, jumpSpeed, _angle);
            _doubleJumpParticle.Reset();
            CurrentMovePoints -= 20;
        }

        public void StartTimeWarp()
        {
            EngineGlobals.TimeSpeed = GameGlobals.ArcadeMode ? 0.5f : 0.2f;
            CurrentPowerUp = GameGlobals.ArcadeMode ? PowerUpType.TimeWarp : PowerUpType.None;
            _timeWarpTimer.Start(TimeWarpTime);
            EngineGlobals.SoundManager.Play(
                "time_slowdown",
                "slowdown",
                this,
                1.0f,
                false,
                1.0f,
                1.0f);
        }


        public void StopTimeWarpPowerup()
        {
            EngineGlobals.SoundManager.Play(
                        "time_backtonormal",
                        "backtonormal",
                        this,
                        1.0f,
                        false,
                        1.0f,
                        1.0f);
            EngineGlobals.TimeSpeed = 1f;
            if (GameGlobals.ArcadeMode)
                CurrentPowerUp = PowerUpType.None;
            if (_timeWarpTimer.Ticking)
                _timeWarpTimer.Stop();
        }


       public void UsePowerUp(Point p)
        {
            if (GameGlobals.ArcadeMode)
                return;
            if (!_inAir)
            {
                CurrentPowerUp = PowerUpType.None;
                Jump(p);
                return;
            }
            switch (CurrentPowerUp)
            {
                case PowerUpType.TimeWarp:
                    StartTimeWarp();
                    break;
                case PowerUpType.InkCollect:
                    foreach (var physicalObject in GameGlobals.Map.GameObjects)
                    {
                        if (physicalObject is InkDot && !(physicalObject is PowerUp) && physicalObject.IsActivated)
                        {
                            if (Vector2.Distance(physicalObject.HalfPos, HalfPos) < InkDotCollectionPowerupDistance)
                            {
                                EngineGlobals.SoundManager.Play(
                                    "player_collectink",
                                    "collectink",
                                    this,
                                    1.0f,
                                    false,
                                    1.0f,
                                    1.0f);
                                ((InkDot) physicalObject).Follow = true;
                            }
                        }
                    }
                    break;
                case PowerUpType.DoubleJump:
                    break;
                case PowerUpType.Shield:
                    break;
                default:
                    throw new Exception("Undefined PowerUp type: '" + CurrentPowerUp + "'");
            }
        }

        /* 
        public void UseArcadePowerup(ArcadePowerUp powerUp)
        {
            if (!GameGlobals.ArcadeMode)
                return;
            switch (CurrentPowerUp)
            {
                case PowerUpType.TimeWarp:
                    EngineGlobals.TimeSpeed = 0.2f;
                    CurrentPowerUp = PowerUpType.None;
                    _timeWarpTimer.Start(powerUp.Duration);
                    EngineGlobals.SoundManager.Play(
                        "time_slowdown",
                        "slowdown",
                        this,
                        1.0f,
                        false,
                        1.0f,
                        1.0f);
                    break;
                case PowerUpType.InkCollect:
                    foreach (var physicalObject in GameGlobals.Map.GameObjects)
                    {
                        if (physicalObject is InkDot && !(physicalObject is PowerUp) && physicalObject.IsActivated)
                        {
                            if (Vector2.Distance(physicalObject.HalfPos, HalfPos) < InkDotCollectionPowerupDistance)
                            {
                                EngineGlobals.SoundManager.Play(
                                    "player_collectink",
                                    "collectink",
                                    this,
                                    1.0f,
                                    false,
                                    1.0f,
                                    1.0f);
                                ((InkDot)physicalObject).Follow = true;
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("Undefined PowerUp type: '" + CurrentPowerUp + "'");
            }
        }
        */

        #endregion

        #region Collision Logic

        private void StickCorrner(CollisionResult currentCollision)
        {
            HalfPos = currentCollision.NewPos;

            _sprite.SetAnimation(AnimationNames.Circle);
            RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);
        }

        private void StickRight(CollisionResult currentCollision)
        {
            _sprite.Flip = EngineGlobals.Gravity.Y > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (_onWall)
            {
                HalfPos = new Vector2(
                currentCollision.Target.HalfPos.X + currentCollision.Target.HalfSize.X + HalfSize.X,
                currentCollision.NewPos.Y);
                

                _sprite.SetAnimation(AnimationNames.Wall);
                
                if (EngineGlobals.Gravity.Y > 0)
                    RescaleBySize(20, 50, ScaleAttachSide.BottomLeft);
                else
                    RescaleBySize(20, 50, ScaleAttachSide.BottomRight);
            }
            else
            {
                HalfPos = new Vector2(
                currentCollision.Target.HalfPos.X + currentCollision.Target.HalfSize.X,
                currentCollision.NewPos.Y);

                _sprite.SetAnimation(AnimationNames.Circle);
                RescaleByRatio(RescaleType.Horizontal, ScaleAttachSide.Center);
            }
        }

        private void StickLeft(CollisionResult currentCollision)
        {
            _sprite.Flip = EngineGlobals.Gravity.Y > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (_onWall)
            {
                HalfPos = new Vector2(
               currentCollision.Target.HalfPos.X - currentCollision.Target.HalfSize.X - HalfSize.X,
               currentCollision.NewPos.Y);

                _sprite.SetAnimation(AnimationNames.Wall);

                if (EngineGlobals.Gravity.Y > 0)
                    RescaleBySize(20, 50, ScaleAttachSide.BottomRight);
                else
                    RescaleBySize(20, 50, ScaleAttachSide.BottomLeft);
            }
            else
            {
                HalfPos = new Vector2(
               currentCollision.Target.HalfPos.X - currentCollision.Target.HalfSize.X,
               currentCollision.NewPos.Y);

                _sprite.SetAnimation(AnimationNames.Circle);
                RescaleByRatio(RescaleType.Horizontal, ScaleAttachSide.Center);
            }
        }

        private void StickBottom(CollisionResult currentCollision)
        {
            _sprite.Flip = SpriteEffects.FlipVertically;
            if (_onWall)
            {
                HalfPos = new Vector2(
                    currentCollision.NewPos.X,
                    currentCollision.Target.HalfPos.Y + currentCollision.Target.HalfSize.Y + HalfSize.Y);

                _sprite.SetAnimation(AnimationNames.Ground);
                
                RescaleByRatio(RescaleType.Horizontal, ScaleAttachSide.TopCenter);
            }
            else
            {
                HalfPos = new Vector2(
                    currentCollision.NewPos.X,
                    currentCollision.Target.HalfPos.Y + currentCollision.Target.HalfSize.Y);

                _sprite.SetAnimation(AnimationNames.Circle);

                RescaleByRatio(RescaleType.Horizontal, ScaleAttachSide.BottomCenter);
            }
        }

        private void StickTop(CollisionResult currentCollision)
        {
            _sprite.Flip = SpriteEffects.None;
            if (_onWall)
            {
                HalfPos = new Vector2(
                currentCollision.NewPos.X,
                currentCollision.Target.HalfPos.Y - currentCollision.Target.HalfSize.Y - HalfSize.Y);

                _sprite.SetAnimation(AnimationNames.Ground);
                
                RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);
            }
            else
            {
                HalfPos = new Vector2(
                currentCollision.NewPos.X,
                currentCollision.Target.HalfPos.Y - currentCollision.Target.HalfSize.Y);

                _sprite.SetAnimation(AnimationNames.Circle);
                RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);
            }
        }

        public override CollisionResponce Collide(CollisionResult collision)
        {
            var currentCollision = collision;
            if (CurrentPowerUp != PowerUpType.None && !GameGlobals.ArcadeMode)
                CurrentPowerUp = PowerUpType.None;
            if (currentCollision.Target is Spike)
            {
                Die();
            }
            else if (currentCollision.Target is SpikeBullet || currentCollision.Target is SpikeShooter)
            {
                Die();
            }
            else if (currentCollision.Target is TrapBtn)
            {
                _inAir = false;
                ((TrapBtn)currentCollision.Target).TurnOn();
                return StickOnTile(currentCollision);
            }
            else if (currentCollision.Target is Plane)
            {
                _inAir = false;
                return StickOnTile(currentCollision);
            }
            else if (currentCollision.Target is Tile)
            {
                _inAir = false;
                return StickOnTile(currentCollision);
            }
            else if (currentCollision.Target is WallSlide)
            {
                _inAir = false;
                return StickOnSlideWall(currentCollision);
            }
            else if (currentCollision.Target is Saw)
            {
                Die();
            }
            else if (currentCollision.Target is JumpSpot)
            {
                if (CurrentObject != null && CurrentObject.Equals(currentCollision.Target) || ((JumpSpot)currentCollision.Target).ReleaseMode > 0)
                {
                    return CollisionResponce.GoTrought;
                }

                _inAir = false;
               
                StickOnJumpSpot(currentCollision);
            }
            /*else if (currentCollision.Target is LevelEnd)
            {
                _inAir = false;
                GameGlobals.GameOver = true;
                var dist = currentCollision.Target.HalfPos - HalfPos;
                Mask.Rotation = (float)Math.Atan2(dist.Y, dist.X) - MathHelper.PiOver2;
                #if !EDITOR
                GameGlobals.LevelComplete = true;
                
                GameGlobals.Game.NextLevel();
                #endif

            }*/
            else if (currentCollision.Target is Circle)
            {
                if (CurrentObject != null && CurrentObject.Equals(currentCollision.Target))
                {
                    return CollisionResponce.GoTrought;
                }

                _inAir = false;
                StickOnCircle(currentCollision);
            }
            else if (currentCollision.Target is DeathBall.Spikes)
            {
                Die();
            }
            else
            {
                throw new Exception("Unidentified object collision!");
            }

            return base.Collide(collision);
        }

        public void Die()
        {
            _inAir = false;
            ShowDeathAnimation();
            EngineGlobals.TimeSpeed = 1f;
            GameGlobals.TotalDeaths++;
            GameGlobals.DeathsPerLevel++;
#if !EDITOR
            ((GameScreen)EngineGlobals.ScreenManager.CurrentScreen).QueueGameOver();
#else
            GameGlobals.GameOver = true;
#endif

        }

        public void ShowDeathAnimation()
        {
            if (!GameGlobals.GameOver)
            {
                #if WINDOWS_PHONE
                if (GameGlobals.Vibrate)
                {
                    GameGlobals.VibrateController.Start(TimeSpan.FromMilliseconds(300));
                }
                #endif

                Mask.IsHidden = true;

                var img = new Image(EngineGlobals.Resources.Textures[_deadthSplash][0],
                                    new Rectangle((int) HalfPos.X, (int) HalfPos.Y, 450, 400));
                EngineGlobals.SoundManager.Play(
                    "player_death",
                    "death",
                    this,
                    1.0f,
                    false,
                    1.0f,
                    1.0f);

                img.Orgin = img.OriginCenter();
                img.Rotation = Mask.Rotation;
                img.Owner = this;
                Controller.AddObject(img);
            }
        }

        private void StickOnFlatSurface(CollisionResult currentCollision)
        {
            _onPlane = true;
            TrailParticleManager.Enabled = false;
            _diagonalCollision = currentCollision.DiagonalCollision;
            if (CurrentPowerUp != PowerUpType.None && !GameGlobals.ArcadeMode)
                CurrentPowerUp = PowerUpType.None;

            //=====debug=====
            Diagonal = _diagonalCollision;
            V = currentCollision.VerticalAxis;
            H = currentCollision.HorizontalAxis;
            //=====debug======

            FixPosition = currentCollision.FixPosition;

            if (_diagonalCollision)
            {
                StickCorrner(currentCollision);
            }
            else if(FixPosition == FixPosition.Left)
            {
                StickLeft(currentCollision);
            }
            else if(FixPosition == FixPosition.Right)
            {
                StickRight(currentCollision);
            }
            else if (FixPosition == FixPosition.Top)
            {
                StickTop(currentCollision);
            }
            else if(FixPosition == FixPosition.Bottom)
            {
                StickBottom(currentCollision);
            }
            else
            {
                throw new Exception("Diagonal surface is not supported.");
            }

            if (EngineGlobals.Gravity.Y < 0)
            {
                Rotation = 180;
            }
            else
                Rotation = 0;
        }

        private CollisionResponce StickOnSlideWall(CollisionResult currentCollision)
        {
            EngineGlobals.SoundManager.Play("player_stick", "stick", this, 0.75f, false, -0.5f, -0.5f);

            var wall = currentCollision.Target as WallSlide;

            if (wall != null)
            {
                ResetFlags();
                _onWall = true;
                _currentWallFriction = wall.Friction;
                StickOnFlatSurface(currentCollision);
                SplashParticleManager.CreateSpash(SpashType.WallSplash, FixPosition, 10);
                CurrentObject = wall;

                return CollisionResponce.Hit;
            }

            throw new Exception("Wall is not a wall!");
        }

        private CollisionResponce StickOnTile(CollisionResult currentCollision)
        {
            EngineGlobals.SoundManager.Play("player_stick", "stick", this, 0.75f, false, -0.5f, -0.5f);
            ResetFlags();
            StickOnFlatSurface(currentCollision);

            if (_diagonalCollision)
            {
                SplashParticleManager.CreateSpash(SpashType.CircleSplash, FixPosition, 0);
            }
            else
            {
                SplashParticleManager.CreateSpash(SpashType.WallSplash, FixPosition, 25);
            }
            

            return CollisionResponce.Stop;
        }

        private void StickOnCircle(CollisionResult currentCollision)
        {
            CurrentObject = currentCollision.Target;
            if (CurrentPowerUp != PowerUpType.None && !GameGlobals.ArcadeMode)
                CurrentPowerUp = PowerUpType.None;

            _worldRotation = FallOnCircleDirrection(currentCollision.NewPos.Y) * (float)Math.Acos((currentCollision.CollisionPos.X - CurrentObject.HalfPos.X) / CurrentObject.HalfSize.X);

            HalfPos = CurrentObject.HalfPos;
            _circleStartRotation = _worldRotation - CurrentObject.Mask.Rotation;
            var circle = CurrentObject as Circle;
            if (circle != null)
            {
                RotationSpeed = circle.RotationSpeed;
            }
            if (CurrentObject is MovingCircle)
            {
                EngineGlobals.Camera2D.FollowObject(CurrentObject);
                Controller.ReallocateObjectsStack(this, CurrentObject);
            }
            else
            {
                EngineGlobals.Camera2D.MoveTo(
                    new Vector2((int) CurrentObject.HalfPos.X, (int) CurrentObject.HalfPos.Y - _cameraOffset),
                    _cameraSpeed);
            }

            CalculateRotationOnCircle();
            if (CurrentObject is CircleSpikes)
            {
                if ((CurrentObject as CircleSpikes).CheckCollision(_worldRotation))
                {
                    Die();
                    return;
                }
            }
            _onCircle = true;
            TrailParticleManager.Enabled = false;
            EngineGlobals.SoundManager.Play("player_stick", "stick", this, 0.75f, false, -0.5f, -0.5f);
            SplashParticleManager.CreateSpash(SpashType.CircleSplash, 0, 0);
            _sprite.SetAnimation(AnimationNames.Circle);
            RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);
        }

        private void StickOnJumpSpot(CollisionResult currentCollision)
        {
            _onJumpSpot = true;
            CurrentObject = currentCollision.Target;
            if (CurrentPowerUp != PowerUpType.None && !GameGlobals.ArcadeMode)
                CurrentPowerUp = PowerUpType.None;
            ((JumpSpot)CurrentObject).PlayerJumpsOn();
            TrailParticleManager.Enabled = false;

            HalfPos = currentCollision.Target.HalfPos;

            SplashParticleManager.CreateSpash(SpashType.CircleSplash, 0, 0);
            _sprite.SetAnimation(AnimationNames.Circle);
            RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);
        }

        #endregion

        #region Support Logic

        private int FallOnCircleDirrection(float posY)
        {
            if (posY < CurrentObject.HalfPos.Y)
            {
                return -1;
            }

            return 1;
        }

        public void CalculateRotationOnCircle()
        {
            var rotation = _circleStartRotation + CurrentObject.Mask.Rotation;
            if (float.IsNaN(_circleStartRotation))
            {
                
            }
            Mask.Rotation = rotation + (float)Math.PI / 2;
            _worldRotation = rotation;
            HalfPos = new Vector2(CurrentObject.HalfPos.X + (float)Math.Cos(rotation) * CurrentObject.Mask.Width / 2, CurrentObject.HalfPos.Y + (float)Math.Sin(rotation) * CurrentObject.Mask.Width / 2);
        }

        private void ResetFlags()
        {
            _inAir = false;
            _onCircle = false;
            _onPlane = false;
            _onWall = false;
            _onJumpSpot = false;
            if (!GameGlobals.ArcadeMode)
                CurrentPowerUp = PowerUpType.None;  
        }

        #endregion

        #region Update


        public override void Update()
        {
            if (GameGlobals.GameOver)
            {
                return;
            }
            if (_timeWarpTimer.Ticking)
            {
                _timeWarpTimer.Update();
                if (_timeWarpTimer.Finished)
                {
                    StopTimeWarpPowerup();
                }
            }
            _doubleJumpParticle.Update();
            if (_inAir)
            {
                var y = Direction.Y;
                var x = Direction.X;
                if (Direction == Vector2.Zero)
                {
                    Mask.Rotation = 0f;
                }
                else
                {
                    Mask.Rotation = (float) Math.Atan2(y, x) - (float) Math.PI/2;
                }
                
                #if !EDITOR
                GameGlobals.SaveData.DistanceTraveled += Direction.Length();
                #endif
            }
            else if (_onCircle)
            {
                if (CurrentObject is DeathBall && ((DeathBall)CurrentObject).Deadly)
                {
                    Die();
                    return;
                }

                CalculateRotationOnCircle();
            }
            else if (_onJumpSpot)
            {
                if (CurrentObject is JumpSpot && ((JumpSpot)CurrentObject).Deadly)
                {
                    Die();
                    return;
                }

              //  CalculateRotationOnCircle();
            }
            else if (_onWall)
            {
                var wall = CurrentObject as WallSlide;
                if (wall != null)
                {
                    #if !EDITOR
                    GameGlobals.SaveData.DistanceTraveled += Direction.Length();
                    #endif
                    var rect = CurrentObject.Rectangle.GetRectangle();
                    if (rect.Height + rect.Y < HalfPos.Y)
                    {
                        _onWall = false;
                        CurrentObject = null;
                        _inAir = true;
                        _sprite.SetAnimation("Air");
                        RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);
                        RotationDirection = 0;
                        Rotation = 0;
                        Mask.Rotation = 0;
                        Flip = SpriteEffects.FlipHorizontally;
                    }
                }
                else
                {
                    throw new Exception("Current wall is null!");
                }

            }

            if(CurrentObject is Circle || CurrentObject is JumpSpot)
            {
                var distance = Vector2.Distance(CurrentObject.HalfPos, HalfPos);
                if (distance > (CurrentObject.HalfSize.X + HalfSize.X))
                {
                    CurrentObject = null;
                }
            }
            
            TrailParticleManager.Update();

            /*
            if (!_markImage.IsHidden)
            {
                UpdateMarker();
            }
             */
        }

        public override void Move(Vector2 shift)
        {
            Mask.MoveSpeed = (float)Math.Sqrt(Math.Pow(shift.X, 2) + Math.Pow(shift.Y, 2));
            if (_onWall)
            {
                base.Move(shift*_currentWallFriction);
            }
            else
            {
                base.Move(shift);
            }
            var shiftDistance = (float) Math.Sqrt(
                Math.Pow(shift.X/EngineGlobals.PixelsPerMeter, 2) + Math.Pow(shift.Y/EngineGlobals.PixelsPerMeter, 2));
            CurrentMovePoints -= shiftDistance * (GameGlobals.ArcadeMode ? 1 : 1.5f); //was 0.75
            GameGlobals.HealthBar.UpdateBar();
            TotalDistanceTravelled += shiftDistance;
        }

        #endregion

        #region Load & Reset

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            TrailParticleManager = new PlayerTrailParticleManager();
            TrailParticleManager.LoadDefault();
            Load(resourceId, index);
            RotationSpeed = 0;
            JumpSpeed = 12;
            SlideSpeed = 4;
            SetRectangle(0, 0, 30, 30);
            _sprite.SetAnimation("Air");
            RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);
            LayerDepth = 0.7f;
            CurrentMovePoints = MaxMovePoints;
        }
#endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            IgnoreInGameTime = true;
            //CurrentMovePoints = MaxMovePoints;
            _outOfInkParticleManager = new PlayerOutOfInkParticleManager();
            _outOfInkParticleManager.LoadDefault();
            _outOfInkParticleManager.Load(this);
            _doubleJumpParticle = new DoubleJumpParticle();
            _doubleJumpParticle.LoadDefault();
            _doubleJumpParticle.Load(this);
            _doubleJumpParticle.Enabled = false;
            RescaleByRatio(RescaleType.Vertical, ScaleAttachSide.BottomCenter);
            StartPosition = HalfPos;
            Mask.Rotation = 0;
            _worldRotation = 0;
            _inAir = true;
            _onCircle = false;
            _onPlane = false;
            _onWall = false;
            CurrentObject = null;
            Direction = Vector2.Zero;
            TrailParticleManager.Load(this);
            SplashParticleManager = new PlayerSplashParticleManager(this);
            _timeWarpTimer = new Timer();

            _cameraOffset = GameGlobals.ArcadeMode ? 100 : 25;
            _cameraSpeed = GameGlobals.ArcadeMode ? 500 : EngineGlobals.Camera2D.DefaultSpeed;

            /*
            _markTimer = new Timer();
            _markImage = new Image(EngineGlobals.Resources.Textures["marker"][0]);
            _markImage.Rect = new Rectangle(0, 0, 30, 30);
            _markImage.StaticPosition = true;
            _markImage.IsHidden = true;
            Controller.AddObject(_markImage);
             */
        }

        #endregion


        public override object Clone()
        {
            var clone = (Player)base.Clone();
            clone.TrailParticleManager = (PlayerTrailParticleManager)TrailParticleManager.Clone();
            return clone;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (SplashParticleManager != null)
                SplashParticleManager.Dispose();
            if (TrailParticleManager != null)
                TrailParticleManager.Dispose();
        }

    }
}
