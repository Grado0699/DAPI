using Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gengbleng.Commands
{
    public class Core : BaseCommandModule
    {
        public static double TimerSpan = 1800000;
        public static bool EnableTimer = false;

        [Command("settimer"), Aliases("t"), Description("Enable/disable the timer.")]
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
            else if (Message.Result.Content.ToLower().Contains("n"))
            {
                await ctx.RespondAsync("Do nothing.");
            }
            else
            {
                await ctx.RespondAsync("Invalid input.");
                return;
            }
        }

        [Command("interval"), Aliases("i"), Description("Set new timer interval.")]
        public async Task SetInterval(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            await ctx.RespondAsync($"Current interval is `{TimerSpan / 1000d}`. Enter new interval in `x seconds`:");

            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (double.TryParse(Message.Result.Content, out TimerSpan))
            {
                TimerSpan *= 1000d;

                await ctx.RespondAsync($"Interval set to: `{TimerSpan / 1000d}` seconds.");
            }
            else
            {
                TimerSpan = 1800000;
                await ctx.RespondAsync($"Invalid input. Set interval to default value: `{TimerSpan / 1000d}` seconds.");
            }
        }

        [Command("barrel"), Aliases("b"), Description("BARREL!")]
        public async Task Barrel(CommandContext ctx)
        {
            const string soundFile = "Ressources/E1.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("shotgunknees"), Aliases("s"), Description("Plays the 'Shotgun-Knees' sound.")]
        public async Task ShotgunKnees(CommandContext ctx)
        {
            const string soundFile = "Ressources/ShotgunKnees.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "500");
        }

        [Command("random"), Aliases("r"), Description("Plays a random soundfile.")]
        public async Task PlayRandomSoundFile(CommandContext ctx)
        {
            var random = new Random();

            var allSoundFiles = Directory.GetFiles(@"Ressources/", "*.ogg");
            var randomSoundFile = allSoundFiles[random.Next(0, allSoundFiles.Length - 1)];

            if (!File.Exists(randomSoundFile))
            {
                throw new FileNotFoundException("Either image or soundfile is missing.");
            }

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(randomSoundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("shanty"), Aliases("sh"), Description("Plays a random shanty.")]
        public async Task PlayShanty(CommandContext ctx)
        {
            const string soundFile = "Ressources/ShantyLeaveHerJohnny.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("yooooooooooo"), Aliases("y"), Description("Plays yooooooooooo.")]
        public async Task PlayYooooo(CommandContext ctx)
        {
            const string soundFile = "Ressources/Yooooooooooo.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }
    }
}
