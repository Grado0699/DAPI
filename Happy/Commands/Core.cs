using Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Happy.Commands
{
    public class Core : BaseCommandModule
    {
        [Command("test")]
        public async Task Test(CommandContext ctx)
        {
            await ctx.RespondAsync($"Hi {ctx.Member.Mention}");
        }
    }
}
