﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Backend;

public class AudioStreamer : IAudioStreamer {
    private readonly DiscordClient _client;
    private readonly ILogger _logger = new LoggerFactory().AddSerilog().CreateLogger<EventsClient>();

    public AudioStreamer(DiscordClient client) {
        _client = client;
    }

    /// <summary>
    ///     Connects to the a channel and plays a sound-file.
    /// </summary>
    public async Task PlaySoundFileAsync(string soundFile, DiscordChannel voiceChannel, string volume = "100") {
        var voiceNextExt = _client.GetVoiceNext();

        var guild = _client.Guilds.Values.FirstOrDefault(x => x.Id == ConfigLoader.GuildId);
        var voiceConnection = voiceNextExt.GetConnection(guild);

        if (voiceConnection != null) {
            _logger.LogWarning("An old connection was still up, successfully closed old one.");

            voiceConnection.Disconnect();
        }

        voiceConnection ??= await voiceNextExt.ConnectAsync(voiceChannel);

        _logger.LogInformation("Connected to channel successfully.");

        var streamer = new ProcessStartInfo {
            FileName = "ffmpeg",
            Arguments = $"""-i "{soundFile}" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol {volume}""",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        _logger.LogInformation("Initialized streamer successfully.");

        await voiceConnection.SendSpeakingAsync();

        var ffmpeg = Process.Start(streamer);
        var baseStream = ffmpeg?.StandardOutput.BaseStream;
        var txStream = voiceConnection.GetTransmitSink();

        await baseStream.CopyToAsync(txStream);
        await txStream.FlushAsync();
        await voiceConnection.WaitForPlaybackFinishAsync();

        _logger.LogInformation("Playback finished successfully.");

        await voiceConnection.SendSpeakingAsync(false);
        voiceConnection.Disconnect();

        _logger.LogInformation("Disconnected from channel successfully.");
    }
}