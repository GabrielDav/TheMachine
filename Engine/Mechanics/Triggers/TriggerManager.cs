using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Engine.Mechanics.Triggers
{
    public enum ItemType
    {
        Event = 0,
        Condition = 1,
        Action = 2
    }

    public enum EventType
    {
        ObjectEntersRegion = 1,
        ObjectLeavesRegion = 2,
        GameInitialization = 3,
        ObjectClicked = 4,
        BackPressed = 5
    }

    public enum ConditionType
    {
        TriggeringRegion = 1,
        TriggeringObject = 2,
        CameraIsMoving = 3
    }

    public enum ActionType
    {
        SetCameraZoom = 1,
        ActivateObject = 2,
        MoveCamera = 3,
        SetCameraPosition = 4,
        DisableTrigger = 5,
        ExitGame = 6,
        SetGravity = 7,
        RotateCamera = 8
    }

    public enum ParameterType
    {
        Region = 1,
        Bool = 2,
        Float = 3,
        Int = 4,
        PhysicalObject = 5,
        Point = 6,
        Trigger = 7,
        String = 8,
        MenuObject = 9,
        CameraPath = 10
    }

    public interface ITriggerItem : IDisposable
    {
        int TypeId { get; }
        int[] EditorGetParametersTypes();
        object[] EditorGetPatametersValues();
        void EditorSetValue(int index, object value);
    }

    public interface ITriggerAction : ITriggerItem
    {
        void DoAction(EventParams eventParams);

        object Clone();
    }

    public class EventParams
    {
        public Region TriggeringRegion;
        public PhysicalObject TriggeringObject;
        public Point Location;
    }

    public class Event
    {
        public int Id;
    }

    public class TriggerManager : IDisposable
    {
        protected List<int> _registeredEvents;
        protected List<Trigger> _triggers;
        protected Dictionary<int, List<Trigger>> _eventLinks;
        public List<PhysicalObject> MapObjects;

        public List<Trigger> Triggers
        {
            get { return _triggers; }
        }

        public TriggerManager(List<Trigger> triggers, List<PhysicalObject> mapObjects)
        {
            _triggers = triggers;
            _eventLinks = new Dictionary<int, List<Trigger>>();
            _registeredEvents = new List<int>();
            MapObjects = mapObjects;

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
            if (!_registeredEvents.Contains(eventId))
            {
                _registeredEvents.Add(eventId);
            }


            if (_eventLinks.ContainsKey(eventId) && _eventLinks[eventId].Contains(triggerToAttach))
            {
                return;
            }

            if (!_eventLinks.ContainsKey(eventId))
            {
                _eventLinks.Add(eventId, new List<Trigger> {triggerToAttach});
            }
            else
            {
                _eventLinks[eventId].Add(triggerToAttach);
            }
        }

        public void ActionOccured(int eventType, EventParams eventParams)
        {
            foreach (var eventLink in _eventLinks)
            {
                if (eventLink.Key == eventType)
                {
                    foreach (var trigger in eventLink.Value)
                    {
                        if (!trigger.Enabled)
                            continue;
                        trigger.Check(eventParams);
                    }
                }
            }
        }

        public void Dispose()
        {
            _triggers = null;
            _eventLinks.Clear();
            _registeredEvents.Clear();
            MapObjects = null;
        }
    }
}
