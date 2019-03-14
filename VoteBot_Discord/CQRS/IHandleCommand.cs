using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.CQRS
{
    public interface IHandleCommand<TCommand>
    {
        IEnumerable Handle(TCommand c);
    }
}
