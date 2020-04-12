using DSharpPlus;
using DSharpPlus.VoiceNext;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gengbleng.Backend
{
    public class Streamer
    {      
        private readonly DiscordClient _client;
        private readonly Logger _logger;
        public Streamer(DiscordClient client)
        {
            _client = client;
            _logger = new Logger(client);
        }

        /// <summary>
        ///     Plays a random soundfile.
        /// </summary>
        /// <returns></returns>
        public async Task PlayRandomSoundFile()
        {
            var Guild = _client.Guilds.Values.Where(x => x.Id == ConfigLoader.GuildId).FirstOrDefault();

            if (Guild == null)
            {
                throw new ArgumentNullException("The specified Guild was not found. Check the 'Guild' parameter in the configuration file.");
            }

            var MembersWithVoiceStateUp = Guild.Members.Values.Where(x => x.VoiceState != null && x.VoiceState.Channel != null && x.VoiceState.Channel.GuildId == Guild.Id && !x.IsBot && x.VoiceState.Channel.Id != x.Guild.AfkChannel.Id).ToList();

            if (MembersWithVoiceStateUp == null)
            {
                throw new ArgumentNullException("List of users with voicestate 'up' is null.");
            }

            if (MembersWithVoiceStateUp.Count == 0)
            {
                _logger.Log("There are currently no users with voicestate 'up'.", LogLevel.Warning);
                return;
            }

            var RandomNumber = new Random();
            var RandomMember = MembersWithVoiceStateUp[RandomNumber.Next(0, MembersWithVoiceStateUp.Count - 1)];

            _logger.Log("Retrieved a random member successfully.", LogLevel.Debug);

            var VoiceNextExt = _client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(Guild);

            if (VoiceConnection != null)
            {
                VoiceConnection.Disconnect();
                _logger.Log("An old connection was still up, successfully closed old one.", LogLevel.Warning);
            }

            _logger.Log("Voice connection retrieved successfully.", LogLevel.Debug);

            var SoundFiles = Directory.GetFiles(@"Ressources\", "*.ogg");
            var SoundFile = SoundFiles[RandomNumber.Next(0, SoundFiles.Length -1 )];

            if (!File.Exists(SoundFile))
            {
                throw new FileNotFoundException("Either image or soundfile is missing.");
            }

            var StreamProcess = new ProcessStartInfo();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _logger.Log("Application is running on Windows.", LogLevel.Debug);

                if (!File.Exists("ffmpeg.exe"))
                {
                    throw new FileNotFoundException("ffmpeg.exe is missing.");
                }

                StreamProcess = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{SoundFile}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
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

                    StreamProcess = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{SoundFile}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
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

            if(VoiceConnection == null)
            {
                VoiceConnection = await VoiceNextExt.ConnectAsync(RandomMember.VoiceState.Channel);
            }

            _logger.Log("Connected to channel successfully.", LogLevel.Info);

            await VoiceConnection.SendSpeakingAsync(true);

            var ffmpeg = Process.Start(StreamProcess);
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
