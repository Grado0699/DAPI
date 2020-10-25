using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Ram.Commands
{
    public class Core : BaseCommandModule
    {
        private const ulong UserIdFilib = 275704530665078786;

        [Command("fg")]
        public async Task Faggot(CommandContext ctx)
        {
            var discordUser = await ctx.Client.GetUserAsync(UserIdFilib);

            await ctx.RespondAsync($"{discordUser.Mention}");
            await ctx.RespondWithFileAsync("Ressources/ram_faggot.gif");
        }
    }
}