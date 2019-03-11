using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using VoteBot_Discord.Models;

namespace VoteBot_Discord.Services
{
    public class VoteService
    {
        private List<Vote> Votes;

        public VoteService()
        {
            Votes = new List<Vote>();
        }

        public void HandleEvent(Event evt)
        {
            Vote vote;
            switch (evt.Type)
            {
                case EventType.VotedCreated:
                    var voteCreatedEvent = evt as VoteCreatedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(voteCreatedEvent));
                    Votes.Add(new Vote(evt as VoteCreatedEvent));
                    break;
                case EventType.VoteEnded:
                    var voteEndedEvent = evt as VoteEndedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(voteEndedEvent));
                    vote = Votes.Find(v => v.Id == voteEndedEvent.Id);
                    Votes.Remove(vote);
                    break;
                case EventType.OptionAdded:
                    var optionAddedEvent = evt as OptionAddedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(optionAddedEvent));
                    vote = Votes.Find(v => v.Id == optionAddedEvent.Id);
                    vote.HandleEvent(optionAddedEvent);
                    break;
                case EventType.MemberVoted:
                    var memberVotedEvent = evt as MemberVotedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(memberVotedEvent));
                    vote = Votes.Find(v => v.Id == memberVotedEvent.Id);
                    vote.HandleEvent(memberVotedEvent);
                    break;
                case EventType.MemberUnvoted:
                    var memberUnvotedEvent = evt as MemberUnvotedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(memberUnvotedEvent));
                    vote = Votes.Find(v => v.Id == memberUnvotedEvent.Id);
                    vote.HandleEvent(memberUnvotedEvent);
                    break;
                case EventType.TimeAdded:
                    var timeAddedEvent = evt as TimeAddedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(timeAddedEvent));
                    vote = Votes.Find(v => v.Id == timeAddedEvent.Id);
                    vote.HandleEvent(timeAddedEvent);
                    break;
                case EventType.DescriptionAdded:
                    var descriptionAddedEvent = evt as DescriptionAddedEvent;
                    Console.WriteLine(JsonConvert.SerializeObject(descriptionAddedEvent));
                    vote = Votes.Find(v => v.Id == descriptionAddedEvent.Id);
                    vote.HandleEvent(descriptionAddedEvent);
                    break;
            }
        }

        public IVote GetVote(Guid id)
        {
            return Votes.Find(v => v.Id == id);
            // TODO if not in memory, get vote from database
        }

        public IVote GetVote(string name)
        {
            return Votes.Find(v => v.Name == name);
        }
    }
}
