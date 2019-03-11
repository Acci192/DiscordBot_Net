using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class MemberUnvotedEvent : Event
    {
        public Guid Id { get; set; }
        public ulong Member { get; set; }

        public MemberUnvotedEvent(Guid id, ulong member)
        {
            Type = EventType.MemberUnvoted;
            Id = id;
            Member = member;
        }
    }
}
