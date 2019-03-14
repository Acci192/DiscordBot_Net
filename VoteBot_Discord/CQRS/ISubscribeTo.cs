using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.CQRS
{
    public interface ISubscribeTo<TEvent>
    {
        void Handle(TEvent evt);
    }
}
