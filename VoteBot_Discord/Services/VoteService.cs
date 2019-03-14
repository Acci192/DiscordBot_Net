using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using VoteBot_Discord.Aggregates;
using VoteBot_Discord.Commands;
using VoteBot_Discord.CQRS;
using VoteBot_Discord.ReadModels;

namespace VoteBot_Discord.Services
{
    public class VoteService
    {
        public MessageDispatcher Dispatcher;
        public IActivePollQueries ActivePollQueries;
        public IPollInformationQueries PollInformationQueries;

        public VoteService()
        {
            Dispatcher = new MessageDispatcher(new InMemoryEventStore());
            Dispatcher.ScanInstance(new PollAggregate());

            ActivePollQueries = new ActivePoll();
            Dispatcher.ScanInstance(ActivePollQueries);

            PollInformationQueries = new PollInformation();
            Dispatcher.ScanInstance(PollInformationQueries);
        }

        public void test(CreatePoll cmd)
        {
            Dispatcher.SendCommand(cmd);
        }

        //public void HandleEvent(Event evt)
        //{
        //    Vote vote;
        //    switch (evt.Type)
        //    {
        //        case EventType.VotedCreated:
        //            var voteCreatedEvent = evt as VoteCreatedEvent;
        //            Console.WriteLine(JsonConvert.SerializeObject(voteCreatedEvent));
        //            Votes.Add(new Vote(evt as VoteCreatedEvent));
        //            break;
        //        case EventType.VoteEnded:
        //            var voteEndedEvent = evt as VoteEndedEvent;
        //            Console.WriteLine(JsonConvert.SerializeObject(voteEndedEvent));
        //            vote = Votes.Find(v => v.Id == voteEndedEvent.Id);
        //            Votes.Remove(vote);
        //            break;
        //        case EventType.OptionAdded:
        //        case EventType.MemberVoted:
        //        case EventType.MemberUnvoted:
        //        case EventType.TimeAdded:
        //        case EventType.DescriptionAdded:
        //            Console.WriteLine(JsonConvert.SerializeObject(evt));
        //            vote = Votes.Find(v => v.Id == evt.Id);
        //            vote.HandleEvent(evt);
        //            break;
        //    }
        //}
    }
}
