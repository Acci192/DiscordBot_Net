using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoteBot_Discord.Commands;
using VoteBot_Discord.Events;
using VoteBot_Discord.Exceptions;
using VoteBot_Discord.CQRS;

namespace VoteBot_Discord.Aggregates
{
    public class PollAggregate : Aggregate,
        IHandleCommand<CreatePoll>,
        IHandleCommand<AddOptions>,
        IHandleCommand<AddDescription>,
        IHandleCommand<EndPoll>,
        IHandleCommand<PlaceVote>,
        IHandleCommand<RemoveVote>,
        IApplyEvent<PollCreated>,
        IApplyEvent<PollEnded>,
        IApplyEvent<OptionsAdded>,
        IApplyEvent<VotePlaced>,
        IApplyEvent<VoteRemoved>,
        IApplyEvent<DescriptionAdded>
    {
        private bool Created = false;
        private bool Ended = false;
        private ulong Channel = 0;
        private string Name = "";
        private ulong Owner = 0;
        private List<string> Options = new List<string>();
        private List<ulong> UsersThatVoted = new List<ulong>();

        public void Apply(PollCreated evt)
        {
            Created = true;
            Channel = evt.Channel;
            Name = evt.Name;
            Owner = evt.Owner;
        }

        public void Apply(PollEnded evt)
        {
            Ended = true;   
        }

        public void Apply(OptionsAdded evt)
        {
            Options.AddRange(evt.Options);
        }

        public void Apply(VotePlaced evt)
        {
            UsersThatVoted.Add(evt.User);
        }

        public void Apply(VoteRemoved evt)
        {
            UsersThatVoted.Remove(evt.User);
        }

        public void Apply(DescriptionAdded evt)
        {
            // Nothing need to be happen (Aggregate still need to handle the event)
        }

        public IEnumerable Handle(CreatePoll c)
        {
            yield return new PollCreated
            {
                Id = c.Id,
                Channel = c.Channel,
                Name = c.Name,
                Owner = c.Owner
            };
        }

        public IEnumerable Handle(AddOptions c)
        {
            if (!Created)
                throw new PollNotCreated();
            if (c.User != Owner)
                throw new UnauthorizedUser();
            if (Ended)
                throw new PollAlreadyEnded();

            var uniqueOptions = c.Options.Distinct().Except(Options).ToList();

            if (uniqueOptions.Count == 0)
                throw new NoOptionAdded();

            yield return new OptionsAdded
            {
                Id = c.Id,
                Options = uniqueOptions
            };
        }

        public IEnumerable Handle(AddDescription c)
        {
            if (!Created)
                throw new PollNotCreated();
            if (c.User != Owner)
                throw new UnauthorizedUser();
            if (Ended)
                throw new PollAlreadyEnded();
            yield return new DescriptionAdded
            {
                Id = c.Id,
                Description = c.Description
            };
        }

        public IEnumerable Handle(EndPoll c)
        {
            if (!Created)
                throw new PollNotCreated();
            if (Ended)
                throw new PollAlreadyEnded();
            if (Owner != c.User)
                throw new UnauthorizedUser();
            yield return new PollEnded
            {
                Id = c.Id,
            };
        }

        public IEnumerable Handle(RemoveVote c)
        {
            if (!Created)
                throw new PollNotCreated();
            if (Ended)
                throw new PollAlreadyEnded();
            if(!UsersThatVoted.Contains(c.User))
                throw new UserHasNotVoted();
            yield return new VoteRemoved
            {
                Id = c.Id,
                User = c.User
            };
        }

        public IEnumerable Handle(PlaceVote c)
        {
            if (!Created)
                throw new PollNotCreated();
            if(Ended)
                throw new PollAlreadyEnded();
            if(!Options.Contains(c.Option))
                throw new InvalidOption();
            if(UsersThatVoted.Contains(c.User))
                throw new UserAlreadyVoted();
            yield return new VotePlaced
            {
                Id = c.Id,
                User = c.User,
                Option = c.Option
            };
        }
    }
}
