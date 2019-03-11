using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class TimeAddedEvent : Event
    {
        public Guid Id { get; set; }
        public int MinutesAdded { get; set; }

        public TimeAddedEvent(Guid id, int minutesAdded)
        {
            Type = EventType.TimeAdded;
            Id = id;
            MinutesAdded = minutesAdded;
        }
    }
}
