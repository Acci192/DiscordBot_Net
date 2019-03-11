using System;
using System.Collections.Generic;
using System.Text;

namespace VoteBot_Discord.Models
{
    public interface IVote
    {
        Guid GetId();
        bool ContainsOption(string option);
        bool UserVoted(ulong user);
        bool IsOwner(ulong user);
        string GetInformation();
        string GetResults();
    }
}
