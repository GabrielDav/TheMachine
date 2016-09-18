using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using GameLibrary;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;
using XnaGraphicsDeviceControl;

namespace GameEditor
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to draw animating
    /// 3D graphics inside a WinForms application. It hooks the Application.Idle
    /// event, using this to invalidate the control, which will cause the animation
    /// to constantly redraw.
    /// </summary>
    internal class XnaWindow : GraphicsDeviceControl
    {
        protected bool _initializeDone;
        protected SpriteFont _font;
        public Map GameLevel;
        protected TextRegion _debugInfo;
        public Image _tempObjectPreviewImg;
        public Point MousePosCell = new Point(0,0);
        public Point MousePosReal = new Point(0,0);
        public Image Selection;
        public IEditorObject SelectedObject;
        //public Dictionary<string, GameTexture> Textures;
        //public Dictionary<string, SpriteData> Sprites;
        //public ResourceCollection Resources;
        public event VoidEvent DrawRedirect;
        public Rectangle TempObjectRect;
        public PhysicalObject TempObject;
        protected System.Timers.Timer _timer;
        public Region SelectedRegion;
        protected PrimitiveBatch _primitiveBatch;
        public Image _debugCircle;
        public SelectionRegion MultiSelectionRegion;
        protected bool _multiSelectionMode;
        public List<IEditorObject> MultiSelectedObjects;
        protected List<Image> _selections;
        protected GameTexture _selectionTexture;

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            Controller.InitFormWindow(GraphicsDevice, new ContentManager(Services) { RootDirectory = "GameContent" });
            Load();
            // Hook the idle event to constantly redraw our animation.
            //Application.Idle += delegate { Invalidate(); };
            _initializeDone = true;
            _timer = new System.Timers.Timer {Interval = 12};
            _timer.Elapsed += (sender, args) => Invalidate();
            _timer.Start();
        }

        protected new void Load()
        {
            try
            {
                _font = EngineGlobals.ContentCache.Load<SpriteFont>("GameFont");
                _debugInfo = new TextRegion(new Rectangle(10, 5, 400, 100), _font, Color.Blue, "", true)
                                 {LayerDepth = 0f};
                Controller.AddObject(_debugInfo);
                _selectionTexture = new GameTexture("Selection");
                Selection = new Image(_selectionTexture) {IsHidden = true};
                _primitiveBatch = new PrimitiveBatch(EngineGlobals.Device);
                MultiSelectionRegion = new SelectionRegion {SelectionFill = new GameTexture("multiselect")};
                Controller.AddObject(MultiSelectionRegion);
                _debugCircle = new Image(new GameTexture("DebugCircle")) {Color = {A = 55}};
                _debugCircle.Orgin = _debugCircle.OriginCenter();
                _debugCircle.IsHidden = true;
                _selections = new List<Image>();
                //Controller.AddObject(_debugCircle);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void MultiSelectObjects(List<IEditorObject> objects)
        {
            _multiSelectionMode = true;
            MultiSelectedObjects = objects;
            foreach (var selection in _selections)
            {
                Controller.RemoveObject(selection);
            }
            _selections.Clear();
            foreach (var editorObject in objects)
            {
                var selection = new Image(_selectionTexture, editorObject.Rectangle.GetRectangle())
                    {LayerDepth = editorObject.LayerDepth - 0.01f, Owner = editorObject};
                _selections.Add(selection);
                Controller.AddObject(selection);
            }
        }

        public void DeselectMultiSelectedObjects()
        {
            _multiSelectionMode = false;
            MultiSelectedObjects.Clear();
            foreach (var selection in _selections)
            {
                Controller.RemoveObject(selection);
            }
            _selections.Clear();
        }

        public void SelectObject(PhysicalObject objectToSelect)
        {
            if (SelectedObject != null)
                throw new Exception("Previous object has not been deselected");
            SelectedObject = objectToSelect;
            Selection.Rect = SelectedObject.Rectangle.GetRectangle();
            SelectedObject.PropertyChanged += SelectedObjectPropertyChange;
            Selection.IsHidden = false;
            if (SelectedObject.EditorIsDebugFlagEnabled(DebugFlag.Circle))
            {
                _debugCircle.IsHidden = false;
                RefreshSelection();
            }
        }

        public void DeselectObject()
        {

            if (SelectedObject != null)
            {
                Form1.Log("Deselect object: " + SelectedObject.EditorGetName());
                SelectedObject.PropertyChanged -= SelectedObjectPropertyChange;
            }
            else
            {
                Form1.Log("WARNING: Can't deselect object, no object is selected");
            }
            SelectedObject = null;
            Selection.IsHidden = true;
            _debugCircle.IsHidden = true;
        }

        public IEditorObject CheckSelection()
        {
            IEditorObject selectedObj = null;
            if (Form1.BackgroundMode)
            {
                foreach (var mapObject in GameLevel.BackgroundObjects.Where(mapObject => mapObject.Rectangle.Contains(MousePosReal.X, MousePosReal.Y)))
                {
                    if (selectedObj == null)
                        selectedObj = mapObject;
                    else if (selectedObj.LayerDepth > mapObject.LayerDepth)
                        selectedObj = mapObject;
                }
            }
            else if (Form1.RegionEditingMode)
            {
                foreach (var mapObject in GameLevel.Regions.Where(mapObject => mapObject.Rectangle.Contains(MousePosReal.X, MousePosReal.Y)))
                {
                    if (selectedObj == null)
                        selectedObj = mapObject;
                    else if (selectedObj.LayerDepth > mapObject.LayerDepth)
                        selectedObj = mapObject;
                }
            }
            else
            {
                foreach (
                    var mapObject in
                        GameLevel.GameObjects.Where(
                            mapObject => mapObject.Rectangle.Contains(MousePosReal.X, MousePosReal.Y)))
                {
                    if (selectedObj == null)
                        selectedObj = mapObject;
                    else if (selectedObj.LayerDepth > mapObject.LayerDepth)
                        selectedObj = mapObject;
                }
            }
            return selectedObj;
        }

        private void SelectedObjectPropertyChange(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Rectangle" || propertyChangedEventArgs.PropertyName == "Orientation")
                RefreshSelection();
        }

        public void RefreshSelection()
        {
            if (!_multiSelectionMode && SelectedObject != null)
            {
                Selection.Rect = SelectedObject.Rectangle.GetRectangle();
                if (SelectedObject.EditorIsDebugFlagEnabled(DebugFlag.Circle))
                {
                    var v = Math.Pow(((Circle)SelectedObject).RotationSpeed / 3 + 12, 2);
                    var dist = ((v / (2 * EngineGlobals.Gravity.Y)) * EngineGlobals.PixelsPerMeter)*2 + 50;
                    _debugCircle.Pos = new Vector2(SelectedObject.EditorGetCenter().X, SelectedObject.EditorGetCenter().Y);
                    _debugCircle.Width = (int)(SelectedObject.Rectangle.Width + dist);
                    _debugCircle.Height = (int)(SelectedObject.Rectangle.Width + dist);
                    _debugCircle.LayerDepth = SelectedObject.LayerDepth + 0.5f;
                }
            }
            else if (_multiSelectionMode)
            {
                foreach (var selection in _selections)
                {
                    selection.Rect = ((IEditorObject) selection.Owner).Rectangle.GetRectangle();
                }
            }
        }

        public void CreateObjectPreview(PhysicalObject obj, bool background)
        {

            obj.HideDetailImages(true);
            if (!obj.Animated)
            {
                _tempObjectPreviewImg = new Image(EngineGlobals.Resources.Textures[obj.ResourceId][obj.ResourceVariationEditor-1],
                                                  obj.Mask.Rect)
                    {Color = {A = 100}, Scale = obj.Mask.Scale, Rotation = obj.Mask.Rotation, LayerDepth = obj.LayerDepth};
            }
            else
            {
                var sprite = new Sprite(EngineGlobals.Resources.Sprites[obj.ResourceId][obj.ResourceVariationEditor-1]);
                _tempObjectPreviewImg = sprite.GetFirstFrame();
                _tempObjectPreviewImg.Rect = obj.Rectangle.GetRectangle();
                _tempObjectPreviewImg.Color.A = 100;
            }
        }

        public void ExecuteMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        public void RemoveObjectPreview(bool background)
        {
            if (_tempObjectPreviewImg == null)
                return;
            _tempObjectPreviewImg.Dispose();
            _tempObjectPreviewImg = null;
        }

        protected void DrawDebugDestination()
        {
            _primitiveBatch.Begin(PrimitiveType.LineList, EngineGlobals.Camera2D.GetTransformation());
            var destination = SelectedObject.EditorDestination;
            if (destination.X > 0 && destination.Y > 0)
            {
                var center = SelectedObject.EditorGetCenter();
                _primitiveBatch.DrawLine(center, destination, Color.Green);
                var distance = destination - center;
                distance.Normalize();
                var distanceNormal = distance.GetNormal();
                var r = SelectedObject.Rectangle.Width / 2;

                _primitiveBatch.DrawLine(destination.X + r * distanceNormal.X, destination.Y + r * distanceNormal.Y,
                                destination.X - r * distanceNormal.X, destination.Y - r * distanceNormal.Y, Color.DarkRed);
                _primitiveBatch.DrawLine(center.X + r * distanceNormal.X, center.Y + r * distanceNormal.Y,
                                center.X - r * distanceNormal.X, center.Y - r * distanceNormal.Y, Color.DarkRed);
                _primitiveBatch.DrawLine(center.X + r * distanceNormal.X, center.Y + r * distanceNormal.Y,
                                destination.X + r * distanceNormal.X, destination.Y + r * distanceNormal.Y, Color.DarkRed);
                _primitiveBatch.DrawLine(center.X - r * distanceNormal.X, center.Y - r * distanceNormal.Y,
                                destination.X - r * distanceNormal.X, destination.Y - r * distanceNormal.Y, Color.DarkRed);

            }
            _primitiveBatch.End();
        }

        protected void DrawDebugPath()
        {
            _primitiveBatch.Begin(PrimitiveType.LineList, EngineGlobals.Camera2D.GetTransformation());
            var path = ((CameraPath) SelectedObject).Path;
            var color = ((CameraPath) SelectedObject).LineColor;
            var destination = ((CameraPath) SelectedObject).EditorNewDestination;
            Vector2 tail;
            if (path.Length > 0)
            {
                var center = SelectedObject.EditorGetCenter();
                _primitiveBatch.DrawLine(center, path[0].ToVector(), color);
                for (int i = 0; i < path.Length - 1; i++)
                {
                    _primitiveBatch.DrawLine(path[i].ToVector(), path[i+1].ToVector(), color);
                }
                tail = path[path.Length - 1].ToVector();
            }
            else
            {
                tail = SelectedObject.EditorGetCenter();
            }
            if (destination.X > 0 && destination.Y > 0)
                _primitiveBatch.DrawLine(tail, destination.ToVector(), color);
            _primitiveBatch.End();
        }

        protected void DrawDebugCircle()
        {
            
            //EngineGlobals.Batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, EngineGlobals.Camera2D.GetTransformation());
            //_debugCircle.Draw();
            //EngineGlobals.Batch.End();
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            if (!_initializeDone)
                return;

            _debugInfo.Text = "Camera pos: " + EngineGlobals.Camera2D.CornerPos.X + "X " +
                              EngineGlobals.Camera2D.CornerPos.Y +
                              "Y\nMouse pos: " + MousePosReal.X + "X " + MousePosReal.Y + "Y" + "\n";
                             /* "MultiselectDrag start: " + MultiSelectionRegion.SelectionStart.X + "X " +
                              MultiSelectionRegion.SelectionStart.Y + "Y\n" +
                              "MultiselectDrag region: " + MultiSelectionRegion.SelectedRegion.X + "X " +
                              MultiSelectionRegion.SelectedRegion.Y + "Y " +
                              MultiSelectionRegion.SelectedRegion.Width + "W " +
                              MultiSelectionRegion.SelectedRegion.Height + "H";*/
            GraphicsDevice.Clear(GameGlobals.Map.BackgroundColor);

            try
            {
                if (DrawRedirect != null)
                {
                    DrawRedirect();
                    return;
                }
                if (_tempObjectPreviewImg != null)
                {

                    var x = MousePosReal.X - _tempObjectPreviewImg.Width/2;
                    if (x + _tempObjectPreviewImg.Width > EngineGlobals.Grid.WidthReal)
                        x = EngineGlobals.Grid.WidthReal - _tempObjectPreviewImg.Width;
                    else if (x < 0)
                        x = 0;
                    var y = MousePosReal.Y - _tempObjectPreviewImg.Height/2;
                    if (y + _tempObjectPreviewImg.Height > EngineGlobals.Grid.HeightReal)
                        y = EngineGlobals.Grid.HeightReal - _tempObjectPreviewImg.Height;
                    else if (y < 0)
                        y = 0;
                    TempObjectRect =
                        EngineGlobals.Grid.ToCellRectangle(new Rectangle(x, y, _tempObjectPreviewImg.Width,
                                                                         _tempObjectPreviewImg.Height));
                    _tempObjectPreviewImg.Rect = EngineGlobals.Grid.ToRealRectangle(TempObjectRect);
                    if (TempObject != null)
                        TempObject.UpdatePreviewRectangle(_tempObjectPreviewImg.Rect);
                }
                Controller.Draw();
                if (Form1.RegionEditingMode)
                {
                    EngineGlobals.Batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null,
                                              EngineGlobals.Camera2D.GetTransformation());
                    foreach (var region in GameGlobals.Map.Regions)
                    {
                        var clr = region.Color;
                        clr.A = region == SelectedRegion ? (byte) 55 : (byte) 25;
                        region.Draw(clr);
                    }
                    EngineGlobals.Batch.End();
                }
                else if (SelectedObject != null)
                {
                    if (SelectedObject.EditorIsDebugFlagEnabled(DebugFlag.Destination))
                        DrawDebugDestination();
                    else if (SelectedObject.EditorIsDebugFlagEnabled(DebugFlag.Path))
                        DrawDebugPath();
                    //if (SelectedObject.EditorIsDebugFlagEnabled(DebugFlag.Circle))
                    //    DrawDebugCircle();
                }

                EngineGlobals.Batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null,
                                          Form1.BackgroundMode
                                              ? EngineGlobals.Camera2D.GetBackgroundTransformation()
                                              : EngineGlobals.Camera2D.GetTransformation());
                if (_tempObjectPreviewImg != null)
                    _tempObjectPreviewImg.Draw();
                Selection.Draw();
                MultiSelectionRegion.Draw();
                _debugCircle.Draw();
                foreach (var selection in _selections)
                {
                    selection.Draw();
                }
                EngineGlobals.Batch.End();


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
