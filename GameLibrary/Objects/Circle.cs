using Engine.Core;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using GameLibrary.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Objects
{
    public class Circle : CirclePhysicalObject
    {
       
        
        public Circle()
        {
            Animated = false;
            TypeId = GameObjectType.Circle.ToString();
            Init();
        }

        public virtual void Rotate()
        {
            if (_rotationDirection == Engine.Core.Direction.Clockwise)
            {
                Mask.Rotation += (RotationSpeed / 1000 * (float)EngineGlobals.GameTime.ElapsedGameTime.TotalMilliseconds);
                if (Mask.Rotation >= MathHelper.Pi * 2f)
                {
                    Mask.Rotation = Mask.Rotation - MathHelper.Pi * 2f;
                }
            }
            else
            {
                Mask.Rotation -= (RotationSpeed / 1000 * (float)EngineGlobals.GameTime.ElapsedGameTime.TotalMilliseconds);
                if (Mask.Rotation <= -MathHelper.Pi * 2f)
                {
                    Mask.Rotation = Mask.Rotation + MathHelper.Pi * 2f;
                }
            }
        }

        public virtual void RotateByGameTime()
        {
            if (_rotationDirection == Engine.Core.Direction.Clockwise)
            {
                Mask.Rotation += (RotationSpeed / 1000 * EngineGlobals.GetElapsedInGameTime());
                if (Mask.Rotation >= MathHelper.Pi * 2f)
                {
                    Mask.Rotation = Mask.Rotation - MathHelper.Pi * 2f;
                }
            }
            else
            {
                Mask.Rotation -= (RotationSpeed / 1000 * EngineGlobals.GetElapsedInGameTime());
                if (Mask.Rotation <= -MathHelper.Pi * 2f)
                {
                    Mask.Rotation = Mask.Rotation + MathHelper.Pi * 2f;
                }
            }
        }

        public override void Update()
        {
            if (GameGlobals.ArcadeMode)
                RotateByGameTime();
            else
                Rotate();

            if (IsActivated)
            {
                base.Update();
            }
        }

#if EDITOR
        public override bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return flag == DebugFlag.Circle;
        }
        

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            Load(resourceId, index);
            Diameter = 10;
            RotationSpeed = 3f;
            Mask.LayerDepth = 0.5f;
            
            IsActivated = true;
        }

#endif

       

    }
}
