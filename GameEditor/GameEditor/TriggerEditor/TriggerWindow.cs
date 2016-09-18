using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Engine.Core;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using Engine.Mechanics.Triggers.Actions;
using Engine.Mechanics.Triggers.Conditions;
using GameEditor.TriggerEditor.Selectors;
using GameLibrary.Triggers;
using Microsoft.Xna.Framework;

namespace GameEditor.TriggerEditor
{
    public partial class TriggerWindow : UserControl
    {

        public delegate void TriggerEventAddedEventHandler(object sender, TriggerEventActionEventArgs e);
        public delegate void TriggerEventRemovedEventHandler(object sender, TriggerEventActionEventArgs e);
        public delegate void TriggerEventChangedEventHandler(object sender, TriggerEventChangedEventArgs e);
        public delegate void TriggerEventPositionChangedEventHandler(
            object sender, TriggerEventPositionChangedEventArgs e);

        public delegate void TriggerItemEventHandler(object sender, TriggerItemEventArgs e);

        public delegate void TriggerConditionRemovedEventHandler(object sender, TriggerItemEventArgs e);

        public delegate void TriggerItemChangedEventHandler(object sender, TriggerItemChangeEventArgs e);

        public delegate void TriggerItemParameterChangedEventHandler(object sender, TriggerItemParameterEventArgs e);

        public delegate void TriggerItemPositionChangedEventHandler(object sender, TriggerItemPositionEventArgs e);


        public event TriggerEventAddedEventHandler TriggerEventAdded;
        public event TriggerEventRemovedEventHandler TriggerEventRemoved;
        public event TriggerEventChangedEventHandler TriggerEventChanged;
        public event TriggerEventPositionChangedEventHandler TriggerEventPositionChanged;
        public event TriggerItemEventHandler TriggerItemAdded;
        public event TriggerItemEventHandler TriggerItemRemoved;
        public event TriggerItemChangedEventHandler TriggerItemChanged;
        public event TriggerItemParameterChangedEventHandler TriggerItemParameterChanged;
        public event TriggerItemPositionChangedEventHandler TriggerItemPositionChanged;

        public enum RootNode
        {
            Events, Conditions, Actions
        }

        public Trigger SelectedTrigger { protected set; get; }

        public TriggerWindow()
        {
            InitializeComponent();
            triggerTreeView.Enabled = false;
        }

        #region UI

        private void TriggerTreeViewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var node = triggerTreeView.GetNodeAt(e.Location);
                if (node == null)
                {
                    return;
                }

