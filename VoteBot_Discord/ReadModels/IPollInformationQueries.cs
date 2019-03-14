using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.ReadModels
{
    public interface IPollInformationQueries
    {
        string GetInformation(Guid id);
        string GetResults(Guid id);
        ulong GetPollChannel(Guid id);
    }
}
