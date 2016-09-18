using System;
using Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics
{
    public class Grid : IGraphicsObject
    {
        protected Vector2 _maxBounds;
        protected Vector2 _minBounds;
        protected Rectangle _visibleGrid;
        protected readonly bool _drawable;
        protected readonly PrimitiveBatch _primitiveBatch;

        public Color LineColor;
        public int CellWidth { get; protected set; }
        public int CellHeight { get; protected set; }
        public Rectangle GridRectangle { get; protected set; }
        public int WidthReal { get; protected set; }
        public int HeightReal { get; protected set; }
        public bool Background { get; set; }

        public bool IsHidden { get; set; }
        

#if EDITOR
        /// <summary>
        /// Creates grid
        /// </summary>
        /// <param name="gridRectangle">Rectangle where X and Y starting position in game, Width and Height - number of cells in grid</param>
        /// <param name="cellWidth">Cell width</param>
        /// <param name="cellHeight">Cell height</param>
        /// /// <param name="drawable">Wahatever grid will be drawable in runtime</param>
        public Grid(Rectangle gridRectangle, int cellWidth, int cellHeight, bool drawable)
        {
            _minBounds = new Vector2(gridRectangle.X, gridRectangle.Y);
            _maxBounds = new Vector2(_minBounds.X + cellWidth*gridRectangle.Width,
                                     _minBounds.Y + cellHeight*gridRectangle.Height);
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            GridRectangle = gridRectangle;
            WidthReal = GridRectangle.Width*CellWidth;
            HeightReal = GridRectangle.Height*CellHeight;
            _drawable = drawable;
            if (drawable)
            {
                _primitiveBatch = new PrimitiveBatch(EngineGlobals.Device);
                Controller.CameraMove += OnCameraUpdate;
                OnCameraUpdate(null);
                LineColor = Color.Black;
            }
            
        }
#else
        /// <summary>
        /// Creates grid
        /// </summary>
        /// <param name="gridRectangle">Rectangle where X and Y starting position in game, Width and Height - number of cells in grid</param>
        /// <param name="cellWidth">Cell width</param>
        /// <param name="cellHeight">Cell height</param>
        /// /// <param name="drawable">Wahatever grid will be drawable in runtime</param>
        public Grid(Rectangle gridRectangle, int cellWidth, int cellHeight, bool drawable)
        {
            _minBounds = new Vector2(gridRectangle.X, gridRectangle.Y);
            _maxBounds = new Vector2(_minBounds.X + cellWidth * gridRectangle.Width,
                                     _minBounds.Y + cellHeight * gridRectangle.Height);
            CellWidth = cellWidth;
            CellHeight = cellHeight;
            GridRectangle = gridRectangle;
            WidthReal = GridRectangle.Width * CellWidth;
            HeightReal = GridRectangle.Height * CellHeight;
            _drawable = drawable;
            if (drawable)
            {
                _primitiveBatch = new PrimitiveBatch(EngineGlobals.Device);
                //EngineGlobals.Camera.OnCameraMove += OnCameraUpdate;
                //OnCameraUpdate(null);
            }
        }
#endif

        private void OnCameraUpdate(object sender)
        {
            _visibleGrid = new Rectangle();
            var startPos = new Vector2();
            var camRect = EngineGlobals.Camera2D.CameraRectangle;
            if (Background)
            {
                camRect.X /= EngineGlobals.Camera2D.BackgroundSpeedModifier;
                camRect.Y /= EngineGlobals.Camera2D.BackgroundSpeedModifier;
            }
            if (camRect.X > _maxBounds.X || camRect.Y > _maxBounds.Y)
                return;

            if (camRect.X <= GridRectangle.X)
                startPos.X = GridRectangle.X;
            else
            {
                startPos.X = (int)(CellWidth * Math.Floor((float)camRect.X / CellWidth));
            }
            //startPos.X = (int) (EngineGlobals.Camera2D.CameraRectangle.X - _minBounds.X);

            if (camRect.Y <= GridRectangle.Y)
                startPos.Y = GridRectangle.Y;
            else
            {
                startPos.Y = (int)(CellHeight * Math.Floor((float)camRect.Y / CellHeight));
            }
                //startPos.Y = (int) (EngineGlobals.Camera2D.CameraRectangle.Y - _minBounds.Y);

            //startPos = EngineGlobals.Camera2D.ViewToWorld(startPos);

            //_visibleGrid.X = float.IsNaN(startPos.X) ? 0 : (int) startPos.X;
            //_visibleGrid.Y = float.IsNaN(startPos.Y) ? 0 : (int) startPos.Y;
            _visibleGrid.X = (int)startPos.X; 
            _visibleGrid.Y = (int)startPos.Y;


            if (camRect.X + EngineGlobals.Device.Viewport.Width / EngineGlobals.Camera2D.Zoom > _maxBounds.X)
                _visibleGrid.Width = (GridRectangle.Width * CellWidth - _visibleGrid.X)/CellWidth;
            else
                _visibleGrid.Width = (int)((float)EngineGlobals.Device.Viewport.Width/CellWidth/EngineGlobals.Camera2D.Zoom) + 1;

            if (camRect.Y + EngineGlobals.Device.Viewport.Height / EngineGlobals.Camera2D.Zoom > _maxBounds.Y)
                _visibleGrid.Height = (GridRectangle.Height * CellHeight - _visibleGrid.Y)/CellHeight;
            else
                _visibleGrid.Height = (int)((float)EngineGlobals.Device.Viewport.Height / CellHeight / EngineGlobals.Camera2D.Zoom) + 1;
        }

        public Point ToCell(Vector2 position)
        {
            if (position.X < _minBounds.X || position.X > _maxBounds.X)
                throw new Exception("Position " + position + " is outside grid width bounds");
            if (position.Y < _minBounds.Y || position.Y > _maxBounds.Y)
                throw new Exception("Position " + position + " is outside grid height bounds");
            return new Point((int)(position.X - _minBounds.X)/CellWidth,
                             (int)(position.Y - _minBounds.Y)/CellHeight);
        }

        public Vector2 ToPosition(Point cell)
        {
            if (cell.X < 0 || cell.X > GridRectangle.Width)
                throw new Exception("Point (" + cell.X + "," + cell.Y + ") is outside grid width bounds");
            if (cell.Y < 0 || cell.Y > GridRectangle.Height)
                throw new Exception("Point (" + cell.X + "," + cell.Y + ") is outside grid height bounds");
            return new Vector2(_minBounds.X + cell.X*CellWidth, _minBounds.Y + cell.Y*CellHeight);
        }

        public Rectangle ToRealRectangle(Rectangle rectangle)
        {
            var point = ToPosition(new Point(rectangle.X, rectangle.Y));
            return new Rectangle((int) point.X, (int) point.Y, rectangle.Width*CellWidth, rectangle.Height*CellHeight);
        }

        public Rectangle ToCellRectangle(Rectangle rectangle)
        {
            var point = ToCell(new Vector2(rectangle.X, rectangle.Y));
            return new Rectangle(point.X, point.Y, rectangle.Width/CellWidth, rectangle.Height/CellHeight);
        }

        public Rectangle ToAlignedRectangle(Rectangle realRectangle)
        {
            //if (realRectangle.X < _minBounds.X || realRectangle.X > _maxBounds.X)
            //    throw new Exception("Rectangle " + realRectangle + " is outside grid width bounds");
            //if (realRectangle.Y < _minBounds.Y || realRectangle.Y > _maxBounds.Y)
            //    throw new Exception("Rectangle " + realRectangle + " is outside grid height bounds");
            return new Rectangle((realRectangle.X / CellWidth) * CellWidth, (realRectangle.Y / CellHeight) * CellHeight,
                (realRectangle.Width / CellWidth) * CellWidth, (realRectangle.Height / CellHeight) * CellHeight);

        }

        public Point ToAlignedPoint(Point point)
        {
            return new Point((point.X / CellWidth) * CellWidth, (point.Y / CellHeight) * CellHeight);
        }

        public bool StaticPosition
        {
            get { return false; }
            set { throw new Exception("Grid does not support static position."); }
        }

        public bool IgnoreCulling { get { return true; } set { } }
        public Rectangle Rect { get { return GridRectangle; } set { throw new NotImplementedException();} }
        [ContentSerializerIgnore]
        public Rectangle CornerRectangle { get { return GridRectangle; } }

        public event SimpleEvent OnPositionTypeChanged;

        public void Draw()
        {
            if (!_drawable)
                return;
            if (_visibleGrid.Width < 1 || _visibleGrid.Height < 1)
                return;
            if (IsHidden)
                return;


            _primitiveBatch.Begin(PrimitiveType.LineList, Background?EngineGlobals.Camera2D.GetBackgroundTransformation():EngineGlobals.Camera2D.GetTransformation());
            var start = _visibleGrid.X;
            for (var i = 0; i < _visibleGrid.Width; i++)
            {
                _primitiveBatch.AddVertex(new Vector2(start, _visibleGrid.Y), LineColor);
                _primitiveBatch.AddVertex(new Vector2(start, _visibleGrid.Y + _visibleGrid.Height * CellHeight),
                                          LineColor);
                start += CellWidth;
            }
            start = _visibleGrid.Y;
            for (var i = 0; i < _visibleGrid.Height; i++)
            {
                _primitiveBatch.AddVertex(new Vector2(_visibleGrid.X, start), LineColor);
                _primitiveBatch.AddVertex(new Vector2(_visibleGrid.X + _visibleGrid.Width * CellWidth, start),
                                          LineColor);
                start += CellWidth;
            }
            _primitiveBatch.End();
        }
    }
}
