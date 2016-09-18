using System;
using Engine.Core;
using Engine.Graphics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using GameLibrary.Particles;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Objects
{
    public class LevelEnd : Circle
    {
        //protected Image _layer2;
       // protected Image _layer3;
        protected LevelEndParticleManager _particleManager;
        
        public LevelEnd()
        {
            Animated = false;
            TypeId = GameObjectType.LevelEnd.ToString();
            IgnoreCollision = true;
            Init();
        }

        public override void Load(string resourceId, int index)
        {
           /* _layer2 = new Image(EngineGlobals.Resources.Textures["LevelEndDetails"][0], Mask.Rect);
            _layer2.Scale = new Vector2(0.6f, 0.6f);
            _layer2.Orgin = _layer2.OriginCenter();
            _layer2.Owner = this;
            Controller.AddObject(_layer2);
            _layer3 = new Image(EngineGlobals.Resources.Textures["LevelEndDetails"][1], Mask.Rect);
            _layer3.Scale = new Vector2(0.3f, 0.3f);
            _layer3.Orgin = _layer3.OriginCenter();
            _layer3.Owner = this;
            Controller.AddObject(_layer3);*/
            _particleManager = new LevelEndParticleManager();
            _particleManager.LoadDefault();
            _particleManager.Load(this);
            base.Load(resourceId, index);
        }

        public override void Update()
        {
            base.Update();
           /* _layer3.Rotation -= (RotationSpeed / 1000 * EngineGlobals.GetElapsedInGameTime() *
                                EngineGlobals.TimeSpeed);
            if (_layer3.Rotation <= -MathHelper.Pi * 2f)
            {
                _layer3.Rotation = _layer3.Rotation + MathHelper.Pi * 2f;
            }*/
            _particleManager.Update();
            if (Vector2.Distance(GameGlobals.Player.HalfPos, HalfPos) <
                HalfSize.X + _particleManager.MaxCreationDistance && !GameGlobals.GameOver)
            {
                GameGlobals.GameOver = true;
                //var dist = HalfPos - GameGlobals.Player.HalfPos;
                //GameGlobals.Player.Mask.Rotation = (float)Math.Atan2(dist.Y, dist.X) - MathHelper.PiOver2;
#if !EDITOR
                GameGlobals.LevelComplete = true;
                
                GameGlobals.Game.NextLevel(this);
#endif
            }
        }

#if EDITOR
        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            Diameter = 8;
            RotationSpeed = 2;
            Mask.LayerDepth = 0.71f;
        }

        public override void EditorDeleteObject()
        {
          //  Controller.RemoveObject(_layer2);
         //   Controller.RemoveObject(_layer3);
          //  _layer2.Dispose();
         //   _layer3.Dispose();
            base.EditorDeleteObject();
        }
#endif

    }
}
