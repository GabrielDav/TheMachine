using System;
using System.ComponentModel;
using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics
{
    public enum ResizeType
    {
        None,
        Right,
        BottomRight,
        Bottom,
        Top,
        Left,
        TopLeft,
        TopRight,
        BottomLeft
    }

    public enum  DebugFlag
    {
        Circle,
        Destination,
        Path
    }

    #if EDITOR
    public interface IEditorObject : IDisposable, ICloneable
    #else
    public interface IEditorObject : IDisposable
    #endif
    {
        float LayerDepth { get; set; }

        string ResourceId { get; set; }

        bool Animated { get; }

        RectangleF Rectangle { get; set; }

        Vector2 EditorDestination { get; set; }

        event PropertyChangingEventHandler PropertyChanging;

        event PropertyChangedEventHandler PropertyChanged;

        bool IsResizeAvailable(ResizeType resizeType);

        string EditorGetName();

        void DoResize(ResizeType resizeDragType, Rectangle startRectangle, Rectangle newRectangle);

        IGraphicsObject EditorGetVisualObject();

        Vector2 EditorGetCenter();

        bool EditorIsDebugFlagEnabled(DebugFlag flag);

        void EditorDeleteObject();
    }
}
