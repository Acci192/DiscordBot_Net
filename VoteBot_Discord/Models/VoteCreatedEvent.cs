using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class VoteCreatedEvent : Event
    {
        public string Id { get; set; }
        public ulong Owner { get; set; }

        public VoteCreatedEvent(string id, ulong owner)
        {
            Type = EventType.VotedCreated;
            Id = id;
            Owner = owner;
        }
    }
}
