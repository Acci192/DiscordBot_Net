using System;
using System.Collections.Generic;
using System.Text;
using static VoteBot_Discord.ReadModels.ActivePoll;

namespace VoteBot_Discord.ReadModels
{
    public interface IActivePollQueries
    {
        List<Poll> GetActivePolls();
        List<string> GetActivePollsName();
        Guid GetPollId(string name);
    }
}
