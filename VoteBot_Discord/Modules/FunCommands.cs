using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VoteBot_Discord.Services;

namespace VoteBot_Discord.Modules
{
    public class FunCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        private readonly TestService _test;

        public FunCommands(DiscordSocketClient client, TestService test)
        {
            _client = client;
            _test = test;
        }


        [Command("ping")]
        [Summary("Reply with the pingtime")]
        public async Task PingAsync()
        {
            var pingTime = $"{Context.User.Mention}Ping for the bot is: {Context.Client.Latency}ms";

            await (await Context.User.GetOrCreateDMChannelAsync()).SendMessageAsync("Hey");
            await ReplyAsync(pingTime);
        }
    }
}
