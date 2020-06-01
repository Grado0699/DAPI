﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Backend
{
    public class AudioStreamer
    {
        private readonly DiscordClient _client;
        private readonly Logger _logger;

        public AudioStreamer(DiscordClient client)
        {
            _client = client;
            _logger = new Logger(client);
        }

        /// <summary>
        ///     Connects to the a channel and plays a soundfile.
        /// </summary>
        /// <param name="soundFile"></param>
        /// <param name="voiceChannel"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public async Task PlaySoundFileAsync(string soundFile, DiscordChannel voiceChannel, string volume = "100")
        {
            var VoiceNextExt = _client.GetVoiceNext();

            var Guild = _client.Guilds.Values.Where(x => x.Id == ConfigLoader.GuildId).FirstOrDefault();
            var VoiceConnection = VoiceNextExt.GetConnection(Guild);

            if (VoiceConnection != null)
            {
                VoiceConnection.Disconnect();
                _logger.Log("An old connection was still up, successfully closed old one.", LogLevel.Warning);
            }

            if (VoiceConnection == null)
            {
                VoiceConnection = await VoiceNextExt.ConnectAsync(voiceChannel);
            }

            _logger.Log("Connected to channel successfully.", LogLevel.Info);

            var Streamer = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{soundFile}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol {volume}",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            _logger.Log("Initialized streamer successfully.", LogLevel.Debug);

            await VoiceConnection.SendSpeakingAsync(true);

            var ffmpeg = Process.Start(Streamer);
            var ffout = ffmpeg.StandardOutput.BaseStream;
            var txStream = VoiceConnection.GetTransmitStream();

            await ffout.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await VoiceConnection.WaitForPlaybackFinishAsync();

            _logger.Log("Playback finished successfully.", LogLevel.Debug);

            await VoiceConnection.SendSpeakingAsync(false);
            VoiceConnection.Disconnect();

            _logger.Log("Disconnected from channel successfully.", LogLevel.Info);
        }
    }
}