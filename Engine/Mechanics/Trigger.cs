using System;
using System.Collections.Generic;
using Engine.Core;

namespace Engine.Mechanics
{
    public enum ItemType { Event = 0, Condition = 1, Action = 2}

    public enum EventType { ObjectEntersRegion = 1, ObjectLeavesRegion = 2 }

    public enum ConditionType { TriggeringRegion = 1 }

    public enum ActionType { SetCameraZoom = 1 }

    public enum ParameterType { Region = 1, Bool = 2, Float = 3, Int = 4, PhysicalObject = 5 }

    public interface ITriggerItem
    {
        int TypeId { get; }
        int[] EditorGetParametersTypes();
        object[] EditorGetPatametersValues();
        void EditorSetValue(int index, object value);
    }

    public interface ITriggerAction : ITriggerItem
    {
        void DoAction();
    }

    public interface ICondition : ITriggerItem
    {
        bool Check(EventParams eventParams); 
    }

    public class EventParams
    {
        public Region TriggeringRegion;
        public PhysicalObject TriggeringObject;
    }

    public class Event
    {
        public int Id;
    }

    public class Trigger
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<int> Events;
        public List<ITriggerAction> Actions;
        public List<ICondition> Conditions;

        public Trigger()
        {
            Events = new List<int>();
            Actions = new List<ITriggerAction>();
            Conditions = new List<ICondition>();
        }

        public void Check(EventParams eventParams)
        {
            var execute = true;
            foreach (var condition in Conditions)
            {
                if (!condition.Check(eventParams))
                {
                    execute = false;
                    break;
                }
            }
            if (!execute)
                return;
            foreach (var triggerAction in Actions)
            {
                triggerAction.DoAction();
            }
        }

    }

    public class TriggerManager
    {
        protected List<int> _registeredEvents;
        protected List<Trigger> _triggers;
        protected Dictionary<int, List<Trigger>> _eventLinks;

        public TriggerManager(List<Trigger> triggers)
        {
            _triggers = triggers;
            _eventLinks = new Dictionary<int, List<Trigger>>();
            _registeredEvents = new List<int>();
            foreach (var trigger in triggers)
            {
                foreach (var e in trigger.Events)
                {
                    RegisterEvent(e, trigger);
                }
            }
        }

        public void AddTrigger(Trigger trigger)
        {
            foreach (var item in _triggers)
            {
                if (item.Name.ToLower() == trigger.Name.ToLower())
                {
                    throw new Exception("Trigger named '" + item.Name + "' already exists.");
                }
            }
            _triggers.Add(trigger);
        }

        public void RegisterEvent(int eventId, Trigger triggerToAttach)
        {
            if (_registeredEvents.Contains(eventId))
                return;
            _registeredEvents.Add(eventId);
            if (_eventLinks.ContainsKey(eventId) && _eventLinks[eventId].Contains(triggerToAttach))
                return;
            if (!_eventLinks.ContainsKey(eventId))
                _eventLinks.Add(eventId, new List<Trigger> {triggerToAttach});
            else
                _eventLinks[eventId].Add(triggerToAttach);
        }

        public void ActionOccured(int actionType,  EventParams eventParams)
        {
            foreach (var eventLink in _eventLinks)
            {
                if (eventLink.Key == actionType)
                {
                    foreach (var trigger in eventLink.Value)
                    {
                        trigger.Check(eventParams);
                    }
                }
            }
        }

    }

    public class ConditionTriggeringRegion : ICondition
    {
        public string RegionName;
        public bool ConditionCheck = true;

        public int TypeId
        {
            get { return (int) ConditionType.TriggeringRegion; }
        }

        public int[] EditorGetParametersTypes()
        {
            return new[] {(int) ParameterType.Region, (int) ParameterType.Bool };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[] {RegionName, ConditionCheck};
        }

        public bool Check(EventParams eventParams)
        {
            if (eventParams.TriggeringRegion.Name != RegionName)
                return false;
            return true;
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    RegionName = (string) value;
                    break;
                case 1:
                    ConditionCheck = (bool) value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return "Triggering region is {" + (string.IsNullOrEmpty(RegionName) ? "01:NULL" : "01:" + RegionName) + "} equals {02:" + ConditionCheck + "}";
        }

    }

    public class ActionSetCameraZoom : ITriggerAction
    {
        public int TypeId { get { return (int) ActionType.SetCameraZoom; } }

        public float Zoom;

        public ActionSetCameraZoom()
        {
            Zoom = 1.0f;
        }

        public void DoAction()
        {
            EngineGlobals.Camera2D.Zoom = Zoom;
        }
        
        public int[] EditorGetParametersTypes()
        {
            return new[] { (int)ParameterType.Float };
        }

        public object[] EditorGetPatametersValues()
        {
            return new object[] {Zoom};
        }

        public void EditorSetValue(int index, object value)
        {
            switch (index)
            {
                case 0:
                    Zoom = (float)value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public override string ToString()
        {
            return "Set camera zoom to {01:" + Zoom.ToString("0.00") + "}";
        }
    }
}
