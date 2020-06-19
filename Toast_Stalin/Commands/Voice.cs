using Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using System.Threading.Tasks;

namespace Toast_Stalin.Commands
{
    public class Voice : BaseCommandModule
    {
        [Command("supreme"), Aliases("s"), Description("Slava!")]
        public async Task Supreme(CommandContext ctx)
        {
            const string soundFile = "Ressources/Supreme.mp3";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("supreme+"), Aliases("s+"), Description("Bogatyye!")]
        public async Task SupremePlus(CommandContext ctx)
        {
            const string soundFile = "Ressources/SupremePlus.mp3";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("exasupreme"), Aliases("es"), Description("Moshchnost'!")]
        public async Task ExaSupreme(CommandContext ctx)
        {
            const string soundFile = "Ressources/ExaSupreme.mp3";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("boris"), Aliases("b"), Description("Moya lyubimaya muzyka, Boris!")]
        public async Task Hardbass(CommandContext ctx)
        {
            const string soundFile = "Ressources/Boris.mp3";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("slap"), Aliases("ls"), Description("Шлепок!")]
        public async Task SlapLow(CommandContext ctx)
        {
            const string soundFile = "Ressources/Slap_low.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("slapper"), Aliases("ms"), Description("Племянник!")]
        public async Task SlapMedium(CommandContext ctx)
        {
            const string soundFile = "Ressources/Slap_medium.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("slappest"), Aliases("hs"), Description("Слаппест!")]
        public async Task SlapHard(CommandContext ctx)
        {
            const string soundFile = "Ressources/Slap_hard.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("megaslap"), Aliases("ms"), Description("Мегаслап!")]
        public async Task SlapAll(CommandContext ctx)
        {
            const string soundFile = "Ressources/Slap_all.ogg";

            var audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("leave"), Aliases("l"), Description("Pokinut' kanal!")]
        public async Task LeaveChannel(CommandContext ctx)
        {
            var VoiceNextExt = ctx.Client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(ctx.Guild);

            if (VoiceConnection == null)
            {
                await ctx.RespondAsync($"There is currently no voice connection up. Into Goulag with you!");
                return;
            }

            VoiceConnection.Disconnect();
        }
    }
}
