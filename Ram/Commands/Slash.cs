using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Ram.Commands;

public class Slash : ApplicationCommandModule {
    [SlashCommand("test", "Test")]
    public async Task TestCommand(InteractionContext ctx) {
        // Logger.LogDebug($"Init: {nameof(Faggot)}");

        var discordUser = await ctx.Client.GetUserAsync(275704530665078786);

        await using var fileStream = new FileStream("Resources/ram_faggot.gif", FileMode.Open, FileAccess.Read);
        await new DiscordMessageBuilder().WithContent(discordUser.Mention).AddFile(fileStream).SendAsync(ctx.Channel);
    }
}