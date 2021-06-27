using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using Happy.Backend;
using System.Threading.Tasks;

namespace Happy.Commands {
    public class Core : BaseCommandModule {
        [Command("test")]
        public async Task Test(CommandContext ctx) {
            await ctx.RespondAsync($"Hi {ctx.Member.Mention}");
        }

        [Command("join"), Aliases("j"), Description("Join voice channel.")]
        public async Task JoinChannel(CommandContext ctx) {
            var streamer = new Streamer(ctx.Client);
            await streamer.PlayDefaultSoundFile(ctx.Member.VoiceState.Channel);
        }

        [Command("leave"), Aliases("l"), Description("Leave voice channel.")]
        public async Task LeaveChannel(CommandContext ctx) {
            var voiceNextExt = ctx.Client.GetVoiceNext();
            var voiceConnection = voiceNextExt.GetConnection(ctx.Guild);

            voiceConnection?.Disconnect();
        }
    }
}