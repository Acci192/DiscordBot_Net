using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.CQRS
{
    public interface IApplyEvent<TEvent>
    {
        void Apply(TEvent evt);
    }
}
