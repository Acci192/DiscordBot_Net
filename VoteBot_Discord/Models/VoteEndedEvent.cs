using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class VoteEndedEvent : Event
    {
        public string Id { get; set; }

        public VoteEndedEvent(string id)
        {
            Type = EventType.VoteEnded;
            Id = id;
        }
    }
}
