﻿using System;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Ram.Commands
{
    public class Core : BaseCommandModule
    {
        private const ulong UserIdFilib = 275704530665078786;

        [Command("faggot"), Aliases("f"), Description("Nesama, Nesama, Filib ")]
        public async Task Faggot(CommandContext ctx)
        {
            var discordUser = await ctx.Client.GetUserAsync(UserIdFilib);

            await ctx.RespondAsync($"{discordUser.Mention}");
            await ctx.RespondWithFileAsync("Resources/ram_faggot.gif");
        }

        [Command("startai")]
        public async Task Ki(CommandContext ctx)
        {
            for (; ; )
            {
                var sInput = Console.ReadLine();

                if(sInput == string.Empty) continue;

                await ctx.RespondAsync($"{sInput}");

            }
        }
    }
}