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
        private readonly TestService _test;

        public VoteCommands(DiscordSocketClient client, TestService test)
        {
            _client = client;
            _test = test;
        }

        [Command("create")]
        [Summary("Creates a new vote")]
        public async Task CreateVoteAsync([Summary("Name of the vote")] string name,
            [Summary("Options to add. Can be more than one (separated with spaces). If the option contains spaces use \"\" to surround the option")] params string[] options)
        {
            if (_test.CanCreateVote(name))
            {
                var e = new VoteCreatedEvent(name, Context.User.Id);
                _test.HandleEvent(e);

                foreach (var o in options)
                {
                    if (_test.CanAddOption(name, o))
                    {
                        _test.HandleEvent(new OptionAddedEvent(name, o));
                    }
                }
                var v = _test.GetVote(name);
                await ReplyAsync($"New Vote ({name}) created by {Context.User.Username}\n" +
                    $"{v.ShowOptions()}.\n" +
                    $"It will end at {v.EndTime.ToShortTimeString()}");
            }
        }

        [Command("addoption")]
        [Summary("Add options to a vote")]
        public async Task AddOptions([Summary("Name of the vote")] string name,
            [Summary("Options to add. Can be more than one (separated with spaces). If the option contains spaces use \"\" to surround the option")] params string[] options)
        {
            if (_test.CanCreateVote(name))
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }
            // TODO Fix this bad code
            var vote = _test.GetVote(name);
            if (vote.Owner != Context.User.Id)
            {
                await ReplyAsync($"{Context.User.Mention} you are not the owner of this vote");
                return;
            }

            foreach (var o in options)
            {
                if (_test.CanAddOption(name, o))
                {
                    _test.HandleEvent(new OptionAddedEvent(name, o));
                }
            }

            var v = _test.GetVote(name);
            await ReplyAsync($"Options added to {name}\n{v.ShowOptions()}");
        }

        [Command("vote")]
        [Summary("Place your vote, (PRIVATE MESSAGE)")]
        public async Task PlaceVoteAsync([Summary("Name of the vote")] string name,
            [Summary("Option to vote on")] string option)
        {
            if (Context.IsPrivate)
            {
                if (_test.CanCreateVote(name))
                {
                    await ReplyAsync($"There is no vote with that name");
                    return;
                }
                if (_test.CanPlaceVote(name, Context.User.Id, option))
                {
                    _test.HandleEvent(new MemberVotedEvent(name, option, Context.User.Id));
                    await ReplyAsync($"Your vote on {option} has been registred");
                }
                else
                {
                    await ReplyAsync($"Your vote has not been registred, either you already voted or the option is not available");
                }
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
            if (_test.CanCreateVote(name))
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }
            if (_test.CanRemoveVote(name, Context.User.Id))
            {
                _test.HandleEvent(new MemberUnvotedEvent(name, Context.User.Id));
                await ReplyAsync("Your vote has been removed, please vote again");
            }
        }

        [Command("end")]
        [Summary("End the vote")]
        public async Task EndVoteAsync([Summary("Name of the vote")] string name)
        {
            if (_test.CanCreateVote(name))
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }
            var vote = _test.GetVote(name);
            if (vote.Owner != Context.User.Id)
            {
                await ReplyAsync($"{Context.User.Mention} you are not the owner of this vote");
                return;
            }

            await ReplyAsync($"{vote.ShowResults()}");
            _test.HandleEvent(new VoteEndedEvent(name));
        }

        [Command("show")]
        [Summary("Show information about the vote")]
        public async Task ShowInformationAsync([Summary("Name of the vote")] string name)
        {
            if (_test.CanCreateVote(name))
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }

            var vote = _test.GetVote(name);
            await ReplyAsync($"Name of the Vote: {vote.Id}\n" +
                $"{vote.ShowOptions()}\n" +
                $"It will end at {vote.EndTime.ToShortTimeString()}");
        }

        [Command("adddescription")]
        [Summary("Add a description to the vote")]
        public async Task AddDescriptionAsync([Summary("Name of the vote")] string name,
            [Remainder][Summary("Description to add to the vote")] string description)
        {
            if (_test.CanCreateVote(name))
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }

            _test.HandleEvent(new DescriptionAddedEvent(name, description));
            var vote = _test.GetVote(name);
            await ReplyAsync($"Vote has been updated\n" +
                $"{vote.ShowInformation()}");
        }

        [Command("addtime")]
        [Summary("Add time to the vote")]
        public async Task AddTimeAsync([Summary("Name of the vote")] string name,
            [Summary("Minutes to add")] int minutes)
        {
            if (_test.CanCreateVote(name))
            {
                await ReplyAsync($"There is no vote with that name");
                return;
            }
            var vote = _test.GetVote(name);
            if (vote.Owner != Context.User.Id)
            {
                await ReplyAsync($"{Context.User.Mention} you are not the owner of this vote");
                return;
            }

            _test.HandleEvent(new TimeAddedEvent(name, minutes));
            vote = _test.GetVote(name);
            await ReplyAsync($"Vote has been updated\n" +
                $"{vote.ShowInformation()}");
        }
    }
}
