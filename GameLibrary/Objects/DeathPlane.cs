using System.ComponentModel;
#if EDITOR
using System.Drawing;
#endif
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Microsoft.Xna.Framework;
using TheGoo;

namespace GameLibrary.Objects
{
    public class DeathPlane : Spike
    {
        #if EDITOR
        [ReadOnly(false)]
        public override Size GridSize { get; set; }
        #endif

        public override void Draw()
        {
#if DEBUG
            base.Draw();
#endif
            
        }

        protected override void Init()
        {
            base.Init();
#if !EDITOR
            if (!GameGlobals.EditorMode)
                Mask.IsHidden = false;
#endif
        }
    }
}
