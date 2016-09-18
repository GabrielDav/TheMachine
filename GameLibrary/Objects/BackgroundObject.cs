using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Objects
{


    public class BackgroundObject : Circle
    {

        public override float LayerDepth
        {
            get
            {
                return base.LayerDepth;
            }
            set
            {
                if (value > 0.9f)
                    value = 0.9f;
                else if (value < 0.8f)
                    value = 0.8f;
                base.LayerDepth = value;
            }
        }

        public BackgroundObject()
        {
            Animated = false;
            TypeId = GameObjectType.BackgroundGear.ToString();
            Init();
            Mask.Rotation = _rotation;
        }

        public override void Update()
        {
            RotateByGameTime();
        }


#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Diameter = 20;
            RotationSpeed = 1f;
            Mask.LayerDepth = 0.85f;

        }

        public override bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return false;
        }
#endif
    }


    class BackgroundGearSmall : BackgroundObject
    {
#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Diameter = 8;
            RotationSpeed = 1f;
            Mask.LayerDepth = 0.84f;
        }

        public override bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return false;
        }
#endif

        public override void Update()
        {
            RotateByGameTime();
        }
    }
}
