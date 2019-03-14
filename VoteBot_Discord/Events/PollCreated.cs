using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Events
{
    public class PollCreated
    {
        public Guid Id;
        public ulong Channel;
        public string Name;
        public ulong Owner;
    }
}
