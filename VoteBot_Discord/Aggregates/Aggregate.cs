using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VoteBot_Discord.Events;
using VoteBot_Discord.CQRS;

namespace VoteBot_Discord.Aggregates
{
    public abstract class Aggregate
    {
        public int EventsLoaded { get; private set; }
        public Guid Id { get; internal set; }

        public void ApplyEvents(IEnumerable events)
        {
            foreach (var evt in events)
                GetType().GetMethod("ApplyOneEvent")
                    .MakeGenericMethod(evt.GetType())
                    .Invoke(this, new object[] { evt });
        }

        public void ApplyOneEvent<TEvent>(TEvent evt)
        {
            if (!(this is IApplyEvent<TEvent> applier))
            {
                throw new InvalidOperationException($"Aggregate {GetType().Name} does not know how to apply event {evt.GetType().Name}");
            }
            applier.Apply(evt);
            EventsLoaded++;
        }
    }
}
