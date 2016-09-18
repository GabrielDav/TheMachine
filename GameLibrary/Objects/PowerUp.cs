using System;
using System.ComponentModel;
using Engine.Core;
#if EDITOR
using System.Drawing;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Objects
{
    public enum PowerUpType { None = -1, DoubleJump = 0, InkCollect = 1, TimeWarp = 2, Shield = 3 }

    public class PowerUp : InkDot
    {
        [Browsable(false)]
        [ContentSerializerIgnore]
        public override float InkToRecover
        {
            get;
            set;
        }

        #if EDITOR
        [Browsable(false)]
        public override int ResourceVariationEditor
        {
            get;
            set;
        }
        #endif

        public PowerUpType PowerUpType { get; set; }

        public PowerUp()
        {
            TypeId = GameObjectType.PowerUp.ToString();
        }

#if EDITOR

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            PowerUpType = (PowerUpType)subObjectId;
            Load(resourceId, index);
            RotationSpeed = 0f;
            Mask.LayerDepth = 0.3f;
            SpeedIncrement = 100;
            GridSize = new Size(4, 4);
            TriggerDistance = 240;
            MoveSpeed = 300;
        }

#endif

        public override void Load(string resourceId, int index)
        {
            var size = new Vector2(Rectangle.Width, Rectangle.Height);
            base.Load(resourceId, (int)PowerUpType);
            #if EDITOR
            ResourceVariationEditor = (int)PowerUpType + 1;
            #endif
            SetRectangle(_rectangle.X, _rectangle.Y, size.X, size.Y);
            IgnoreCollision = true;
        }

        public override void Merge()
        {
            if (PowerUpType == PowerUpType.Shield)
            {
                foreach (var physicalObject in GameGlobals.Map.GameObjects)
                {
                    if (physicalObject is SeekerDot && ((SeekerDot) physicalObject).Follow)
                    {
                        ((SeekerDot) physicalObject).Die();
                    }
                }
            }
            Mask.IsHidden = true;
            IsActivated = false;
            Follow = false;
            if (GameGlobals.Player.IsInAir)
            {
                GameGlobals.Player.CurrentPowerUp = PowerUpType;
            }

        }
    }
}
