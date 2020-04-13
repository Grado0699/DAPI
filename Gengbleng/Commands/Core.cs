using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using Gengbleng.Backend;
using System;
using System.Threading.Tasks;

namespace Gengbleng.Commands
{
    public class Core : BaseCommandModule
    {
        public static double TimerSpan = 1800000;
        public static bool EnableTimer = true;

        [Command("settimer"), Aliases("-t"), Description("Enable/disable the timer.")]
        public async Task SetTimer(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            if (EnableTimer)
            {
                await ctx.RespondAsync($"Current status of timer is `{EnableTimer}`. Deactivate timer? [y/n]");
            }
            else
            {
                await ctx.RespondAsync($"Current status of timer is `{EnableTimer}`. Activate timer? [y/n]");
            }

            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (Message.Result.Content.ToLower().Contains("y"))
            {
                EnableTimer = !EnableTimer;
                await ctx.RespondAsync($"Timer set to `{EnableTimer}`.");
            }
            else if(Message.Result.Content.ToLower().Contains("n"))
            {
                await ctx.RespondAsync("Do nothing.");
            }
            else
            {
                await ctx.RespondAsync("Invalid input.");
                return;
            }
        }

        [Command("interval"), Aliases("-i"), Description("Set new timer interval.")]
        public async Task SetInterval(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            await ctx.RespondAsync($"Current interval is `{TimerSpan/1000d}`. Enter new interval in `x seconds`:");

            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (double.TryParse(Message.Result.Content, out TimerSpan))
            {
                TimerSpan *= 1000d;

                await ctx.RespondAsync($"Interval set to: `{TimerSpan/1000d}`.");
            }
            else
            {
                TimerSpan = 900000d;
                await ctx.RespondAsync($"Invalid input. Set interval to default value: `{TimerSpan/1000d}`.");
            }
        }

        [Command("barrel"), Aliases("b"), Description("BARREL!")]
        public async Task Barrel(CommandContext ctx)
        {
            const string soundFile = "Ressources\\E1.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel);
        }
    }
}
