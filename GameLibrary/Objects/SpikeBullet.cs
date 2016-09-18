using System.Collections.Generic;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;

namespace GameLibrary.Objects
{
    public class SpikeBullet : CirclePhysicalObject
    {
        protected Vector2 _startPos;
        public Vector2 _endPos;
        protected float _distance;
        public int _speed;
        public bool _isReused;
        public List<PhysicalObject> IgnoreItems;
        private readonly Timer _timer;
        private readonly Image _hitImg;
        private int _shiftX = -15;
        private int _shiftY = 0;

        public bool InFlight;

        [ContentSerializerIgnore]
        public SpikeShooter ParentShooter;

        public SpikeBullet(SpikeShooter parentShooter)
        {
            GlobalUpdate = true;
            ParentShooter = parentShooter;
            Init();
            Mask.LayerDepth = 0.55f;
            Mask.IsHidden = true;
            IsActivated = false;
            HalfSize = new Vector2(10, 10);
            IgnoreItems = new List<PhysicalObject>
                {
                    parentShooter
                };
            IgnoreGravity = true;

            _hitImg = new Image(
                EngineGlobals.Resources.Textures["Hit"][0],
                new Rectangle(0, 0, 30, 30))
                {
                    Owner = this,
                    IsHidden = true
                };
            Controller.AddObject(_hitImg);
            _timer = new Timer(true);
        }

        public void InitBullet(
            Vector2 startPos,
            Vector2 endPos,
            int speed, 
            PhysicalObject objectAttachedTo)
        {
            float x = ParentShooter.HalfPos.X;
            float y = ParentShooter.HalfPos.Y;

            Flip = ParentShooter.Flip;

            if (Flip == SpriteEffects.FlipHorizontally)
            {
                _shiftX = -_shiftX;
            }

            if (ParentShooter.Orientation == Orientation.Right)
            {
                Rotation = 90;
                _shiftX = 0;
                _shiftY = -15;
            }
            else if (ParentShooter.Orientation == Orientation.Left)
            {
                Rotation = 270;
                _shiftX = 0;
                _shiftY = 15;
            }

            if (ParentShooter.Orientation == Orientation.Top)
            {
                _shiftX = -15;
            }
            else if (ParentShooter.Orientation == Orientation.Bottom)
            {
                _shiftX = 15;
            }

            if ((endPos.X - ParentShooter.HalfSize.X) > ParentShooter.HalfPos.X)
            {
                x = ParentShooter.HalfPos.X +
                    ParentShooter.HalfSize.X +
                    HalfSize.X + 5;
            }

            if ((endPos.X + ParentShooter.HalfSize.X) < ParentShooter.HalfPos.X)
            {
                x = ParentShooter.HalfPos.X -
                    ParentShooter.HalfSize.X -
                    HalfSize.X-5; 
            }

            if ((endPos.Y - ParentShooter.HalfSize.Y) > ParentShooter.HalfPos.Y)
            {
                y = ParentShooter.HalfPos.Y +
                    ParentShooter.HalfSize.Y +
                    HalfSize.Y;
            }

            if ((endPos.Y + ParentShooter.HalfSize.Y) < ParentShooter.HalfPos.Y)
            {
                y = ParentShooter.HalfPos.Y -
                    ParentShooter.HalfSize.Y -
                    HalfSize.Y;
            }

            HalfPos = new Vector2(x, y);
            _startPos = HalfPos;
            _endPos = endPos;
            _speed = speed;

            _distance = Vector2.Distance(_startPos, _endPos);
            Direction = Vector2.Normalize(_endPos - _startPos);
            IgnoreItems.Add(objectAttachedTo);
        }

        public void Activate()
        {
            IsActivated = true;
            Mask.IsHidden = false;
            InFlight = true;
            
            if (!_isReused)
            {
                GameGlobals.Physics.AddObjectToQueue(this);
                GameGlobals.Physics.CommitQueue();
                _isReused = true;
            }

            HalfPos = _startPos;

            GameGlobals.Physics.PushObject(this, _endPos, _speed);
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Mask.LayerDepth = 0.55f;
        }
#endif

        public void RemoveBullet()
        {
            HalfPos = _startPos;
            Mask.IsHidden = true;
            InFlight = false;
            ParentShooter.CurrentBulletCount--;
        }

        public override void Update()
        {
            if (InFlight)
            {
                //var delta = (float) EngineGlobals.GameTime.ElapsedGameTime.TotalSeconds*_speed;

                //HalfPos += Direction*delta;

                base.Update();

                if (Vector2.Distance(HalfPos, _startPos) >= _distance)
                {
                    RemoveBullet();
                }
            }

            if (!_hitImg.IsHidden)
            {
                _timer.Update();
                if (_timer.Finished)
                {
                    _hitImg.IsHidden = true;
                }
            }
        }

        public override CollisionResponce Collide(CollisionResult collisionResult)
        {
            foreach (var item in IgnoreItems)
            {
                if (collisionResult.Target == item)
                {
                    return CollisionResponce.GoTrought;
                }
            }

            var target = collisionResult.Target as Player;
            if (target != null)
            {
               target.Die();
            }
            else
            {
                HitAnimation();
            }

            RemoveBullet();
            return CollisionResponce.Stop;
        }

        private void HitAnimation()
        {
            _hitImg.IsHidden = false;
            _hitImg.Pos = new Vector2(HalfPos.X + _shiftX, HalfPos.Y + _shiftY);
            _hitImg.Orgin = _hitImg.OriginCenter();
            _hitImg.Rotation = Mask.Rotation;
            _timer.Start(250, false);

            EngineGlobals.SoundManager.Play(
                "bullet_hit",
                "hit",
                this,
                0.5f,
                false,
                0.25f,
                0.25f);
        }
    }
}
