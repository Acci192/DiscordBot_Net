using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using VoteBot_Discord.Models;

namespace VoteBot_Discord.Services
{
    public class TestService
    {
        private List<Vote> Votes;

        public TestService()
        {
            Votes = new List<Vote>();
        }

        public void HandleEvent(Event ev)
        {
            Vote vote;
            switch (ev.Type)
            {
                case EventType.VotedCreated:
                    var voteCreatedEvent = ev as VoteCreatedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(voteCreatedEvent));
                    Votes.Add(new Vote(voteCreatedEvent.Id, voteCreatedEvent.Owner));
                    break;
                case EventType.OptionAdded:
                    var optionAddedEvent = ev as OptionAddedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(optionAddedEvent));
                    vote = Votes.Find(v => v.Id == optionAddedEvent.Id);
                    vote.AddOption(optionAddedEvent.Option);
                    break;
                case EventType.MemberVoted:
                    var memberVotedEvent = ev as MemberVotedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(memberVotedEvent));
                    vote = Votes.Find(v => v.Id == memberVotedEvent.Id);
                    vote.PlaceVote(memberVotedEvent.Member, memberVotedEvent.Option);
                    break;
                case EventType.MemberUnvoted:
                    var memberUnvotedEvent = ev as MemberUnvotedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(memberUnvotedEvent));
                    vote = Votes.Find(v => v.Id == memberUnvotedEvent.Id);
                    vote.RemoveVote(memberUnvotedEvent.Member);
                    break;
                case EventType.VoteEnded:
                    var voteEndedEvent = ev as VoteEndedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(voteEndedEvent));
                    vote = Votes.Find(v => v.Id == voteEndedEvent.Id);
                    Votes.Remove(vote);
                    break;
                case EventType.TimeAdded:
                    var timeAddedEvent = ev as TimeAddedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(timeAddedEvent));
                    vote = Votes.Find(v => v.Id == timeAddedEvent.Id);
                    vote.AddMinutes(timeAddedEvent.MinutesAdded);
                    break;
                case EventType.DescriptionAdded:
                    var descriptionAddedEvent = ev as DescriptionAddedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(descriptionAddedEvent));
                    vote = Votes.Find(v => v.Id == descriptionAddedEvent.Id);
                    vote.UpdateDescription(descriptionAddedEvent.Description);
                    break;
                default:
                    break;
            }
        }

        public bool CanCreateVote(string id)
        {
            return !Votes.Exists(v => v.Id == id);
        }

        public bool CanAddOption(string id, string option)
        {
            return !Votes.Find(v => v.Id == id).Options.Contains(option);
        }

        public bool CanPlaceVote(string id, ulong user, string option)
        {
            var vote = Votes.Find(v => v.Id == id);
            return !vote.Votes.ContainsKey(user) && vote.Options.Contains(option);
        }

        public bool CanRemoveVote(string id, ulong user)
        {
            return Votes.Find(v => v.Id == id).Votes.ContainsKey(user);
        }

        public Vote GetVote(string id)
        {
            return Votes.Find(v => v.Id == id);
        }
    }
}
