using Engine.Core;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Objects
{
    public class InkDot : CirclePhysicalObject
    {
        public virtual float InkToRecover { get; set; }

        public float MoveSpeed { get; set; }

        public int TriggerDistance { get; set; }

        public float SpeedIncrement { get; set; }

        protected bool _ignoreInkCollect;


        [ContentSerializerIgnore]
        public bool Follow;

        [ContentSerializerIgnore]
        public float SpeedIncrementTotal;

        protected int _arcadeInkCollectPowerupDistance;

        public InkDot()
        {
            Animated = false;
            IgnoreCollision = true;
            TypeId = GameObjectType.InkDot.ToString();
            _arcadeInkCollectPowerupDistance = 400;

            Init();
        }

        public void TempInit()
        {
            Animated = false;
            IgnoreCollision = true;
            TypeId = GameObjectType.Circle.ToString();
            Init();
        }

#if EDITOR

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Diameter = 1;
            RotationSpeed = 0f;
            Mask.LayerDepth = 0.3f;
            InkToRecover = 5;
            MoveSpeed = 200;
            TriggerDistance = 125;
            SpeedIncrement = 75;
            IsActivated = true;
        }
#endif
        public override void Update()
        {
            if (IsActivated)
            {
                var distance = Vector2.Distance(HalfPos, GameGlobals.Player.HalfPos);
                if (distance <= HalfSize.X + GameGlobals.Player.HalfSize.X)
                {
                    Merge();
                }
                else if (Follow)
                {
                    SpeedIncrementTotal += (SpeedIncrement * (float)EngineGlobals.GameTime.ElapsedGameTime.TotalMilliseconds / 1000f);
                    var direction = Vector2.Normalize(GameGlobals.Player.HalfPos - HalfPos);

                    var delta = (float)EngineGlobals.GameTime.ElapsedGameTime.TotalMilliseconds / 1000 *
                                (MoveSpeed + SpeedIncrementTotal);
                    HalfPos += direction * delta;
                }
                else
                {
                    var brderDistance = distance + HalfSize.X + GameGlobals.Player.HalfSize.X;

                    if (brderDistance <= TriggerDistance ||
                        (GameGlobals.Player.CurrentPowerUp == PowerUpType.InkCollect && GameGlobals.ArcadeMode && brderDistance <= 400 && !_ignoreInkCollect))
                    {
                        StartFollow();
                    }
                }

                
            }
        }

        public virtual void StartFollow()
        {
            Follow = true;
        }

        public override void Draw()
        {
            base.Draw();
        }

        public virtual void Merge()
        {
            GameGlobals.Player.CurrentMovePoints += InkToRecover;
            Mask.IsHidden = true;
            IsActivated = false;
            Follow = false;
            GameGlobals.Score += (int)InkToRecover;
            GameGlobals.UpdateScore();
            GameGlobals.HealthBar.UpdateBar();
            GameGlobals.CreditsData.InkCollected++;
            GameGlobals.TotalInkCollected++;
        }

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            SetRectangle(_rectangle.X, _rectangle.Y, 14,14);
            ///IsActivated = true;
        }

    }
}
