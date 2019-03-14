using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoteBot_Discord.Events;
using VoteBot_Discord.CQRS;

namespace VoteBot_Discord.ReadModels
{
    public class PollInformation : IPollInformationQueries,
        ISubscribeTo<PollCreated>,
        ISubscribeTo<DescriptionAdded>,
        ISubscribeTo<OptionsAdded>,
        ISubscribeTo<VotePlaced>,
        ISubscribeTo<VoteRemoved>
    {
        public class PollInfo
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public string Description { get; set; }
            public ulong Owner { get; set; }
            public ulong Channel { get; set; }
            public List<string> Options { get; set; } = new List<string>();
            public Dictionary<ulong, string> Votes { get; set; } = new Dictionary<ulong, string>();
        }

        private List<PollInfo> Polls = new List<PollInfo>();
        public string GetInformation(Guid id)
        {
            var poll = Polls.Find(p => p.Id == id);
            if(poll != null)
            {
                return $"PollName: {poll.Name}\n" +
                    $"PollId: {poll.Id}\n" +
                    $"Description: {poll.Description}\n" +
                    $"Options: [{string.Join(", ", poll.Options)}]";
            }
            return "Poll not found";
        }

        public string GetResults(Guid id)
        {
            var poll = Polls.Find(p => p.Id == id);
            if(poll != null)
            {
                var results = new Dictionary<string, int>();
                foreach (var option in poll.Options)
                {
                    results[option] = poll.Votes.Values.Where(x => x == option).Count();
                }
                return $"Results: \n" +
                    $"{string.Join("\n", results.Select(kvp => kvp.Key + ": " + kvp.Value))}\n";
            }
            return "Poll not found";
        }

        public ulong GetPollChannel(Guid id)
        {
            return Polls.Find(p => p.Id == id).Channel;
        }

        public void Handle(PollCreated evt)
        {
            Polls.Add(new PollInfo
            {
                Name = evt.Name,
                Id = evt.Id,
                Description = "",
                Owner = evt.Owner,
                Channel = evt.Channel
            });
        }

        public void Handle(DescriptionAdded evt)
        {
            Polls.Find(p => p.Id == evt.Id).Description = evt.Description;
        }

        public void Handle(OptionsAdded evt)
        {
            Polls.Find(p => p.Id == evt.Id).Options.AddRange(evt.Options);
        }

        public void Handle(VotePlaced evt)
        {
            Polls.Find(p => p.Id == evt.Id).Votes[evt.User] = evt.Option;
        }

        public void Handle(VoteRemoved evt)
        {
            Polls.Find(p => p.Id == evt.Id).Votes.Remove(evt.User);
        }

        
    }
}
