using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Events
{
    public class OptionsAdded
    {
        public Guid Id;
        public List<string> Options;
    }
}