                triggerTreeView.SelectedNode = node;
                if (node.Name == RootNode.Events.ToString() || node.Name == RootNode.Conditions.ToString() || node.Name == RootNode.Actions.ToString())
                {
                    rootMenuStrip.Show(triggerTreeView, e.Location);
                }
                else if (node.Level == 1)
                {
                    itemsMenuStrip.Show(triggerTreeView, e.Location);
                }
            }
        }

        private void RootMenuStripOpening(object sender, CancelEventArgs e)
        {
            if (triggerTreeView.SelectedNode == triggerTreeView.Nodes[RootNode.Events.ToString()])
            {
                addEventToolStripMenuItem.Enabled = true;
                addConditionToolStripMenuItem.Enabled = false;
                addActionToolStripMenuItem.Enabled = false;
            }
            else if (triggerTreeView.SelectedNode == triggerTreeView.Nodes[RootNode.Conditions.ToString()])
            {
                addEventToolStripMenuItem.Enabled = false;
                addConditionToolStripMenuItem.Enabled = true;
                addActionToolStripMenuItem.Enabled = false;
            }
            else if (triggerTreeView.SelectedNode == triggerTreeView.Nodes[RootNode.Actions.ToString()])
            {
                addEventToolStripMenuItem.Enabled = false;
                addConditionToolStripMenuItem.Enabled = false;
                addActionToolStripMenuItem.Enabled = true;
            }
        }

        private void TriggerTreeViewDoubleClick(object sender, EventArgs e)
        {
            var location = triggerTreeView.PointToClient(MousePosition);
            var node = triggerTreeView.GetNodeAt(location);
            triggerTreeView.SelectedNode = node;
            if (node.Level != 2)
                return;
            EditItem(node);
        }

        private void ItemsMenuStripOpening(object sender, CancelEventArgs e)
        {
            moveUpToolStripMenuItem.Enabled = triggerTreeView.SelectedNode.Index >= 1;
            moveDownToolStripMenuItem.Enabled = triggerTreeView.SelectedNode.Index <
                                                triggerTreeView.SelectedNode.Parent.Nodes.Count - 1;
            editStripMenuItem.Enabled = triggerTreeView.SelectedNode.Tag is int;

        }

        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            DeleteItem(triggerTreeView.SelectedNode);
        }

        private void EditStripMenuItemClick(object sender, EventArgs e)
        {
            if (triggerTreeView.SelectedNode.Parent == triggerTreeView.Nodes[RootNode.Events.ToString()])
                EditEvent((int)triggerTreeView.SelectedNode.Tag);
            else if (triggerTreeView.SelectedNode.Parent == triggerTreeView.Nodes[RootNode.Conditions.ToString()] || triggerTreeView.SelectedNode.Parent == triggerTreeView.Nodes[RootNode.Actions.ToString()])
                EditItem(triggerTreeView.SelectedNode);
        }

        private void MoveUpToolStripMenuItemClick(object sender, EventArgs e)
        {
            var index = triggerTreeView.SelectedNode.Index - 1;
            var parent = triggerTreeView.SelectedNode.Parent;
            var temp = triggerTreeView.SelectedNode;
            parent.Nodes.Remove(temp);
            parent.Nodes.Insert(index, temp);
            triggerTreeView.SelectedNode = temp;
            if (temp.Tag is ITriggerItem)
            {
                if (TriggerItemPositionChanged != null)
                    TriggerItemPositionChanged(this,
                                               new TriggerItemPositionEventArgs(SelectedTrigger.Name,
                                                                                (ITriggerItem) temp.Tag, index));
            }
            else
            {
                if (TriggerEventPositionChanged != null)
                    TriggerEventPositionChanged(this,
                                                new TriggerEventPositionChangedEventArgs(SelectedTrigger.Name,
                                                                                         (int) temp.Tag, index));
            }
        }

        private void MoveDownToolStripMenuItemClick(object sender, EventArgs e)
        {
            var index = triggerTreeView.SelectedNode.Index + 1;
            var parent = triggerTreeView.SelectedNode.Parent;
            var temp = triggerTreeView.SelectedNode;
            parent.Nodes.Remove(temp);
            parent.Nodes.Insert(index, temp);
            triggerTreeView.SelectedNode = temp;
            if (temp.Tag is ITriggerItem)
            {
                if (TriggerItemPositionChanged != null)
                    TriggerItemPositionChanged(this, new TriggerItemPositionEventArgs(SelectedTrigger.Name, (ITriggerItem)temp.Tag, index));
            }
            else
            {
                if (TriggerEventPositionChanged != null)
                    TriggerEventPositionChanged(this, new TriggerEventPositionChangedEventArgs(SelectedTrigger.Name, (int)temp.Tag, index));
            }
            
        }

        #endregion

        #region Utility

        public void DeselectTrigger(bool editing = false)
        {
            SelectedTrigger = null;
            if (!editing)
                triggerTreeView.BeginUpdate();
            triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Clear();
            triggerTreeView.Nodes[RootNode.Conditions.ToString()].Nodes.Clear();
            triggerTreeView.Nodes[RootNode.Actions.ToString()].Nodes.Clear();
            if (!editing)
                triggerTreeView.EndUpdate();
            triggerTreeView.Enabled = false;
        }

        public void SelectTrigger(Trigger trigger)
        {
            triggerTreeView.BeginUpdate();
            DeselectTrigger(true);
            SelectedTrigger = trigger;
            foreach (var e in trigger.Events)
            {
                var node = triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Add(TriggerController.EventsStrings[e]);
                node.ImageKey = TriggerController.EventImageKeys[e];
                node.SelectedImageKey = TriggerController.EventImageKeys[e];
                node.Tag = e;
            }
            foreach (var c in trigger.Conditions)
            {
                AddItemInner(c);
            }
            foreach (var action in trigger.Actions)
            {
                AddItemInner(action);
            }
            triggerTreeView.EndUpdate();
            triggerTreeView.Enabled = true;
        }



        private void AddParameterNodes(TreeNode node, int[] parameterTypes, object[] values)
        {
            TreeNode n = null;
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                var p = parameterTypes[i];
                switch (p)
                {
                    case (int)ParameterType.Region:
                        n = node.Nodes.Add("(" + (i+1).ToString("00") + "):" + (string.IsNullOrEmpty((string)values[i])? "NULL" : (string)values[i]));
                        break;
                    case (int)ParameterType.Bool:
                        n = node.Nodes.Add("(" + (i+1).ToString("00") + "):" + (bool)values[i]);
                        break;
                    case (int)ParameterType.Float:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + ((float) values[i]).ToString("0.00"));
                        break;
                    case (int)ParameterType.Int:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + ((int)values[i]));
                        break;
                    case (int)ParameterType.PhysicalObject:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + (string.IsNullOrEmpty((string)values[i]) ? "NULL" : (string)values[i]));
                        break;
                    case (int)ParameterType.Point:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + (((Point?)values[i]) == null ? "NULL" : ((Point)values[i]).ToFormatedString()));
                        break;
                    case (int)ParameterType.Trigger:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + (string.IsNullOrEmpty((string)values[i]) ? "NULL" : values[i].ToString()));
                        break;
                    case (int)GameParameterType.NativeFunction:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + values[i]);
                        break;
                    case (int)GameParameterType.NativeParameterBool:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + values[i]);
                        break;
                    case (int)ParameterType.String:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + "'" + values[i] + "'");
                        break;
                    case (int)ParameterType.MenuObject:
                        n = node.Nodes.Add("(" + (i + 1).ToString("00") + "):" + (string.IsNullOrEmpty((string)values[i]) ? "NULL" : values[i].ToString()));
                        break;
                    case (int)ParameterType.CameraPath:
                        n =
                            node.Nodes.Add("(" + (i + 1).ToString("00") + "):" +
                                           (string.IsNullOrEmpty((string) values[i]) ? "NULL" : values[i].ToString()));
                        break;
                    default:
                        throw new Exception("Undefined parameter type: '" + p + "'");
                }
                n.SelectedImageKey = TriggerController.ParameterGetImageKey(parameterTypes[i]);
                n.ImageKey = TriggerController.ParameterGetImageKey(parameterTypes[i]);
                n.Tag = parameterTypes[i];
                
            }
            node.Expand();
        }

        private string GetRoot(TreeNode node)
        {
            if (node.Level == 2)
            {
                return node.Parent.Parent.Name;
            }
            return node.Parent.Name;
        }

        private string GetRoot(ITriggerItem item)
        {
            if (item is ICondition)
                return RootNode.Conditions.ToString();
            else if (item is ITriggerAction)
                return RootNode.Actions.ToString();
            else
            {
                throw new Exception("Item type not defined: " + item);
            }
        }

        public void SetItemParameterValueForced(ITriggerItem item, int parameterIndex, object value)
        {
            var itemNode = triggerTreeView.Nodes[GetRoot(item)].Nodes.Cast<TreeNode>().First(n => n.Tag == item);
            var paramNode = itemNode.Nodes[parameterIndex];
            SetItemParameterValue(paramNode, (int)paramNode.Tag, parameterIndex, value);
            itemNode.Text = item.ToString();
        }

        #endregion

        #region EventManagement

        public void AddEvent(int eventId)
        {
            var node = triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Add(TriggerController.EventsStrings[eventId]);
            node.ImageKey = TriggerController.EventImageKeys[eventId];
            node.SelectedImageKey = TriggerController.EventImageKeys[eventId];
            node.Tag = eventId;
            triggerTreeView.Nodes[RootNode.Events.ToString()].Expand();
            if (TriggerEventAdded != null)
                TriggerEventAdded(this, new TriggerEventActionEventArgs(SelectedTrigger.Name, eventId));
        }

        private void AddEventToolStripMenuItemClick(object sender, EventArgs e)
        {
            var addEventDialog = new EventSelector();
            if (addEventDialog.ShowDialog() == DialogResult.OK)
            {
                if (triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Cast<TreeNode>().Any(n => (int)n.Tag == addEventDialog.Result))
                {
                    MessageBox.Show("Cannot add event '" + TriggerController.EventsStrings[addEventDialog.Result] +
                                    "' - trigger already registered.");
                    return;
                }
                AddEvent(addEventDialog.Result);
            }
        }

        private void EditEvent(int eventId)
        {
            var node =
                triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Cast<TreeNode>().First(
                    n => (int)n.Tag == eventId);
            var addEventDialog = new EventSelector();
            addEventDialog.SetResult(eventId);
            if (addEventDialog.ShowDialog() == DialogResult.OK)
            {
                if ((int)node.Tag == addEventDialog.Result)
                    return;
                if (triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Cast<TreeNode>().Any(n => (int)n.Tag == addEventDialog.Result))
                {
                    MessageBox.Show("Cannot add event '" + TriggerController.EventsStrings[addEventDialog.Result] +
                                    "' - trigger already registered.");
                    return;
                }
                node.Tag = addEventDialog.Result;
                node.Text = TriggerController.EventsStrings[addEventDialog.Result];
                node.ImageKey = TriggerController.EventImageKeys[addEventDialog.Result];
                node.SelectedImageKey = TriggerController.EventImageKeys[addEventDialog.Result];
                if (TriggerEventChanged != null)
                    TriggerEventChanged(this, new TriggerEventChangedEventArgs(SelectedTrigger.Name, eventId, addEventDialog.Result));

            }
        }

        public void DeleteEvent(int eventId)
        {
            var node =
                triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Cast<TreeNode>().First(
                    n => (int)n.Tag == eventId);
            triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Remove(node);
            if (TriggerEventRemoved != null)
                TriggerEventRemoved(this, new TriggerEventActionEventArgs(SelectedTrigger.Name, eventId));
        }

        public void TriggerEventChangePositionForced(int eventId, int newPos)
        {
            var item = triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Cast<TreeNode>().First(
                node => (int)node.Tag == eventId);
            triggerTreeView.BeginUpdate();
            triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Remove(item);
            triggerTreeView.Nodes[RootNode.Events.ToString()].Nodes.Insert(newPos, item);
            triggerTreeView.EndUpdate();
            triggerTreeView.SelectedNode = item;
        }

        public void TriggerActionChangePositionForced(ITriggerAction action, int newPos)
        {
            var item = triggerTreeView.Nodes[RootNode.Actions.ToString()].Nodes.Cast<TreeNode>().First(
                node => node.Tag == action);
            triggerTreeView.BeginUpdate();
            triggerTreeView.Nodes[RootNode.Actions.ToString()].Nodes.Remove(item);
            triggerTreeView.Nodes[RootNode.Actions.ToString()].Nodes.Insert(newPos, item);
            triggerTreeView.EndUpdate();
            triggerTreeView.SelectedNode = item;
        }

        public void TriggerConditionChangePositionForced(ICondition condition, int newPos)
        {
            var item = triggerTreeView.Nodes[RootNode.Conditions.ToString()].Nodes.Cast<TreeNode>().First(
                node => node.Tag == condition);
            triggerTreeView.BeginUpdate();
            triggerTreeView.Nodes[RootNode.Conditions.ToString()].Nodes.Remove(item);
            triggerTreeView.Nodes[RootNode.Conditions.ToString()].Nodes.Insert(newPos, item);
            triggerTreeView.EndUpdate();
            triggerTreeView.SelectedNode = item;
        }

        #endregion

        #region ConditionManagement

        private ICondition UserGetCondition(int preset = 0)
        {
            var addConditionDialog = new ConditionSelector();
            if (addConditionDialog.ShowDialog() == DialogResult.OK)
            {
                switch (addConditionDialog.Result)
                {
                    case (int)ConditionType.TriggeringRegion:
                        return new TriggeringRegion();
                    case (int)ConditionType.TriggeringObject:
                        return new TriggeringObject();
                    case (int)GameConditionType.GetNativeParameterBool:
                        return new GetNativeParameterBool();
                    case (int)ConditionType.CameraIsMoving:
                        return new CameraIsMoving();
                    default:
                        throw new Exception("Undefined condition type '" + addConditionDialog.Result + "'");
                }
            }
            return null;
        }

        private void AddConditionToolStripMenuItemClick(object sender, EventArgs e)
        {
            var condition = UserGetCondition();
            if (condition != null)
                AddItem(condition);
        }

        #endregion

        #region ActionManagement

        private ITriggerAction UserGetAction(int preset = 0)
        {
            var addActionDialog = new ActionSelector();
            if (addActionDialog.ShowDialog() == DialogResult.OK)
            {
                switch (addActionDialog.Result)
                {
                    case (int)ActionType.SetCameraZoom:
                        return new ZoomCamera();
                    case (int)ActionType.ActivateObject:
                        return new ActivateObject();
                    case (int)ActionType.MoveCamera:
                        return new MoveCamera();
                    case (int)ActionType.SetCameraPosition:
                        return new SetCameraPosition();
                    case (int)ActionType.DisableTrigger:
                        return new DisableTrigger();
                    case (int)GameActionType.ExecuteNative:
                        return new ExecuteNative();
                    case (int)GameActionType.SetButtonText:
                        return new SetButtonText();
                    case (int)GameActionType.SetCameraPath:
                        return new SetCameraPath();
                    case (int)GameActionType.PlayClickSound:
                        return new PlayClickSound();
                    case (int)ActionType.ExitGame:
                        return new ExitGame();
                    case (int)GameActionType.SetMusicText:
                        return new SetMusicText();
                    case (int)GameActionType.StartBtnAnimation:
                        return new StartBtnAnimation();
                    case (int)GameActionType.ReturnMovingCircle:
                        return new MovingCircleReturnAndStop();
                    case (int)GameActionType.RestartMovingCircle:
                        return new MovingCircleRestart();
                    case (int)ActionType.SetGravity:
                        return new SetGravity();
                    case (int)ActionType.RotateCamera:
                        return new RotateCamera();
                    case (int)GameActionType.SetDeathBallState:
                        return new ChangeDeathBallState();
                    case (int)GameActionType.SetCameraBoundsTopRight:
                        return new SetCameraBoundsTopRight();
                    default:
                       throw new Exception("Undefined action type '" + addActionDialog.Result + "'");
                }
            }
            return null;
        }

        private void AddActionToolStripMenuItemClick(object sender, EventArgs e)
        {
            var action = UserGetAction();
            if (action != null)
                AddItem(action);
        }

        #endregion

        #region ItemManagement

        private TreeNode CreateItemNode(ITriggerItem item)
        {
            var node = new TreeNode(item.ToString());
            if (item is ICondition)
            {
                node.ImageKey = TriggerController.ConditionImageKeys[item.TypeId];
                node.SelectedImageKey = TriggerController.ConditionImageKeys[item.TypeId];
            }
            else if (item is ITriggerAction)
            {
                node.ImageKey = TriggerController.ActionImageKeys[item.TypeId];
                node.SelectedImageKey = TriggerController.ActionImageKeys[item.TypeId];
            }
            else
            {
                throw new Exception("Undefined item type: " + item.GetType());
            }
            node.Tag = item;
            AddParameterNodes(node, item.EditorGetParametersTypes(), item.EditorGetPatametersValues());
            return node;
        }

        private void AddItemInner(ITriggerItem item)
        {
            var node = CreateItemNode(item);
            var rootNode = item is ITriggerAction
                               ? triggerTreeView.Nodes[RootNode.Actions.ToString()]
                               : triggerTreeView.Nodes[RootNode.Conditions.ToString()];
            rootNode.Nodes.Add(node);
            rootNode.Expand();
            node.Expand();
        }

        public void AddItem(ITriggerItem item)
        {
            AddItemInner(item);
            if (TriggerItemAdded != null)
                TriggerItemAdded(this, new TriggerItemEventArgs(SelectedTrigger.Name, item));
        }

        public void DeleteItem(ITriggerItem item)
        {
            var nodeToDelete =
                triggerTreeView.Nodes[GetRoot(item)].Nodes.Cast<TreeNode>().First(node => node.Tag == item);
            triggerTreeView.Nodes[GetRoot(item)].Nodes.Remove(nodeToDelete);
            if (TriggerItemRemoved != null)
                TriggerItemRemoved(this, new TriggerItemEventArgs(SelectedTrigger.Name, item));
        }

        public void EditTriggerItem(ITriggerItem item)
        {
            var newItem = item is ITriggerAction? (ITriggerItem)UserGetAction(item.TypeId) : UserGetCondition(item.TypeId);
            if (newItem == null || newItem.GetType() == item.GetType())
                return;
            var newNode = CreateItemNode(newItem);
            var currentNodeIndex = triggerTreeView.Nodes[GetRoot(item)].Nodes.Cast<TreeNode>().First(
                node => node.Tag == item).Index;
            triggerTreeView.Nodes[GetRoot(item)].Nodes[currentNodeIndex] = newNode;
            if (TriggerItemChanged != null)
                TriggerItemChanged(this, new TriggerItemChangeEventArgs(SelectedTrigger.Name, item, newItem));
        }

        private void DeleteItem(TreeNode node)
        {
            if (node.Parent.Text == RootNode.Events.ToString())
                DeleteEvent((int)node.Tag);
            else if (node.Parent.Text == RootNode.Conditions.ToString() || node.Parent.Text == RootNode.Actions.ToString())
                DeleteItem((ITriggerItem)node.Tag);
        }

        private void SetItemParameterValue(TreeNode node, int parameterType, int parameterIndex, object value)
        {
            switch (parameterType)
            {
                case (int)ParameterType.Region:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + value;
                    break;
                case (int)ParameterType.Bool:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + value;
                    break;
                case (int)ParameterType.Float:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + ((float)value).ToString("0.00");
                    break;
                case (int)ParameterType.Int:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + ((int)value);
                    break;
                case (int)ParameterType.PhysicalObject:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + value;
                    break;
                case (int)ParameterType.Point:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + ((Point)value).ToFormatedString();
                    break;
                case (int)ParameterType.Trigger:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + (string.IsNullOrEmpty((string)value)? "NULL" : value.ToString());
                    break;
                case (int)GameParameterType.NativeFunction:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + Enum.ToObject(typeof(NativeFunctions), value);
                    break;
                case (int)GameParameterType.NativeParameterBool:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + Enum.ToObject(typeof(NativeParameterBool), value);
                    break;    
                case (int)ParameterType.MenuObject:
                case (int)ParameterType.CameraPath:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + value;
                    break;
                case (int)ParameterType.String:
                    node.Text = "(" + (parameterIndex + 1).ToString("00") + "):" + "'" + value + "'";
                    break;
                default:
                    throw new Exception("Undefined parameterType: '" + parameterType + "'");
            }
            ((ITriggerItem)node.Parent.Tag).EditorSetValue(parameterIndex, value);
        }

        private void EditItem(TreeNode node)
        {
            var parameterType = (int)node.Tag;
            var paramIndex = Convert.ToInt32(node.Text.Substring(1, 2)) - 1;
            var item = (ITriggerItem)node.Parent.Tag;
            object oldValue;
            object newValue;
            switch (parameterType)
            {
                case (int)ParameterType.Region:
                    var regionDialog = new RegionSelector();
                    oldValue = node.Text.Substring(5);
                    if ((string)oldValue != "NULL")
                        regionDialog.RegionName = (string)oldValue;
                    if (regionDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(regionDialog.RegionName))
                        return;
                    if (regionDialog.RegionName == (string)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, regionDialog.RegionName);
                    newValue = regionDialog.RegionName;
                    break;
                case (int)ParameterType.PhysicalObject:
                    var physicalObjectDialog = new PhysicalObjectSelector();
                    oldValue = node.Text.Substring(5);
                    if ((string)oldValue != "NULL")
                        physicalObjectDialog.PhysicalObjectName = (string)oldValue;
                    if (physicalObjectDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(physicalObjectDialog.PhysicalObjectName))
                        return;
                    if (physicalObjectDialog.PhysicalObjectName == (string)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, physicalObjectDialog.PhysicalObjectName);
                    newValue = physicalObjectDialog.PhysicalObjectName;
                    break;
                case (int)ParameterType.Bool:
                    var boolDialog = new BoolSelector();
                    oldValue = Convert.ToBoolean(node.Text.Substring(5));
                    boolDialog.SetValue((bool)oldValue);
                    if (boolDialog.ShowDialog() != DialogResult.OK)
                        return;
                    if (boolDialog.BoolValue == (bool)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, boolDialog.BoolValue);
                    newValue = boolDialog.BoolValue;
                    break;
                case (int)ParameterType.Float:
                    var floatDialog = new FloatSelector();
                    oldValue = Convert.ToSingle(node.Text.Substring(5));
                    floatDialog.SetValue((float)oldValue);
                    if (floatDialog.ShowDialog() != DialogResult.OK)
                        return;
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (floatDialog.DecimalValue == (float)oldValue)
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, floatDialog.DecimalValue);
                    newValue = floatDialog.DecimalValue;
                    break;
                case (int)ParameterType.Int:
                    var integerDialog = new IntegerSelector();
                    oldValue = Convert.ToInt32(node.Text.Substring(5));
                    integerDialog.SetValue((int)oldValue);
                    if (integerDialog.ShowDialog() != DialogResult.OK)
                        return;
                    if (integerDialog.IntegerValue == (int)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, integerDialog.IntegerValue);
                    newValue = integerDialog.IntegerValue;
                    break;
                case (int)ParameterType.Point:
                    var pointDialog = new PointSelector();
                    Point p;
                    oldValue = node.Text.Substring(5).ParsePoint();
                    pointDialog.SetValue((Point)oldValue);
                    if (pointDialog.ShowDialog() != DialogResult.OK)
                        return;
                    if (pointDialog.PointValue == (Point)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, pointDialog.PointValue);
                    newValue = pointDialog.PointValue;
                    break;
                case (int)ParameterType.Trigger:
                    var triggerDialog = new TriggerSelector();
                    string trigger;
                    oldValue = node.Text.Substring(5);
                    if (oldValue.ToString() != "NULL")
                        triggerDialog.DefaultValue = oldValue.ToString();
                    if (triggerDialog.ShowDialog() != DialogResult.OK)
                        return;
                    if (triggerDialog.SelectedTrigger == oldValue.ToString())
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, triggerDialog.SelectedTrigger);
                    newValue = triggerDialog.SelectedTrigger;
                    break;
                case (int)GameParameterType.NativeFunction:
                    var nativeFunctionsDialog = new NativeFunctionsSelector();
                    oldValue = (int)Enum.Parse(typeof(NativeFunctions), node.Text.Substring(5));
                    nativeFunctionsDialog.DefaultValue = (int)oldValue;
                    if (nativeFunctionsDialog.ShowDialog() != DialogResult.OK)
                        return;
                    if (nativeFunctionsDialog.SelectedValue == (int)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, nativeFunctionsDialog.SelectedValue);
                    newValue = nativeFunctionsDialog.SelectedValue;
                    break;
                case (int)GameParameterType.NativeParameterBool:
                    var nativeParametersBoolSelector = new NativeParametersBoolSelector();
                    oldValue = (int)Enum.Parse(typeof(NativeParameterBool), node.Text.Substring(5));
                    nativeParametersBoolSelector.DefaultValue = (int)oldValue;
                    if (nativeParametersBoolSelector.ShowDialog() != DialogResult.OK)
                        return;
                    if (nativeParametersBoolSelector.SelectedValue == (int)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, nativeParametersBoolSelector.SelectedValue);
                    newValue = nativeParametersBoolSelector.SelectedValue;
                    break;
                case (int)ParameterType.MenuObject:
                    var menuObjectDialog = new MenuObjectSelector();
                    oldValue = node.Text.Substring(5);
                    if ((string)oldValue != "NULL")
                        menuObjectDialog.MenuObjectName = (string)oldValue;
                    if (menuObjectDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(menuObjectDialog.MenuObjectName))
                        return;
                    if (menuObjectDialog.MenuObjectName == (string)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, menuObjectDialog.MenuObjectName);
                    newValue = menuObjectDialog.MenuObjectName;
                    break;
                case (int)ParameterType.CameraPath:
                    var cameraPathDialog = new CameraPathSelector();
                    oldValue = node.Text.Substring(5);
                    if ((string)oldValue != "NULL")
                        cameraPathDialog.CameraPathName = (string)oldValue;
                    if (cameraPathDialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(cameraPathDialog.CameraPathName))
                        return;
                    if (cameraPathDialog.CameraPathName == (string)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, cameraPathDialog.CameraPathName);
                    newValue = cameraPathDialog.CameraPathName;
                    break;
                case (int)ParameterType.String:
                    var stringDialog = new StringSelector();
                    oldValue = Convert.ToString(node.Text.Substring(5)).Replace("'", "");
                    stringDialog.SetValue((string)oldValue);
                    if (stringDialog.ShowDialog() != DialogResult.OK)
                        return;
                    if (stringDialog.StringValue == (string)oldValue)
                        return;
                    SetItemParameterValue(node, parameterType, paramIndex, stringDialog.StringValue);
                    newValue = stringDialog.StringValue;
                    break;
                default:
                    throw new Exception("Undefined item type: " + parameterType);
            }
            node.Parent.Text = node.Parent.Tag.ToString();
            if (TriggerItemParameterChanged != null)
                TriggerItemParameterChanged(this, new TriggerItemParameterEventArgs(SelectedTrigger.Name, item, paramIndex, oldValue, newValue));

        }

        #endregion


    }

    public class TriggerEventActionEventArgs : EventArgs
    {
        public string TriggerName;
        public int EventId;

        public TriggerEventActionEventArgs(string triggerName, int eventId)
        {
            TriggerName = triggerName;
            EventId = eventId;
        }
    }

    public class TriggerEventChangedEventArgs : EventArgs
    {
        public string TriggerName;
        public int OldEventId;
        public int NewEventId;

        public TriggerEventChangedEventArgs(string triggerName, int oldEventId, int newEventId)
        {
            TriggerName = triggerName;
            OldEventId = oldEventId;
            NewEventId = newEventId;
        }
    }

    public class TriggerEventPositionChangedEventArgs : EventArgs
    {
        public string TriggerName;
        public int EventId;
        public int NewPos;

        public TriggerEventPositionChangedEventArgs(string triggerName, int eventId, int newPos)
        {
            TriggerName = triggerName;
            EventId = eventId;
            NewPos = newPos;
        }
    }

    public class TriggerItemEventArgs : EventArgs
    {
        public string TriggerName;
        public ITriggerItem Item;

        public TriggerItemEventArgs(string triggerName, ITriggerItem item)
        {
            TriggerName = triggerName;
            Item = item;
        }
    }

    public class TriggerItemChangeEventArgs : EventArgs
    {
        public string TriggerName;
        public ITriggerItem OldItem;
        public ITriggerItem NewItem;

        public TriggerItemChangeEventArgs(string triggerName, ITriggerItem oldItem, ITriggerItem newItem)
        {
            TriggerName = triggerName;
            OldItem = oldItem;
            NewItem = newItem;
        }
    }

    public class TriggerItemPositionEventArgs : EventArgs
    {
        public string TriggerName;
        public ITriggerItem Item;
        public int NewPos;


        public TriggerItemPositionEventArgs(string triggerName, ITriggerItem item, int newPos)
        {
            TriggerName = triggerName;
            Item = item;
            NewPos = newPos;
        }
    }

    public class TriggerItemParameterEventArgs : EventArgs
    {
        public string TriggerName;
        public ITriggerItem Item;
        public int ParamIndex;
        public object OldValue;
        public object NewValue;


        public TriggerItemParameterEventArgs(string triggerName, ITriggerItem item, int paramIndex, object oldValue, object newValue)
        {
            TriggerName = triggerName;
            Item = item;
            ParamIndex = paramIndex;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }


}
