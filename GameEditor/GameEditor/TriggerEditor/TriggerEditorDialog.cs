using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Engine.Mechanics;
using Engine.Mechanics.Triggers;
using Engine.Mechanics.Triggers.Actions;
using Engine.Mechanics.Triggers.Conditions;
using TheGoo;

namespace GameEditor.TriggerEditor
{
    public partial class TriggerEditorDialog : Form
    {
        public class Action
        {
            public enum ActionType
            {
                CreateTrigger, DeleteTrigger, TriggerPropertyChange, TriggerAddEvent, TriggerRemoveEvent, TriggerEventPositionChanged, AddItem, RemoveItem, ItemParameterChanged, ItemPositionChanged
            }

            public ActionType ExecutedAction { get; protected set; }

            public Trigger Target;

            public object PropertyBeforeChange;
            public object PropertyAfterChange;
            public string PropertyName;
            public object SubTarget;
            public int Index;

            public Action(ActionType action)
            {
                ExecutedAction = action;
            }

            public override string ToString()
            {
                return "Action: target = '" + Target.Name + "' propertyName = '" + PropertyName +
                       "' ActionType = '" + ExecutedAction + "'";
            }
        }

        protected Action _tempAction;

        public bool IsActionsBlocked;
        public readonly List<Action> Actions;
        public int ActionIndex;
        public static TriggerEditorDialog Form;

        

        public TriggerEditorDialog()
        {
            InitializeComponent();
            Form = this;
            TriggerController.Init();
            Actions = new List<Action>();
            triggersBox1.Fill(GameGlobals.Map.Triggers);
            triggersBox1.TriggerSelected += (sender, name) => SelectTrigger(name);
            triggersBox1.TriggerAdded += (sender, args) => AddTrigger(args.TriggerName);
            triggersBox1.TriggerRemoved += (sender, args) => RemoveTrigger(args.TriggerName);
            triggersBox1.TriggerPropertyChanging +=
                (sender, args) => TriggerPropertyChanging(args.TriggerName, args.PropertyName, args.PropertyValue);
            triggersBox1.TriggerPropertyChanged += (sender, args) => TriggerPropertyChanged(args.PropertyValue);
            triggerWindow1.TriggerEventAdded += (sender, args) => TriggerAddEvent(args.TriggerName, args.EventId);
            triggerWindow1.TriggerEventRemoved += (sender, args) => TriggerRemoveEvent(args.TriggerName, args.EventId);
            triggerWindow1.TriggerEventChanged +=
                (sender, args) => TriggerChangeEvent(args.TriggerName, args.OldEventId, args.NewEventId);
            triggerWindow1.TriggerEventPositionChanged +=
                (sender, args) => TriggerChangeEventPosition(args.TriggerName, args.EventId, args.NewPos);
            triggerWindow1.TriggerItemPositionChanged += (sender, args) => TriggerChangeItemPosition(args.TriggerName, args.Item, args.NewPos);
            triggerWindow1.TriggerItemAdded += (sender, args) => TriggerAddItem(args.TriggerName, args.Item);
            triggerWindow1.TriggerItemRemoved += (sender, args) => TriggerRemoveItem(args.TriggerName, args.Item);
            triggerWindow1.TriggerItemParameterChanged +=
                (sender, args) =>
                TriggerItemParameterChanged(args.TriggerName, args.Item, args.ParamIndex, args.OldValue, args.NewValue);

        }

        public void AddActionToList(Action action)
        {
            if (IsActionsBlocked)
                return;
            Actions.RemoveRange(ActionIndex, Actions.Count - ActionIndex);
            Actions.Add(action);
            ActionIndex++;
            toolStripRedoButton.Enabled = false;
            toolStripUndoButton.Enabled = true;
        }

        private void SelectTrigger(string triggerName)
        {
            if (triggerName == null)
                return;
            Trigger selectedTrigger = null;
            foreach (var trigger in GameGlobals.Map.Triggers)
            {
                if (trigger.Name == triggerName)
                {
                    selectedTrigger = trigger;
                    break;
                }
            }
            if (selectedTrigger == null)
                throw new NullReferenceException("Trriger '" + triggerName + "' not exist");
            SelectTrigger(selectedTrigger);
        }

