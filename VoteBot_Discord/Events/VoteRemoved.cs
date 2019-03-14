using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Events
{
    public class VoteRemoved
    {
        public Guid Id;
        public ulong User;
    }
}
