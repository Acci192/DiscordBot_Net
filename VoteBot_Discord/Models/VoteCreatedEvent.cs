using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class VoteCreatedEvent : Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ulong Owner { get; set; }

        public VoteCreatedEvent(Guid id, string name, ulong owner)
        {
            Type = EventType.VotedCreated;
            Id = id;
            Name = name;
            Owner = owner;
        }
    }
}
