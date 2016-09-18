using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Engine.Core;
using Engine.Graphics;
using Engine.Mechanics;
using GameEditor.Sprite_Editor;
using GameEditor.ToolBox;
using GameLibrary;
using GameLibrary.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheGoo;
using Color = Microsoft.Xna.Framework.Color;
using Player = GameLibrary.Objects.Player;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Image = Engine.Graphics.Image;
using Orientation = GameLibrary.Objects.Orientation;
using Point = Microsoft.Xna.Framework.Point;
using RectangleF = Engine.Graphics.RectangleF;
using Region = Engine.Mechanics.Triggers.Region;

namespace GameEditor
{
    public partial class Form1 : Form
    {
        protected bool _saved;
        protected string _fileName;
        protected Dictionary<string, int> _nameCounter;
        private bool _draging;
        private Point _dragingRel;
        private bool _resizeDrag;
        private Point _lastResizePoint;
        private Rectangle _resizeRectStart;
        protected ResizeType _resizeDragType;
        protected Point _resizeDragRel;
        protected Action _tempAction;
        protected object _clipboardObject;
        protected string _clipboardObjectTypeName;
        public static TraceListener Listener;
        public bool _mouseDown;
        public Point _mouseDownPoint;
        protected bool _passiveSelection;
        protected float _zoom = 1.0f;
        protected bool _playing;
        protected bool _paused;
        protected IEditorGameEmulator _testGame;
        protected MruStripMenu _mruMenu;
        public static Form1 Instance;
        public static bool BackgroundMode;
        protected bool _rotating;
        protected Cursor _cursor;
        protected SpriteEditor _spriteEditor;
        public static string CurrentPath;
        public static bool RegionEditingMode;
        public bool _creatingRegion;
        public TriggerEditor.TriggerEditorDialog TriggerEditorDialog;
        public MapResources EditorResources;
        protected bool _settingDestination;
        protected bool _settingPath;
        protected Region _preselectedRegion;
        protected PhysicalObject _preselectedObject;
        public static bool MultiSelectMode;
        protected bool _multiSelectSelectionRegionDrag;
        protected MultiSelectionObject _multiselectionObject;
        protected Point _multiSelectDragRel;
        protected IEditorObject _multiDragObject;
        protected bool _multiDrag;
        protected bool _multiObjectsCopy;
        protected Dictionary<IEditorObject, string> _multiClipboardObjects;
        protected bool _menuMode;
        protected string _temporaryFileName;
        protected string[] _temporaryFiles;
        

        public enum TreeNodes{ GameObjects, BackgroundObjects, Regions, Resources}

        public class Action
        {
            public enum ActionType
            {
                CreateObject, DeleteObject, PropertyChange, MultiPropertyChange, DeleteRegion, CreateRegion,
                CreateMultipleObjects, DeleteMultipleObjects, MultipleObjectsMultiProperyChange
            }

            public ActionType ExecutedAction { get; protected set; }

            public IEditorObject Target;
            public IEditorObject[] Targets;

            public object PropertyBeforeChange;
            public object PropertyAfterChange;
            public string PropertyName;

            public Action(ActionType action)
            {
                ExecutedAction = action;
   }

            public override string ToString()
            {
                if (ExecutedAction == ActionType.CreateMultipleObjects || ExecutedAction == ActionType.DeleteMultipleObjects) 
                    return "Action: targets { " + string.Join(", ", "'" + Targets.Select(item => item.EditorGetName() + "'")) + " } ActionType = '" + ExecutedAction + "'";
                if (ExecutedAction == ActionType.MultipleObjectsMultiProperyChange)
                    return "Action: targets { " +
                           string.Join(", ", "'" + Targets.Select(item => item.EditorGetName() + "'")) +
                           " } ActionType = '" + ExecutedAction + "'" + " Affected properties: { " +
                           string.Join(",",
                                       "'" +
                                       ((Dictionary<IEditorObject, Dictionary<string, object>>) PropertyBeforeChange).
                                           First().Value.Keys + "'") + " }";
                if (ExecutedAction == ActionType.MultiPropertyChange)
                    return "Action: target = '" + Target.EditorGetName() + "' ActionType = '" + ExecutedAction + "'" +
                           " Affected properties: {" +
                           string.Join(",", ((Dictionary<string, object>) PropertyBeforeChange).Keys) + "}";
                return "Action: target = '" + Target.EditorGetName() + "' propertyName = '" + PropertyName +
                       "' ActionType = '" + ExecutedAction + "'";
            }
        }

        public readonly List<Action> Actions;
        public int ActionIndex;
        

        public Form1()
        {
            Instance = this;
            Listener = new TextWriterTraceListener("editor.log")
                           {TraceOutputOptions = TraceOptions.LogicalOperationStack};
            Log("=================================Starting editor=================================");
            InitializeComponent();
            _saved = true;
            _fileName = "";
            Actions = new List<Action>();
            _cursor = new Cursor("rotate.cur");
            CurrentPath = Directory.GetCurrentDirectory();
            GameGlobals.EditorMode = true;
        }

        #region FormEvents

        #region Form

        private void Form1Load(object sender, EventArgs e)
        {
            try
            {
                Log("==Form1Load==");
                _mruMenu = new MruStripMenuInline(fileToolStripMenuItem, recentFilesToolStripMenuItem, OnMruFile, "SOFTWARE\\TheGoo\\Editor\\mru", 4);
                _mruMenu.LoadFromRegistry();
                MinimumSize = Size;
                _multiselectionObject = new MultiSelectionObject();
                CreateToolboxItems();
                CreateResources();
                DictionaryEditor.CollectionEditorClosed += o => RefreshTreeRoot(TreeNodes.Resources);
                NewMap(1000, 80, MapType.Game);
                InitializePropertyComboBox();
                xnaWindow1.MouseWheel += XnaWindow1OnMouseWheel;

                if (!Directory.Exists(Path.Combine(CurrentPath, "Temp")))
                    Directory.CreateDirectory(Path.Combine(CurrentPath, "Temp"));
                _temporaryFiles = new DirectoryInfo(Path.Combine(CurrentPath, "Temp")).GetFiles().OrderByDescending(p => p.CreationTime).Select(item => item.Name).ToArray();
                CreateTemporaryItemsDropDownMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void XnaWindow1OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_playing)
                return;
            if (xnaWindow1.SelectedObject == null)
                return;
            var obj = (PhysicalObject)xnaWindow1.SelectedObject;
            if (ModifierKeys == Keys.Control)
            {
                if (obj is BoxPhysicalObject)
                {
                    var newOrientation = ((BoxPhysicalObject)obj).Orientation;
                    var delta = mouseEventArgs.Delta / SystemInformation.MouseWheelScrollDelta;
                    if (delta < 0)
                    {
                        switch (((BoxPhysicalObject) obj).Orientation)
                        {
                            case Orientation.Top:
                                newOrientation = Orientation.Right;
                                break;
                            case Orientation.Right:
                                newOrientation = Orientation.Bottom;
                                break;
                            case Orientation.Bottom:
                                newOrientation = Orientation.Left;
                                break;
                            case Orientation.Left:
                                newOrientation = Orientation.Top;
                                break;
                        }
                    }
                    else if (delta > 0)
                    {
                        switch (((BoxPhysicalObject)obj).Orientation)
                        {
                            case Orientation.Top:
                                newOrientation = Orientation.Left;
                                break;
                            case Orientation.Left:
                                newOrientation = Orientation.Bottom;
                                break;
                            case Orientation.Bottom:
                                newOrientation = Orientation.Right;
                                break;
                            case Orientation.Right:
                                newOrientation = Orientation.Top;
                                break;
                        }
                    }
                    // ReSharper disable RedundantCheckBeforeAssignment
                    if (((BoxPhysicalObject)obj).Orientation != newOrientation)
                    // ReSharper restore RedundantCheckBeforeAssignment
                        ((BoxPhysicalObject) obj).Orientation = newOrientation;
                }
                else
                {
                    ((CirclePhysicalObject) obj).Rotation += (mouseEventArgs.Delta/
                                                              SystemInformation.MouseWheelScrollDelta*5);
                }
            }
            else
            {
                var count = 0;
                count = obj.Animated
                            ? EngineGlobals.Resources.Sprites[obj.ResourceId].Count
                            : EngineGlobals.Resources.Textures[obj.ResourceId].Count;
                var delta = mouseEventArgs.Delta/SystemInformation.MouseWheelScrollDelta;
                if (obj.ResourceVariationEditor + delta < 1)
                    delta = 1;
                else if (obj.ResourceVariationEditor + delta > count)
                    delta = count;
                else
                    delta = obj.ResourceVariationEditor + delta;
                obj.ResourceVariationEditor = delta;
            }
        }

        private void Form1Resize(object sender, EventArgs e)
        {
            //xnaWindow1.Size = new Size(Width - 20 - 114, Height - 37 - 57);
            //hScrollBar1.Location = new System.Drawing.Point(hScrollBar1.Location.X, xnaWindow1.Location.Y + xnaWindow1.Height);
            //hScrollBar1.Size = new Size(xnaWindow1.Width, hScrollBar1.Height);
            //vScrollBar1.Location = new System.Drawing.Point(xnaWindow1.Location.X + xnaWindow1.Width, vScrollBar1.Location.Y);
            //vScrollBar1.Size = new Size(vScrollBar1.Width, xnaWindow1.Height);
        }

        private void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!_saved && ActionIndex != 0)
            {
                var result = MessageBox.Show("File is not saved, do You want to save it now?", "Save level?",
                                             MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                    Save(false);
                else
                    SaveTemporary();
            }
            var dialogNew = new DialogNew();
            if (dialogNew.ShowDialog() == DialogResult.OK)
            {
                var mapType = MapType.Game;
                if ((string)dialogNew.comboBox1.SelectedItem == "Background")
                    mapType = MapType.Background;
                else if ((string)dialogNew.comboBox1.SelectedItem == "Menu")
                    mapType = MapType.Menu;
                NewMap(Convert.ToInt32(dialogNew.textBox1.Text), Convert.ToInt32(dialogNew.textBox2.Text), mapType);
            }
            dialogNew.Dispose();
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!_saved && ActionIndex != 0)
            {
                var result = MessageBox.Show("File is not saved, do You want to save it now?", "Save level?",
                                             MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                    Save(false);
                else
                    SaveTemporary();
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                _fileName = openFileDialog1.FileName;
                OpenMap(openFileDialog1.FileName);
                _mruMenu.AddFile(openFileDialog1.FileName);
                _mruMenu.SaveToRegistry();
            }
        }

        private void OpenLastToolStripMenuItemClick(object sender, EventArgs e)
        {
            var files = _mruMenu.GetFiles();
            if (!files.Any())
            {
                MessageBox.Show("No recent files", "Information", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }
            if (!File.Exists(files[0]))
            {
                MessageBox.Show("The file '" + files[0] + "' cannot be found"
                    , "Warning"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Warning);
                return;
            }
            if (!_saved && ActionIndex != 0)
            {
                var result = MessageBox.Show("File is not saved, do You want to save it now?", "Save level?",
                                             MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                    Save(false);
                else
                    SaveTemporary();
            }
            _fileName = files[0];
            OpenMap(files[0]);
        }

        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            Save(false);
        }

        private void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            Save(true);
        }

        private void BringForwardToolStripMenuItemClick(object sender, EventArgs e)
        {
            ItemBringForward();
        }

        private void BringToFrontToolStripMenuItemClick(object sender, EventArgs e)
        {
            ItemBringToFront();
        }

        private void SendBackwardToolStripMenuItemClick(object sender, EventArgs e)
        {
            ItemSendBackward();
        }

        private void SendToBackToolStripMenuItemClick(object sender, EventArgs e)
        {
            ItemSendToBack();
        }

