using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Commands
{
    public class AddOptions
    {
        public Guid Id;
        public ulong User;
        public List<string> Options;
    }
}
