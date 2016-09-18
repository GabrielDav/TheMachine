using System;
using System.ComponentModel;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;

#if EDITOR
using System.Drawing;
#endif

namespace GameLibrary.Objects
{
    /*public enum DecorativeType
    {
        None = 0,
        WallSmall = 1,
        WallMed = 2,
        WallLarge = 3,
        WallSmallDec = 4,
        WallSmallCorner = 5,
        WallMedCorner = 6,
        WallDecCircleBot = 7,
        WallDecObjectBot = 8,
        WallLargeBot = 9,
        WallMedBot = 10,
        WallSmallBot = 11,
        WallDecCircleTop = 12,
        WallDecObjectTop = 13,
        WallLargeTop = 14,
        WallMedTop = 15,
        WallSmallTop = 16
    };*/

    public enum DecorativeType
    {
        None = 0,
        WallSmall = 1,
        WallMed = 2,
        WallLarge = 3,
        WallSmallDec = 4,
        WallMedDec = 20,
        WallLargeDec = 21,
        WallSmallCorner = 5,
        WallDecCircleBot = 7,
        WallDecObjectBot = 8,
        WallDecCircleTop = 12,
        WallDecObjectTop = 13,
        WallDec1 = 14,
        WallDec2 = 15,
        WallDec3 = 16,
        WallDec4 = 17,
        HandWall = 18,
        WallSlideSmall = 22,
        WallSlideLarge = 23,
        WallMini = 24,
        ContactInfo = 25,
        HandWallSmall = 26,
        GameName = 27
    };


    public class DecorativeObject : BoxPhysicalObject
    {
        public override bool IgnoresPhysics
        {
            get
            {
                return true;
            }
        }
        public DecorativeObject()
        {
            Animated = false;
            TypeId = GameObjectType.DecorativeObject.ToString();
            IgnoreCollision = true;
            Init();
        }

#if EDITOR

        [ReadOnly(true)]
        public override Size GridSize
        {
            get
            {
                return base.GridSize;
            }
            set
            {
                base.GridSize = value;
            }
        }

        public override bool IsResizeAvailable(Engine.Mechanics.ResizeType resizeType)
        {
            return false;
        }

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Mask.LayerDepth = 0.49f;
            var decorativeType = (DecorativeType)subObjectId;
            TypeId = decorativeType.ToString();
            switch (decorativeType)
            {
                case DecorativeType.WallSmall:
                    GridSize = new Size(10, 2);
                    break;
                case DecorativeType.WallMed:
                    GridSize = new Size(25, 2);
                    break;
                case DecorativeType.WallLarge:
                    GridSize = new Size(50, 2);
                    break;
                case DecorativeType.WallSmallDec:
                    GridSize = new Size(8, 5);
                    break;
                case DecorativeType.WallMedDec:
                    GridSize = new Size(22, 7);
                    break;
                case DecorativeType.WallLargeDec:
                    GridSize = new Size(49, 7);
                    break;
                case DecorativeType.WallSmallCorner:
                    GridSize = new Size(10,10);
                    break;
                case DecorativeType.WallDecCircleBot:
                case DecorativeType.WallDecCircleTop:
                    GridSize = new Size(26, 20);
                    break;
                case DecorativeType.WallDecObjectBot:
                case DecorativeType.WallDecObjectTop:
                    GridSize = new Size(5, 13);
                    break;
                case DecorativeType.WallDec1:
                    GridSize = new Size(22, 13);
                    break;
                case DecorativeType.WallDec2:
                    GridSize = new Size(37, 11);
                    break;
                case DecorativeType.WallDec3:
                    GridSize = new Size(15, 9);
                    break;
                case DecorativeType.WallDec4:
                    GridSize = new Size(24, 17);
                    break;
                case DecorativeType.HandWall:
                    GridSize = new Size(10, 24);
                    break;
                case DecorativeType.WallSlideLarge:
                    GridSize = new Size(2, 50);
                    break;
                case DecorativeType.WallSlideSmall:
                    GridSize = new Size(2, 10);
                    break;
                case DecorativeType.WallMini:
                    GridSize = new Size(3, 2);
                    break;
                case DecorativeType.ContactInfo:
                    GridSize = new Size(20, 6);
                    break;
                    case DecorativeType.HandWallSmall:
                    GridSize = new Size(8, 20);
                    break;
                    case DecorativeType.GameName:
                    GridSize = new Size(40, 9);
                    break;
                default:
                    throw new Exception("Undefined decorative type: '" + decorativeType + "'");
            }
        }
#endif
    }
}
