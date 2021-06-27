using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Backend {
    public class AudioStreamer : IAudioStreamer {
        private readonly DiscordClient _client;
        private readonly Logger _logger;

        public AudioStreamer(DiscordClient client) {
            _client = client;
            _logger = new Logger(client);
        }

        /// <summary>
        ///     Connects to the a channel and plays a sound-file.
        /// </summary>
        public async Task PlaySoundFileAsync(string soundFile, DiscordChannel voiceChannel, string volume = "100") {
            var voiceNextExt = _client.GetVoiceNext();

            var guild = _client.Guilds.Values.FirstOrDefault(x => x.Id == ConfigLoader.GuildId);
            var voiceConnection = voiceNextExt.GetConnection(guild);

            if (voiceConnection != null) {
                voiceConnection.Disconnect();
                _logger.Log("An old connection was still up, successfully closed old one.", LogLevel.Warning);
            }

            voiceConnection ??= await voiceNextExt.ConnectAsync(voiceChannel);

            _logger.Log("Connected to channel successfully.", LogLevel.Information);

            var streamer = new ProcessStartInfo {
                FileName = "ffmpeg", Arguments = $@"-i ""{soundFile}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol {volume}", RedirectStandardOutput = true, UseShellExecute = false
            };

            _logger.Log("Initialized streamer successfully.", LogLevel.Debug);

            await voiceConnection.SendSpeakingAsync();

            var ffmpeg = Process.Start(streamer);
            var ffout = ffmpeg?.StandardOutput.BaseStream;
            var txStream = voiceConnection.GetTransmitSink();

            await ffout.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await voiceConnection.WaitForPlaybackFinishAsync();

            _logger.Log("Playback finished successfully.", LogLevel.Debug);

            await voiceConnection.SendSpeakingAsync(false);
            voiceConnection.Disconnect();

            _logger.Log("Disconnected from channel successfully.", LogLevel.Information);
        }
    }
}