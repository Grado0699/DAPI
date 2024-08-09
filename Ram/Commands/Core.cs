using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ram.Commands;

public class Core : BaseCommandModule {
    private const ulong UserIdFilib = 275704530665078786;

    public ILogger Logger { private get; set; }

    [Command("ff"), Aliases("f"), Description("Nesama, Nesama, Filib ")]
    public async Task Faggot(CommandContext ctx) {
        Logger.LogDebug($"Init: {nameof(Faggot)}");

        var discordUser = await ctx.Client.GetUserAsync(UserIdFilib);

        await using var fileStream = new FileStream("Resources/ram_faggot.gif", FileMode.Open, FileAccess.Read);
        await new DiscordMessageBuilder().WithContent(discordUser.Mention).AddFile(fileStream).SendAsync(ctx.Channel);
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