using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.CQRS
{
    public interface IEventStore
    {
        IEnumerable LoadEventsFor<Taggregate>(Guid id);
        void SaveEventsFor<TAggregate>(Guid id, int eventsLoaded, ArrayList newEvents);
    }
}
