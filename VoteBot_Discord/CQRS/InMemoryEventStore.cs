using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VoteBot_Discord.CQRS
{
    public class InMemoryEventStore : IEventStore
    {
        private class Stream
        {
            public ArrayList Events;
        }

        private ConcurrentDictionary<Guid, Stream> Store = new ConcurrentDictionary<Guid, Stream>();

        public IEnumerable LoadEventsFor<TAggregate>(Guid id)
        {
            Stream s;
            if (Store.TryGetValue(id, out s))
                return s.Events;
            else
                return new ArrayList();
        }

        public void SaveEventsFor<TAggregate>(Guid aggregateId, int eventsLoaded, ArrayList newEvents)
        {
            var s = Store.GetOrAdd(aggregateId, _ => new Stream());

            while (true)
            {
                var eventList = s.Events;

                var prevEvents = eventList == null ? 0 : eventList.Count;

                if (prevEvents != eventsLoaded)
                    throw new Exception("Concurrenct conflict; cannot persist therse events");

                var newEventList = eventList == null
                    ? new ArrayList()
                    : (ArrayList)eventList.Clone();
                newEventList.AddRange(newEvents);

                if (Interlocked.CompareExchange(ref s.Events, newEventList, eventList) == eventList)
                    break;
            }
        }

        private Guid GetAggregateIdFromEvent(object e)
        {
            var idField = e.GetType().GetField("Id");
            if(idField == null)
                throw new Exception($"Event type {e.GetType().Name} is missing and Id field");

            return (Guid)idField.GetValue(e);
        }
    }
}
