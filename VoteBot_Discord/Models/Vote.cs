using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoteBot_Discord.Models
{
    public class Vote
    {
        public List<string> Options { get; private set; } = new List<string>();
        public Dictionary<ulong, string> Votes { get; private set; } = new Dictionary<ulong, string>();
        public string Id { get; private set; }
        public ulong Owner { get; private set; }
        public string Description { get; private set; }
        public DateTime EndTime { get; private set; }

        public Vote(string id, ulong owner)
        {
            Id = id;
            Owner = owner;
            EndTime = DateTime.Now.AddHours(1);
        }

        public bool AddOption(string option)
        {
            if (Options.Contains(option))
            {
                return false;
            }

            Options.Add(option);
            return true;
        }

        public bool PlaceVote(ulong user, string option)
        {
            if (Votes.ContainsKey(user))
            {
                return false;
            }

            Votes[user] = option;
            return true;
        }

        public bool RemoveVote(ulong user)
        {
            return Votes.Remove(user);
        }

        public string ShowOptions()
        {
            return $"The current options in {Id} are: [{string.Join(", ", Options)}]";
        }

        public string ShowResults()
        {
            var results = new Dictionary<string, int>();
            foreach (var option in Options)
            {
                results[option] = Votes.Values.Where(x => x == option).Count();
            }
            return $"Results:\n{string.Join("\n", results.Select(kvp => kvp.Key + ": " + kvp.Value))}";
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void AddMinutes(int minutes)
        {
            EndTime = EndTime.AddMinutes(minutes);
        }

        public string ShowInformation()
        {
            var info = new StringBuilder($"Name: {Id}\n");
            if (!string.IsNullOrWhiteSpace(Description))
            {
                info.Append($"Description: {Description}\n");
            }
            info.Append($"{ShowOptions()}\nIt will end at {EndTime.ToShortTimeString()}");
            return info.ToString();
        }
    }
}
