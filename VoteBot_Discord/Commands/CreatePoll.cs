using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Commands
{
    public class CreatePoll
    {
        public Guid Id;
        public ulong Channel;
        public string Name;
        public ulong Owner;
    }
}
