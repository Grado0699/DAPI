using Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using System.Threading.Tasks;

namespace Toast_Stalin.Commands {
    public class Voice : BaseCommandModule {
        [Command("supreme"), Aliases("s"), Description("Slava!")]
        public async Task Supreme(CommandContext ctx) {
            const string soundFile = "Resources/Supreme.mp3";

            IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("supreme+"), Aliases("s+"), Description("Bogatyye!")]
        public async Task SupremePlus(CommandContext ctx) {
            const string soundFile = "Resources/SupremePlus.mp3";

            IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("exasupreme"), Aliases("es"), Description("Moshchnost'!")]
        public async Task ExaSupreme(CommandContext ctx) {
            const string soundFile = "Resources/ExaSupreme.mp3";

            IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("boris"), Aliases("b"), Description("Moya lyubimaya muzyka, Boris!")]
        public async Task Hardbass(CommandContext ctx) {
            const string soundFile = "Resources/Boris.mp3";

            IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");
        }

        [Command("slap"), Aliases("ls"), Description("Шлепок!")]
        public async Task SlapLow(CommandContext ctx) {
            const string soundFile = "Resources/Slap_low.ogg";

            IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "100");
        }

        [Command("slapper"), Aliases("ms"), Description("Племянник!")]
        public async Task SlapMedium(CommandContext ctx) {
            const string soundFile = "Resources/Slap_medium.ogg";

            IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "100");
        }

        [Command("slappest"), Aliases("hs"), Description("Слаппест!")]
        public async Task SlapHard(CommandContext ctx) {
            const string soundFile = "Resources/Slap_hard.ogg";

            IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "100");
        }

        [Command("megaslap"), Aliases("m"), Description("Мегаслап!")]
        public async Task SlapAll(CommandContext ctx) {
            const string soundFile = "Resources/Slap_all.ogg";

            IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
            await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "100");
        }

        [Command("leave"), Aliases("l"), Description("Pokinut' kanal!")]
        public async Task LeaveChannel(CommandContext ctx) {
            var voiceNextExt = ctx.Client.GetVoiceNext();
            var voiceConnection = voiceNextExt.GetConnection(ctx.Guild);

            if (voiceConnection == null) {
                await ctx.RespondAsync("There is currently no voice connection up. Into Goulag with you!");
                return;
            }

            voiceConnection.Disconnect();
        }
    }
}