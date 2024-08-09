using Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;

namespace Holo.Commands;

public class Commands : BaseCommandModule {
    [Command("wakeup"), Aliases("w"), Description("Wake somebody up")]
    public async Task WakeUp(CommandContext ctx) {
        const string soundFile = "Resources/WakeUp.ogg";

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel);
    }

    [Command("leave"), Aliases("l"), Description("Leaves the current voice channel.")]
    public async Task LeaveChannel(CommandContext ctx) {
        var voiceNextExt = ctx.Client.GetVoiceNext();
        var voiceConnection = voiceNextExt.GetConnection(ctx.Guild);

        voiceConnection?.Disconnect();

        await Task.CompletedTask;
    }
}