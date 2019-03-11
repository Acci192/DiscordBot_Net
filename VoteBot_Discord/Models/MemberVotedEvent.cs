using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class MemberVotedEvent : Event
    {
        public Guid Id { get; set; }
        public string Option { get; set; }
        public ulong Member { get; set; }

        public MemberVotedEvent(Guid id, string option, ulong member)
        {
            Type = EventType.MemberVoted;
            Id = id;
            Option = option;
            Member = member;
        }
    }
}
