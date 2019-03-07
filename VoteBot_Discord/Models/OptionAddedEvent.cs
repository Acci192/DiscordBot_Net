using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class OptionAddedEvent : Event
    {
        public string Id { get; set; }
        public string Option { get; set; }

        public OptionAddedEvent(string id, string option)
        {
            Type = EventType.OptionAdded;
            Id = id;
            Option = option;
        }
    }
}
