using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class DescriptionAddedEvent : Event
    {
        public Guid Id { get; set; }
        public string Description { get; set; }

        public DescriptionAddedEvent(Guid id, string description)
        {
            Type = EventType.DescriptionAdded;
            Id = id;
            Description = description;
        }

    }
}
