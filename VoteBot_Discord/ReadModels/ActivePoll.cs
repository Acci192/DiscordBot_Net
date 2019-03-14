using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoteBot_Discord.Events;
using VoteBot_Discord.CQRS;

namespace VoteBot_Discord.ReadModels
{
    public class ActivePoll : IActivePollQueries,
        ISubscribeTo<PollCreated>,
        ISubscribeTo<PollEnded>
    {
        public class Poll
        {
            public string Name;
            public Guid Id;
        }
        private List<Poll> ActivePolls = new List<Poll>();
        public List<Poll> GetActivePolls()
        {
            lock (ActivePolls)
            {
                return ActivePolls;
            }
        }

        public List<string> GetActivePollsName()
        {
            lock (ActivePolls)
            {
                return ActivePolls.Select(p => p.Name).ToList();
            }
        }
        public Guid GetPollId(string name)
        {
            lock (ActivePolls)
            {
                var poll = ActivePolls.FirstOrDefault(p => p.Name == name);
                return poll != null ? poll.Id : Guid.Empty;
            }
        }

        public void Handle(PollCreated evt)
        {
            lock (ActivePolls)
            {
                ActivePolls.Add(new Poll { Name = evt.Name, Id = evt.Id });
            }
        }

        public void Handle(PollEnded evt)
        {
            lock (ActivePolls)
            {
                ActivePolls.RemoveAll(p => p.Id == evt.Id);
            }
        }
    }
}
