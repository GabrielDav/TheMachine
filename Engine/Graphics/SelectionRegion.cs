using System;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    public class SelectionRegion : IGraphicsObject
    {
        protected readonly PrimitiveBatch _primitiveBatch;

        public bool StaticPosition { get { return false; } set { throw new NotImplementedException();} }
        [ContentSerializerIgnore]
        public bool IgnoreCulling { get { return true; } set { throw new NotImplementedException();}}
        public Rectangle Rect { get { return SelectedRegion.GetRectangle(); } set{ throw new NotImplementedException();} }
        [ContentSerializerIgnore]
        public Rectangle CornerRectangle { get { return Rect; } }
        public Color LineColor;
        public RectangleF SelectedRegion { get; protected set; }
        public bool IsHidden { get; set; }
        public GameTexture SelectionFill;
        public Vector2 SelectionStart { get; protected set; }

        public event SimpleEvent OnPositionTypeChanged;

        public SelectionRegion()
        {
            _primitiveBatch = new PrimitiveBatch(EngineGlobals.Device);
            LineColor = Color.Green;
            SelectedRegion = new RectangleF();
        }

        public void SetSelectedRegionEnd(Vector2 pos)
        {
            if (SelectionStart.X < pos.X && SelectionStart.Y < pos.Y)
                SelectedRegion = new RectangleF(SelectionStart.X, SelectionStart.Y, pos.X - SelectionStart.X,
                                                pos.Y - SelectionStart.Y);
            else if (SelectionStart.X < pos.X && SelectionStart.Y > pos.Y)
                SelectedRegion = new RectangleF(SelectionStart.X, pos.Y, pos.X - SelectionStart.X, SelectionStart.Y - pos.Y);
            else if (SelectionStart.X > pos.X && SelectionStart.Y < pos.Y)
                SelectedRegion = new RectangleF(pos.X, SelectionStart.Y, SelectionStart.X - pos.X, pos.Y - SelectionStart.Y);
            else if (SelectionStart.X > pos.X && SelectionStart.Y > pos.Y)
                SelectedRegion = new RectangleF(pos.X, pos.Y, SelectionStart.X - pos.X, SelectionStart.Y - pos.Y);
            else
                SelectedRegion = new RectangleF();
        }

        public void StartSelection(Vector2 pos)
        {
            SelectedRegion = new RectangleF();
            SelectionStart = pos;
        }

        public void Draw()
        {
            if (IsHidden)
                return;
            if (SelectedRegion.Width < 1 || SelectedRegion.Height < 1)
                return;

            _primitiveBatch.Begin(PrimitiveType.LineList, EngineGlobals.Camera2D.GetTransformation());

            _primitiveBatch.AddVertex(new Vector2(SelectedRegion.X, SelectedRegion.Y-1), LineColor);
            _primitiveBatch.AddVertex(new Vector2(SelectedRegion.X + SelectedRegion.Width, SelectedRegion.Y-1), LineColor);

            _primitiveBatch.AddVertex(new Vector2(SelectedRegion.X + SelectedRegion.Width, SelectedRegion.Y-1), LineColor);
            _primitiveBatch.AddVertex(new Vector2(SelectedRegion.X + SelectedRegion.Width, SelectedRegion.Y + SelectedRegion.Height + 1), LineColor);

            _primitiveBatch.AddVertex(new Vector2(SelectedRegion.X-1, SelectedRegion.Y + SelectedRegion.Height), LineColor);
            _primitiveBatch.AddVertex(new Vector2(SelectedRegion.X + SelectedRegion.Width, SelectedRegion.Y + SelectedRegion.Height), LineColor);

            _primitiveBatch.AddVertex(new Vector2(SelectedRegion.X-1, SelectedRegion.Y - 1), LineColor);
            _primitiveBatch.AddVertex(new Vector2(SelectedRegion.X-1, SelectedRegion.Y + SelectedRegion.Height), LineColor);

            _primitiveBatch.End();

            if (SelectionFill == null)
                return;
            EngineGlobals.Batch.Draw(SelectionFill.Data, SelectedRegion.GetRectangle(), Color.White);
        }
    }
}
