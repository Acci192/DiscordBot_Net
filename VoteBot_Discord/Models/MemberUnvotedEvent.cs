using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class MemberUnvotedEvent : Event
    {
        public string Id { get; set; }
        public ulong Member { get; set; }

        public MemberUnvotedEvent(string id, ulong member)
        {
            Type = EventType.MemberUnvoted;
            Id = id;
            Member = member;
        }
    }
}
