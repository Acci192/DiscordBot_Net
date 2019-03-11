using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VoteBot_Discord.Models;
using VoteBot_Discord.Services;

namespace VoteBot_Discord.Modules
{
    public class VoteCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly VoteService _voteService;

        public VoteCommands(DiscordSocketClient client, VoteService voteService)
        {
            _client = client;
            _voteService = voteService;
        }

        [Command("create")]
        [Summary("Creates a new vote")]
        public async Task CreateVoteAsync([Summary("Name of the vote")] string name,
            [Summary("Options to add. Can be more than one (separated with spaces). If the option contains spaces use \"\" to surround the option")] params string[] options)
        {
            var vote = _voteService.GetVote(name);
            if(vote != null)
            {
                await ReplyAsync($"A vote with that name is currently ongoing");
                return;
            }

            var e = new VoteCreatedEvent(Guid.NewGuid(), name, Context.User.Id);
            _voteService.HandleEvent(e);
            foreach(var o in options)
            {
                if (!_voteService.GetVote(e.Id).ContainsOption(o))
                {
                    _voteService.HandleEvent(new OptionAddedEvent(e.Id, o));
                }
            }
            var v = _voteService.GetVote(e.Id);
            await ReplyAsync($"New Vote ({name}) created by {Context.User.Username}\n" +
                    $"{v.GetInformation()}.\n");
        }

        [Command("addoption")]
        [Summary("Add options to a vote")]
        public async Task AddOptions([Summary("Name of the vote")] string name,
            [Summary("Options to add. Can be more than one (separated with spaces). If the option contains spaces use \"\" to surround the option")] params string[] options)
        {
            var vote = _voteService.GetVote(name);
            if (vote == null)
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }
            // TODO Fix this bad code
            if (!vote.IsOwner(Context.User.Id))
            {
                await ReplyAsync($"{Context.User.Mention} you are not the owner of this vote");
                return;
            }

            foreach (var o in options)
            {
                if (!_voteService.GetVote(name).ContainsOption(o))
                {
                    _voteService.HandleEvent(new OptionAddedEvent(vote.GetId(), o));
                }
            }

            
            await ReplyAsync($"Options added to {name}\n{_voteService.GetVote(name).GetInformation()}");
        }

        [Command("vote")]
        [Summary("Place your vote, (PRIVATE MESSAGE)")]
        public async Task PlaceVoteAsync([Summary("Name of the vote")] string name,
            [Summary("Option to vote on")] string option)
        {
            if (Context.IsPrivate)
            {
                var vote = _voteService.GetVote(name);
                if (vote == null)
                {
                    await ReplyAsync($"There is no vote with that name");
                    return;
                }
                if (vote.UserVoted(Context.User.Id))
                {
                    await ReplyAsync($"You have already voted");
                    return;
                }
                if (!vote.ContainsOption(option))
                {
                    await ReplyAsync($"That is an invalid option");
                    return;
                }

                _voteService.HandleEvent(new MemberVotedEvent(vote.GetId(), option, Context.User.Id));
                await ReplyAsync($"Your vote on {option} has been registred");

            }
            else
            {
                await Context.Message.DeleteAsync();
                await ReplyAsync("Send me a PM, you don't want to show your vote. (Thank me later)");
            }
        }

        [Command("unvote")]
        [Summary("Remove your vote")]
        public async Task RemoveVoteAsync([Summary("Name of the vote")] string name)
        {
            var vote = _voteService.GetVote(name);
            if (vote == null)
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }
            if (vote.UserVoted(Context.User.Id))
            {
                _voteService.HandleEvent(new MemberUnvotedEvent(vote.GetId(), Context.User.Id));
                await ReplyAsync("Your vote has been removed, please vote again");
            }
        }

        [Command("end")]
        [Summary("End the vote")]
        public async Task EndVoteAsync([Summary("Name of the vote")] string name)
        {
            var vote = _voteService.GetVote(name);
            if (vote == null)
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }

            if (!vote.IsOwner(Context.User.Id))
            {
                await ReplyAsync($"{Context.User.Mention} you are not the owner of this vote");
                return;
            }

            await ReplyAsync($"{vote.GetResults()}");
            _voteService.HandleEvent(new VoteEndedEvent(vote.GetId()));
        }

        [Command("show")]
        [Summary("Show information about the vote")]
        public async Task ShowInformationAsync([Summary("Name of the vote")] string name)
        {
            var vote = _voteService.GetVote(name);
            if (vote == null)
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }

            await ReplyAsync(vote.GetInformation());
        }

        [Command("adddescription")]
        [Summary("Add a description to the vote")]
        public async Task AddDescriptionAsync([Summary("Name of the vote")] string name,
            [Remainder][Summary("Description to add to the vote")] string description)
        {
            var vote = _voteService.GetVote(name);
            if (vote == null)
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }

            _voteService.HandleEvent(new DescriptionAddedEvent(vote.GetId(), description));
            vote = _voteService.GetVote(name);
            await ReplyAsync($"Vote has been updated\n" +
                $"{vote.GetInformation()}");
        }

        [Command("addtime")]
        [Summary("Add time to the vote")]
        public async Task AddTimeAsync([Summary("Name of the vote")] string name,
            [Summary("Minutes to add")] int minutes)
        {
            var vote = _voteService.GetVote(name);
            if (vote == null)
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }

            if (!vote.IsOwner(Context.User.Id))
            {
                await ReplyAsync($"{Context.User.Mention} you are not the owner of this vote");
                return;
            }

            _voteService.HandleEvent(new TimeAddedEvent(vote.GetId(), minutes));
            vote = _voteService.GetVote(name);
            await ReplyAsync($"Vote has been updated\n" +
                $"{vote.GetInformation()}");
        }
    }
}
