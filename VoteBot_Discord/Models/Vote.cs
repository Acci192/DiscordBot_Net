using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class Vote : IVote
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public ulong Owner { get; private set; }
        public string Description { get; private set; }
        public List<string> Options { get; private set; } = new List<string>();
        public Dictionary<ulong, string> Votes { get; private set; } = new Dictionary<ulong, string>();
        public DateTime EndTime { get; private set; }

        public Vote(List<Event> eventStream)
        {
            if (eventStream.Count == 0 || !(eventStream.First() is VoteCreatedEvent))
            {
                throw new ArgumentException("The provided eventstream is invalid");
            }
            var evt = eventStream.First() as VoteCreatedEvent;
            eventStream.RemoveAt(0);
            Id = evt.Id;
            Name = evt.Name;
            Owner = evt.Owner;
            var t = DateTime.Now;
            EndTime = new DateTime(t.Year, t.Month, t.Day, t.Hour + 1, t.Minute, 0, 0, t.Kind);

            foreach(var e in eventStream)
            {
                HandleEvent(e);
            }
        }

        public Vote(VoteCreatedEvent evt)
        {
            Id = evt.Id;
            Name = evt.Name;
            Owner = evt.Owner;
            var t = DateTime.Now;
            EndTime = new DateTime(t.Year, t.Month, t.Day, t.Hour + 1, t.Minute, 0, 0, t.Kind);
        }

        public void HandleEvent(Event evt)
        {
            switch (evt.Type)
            {
                case EventType.OptionAdded:
                    var optionAddedEvent = evt as OptionAddedEvent;
                    Options.Add(optionAddedEvent.Option);
                    break;
                case EventType.MemberVoted:
                    var memberVotedEvent = evt as MemberVotedEvent;
                    Votes[memberVotedEvent.Member] = memberVotedEvent.Option;
                    break;
                case EventType.MemberUnvoted:
                    var memberUnvotedEvent = evt as MemberUnvotedEvent;
                    Votes.Remove(memberUnvotedEvent.Member);
                    break;
                case EventType.TimeAdded:
                    var timeAddedEvent = evt as TimeAddedEvent;
                    EndTime = EndTime.AddMinutes(timeAddedEvent.MinutesAdded);
                    break;
                case EventType.DescriptionAdded:
                    var descriptionAddedEvent = evt as DescriptionAddedEvent;
                    Description = descriptionAddedEvent.Description;
                    break;
                default:
                    break;
            }
        }

        private string ShowOptions()
        {
            return $"The current options in {Id} are: [{string.Join(", ", Options)}]";
        }

        public string GetResults()
        {
            // TODO Format Results in a better way
            var results = new Dictionary<string, int>();
            foreach (var option in Options)
            {
                results[option] = Votes.Values.Where(x => x == option).Count();
            }
            return $"Results:\n{string.Join("\n", results.Select(kvp => kvp.Key + ": " + kvp.Value))}";
        }

        public string GetInformation()
        {
            // TODO Format Information in a better way
            var info = new StringBuilder($"Name: {Id}\n");
            if (!string.IsNullOrWhiteSpace(Description))
            {
                info.Append($"Description: {Description}\n");
            }
            info.Append($"{ShowOptions()}\nIt will end at {EndTime.ToShortTimeString()}");
            return info.ToString();
        }

        public Guid GetId()
        {
            return Id;
        }

        public bool ContainsOption(string option)
        {
            return Options.Contains(option);
        }

        public bool UserVoted(ulong user)
        {
            return Votes.ContainsKey(user);
        }
        public bool IsOwner(ulong user)
        {
            return Owner == user;
        }
    }
}
