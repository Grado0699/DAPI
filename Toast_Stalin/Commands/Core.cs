using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Toast_Stalin.Backend;

namespace Toast_Stalin.Commands
{
    public class Core : BaseCommandModule
    {
        [Command("hi"), Aliases("-h"), Description("Privetstvennoye slovo.")]
        public async Task Hi(CommandContext ctx)
        {
            await ctx.RespondAsync($"Privet {ctx.Member.Mention}");
        }

        [Command("goulag"), Aliases("-g"), Description("Otpravit' zlo!")]
        public async Task Goulag(CommandContext ctx, DiscordMember UserToGoulag)
        {
            if (UserToGoulag.IsBot)
            {
                await ctx.RespondAsync("You can't goulag me. Into Goulag with you!");
                UserToGoulag = ctx.Member;
            }

            if (UserToGoulag.VoiceState == null)
            {
                ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Member is not in a voicechannel.", DateTime.Now);
                return;
            }

            var Goulag = new GoulagHandler(ctx, UserToGoulag);
            await Goulag.StartGoulagAsync();
        }

        [Command("happybirthday"), Aliases("-hb"), Description("Желаю тебе счастливого дня рождения, товарищ!")]
        public async Task HappyBirthday(CommandContext ctx, DiscordMember discordMember)
        {
            await ctx.RespondWithFileAsync("HappyBirthday.jpg", discordMember.Mention);
        }
    }
}