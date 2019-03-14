using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoteBot_Discord.Commands;
using VoteBot_Discord.Exceptions;
using VoteBot_Discord.CQRS;
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
        [Alias("c")]
        [Summary("Creates a new vote")]
        public async Task CreateVoteAsync([Summary("Name of the vote")] string name,
            [Summary("Options to add. Can be more than one (separated with spaces). If the option contains spaces use \"\" to surround the option")] params string[] options)
        {           
            if (_voteService.ActivePollQueries.GetPollId(name) == Guid.Empty)
            {
                var id = Guid.NewGuid();
                _voteService.SendCommand(new CreatePoll
                {
                    Id = id,
                    Channel = Context.Channel.Id,
                    Owner = Context.User.Id,
                    Name = name
                });

                if(options.Length > 0)
                    _voteService.SendCommand(new AddOptions
                    {
                        Id = id,
                        User = Context.User.Id,
                        Options = options.ToList()
                    });
                await ReplyAsync(_voteService.PollInformationQueries.GetInformation(id));
            }
            else
            {
                await ReplyAsync("A vote with that name is already active");
            }
        }

        [Command("addoption")]
        [Alias("ao")]
        [Summary("Add options to a vote")]
        public async Task AddOptions([Summary("Name of the vote")] string name,
            [Summary("Options to add. Can be more than one (separated with spaces). If the option contains spaces use \"\" to surround the option")] params string[] options)
        {
            var pollId = _voteService.ActivePollQueries.GetPollId(name);
            if(pollId != Guid.Empty)
            {
                try
                {
                    _voteService.SendCommand(new AddOptions
                    {
                        Id = pollId,
                        User = Context.User.Id,
                        Options = options.ToList()
                    });
                    await ReplyAsync(_voteService.PollInformationQueries.GetInformation(pollId));
                }
                catch (NoOptionAdded e)
                {
                    await ReplyAsync("No unique option was added");
                }
            }
            else
            {
                await ReplyAsync($"There is no active vote with that name");
            }
        }

        [Command("vote")]
        [Alias("v")]
        [Summary("Place your vote, (PRIVATE MESSAGE)")]
        public async Task PlaceVoteAsync([Summary("Name of the vote")] string name,
            [Summary("Option to vote on")] string option)
        {
            if (Context.IsPrivate)
            {
                var pollId = _voteService.ActivePollQueries.GetPollId(name);
                if (pollId != Guid.Empty)
                {
                    try
                    {
                        _voteService.SendCommand(new PlaceVote
                        {
                            Id = pollId,
                            User = Context.User.Id,
                            Option = option
                        });
                        await ReplyAsync($"Your vote on <{option}> has been registered. Thank you for your participation!");
                    }
                    catch (UserAlreadyVoted)
                    {
                        await ReplyAsync("You have already voted");
                    }
                    catch (InvalidOption)
                    {
                        await ReplyAsync("No such option available");
                    }
                }
                else
                {
                    await ReplyAsync($"There is no active vote with that name");
                }
            }
            else
            {
                await Context.Message.DeleteAsync();
                await ReplyAsync($"{Context.User.Mention} Send me a PM, you don't want to show your vote. (Thank me later)");
            }
        }

        [Command("unvote")]
        [Alias("uv")]
        [Summary("Remove your vote")]
        public async Task RemoveVoteAsync([Summary("Name of the vote")] string name)
        {
            var pollId = _voteService.ActivePollQueries.GetPollId(name);
            if (pollId != Guid.Empty)
            {
                try
                {
                    _voteService.SendCommand(new RemoveVote
                    {
                        Id = pollId,
                        User = Context.User.Id
                    });
                    await ReplyAsync($"Your vote has been unregistered. Please place another vote!");
                }
                catch (UserHasNotVoted)
                {
                    await ReplyAsync("You have not placed a vote yet");
                }
            }
            else
            {
                await ReplyAsync($"There is no active vote with that name");
            }
        }

        [Command("end")]
        [Alias("e")]
        [Summary("End the vote")]
        public async Task EndVoteAsync([Summary("Name of the vote")] string name)
        {
            var pollId = _voteService.ActivePollQueries.GetPollId(name);
            if (pollId != Guid.Empty)
            {
                try
                {
                    _voteService.SendCommand(new EndPoll
                    {
                        Id = pollId,
                        User = Context.User.Id
                    });
                    if (_client.GetChannel(_voteService.PollInformationQueries.GetPollChannel(pollId)) is SocketTextChannel chan)
                    {
                        await chan.SendMessageAsync($"{_voteService.PollInformationQueries.GetResults(pollId)}");
                    }
                }
                catch (UnauthorizedUser)
                {
                    await ReplyAsync($"{Context.User.Mention} you are not the owner of this vote. Please be patient!");
                }
            }
            else
            {
                await ReplyAsync($"There is no active vote with that name");
            }
        }

        [Command("show")]
        [Summary("Show information about the vote")]
        public async Task ShowInformationAsync([Summary("Name of the vote")] string name)
        {
            var pollId = _voteService.ActivePollQueries.GetPollId(name);
            if(pollId != Guid.Empty)
            {
                await ReplyAsync(_voteService.PollInformationQueries.GetInformation(pollId));
            }
            else
            {
                await ReplyAsync($"There is no active vote with that name");
            }
        }

        [Command("adddescription")]
        [Alias("ad")]
        [Summary("Add a description to the vote")]
        public async Task AddDescriptionAsync([Summary("Name of the vote")] string name,
            [Remainder][Summary("Description to add to the vote")] string description)
        {
            var pollId = _voteService.ActivePollQueries.GetPollId(name);
            if (pollId != Guid.Empty)
            {
                try
                {
                    _voteService.SendCommand(new AddDescription
                    {
                        Id = pollId,
                        User = Context.User.Id,
                        Description = description
                    });
                    await ReplyAsync(_voteService.PollInformationQueries.GetInformation(pollId));
                }
                catch (UnauthorizedUser)
                {
                    await ReplyAsync($"{Context.User.Mention} you are not the owner of this vote.");
                }
            }
            else
            {
                await ReplyAsync($"There is no active vote with that name");
            }
        }
    }
}
