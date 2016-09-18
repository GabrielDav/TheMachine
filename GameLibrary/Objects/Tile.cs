using System;
using Engine.Core;
using Engine.Mechanics;
using GameLibrary.Gui.ScreenManagement.NewScreens;
using Microsoft.Xna.Framework;

namespace GameLibrary.Objects
{
    public class Tile : BoxPhysicalObject
    {
        public Tile ()
        {
            TypeId = GameObjectType.Tile.ToString();
            Init();
        }

        #if EDITOR

        public override void LoadDefault(string resourceId, int index, int subObjectId)
        {
            _resourceId = resourceId;
            _resourceVariation = index;
            Load(resourceId, index);
            SetRectangle(0, 0, 100, 100);
            Mask.LayerDepth = 0.5f;
        }

        public override bool IsResizeAvailable(ResizeType resizeType)
        {
            return resizeType == ResizeType.BottomRight || resizeType == ResizeType.TopRight ||
                   resizeType == ResizeType.BottomLeft || resizeType == ResizeType.TopLeft;
        }

        public override void DoResize(ResizeType resizeDragType, Rectangle startRectangle, Rectangle newRectangle)
        {
            switch (resizeDragType)
            {
                case ResizeType.TopLeft:
                    if (startRectangle.X - newRectangle.X > startRectangle.Y - newRectangle.Y)
                    {
                        newRectangle.Y = startRectangle.Y - (startRectangle.X - newRectangle.X);
                        newRectangle.Height = newRectangle.Width;
                    }
                    else if (startRectangle.Y - newRectangle.Y > startRectangle.X - newRectangle.X)
                    {
                        newRectangle.X = startRectangle.X - (startRectangle.Y - newRectangle.Y);
                        newRectangle.Width = newRectangle.Height;
                    }
                    if (newRectangle.Y < 0)
                    {
                        newRectangle.Y = 0;
                        newRectangle.Height = startRectangle.Y + startRectangle.Height;
                        newRectangle.X = startRectangle.X - startRectangle.Y;
                        newRectangle.Width = newRectangle.Height;
                    }
                    else if (newRectangle.X < 0)
                    {
                        newRectangle.X = 0;
                        newRectangle.Width = startRectangle.X + startRectangle.Width;
                        newRectangle.Y = startRectangle.Y - startRectangle.X;
                        newRectangle.Height = newRectangle.Width;
                    }
                    break;
                case ResizeType.TopRight:
                    if (newRectangle.Width > newRectangle.Height)
                    {
                        newRectangle.Y = startRectangle.Y + startRectangle.Height - newRectangle.Width;
                        newRectangle.Height = newRectangle.Width;
                    }
                    else if (newRectangle.Width < newRectangle.Height)
                    {
                        newRectangle.Width = newRectangle.Height;
                    }
                    if (newRectangle.Y < 0)
                    {
                        newRectangle.Y = 0;
                        newRectangle.Height = startRectangle.Y + startRectangle.Height;
                        newRectangle.Width = newRectangle.Height;
                    }
                    if (newRectangle.X + newRectangle.Width > EngineGlobals.Grid.WidthReal)
                    {
                        newRectangle.Width = EngineGlobals.Grid.WidthReal - newRectangle.X;
                        newRectangle.Y = startRectangle.Y + startRectangle.Height - newRectangle.Width;
                        newRectangle.Height = newRectangle.Width;
                    }
                    break;
                case ResizeType.BottomLeft:
                    if (newRectangle.Height > newRectangle.Width)
                    {
                        newRectangle.X = startRectangle.X + startRectangle.Width - newRectangle.Height;
                        newRectangle.Width = newRectangle.Height;
                    }
                    else if (newRectangle.Height < newRectangle.Width)
                    {
                        newRectangle.Height = newRectangle.Width;
                    }
                    if (newRectangle.X < 0)
                    {
                        newRectangle.X = 0;
                        newRectangle.Width = startRectangle.X + startRectangle.Width;
                        newRectangle.Height = newRectangle.Width;
                    }
                    if (newRectangle.Y + newRectangle.Height > EngineGlobals.Grid.HeightReal)
                    {
                        newRectangle.Height = EngineGlobals.Grid.HeightReal - newRectangle.Y;
                        newRectangle.X = startRectangle.X + startRectangle.Width - newRectangle.Height;
                        newRectangle.Width = newRectangle.Height;
                    }
                    break;
                case ResizeType.BottomRight:
                    if (newRectangle.Width > newRectangle.Height)
                    {
                        newRectangle.Height = newRectangle.Width;
                    }
                    else if (newRectangle.Width < newRectangle.Height)
                    {
                        newRectangle.Width = newRectangle.Height;
                    }
                    break;
                default:
                    throw new Exception("Unsuported resize type: " + resizeDragType);
            }
            if (newRectangle.X + newRectangle.Width > EngineGlobals.Grid.WidthReal)
            {
                newRectangle.Width = EngineGlobals.Grid.WidthReal - newRectangle.X;
                newRectangle.Height = newRectangle.Width;
            }
            if (newRectangle.Y + newRectangle.Height > EngineGlobals.Grid.HeightReal)
            {
                newRectangle.Height = EngineGlobals.Grid.HeightReal - newRectangle.Y;
                newRectangle.Width = newRectangle.Height;
            }
            SetRectangle(newRectangle.X, newRectangle.Y, newRectangle.Width, newRectangle.Height);
        }
#endif

    }
}
