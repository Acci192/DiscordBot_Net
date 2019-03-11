using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class VoteEndedEvent : Event
    {
        public Guid Id { get; set; }

        public VoteEndedEvent(Guid id)
        {
            Type = EventType.VoteEnded;
            Id = id;
        }
    }
}
