using System;
using System.Collections.Generic;
using Engine.Mechanics.Triggers.Conditions;

namespace Engine.Mechanics.Triggers
{
    public class Trigger : IDisposable
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
            {
                return;
            }

            foreach (var triggerAction in Actions)
            {
                triggerAction.DoAction(eventParams);
            }
        }

        public object Clone()
        {
            var clone = (Trigger)MemberwiseClone();

            var actions = new List<ITriggerAction>();
            foreach (var action in Actions)
            {
                actions.Add((ITriggerAction)action.Clone());
            }

            clone.Actions = actions;

            var conditions = new List<ICondition>();
            foreach (var condition in Conditions)
            {
                conditions.Add((ICondition)condition.Clone());
            }

            clone.Conditions = conditions;

            var events = new List<int>();
            foreach (var triggerEvent in Events)
            {
                events.Add(triggerEvent);
            }

            clone.Events = events;

            return clone;
        }

        public void Dispose()
        {
            Name = null;
            Events.Clear();
            Events = null;
            foreach (var triggerAction in Actions)
            {
                triggerAction.Dispose();
            }
            Actions.Clear();
            Actions = null;
            foreach (var triggerCondition in Conditions)
            {
                triggerCondition.Dispose();
            }
            Conditions.Clear();
            Conditions = null;
        }
    }
}
