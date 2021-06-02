using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using Toast_Stalin.Backend;

namespace Toast_Stalin.Commands
{
    public class Core : BaseCommandModule
    {
        [Command("hi"), Aliases("h"), Description("Privetstvennoye slovo.")]
        public async Task Hi(CommandContext ctx)
        {
            await ctx.RespondAsync($"Privet {ctx.Member.Mention}");
        }

        [Command("gulag"), Aliases("g"), Description("Otpravit' zlo!")]
        public async Task Goulag(CommandContext ctx, DiscordMember userToGoulag)
        {
            if (userToGoulag.IsBot)
            {
                await ctx.RespondAsync("You can't gulag me. Into Gulag with you!");
                userToGoulag = ctx.Member;
            }

            if (userToGoulag.VoiceState == null)
            {
                await ctx.RespondAsync("This user does not has an active voicestate. Into Gulag with you!");
                userToGoulag = ctx.Member;
            }

            var gulagHandler = new GulagHandler(ctx, userToGoulag);
            await gulagHandler.StartGulagAsync();
        }

        [Command("happybirthday"), Aliases("hb"), Description("Желаю тебе счастливого дня рождения, товарищ!")]
        public async Task HappyBirthday(CommandContext ctx, DiscordMember discordMember)
        {
            //await ctx.RespondWithFileAsync("Resources/HappyBirthday.jpg", discordMember.Mention);
        }
    }
}