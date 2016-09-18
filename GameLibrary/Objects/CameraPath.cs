using System;
using System.Collections.Generic;
using System.ComponentModel;
#if EDITOR
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
#endif
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TheGoo;

namespace GameLibrary.Objects
{

    using Point = Microsoft.Xna.Framework.Point;
    using Color = Microsoft.Xna.Framework.Color;

    public class CameraPath : Tile
    {
        protected PathPoint[] _path;

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

        [DisplayName("Path")]
        public PathPoint[] Path
        {
            get
            {
                return _path;
            }
            set
            {
                FirePropertyChangingEvent("Path");
                var path = new List<PathPoint>();
                foreach (var point in value)
                {
                    path.Add(new PathPoint(point.X, point.Y, point.Speed == 0f? DefaultSpeed: point.Speed, point.WaitTime == 0? DefaultWaitTime : point.WaitTime));
                }
                _path = path.ToArray();
                FirePropertyChangedEvent("Path");
            }
        }
        
#else
        public PathPoint[] Path { get { return _path; } set { _path = value; } }
#endif


#if EDITOR



        [ContentSerializerIgnore]
        [Browsable(false)]
        public PathPoint EditorNewDestination { get; set; }

        
#endif

        public Color LineColor { get; set; }

        public float DefaultSpeed { get; set; }

        public long DefaultWaitTime { get; set; }

        public override bool IgnoresPhysics
        {
            get { return true; }
        }

        public CameraPath()
        {
            _path = new PathPoint[0];
            TypeId = GameObjectType.CameraPath.ToString();
        }

#if EDITOR

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            base.LoadDefault(resourceId, index, subObjectId);
            GridSize = new Size(2, 2);
            var colors = new[] {Color.Red, Color.Green, Color.Blue, Color.Purple, Color.Orange};
            LineColor = colors[EngineGlobals.Random.Next(0, colors.Length)];
            DefaultSpeed = 100f;
            DefaultWaitTime = 0;
        }

        public override bool EditorIsDebugFlagEnabled(DebugFlag flag)
        {
            return flag == DebugFlag.Path;
        }

#endif

        public override void Load(string resourceId, int index)
        {
            base.Load(resourceId, index);
            if (!GameGlobals.EditorMode)
                Mask.IsHidden = true;
        }



    }
}