        private void ContextMenuStrip1Opening(object sender, CancelEventArgs e)
        {
            //pasteToolStripMenuItem1.Enabled = menuStripPasteButton.Enabled;
            //if (xnaWindow1.SelectedObject != null && xnaWindow1.SelectedObject.Rectangle.Contains(xnaWindow1.MousePosReal))
            //{
            //    if ((xnaWindow1.SelectedObject.LayerDepth - 0.1f) > EngineGlobals.Epsilon)
            //    {
            //        contextMenuStrip1.Items[0].Enabled = true;
            //        contextMenuStrip1.Items[1].Enabled = true;
            //    }
            //    else
            //    {
            //        contextMenuStrip1.Items[0].Enabled = false;
            //        contextMenuStrip1.Items[1].Enabled = false;
            //    }
            //    if ((0.9f - xnaWindow1.SelectedObject.LayerDepth) > EngineGlobals.Epsilon)
            //    {
            //        contextMenuStrip1.Items[2].Enabled = true;
            //        contextMenuStrip1.Items[3].Enabled = true;
            //    }
            //    else
            //    {
            //        contextMenuStrip1.Items[2].Enabled = false;
            //        contextMenuStrip1.Items[3].Enabled = false;
            //    }
            //    cutToolStripMenuItem1.Enabled = true;
            //    copyToolStripMenuItem1.Enabled = true;
            //    deleteToolStripMenuItem.Enabled = true;

            //}
            //else
            //{
            //    contextMenuStrip1.Items[0].Enabled = false;
            //    contextMenuStrip1.Items[1].Enabled = false;
            //    contextMenuStrip1.Items[2].Enabled = false;
            //    contextMenuStrip1.Items[3].Enabled = false;
            //    cutToolStripMenuItem1.Enabled = false;
            //    copyToolStripMenuItem1.Enabled = false;
            //    deleteToolStripMenuItem.Enabled = false;

            //}
            //pasteToolStripMenuItem1.Enabled = _clipboardObject != null;
        }

        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            Undo();
        }

        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            Redo();
        }

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            Copy();
        }

        private void PasteToolStripMenuItemClick(object sender, EventArgs e)
        {
            Paste(true);
        }

        private void CutToolStripMenuItemClick(object sender, EventArgs e)
        {
            Cut();
        }

        private void PasteToolStripMenuItem1Click(object sender, EventArgs e)
        {
            Paste(false);
        }

        private void ShowGridToolStripMenuItemCheckedChanged(object sender, EventArgs e)
        {
            EngineGlobals.Grid.IsHidden = !menuStripShowGridButton.Checked;
        }

        private void ZoomItemChecked(object sender, EventArgs e)
        {
            var items = new List<ToolStripMenuItem>();
            items.AddRange(new[] { zoom25, zoom50, zoom100, zoom200});
            items.Remove((ToolStripMenuItem)sender);
            ((ToolStripMenuItem) sender).Enabled = false;
            foreach (var toolStripMenuItem in items)
            {
                toolStripMenuItem.Checked = false;
                toolStripMenuItem.Enabled = true;
            }
            if (sender == zoom25)
                ChangeZoom(0.25f);
            else if (sender == zoom50)
                ChangeZoom(0.50f);
            else if (sender == zoom100)
                ChangeZoom(1f);
            else if (sender == zoom200)
                ChangeZoom(2f);
            else
                throw new Exception("Unknown zoom sender " + sender);
        }

        private void TreeView1NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 2)
            {
                if (e.Node.Parent.Parent != treeView1.Nodes[0])
                    return;
                if (BackgroundMode)
                {
                    comboBox1.SelectedIndex =
                    comboBox1.Items.IndexOf(GameGlobals.Map.BackgroundObjects.First(obj => obj.Name == e.Node.Text));
                }
                else
                {
                    var selectedObject = GameGlobals.Map.GameObjects.First(obj => obj.Name == e.Node.Text);
                    if (!(selectedObject is BackgroundObject))
                        comboBox1.SelectedIndex = comboBox1.Items.IndexOf(selectedObject);
                    
                }
                

                tabControl1.SelectTab(1);
            }

        }

        private void EditToolStripMenuItem1MouseDown(object sender, MouseEventArgs e)
        {
            menuStripPasteButton.Enabled = _clipboardObject != null || (_multiObjectsCopy);

        }

        private void MenuStripDeleteButtonClick(object sender, EventArgs e)
        {
            DeleteSelectedObject();
        }

        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            _mouseDown = false;
            if (!RegionEditingMode)
                DeleteSelectedObject();
            else
                DeleteSelectedRegion();
        }

        private void OnMruFile(int number, string filename)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("The file '" + filename + "' cannot be found and will be removed from the Recent list"
                    , "MruToolStripMenu Demo"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Warning);
                return;
            }
            _fileName = filename;
            OpenMap(filename);
        }

        private void ToolStripNewRegionButtonCheckedChanged(object sender, EventArgs e)
        {

        }

        private void ToolStripTriggerEditorButtonClick(object sender, EventArgs e)
        {
            StartTriggerEditor();
        }

        private void SetDestinationToolStripMenuItemClick(object sender, EventArgs e)
        {
            _settingDestination = true;
            if (xnaWindow1.SelectedObject is CameraPath)
            {
                _settingPath = true;
                BeginPropertyChange(xnaWindow1.SelectedObject, "Path");
                var obj = (CameraPath) xnaWindow1.SelectedObject;
                var tail = obj.Path.Length < 1 ? xnaWindow1.SelectedObject.EditorGetCenter().ToPoint() : obj.Path[obj.Path.Length - 1].ToPoint();
                var point = new Point((int)(tail.X - (xnaWindow1.Width / 2f / _zoom)), (int)(tail.Y - (xnaWindow1.Height / 2f / _zoom)));
                if (point.X + xnaWindow1.Width / _zoom > EngineGlobals.Grid.WidthReal)
                    point.X = (int)(EngineGlobals.Grid.WidthReal - xnaWindow1.Width / _zoom);
                if (point.X < 0)
                    point.X = 0;
                if (point.Y + xnaWindow1.Height / _zoom > EngineGlobals.Grid.HeightReal)
                    point.Y = (int)(EngineGlobals.Grid.HeightReal - xnaWindow1.Height / _zoom);
                if (point.Y < 0)
                    point.Y = 0;
                hScrollBar1.Value = (int)(point.X / (float)EngineGlobals.Grid.CellWidth * _zoom);
                vScrollBar1.Value = (int)(point.Y / (float)EngineGlobals.Grid.CellHeight * _zoom);
                xnaWindow1.Focus();
            }
            else
                BeginPropertyChange(xnaWindow1.SelectedObject, "EditorDestination");
        }

        private void TriggerEditorToolStripMenuItemClick(object sender, EventArgs e)
        {
            StartTriggerEditor();
        }

        protected void CreateTemporaryItemsDropDownMenu()
        {
            menuStripRecoverButton.DropDownItems.Clear();
            foreach (var item in _temporaryFiles.Select(temporaryFile => new ToolStripMenuItem(temporaryFile)))
            {
                item.Click += ItemOnClick;
                menuStripRecoverButton.DropDownItems.Add(item);
            }
            toolStripRecoverButton.Enabled = _temporaryFiles.Length > 0;
            menuStripRecoverButton.Enabled = _temporaryFiles.Length > 0;
        }

        private void ItemOnClick(object sender, EventArgs eventArgs)
        {
            var item = ((ToolStripMenuItem) sender);
            if (item.Text != _temporaryFileName)
            {
                if (!_saved && ActionIndex != 0)
                {
                    var result = MessageBox.Show("File is not saved, do You want to save it now?", "Save level?",
                                                 MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Cancel)
                        return;
                    if (result == DialogResult.Yes)
                        Save(false);
                    else
                        SaveTemporary();
                }
            }
            LoadTemporaryFile(item.Text);
        }

        #endregion

        #region XnaWindow

        private void XnaWindow1MouseMove(object sender, MouseEventArgs e)
        {
            UpdateMouse(e.X, e.Y);
            if (_playing)
                return;
            if (RegionEditingMode)
            {
                if (_mouseDown)
                {
                    if (!_resizeDrag && !_draging)
                    {
                        if (toolStripNewRegionButton.Checked)
                        {
                            if (Math.Abs(xnaWindow1.MousePosReal.X - _mouseDownPoint.X) >= EngineGlobals.Grid.CellWidth &&
                                Math.Abs(xnaWindow1.MousePosReal.Y - _mouseDownPoint.Y) >= EngineGlobals.Grid.CellHeight)
                            {
                                EditorBeginCreateRegion(EngineGlobals.Grid.ToAlignedPoint(_mouseDownPoint),
                                                        EngineGlobals.Grid.ToAlignedPoint(xnaWindow1.MousePosReal));
                                xnaWindow1.SelectedRegion.BeginResize();
                                _resizeDrag = true;
                                _resizeDragRel = _mouseDownPoint;
                                _resizeDragType = ResizeType.BottomRight;
                                _resizeRectStart = xnaWindow1.SelectedRegion.Rectangle.GetRectangle();
                            }
                        }
                        else if (xnaWindow1.SelectedRegion != null && xnaWindow1.SelectedRegion.Rectangle.Contains(_mouseDownPoint))
                        {
                            
                            var resizeType = GetResizeType(_mouseDownPoint.X, _mouseDownPoint.Y, xnaWindow1.SelectedRegion);
                            if (resizeType != ResizeType.None)
                            {
                                BeginPropertyChange(xnaWindow1.SelectedRegion, "Rectangle");
                                _lastResizePoint = _mouseDownPoint;
                                _resizeDragType = resizeType;
                                _resizeRectStart = xnaWindow1.SelectedRegion.Rectangle.GetRectangle();
                                _resizeDrag = true;
                            }
                            else
                            {
                                _draging = true;
                                var rect = xnaWindow1.SelectedRegion.Rectangle.GetRectangle();
                                _dragingRel =
                                    new Point(_mouseDownPoint.X - (rect.X + rect.Width/2),
                                              _mouseDownPoint.Y -
                                              (rect.Y + rect.Height/2));
                                Log("Starting drag: dragingRel = " + _dragingRel);
                                BeginPropertyChange(xnaWindow1.SelectedRegion, "Rectangle");
                            }
                        }
                    }
                    else if (_resizeDrag)
                    {
                        var rectangle = xnaWindow1.SelectedRegion.Rectangle.GetRectangle();
                        _lastResizePoint = xnaWindow1.MousePosCell;
                        ResizeObject(_resizeDragType, xnaWindow1.SelectedRegion, ref rectangle);
                        xnaWindow1.SelectedRegion.DoResize(_resizeDragType, _resizeRectStart, rectangle);
                        
                    }
                    else if (_draging)
                    {
                        var rect = xnaWindow1.SelectedRegion.Rectangle.GetRectangle();
                        var x = xnaWindow1.MousePosReal.X - rect.Width / 2 - _dragingRel.X;
                        if (x + rect.Width > EngineGlobals.Grid.WidthReal)
                            x = EngineGlobals.Grid.WidthReal - rect.Width;
                        else if (x < 0)
                            x = 0;
                        var y = xnaWindow1.MousePosReal.Y - rect.Height / 2 - _dragingRel.Y;
                        if (y + rect.Height > EngineGlobals.Grid.HeightReal)
                            y = EngineGlobals.Grid.HeightReal - rect.Height;
                        else if (y < 0)
                            y = 0;

                        xnaWindow1.SelectedRegion.Rectangle = new RectangleF(EngineGlobals.Grid.ToAlignedRectangle(new Rectangle(x, y, rect.Width, rect.Height)));
                        return;
                    }
                }
                else
                {
                    
                    CheckResizeCursor(null);
                }
                return;
            }
            if (_rotating)
            {
                Cursor = _cursor;
                var rotatingObj = (CirclePhysicalObject) xnaWindow1.SelectedObject;
                var dist = rotatingObj.HalfPos - new Vector2(xnaWindow1.MousePosReal.X, xnaWindow1.MousePosReal.Y);
                var angle = (float)Math.Atan2(dist.Y, dist.X);

                rotatingObj.Rotation = (int)MathHelper.ToDegrees(angle);
                return;
            }
            if (_settingDestination)
            {
                if (_settingPath)
                {
                    ((CameraPath)xnaWindow1.SelectedObject).EditorNewDestination = new PathPoint((int)EngineGlobals.Camera2D.Position.X,
                                                                          (int)EngineGlobals.Camera2D.Position.Y, ((CameraPath)xnaWindow1.SelectedObject).DefaultSpeed);
                }
                else
                    xnaWindow1.SelectedObject.EditorDestination = new Vector2(xnaWindow1.MousePosReal.X,
                                                                          xnaWindow1.MousePosReal.Y);
                return;
            }
            if (_multiSelectSelectionRegionDrag)
            {
                xnaWindow1.MultiSelectionRegion.SetSelectedRegionEnd(xnaWindow1.MousePosReal.ToVector());
                return;

            }
            if (!_draging && !_resizeDrag && !_multiDrag && _mouseDown)
            {
                if (!MultiSelectMode)
                {
                    var resizeType = ResizeType.None;
                    var attributes =
                        TypeDescriptor.GetProperties(xnaWindow1.SelectedObject)["GridSize"].Attributes;
                    if (!attributes[typeof (ReadOnlyAttribute)].Equals(ReadOnlyAttribute.Yes))
                        resizeType = GetResizeType(_mouseDownPoint.X, _mouseDownPoint.Y, xnaWindow1.SelectedObject);
                    if (resizeType != ResizeType.None)
                    {
                        _resizeDragType = resizeType;
                        _resizeDragRel = _mouseDownPoint;
                        _lastResizePoint = _mouseDownPoint;
                        _resizeRectStart = xnaWindow1.SelectedObject.Rectangle.GetRectangle();
                        _resizeDrag = true;
                        Log("Starting resize: ResizeType = " + resizeType + " ResizeDragRel = " + _resizeDragRel +
                            " LastResizePoint = " + _lastResizePoint);
                        BeginPropertyChange(xnaWindow1.SelectedObject, "Rectangle");
                    }
                    else
                    {
                        _draging = true;
                        var rect = xnaWindow1.SelectedObject.Rectangle.GetRectangle();
                        _dragingRel =
                            new Point(_mouseDownPoint.X - (rect.X + rect.Width/2),
                                      _mouseDownPoint.Y -
                                      (rect.Y + rect.Height/2));
                        Log("Starting drag: dragingRel = " + _dragingRel);
                        BeginPropertyChange(xnaWindow1.SelectedObject, "Pos");
                    }
                    timer1.Start();
                }
                else
                {
                    _multiDrag = true;
                    BeginMultiObjectsMultiPropertyChange(xnaWindow1.MultiSelectedObjects, new[]{"Rectangle"});
                    timer1.Start();
                }
            }
            if (_draging)
            {
                var rect = xnaWindow1.SelectedObject.Rectangle.GetRectangle();
                var x = xnaWindow1.MousePosReal.X - rect.Width / 2 - _dragingRel.X;
                if (x + rect.Width > EngineGlobals.Grid.WidthReal)
                    x = EngineGlobals.Grid.WidthReal - rect.Width;
                else if (x < 0)
                    x = 0;
                var y = xnaWindow1.MousePosReal.Y - rect.Height / 2 - _dragingRel.Y;
                if (y + rect.Height > EngineGlobals.Grid.HeightReal)
                    y = EngineGlobals.Grid.HeightReal - rect.Height;
                else if (y < 0)
                    y = 0;
                var newRect = new RectangleF(EngineGlobals.Grid.ToAlignedRectangle(new Rectangle(x, y, rect.Width, rect.Height)));


                    xnaWindow1.SelectedObject.Rectangle = newRect;
                    xnaWindow1.RefreshSelection();

                return;
            }
            if (_multiDrag)
            {
                var rect = _multiDragObject.Rectangle.GetRectangle();
                //var x = xnaWindow1.MousePosReal.X - rect.Width / 2 - _dragingRel.X;
                //if (x + rect.Width > EngineGlobals.Grid.WidthReal)
                //    x = EngineGlobals.Grid.WidthReal - rect.Width;
                //else if (x < 0)
                //    x = 0;
                //var y = xnaWindow1.MousePosReal.Y - rect.Height / 2 - _dragingRel.Y;
                //if (y + rect.Height > EngineGlobals.Grid.HeightReal)
                //    y = EngineGlobals.Grid.HeightReal - rect.Height;
                //else if (y < 0)
                //    y = 0;
                var diff = new Vector2(xnaWindow1.MousePosReal.X,xnaWindow1.MousePosReal.Y) - new Vector2(_multiDragObject.Rectangle.X + rect.Width/2 + _dragingRel.X,
                                       _multiDragObject.Rectangle.Y + rect.Height / 2 + _dragingRel.Y);
                foreach (var multiSelectedObject in xnaWindow1.MultiSelectedObjects)
                {
                    var newPos = new Vector2(multiSelectedObject.Rectangle.X,
                                             multiSelectedObject.Rectangle.Y) + diff;
                    if (newPos.X + multiSelectedObject.Rectangle.Width > EngineGlobals.Grid.WidthReal)
                        newPos.X = EngineGlobals.Grid.WidthReal - multiSelectedObject.Rectangle.Width;
                    else if (newPos.X < 0)
                        newPos.X = 0;
                    if (newPos.Y + multiSelectedObject.Rectangle.Height > EngineGlobals.Grid.HeightReal)
                        newPos.Y = EngineGlobals.Grid.HeightReal - multiSelectedObject.Rectangle.Height;
                    else if (newPos.Y < 0)
                        newPos.Y = 0;
                    multiSelectedObject.Rectangle =
                        new RectangleF(
                            EngineGlobals.Grid.ToAlignedRectangle(
                                new RectangleF(newPos.X, newPos.Y, multiSelectedObject.Rectangle.Width,
                                               multiSelectedObject.Rectangle.Height).GetRectangle()));
                }
                xnaWindow1.RefreshSelection();
                return;
            }
            if (_resizeDrag)
            {
                if (xnaWindow1.MousePosCell == _lastResizePoint)
                    return;
                var rectangle =xnaWindow1.SelectedObject.Rectangle.GetRectangle();
                _lastResizePoint = xnaWindow1.MousePosCell;
                ResizeObject(_resizeDragType, xnaWindow1.SelectedObject, ref rectangle);
                xnaWindow1.SelectedObject.DoResize(_resizeDragType, _resizeRectStart, rectangle);
                xnaWindow1.RefreshSelection();
                #region INPORTANT-DO RESIZE
                
                //if (xnaWindow1.SelectedObject.CollidingType == CollidingObjectType.Circle)
                //{
                //    if (_resizeDragType == ResizeType.TopLeft)
                //    {
                //        if (_resizeRectStart.X - rectangle.X > _resizeRectStart.Y - rectangle.Y)
                //        {
                //            rectangle.Y = _resizeRectStart.Y - (_resizeRectStart.X - rectangle.X);
                //             rectangle.Height = rectangle.Width;
                //        }
                //        else if (_resizeRectStart.Y - rectangle.Y > _resizeRectStart.X - rectangle.X)
                //        {
                //            rectangle.X = _resizeRectStart.X - (_resizeRectStart.Y - rectangle.Y);
                //            rectangle.Width = rectangle.Height;
                //        }

                //        if (rectangle.Y < 0)
                //        {
                //            rectangle.Y = 0;
                //            rectangle.Height = _resizeRectStart.Y + _resizeRectStart.Height;
                //            rectangle.X = _resizeRectStart.X - _resizeRectStart.Y;
                //            rectangle.Width = rectangle.Height;
                //        }
                //        else if (rectangle.X < 0)
                //        {
                //            rectangle.X = 0;
                //            rectangle.Width = _resizeRectStart.X + _resizeRectStart.Width;
                //            rectangle.Y = _resizeRectStart.Y - _resizeRectStart.X;
                //            rectangle.Height = rectangle.Width;
                //        }
                //    }
                //    else if (_resizeDragType == ResizeType.TopRight)
                //    {

                //        if (rectangle.Width > rectangle.Height)
                //        {
                //            rectangle.Y = _resizeRectStart.Y + _resizeRectStart.Height - rectangle.Width;
                //            rectangle.Height = rectangle.Width;
                //        }
                //        else if (rectangle.Width < rectangle.Height)
                //        {
                //            rectangle.Width = rectangle.Height;
                //        }

                //        if (rectangle.Y < 0)
                //        {
                //            rectangle.Y = 0;
                //            rectangle.Height = _resizeRectStart.Y + _resizeRectStart.Height;
                //            rectangle.Width = rectangle.Height;
                //        }
                //        if (rectangle.X + rectangle.Width > EngineGlobals.Grid.WidthReal)
                //        {
                //            rectangle.Width = EngineGlobals.Grid.WidthReal - rectangle.X;
                //            rectangle.Y = _resizeRectStart.Y + _resizeRectStart.Height - rectangle.Width;
                //            rectangle.Height = rectangle.Width;
                //        }

                //    }
                //    else if (_resizeDragType == ResizeType.BottomLeft)
                //    {
                //        if (rectangle.Height > rectangle.Width)
                //        {
                //            rectangle.X = _resizeRectStart.X + _resizeRectStart.Width - rectangle.Height;
                //            rectangle.Width = rectangle.Height;
                //        }
                //        else if (rectangle.Height < rectangle.Width)
                //        {
                //            rectangle.Height = rectangle.Width;
                //        }

                //        if (rectangle.X < 0)
                //        {
                //            rectangle.X = 0;
                //            rectangle.Width = _resizeRectStart.X + _resizeRectStart.Width;
                //            rectangle.Height = rectangle.Width;
                //        }
                //        if (rectangle.Y + rectangle.Height > EngineGlobals.Grid.HeightReal)
                //        {
                //            rectangle.Height = EngineGlobals.Grid.HeightReal - rectangle.Y;
                //            rectangle.X = _resizeRectStart.X + _resizeRectStart.Width - rectangle.Height;
                //            rectangle.Width = rectangle.Height;
                //        }
                //    }
                //    else
                //    {
                //        if (rectangle.Width > rectangle.Height)
                //        {
                //            rectangle.Height = rectangle.Width;
                //        }
                //        else if (rectangle.Width < rectangle.Height)
                //        {
                //            rectangle.Width = rectangle.Height;
                //        }
                //    }
                    
                //}
                //if (rectangle.X + rectangle.Width > EngineGlobals.Grid.WidthReal)
                //{
                //    rectangle.Width = EngineGlobals.Grid.WidthReal - rectangle.X;
                //    if (xnaWindow1.SelectedObject.CollidingType == CollidingObjectType.Circle)
                //        rectangle.Height = rectangle.Width;
                //}
                //if (rectangle.Y + rectangle.Height > EngineGlobals.Grid.HeightReal)
                //{
                //    rectangle.Height = EngineGlobals.Grid.HeightReal - rectangle.Y;
                //    if (xnaWindow1.SelectedObject.CollidingType == CollidingObjectType.Circle)
                //        rectangle.Width = rectangle.Height;
                //}
                //xnaWindow1.SelectedObject.SetCornerRectangle(EngineGlobals.Grid.ToAlignedRectangle(rectangle));
                //if (_lastResizeRegRect.Width != 0 && _lastResizeRegRect.Width != rectangle.Width)
                #endregion
                //xnaWindow1.RefreshSelection();
                //_lastResizeRegRect.Width = rectangle.Width;
                return;
            }
            CheckResizeCursor(xnaWindow1.SelectedObject);
        }

        public void ResizeObject(ResizeType resizeType, IEditorObject resizeObj, ref Rectangle rectangle)
        {
            switch (resizeType)
            {
                case ResizeType.Right:
                    DoResize(ResizeType.Right, ref rectangle);
                    break;
                case ResizeType.Bottom:
                    DoResize(ResizeType.Bottom, ref rectangle);
                    break;
                case ResizeType.Top:
                    DoResize(ResizeType.Top, ref rectangle);
                    break;
                case ResizeType.Left:
                    DoResize(ResizeType.Left, ref rectangle);
                    break;
                case ResizeType.BottomRight:
                    DoResize(ResizeType.Bottom, ref rectangle);
                    DoResize(ResizeType.Right, ref rectangle);
                    break;
                case ResizeType.TopLeft:
                    DoResize(ResizeType.Top, ref rectangle);
                    DoResize(ResizeType.Left, ref rectangle);
                    break;
                case ResizeType.TopRight:
                    DoResize(ResizeType.Top, ref rectangle);
                    DoResize(ResizeType.Right, ref rectangle);
                    break;
                case ResizeType.BottomLeft:
                    DoResize(ResizeType.Bottom, ref rectangle);
                    DoResize(ResizeType.Left, ref rectangle);
                    break;
            }
        }

        private void XnaWindow1DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof (ToolboxType)))
            {

                e.Effect = DragDropEffects.All;
                var p = xnaWindow1.PointToClient(new System.Drawing.Point(e.X, e.Y));
                UpdateMouse(p.X, p.Y);
                xnaWindow1.Refresh();
            }
        }

        private void XnaWindow1DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(typeof(ToolboxType))) return;
                Log("==DragDrop==");
                Log("Removing Temp object from XnaWindow");
                xnaWindow1.TempObject.HideDetailImages(false);
                xnaWindow1.RemoveObjectPreview(BackgroundMode);
                timer1.Stop();
                AddObject(xnaWindow1.TempObject);
                xnaWindow1.Focus();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }

        private void XnaWindow1DragEnter(object sender, DragEventArgs e)
        {
            Log("Drag Enter");
            if (!e.Data.GetDataPresent(typeof (ToolboxType))) return;
            var newObject = ((ToolboxType) e.Data.GetData(typeof (ToolboxType)));
            var type = newObject.ObjectTypeName;
            var resourceId = newObject.ResourceIdentifier.Name;
            Log("Object type = '" + type + "'");
            Log("Object resource type='" + newObject.ResourceIdentifier.ResourceType + "' ResourceId='" + newObject.ResourceIdentifier.Name + "'");
            AddResource(newObject.ResourceIdentifier);
            timer1.Start();
            Log("Creating Temp object for XnaWindow");
            xnaWindow1.TempObject = GameGlobals.Map.CreateObjectVirtual(type, resourceId, newObject.SubObjectType);
            xnaWindow1.CreateObjectPreview(xnaWindow1.TempObject, BackgroundMode);
        }

        private void XnaWindow1DragLeave(object sender, EventArgs e)
        {
            Log("Drag leave");
            timer1.Stop();
            if (xnaWindow1.TempObject is DeathBall)
                (xnaWindow1.TempObject as DeathBall).RemoveDetails();
            xnaWindow1.RemoveObjectPreview(BackgroundMode);
        }

        private void XnaWindow1DoubleClick(object sender, EventArgs e)
        {
            if (_playing)
                return;
            var selection = xnaWindow1.CheckSelection();
            if (selection != null)
            {
                Log("DoubleClick object selected: '" + selection.EditorGetName() + "'");
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(selection);
                tabControl1.SelectedIndex = 1;
                tabControl1.TabPages[1].Focus();
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void XnaWindow1MouseDown(object sender, MouseEventArgs e)
        {
            UpdateMouse(e.X, e.Y);
            if (_rotating)
            {
                _rotating = false;
                propertyGrid1.Refresh();
                EndPorperyChange();
                Log("Mouse down and rotating, stoped rotate");
                return;
            }
            if (_settingDestination)
            {
                _settingDestination = false;
                if (xnaWindow1.SelectedObject is CameraPath)
                {
                    var currentPath = ((CameraPath) xnaWindow1.SelectedObject).Path;
                    var newPath = new PathPoint[currentPath.Length + 1];
                    for (int i = 0; i < currentPath.Length; i++)
                    {
                        newPath[i] = currentPath[i];
                    }
                    newPath[currentPath.Length] = ((CameraPath) xnaWindow1.SelectedObject).EditorNewDestination;
                    ((CameraPath) xnaWindow1.SelectedObject).Path = newPath;
                    ((CameraPath) xnaWindow1.SelectedObject).EditorNewDestination = new PathPoint(0, 0,
                                                                                                  ((CameraPath) xnaWindow1.SelectedObject).
                                                                                                      DefaultSpeed);
                }
                propertyGrid1.Refresh();
                EndPorperyChange();
                Log("Mouse down and setting destination, stop destination set");
                return;
            }
            if (_playing && e.Button == MouseButtons.Left)
            {
                if (xnaWindow1.ClientRectangle.Contains(e.X, e.Y))
                    _testGame.InputOnPress(new Point(e.X, e.Y), 0);
                return;
            }
            if (_playing)
                return;

            var selection = xnaWindow1.CheckSelection();
            
            if (RegionEditingMode)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _mouseDown = true;
                    _mouseDownPoint = xnaWindow1.MousePosReal;
                }
                if (selection != null)
                {
                    _passiveSelection = true;
                    comboBox1.SelectedItem = selection;
                    comboBox1.SelectedIndex =
                        comboBox1.Items.IndexOf(selection);
                    CheckResizeCursor(null);
                }
                else
                {
                    comboBox1.SelectedItem = GameGlobals.Map;
                }
                return;

            }
            if (selection == null)
            {
                comboBox1.SelectedIndex = 0;
                if (e.Button == MouseButtons.Left)
                {
                    Log("Mouse down - multiselect drag");
                    _multiSelectSelectionRegionDrag = true;
                    xnaWindow1.MultiSelectionRegion.StartSelection(xnaWindow1.MousePosReal.ToVector());
                    xnaWindow1.MultiSelectionRegion.IsHidden = false;
                    timer1.Start();
                    DeselectObject();
                }
                return;
            }
            if (xnaWindow1.SelectedObject != null)
            {
                if (xnaWindow1.SelectedObject.Rectangle.Contains(xnaWindow1.MousePosReal) && e.Button == MouseButtons.Left)
                {
                    Log("Mouse down on selected object(" + xnaWindow1.SelectedObject.EditorGetName() + ")");
                    _mouseDown = true;
                    _mouseDownPoint = xnaWindow1.MousePosReal;
                    return;
                }
                
            }
            if (MultiSelectMode && xnaWindow1.MultiSelectedObjects.Contains(selection))
            {
                Log("Mouse down on multiselected object");
                _mouseDown = true;
                _dragingRel = new Vector2(xnaWindow1.MousePosReal.X - (selection.Rectangle.X + selection.Rectangle.Width / 2),
                                      xnaWindow1.MousePosReal.Y -
                                      (selection.Rectangle.Y + selection.Rectangle.Height / 2)).ToPoint();
                _multiDragObject = selection;
                return;
            }
            Log("Mouse down on not selected object");
            
            _passiveSelection = true;
            comboBox1.SelectedIndex =
                    comboBox1.Items.IndexOf(selection);
            if (e.Button == MouseButtons.Left)
            {
                _mouseDown = true;
                _mouseDownPoint = xnaWindow1.MousePosReal;
            }
            CheckResizeCursor(xnaWindow1.SelectedObject);
        }

        private void XnaWindow1MouseLeave(object sender, EventArgs e)
        {
            if (_playing)
                return;
            _draging = false;
            _resizeDrag = false;
            if (_draging || _resizeDrag)
            {
                if (RegionEditingMode)
                {
                    if (_creatingRegion)
                        EndCreatingRegion();
                    else
                        EndPorperyChange();
                }
                else
                    EndPorperyChange();
            }
            if (_multiDrag)
            {
                EndMultiObjectsMultiPropertyChange();
            }
            timer1.Stop();
            Cursor = Cursors.Arrow;
        }

        private void XnaWindow1MouseUp(object sender, MouseEventArgs e)
        {
            if (_playing)
                return;
            if (e.Button == MouseButtons.Left)
            {
                
                if (_draging || _resizeDrag)
                {
                    Log("MousUp and was " + (_draging?"draging":"resizing") );
                    timer1.Stop();
                    if (RegionEditingMode)
                    {
                        if (_creatingRegion)
                            EndCreatingRegion();
                        else
                            EndPorperyChange();
                    }
                    else
                        EndPorperyChange();
                }
                else if (_multiDrag)
                {
                    Log("MousUp and was multi draging objects");
                    _multiDrag = false;
                    EndMultiObjectsMultiPropertyChange();
                    timer1.Stop();
                }
                else if (_multiSelectSelectionRegionDrag)
                {
                    _multiSelectSelectionRegionDrag = false;
                    timer1.Stop();
                    xnaWindow1.MultiSelectionRegion.IsHidden = true;
                    MultiSelectObjects(xnaWindow1.MultiSelectionRegion.SelectedRegion.GetRectangle());
                }
                else
                {
                    Log("Mouse left button Up");
                }
                _draging = false;
                _resizeDrag = false;
                _mouseDown = false;
                
            }

        }

        #endregion

        #region Other

        private void HScrollBar1Scroll(object sender, ScrollEventArgs e)
        {
            EngineGlobals.Camera2D.CornerPos = new Vector2(e.NewValue * (10 / _zoom), EngineGlobals.Camera2D.CornerPos.Y);
            xnaWindow1.Invalidate();

        }

        private void HScrollBar1ValueChanged(object sender, EventArgs e)
        {
            EngineGlobals.Camera2D.CornerPos = new Vector2(hScrollBar1.Value * (10 / _zoom), EngineGlobals.Camera2D.CornerPos.Y);
            var mousePos = xnaWindow1.PointToClient(MousePosition);
            xnaWindow1.ExecuteMouseMove(new MouseEventArgs(MouseButtons.None, 0, mousePos.X, mousePos.Y, 0));
        }

        private void VScrollBar1Scroll(object sender, ScrollEventArgs e)
        {
            EngineGlobals.Camera2D.CornerPos = new Vector2(EngineGlobals.Camera2D.CornerPos.X,
                                                                              e.NewValue * (10 / _zoom));
            xnaWindow1.Invalidate();
        }

        private void VScrollBar1ValueChanged(object sender, EventArgs e)
        {
            EngineGlobals.Camera2D.CornerPos = new Vector2(EngineGlobals.Camera2D.CornerPos.X,
                                                                              vScrollBar1.Value*(10/_zoom));
            var mousePos = xnaWindow1.PointToClient(MousePosition);
            xnaWindow1.ExecuteMouseMove(new MouseEventArgs(MouseButtons.None, 0, mousePos.X, mousePos.Y, 0));
        }

        private void ComboBox1SelectedIndexChanged1(object sender, EventArgs e)
        {
            if (!contextMenuStrip1.Enabled)
                return;
            Log("==ComboBoxSelectedIndexChanged==");
            if (comboBox1.SelectedItem is Region && !RegionEditingMode)
            {
                _preselectedRegion = (Region)comboBox1.SelectedItem;
                toolStripRegionEditorButton.Checked = true;
            }
            else if (comboBox1.SelectedItem is PhysicalObject && RegionEditingMode)
            {
                _preselectedObject = (PhysicalObject) comboBox1.SelectedItem;
                toolStripRegionEditorButton.Checked = false;
            }
            else if (comboBox1.SelectedItem is MultiSelectionObject)
                return;
            if (RegionEditingMode)
            {

                if (!(comboBox1.SelectedItem is Region) && !(comboBox1.SelectedItem is Map))
                {
                    MessageBox.Show("Unable to select object in Regions Editing Mode. Stop Regions Editing Mode first.");
                    comboBox1.SelectedItem = GameGlobals.Map;
                    return;
                }
                else if (comboBox1.SelectedItem is Region)
                {
                    Log("Selected region");
                    SelectRegion((Region)comboBox1.SelectedItem);
                    return;
                }
                else
                {
                    Log("Chosen item is map, deselect previously selected object");
                    DeselectRegion();
                    propertyGrid1.SelectedObject = comboBox1.SelectedItem;
                    return;
                }
            }
            propertyGrid1.SelectedObject = comboBox1.SelectedItem;
            if (MultiSelectMode)
            {
                DeselectMultiSelectedObjects(comboBox1.SelectedItem);
            }
            if (comboBox1.SelectedItem != GameGlobals.Map)
            {
                Log("Selected new item");
                SelectItem((PhysicalObject)comboBox1.SelectedItem);
            }
            else
            {
                Log("Chosen item is map, deselect previously selected object");
                DeselectObject();
                menuStripCutButton.Enabled = false;
                menuStripCopyButton.Enabled = false;
            }
        }

        private void Timer1Tick(object sender, EventArgs e)
        {
            var mousePos = xnaWindow1.PointToClient(MousePosition);
            if (mousePos.X > xnaWindow1.ClientRectangle.Width - 20)
            {
                if (hScrollBar1.Value < hScrollBar1.Maximum - 9)
                    hScrollBar1.Value++;
            }
            else if (mousePos.X < 20 && hScrollBar1.Value > hScrollBar1.Minimum)
                hScrollBar1.Value--;
            if (mousePos.Y > xnaWindow1.ClientRectangle.Height - 20 && vScrollBar1.Value < vScrollBar1.Maximum - 9)
                vScrollBar1.Value++;
            else if (mousePos.Y < 20 && vScrollBar1.Value > vScrollBar1.Minimum)
                vScrollBar1.Value--;
        }

        #endregion

        #endregion

        #region methods

        protected void ResetData()
        {
            menuStripUndoButton.Enabled = false;
            toolStripUndoButton.Enabled = false;
            menuStripRedoButton.Enabled = false;
            toolStripRedoButton.Enabled = false;
            toolStripPasteButton.Enabled = false;
            RefreshMenus();
            ActionIndex = 0;
            Actions.Clear();
            hScrollBar1.Maximum = (EngineGlobals.Grid.GridRectangle.Width - 80) + 9;
            hScrollBar1.Value = 0;
            hScrollBar1.SmallChange = 1;
            hScrollBar1.LargeChange = 10;
            vScrollBar1.Maximum = (EngineGlobals.Grid.GridRectangle.Height - 48) + 9;
            vScrollBar1.Value = 0;
            vScrollBar1.SmallChange = 1;
            vScrollBar1.LargeChange = 10;
            Log("Name counter reset");
            _nameCounter = new Dictionary<string, int>();
            xnaWindow1.Focus();
            RefreshTreeRoot(TreeNodes.Resources);
        }

        protected void CreateTemporary()
        {
            _temporaryFileName = (string.IsNullOrEmpty(_fileName) ? string.Empty : Path.GetFileName(_fileName)) + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "." +
                                 Guid.NewGuid() + ".tmp";
            if (_temporaryFiles.Length > 2)
            {
                var info = new DirectoryInfo(Path.Combine(CurrentPath, "Temp"));
                var files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                var toDelete = files.Take(files.Length - 2).ToArray();
                foreach (var file in toDelete)
                {
                    file.Delete();
                }
                
            }
            var tempFiles = new DirectoryInfo(Path.Combine(CurrentPath, "Temp")).GetFiles().OrderByDescending(p => p.CreationTime).Select(item => item.Name).ToList();
            tempFiles.Insert(0, _temporaryFileName);
            _temporaryFiles = tempFiles.ToArray();

            

        }

        protected void SaveTemporary()
        {
            if (string.IsNullOrEmpty(_temporaryFileName))
                CreateTemporary();
            var stream = File.Open(Path.Combine(CurrentPath, "Temp", _temporaryFileName), FileMode.Create, FileAccess.ReadWrite);
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.WriteLine((string.IsNullOrEmpty(_fileName)? string.Empty : _fileName));
                var ms = new MemoryStream();
                xnaWindow1.GameLevel.Save(ms, true);
                streamWriter.Write(Encoding.UTF8.GetString(ms.ToArray()));
                ms.Close();
            }
        }

        protected void LoadTemporaryFile(string fname)
        {
            var path = Path.Combine(CurrentPath, "Temp", fname);
            Stream stream = File.Open(path, FileMode.Open);
            var originalPath = string.Empty;
            using (var reader = new StreamReader(stream))
            {
                originalPath = reader.ReadLine();
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd()));
                OpenMap(originalPath, ms, path);
                ms.Close();
            }

        }

        protected void Save(bool saveAs)
        {
            Log("==Save==");
            
            if (saveAs || _fileName == "")
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Log("Saving map to: '" + saveFileDialog1.FileName + "'");
                    var stream = File.Open(saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite);
                    xnaWindow1.GameLevel.Save(stream);
                    stream.Close();
                    _fileName = saveFileDialog1.FileName;
                    _mruMenu.AddFile(_fileName);
                    _mruMenu.SaveToRegistry();
                    Text = Path.GetFileName(_fileName) + " - Game Editor";
                    _saved = true;
                    SaveTemporary();
                }
                else
                {
                    SaveTemporary();
                }
            }
            else
            {
                Log("Saving map to: '" + _fileName + "'");
                var stream = File.Open(_fileName, FileMode.Create, FileAccess.ReadWrite);
                xnaWindow1.GameLevel.Save(stream);
                stream.Close();
                _saved = true;
            }
        }

        protected void InitializePropertyComboBox()
        {

        }

        protected void CreateToolboxItems()
        {
            Log("==CreateToolboxItems==");
            Logl("Add group: ");
            var group = new ToolboxGroup("All game objects");
            Log(group.Caption, false);
            group.Items.Add(new ToolboxItem("Player", imageList1.Images.IndexOfKey("Player.png"),
                new ToolboxType(GameObjectType.Player,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.Player.ToString(),
                        Path = new[] {"GameObjects/Sprites/Player/Player"},
                        ResourceType = ResourceType.Sprite
                    })));
            var pathResource = new ResourceIdentifier("Path", new[]
            {
                "GameObjects/Images/Path/Path",
                "GameObjects/Images/Path/PathEnd"
            }, ResourceType.Texture);
            var circleResource = new ResourceIdentifier
            {
                Name = GameObjectType.Circle.ToString(),
                Path =
                    new[]
                    {
                        "GameObjects/Images/Circle_1",
                        "GameObjects/Images/Circle_2",
                        "GameObjects/Images/Circle_3",
                        "GameObjects/Images/Circle_4",
                        "GameObjects/Images/Circle_5",
                        "GameObjects/Images/Circle_6"
                    },
                ResourceType = ResourceType.Texture,
                SubResources = new[]
                {
                    pathResource
                }
            };
            group.Items.Add(new ToolboxItem("Circle", imageList1.Images.IndexOfKey("Circle.png"),
                new ToolboxType(GameObjectType.Circle, circleResource
                    )));
            group.Items.Add(new ToolboxItem("MovingCircle", imageList1.Images.IndexOfKey("Circle.png"),
                new ToolboxType(GameObjectType.MovingCircle, circleResource
                    )));
            /*group.Items.Add(new ToolboxItem("Tile", imageList1.Images.IndexOfKey("Tile.png"),
                                            new ToolboxType(GameObjectType.Tile,
                                                            new ResourceIdentifier
                                                                {
                                                                    Name = GameObjectType.Tile.ToString(),
                                                                    Path = new[] {"GameObjects/Images/Tile"},
                                                                    ResourceType = ResourceType.Texture
                                                                })));*/
            group.Items.Add(new ToolboxItem("Plane", imageList1.Images.IndexOfKey("Plane.png"),
                new ToolboxType(GameObjectType.Plane,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.Plane.ToString(),
                        Path = new[] {"GameObjects/Images/Plane"},
                        ResourceType = ResourceType.Texture
                    })));
            group.Items.Add(new ToolboxItem("SlidePlane", imageList1.Images.IndexOfKey("WallLarge_Slide.png"),
                new ToolboxType(GameObjectType.SlidePlane,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.SlidePlane.ToString(),
                        Path = new[] {"GameObjects/Images/SlidePlane"},
                        ResourceType = ResourceType.Texture
                    })));
            group.Items.Add(new ToolboxItem("Saw", imageList1.Images.IndexOfKey("Saw.png"),
                new ToolboxType(GameObjectType.Saw,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.Saw.ToString(),
                        Path = new[] {"GameObjects/Images/Saw"},
                        ResourceType = ResourceType.Texture,
                        SubResources = new[] {pathResource}
                    })));
            group.Items.Add(new ToolboxItem("DeathBall", imageList1.Images.IndexOfKey("DeathBall.png"),
                new ToolboxType(GameObjectType.DeathBall,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.DeathBall.ToString(),
                        Path = new[] {"GameObjects/Images/DeathBall/OuterCircle"},
                        ResourceType = ResourceType.Texture,
                        SubResources = new[]
                        {
                            new ResourceIdentifier("CircleDetails", new[]
                            {
                                "GameObjects/Images/DeathBall/" + DeathBall.Details.CircleLayerBg.ToString(),
                                "GameObjects/Images/DeathBall/" + DeathBall.Details.InnerCircle.ToString(),
                                "GameObjects/Images/DeathBall/" + DeathBall.Details.State1.ToString(),
                                "GameObjects/Images/DeathBall/" + DeathBall.Details.State2.ToString(),
                                "GameObjects/Images/DeathBall/" + DeathBall.Details.State3.ToString(),
                                "GameObjects/Images/DeathBall/" + DeathBall.Details.Spikes.ToString(),
                            }, ResourceType.Texture)
                        }
                    })));
            /*group.Items.Add(new ToolboxItem("RedBall", imageList1.Images.IndexOfKey("RedBall.png"),
                                            new ToolboxType(GameObjectType.RedBall,
                                                            new ResourceIdentifier
                                                                {
                                                                    Name = GameObjectType.RedBall.ToString(),
                                                                    Path = new[] {"GameObjects/Sprites/Test"},
                                                                    ResourceType = ResourceType.Sprite
                                                                })));*/
            group.Items.Add(new ToolboxItem("Spike", imageList1.Images.IndexOfKey("Spike.png"),
                new ToolboxType(GameObjectType.Spike,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.Spike.ToString(),
                        Path = new[]
                        {
                            "GameObjects/Images/Spike"
                            /*"GameObjects/Images/Spike_1"
                                                                                   "GameObjects/Images/Spike_2"*/
                        },
                        ResourceType = ResourceType.Texture
                    })));
            group.Items.Add(new ToolboxItem("SpikeSmall", imageList1.Images.IndexOfKey("Spike.png"),
                new ToolboxType(GameObjectType.SpikeSmall,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.SpikeSmall.ToString(),
                        Path = new[]
                        {
                            "GameObjects/Images/SpikeSmall"
                        },
                        ResourceType = ResourceType.Texture
                    })));
            group.Items.Add(new ToolboxItem("DeathPlane", imageList1.Images.IndexOfKey("Spike.png"),
                new ToolboxType(GameObjectType.DeathPlane,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.DeathPlane.ToString(),
                        Path = new[]
                        {
                            "RedTexture"
                        },
                        ResourceType = ResourceType.Texture
                    })));
            group.Items.Add(new ToolboxItem("SpikeShooter", imageList1.Images.IndexOfKey("SpikeShooter.png"),
                new ToolboxType(GameObjectType.SpikeShooter,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.SpikeShooter.ToString(),
                        Path = new[]
                        {
                            "GameObjects/Images/SpikeShooter/SpikeShooterBase"
                            ,
                        },
                        ResourceType = ResourceType.Texture,
                        SubResources = new[]
                        {
                            new ResourceIdentifier(
                                "SpikeShooterParts",
                                new[]
                                {
                                    "GameObjects/Images/SpikeShooter/SpikeBullet",
                                    "GameObjects/Images/SpikeShooter/SpikeShooterBack"
                                },
                                ResourceType.Texture)
                        }
                    })));
            /*group.Items.Add(new ToolboxItem("TrapDoor", imageList1.Images.IndexOfKey("wall_ico.png"),
                                            new ToolboxType(GameObjectType.TrapDoor,
                                                            new ResourceIdentifier
                                                            {
                                                                Name = GameObjectType.TrapDoor.ToString(),
                                                                Path = new[]
                                                                               {
                                                                                   "GameObjects/Images/Bar",
                                                                               },
                                                                ResourceType = ResourceType.Texture
                                                            })));
            group.Items.Add(new ToolboxItem("Water", imageList1.Images.IndexOfKey("Water_ico.png"),
                                            new ToolboxType(GameObjectType.Water,
                                                            new ResourceIdentifier
                                                                {
                                                                    Name = GameObjectType.Water.ToString(),
                                                                    Path = new[] {"GameObjects/Images/Water"},
                                                                    ResourceType = ResourceType.Texture
                                                                })));*/
            group.Items.Add(new ToolboxItem("IncDot", imageList1.Images.IndexOfKey("InkDot.png"),
                new ToolboxType(GameObjectType.InkDot,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.InkDot.ToString(),
                        Path = new[] {"GameObjects/Images/InkDot"},
                        ResourceType = ResourceType.Texture

                    })));
            group.Items.Add(new ToolboxItem("JumpSpot", imageList1.Images.IndexOfKey("JumpSpot.png"),
                new ToolboxType(GameObjectType.JumpSpot,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.JumpSpot.ToString(),
                        Path = new[] {"GameObjects/Images/JumpSpot"},
                        ResourceType = ResourceType.Texture

                    })));
            /*group.Items.Add(new ToolboxItem("LevelEnd", imageList1.Images.IndexOfKey("LevelEnd.png"),
                                            new ToolboxType(GameObjectType.LevelEnd,
                                                            new ResourceIdentifier
                                                            {
                                                                Name = GameObjectType.LevelEnd.ToString(),
                                                                Path = new[] { "GameObjects/Images/LevelEndObject/layer_1" },
                                                                SubResources = new[]
                                                                                   {
                                                                                       new ResourceIdentifier("LevelEndDetails", new[] { 
                                                                                       "GameObjects/Images/LevelEndObject/layer_2",
                                                                                       "GameObjects/Images/LevelEndObject/layer_3"
                                                                                       }, ResourceType.Texture)
                                                                                   },
                                                                ResourceType = ResourceType.Texture
                                                            })));*/
            group.Items.Add(new ToolboxItem("LevelEnd", imageList1.Images.IndexOfKey("LevelEnd.png"),
                new ToolboxType(GameObjectType.LevelEnd,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.LevelEnd.ToString(),
                        Path = new[] {"GameObjects/Images/LevelComplete"},
                        ResourceType = ResourceType.Texture
                    })));
            group.Items.Add(new ToolboxItem("TrapBtn", imageList1.Images.IndexOfKey("TrapBtn.png"),
                new ToolboxType(GameObjectType.TrapBtn,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.TrapBtn.ToString(),
                        Path =
                            new[]
                            {"GameObjects/Images/TrapButton/TrapButton"},
                        ResourceType = ResourceType.Texture,
                        SubResources = new[]
                        {
                            new ResourceIdentifier("TrapBtnDetails",
                                new[]
                                {
                                    "GameObjects/Images/TrapButton/TrapButtonSwitch"
                                },
                                ResourceType.Texture)
                        }
                    })));
            group.Items.Add(new ToolboxItem("CameraPath", imageList1.Images.IndexOfKey("camera_ico.png"),
                new ToolboxType(GameObjectType.CameraPath,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.CameraPath.ToString(),
                        Path = new[] {"GameObjects/Images/CameraPath"},
                        ResourceType = ResourceType.Texture,
                    })));
            group.Items.Add(new ToolboxItem("CircleSpikes", imageList1.Images.IndexOfKey("Spike.png"),
                new ToolboxType(GameObjectType.CircleSpikes,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.CircleSpikes.ToString(),
                        Path = new[] {"GameObjects/Images/CircleSpikes"},
                        ResourceType = ResourceType.Texture
                    })));
            group.Items.Add(new ToolboxItem("Seeker", imageList1.Images.IndexOfKey("SeekerDot.png"),
                new ToolboxType(GameObjectType.SeekerDot,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.SeekerDot.ToString(),
                        Path = new[] {"GameObjects/Sprites/Seeker/Seeker"},
                        ResourceType = ResourceType.Sprite
                    })));
            var powerUpResourceIdentifier = new ResourceIdentifier()
            {
                Name = GameObjectType.PowerUp.ToString(),
                Path =
                    new[]
                    {
                        "GameObjects/Images/DoubleJumpPowerUp", "GameObjects/Images/InkCollectPowerUp",
                        "GameObjects/Images/TimeWarpPowerUp", "GameObjects/Images/ShieldPowerUp"
                    },
                ResourceType = ResourceType.Texture
            };
            group.Items.Add(new ToolboxItem("DoubleJumpPowerUp",
                imageList1.Images.IndexOfKey("DoubleJumpPowerUp_ico.png"),
                new ToolboxType(GameObjectType.PowerUp,
                    powerUpResourceIdentifier, (int) PowerUpType.DoubleJump)));
            group.Items.Add(new ToolboxItem("InkCollectPowerUp",
                imageList1.Images.IndexOfKey("InkCollectPowerUp_ico.png"),
                new ToolboxType(GameObjectType.PowerUp,
                    powerUpResourceIdentifier, (int) PowerUpType.InkCollect)));
            group.Items.Add(new ToolboxItem("TimeWarpPowerUp", imageList1.Images.IndexOfKey("TimeWarpPowerUp_ico.png"),
                new ToolboxType(GameObjectType.PowerUp,
                    powerUpResourceIdentifier, (int) PowerUpType.TimeWarp)));
            group.Items.Add(new ToolboxItem("ShieldPowerUp", imageList1.Images.IndexOfKey("ShieldPowerUp_ico.png"),
                new ToolboxType(GameObjectType.PowerUp,
                    powerUpResourceIdentifier, (int) PowerUpType.Shield)));
            group.Items.Add(new ToolboxItem("HintBox", imageList1.Images.IndexOfKey("Hint.png"),
                new ToolboxType(GameObjectType.Hint,
                     new ResourceIdentifier
                     {
                         Name = GameObjectType.Hint.ToString(),
                         Path = new[] { "blank", "BlackTexture" },
                         ResourceType = ResourceType.Texture
                     })));

            Log("Items(" + group.Items.Count + "): " + string.Join("|", group.Items.Select(item => item.Caption)));
            group.Expanded = true;
            toolbox1.Groups.Add(group.Caption, group);
            group = new ToolboxGroup("Walls");
            Log(group.Caption, false);
            group.Items.Add(new ToolboxItem("Wall-Slide-Small", imageList1.Images.IndexOfKey("WallSmall_Slide.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallSlideSmall.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallSmall_Slide"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallSlideSmall)));
            group.Items.Add(new ToolboxItem("Wall-Slide-Large", imageList1.Images.IndexOfKey("WallLarge_Slide.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallSlideLarge.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallLarge_Slide"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallSlideLarge)));
            group.Items.Add(new ToolboxItem("Wall-Hand", imageList1.Images.IndexOfKey("wall_ico.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.HandWall.ToString(),
                        Path = new[] {"GameObjects/Images/Wall"},
                        ResourceType = ResourceType.Texture
                    },
                    (int) DecorativeType.HandWall)));
            group.Items.Add(new ToolboxItem("Wall-Hand-Small", imageList1.Images.IndexOfKey("wall_ico.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.HandWallSmall.ToString(),
                        Path = new[] {"GameObjects/Images/Wall"},
                        ResourceType = ResourceType.Texture
                    },
                    (int) DecorativeType.HandWallSmall)));
            group.Items.Add(new ToolboxItem("Wall-Mini", imageList1.Images.IndexOfKey("WallMini.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallMini.ToString(),
                        Path =
                            new[] {"GameObjects/Images/Walls/WallMini_White", "GameObjects/Images/Walls/WallMini_Black"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallMini)));
            group.Items.Add(new ToolboxItem("Wall-Small", imageList1.Images.IndexOfKey("WallSmall.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallSmall.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallSmall"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallSmall)));
            group.Items.Add(new ToolboxItem("Wall-Medium", imageList1.Images.IndexOfKey("WallMed.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallMed.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallMed"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallMed)));
            group.Items.Add(new ToolboxItem("Wall-Large", imageList1.Images.IndexOfKey("WallLarge.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallLarge.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallLarge"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallLarge)));
            group.Items.Add(new ToolboxItem("Corner-Small", imageList1.Images.IndexOfKey("WallSmallCorner.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallSmallCorner.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallSmallCorner"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallSmallCorner)));
            var wallSmallDecResource = new ResourceIdentifier
            {
                Name = DecorativeType.WallSmallDec.ToString(),
                Path =
                    new[]
                    {"GameObjects/Images/Walls/WallSmallDec_Var1", "GameObjects/Images/Walls/WallSmallDec_Var2"},
                ResourceType = ResourceType.Texture
            };
            group.Items.Add(new ToolboxItem("Wall-Dec-Small", imageList1.Images.IndexOfKey("WallDec.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    wallSmallDecResource, (int) DecorativeType.WallSmallDec)));
            group.Items.Add(new ToolboxItem("Wall-Dec-Medium", imageList1.Images.IndexOfKey("WallDec.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallMedDec.ToString(),
                        Path = new[]
                        {
                            "GameObjects/Images/Walls/WallMedDec_Var1", "GameObjects/Images/Walls/WallMedDec_Var2",
                            "GameObjects/Images/Walls/WallMedDec_Var3", "GameObjects/Images/Walls/WallMedDec_Var4"
                        },
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallMedDec)));
            group.Items.Add(new ToolboxItem("Wall-Dec-Large", imageList1.Images.IndexOfKey("WallDec.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallLargeDec.ToString(),
                        Path = new[]
                        {
                            "GameObjects/Images/Walls/WallMaxDec_Var1", "GameObjects/Images/Walls/WallMaxDec_Var2",
                            "GameObjects/Images/Walls/WallMaxDec_Var3", "GameObjects/Images/Walls/WallMaxDec_Var4"
                        },
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallLargeDec)));
            group.Items.Add(new ToolboxItem("Wall-Dec-1", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallDec1.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallDec_1"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallDec1)));
            group.Items.Add(new ToolboxItem("Wall-Dec-2", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallDec2.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallDec_2"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallDec2)));
            group.Items.Add(new ToolboxItem("Wall-Dec-3", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallDec3.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallDec_3"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallDec3)));
            group.Items.Add(new ToolboxItem("Wall-Dec-4", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallDec4.ToString(),
                        Path = new[] {"GameObjects/Images/Walls/WallDec_4"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallDec4)));
            group.Items.Add(new ToolboxItem("Dec-Cricle-Top", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallDecCircleTop.ToString(),
                        Path =
                            new[]
                            {
                                "GameObjects/Images/Walls/Top/WallDecCircleTop_Var1",
                                "GameObjects/Images/Walls/Top/WallDecCircleTop_Var2"
                            },
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallDecCircleTop)));
            group.Items.Add(new ToolboxItem("Dec-Object-Top", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallDecObjectTop.ToString(),
                        Path =
                            new[]
                            {
                                "GameObjects/Images/Walls/Top/WallDecObjectTop_Var1",
                                "GameObjects/Images/Walls/Top/WallDecObjectTop_Var2"
                            },
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallDecObjectTop)));
            group.Items.Add(new ToolboxItem("Dec-Circle-Bot", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallDecCircleBot.ToString(),
                        Path =
                            new[]
                            {
                                "GameObjects/Images/Walls/Bottom/WallDecCircleBot_Var1",
                                "GameObjects/Images/Walls/Bottom/WallDecCircleBot_Var2"
                            },
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallDecCircleBot)));
            group.Items.Add(new ToolboxItem("Dec-Object-Bot", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.WallDecObjectBot.ToString(),
                        Path =
                            new[]
                            {
                                "GameObjects/Images/Walls/Bottom/WallDecObjectBot_Var1",
                                "GameObjects/Images/Walls/Bottom/WallDecObjectBot_Var2"
                            },
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.WallDecObjectBot)));
            group.Items.Add(new ToolboxItem("ContactInfo", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.ContactInfo.ToString(),
                        Path = new[] {"Gui/Menu/ContactInfo"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.ContactInfo)));
            group.Items.Add(new ToolboxItem("GameName", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.DecorativeObject,
                    new ResourceIdentifier
                    {
                        Name = DecorativeType.GameName.ToString(),
                        Path = new[] {"Gui/Menu/GameName"},
                        ResourceType = ResourceType.Texture
                    }, (int) DecorativeType.GameName)));
            group.Items.Add(new ToolboxItem("Dec-Small-Resizable", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.ResizableDecorativeObject,
                    wallSmallDecResource, (int)DecorativeType.WallSmallDec)));
            toolbox1.Groups.Add(group.Caption, group);
            // Background Objects
            group = new ToolboxGroup("Background objects");
            Log(group.Caption, false);
            group.Items.Add(new ToolboxItem("BackgroundGear", imageList1.Images.IndexOfKey("gear.png"),
                new ToolboxType(GameObjectType.BackgroundGear,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.BackgroundGear.ToString(),
                        Path = new[]
                        {
                            "GameObjects/Images/BG_Gear1", "GameObjects/Images/BG_Gear2", "GameObjects/Images/BG_Gear3",
                            "GameObjects/Images/BG_Gear4", "GameObjects/Images/BG_Gear5", "GameObjects/Images/BG_Gear6",
                            "GameObjects/Images/BG_Gear7", "GameObjects/Images/BG_Gear8"
                        },
                        ResourceType = ResourceType.Texture
                    }), MapType.Background) {Enabled = false});
            group.Items.Add(new ToolboxItem("BackgroundGearSmall", imageList1.Images.IndexOfKey("gearsmall.png"),
                new ToolboxType(GameObjectType.BackgroundGearSmall,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.BackgroundGearSmall.ToString(),
                        Path =
                            new[]
                            {
                                "GameObjects/Images/BG_Gear_Small1", "GameObjects/Images/BG_Gear_Small2",
                                "GameObjects/Images/BG_Gear_Small3"
                            },
                        ResourceType = ResourceType.Texture
                    }), MapType.Background) {Enabled = false});
            /*group.Items.Add(new ToolboxItem("BackgroundGearSmall", imageList1.Images.IndexOfKey("gearsmall.png"),
                                            new ToolboxType(GameObjectType.BackgroundGearSmall,
                                                            new ResourceIdentifier
                                                            {
                                                                Name = GameObjectType.BackgroundGearSmall.ToString(),
                                                                Path = new[] { "GameObjects/Images/BG_Gear_Small1", "GameObjects/Images/BG_Gear_Small2", "GameObjects/Images/BG_Gear_Small3" },
                                                                ResourceType = ResourceType.Texture
                                                            }), MapType.Background) { Enabled = false });*/
            Log("Items(" + group.Items.Count + "): " + string.Join("|", group.Items.Select(item => item.Caption)));
            group.Expanded = false;
            toolbox1.Groups.Add(group.Caption, group);
            // Menu Objects
            group = new ToolboxGroup("Menu objects");
            Log(group.Caption, false);
            group.Items.Add(new ToolboxItem("Menu button", imageList1.Images.IndexOfKey("gear.png"),
                new ToolboxType(GameObjectType.MenuObject,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.MenuObject.ToString(),
                        Path = new[] {"GUI/Menu/MenuButton"},
                        ResourceType = ResourceType.Texture
                    }), MapType.Menu) {Enabled = false});
            group.Items.Add(new ToolboxItem("Menu line", imageList1.Images.IndexOfKey("Plane.png"),
                new ToolboxType(GameObjectType.MenuLine,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.MenuLine.ToString(),
                        Path = new[] {"GUI/Menu/MenuLine"},
                        ResourceType = ResourceType.Texture
                    }), MapType.Menu) {Enabled = false});
            group.Items.Add(new ToolboxItem("Menu decoration", imageList1.Images.IndexOfKey("WallDecObject.png"),
                new ToolboxType(GameObjectType.MenuDec,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.MenuDec.ToString(),
                        Path =
                            new[] {"GUI/Menu/MenuDec1", "GUI/Menu/MenuDec2", "GUI/Menu/MenuDec3", "GUI/Menu/MenuDec4"},
                        ResourceType = ResourceType.Texture
                    }), MapType.Menu) {Enabled = false});
            group.Items.Add(new ToolboxItem("Menu gear", imageList1.Images.IndexOfKey("gear.png"),
                new ToolboxType(GameObjectType.MenuGear,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.MenuGear.ToString(),
                        Path =
                            new[]
                            {"GUI/Menu/Menu_gear1", "GUI/Menu/Menu_gear2", "GUI/Menu/Menu_gear3", "GUI/Menu/Menu_gear4"},
                        ResourceType = ResourceType.Texture
                    }), MapType.Menu) {Enabled = false});
            group.Items.Add(new ToolboxItem("Menu pointer button", imageList1.Images.IndexOfKey("WallDec.png"),
                new ToolboxType(GameObjectType.MenuBtnPointer,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.MenuBtnPointer.ToString(),
                        Path = new[] {"GameObjects/Sprites/MenuBtnPointer/MenuBtnPointer"},
                        ResourceType = ResourceType.Sprite
                    }), MapType.Menu) {Enabled = false});
            group.Items.Add(new ToolboxItem("Menu map button", imageList1.Images.IndexOfKey("gear.png"),
                new ToolboxType(GameObjectType.MenuBtnMap,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.MenuBtnMap.ToString(),
                        Path = new[] {"GUI/Menu/BtnMap"},
                        ResourceType = ResourceType.Texture
                    }), MapType.Menu) {Enabled = false});
            group.Items.Add(new ToolboxItem("Version", imageList1.Images.IndexOfKey("gear.png"),
                new ToolboxType(GameObjectType.VersionInfo,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.VersionInfo.ToString(),
                        Path = new[] {"GUI/Menu/Version"},
                        ResourceType = ResourceType.Texture
                    }), MapType.Menu) {Enabled = false});
            group.Items.Add(new ToolboxItem("Switch button", imageList1.Images.IndexOfKey("gear.png"),
                new ToolboxType(GameObjectType.SwitchBtn,
                    new ResourceIdentifier
                    {
                        Name = GameObjectType.SwitchBtn.ToString(),
                        Path = new[] {"GUI/Menu/BtnSwitch"},
                        ResourceType = ResourceType.Texture
                    }), MapType.Menu) {Enabled = false});
            /*group.Items.Add(new ToolboxItem("Score display device", imageList1.Images.IndexOfKey("gear.png"),
                                            new ToolboxType(GameObjectType.ScoreDisplayDevice,
                                                            new ResourceIdentifier
                                                            {
                                                                Name = GameObjectType.ScoreDisplayDevice.ToString(),
                                                                Path = new[] { "GUI/Menu/ScoreScreen/Hand" },
                                                                ResourceType = ResourceType.Texture,
                                                                SubResources = new[]
                                                                {
                                                                    new ResourceIdentifier
                                                                    {
                                                                        Name = "ScoreDisplayDeviceDetails",
                                                                        Path = new[] { "GUI/Menu/ScoreScreen/Cilinder" },
                                                                        ResourceType = ResourceType.Texture
                                                                    } 
                                                                }
                                                            }), MapType.Menu) { Enabled = false });*/
            Log("Items(" + group.Items.Count + "): " + string.Join("|", group.Items.Select(item => item.Caption)));
            group.Expanded = false;
            toolbox1.Groups.Add(group.Caption, group);
        }

        protected void CreateResources()
        {
            Log("==CreateResources==");
            EngineGlobals.Resources = new ResourcesManager();
            EditorResources = new MapResources();
            //xnaWindow1.Resources = new ResourceCollection();
            //xnaWindow1.Textures = new Dictionary<string, GameTexture>();
            //xnaWindow1.Sprites = new Dictionary<string, SpriteData>();
            foreach (var toolboxGroup in toolbox1.Groups.Values)
            {

                Log("In group: " + toolboxGroup.Caption);
                foreach (var toolboxItem in toolboxGroup.Items)
                {
                    Log("\tResources.Add(" + toolboxItem.TypeInfo.ResourceIdentifier + ")", false);
                    EditorResources.Add(toolboxItem.TypeInfo.ResourceIdentifier);
                    EngineGlobals.Resources.LoadResource(toolboxItem.TypeInfo.ResourceIdentifier);
                    //xnaWindow1.Resources.Add(toolboxItem.TypeInfo.ResourceIdentifier);
                    /*if (toolboxItem.TypeInfo.ResourceIdentifier.ResourceType == ResourceType.Texture)
                    {
                        Log("\txnaWindow1.Textures.Add(" + toolboxItem.TypeInfo.ObjectTypeName + ", " + toolboxItem.TypeInfo.ResourceIdentifier.Path + ")", false);
                        xnaWindow1.Textures.Add(toolboxItem.TypeInfo.ObjectTypeName.ToString(),
                                                new GameTexture(toolboxItem.TypeInfo.ResourceIdentifier.Path));
                    }
                    else if (toolboxItem.TypeInfo.ResourceIdentifier.ResourceType == ResourceType.Sprite)
                    {
                        Log("\txnaWindow1.Sprites.Add(" + toolboxItem.TypeInfo.ObjectTypeName + ", " + toolboxItem.TypeInfo.ResourceIdentifier.Path + ")", false);
                        xnaWindow1.Sprites.Add(toolboxItem.TypeInfo.ObjectTypeName.ToString(),
                                               EngineGlobals.Content.Load<SpriteData>(
                                                   toolboxItem.TypeInfo.ResourceIdentifier.Path));
                    }
                    else
                        throw new Exception("Unknow resource type: " + toolboxItem.TypeInfo.ResourceIdentifier.ResourceType);*/
                }
            }
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Splash", new[] { @"Particles\Splash" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("SplashWall", new[]
                {
                    @"Particles\SplashWall_1", @"Particles\SplashWall_2",
                    @"Particles\SplashWall_3", @"Particles\SplashWall_4",
                    @"Particles\SplashWall_5", @"Particles\SplashWall_6"
                },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Splash_Death", new[] { @"Particles\Splash_Death" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("SawParticle",
                                                                       new[] { @"Particles\SawParticle_1", @"Particles\SawParticle_2", @"Particles\SawParticle_3" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Particle",
                                                                       new[] { @"Particles\Particle" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("HealthBar",
                                                                       new[] { @"GameObjects\Images\Bar" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("HealthBarBorder",
                                                                       new[] { @"GameObjects\Images\Border" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("Hit", new[] { @"Particles\Hit" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("ScoreBack", new[] { @"Gui\ScoreBack" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadTexture(new ResourceIdentifier("SeekerParticle",
                                                                       new[] { @"Particles\SeakerParticle_1", @"Particles\SeakerParticle_2" },
                                                                       ResourceType.Texture));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("stick", new[] { @"Audio\Stick" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("death", new[] { @"Audio\Death" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("shot", new[] { @"Audio\Shot" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("hit", new[] { @"Audio\Hit" }, ResourceType.Sound));;
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("click", new[] { @"Audio\click2" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("explode", new[] { @"Audio\explode" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("backtonormal", new[] { @"Audio\backtonormal" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("collectink", new[] { @"Audio\collectink" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("shatter", new[] { @"Audio\shatter" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("slowdown", new[] { @"Audio\slowdown" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("trapBtn", new[] { @"Audio\trapBtn" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("ticking", new[] { @"Audio\ticking" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("switchOff", new[] { @"Audio\switchOff" }, ResourceType.Sound));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("MenuFont", new[] { "MenuFont" }, ResourceType.Font));
            EngineGlobals.Resources.LoadResource(new ResourceIdentifier("blank", new[] { "blank" }, ResourceType.Texture));
            EngineGlobals.Resources.LoadFont(new ResourceIdentifier("GameFont", new[] { "GameFont" }, ResourceType.Font));
            GameGlobals.MenuGlobals.MenuFont = EngineGlobals.Resources.Fonts["MenuFont"];
            GameGlobals.Font = EngineGlobals.Resources.Fonts["GameFont"];

            EngineGlobals.Resources.LoadTexture(
                new ResourceIdentifier(
                    "marker", new[]
                        {
                            @"Gui\marker"
                        },
                    ResourceType.Texture));

        }

        protected void NewMap(int width, int height, MapType mapType)
        {
            Log("==NewMap==");
            _temporaryFileName = string.Empty;
            _clipboardObject = null;
            _clipboardObjectTypeName = null;
            _multiObjectsCopy = false;
            _fileName = "";
            _saved = false;
            BackgroundMode = mapType == MapType.Background;
            _menuMode = mapType == MapType.Menu;
            menuStripimportBackgroundButton.Enabled = !BackgroundMode;
            toolStripImportBackgroundButton.Enabled = !BackgroundMode;
            Controller.ClearRelativeObjects();
            Controller.ClearBackgroundobjects();
            Controller.ClearGameObjects();
            if (!BackgroundMode)
            {
                Controller.AddObject(xnaWindow1._debugCircle);
                Controller.AddObject(xnaWindow1.MultiSelectionRegion);
                EngineGlobals.Camera2D.BackgroundOffset = new Vector2(350f, 210f);
            }
            else
            {
                EngineGlobals.Camera2D.BackgroundOffset = new Vector2(350f, 210f);
            }
            
            treeView1.Nodes.Clear();
            if (BackgroundMode)
            {
                Log("Background type");
                foreach (var group in toolbox1.Groups)
                {
                    foreach (var toolboxItem in group.Value.Items)
                    {
                        toolboxItem.Enabled = toolboxItem.ObjectMapType == MapType.Background;
                    }
                }
                toolbox1.Groups["All game objects"].Expanded = false;
                toolbox1.Groups["Background objects"].Expanded = true;
                toolbox1.Invalidate();
            }
            else
            {
                foreach (var group in toolbox1.Groups)
                {
                    foreach (var toolboxItem in group.Value.Items)
                    {
                        toolboxItem.Enabled = toolboxItem.ObjectMapType == MapType.Game || (_menuMode && toolboxItem.ObjectMapType == MapType.Menu) ;
                    }
                }
                toolbox1.Groups["All game objects"].Expanded = true;
                toolbox1.Groups["Background objects"].Expanded = false;
                toolbox1.Invalidate();
            }

            treeView1.Nodes.Add("Map objects");
            
            
            xnaWindow1.GameLevel = new Map
                                       {
                                           Width = width*10,
                                           Height = height*10
                                       };
            if (EngineGlobals.Background != null)
                EngineGlobals.Background.Dispose();
            EngineGlobals.Background = new BackgroundManager(xnaWindow1.GameLevel.Width, xnaWindow1.GameLevel.Height);
            if (BackgroundMode)
                xnaWindow1.GameLevel.MapType = MapType.Background;
            else if (!_menuMode)
                xnaWindow1.GameLevel.MapType = MapType.Game;
            else
                xnaWindow1.GameLevel.MapType = MapType.Menu;
            GameGlobals.Map = xnaWindow1.GameLevel;
            GameGlobals.Map.BackgroundChanged += MapOnBackgroundChanged;
            //GameGlobals.Map.Resources.Textures = xnaWindow1.Textures;
            //GameGlobals.Map.Resources.Sprites = xnaWindow1.Sprites;
            //foreach (var resource in xnaWindow1.Resources)
            //{
            //    if (resource.ResourceType == ResourceType.Texture)
            //        GameGlobals.Map.Resources.TextureIdentifiers.Add(resource);
            //}
            //Log("Setting textures: " + GameGlobals.Map.Resources.Textures.Count);
            //foreach (var resource in xnaWindow1.Resources)
            //{
            //    if (resource.ResourceType == ResourceType.Sprite)
            //        GameGlobals.Map.Resources.SpriteIdentifiers.Add(resource);
            //}
            //Log("Setting sprites: " + GameGlobals.Map.Resources.Sprites.Count);
            Log("Creating Grid: (new Rectangle(0, 0, " + width +", " + height + "), 10, 10, true)");
            EngineGlobals.Grid = new Grid(new Rectangle(0, 0, width, height), 10, 10, true);
            Controller.AddObject(EngineGlobals.Grid);
            EngineGlobals.Grid.Background = BackgroundMode;
            EngineGlobals.Grid.IsHidden = !menuStripShowGridButton.Checked;
            Log("Reseting ComboBox and Tree");
            Log("Unregistering event SelectedIndexChanged from ComboBox");
            comboBox1.SelectedIndexChanged -= ComboBox1SelectedIndexChanged1;
            comboBox1.Items.Clear();
            comboBox1.Items.Add(GameGlobals.Map);
            treeView1.Nodes.Clear();
            if (!BackgroundMode)
                treeView1.Nodes.Add(TreeNodes.GameObjects.ToString(), "Map objects");
            else
            {
                RegionEditingMode = false;
                toolStripNewRegionButton.Checked = false;
                toolStripRegionEditorButton.Checked = false;
                toolStripRegionEditorButton.Enabled = false;
            }
            treeView1.Nodes.Add(TreeNodes.BackgroundObjects.ToString(), "Background objects");
            treeView1.Nodes.Add(TreeNodes.Regions.ToString(), "Regions");
            treeView1.Nodes.Add(TreeNodes.Resources.ToString(), "Resources");
            treeView1.Nodes[TreeNodes.Resources.ToString()].Nodes.Add("Textures");
            treeView1.Nodes[TreeNodes.Resources.ToString()].Nodes.Add("Sprites");
            Log("Registering event SelectedIndexChanged from ComboBox");
            comboBox1.SelectedIndexChanged += ComboBox1SelectedIndexChanged1;
            comboBox1.SelectedIndex = 0;
            Log("Reset data");
            ResetData();
        }

        private void MapOnBackgroundChanged(object sender)
        {
            if (string.IsNullOrEmpty(GameGlobals.Map.Background) || !File.Exists(Path.Combine(CurrentPath, "GameContent", GameGlobals.Map.Background + ".xnb")))
            {
                var path = Path.Combine(CurrentPath, "GameContent", GameGlobals.Map.Background + ".xnb");
                if (!string.IsNullOrEmpty(GameGlobals.Map.Background))
                    MessageBox.Show("Background file not found: '" + path + "'");
                EngineGlobals.Background.UnloadBackgroundTexture();
                return;
            }
            try
            {
               // var texture = Texture2D.FromStream(EngineGlobals.Device, File.Open(GameGlobals.Map.Background, FileMode.Open));
                var texture = EngineGlobals.ContentCache.Load<Texture2D>(GameGlobals.Map.Background);
                EngineGlobals.Background.LoadBackground(new GameTexture(texture));
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "Error loading background file: '" + GameGlobals.Map.Background + "': " + exception.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                GameGlobals.Map.Background = "";
            }
            
        }

        protected void OpenMap(string path, Stream stream = null, string tmpPath = "")
        {
            Log("==OpenMap==");
            if (stream == null)
            {
                Log("Load map from: '" + path + "'");
                stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                _temporaryFileName = string.Empty;

            }
            else if (stream != null)
            {
                Log("Load map from: '" + tmpPath + "'; Using file name: '" + path + "'");
                _temporaryFileName = Path.GetFileName(tmpPath);
                _fileName = path;

                Text = "Recovered map - Game Editor";
            }
            Text = (string.IsNullOrEmpty(path) ? "Recovered unsaved map" : Path.GetFileName(path)) + " - Game Editor";
            
            Controller.ClearRelativeObjects();
            Controller.ClearGameObjects();
            Controller.ClearBackgroundobjects();
            _clipboardObject = null;
            _clipboardObjectTypeName = null;
            _multiObjectsCopy = false;
            EngineGlobals.Background.UnloadBackgroundTexture();
            GameGlobals.Map = Map.LoadEditor(stream);
            stream.Close();
            xnaWindow1.GameLevel = GameGlobals.Map;
            xnaWindow1.GameLevel.BackgroundChanged += MapOnBackgroundChanged;
            BackgroundMode = xnaWindow1.GameLevel.MapType == MapType.Background;
            _menuMode = xnaWindow1.GameLevel.MapType == MapType.Menu;
            menuStripimportBackgroundButton.Enabled = !BackgroundMode;
            toolStripImportBackgroundButton.Enabled = !BackgroundMode;
            if (!BackgroundMode)
            {
                EngineGlobals.Camera2D.BackgroundOffset = new Vector2(350f, 210f);
            }
            else
            {
                EngineGlobals.Camera2D.BackgroundOffset = new Vector2(350f, 210f);
            }
            Log("Selecting current level(" + xnaWindow1.GameLevel.Width + "x" + xnaWindow1.GameLevel.Height + ")");
            propertyGrid1.SelectedObject = GameGlobals.Map;
            //Log("Setting textures: " + GameGlobals.Map.Resources.Textures.Count);
            //foreach (var resource in xnaWindow1.Resources)
            //{
            //    if (resource.ResourceType == ResourceType.Texture)
            //        GameGlobals.Map.Resources.TextureIdentifiers.Add(resource);
            //}
            //Log("Setting sprites: " + GameGlobals.Map.Resources.Sprites.Count);
            //foreach (var resource in xnaWindow1.Resources)
            //{
            //    if (resource.ResourceType == ResourceType.Sprite)
            //        GameGlobals.Map.Resources.SpriteIdentifiers.Add(resource);
            //}
            if (BackgroundMode)
            {
                Log("Background type");
                foreach (var group in toolbox1.Groups)
                {
                    foreach (var toolboxItem in group.Value.Items)
                    {
                        toolboxItem.Enabled = toolboxItem.ObjectMapType == MapType.Background;
                    }
                }

                toolbox1.Groups["All game objects"].Expanded = false;
                toolbox1.Groups["Background objects"].Expanded = true;
                toolbox1.Invalidate();
            }
            else
            {
                foreach (var group in toolbox1.Groups)
                {
                    foreach (var toolboxItem in group.Value.Items)
                    {
                        toolboxItem.Enabled = toolboxItem.ObjectMapType == MapType.Game || (_menuMode && toolboxItem.ObjectMapType == MapType.Menu);
                    }
                }
                toolbox1.Groups["All game objects"].Expanded = true;
                toolbox1.Groups["Background objects"].Expanded = false;
                toolbox1.Invalidate();
            }
            Log("Creating Grid: new Rectangle(0, 0, " + xnaWindow1.GameLevel.Width/10 + ", " + xnaWindow1.GameLevel.Height/10 + "), 10, 10, true");
            EngineGlobals.Grid = new Grid(new Rectangle(0, 0, xnaWindow1.GameLevel.Width/10, xnaWindow1.GameLevel.Height/10), 10, 10, true);
            Controller.AddObject(EngineGlobals.Grid);
            EngineGlobals.Grid.Background = BackgroundMode;
            EngineGlobals.Grid.IsHidden = !menuStripShowGridButton.Checked;
            Log("Reseting ComboBox and Tree");
            Log("Unregistering event SelectedIndexChanged from ComboBox");
            comboBox1.SelectedIndexChanged -= ComboBox1SelectedIndexChanged1;
            comboBox1.Items.Clear();
            comboBox1.Items.Add(GameGlobals.Map);
            treeView1.Nodes.Clear();
            if (!BackgroundMode)
                treeView1.Nodes.Add(TreeNodes.GameObjects.ToString(), "Map objects");
            treeView1.Nodes.Add(TreeNodes.BackgroundObjects.ToString(), "Background Objects");
            treeView1.Nodes.Add(TreeNodes.Regions.ToString(), "Regions");
            treeView1.Nodes.Add(TreeNodes.Resources.ToString(), "Resources");
            treeView1.Nodes[TreeNodes.Resources.ToString()].Nodes.Add("Textures");
            treeView1.Nodes[TreeNodes.Resources.ToString()].Nodes.Add("Sprites");
            if (!BackgroundMode)
            {
                foreach (var objectData in GameGlobals.Map.GameObjects)
                {
                    comboBox1.Items.Add(objectData);
                }
                foreach (var region in GameGlobals.Map.Regions)
                {
                    comboBox1.Items.Add(region);
                }
            }
            else
            {
                foreach (var objectData in GameGlobals.Map.BackgroundObjects)
                {
                    comboBox1.Items.Add(objectData);
                }
                RegionEditingMode = false;
                toolStripNewRegionButton.Checked = false;
                toolStripRegionEditorButton.Checked = false;
                toolStripRegionEditorButton.Enabled = false;
            }
            if (!BackgroundMode)
            {
                AddObjectsCollectionToTree(GameGlobals.Map.GameObjects.ToArray(), TreeNodes.GameObjects);
                AddObjectsCollectionToTree(GameGlobals.Map.Regions.ToArray(), TreeNodes.Regions);
            }
            AddObjectsCollectionToTree(GameGlobals.Map.BackgroundObjects.ToArray(), TreeNodes.BackgroundObjects);
            Log("Registering event SelectedIndexChanged from ComboBox");
            comboBox1.SelectedIndexChanged += ComboBox1SelectedIndexChanged1;
            comboBox1.SelectedIndex = 0;
            ResetData();
        }

        protected void AddObject(PhysicalObject newObject)
        {
            try
            {
                Log("==AddObject==");
                var type = newObject.TypeId;
                var resource = newObject.ResourceId;
                Log("Object: type = '" + type + "' resource = '" + resource + "'");
               
                //newObject.Rectangle = new RectangleF(EngineGlobals.Grid.ToRealRectangle(xnaWindow1.TempObjectRect));
                newObject.Pos = EngineGlobals.Grid.ToPosition(new Point(xnaWindow1.TempObjectRect.X, xnaWindow1.TempObjectRect.Y)).ToPoint();
                Log("Object position: " + newObject.Rectangle.GetRectangle());
                Log("Setting name");
                while (true)
                {
                    if (_nameCounter.ContainsKey(type))
                        _nameCounter[type]++;
                    else
                        _nameCounter.Add(type, 1);
                    var found = GameGlobals.Map.GameObjects.Any(objectData => objectData.Name == type.ToString() + _nameCounter[type.ToString()]);
                    if (!found)
                        break;
                }
                newObject.Name = type + _nameCounter[type];
                Log("Object name = '" + newObject.Name + "'");
                Log("Creating target");
                Log("Addeding object to Level");
                GameGlobals.Map.AddObject(newObject);
                AddObjectToTree(newObject);
                Log("Addeding object to ComboBox");
                comboBox1.Items.Add(newObject);
                SelectPassive(newObject);
                var a = new Action(Action.ActionType.CreateObject) {Target = newObject};
                AddActionToList(a);
            }
            catch (Exception ex)
            {
                throw new Exception("Unhandled exception: " + ex.Message);
            }
        }

        public void AddObjectToTree(IEditorObject data)
        {
            var name = data.EditorGetName();
            Log("Addeding object(" + name + ") to Tree");
            var nodeType = TreeNodes.GameObjects;
            
            string typeId = null;
            if (data is PhysicalObject)
            {
                typeId = ((PhysicalObject) data).TypeId;
                if (data is BackgroundObject)
                    nodeType = TreeNodes.BackgroundObjects;

                if (treeView1.Nodes[nodeType.ToString()].Nodes.ContainsKey(typeId))
                {
                    treeView1.Nodes[nodeType.ToString()].Nodes[typeId].Nodes.Add(name, name);
                    treeView1.Nodes[nodeType.ToString()].Nodes[typeId].Text = typeId + "(" +
                                                                              treeView1.Nodes[nodeType.ToString()].Nodes
                                                                                  [
                                                                                      typeId].Nodes.Count +
                                                                              ")";
                }
                else
                {
                    treeView1.Nodes[nodeType.ToString()].Nodes.Add(typeId, typeId + "(1)");
                    treeView1.Nodes[nodeType.ToString()].Nodes[typeId].Nodes.Add(name, name);
                }
            }
            else if (data is Region)
            {
                nodeType = TreeNodes.Regions;
                treeView1.Nodes[nodeType.ToString()].Nodes.Add(name, name);
                treeView1.Nodes[nodeType.ToString()].Text = "Regions(" +
                                                            treeView1.Nodes[nodeType.ToString()].Nodes.Count + ")";
            }
            else
            {
                throw new Exception("Unsuported node type: '" + data.GetType() + "' in tree");
            }

            RefreshTreeRoot(nodeType);
        }

        public void AddObjectsCollectionToTree(IEnumerable<IEditorObject> data, TreeNodes nodeType)
        {
            Log("Addeding objects collection to tree");
            treeView1.BeginUpdate();
            if (nodeType == TreeNodes.GameObjects || nodeType == TreeNodes.BackgroundObjects)
            {
                foreach (var physicalObject in data.Cast<PhysicalObject>())
                {
                    if (treeView1.Nodes[nodeType.ToString()].Nodes.ContainsKey(physicalObject.TypeId))
                    {
                        treeView1.Nodes[nodeType.ToString()].Nodes[physicalObject.TypeId].Nodes.Add(
                            physicalObject.Name, physicalObject.Name);
                        treeView1.Nodes[nodeType.ToString()].Nodes[physicalObject.TypeId].Text = physicalObject.TypeId +
                                                                                                 "(" +
                                                                                                 treeView1.Nodes[
                                                                                                     nodeType.ToString()
                                                                                                     ].Nodes[
                                                                                                         physicalObject.
                                                                                                             TypeId].
                                                                                                     Nodes.Count +
                                                                                                 ")";
                    }
                    else
                    {
                        treeView1.Nodes[nodeType.ToString()].Nodes.Add(physicalObject.TypeId,
                                                                       physicalObject.TypeId + "(1)");
                        treeView1.Nodes[nodeType.ToString()].Nodes[physicalObject.TypeId].Nodes.Add(
                            physicalObject.Name, physicalObject.Name);
                    }
                }
            }
            else if (nodeType == TreeNodes.Regions)
            {
                foreach (var editorObject in data)
                {
                    treeView1.Nodes[nodeType.ToString()].Nodes.Add(editorObject.EditorGetName(),
                                                                   editorObject.EditorGetName());
                }
                treeView1.Nodes[nodeType.ToString()].Text = treeView1.Nodes[nodeType.ToString()].Nodes.Count > 0
                                                                ? nodeType.ToString() + "(" +
                                                                  treeView1.Nodes[nodeType.ToString()].Nodes.Count + ")"
                                                                : nodeType.ToString();
            }
            treeView1.EndUpdate();
            RefreshTreeRoot(nodeType);
        }

        public void DeleteObjectFromTree(IEditorObject data, TreeNodes treeNodes)
        {
            Log("Removing object(" + data.EditorGetName() + ") from Tree");
            if (data is PhysicalObject)
            {
                var typeId = ((PhysicalObject) data).TypeId;
                if (treeView1.Nodes[treeNodes.ToString()].Nodes[typeId].Nodes.Count < 2)
                {
                    treeView1.Nodes[treeNodes.ToString()].Nodes[typeId].Nodes.Clear();
                    treeView1.Nodes[treeNodes.ToString()].Nodes.RemoveByKey(typeId);
                    RefreshTreeRoot(treeNodes);
                    return;
                }
                treeView1.Nodes[treeNodes.ToString()].Nodes[typeId].Nodes.RemoveByKey(data.EditorGetName());
                treeView1.Nodes[treeNodes.ToString()].Nodes[typeId].Text = typeId + "(" +
                                                                                treeView1.Nodes[treeNodes.ToString()].
                                                                                    Nodes[typeId].
                                                                                    Nodes.Count + ")";
            }
            else if (data is Region)
            {
                treeView1.Nodes[treeNodes.ToString()].Nodes.RemoveByKey(data.EditorGetName());
                if (treeView1.Nodes[treeNodes.ToString()].Nodes.Count < 1)
                {
                    treeView1.Nodes[treeNodes.ToString()].Text = treeNodes.ToString();
                }
                else
                {
                    treeView1.Nodes[treeNodes.ToString()].Text = treeNodes + "(" +
                                                                                treeView1.Nodes[treeNodes.ToString()].
                                                                                    Nodes.Count + ")";
                }
                return;
            }
            else
            {
                throw new Exception("Undefined data type '" + data.GetType() + "'");
            }
            RefreshTreeRoot(treeNodes);
        }

        public void RefreshTreeRoot(TreeNodes treeNodes)
        {
            if (treeNodes == TreeNodes.GameObjects)
            {
                var count = treeView1.Nodes[treeNodes.ToString()].Nodes.Cast<TreeNode>().Sum(node => node.Nodes.Count);
                treeView1.Nodes[treeNodes.ToString()].Text = count > 0 ? "Map objects(" + count + ")" : "Map objects";
            }
            else if (treeNodes == TreeNodes.BackgroundObjects)
            {
                var count = treeView1.Nodes[treeNodes.ToString()].Nodes.Cast<TreeNode>().Sum(node => node.Nodes.Count);
                treeView1.Nodes[treeNodes.ToString()].Text = count > 0 ? "Background objects(" + count + ")" : "Background objects";
            }
            else if (treeNodes == TreeNodes.Resources)
            {
                treeView1.BeginUpdate();
                treeView1.Nodes[treeNodes.ToString()].Nodes[0].Nodes.Clear();
                foreach (var textureIdentifier in GameGlobals.Map.Resources.TextureIdentifiers)
                {
                    treeView1.Nodes[treeNodes.ToString()].Nodes[0].Nodes.Add(textureIdentifier.Name, textureIdentifier.Name);
                }
                treeView1.Nodes[treeNodes.ToString()].Nodes[1].Nodes.Clear();
                foreach (var spriteIdentifier in GameGlobals.Map.Resources.SpriteIdentifiers)
                {
                    treeView1.Nodes[treeNodes.ToString()].Nodes[1].Nodes.Add(spriteIdentifier.Name, spriteIdentifier.Name);
                }
                var texturesNum = treeView1.Nodes[treeNodes.ToString()].Nodes[0].Nodes.Count;
                treeView1.Nodes[treeNodes.ToString()].Nodes[0].Text = texturesNum > 0
                                                               ? "Textures(" + texturesNum + ")" : "Textures";
                var spritesNum = treeView1.Nodes[treeNodes.ToString()].Nodes[1].Nodes.Count;
                treeView1.Nodes[treeNodes.ToString()].Nodes[1].Text = spritesNum > 0
                                                               ? "Sprites(" + spritesNum + ")" : "Sprites";
                treeView1.Nodes[treeNodes.ToString()].Text = (texturesNum + spritesNum) > 0
                                                      ? "Resources(" + (texturesNum + spritesNum) + ")" : "Resources";
                treeView1.EndUpdate();
            }
        }

        public void RefreshObjectName(PhysicalObject target, string prevName)
        {
            Log("Setting object name from '" + prevName + "' to '" + target.Name + "'");
            var root = treeView1.Nodes[0].Nodes[target.TypeId];
            root.Nodes.Remove(root.Nodes[prevName]);
            root.Nodes.Add(target.Name, target.Name);
            comboBox1.SelectedIndexChanged -= ComboBox1SelectedIndexChanged1;
            comboBox1.Items.Remove(target);
            comboBox1.Items.Add(target);
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(target);
            comboBox1.SelectedIndexChanged += ComboBox1SelectedIndexChanged1;
        }

        protected void DeleteSelectedObject()
        {
            Log("==DeleteSelectedObject==");
            if (MultiSelectMode)
            {
                Log("Multiselec mode delete");
                AddActionToList(new Action(Action.ActionType.DeleteMultipleObjects) { Targets = xnaWindow1.MultiSelectedObjects.ToArray() });
                DeleteMultipleObjects(xnaWindow1.MultiSelectedObjects.ToArray());
                return;
            }
            AddActionToList(new Action(Action.ActionType.DeleteObject) { Target = xnaWindow1.SelectedObject});
            DeleteObject(xnaWindow1.SelectedObject);
            
        }

        protected void HandleWindowKeys(object sender, KeyEventArgs e)
        {
            if (_playing)
            {
                switch (e.KeyData)
                {
                    case Keys.Escape:
                        StopGame(this, null);
                        e.Handled = true;
                        break;
                    case Keys.F6:
                        if (!_paused)
                            PauseGame(this, null);
                        e.Handled = true;
                        break;
                    case Keys.F5:
                        if (_paused)
                            RunGame(this, null);
                        e.Handled = true;
                        break;
                }
                return;
            }
            switch (e.KeyData)
            {
                case Keys.Left:
                    if (hScrollBar1.Value - 1 >= hScrollBar1.Minimum)
                        hScrollBar1.Value--;
                    e.Handled = true;
                    break;
                case Keys.Right:
                    if (hScrollBar1.Value + 10 <= hScrollBar1.Maximum)
                        hScrollBar1.Value++;
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (vScrollBar1.Value - 1 >= hScrollBar1.Minimum)
                        vScrollBar1.Value--;
                    e.Handled = true;
                    break;
                case Keys.Down:
                    if (vScrollBar1.Value + 10 <= vScrollBar1.Maximum)
                        vScrollBar1.Value++;
                    e.Handled = true;
                    break;
                case Keys.V:
                    if ((MultiSelectMode || xnaWindow1.SelectedObject != null) && ModifierKeys != Keys.Control)
                    {
                        if (!MultiSelectMode)
                        {
                            ((PhysicalObject) xnaWindow1.SelectedObject).Flip =
                                ((PhysicalObject) xnaWindow1.SelectedObject).Flip == SpriteEffects.FlipVertically
                                    ? SpriteEffects.None
                                    : SpriteEffects.FlipVertically;
                        }
                        else
                        {
                            foreach (PhysicalObject editorObject in xnaWindow1.MultiSelectedObjects)
                            {
                                editorObject.Flip =
                                editorObject.Flip == SpriteEffects.FlipVertically
                                    ? SpriteEffects.None
                                    : SpriteEffects.FlipVertically;
                            }
                        }
                    }
                    break;
                case Keys.H:
                    if ((MultiSelectMode || xnaWindow1.SelectedObject != null) && ModifierKeys != Keys.Control)
                    {
                        if (!MultiSelectMode)
                        {
                            ((PhysicalObject)xnaWindow1.SelectedObject).Flip =
                                ((PhysicalObject)xnaWindow1.SelectedObject).Flip == SpriteEffects.FlipHorizontally
                                    ? SpriteEffects.None
                                    : SpriteEffects.FlipHorizontally;
                        }
                        else
                        {
                            foreach (PhysicalObject editorObject in xnaWindow1.MultiSelectedObjects)
                            {
                                editorObject.Flip =
                                editorObject.Flip == SpriteEffects.FlipHorizontally
                                    ? SpriteEffects.None
                                    : SpriteEffects.FlipHorizontally;
                            }
                        }
                    }
                    break;
                case Keys.Delete:
                    if (RegionEditingMode)
                    {
                        if (xnaWindow1.SelectedRegion != null)
                        {
                            DeleteSelectedRegion();
                        }
                    }
                    else
                    {
                        if (xnaWindow1.SelectedObject != null || MultiSelectMode)
                        {
                            DeleteSelectedObject();
                        }
                    }
                    e.Handled = true;
                    break;
                case Keys.F5:
                    if (runToolStripMenuItem.Enabled)
                        RunGame(null, null);
                    e.Handled = true;
                    break;
                case Keys.Add:
                    if (zoom25.Checked)
                    {
                        zoom50.Checked = true;
                        ZoomItemChecked(zoom50, new EventArgs());
                    }
                    else if (zoom50.Checked)
                    {
                        zoom100.Checked = true;
                        ZoomItemChecked(zoom100, new EventArgs());
                    }
                    else if (zoom100.Checked)
                    {
                        zoom200.Checked = true;
                        ZoomItemChecked(zoom200, new EventArgs());
                    }
                    e.Handled = true;
                    break;
                case Keys.Subtract:
                    if (zoom200.Checked)
                    {
                        zoom100.Checked = true;
                        ZoomItemChecked(zoom100, new EventArgs());
                    }
                    else if (zoom100.Checked)
                    {
                        zoom50.Checked = true;
                        ZoomItemChecked(zoom50, new EventArgs());
                    }
                    else if (zoom50.Checked)
                    {
                        zoom25.Checked = true;
                        ZoomItemChecked(zoom25, new EventArgs());
                    }
                    e.Handled = true;
                    break;
                
                default:
                    if (e.KeyCode == Keys.Z && (e.Control) && menuStripUndoButton.Enabled)
                        Undo();
                    if (e.KeyCode == Keys.Y && (e.Control) && menuStripRedoButton.Enabled)
                        Redo();
                    if (e.KeyCode == Keys.C && (e.Control) && menuStripCopyButton.Enabled)
                        Copy();
                    if (e.KeyCode == Keys.X && (e.Control) && menuStripCutButton.Enabled)
                        Cut();
                    if (e.KeyCode == Keys.V && (e.Control) && menuStripPasteButton.Enabled)
                        Paste(false);
                    break;
            }
        }

        private void DeleteSelectedRegion()
        {
            Log("==DeleteSelectedRegion==");
            AddActionToList(new Action(Action.ActionType.DeleteRegion) { Target = xnaWindow1.SelectedRegion });
            DeleteRegion(xnaWindow1.SelectedRegion);
        }

        protected void CheckResizeCursor(IEditorObject targetObj)
        {

            if (targetObj == null ||
                !targetObj.Rectangle.Contains(xnaWindow1.MousePosReal.X, xnaWindow1.MousePosReal.Y))
            {
                List<IEditorObject> collidingObjects = null;
                if (BackgroundMode)
                {
                    collidingObjects = GameGlobals.Map.BackgroundObjects.Cast<IEditorObject>().Where(
                        editorObject =>
                        editorObject.Rectangle.Contains(xnaWindow1.MousePosReal.X, xnaWindow1.MousePosReal.Y)).ToList();

                }
                else if (RegionEditingMode)
                {
                    if (toolStripNewRegionButton.Checked)
                    {
                        Cursor = Cursors.Arrow;
                        return;
                    }
                    collidingObjects = GameGlobals.Map.Regions.Cast<IEditorObject>().Where(
                        editorObject =>
                        editorObject.Rectangle.Contains(xnaWindow1.MousePosReal.X, xnaWindow1.MousePosReal.Y)).ToList();
                }
                else
                {
                    collidingObjects = GameGlobals.Map.GameObjects.Cast<IEditorObject>().Where(
                       editorObject =>
                       editorObject.Rectangle.Contains(xnaWindow1.MousePosReal.X, xnaWindow1.MousePosReal.Y)).ToList();
                }
                IEditorObject topMost = null;
                foreach (var collidingObject in collidingObjects)
                {
                    if (topMost == null || collidingObject.LayerDepth < topMost.LayerDepth)
                        topMost = collidingObject;
                }
                targetObj = topMost;
            }
            if (MultiSelectMode && xnaWindow1.MultiSelectedObjects.Contains(targetObj))
            {
                Cursor = Cursors.SizeAll;
                return;
            }
            if (targetObj is PhysicalObject)
            {
                if (!Enum.GetValues(typeof(ResizeType)).Cast<int>().Where(value => value != (int)ResizeType.None).Any(item => targetObj.IsResizeAvailable((ResizeType)item)))
                {
                    Cursor = Cursors.SizeAll;
                    return;
                }
            }

            if (targetObj != null)
            {
                var resizeType = GetResizeType(xnaWindow1.MousePosReal.X, xnaWindow1.MousePosReal.Y, targetObj);
                switch (resizeType)
                {
                    case ResizeType.Right:
                        Cursor = Cursors.SizeWE;
                        break;
                    case ResizeType.BottomRight:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case ResizeType.Bottom:
                        Cursor = Cursors.SizeNS;
                        break;
                    case ResizeType.Top:
                        Cursor = Cursors.SizeNS;
                        break;
                    case ResizeType.Left:
                        Cursor = Cursors.SizeWE;
                        break;
                    case ResizeType.TopLeft:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case ResizeType.TopRight:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case ResizeType.BottomLeft:
                        Cursor = Cursors.SizeNESW;
                        break;
                    default:
                        Cursor = Cursors.SizeAll;
                        break;
                }
                return;
            }
            Cursor = Cursors.Arrow;
        }

        protected ResizeType GetResizeType(int mouseX, int mouseY, IEditorObject targetObj)
        {
            var rect = targetObj.Rectangle.GetRectangle();
            if (rect.Width >= 6 && rect.Height >= 6)
            {
                if (mouseY >= rect.Y && mouseY <= rect.Y + 3 && mouseX < rect.X + rect.Width &&
                    mouseX >= rect.X + rect.Width - 3)
                {
                    return targetObj.IsResizeAvailable(ResizeType.TopRight) ? ResizeType.TopRight : ResizeType.None;
                }
                if (mouseY >= rect.Y + rect.Height - 3 && mouseY < rect.Y + rect.Height && mouseX <= rect.X + 3 &&
                    mouseX >= rect.X)
                {
                    return targetObj.IsResizeAvailable(ResizeType.BottomLeft) ? ResizeType.BottomLeft : ResizeType.None;
                }
                if (mouseY >=
                    rect.Y + rect.Height - 3 &&
                    mouseY <
                    rect.Y + rect.Height &&
                    mouseX >=
                    rect.X + rect.Width - 3 &&
                    mouseX <
                    rect.X + rect.Width)
                {
                    return targetObj.IsResizeAvailable(ResizeType.BottomRight) ? ResizeType.BottomRight : ResizeType.None;
                }

                if (mouseX >=
                    rect.X + rect.Width - 3 &&
                    mouseX <
                    rect.X + rect.Width)
                {
                    return targetObj.IsResizeAvailable(ResizeType.Right)? ResizeType.Right : ResizeType.None;
                }
                if (mouseY >=
                    rect.Y + rect.Height - 3 &&
                    mouseY <
                    rect.Y + rect.Height)
                {
                    return targetObj.IsResizeAvailable(ResizeType.Bottom) ? ResizeType.Bottom : ResizeType.None;
                }
                if (mouseY >= rect.Y && mouseY <= rect.Y + 3 && mouseX <= rect.X + 3 && mouseX >= rect.X)
                {
                    return ResizeType.TopLeft;
                }
                if (mouseY >=
                    rect.Y &&
                    mouseY <=
                    rect.Y + 3)
                {
                    return targetObj.IsResizeAvailable(ResizeType.Top) ? ResizeType.Top : ResizeType.None;
                }
                if (mouseX <= rect.X + 3 && mouseX >= rect.X)
                {
                    return targetObj.IsResizeAvailable(ResizeType.Left) ? ResizeType.Left : ResizeType.None;
                }


                return ResizeType.None;
            }
            return ResizeType.None;
        }

        protected void DoResize(ResizeType resizeType, ref Rectangle rectangle)
        {

            switch (resizeType)
            {
                case ResizeType.Bottom:
                    if (xnaWindow1.MousePosReal.Y - rectangle.Y < EngineGlobals.Grid.CellHeight)
                        rectangle = new Rectangle(rectangle.X,
                                                  rectangle.Y,
                                                  rectangle.Width,
                                                  EngineGlobals.Grid.CellHeight);
                    else if (xnaWindow1.MousePosReal.Y >= EngineGlobals.Grid.HeightReal)
                    {
                        rectangle = new Rectangle(rectangle.X,
                                                  rectangle.Y,
                                                  rectangle.Width,
                                                  EngineGlobals.Grid.HeightReal -
                                                  rectangle.Y);
                    }
                    else
                    {
                        rectangle = new Rectangle(rectangle.X,
                                                  rectangle.Y,
                                                  rectangle.Width,
                                                  xnaWindow1.MousePosCell.Y * EngineGlobals.Grid.CellHeight -
                                                  rectangle.Y);
                    }
                    break;
                case ResizeType.Top:
                    var newHeight = _resizeRectStart.Y + _resizeRectStart.Height -
                                    xnaWindow1.MousePosCell.Y*EngineGlobals.Grid.CellHeight;
                    var newY = xnaWindow1.MousePosCell.Y*EngineGlobals.Grid.CellHeight;
                    if (newHeight < EngineGlobals.Grid.CellHeight)
                    {
                        newHeight = EngineGlobals.Grid.CellHeight;
                        newY = _resizeRectStart.Y + _resizeRectStart.Height - EngineGlobals.Grid.CellHeight;
                    }
                    if (newY < 0)
                        newY = 0;
                    rectangle = new Rectangle(rectangle.X, newY , rectangle.Width, newHeight);
                    break;
                case ResizeType.Left:
                    var newWidth =
                        _resizeRectStart.X + _resizeRectStart.Width -
                        xnaWindow1.MousePosCell.X*EngineGlobals.Grid.CellWidth;
                    var newX = xnaWindow1.MousePosCell.X*EngineGlobals.Grid.CellWidth;
                    if (newWidth < EngineGlobals.Grid.CellWidth)
                    {
                        newWidth = EngineGlobals.Grid.CellWidth;
                        newX = _resizeRectStart.X + _resizeRectStart.Width - EngineGlobals.Grid.CellWidth;
                    }
                    if (newX < 0)
                        newX = 0;
                    rectangle = new Rectangle(newX, rectangle.Y, newWidth, rectangle.Height);
                    break;
                case ResizeType.Right:
                    if (xnaWindow1.MousePosReal.X - rectangle.X < EngineGlobals.Grid.CellWidth)
                        rectangle = new Rectangle(rectangle.X,
                                                  rectangle.Y,
                                                  EngineGlobals.Grid.CellWidth,
                                                  rectangle.Height);
                    else if (xnaWindow1.MousePosReal.X >= EngineGlobals.Grid.WidthReal)
                    {
                        rectangle = new Rectangle(rectangle.X,
                                                  rectangle.Y,
                                                  EngineGlobals.Grid.WidthReal -
                                                  rectangle.X,
                                                  rectangle.Height);
                    }
                    else
                    {
                        rectangle = new Rectangle(rectangle.X,
                                                  rectangle.Y,
                                                  xnaWindow1.MousePosCell.X * EngineGlobals.Grid.CellWidth -
                                                  rectangle.X,
                                                  rectangle.Height);
                    }
                    break;

            }
        }

        protected void Undo()
        {
            Log("==Undo==");
            menuStripRedoButton.Enabled = true;
            toolStripRedoButton.Enabled = true;
            ActionIndex--;
            var a = Actions[ActionIndex];
            Log("Action from list( at " + ActionIndex + "): " + a);

            if (ActionIndex == 0)
            {
                menuStripUndoButton.Enabled = false;
                toolStripUndoButton.Enabled = false;
            }
            var multipleObjects = (a.ExecutedAction == Action.ActionType.MultipleObjectsMultiProperyChange || a.ExecutedAction == Action.ActionType.DeleteMultipleObjects ||
                a.ExecutedAction == Action.ActionType.CreateMultipleObjects) ;
            if (!multipleObjects)
            {
                Log("Disabling propertyChanged and proeprtyChanging events for object: '" + a.Target.EditorGetName() + "'");
                a.Target.PropertyChanging -= ObjectDataPropertyChanging;
                a.Target.PropertyChanged -= ObjectDataPropertyChanged;
            }
            
            switch (a.ExecutedAction)
            {
                case Action.ActionType.DeleteObject:
                    ReviveDeletedObject(a.Target);
                    break;
                case Action.ActionType.CreateObject:
                    DeleteObject(a.Target);
                    break;
                case Action.ActionType.DeleteRegion:
                    ReviveDeletedRegion((Region)a.Target);
                    break;
                case Action.ActionType.CreateRegion:
                    DeleteRegion((Region)a.Target);
                    break;
                case Action.ActionType.MultiPropertyChange:
                    var propertiesBeforeChange = ((Dictionary<string, object>) a.PropertyBeforeChange);
                    var propertiesAfterChange = ((Dictionary<string, object>) a.PropertyAfterChange);
                    foreach (var p in propertiesAfterChange)
                    {
                        UndoPropertyChange(new Action(Action.ActionType.PropertyChange){PropertyBeforeChange = propertiesBeforeChange[p.Key], PropertyAfterChange = p.Value, PropertyName = p.Key, Target = a.Target});
                    }
                    break;
                case Action.ActionType.MultipleObjectsMultiProperyChange:
                    UndoMultiObjectsMultiPropertyChange(a);
                    break;
                case Action.ActionType.DeleteMultipleObjects:
                    ReviveMultipleDeletedObjects(a.Targets.ToList());
                    break;
                case Action.ActionType.CreateMultipleObjects:
                    DeleteMultipleObjects(a.Targets);
                    break;
                case Action.ActionType.PropertyChange:
                    UndoPropertyChange(a);
                    break;
            }
            if (a.Target != null && GameGlobals.Map.GameObjects.Contains(a.Target) && !multipleObjects)
            {
                if (xnaWindow1.SelectedObject == a.Target)
                {
                    Log("Item already selected. Enabling propertyChanged and proeprtyChanging events for object: '" +
                        a.Target.EditorGetName() + "'");
                    a.Target.PropertyChanging += ObjectDataPropertyChanging;
                    a.Target.PropertyChanged += ObjectDataPropertyChanged;
                }
                else
                {
                    Log("Select item: " + a.Target.EditorGetName());
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(a.Target);
                }
            }
            //a.Target.ObjectDataRef.PropertyChanging += ObjectDataPropertyChanging;
            //a.Target.ObjectDataRef.PropertyChanged += ObjectDataPropertyChanged;
        }

        protected void Redo()
        {
            Log("==Redo==");
            var a = Actions[ActionIndex];
            ActionIndex++;
            Log("Action from list( at " + ActionIndex + "): " + a);

            if (Actions.Count == ActionIndex)
            {
                menuStripRedoButton.Enabled = false;
                toolStripRedoButton.Enabled = false;
            }
            var multipleObjects = (a.ExecutedAction == Action.ActionType.MultipleObjectsMultiProperyChange || a.ExecutedAction == Action.ActionType.DeleteMultipleObjects ||
                a.ExecutedAction == Action.ActionType.CreateMultipleObjects);
            if (!multipleObjects)
            {
                Log("Disabling propertyChanged and proeprtyChanging events for object: '" + a.Target.EditorGetName() +
                    "'");
                a.Target.PropertyChanging -= ObjectDataPropertyChanging;
                a.Target.PropertyChanged -= ObjectDataPropertyChanged;
            }
            switch (a.ExecutedAction)
            {
                case Action.ActionType.DeleteObject:
                    DeleteObject(a.Target);
                    break;
                case Action.ActionType.CreateObject:
                    ReviveDeletedObject(a.Target);
                    break;
                case Action.ActionType.DeleteRegion:
                    DeleteRegion((Region)a.Target);
                    break;
                case Action.ActionType.CreateRegion:
                    ReviveDeletedRegion((Region)a.Target);
                    break;
                case Action.ActionType.PropertyChange:
                    RedoPropertyChange(a);
                    if (xnaWindow1.SelectedObject == a.Target)
                    {
                        Log("Item already selected. Enabling propertyChanged and proeprtyChanging events for object: '" +
                            a.Target.EditorGetName() + "'");
                        a.Target.PropertyChanging += ObjectDataPropertyChanging;
                        a.Target.PropertyChanged += ObjectDataPropertyChanged;
                    }
                    else
                    {
                        Log("Select item: " + a.Target.EditorGetName());
                        comboBox1.SelectedIndex = comboBox1.Items.IndexOf(a.Target);
                    }
                    break;
                case Action.ActionType.MultiPropertyChange:
                    var propertiesBeforeChange = ((Dictionary<string, object>) a.PropertyBeforeChange);
                    var propertiesAfterChange = ((Dictionary<string, object>) a.PropertyAfterChange);
                    foreach (var p in propertiesAfterChange)
                    {
                        RedoPropertyChange(new Action(Action.ActionType.PropertyChange){PropertyBeforeChange = propertiesBeforeChange[p.Key], PropertyAfterChange = p.Value, PropertyName = p.Key, Target = a.Target});
                    }
                    if (xnaWindow1.SelectedObject == a.Target)
                    {
                        Log("Item already selected. Enabling propertyChanged and proeprtyChanging events for object: '" +
                            a.Target.EditorGetName() + "'");
                        a.Target.PropertyChanging += ObjectDataPropertyChanging;
                        a.Target.PropertyChanged += ObjectDataPropertyChanged;
                    }
                    else
                    {
                        Log("Select item: " + a.Target.EditorGetName());
                        comboBox1.SelectedIndex = comboBox1.Items.IndexOf(a.Target);
                    }
                    break;
                case Action.ActionType.MultipleObjectsMultiProperyChange:
                    RedoMultiObjectsMultiPropertyChange(a);
                    break;
                case Action.ActionType.CreateMultipleObjects:
                    ReviveMultipleDeletedObjects(a.Targets.ToList());
                    break;
                case Action.ActionType.DeleteMultipleObjects:
                    DeleteMultipleObjects(a.Targets);
                    break;
            }
            menuStripUndoButton.Enabled = true;
            toolStripUndoButton.Enabled = true;
            //a.Target.ObjectDataRef.PropertyChanging += ObjectDataPropertyChanging;
            //a.Target.ObjectDataRef.PropertyChanged += ObjectDataPropertyChanged;
        }

        public void AddActionToList(Action action)
        {
            Log("==AddActionToList==");
            menuStripRedoButton.Enabled = false;
            toolStripRedoButton.Enabled = false;
            Log("Remove last " + (Actions.Count - ActionIndex) + " actions from list");
            Actions.RemoveRange(ActionIndex, Actions.Count - ActionIndex);
            Log("Add new action to list: " + action);
            Actions.Add(action);
            ActionIndex++;
            Log("Add action index is now: " + ActionIndex);
            menuStripUndoButton.Enabled = true;
            toolStripUndoButton.Enabled = true;
        }

        public void UpdateMouse(int x, int y)
        {
            var worldPos = BackgroundMode? EngineGlobals.Camera2D.ViewToBackgroundWorld(new Vector2(x,y)) : EngineGlobals.Camera2D.ViewToWorld(new Vector2(x,y));
            var posX = float.IsNaN(worldPos.X)? 0f : worldPos.X;
            var posY = float.IsNaN(worldPos.Y)? 0f : worldPos.Y;
            xnaWindow1.MousePosReal = new Point((int)posX, (int)posY);
            if (posX > EngineGlobals.Grid.WidthReal)
                posX = EngineGlobals.Grid.WidthReal;
            if (posX < 0)
                posX = 0;

            if (posY > EngineGlobals.Grid.HeightReal)
                posY = EngineGlobals.Grid.HeightReal;
            if (posY < 0)
                posY = 0;
            xnaWindow1.MousePosCell = EngineGlobals.Grid.ToCell(new Vector2(posX, posY));
        }

        public void DeselectObject()
        {
            Log("==DeselectObject==");
            if (MultiSelectMode)
            {
                Log("MultiSelectedMode - deselecting multi selected objects");
                DeselectMultiSelectedObjects(GameGlobals.Map);
                return;
            }
            if (xnaWindow1.SelectedObject == null)
            {
                Log("No object is selected - end");
                return;
            }
            Log("Deselecting object: '" + xnaWindow1.SelectedObject.EditorGetName() + "'");
            Log("Disabling propertyChanged and proeprtyChanging events for object: '" + xnaWindow1.SelectedObject.EditorGetName() + "'");
            xnaWindow1.SelectedObject.PropertyChanged -= ObjectDataPropertyChanged;
            xnaWindow1.SelectedObject.PropertyChanging -= ObjectDataPropertyChanging;
            xnaWindow1.DeselectObject();
            RefreshMenus();
        }

        public void AddResource(ResourceIdentifier resource)
        {
            Log("Checking for existing resources");
            if (resource.ResourceType == ResourceType.Texture)
            {
                if (GameGlobals.Map.Resources.TextureIdentifiers[resource.Name] == null)
                {
                    Log("Addeding resource to map");
                    GameGlobals.Map.Resources.Add(resource);
                    RefreshTreeRoot(TreeNodes.Resources);
                }
            }
            else if (resource.ResourceType == ResourceType.Sprite)
            {
                if (GameGlobals.Map.Resources.SpriteIdentifiers[resource.Name] == null)
                {
                    Log("Addeding resource to map");
                    GameGlobals.Map.Resources.Add(resource);
                    
                }
            }
            else
            {
                throw new Exception("Unsuported resource type: " + resource.ResourceType);
            }
        }

        public void RemoveResource(string resourceId)
        {
            Log("Checking resources to remove");
            var resourceFound = BackgroundMode
                                    ? GameGlobals.Map.BackgroundObjects.Any(backgroundObject => resourceId == backgroundObject.ResourceId)
                                    : GameGlobals.Map.GameObjects.Any(physicalObject => resourceId == physicalObject.ResourceId);
            if (!resourceFound)
            {
                Log("Removing resource('" + resourceId + "') from map resources");
                GameGlobals.Map.Resources.Remove(resourceId);
                RefreshTreeRoot(TreeNodes.Resources);
            }
        }

        public void SelectPassive(PhysicalObject obj)
        {
            _passiveSelection = true;
            Log("Selecting(passive) object at " + (comboBox1.Items.IndexOf(obj) - 1));
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(obj);
        }

        public void ChangeZoom(float zoom)
        {
            Log("Chaging zoom to " + zoom);
            //apply zoom
            EngineGlobals.Camera2D.Zoom = zoom;
            //check if out of map bounds
            var camCorner = EngineGlobals.Camera2D.CornerPos;
            if (camCorner.X + (xnaWindow1.Width / zoom) > EngineGlobals.Grid.WidthReal)
            {
                camCorner.X = EngineGlobals.Grid.WidthReal - (xnaWindow1.Width/zoom);
            }
            if (camCorner.X < 0)
                camCorner.X = 0;
            //if (camCorner.Y + (xnaWindow1.Height / zoom) > EngineGlobals.Grid.HeightReal)
            //    camCorner.Y = EngineGlobals.Grid.WidthReal - (xnaWindow1.Height / zoom);
            if (camCorner.Y < 0)
                camCorner.Y = 0;
            EngineGlobals.Camera2D.CornerPos = camCorner;
            _zoom = zoom;
            var max = (int) ((EngineGlobals.Grid.GridRectangle.Width - 80/zoom)*zoom) + 9;
            if (max < 0)
                max = 0;
            var newVal = (int)(EngineGlobals.Camera2D.CornerPos.X / 10 * zoom);
            if (newVal > max)
                newVal = max;
            hScrollBar1.Maximum = max;
            hScrollBar1.Value = newVal;
            max = (int)(EngineGlobals.Grid.GridRectangle.Height * zoom - 48) + 9;
            if (max < 0)
                max = 0;
            newVal = (int)(EngineGlobals.Camera2D.CornerPos.Y / 10 * zoom);
            if (newVal > max)
                newVal = max;
            vScrollBar1.Maximum = max;
            vScrollBar1.Value = newVal;
            
        }

        public void RefreshMenus()
        {
            if (xnaWindow1.SelectedObject != null && !RegionEditingMode || MultiSelectMode)
            {
                cutToolStripMenuItem1.Enabled = true;
                copyToolStripMenuItem1.Enabled = true;
                deleteToolStripMenuItem.Enabled = true;
                menuStripDeleteButton.Enabled = true;
                menuStripCopyButton.Enabled = true;
                menuStripCutButton.Enabled = true;
                toolStripCutButton.Enabled = true;
                toolStripCopyButton.Enabled = true;
                toolStripDeleteButton.Enabled = true;
                
                
                if (!MultiSelectMode)
                {
                    flipHorizontallyToolStripMenuItem.Enabled = true;
                    flipVerticallyToolStripMenuItem.Enabled = true;
                    rotateToolStripMenuItem.Enabled = xnaWindow1.SelectedObject is CirclePhysicalObject;
                    setDestinationToolStripMenuItem.Enabled =
                        xnaWindow1.SelectedObject.EditorIsDebugFlagEnabled(DebugFlag.Destination) || xnaWindow1.SelectedObject.EditorIsDebugFlagEnabled(DebugFlag.Path);

                    if (((xnaWindow1.SelectedObject.LayerDepth - 0.2f) > EngineGlobals.Epsilon && !BackgroundMode) ||
                        (xnaWindow1.SelectedObject.LayerDepth - 0.8f > EngineGlobals.Epsilon) && BackgroundMode)
                    {
                        menuStripBringForwardButton.Enabled = true;
                        menuStripBringToFrontButton.Enabled = true;
                        toolStripBringForwardButton.Enabled = true;
                        toolStripBringToFrontButton.Enabled = true;
                        bringForwardToolStripMenuItem.Enabled = true;
                        bringToFrontToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        menuStripBringForwardButton.Enabled = false;
                        menuStripBringToFrontButton.Enabled = false;
                        toolStripBringForwardButton.Enabled = false;
                        toolStripBringToFrontButton.Enabled = false;
                        bringForwardToolStripMenuItem.Enabled = false;
                        bringToFrontToolStripMenuItem.Enabled = false;
                    }
                    if (((0.8f - xnaWindow1.SelectedObject.LayerDepth) > EngineGlobals.Epsilon && !BackgroundMode) ||
                        (0.9f - xnaWindow1.SelectedObject.LayerDepth > EngineGlobals.Epsilon && BackgroundMode))
                    {
                        menuStripSendBackwardButton.Enabled = true;
                        menuStripSendToBackButton.Enabled = true;
                        toolStripSendBackwardButton.Enabled = true;
                        toolStripSendToBackButton.Enabled = true;
                        sendBackwardToolStripMenuItem.Enabled = true;
                        sendToBackToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        menuStripSendBackwardButton.Enabled = false;
                        menuStripSendToBackButton.Enabled = false;
                        toolStripSendBackwardButton.Enabled = false;
                        toolStripSendToBackButton.Enabled = false;
                        sendBackwardToolStripMenuItem.Enabled = false;
                        sendToBackToolStripMenuItem.Enabled = false;
                    }
                }
            }
            else
            {
                if (RegionEditingMode && xnaWindow1.SelectedRegion != null)
                {
                    menuStripDeleteButton.Enabled = true;
                    deleteToolStripMenuItem.Enabled = true;
                    toolStripDeleteButton.Enabled = true;
                }
                else
                {
                    toolStripDeleteButton.Enabled = false;
                    deleteToolStripMenuItem.Enabled = false;
                    menuStripDeleteButton.Enabled = false;
                }
                cutToolStripMenuItem1.Enabled = false;
                copyToolStripMenuItem1.Enabled = false;
                menuStripBringForwardButton.Enabled = false;
                menuStripBringToFrontButton.Enabled = false;
                menuStripSendBackwardButton.Enabled = false;
                menuStripSendToBackButton.Enabled = false;
                toolStripBringForwardButton.Enabled = false;
                toolStripBringToFrontButton.Enabled = false;
                toolStripSendBackwardButton.Enabled = false;
                toolStripSendToBackButton.Enabled = false;
                sendBackwardToolStripMenuItem.Enabled = false;
                sendToBackToolStripMenuItem.Enabled = false;
                bringForwardToolStripMenuItem.Enabled = false;
                bringToFrontToolStripMenuItem.Enabled = false;
                toolStripCutButton.Enabled = false;
                toolStripCopyButton.Enabled = false;
                rotateToolStripMenuItem.Enabled = false;
                setDestinationToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem1.Enabled = _clipboardObject != null;
                flipHorizontallyToolStripMenuItem.Enabled = false;
                flipVerticallyToolStripMenuItem.Enabled = false;
            }

            if (RegionEditingMode)
            {
                toolStripNewRegionButton.Enabled = true;
            }
            else
            {
                toolStripNewRegionButton.Enabled = false;
            }
        }

        private void RunGame(object sender, EventArgs e)
        {
            if (_paused)
            {
                Log("Unpausing game");
                _paused = false;
                _testGame.SetState(_paused);
                runToolStripMenuItem.Enabled = false;
                toolStripRunButton.Enabled = false;
                pauseToolStripMenuItem.Enabled = true;
                toolStripPauseButton.Enabled = true;
                return;
            }
            Log("==RunGame==");
            SaveTemporary();
            if (!BackgroundMode && !_menuMode)
            {
                var playerCount = GameGlobals.Map.GameObjects.OfType<Player>().Count();
                if (playerCount > 1)
                {
                    MessageBox.Show("Error: more than one player exist in the map. Cannot start game.");
                    return;
                }
                if (playerCount < 1)
                {
                    MessageBox.Show("No player exists on the map. Cannot start game.");
                    return;   
                }
            }
            Log("Disabling menu items");
            foreach (var dropDownItem in fileToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                dropDownItem.Enabled = false;
            }
            foreach (var dropDownItem in editToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                dropDownItem.Enabled = false;
            }
            foreach (var dropDownItem in viewToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                dropDownItem.Enabled = false;
            }
            toolStripNewButton.Enabled = false;
            toolStripOpenButton.Enabled = false;
            toolStripOpenLastButton.Enabled = false;
            toolStripSaveButton.Enabled = false;
            toolStripCutButton.Enabled = false;
            toolStripCopyButton.Enabled = false;
            toolStripPasteButton.Enabled = false;
            toolStripDeleteButton.Enabled = false;
            toolStripUndoButton.Enabled = false;
            toolStripRedoButton.Enabled = false;
            toolStripSendBackwardButton.Enabled = false;
            toolStripSendToBackButton.Enabled = false;
            toolStripBringForwardButton.Enabled = false;
            toolStripBringToFrontButton.Enabled = false;
            toolStripNewRegionButton.Enabled = false;
            toolStripRegionEditorButton.Enabled = false;
            toolStripTriggerEditorButton.Enabled = false;
            triggerEditorToolStripMenuItem.Enabled = false;
            toolStripImportBackgroundButton.Enabled = false;
            spriteEditorToolStripMenuItem.Enabled = false;
            contextMenuStrip1.Enabled = false;
            toolbox1.Enabled = false;
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    control.Enabled = false;
                }
            }
            if (!BackgroundMode)
            {
                vScrollBar1.Enabled = false;
                hScrollBar1.Enabled = false;
            }
            runToolStripMenuItem.Enabled = false;
            toolStripRunButton.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;
            toolStripPauseButton.Enabled = true;
            stopToolStripMenuItem.Enabled = true;
            toolStripStopButton.Enabled = true;
            GameGlobals.EditorMode = false;
            if (!BackgroundMode)
            {
                Log("Creating game manager");
                _playing = true;
                EngineGlobals.InGame = true;
                if (GameGlobals.Map.MapType == MapType.Menu)
                    _testGame = new EditorMenuManager(GameGlobals.Map);
                else
                    _testGame = new EditorGameManager(GameGlobals.Map);
                Log("Registering Draw redirect");
                xnaWindow1.DrawRedirect += _testGame.Draw;
            }
            else
            {
                Log("Creating background viewer");
                _testGame = new BackgroundViewer(GameGlobals.Map);
                Log("Registering Draw redirect");
                xnaWindow1.DrawRedirect += _testGame.Draw;
            }
        }

        public void StopGame(object sender, EventArgs e)
        {
            Log("==StopGame==");
            Log("Removing Draw redirect");
            xnaWindow1.DrawRedirect -= _testGame.Draw;
            Log("Enabling menu items");
            foreach (var dropDownItem in fileToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                dropDownItem.Enabled = true;
            }
            foreach (var dropDownItem in viewToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                dropDownItem.Enabled = true;
            }
            toolbox1.Enabled = true;
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                foreach (Control control in tabPage.Controls)
                {
                    control.Enabled = true;
                }
            }
            toolStripUndoButton.Enabled = menuStripUndoButton.Enabled = ActionIndex > 0;
            toolStripRedoButton.Enabled = menuStripRedoButton.Enabled = ActionIndex < Actions.Count;
            toolStripPasteButton.Enabled = menuStripPasteButton.Enabled = _clipboardObject != null;
            vScrollBar1.Enabled = true;
            hScrollBar1.Enabled = true;
            runToolStripMenuItem.Enabled = toolStripRunButton.Enabled = true;
            pauseToolStripMenuItem.Enabled = toolStripPauseButton.Enabled = false;
            stopToolStripMenuItem.Enabled = toolStripStopButton.Enabled = false;
            toolStripRegionEditorButton.Enabled = true;
            toolStripTriggerEditorButton.Enabled = true;
            triggerEditorToolStripMenuItem.Enabled = true;
            toolStripNewButton.Enabled = true;
            toolStripOpenButton.Enabled = true;
            toolStripOpenLastButton.Enabled = true;
            toolStripImportBackgroundButton.Enabled = !BackgroundMode;
            spriteEditorToolStripMenuItem.Enabled = true;
            toolStripSaveButton.Enabled = true;
            contextMenuStrip1.Enabled = true;
            EngineGlobals.Gravity = EngineGlobals.DefaultGravity;
            Log("Disposing game manager");
            _testGame.Dispose();
            _playing = false;
            _paused = false;
            EngineGlobals.InGame = false;
            GameGlobals.EditorMode = true;
        }

        private void PauseGame(object sender, EventArgs e)
        {
            Log("Pausing game");
            _paused = true;
            _testGame.SetState(true);
            pauseToolStripMenuItem.Enabled = false;
            toolStripPauseButton.Enabled = false;
            runToolStripMenuItem.Enabled = true;
            toolStripRunButton.Enabled = true;
        }

        public Rectangle ResizeRegion(Point startPos, Point endPos)
        {
            var x = 0;
            var width = 0;
            var y = 0;
            var height = 0;
            if (startPos.X > endPos.X )
            {
                x = endPos.X;
                width = startPos.X - endPos.X;
            }
            else
            {
                x = startPos.X;
                width = endPos.X - startPos.X;
            }
            if (startPos.Y > endPos.Y)
            {
                y = endPos.Y;
                height = startPos.Y - endPos.Y;
            }
            else
            {
                y = startPos.Y;
                height = endPos.Y - startPos.Y;
            }
            if (width < EngineGlobals.Grid.CellWidth)
                width = EngineGlobals.Grid.CellWidth;
            if (height < EngineGlobals.Grid.CellHeight)
                height = EngineGlobals.Grid.CellHeight;
            return EngineGlobals.Grid.ToAlignedRectangle(new Rectangle(x, y, width, height));
        }

        public void SelectRegion(Region region)
        {
            Log("=SelectRegion=");
            if (region == xnaWindow1.SelectedRegion)
            {
                Log("Region already selected - end.");
                return;
            }
            Log("Deselect previously selected region");
            DeselectRegion();
            Log("Selecting region");
            xnaWindow1.SelectedRegion = region;
            propertyGrid1.SelectedObject = region;
            Log("Addeding property change events for region");
            region.PropertyChanging += ObjectDataPropertyChanging;
            region.PropertyChanged += ObjectDataPropertyChanged;
            RefreshMenus();
        }

        public void DeselectRegion()
        {
            Log("=DeselectRegion=");
            if (xnaWindow1.SelectedRegion != null)
            {
                Log("Removing property change events from region");
                xnaWindow1.SelectedRegion.PropertyChanging -= ObjectDataPropertyChanging;
                xnaWindow1.SelectedRegion.PropertyChanged -= ObjectDataPropertyChanged;
                Log("Deselecting region");
                xnaWindow1.SelectedRegion = null;
                comboBox1.SelectedItem = GameGlobals.Map;
                deleteToolStripMenuItem.Enabled = false;
                toolStripDeleteButton.Enabled = false;
                menuStripDeleteButton.Enabled = false;
            }
            else
            {
                Log("No region selected - end");
            }
        }

        public void EditorBeginCreateRegion(Point startPos, Point endPos)
        {
            Log("=EditorBeginCreateRegion=");
            var colors = new[]
                {
                    Color.Green, Color.Blue, Color.Red 
                };
            var color = colors[GameGlobals.Random.Next(0, colors.Length - 1)];
            var region = new Region(new RectangleF(ResizeRegion(startPos, endPos)), color, true);
            _creatingRegion = true;
            var i = 0;
            while(true)
            {
                var name = "Region" + i.ToString("000");
                if (GameGlobals.Map.Regions.Exists(r => r.Name == name))
                {
                    i++;
                    continue;
                }
                break;
            }
            region.Name = "Region" + i.ToString("000");
            Log("Addeding region '" + region.Name + "' into level");
            region.LayerDepth = 0.95f;
            AddObjectToTree(region);
            SelectRegion(region);
            GameGlobals.Map.Regions.Add(xnaWindow1.SelectedRegion);
        }

        public void EndCreatingRegion()
        {
            Log("=EndCreatingRegion=");
            _creatingRegion = false;
            toolStripNewRegionButton.Checked = false;
            AddActionToList(new Action(Action.ActionType.CreateRegion){Target = xnaWindow1.SelectedRegion});
            Log("Addeding region to combobox and selecting it");
            comboBox1.Items.Add(xnaWindow1.SelectedRegion);
            comboBox1.SelectedItem = xnaWindow1.SelectedRegion;

        }

        public void StartTriggerEditor()
        {
            if (TriggerEditorDialog != null)
            {
                TriggerEditorDialog.WindowState = FormWindowState.Normal;
                TriggerEditorDialog.BringToFront();
                return;
            }
            TriggerEditorDialog = new TriggerEditor.TriggerEditorDialog();
            TriggerEditorDialog.Closed += (o, args) => { TriggerEditorDialog = null; };
            TriggerEditorDialog.Show();
        }

        #endregion

        #region actions

        public void ItemSendBackward()
        {
            Log("Sending item('" + xnaWindow1.SelectedObject.EditorGetName() + "') backward from LayerDepth: " + xnaWindow1.SelectedObject.LayerDepth);
            if ((0.8f - xnaWindow1.SelectedObject.LayerDepth < EngineGlobals.Epsilon && !BackgroundMode) ||
                (0.9f - xnaWindow1.SelectedObject.LayerDepth < EngineGlobals.Epsilon && BackgroundMode))
            {
                Log("Item is already below min LayerDepth - end");
                return;
            }
            xnaWindow1.SelectedObject.LayerDepth = BackgroundMode?
                (float)Math.Round(xnaWindow1.SelectedObject.LayerDepth + 0.01f, 2) :
                (float)Math.Round(xnaWindow1.SelectedObject.LayerDepth + 0.1f, 1);
            RefreshMenus();
        }

        public void ItemSendToBack()
        {
            Log("Sending item('" + xnaWindow1.SelectedObject.EditorGetName() + "') to back");
            xnaWindow1.SelectedObject.LayerDepth = BackgroundMode?0.9f:0.8f;
            RefreshMenus();
        }

        public void ItemBringForward()
        {
            Log("Sending item('" + xnaWindow1.SelectedObject.EditorGetName() + "') forward from LayerDepth: " + xnaWindow1.SelectedObject.LayerDepth);
            if (((xnaWindow1.SelectedObject.LayerDepth - 0.2f) < EngineGlobals.Epsilon && !BackgroundMode) ||
                ((xnaWindow1.SelectedObject.LayerDepth - 0.8f) < EngineGlobals.Epsilon && BackgroundMode))
            {
                Log("Item is already above max LayerDepth - end");
                return;
            }
            xnaWindow1.SelectedObject.LayerDepth = BackgroundMode?
                (float)Math.Round(xnaWindow1.SelectedObject.LayerDepth - 0.01f, 2) :
                (float)Math.Round(xnaWindow1.SelectedObject.LayerDepth - 0.1f, 1);
            RefreshMenus();
        }

        public void ItemBringToFront()
        {
            Log("Sending item('" + xnaWindow1.SelectedObject.EditorGetName() + "') to fron");
            xnaWindow1.SelectedObject.LayerDepth = BackgroundMode?0.8f:0.2f;
            RefreshMenus();
        }

        public void DeleteObject(IEditorObject obj)
        {
            Log("=DeleteObject=");
            Log("Removing: " + obj.EditorGetName());
            
            if (xnaWindow1.SelectedObject == obj)
            {
                Log("PropertyGrid - deselect object");
                comboBox1.SelectedItem = GameGlobals.Map;
            }

            Log("ComboBox remove");
            comboBox1.Items.Remove(obj);

            Log("Removing object from Level");
            
            if (obj is BackgroundObject)
            {
                GameGlobals.Map.BackgroundObjects.Remove((PhysicalObject) obj);
                DeleteObjectFromTree(obj, TreeNodes.BackgroundObjects);
                Controller.RemoveBackgroundObject((BackgroundObject)obj);
            }
            else
            {
                GameGlobals.Map.GameObjects.Remove((PhysicalObject)obj);
                DeleteObjectFromTree(obj, TreeNodes.GameObjects);
                Controller.RemoveGameObject((PhysicalObject)obj);
            }
            
            RemoveResource(obj.ResourceId);
            xnaWindow1.DeselectObject();
            obj.EditorDeleteObject();
            obj.Dispose();
            menuStripCutButton.Enabled = false;
            menuStripCopyButton.Enabled = false;
            comboBox1.SelectedIndex = 0;
            RefreshMenus();
        }

        public void DeleteMultipleObjects(IEditorObject[] objects)
        {
            Log("=DeleteMultipleObjects=");
            Log("Removing: " + string.Join(",", objects.Cast<object>()));
            DeselectMultiSelectedObjects(GameGlobals.Map);
            foreach (var editorObject in objects)
            {
                comboBox1.Items.Remove(editorObject);
                if (editorObject is BackgroundObject)
                {
                    GameGlobals.Map.BackgroundObjects.Remove((PhysicalObject)editorObject);
                    DeleteObjectFromTree(editorObject, TreeNodes.BackgroundObjects);
                    Controller.RemoveBackgroundObject((BackgroundObject)editorObject);
                }
                else
                {
                    GameGlobals.Map.GameObjects.Remove((PhysicalObject)editorObject);
                    DeleteObjectFromTree(editorObject, TreeNodes.GameObjects);
                    Controller.RemoveGameObject((PhysicalObject)editorObject);
                }
                RemoveResource(editorObject.ResourceId);
                editorObject.EditorDeleteObject();

            }
            menuStripCutButton.Enabled = false;
            menuStripCopyButton.Enabled = false;
            RefreshMenus();
        }

        public void ReviveDeletedObject(IEditorObject obj)
        {
            Log("=ReviveDeletedObject==");
            Log("Add object(" + obj.EditorGetName() + ") to GameObjects and DateRef To ObjectDatas");
            Controller.AddObject(obj.EditorGetVisualObject());
            if (obj is BackgroundObject)
            {
                GameGlobals.Map.BackgroundObjects.Add((PhysicalObject)obj);
                AddObjectToTree(obj);
            }
            else
            {
                GameGlobals.Map.GameObjects.Add((PhysicalObject)obj);
                AddObjectToTree(obj);
            }
            AddResource(EditorResources.GetResourceIdentifierById(obj.ResourceId));
            
            Log("Add revived object to ComboBox");
            comboBox1.Items.Add(obj);
            Log("Select revived object");
            comboBox1.SelectedItem = obj;
        }

        public void ReviveMultipleDeletedObjects(List<IEditorObject> objects)
        {
            Log("=ReviveDeletedObject==");
            Log("Add objects {" + string.Join(",", objects.Select(item => item.EditorGetName())) + "} to GameObjects and DateRef To ObjectDatas");
            foreach (var obj in objects)
            {
                Controller.AddObject(obj.EditorGetVisualObject());
                if (obj is BackgroundObject)
                {
                    GameGlobals.Map.BackgroundObjects.Add((PhysicalObject)obj);
                    AddObjectToTree(obj);
                }
                else
                {
                    GameGlobals.Map.GameObjects.Add((PhysicalObject)obj);
                    AddObjectToTree(obj);
                }
                AddResource(EditorResources.GetResourceIdentifierById(obj.ResourceId));

                comboBox1.Items.Add(obj);

            }
            MultiSelectObjects(objects);
        }

        public void ReviveDeletedRegion(Region region)
        {
            Log("=ReviveDeletedRegion==");
            GameGlobals.Map.Regions.Add(region);
            AddObjectToTree(region);
            Log("Add revived object to ComboBox");
            comboBox1.Items.Add(region);
            Log("Select revived object");
            comboBox1.SelectedItem = region;
        }

        public void DeleteRegion(Region region)
        {
            Log("=DeleteRegion=");
            Log("Removing: " + region.EditorGetName());
            Log("PropertyGrid - deselect region");
            propertyGrid1.SelectedObject = null;

            if (xnaWindow1.SelectedRegion == region)
            {
                Log("PropertyGrid - deselect region");
                comboBox1.SelectedItem = GameGlobals.Map;
                xnaWindow1.SelectedRegion = null;
            }

            Log("ComboBox remove");
            comboBox1.Items.Remove(region);

            Log("Removing region from Level");

            GameGlobals.Map.Regions.Remove(region);
            DeleteObjectFromTree(region, TreeNodes.Regions);
            region.Dispose();
            RefreshMenus();

        }

        public void BeginMultiPropertyChange(IEditorObject target, string[] propertyNames)
        {
            Log("Register multi property change start for properties (" + string.Join(",", propertyNames) + ") on target type '" + target.GetType() + "'");
            var propertyFieldTypes = propertyNames.Select(propertyName => target.GetType().GetProperty(propertyName)).ToList();
            var values = new Dictionary<string, object>();
            for (var i = 0; i < propertyFieldTypes.Count; i++)
            {
                values.Add(propertyNames[i], propertyFieldTypes[i].GetValue(target, null));
            }
            _tempAction = new Action(Action.ActionType.MultiPropertyChange){ Target = target, PropertyBeforeChange = values};
            Log("Register TempAction = '" + _tempAction + "'");
            Log("Disabling propertyChanged and proeprtyChanging events for object: '" + target.EditorGetName() + "'");
            target.PropertyChanging -= ObjectDataPropertyChanging;
            target.PropertyChanged -= ObjectDataPropertyChanged;
        }

        public void BeginMultiObjectsMultiPropertyChange(List<IEditorObject> targets, string[] propertyNames)
        {
            Log("Register multi property change start for properties { " + string.Join(",", propertyNames) +
                " } on targets { " + string.Join(",", targets.Select(item => item.EditorGetName())) + " }");
            var values = new Dictionary<IEditorObject, Dictionary<string, object>>();
            foreach (var target in targets)
            {
                var propertyFieldTypes = propertyNames.Select(propertyName => target.GetType().GetProperty(propertyName)).ToList();
                values.Add(target, new Dictionary<string, object>());
                for (var i = 0; i < propertyFieldTypes.Count; i++)
                {
                    values[target].Add(propertyNames[i], propertyFieldTypes[i].GetValue(target, null));
                }
            }
            
            _tempAction = new Action(Action.ActionType.MultipleObjectsMultiProperyChange) { Targets = targets.ToArray(), PropertyBeforeChange = values };
            Log("Register TempAction = '" + _tempAction + "'");
            
        }

        public void EndMultiPropertyChange()
        {
            var propertiesBeforeChange = ((Dictionary<string, object>)_tempAction.PropertyBeforeChange);
            Log("Register multi property change end for properties (" + string.Join(",", propertiesBeforeChange.Keys) + ") on target type '" + _tempAction.Target.GetType() + "'");
            var propertyFieldTypes =
                propertiesBeforeChange.Keys.Select(key => _tempAction.Target.GetType().GetProperty(key)).ToList();

            var values = new Dictionary<string, object>();
            for (int i = 0; i < propertyFieldTypes.Count; i++)
            {
                var val = propertyFieldTypes[i].GetValue(_tempAction.Target, null);
                Log("Property('" + propertiesBeforeChange.Keys.ToArray()[i] + "') changed from '" + propertiesBeforeChange.Values.ToArray()[i] + "' to '" + val + "'");
                values.Add(propertiesBeforeChange.Keys.ToArray()[i], val);
            }
            _tempAction.PropertyAfterChange = values;
            propertyGrid1.Refresh();
            AddActionToList(_tempAction);
            Log("Enabling propertyChanged and proeprtyChanging events for object: '" + _tempAction.Target.EditorGetName() + "'");
            _tempAction.Target.PropertyChanging += ObjectDataPropertyChanging;
            _tempAction.Target.PropertyChanged += ObjectDataPropertyChanged;
        }

        public void EndMultiObjectsMultiPropertyChange()
        {
            Log("Register multi property change end forobjects { " + string.Join(",",_tempAction.Targets.Select(item => item.EditorGetName()) + " }"));
            var values = new Dictionary<IEditorObject, Dictionary<string, object>>();
            var valuesBeforeChange =
                (Dictionary<IEditorObject, Dictionary<String, object>>) _tempAction.PropertyBeforeChange;
            foreach (var target in _tempAction.Targets)
            {
                var propertyNames = valuesBeforeChange[target].Keys.ToList();
                var propertyFieldTypes = propertyNames.Select(propertyName => target.GetType().GetProperty(propertyName)).ToList();
                values.Add(target, new Dictionary<string, object>());
                for (var i = 0; i < propertyFieldTypes.Count; i++)
                {
                    values[target].Add(propertyNames[i], propertyFieldTypes[i].GetValue(target, null));
                }
            }
            
            _tempAction.PropertyAfterChange = values;
            propertyGrid1.Refresh();
            AddActionToList(_tempAction);
        }

        public void BeginPropertyChange(IEditorObject target, string propertyName)
        {
            Log("Register property change start '" + propertyName + "' on target type '" + target.GetType() + "'");
            PropertyInfo propertyFieldType = target.GetType().GetProperty(propertyName);
            if (propertyFieldType.PropertyType.IsPointer)
                throw new Exception(propertyName + "(" + propertyFieldType.PropertyType + ") is pointer");
            _tempAction = new Action(Action.ActionType.PropertyChange) { Target = target, PropertyBeforeChange = propertyFieldType.GetValue(target,null), PropertyName = propertyName };
            Log("Register TempAction = '" + _tempAction + "'");
            Log("Disabling propertyChanged and proeprtyChanging events for object: '" + target.EditorGetName() + "'");
            target.PropertyChanging -= ObjectDataPropertyChanging;
            target.PropertyChanged -= ObjectDataPropertyChanged;
        }

        public void EndPorperyChange()
        {
            Log("Register property change end '" + _tempAction + "'");
            if (_tempAction.ExecutedAction == Action.ActionType.MultiPropertyChange)
            {
                Log("Multi property change - redirecting to EndMultiPropertyChange. End.");
                EndMultiPropertyChange();
                return;
            }
            var propertyFieldType = _tempAction.Target.GetType().GetProperty(_tempAction.PropertyName);
            var value = propertyFieldType.GetValue(_tempAction.Target, null);
            if (value.Equals(_tempAction.PropertyBeforeChange))
            {
                Log("No change detected.");
                Log("Enabling propertyChanged and propertyChanging events for object: '" +
                    _tempAction.Target.EditorGetName() + "'");
                _tempAction.Target.PropertyChanging += ObjectDataPropertyChanging;
                _tempAction.Target.PropertyChanged += ObjectDataPropertyChanged;
                Log("End.");
                _tempAction = null;
                return;
            }
            _tempAction.PropertyAfterChange = value;
            Log("Property('" + _tempAction.PropertyName + "') changed from '" + _tempAction.PropertyBeforeChange + "' to '" + _tempAction.PropertyAfterChange + "'");
            propertyGrid1.Refresh();
            AddActionToList(_tempAction);
            Log("Enabling propertyChanged and propertyChanging events for object: '" + _tempAction.Target.EditorGetName() + "'");
            _tempAction.Target.PropertyChanging += ObjectDataPropertyChanging;
            _tempAction.Target.PropertyChanged += ObjectDataPropertyChanged;
        }

        public void UndoPropertyChange(Action action)
        {
            Log("==UndoPropertyChange==");
            PropertyInfo propertyFieldType = action.Target.GetType().GetProperty(action.PropertyName);
            Log("Change property('" + action.PropertyName + "') from '" + action.PropertyAfterChange + "' to '" + action.PropertyBeforeChange + "'");
            propertyFieldType.SetValue(action.Target, action.PropertyBeforeChange, null);
            if (action.Target is PhysicalObject && action.PropertyName == "Name")
                RefreshObjectName((PhysicalObject)action.Target, (string)action.PropertyAfterChange);
            propertyGrid1.Refresh();
            xnaWindow1.RefreshSelection();
        }

        public void UndoMultiObjectsMultiPropertyChange(Action action)
        {
            Log("==UndoMultiObjectsMultiPropertyChange==");
            foreach (var target in action.Targets)
            {
                var propertyesBeforeChange =
                    ((Dictionary<IEditorObject, Dictionary<String, object>>) action.PropertyBeforeChange)[target];
                foreach (var property in propertyesBeforeChange)
                {
                    var propertyFieldType = target.GetType().GetProperty(property.Key);
                    propertyFieldType.SetValue(target, property.Value, null);
                    if (target is PhysicalObject && property.Key == "Name")
                        RefreshObjectName((PhysicalObject)target, (string)property.Value);
                }
                
            }
            xnaWindow1.RefreshSelection();
        }

        public void RedoPropertyChange(Action action)
        {
            Log("==RedoPropertyChange==");
            PropertyInfo propertyFieldType = action.Target.GetType().GetProperty(action.PropertyName);
            propertyFieldType.SetValue(action.Target, action.PropertyAfterChange, null);
            if (action.Target is PhysicalObject && action.PropertyName == "Name")
                RefreshObjectName((PhysicalObject)action.Target, (string)action.PropertyBeforeChange);
            propertyGrid1.Refresh();
            xnaWindow1.RefreshSelection();
        }

        public void RedoMultiObjectsMultiPropertyChange(Action action)
        {
            Log("==RedoMultiObjectsMultiPropertyChange==");
            foreach (var target in action.Targets)
            {
                var propertyesAfterChange =
                    ((Dictionary<IEditorObject, Dictionary<String, object>>)action.PropertyAfterChange)[target];
                foreach (var property in propertyesAfterChange)
                {
                    var propertyFieldType = target.GetType().GetProperty(property.Key);
                    propertyFieldType.SetValue(target, property.Value, null);
                    if (target is PhysicalObject && property.Key == "Name")
                        RefreshObjectName((PhysicalObject)target, (string)property.Value);
                }

            }
            xnaWindow1.RefreshSelection();
        }

        public void SelectItem(PhysicalObject data)
        {
            Log("==SelectItem==");
            if (xnaWindow1.SelectedObject == data)
            {
                Log("Object to select already selected - end");
                _passiveSelection = false;
                return;
            }
            if (xnaWindow1.SelectedObject != null)
            {
                Log("Deselect previosly selected object('" + xnaWindow1.SelectedObject.EditorGetName() + "')");
                Log("Disabling propertyChanged and proeprtyChanging events for object: '" + xnaWindow1.SelectedObject.EditorGetName() + "'");
                DeselectObject();
            }
            if (MultiSelectMode)
            {
                if (xnaWindow1.MultiSelectedObjects.Contains(data))
                    return;
                DeselectMultiSelectedObjects(data);
            }
            Log("Selecting new object('" + data.Name + "')");
            Log("Enabling propertyChanged and propertyChanging events for object: '" + data.Name + "'");
            data.PropertyChanged += ObjectDataPropertyChanged;
            data.PropertyChanging += ObjectDataPropertyChanging;
            xnaWindow1.SelectObject(data);
            menuStripCutButton.Enabled = true;
            menuStripCopyButton.Enabled = true;
            if (!_passiveSelection)
            {
                var point = new Point((int)(xnaWindow1.SelectedObject.EditorGetCenter().X - (xnaWindow1.Width / 2f / _zoom)), (int)(xnaWindow1.SelectedObject.EditorGetCenter().Y - (xnaWindow1.Height / 2f / _zoom)));
                if (point.X + xnaWindow1.Width / _zoom > EngineGlobals.Grid.WidthReal)
                    point.X = (int)(EngineGlobals.Grid.WidthReal - xnaWindow1.Width / _zoom);
                if (point.X < 0)
                    point.X = 0;
                if (point.Y + xnaWindow1.Height/_zoom > EngineGlobals.Grid.HeightReal)
                    point.Y = (int) (EngineGlobals.Grid.HeightReal - xnaWindow1.Height / _zoom);
                if (point.Y < 0)
                    point.Y = 0;
                hScrollBar1.Value = (int) (point.X / (float)EngineGlobals.Grid.CellWidth * _zoom);
                vScrollBar1.Value = (int) (point.Y / (float)EngineGlobals.Grid.CellHeight * _zoom);
            }
            else
                _passiveSelection = false;
            RefreshMenus();
        }

        private void ObjectDataPropertyChanging(object sender, PropertyChangingEventArgs propertyChangingEventArgs)
        {
            PropertyInfo propertyFieldType = sender.GetType().GetProperty(propertyChangingEventArgs.PropertyName);
            if (propertyFieldType.PropertyType.IsPointer)
                throw new Exception(propertyChangingEventArgs.PropertyName + "(" + propertyFieldType.PropertyType + ") is pointer");
            var target = RegionEditingMode?xnaWindow1.SelectedRegion : xnaWindow1.SelectedObject;
            //if (target is PhysicalObject && propertyChangingEventArgs.PropertyName == "Orientation")
            //{
            //    var values = new Dictionary<string, object>
            //        {
            //            {"Orientation", propertyFieldType.GetValue(target, null)},
            //            {"GridPos", ((PhysicalObject) target).GridPos}
            //        };
            //    _tempAction = new Action(Action.ActionType.MultiPropertyChange) { Target = target, PropertyBeforeChange = values, PropertyName = "Orientation" };
            //}
            //else
                _tempAction = new Action(Action.ActionType.PropertyChange)
                    {
                        Target = target,
                        PropertyBeforeChange = propertyFieldType.GetValue(target, null),
                        PropertyName = propertyChangingEventArgs.PropertyName
                    };
        }

        private void ObjectDataPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Log("==ObjectDataPropertyChanged==");
            PropertyInfo propertyFieldType = sender.GetType().GetProperty(_tempAction.PropertyName);
            var value = propertyFieldType.GetValue(_tempAction.Target, null);
            if (value.Equals(_tempAction.PropertyBeforeChange))
            {
                _tempAction = null;
                return;
            }
            //if (_tempAction.Target is PhysicalObject && _tempAction.PropertyName == "Orientation")
            //{
            //    var values = new Dictionary<string, object> { { "Orientation", value }, { "GridPos", ((PhysicalObject)_tempAction.Target).GridPos } };
            //    _tempAction.PropertyAfterChange = values;
            //}
            //else
                _tempAction.PropertyAfterChange = value;
            if (_tempAction.Target is PhysicalObject && _tempAction.PropertyName == "Name")
            {
                RefreshObjectName((PhysicalObject)_tempAction.Target, (string)_tempAction.PropertyBeforeChange);
            }

            AddActionToList(_tempAction);
            propertyGrid1.Refresh();
        }

        public void Copy()
        {
            Log("==Copy==");
            if (!MultiSelectMode && (comboBox1.SelectedItem == null || comboBox1.SelectedItem is Map))
            {
                Log("No item selected - end");
                return;
            }
            if (MultiSelectMode)
            {
                Log("Multiselection copy");
                Log("Cloning targets { " + string.Join(",", xnaWindow1.MultiSelectedObjects) + " }");
                _multiClipboardObjects = new Dictionary<IEditorObject, string>();
                foreach (PhysicalObject selectedObject in xnaWindow1.MultiSelectedObjects)
                {
                    _multiClipboardObjects.Add(((IEditorObject)selectedObject.Clone()), selectedObject.TypeId);
                }
                _multiObjectsCopy = true;
            }
            else
            {
                var target = (PhysicalObject)xnaWindow1.SelectedObject;
                Log("Selected target: '" + target.Name + "'");
                Log("Cloning target");
                var data = target.Clone();
                _clipboardObject = data;
                _clipboardObjectTypeName = target.TypeId;
                _multiObjectsCopy = false;
            }
            pasteToolStripMenuItem1.Enabled = true;
            menuStripPasteButton.Enabled = true;
            toolStripPasteButton.Enabled = true;
        }

        public void Cut()
        {
            Log("==Cut==");
            Copy();
            Log("Deleting selected object");
            DeleteSelectedObject();
        }

        public void MultiObjectsPaste(bool global)
        {
            Log("==MultiObjectsPaste==");
            var relPos = global ? new Point(0, 0) : xnaWindow1.MousePosReal;
            var minDest = -1f;
            IEditorObject relObject = null;
            var newObjects = _multiClipboardObjects.ToDictionary(item => (IEditorObject)item.Key.Clone(), item => item.Value);
            foreach (var obj in newObjects.Keys)
            {
                if (Vector2.Distance(new Vector2(obj.Rectangle.X, obj.Rectangle.Y), relPos.ToVector()) < minDest || minDest < 0)
                {
                    minDest = Vector2.Distance(new Vector2(obj.Rectangle.X, obj.Rectangle.Y), relPos.ToVector());
                    relObject = obj;
                }
            }
            var baseRelPos = new Vector2(relObject.Rectangle.X, relObject.Rectangle.Y);
            foreach (var obj in newObjects.Keys)
            {
                var newPos = obj == relObject
                             ? relPos.ToVector()
                             : new Vector2(relPos.X + (obj.Rectangle.X - baseRelPos.X),
                                           relPos.Y + (obj.Rectangle.Y - baseRelPos.Y));
                if (newPos.X + obj.Rectangle.Width > EngineGlobals.Grid.WidthReal)
                    newPos.X = EngineGlobals.Grid.WidthReal - obj.Rectangle.Width;
                else if (newPos.X < 0)
                    newPos.X = 0;
                if (newPos.Y + obj.Rectangle.Height > EngineGlobals.Grid.HeightReal)
                    newPos.Y = EngineGlobals.Grid.HeightReal - obj.Rectangle.Height;
                else if (newPos.Y < 0)
                    newPos.Y = 0;
                obj.Rectangle = new RectangleF(newPos.X, newPos.Y, obj.Rectangle.Width, obj.Rectangle.Height);
                while (true)
                {
                    if (_nameCounter.ContainsKey(newObjects[obj]))
                        _nameCounter[newObjects[obj]]++;
                    else
                        _nameCounter.Add(newObjects[obj], 1);
                    var found = GameGlobals.Map.GameObjects.Any(objectData => objectData.Name == newObjects[obj] + _nameCounter[newObjects[obj]]);
                    if (!found)
                        break;
                }
                var physicalObject = obj as PhysicalObject;
                if (physicalObject != null)
                {
                    physicalObject.Name = newObjects[obj] + _nameCounter[newObjects[obj]];
                    Log("New object name: '" + physicalObject.Name + "'");
                    Log("Addeding new object to map");
                    if (physicalObject is BackgroundObject)
                    {
                        Controller.AddBackgroundObject(physicalObject);
                        GameGlobals.Map.BackgroundObjects.Add(physicalObject);
                    }
                    else
                    {
                        Controller.AddGameObject(physicalObject);
                        GameGlobals.Map.GameObjects.Add(physicalObject);
                    }
                    Log("Addeding object to ComboBox");
                    comboBox1.Items.Add(physicalObject);
                    AddObjectToTree(physicalObject);
                }
                AddResource(EditorResources.GetResourceIdentifierById(obj.ResourceId));
                
            }
            Log("Addeding objects creation to action list");
            AddActionToList(new Action(Action.ActionType.CreateMultipleObjects) { Targets = newObjects.Keys.ToArray() });
            MultiSelectObjects(newObjects.Keys.ToList());
        }

        public void Paste(bool global)
        {
            Log("==Paste==");
            if (_multiObjectsCopy)
            {
                MultiObjectsPaste(global);
                return;
            }
            var obj = (IEditorObject)((ICloneable)(_clipboardObject)).Clone();
            Log("Object in clipboard: '" + obj.EditorGetName() + "'");
            Log("Creating Virtual Object from data");
            var rect = EngineGlobals.Grid.ToCellRectangle(obj.Rectangle.GetRectangle());
            if (!global)
            {
                var x = xnaWindow1.MousePosCell.X - rect.Width / 2;
                if (x + rect.Width > EngineGlobals.Grid.GridRectangle.Width)
                    x = EngineGlobals.Grid.GridRectangle.Width - rect.Width;
                else if (x < 0)
                    x = 0;
                var y = xnaWindow1.MousePosCell.Y - rect.Height / 2;
                if (y + rect.Height > EngineGlobals.Grid.GridRectangle.Height)
                    y = EngineGlobals.Grid.GridRectangle.Height - rect.Height;
                else if (y < 0)
                    y = 0;
                obj.Rectangle = new RectangleF(EngineGlobals.Grid.ToRealRectangle(new Rectangle(x, y, rect.Width, rect.Height)));
                Log("Creating object at cursor(" + obj.Rectangle.GetRectangle() + ")");
            }
            else
            {
                obj.Rectangle = new RectangleF(new Rectangle((int)EngineGlobals.Camera2D.CornerPos.X, (int)EngineGlobals.Camera2D.CornerPos.Y, (int)obj.Rectangle.Width, (int)obj.Rectangle.Height));
                Log("Creating object at camera position(" +obj.Rectangle.GetRectangle() + ")");
            }
            while (true)
            {
                if (_nameCounter.ContainsKey(_clipboardObjectTypeName))
                    _nameCounter[_clipboardObjectTypeName]++;
                else
                    _nameCounter.Add(_clipboardObjectTypeName, 1);
                var found = GameGlobals.Map.GameObjects.Any(objectData => objectData.Name == _clipboardObjectTypeName.ToString() + _nameCounter[_clipboardObjectTypeName.ToString()]);
                if (!found)
                    break;
            }
            var physicalObject = obj as PhysicalObject;
            if (physicalObject != null)
            {
                physicalObject.Name = _clipboardObjectTypeName + _nameCounter[_clipboardObjectTypeName];
                Log("New object name: '" + physicalObject.Name + "'");
                Log("Addeding new object to map");
                if (physicalObject is BackgroundObject)
                {
                    Controller.AddBackgroundObject(physicalObject);
                    GameGlobals.Map.BackgroundObjects.Add(physicalObject);
                }
                else
                {
                    Controller.AddGameObject(physicalObject);
                    GameGlobals.Map.GameObjects.Add(physicalObject);
                }
                Log("Addeding object to ComboBox");
                comboBox1.Items.Add(physicalObject);
                AddObjectToTree(physicalObject);
                physicalObject.EditorPasteObject();
                SelectPassive(physicalObject);
            }
            AddResource(EditorResources.GetResourceIdentifierById(obj.ResourceId));
            
            Log("Addeding object creation to action list");
            AddActionToList(new Action(Action.ActionType.CreateObject) { Target = obj });
        }

        public void MultiSelectObjects(List<IEditorObject> objects)
        {
            xnaWindow1.MultiSelectObjects(objects);
            MultiSelectMode = true;
            comboBox1.Items.Add(_multiselectionObject);
            comboBox1.SelectedItem = _multiselectionObject;
            propertyGrid1.SelectedObject = null;
            propertyGrid1.Enabled = false;
            RefreshMenus();
        }

        public void MultiSelectObjects(Rectangle selection)
        {
            var selectedObjects = GameGlobals.Map.GameObjects.Where(item => selection.Contains(item.Rectangle.GetRectangle()) ||
                selection.Intersects(item.Rectangle.GetRectangle())).Cast<IEditorObject>().ToList();
            if (selectedObjects.Count < 1)
                return;
            if (selectedObjects.Count == 1)
            {
                SelectPassive((PhysicalObject)selectedObjects[0]);
                return;
            }
            MultiSelectObjects(selectedObjects);
        }

        public void DeselectMultiSelectedObjects(object newSelection)
        {
            Log("==MultiselectMode==");
            if (!MultiSelectMode)
            {
                Log("MutliSelectMode disabled - end");
                return;
            }
            xnaWindow1.DeselectMultiSelectedObjects();
            MultiSelectMode = false;
            comboBox1.SelectedItem = newSelection;
            comboBox1.Items.Remove(_multiselectionObject);
            propertyGrid1.Enabled = true;
        }

        #endregion

        public static void Log(string message, bool info = true)
        {
            if (info)
            {
                var frame = (new StackTrace(true)).GetFrame(1);
                Listener.WriteLine(Path.GetFileNameWithoutExtension(frame.GetFileName()) + "." + frame.GetMethod().Name + "[" + frame.GetFileLineNumber() + "]: " + message);
            }
            else
            {
                Listener.WriteLine(message);
            }
            Listener.Flush();
        }

        public static void Logl(string message, bool info = true)
        {
            if (info)
            {
                var frame = (new StackTrace(true)).GetFrame(1);
                Listener.Write(Path.GetFileNameWithoutExtension(frame.GetFileName()) + "." + frame.GetMethod().Name + "[" + frame.GetFileLineNumber() + "]: " + message);
            }
            else
            {
                Listener.Write(message);
            }
            Listener.Flush();
        }

        public void SetToBounds(PhysicalObject obj)
        {
            //var x = xnaWindow1.MousePosReal.X - obj.HalfSize.X;
            //if (x + obj.Rectangle.Width > EngineGlobals.Grid.WidthReal)
            //    x = EngineGlobals.Grid.WidthReal - _tempObjectPreviewImg.Width;
            //else if (x < 0)
            //    x = 0;
            //var y = MousePosReal.Y - _tempObjectPreviewImg.Height / 2;
            //if (y + _tempObjectPreviewImg.Height > EngineGlobals.Grid.HeightReal)
            //    y = EngineGlobals.Grid.HeightReal - _tempObjectPreviewImg.Height;
            //else if (y < 0)
            //    y = 0;
            //TempObjectRect =
            //    EngineGlobals.Grid.ToCellRectangle(new Rectangle(x, y, _tempObjectPreviewImg.Width, _tempObjectPreviewImg.Height));
            //_tempObjectPreviewImg.Rect = EngineGlobals.Grid.ToRealRectangle(TempObjectRect);
        }

        private void MenuStripimportBackgroundButtonClick(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                GameGlobals.Map.ImportBackground(openFileDialog1.FileName);
                treeView1.BeginUpdate();
                treeView1.Nodes[TreeNodes.BackgroundObjects.ToString()].Nodes.Clear();
                foreach (var backgroundObject in GameGlobals.Map.BackgroundObjects)
                {
                    AddObjectToTree(backgroundObject);
                }
                RefreshTreeRoot(TreeNodes.BackgroundObjects);
                treeView1.EndUpdate();
            }
        }

        private void RotateToolStripMenuItemClick(object sender, EventArgs e)
        {
            _rotating = true;
            BeginPropertyChange(xnaWindow1.SelectedObject, "Rotation");
            Log("Startig rotate");
        }

        private void SpriteEditorToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_spriteEditor == null)
            {
                _spriteEditor = new SpriteEditor();
                _spriteEditor.Closed += (o, args) => _spriteEditor = null;
                _spriteEditor.Show();
                return;
            }
            _spriteEditor.WindowState = FormWindowState.Normal;
        }

        private void FlipVerticallyToolStripMenuItemClick(object sender, EventArgs e)
        {
            var selection = ((PhysicalObject) xnaWindow1.SelectedObject);
            selection.Flip = selection.Flip == SpriteEffects.FlipVertically ? SpriteEffects.None : SpriteEffects.FlipVertically;
        }

        private void FlipHorizontallyToolStripMenuItemClick(object sender, EventArgs e)
        {
            var selection = ((PhysicalObject)xnaWindow1.SelectedObject);
            selection.Flip = selection.Flip == SpriteEffects.FlipHorizontally ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void ToolStripRegionEditorButtonCheckedChanged(object sender, EventArgs e)
        {
            if (toolStripRegionEditorButton.Checked)
            {
                DeselectObject();
                RegionEditingMode = true;
                runToolStripMenuItem.Enabled = false;
                toolStripRunButton.Enabled = false;
                toolbox1.Enabled = false;
                _preselectedObject = null;
                comboBox1.SelectedItem = (object)_preselectedRegion ?? GameGlobals.Map;
            }
            else
            {
                runToolStripMenuItem.Enabled = true;
                toolStripRunButton.Enabled = true;
                RegionEditingMode = false;
                toolbox1.Enabled = true;
                _preselectedRegion = null;
                comboBox1.SelectedItem = (object) _preselectedObject ?? GameGlobals.Map;
            }
            RefreshMenus();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ActionIndex != 0 && !_saved)
                SaveTemporary();
        }

        private void toolStripRecoverButton_Click(object sender, EventArgs e)
        {
            var fileToRecover = "";
            if (!string.IsNullOrEmpty(_temporaryFileName))
            {
                if (_temporaryFiles[0] == _temporaryFileName && _temporaryFiles.Length < 2)
                {
                    fileToRecover = _temporaryFiles[1];
                }
                else if (_temporaryFiles[0] == _temporaryFileName)
                {
                    fileToRecover = _temporaryFiles[1];
                }
                else
                {
                    fileToRecover = _temporaryFiles[0];
                }
            }
            else
            {
                fileToRecover = _temporaryFiles[0];
            }
            if (fileToRecover != _temporaryFileName)
            {
                if (!_saved && ActionIndex != 0)
                {
                    var result = MessageBox.Show("File is not saved, do You want to save it now?", "Save level?",
                                                 MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Cancel)
                        return;
                    if (result == DialogResult.Yes)
                        Save(false);
                    else
                        SaveTemporary();
                }
            }
            LoadTemporaryFile(fileToRecover);

        }

    }

    public static class EditorFormHelper
    {
        public static void ChangeDescriptionHeight(this PropertyGrid grid, int widthPercentage)
        {

            var gv = grid.GetType().GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(grid);
            var type = gv.GetType();
            var methods = gv.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

            var mtf = gv.GetType().GetMethod(@"MoveSplitterTo",
                BindingFlags.NonPublic |
                BindingFlags.Instance);
            mtf.Invoke(gv, new object[] { (int)(grid.Width*(widthPercentage/100.0)) });
        }
    }

    public class MultiSelectionObject
    {
        public override string ToString()
        {
            return "Multiselection";
        }
    }
}
