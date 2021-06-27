using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Ram.Commands {
    public class Core : BaseCommandModule {
        private const ulong UserIdFilib = 275704530665078786;

        [Command("faggot"), Aliases("f"), Description("Nesama, Nesama, Filib ")]
        public async Task Faggot(CommandContext ctx) {
            var discordUser = await ctx.Client.GetUserAsync(UserIdFilib);

            await using var fs = new FileStream("Resources/ram_faggot.gif", FileMode.Open, FileAccess.Read);
            await new DiscordMessageBuilder().WithContent(discordUser.Mention).WithFiles(new Dictionary<string, Stream> {{"Resources/ram_faggot.gif", fs}}).SendAsync(ctx.Channel);
        }

        [Command("startai")]
        public async Task Ki(CommandContext ctx) {
            for (;;) {
                var sInput = Console.ReadLine();

                if (sInput == string.Empty)
                    continue;

                await ctx.RespondAsync($"{sInput}");
            }
        }
    }
}