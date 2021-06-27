using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Bunta.Commands {
    public class Core : BaseCommandModule {
        [Command("hi")]
        public async Task Hi(CommandContext ctx) {
            await ctx.RespondAsync("Hi");
        }
    }
}