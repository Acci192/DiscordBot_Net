using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public abstract class Event
    {
        public EventType Type { get; set; }
    }

    public enum EventType
    {
        VotedCreated,
        OptionAdded,
        MemberVoted,
        MemberUnvoted,
        VoteEnded,
        TimeAdded,
        DescriptionAdded
    }
}
