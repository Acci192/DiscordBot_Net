using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoteBot_Discord.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;

        public HelpModule(CommandService commands)
        {
            _commands = commands;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            // TODO Load prefix from config
            var prefix = "!";
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };
            string description = null;
            foreach (var module in _commands.Modules)
            {
                
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                builder.AddField(x =>
                {
                    x.Name = "Commands";
                    x.Value = description;
                    x.IsInline = false;
                });
            }
            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        public async Task HelpAsync(string command)
        {
            var result = _commands.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"There are no such commands [{command}]");
                return;
            }

            string prefix = "!";
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Information about {command}"
            };

            foreach(var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = $"{prefix}{cmd.Name} <{string.Join("> <", cmd.Parameters.Select(p => p.Name))}>";
                    x.Value = $"Parameters: \n{string.Join("", cmd.Parameters.Select(p => $"{p.Name} - {p.Summary}\n"))}\n" +
                    $"Summary: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