        private void SelectTrigger(Trigger trigger)
        {
            triggerWindow1.SelectTrigger(trigger);
        }

        private void TriggerAddEvent(string triggerName, int eventId)
        {
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == triggerName);
            trigger.Events.Add(eventId);
            AddActionToList(new Action(Action.ActionType.TriggerAddEvent){PropertyAfterChange = eventId, Target = trigger});
        }

        private void TriggerRemoveEvent(string triggerName, int eventId)
        {
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == triggerName);
            trigger.Events.Remove(eventId);
            AddActionToList(new Action(Action.ActionType.TriggerRemoveEvent){PropertyBeforeChange = eventId, Target = trigger});
        }

        private void TriggerAddItem(string triggerName, ITriggerItem item)
        {
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == triggerName);
            if (item is ICondition)
            {
                trigger.Conditions.Add((ICondition)item);
            }
            else if (item is ITriggerAction)
            {
                trigger.Actions.Add((ITriggerAction)item);
            }
            else
            {
                throw new Exception("Undefined item type: " + item);
            }
            AddActionToList(new Action(Action.ActionType.AddItem){ PropertyBeforeChange = item, Target = trigger});
        }

        private void TriggerRemoveItem(string triggerName, ITriggerItem item)
        {
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == triggerName);
            if (item is ICondition)
            {
                trigger.Conditions.Remove((ICondition)item);
            }
            else if (item is ITriggerAction)
            {
                trigger.Actions.Remove((ITriggerAction)item);
            }
            else
            {
                throw new Exception("Undefined item type: " + item);
            }
            AddActionToList(new Action(Action.ActionType.RemoveItem){PropertyBeforeChange = item, Target = trigger});
        }

        private void AddTrigger(string name)
        {
            var trigger = new Trigger{Name = name, Enabled = true};
            AddTrigger(trigger);
        }

        private void AddTrigger(Trigger trigger)
        {
            GameGlobals.Map.Triggers.Add(trigger);
            AddActionToList(new Action(Action.ActionType.CreateTrigger) { PropertyName = trigger.Name, Target = trigger });
        }

        private void RemoveTrigger(string name)
        {
            if (triggerWindow1.SelectedTrigger != null && triggerWindow1.SelectedTrigger.Name == name)
                triggerWindow1.DeselectTrigger();
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == name);
            GameGlobals.Map.Triggers.Remove(trigger);
            foreach (var t in GameGlobals.Map.Triggers)
            {
                foreach (var action in t.Actions)
                {
                    if (action.TypeId == (int)ActionType.DisableTrigger)
                    {
                        var disableTrigger = action as DisableTrigger;
                        if (disableTrigger.TriggerName == name)
                            disableTrigger.TriggerName = null;
                    }
                }
            }
            AddActionToList(new Action(Action.ActionType.DeleteTrigger){PropertyName = name, Target = trigger});
        }

        private void TriggerChangeEvent(string name, int oldEventId, int newEventId)
        {
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == name);
            trigger.Events.Remove(oldEventId);
            trigger.Events.Add(newEventId);
        }

        private void TriggerChangeEventPosition(string name, int eventId, int newPos)
        {
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == name);
            var index = trigger.Events.IndexOf(eventId);
            var temp = trigger.Events[newPos];
            trigger.Events[newPos] = trigger.Events[index];
            trigger.Events[index] = temp;
            triggerWindow1.TriggerEventChangePositionForced(eventId, newPos);
            AddActionToList(new Action(Action.ActionType.TriggerEventPositionChanged){Target = trigger, PropertyBeforeChange = index, PropertyAfterChange = newPos, SubTarget = eventId});
        }

        private void TriggerChangeItemPosition(string triggerName, ITriggerItem item, int newPos)
        {
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == triggerName);
            int index;
            if (item is ITriggerAction)
            {
                index = trigger.Actions.IndexOf((ITriggerAction)item);
                var temp = trigger.Actions[newPos];
                trigger.Actions[newPos] = trigger.Actions[index];
                trigger.Actions[index] = temp;
                triggerWindow1.TriggerActionChangePositionForced((ITriggerAction)item, newPos);
                AddActionToList(new Action(Action.ActionType.ItemPositionChanged) { Target = trigger, PropertyBeforeChange = index, PropertyAfterChange = newPos, SubTarget = item });
            }
            else
            {
                index = trigger.Conditions.IndexOf((ICondition)item);
                var temp = trigger.Conditions[newPos];
                trigger.Conditions[newPos] = trigger.Conditions[index];
                trigger.Conditions[index] = temp;
                triggerWindow1.TriggerConditionChangePositionForced((ICondition)item, newPos);
                AddActionToList(new Action(Action.ActionType.ItemPositionChanged) { Target = trigger, PropertyBeforeChange = index, PropertyAfterChange = newPos, SubTarget = item });
            }

        }

        private void TriggerPropertyChanging(string triggerName, string propertyName, object value)
        {
            var trigger = GameGlobals.Map.Triggers.Find(t => t.Name == triggerName);
            _tempAction = new Action(Action.ActionType.TriggerPropertyChange){Target = trigger, PropertyBeforeChange = value, PropertyName = propertyName};
        }

        private void TriggerPropertyChanged(object value)
        {
            _tempAction.PropertyAfterChange = value;
            PropertyInfo propertyFieldType = _tempAction.Target.GetType().GetProperty(_tempAction.PropertyName);
            if (_tempAction.ExecutedAction == Action.ActionType.TriggerPropertyChange && _tempAction.PropertyName == "Name")
            {
                foreach (var trigger in GameGlobals.Map.Triggers)
                {
                    foreach (var action in trigger.Actions)
                    {
                        if (action.TypeId == (int)ActionType.DisableTrigger)
                        {
                            var disableTrigger = action as DisableTrigger;
                            if (disableTrigger.TriggerName == _tempAction.Target.Name)
                                disableTrigger.TriggerName = value.ToString();
                        }
                    }
                }    
            }
            propertyFieldType.SetValue(_tempAction.Target, _tempAction.PropertyAfterChange, null);
            AddActionToList(_tempAction);
        }

        private void TriggerItemParameterChanged(string triggerName, ITriggerItem item, int parameterIndex, object oldValue, object newValue)
        {
            _tempAction = new Action(Action.ActionType.ItemParameterChanged)
                {
                    Target = GameGlobals.Map.Triggers.Find(t => t.Name == triggerName),
                    PropertyBeforeChange = oldValue,
                    PropertyAfterChange = newValue,
                    SubTarget = item,
                    Index = parameterIndex
                };
            AddActionToList(_tempAction);
        }

        public void UndoPropertyChange(Action action)
        {
            PropertyInfo propertyFieldType = action.Target.GetType().GetProperty(action.PropertyName);
            propertyFieldType.SetValue(action.Target, action.PropertyBeforeChange, null);
            var name = action.PropertyName == "Name" ? (string)action.PropertyAfterChange : action.Target.Name;
            triggersBox1.RefreshTriggerValues(name, action.Target);
                
        }

        public void RedoPropertyChange(Action action)
        {
            PropertyInfo propertyFieldType = action.Target.GetType().GetProperty(action.PropertyName);
            propertyFieldType.SetValue(action.Target, action.PropertyAfterChange, null);
            var name = action.PropertyName == "Name" ? (string)action.PropertyBeforeChange : action.Target.Name;
            triggersBox1.RefreshTriggerValues(name, action.Target);
        }

        private void Undo()
        {
            IsActionsBlocked = true;
            toolStripRedoButton.Enabled = true;
            ActionIndex--;
            var a = Actions[ActionIndex];
            if (ActionIndex == 0)
                toolStripUndoButton.Enabled = false;
            switch (a.ExecutedAction)
            {
                case Action.ActionType.CreateTrigger:
                    RemoveTrigger(a.PropertyName);
                    triggersBox1.TriggerRemoveForced(a.Target.Name);
                    break;
                case Action.ActionType.DeleteTrigger:
                    AddTrigger(a.Target);
                    a.Target.Name = triggersBox1.GetSafeName(a.Target.Name);
                    triggersBox1.AddTriggerForced(a.Target);
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    break;
                case Action.ActionType.TriggerAddEvent:
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    triggerWindow1.DeleteEvent((int)a.PropertyAfterChange);
                    break;
                case Action.ActionType.TriggerRemoveEvent:
                    if (a.Target.Events.Contains((int)a.PropertyBeforeChange))
                        break;
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    triggerWindow1.AddEvent((int)a.PropertyBeforeChange);
                    break;
                case Action.ActionType.TriggerPropertyChange:
                    UndoPropertyChange(a);
                    break;
                case Action.ActionType.TriggerEventPositionChanged:
                    TriggerChangeEventPosition(a.Target.Name, (int)a.SubTarget, (int)a.PropertyBeforeChange);
                    break;
                case Action.ActionType.ItemPositionChanged:
                    TriggerChangeItemPosition(a.Target.Name, (ITriggerItem)a.SubTarget, (int)a.PropertyBeforeChange);
                    break;
                case Action.ActionType.AddItem:
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    triggerWindow1.DeleteItem((ITriggerItem)a.PropertyBeforeChange);
                    break;
                case Action.ActionType.RemoveItem:
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    triggerWindow1.AddItem((ITriggerItem)a.PropertyBeforeChange);
                    break;
                case Action.ActionType.ItemParameterChanged:
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    triggerWindow1.SetItemParameterValueForced((ITriggerItem)a.SubTarget, a.Index, a.PropertyBeforeChange);
                    break;
                default:
                    throw new Exception("Undefined action " + a.ExecutedAction);
            }
            IsActionsBlocked = false;

        }

        public void Redo()
        {
            IsActionsBlocked = true;
            toolStripUndoButton.Enabled = true;
            var a = Actions[ActionIndex];
            ActionIndex++;
            if (Actions.Count == ActionIndex)
            {
                toolStripRedoButton.Enabled = false;
            }
            
            switch (a.ExecutedAction)
            {
                case Action.ActionType.CreateTrigger:
                    AddTrigger(a.Target);
                    a.Target.Name = triggersBox1.GetSafeName(a.Target.Name);
                    triggersBox1.AddTriggerForced(a.Target);
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    break;
                case Action.ActionType.DeleteTrigger:
                    RemoveTrigger(a.PropertyName);
                    triggersBox1.TriggerRemoveForced(a.Target.Name);
                    break;
                case Action.ActionType.TriggerAddEvent:
                    if (a.Target.Events.Contains((int)a.PropertyAfterChange))
                        break;
                    triggerWindow1.AddEvent((int)a.PropertyAfterChange);
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    break;
                case Action.ActionType.TriggerRemoveEvent:
                    triggerWindow1.DeleteEvent((int)a.PropertyBeforeChange);
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    break;
                case Action.ActionType.TriggerPropertyChange:
                    RedoPropertyChange(a);
                    break;
                case Action.ActionType.TriggerEventPositionChanged:
                    TriggerChangeEventPosition(a.Target.Name, (int)a.SubTarget, (int)a.PropertyAfterChange);
                    break;
                case Action.ActionType.ItemPositionChanged:
                    TriggerChangeItemPosition(a.Target.Name, (ITriggerItem)a.SubTarget, (int)a.PropertyAfterChange);
                    break;
                case Action.ActionType.AddItem:
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    triggerWindow1.AddItem((ITriggerItem)a.PropertyBeforeChange);
                    break;
                case Action.ActionType.RemoveItem:
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    triggerWindow1.DeleteItem((ITriggerItem)a.PropertyBeforeChange);
                    break;
                case Action.ActionType.ItemParameterChanged:
                    triggersBox1.TriggerSelectItemForced(a.Target.Name);
                    triggerWindow1.SetItemParameterValueForced((ITriggerItem)a.SubTarget, a.Index, a.PropertyAfterChange);
                    break;
            }
            IsActionsBlocked = false;
        }

        private void ToolStripUndoButtonClick(object sender, EventArgs e)
        {
            Undo();
        }

        private void ToolStripRedoButtonClick(object sender, EventArgs e)
        {
            Redo();
        }

    }
}
