using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gengbleng.Backend
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
        ///     Connects to a specified channel and plays an audiofile.
        /// </summary>
        /// <param name="soundFile"></param>
        /// <param name="voiceChannel"></param>
        /// <returns></returns>
        public async Task PlaySoundFileAsync(string soundFile, DiscordChannel voiceChannel)
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

            var Streamer = new ProcessStartInfo();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _logger.Log("Application is running on Windows.", LogLevel.Debug);

                if (!File.Exists("ffmpeg.exe"))
                {
                    throw new FileNotFoundException("ffmpeg.exe is missing.");
                }

                Streamer = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{soundFile}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                _logger.Log("Initialized streamer for Windows successfully.", LogLevel.Info);
            }
            else
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    _logger.Log("Application is running on Linux.", LogLevel.Debug);

                    Streamer = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{soundFile}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    _logger.Log("Initialized streamer for Linux successfully.", LogLevel.Info);
                }
                else
                {
                    throw new PlatformNotSupportedException("Application is running on an unsupported OS.");
                }
            }

            await VoiceConnection.SendSpeakingAsync(true);

            var ffmpeg = Process.Start(Streamer);
            var ffout = ffmpeg.StandardOutput.BaseStream;
            var txStream = VoiceConnection.GetTransmitStream();

            await ffout.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await VoiceConnection.WaitForPlaybackFinishAsync();

            _logger.Log("Playback finished successfully.", LogLevel.Info);

            VoiceConnection.Disconnect();

            _logger.Log("Disconnected from channel successfully.", LogLevel.Info);
        }
    }
}
