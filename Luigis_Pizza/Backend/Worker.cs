using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace Luigis_Pizza.Backend
{
    public class Worker
    {
        private static ProcessStartInfo StreamProcess { get; set; } = null;

        private const string SoundFile = "Ressources/pizza.mp3";
        private const string PizzaImage = "Ressources/pizza.jpg";

        public async Task StartWorker(DiscordClient Client)
        {
            DiscordGuild Guild = Client.Guilds.Values.Where(x => x.Id == ConfigLoader.GuildId).ToList().FirstOrDefault();

            if(Guild == null)
            {
                throw new ArgumentNullException("The specified Guild was not found. Check the Guild parameter in the configuration file.");
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Retrieved guild successfully.", DateTime.Now);

            List<DiscordMember> MembersVoiceStateUp = Guild.Members.Values.Where(x => x.VoiceState != null && x.VoiceState.Channel != null && x.VoiceState.Channel.GuildId == Guild.Id && !x.IsBot).ToList();

            if(MembersVoiceStateUp == null)
            {
                throw new ArgumentNullException("List of users with voicestate 'up' is null.");
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Retrieved users with active voicestate successfully.", DateTime.Now);

            if (MembersVoiceStateUp.Count == 0)
            {
                Client.DebugLogger.LogMessage(LogLevel.Warning, Assembly.GetExecutingAssembly().GetName().Name, "There are currently no users with voicestate 'up'.", DateTime.Now);
                return;
            }

            var RandomNumber = new Random();
            DiscordMember RandomMember = MembersVoiceStateUp[RandomNumber.Next(0, MembersVoiceStateUp.Count - 1)];

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Retrieved a random member successfully.", DateTime.Now);

            var VoiceNextExt = Client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(Guild);

            if(VoiceConnection != null)
            {
                VoiceConnection.Disconnect();
                Client.DebugLogger.LogMessage(LogLevel.Warning, Assembly.GetExecutingAssembly().GetName().Name, "An old connection was still up, successfully closed old one.", DateTime.Now);
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Voice connection retrieved successfully.", DateTime.Now);

            if (!File.Exists(SoundFile) || !File.Exists(PizzaImage))
            {
                throw new FileNotFoundException("Either image or soundfile is missing.");
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Image and soundfile are present.", DateTime.Now);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Application is running on Windows.", DateTime.Now);

                if (!File.Exists("ffmpeg.exe"))
                {
                    throw new FileNotFoundException("ffmpeg.exe is missing.");
                }

                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "FFMPEG.exe is present.", DateTime.Now);

                StreamProcess = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{SoundFile}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Initialized streamer for Windows successfully.", DateTime.Now);
            }
            else
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Application is running on Linux.", DateTime.Now);

                    StreamProcess = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{SoundFile}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Initialized streamer for Linux successfully.", DateTime.Now);
                }
                else
                {
                    throw new PlatformNotSupportedException("Application is running on an unsupported OS.");
                }
            }

            VoiceConnection = await VoiceNextExt.ConnectAsync(RandomMember.VoiceState.Channel);

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Connected to channel successfully.", DateTime.Now);

            DiscordChannel DefaultChannel = Guild.Channels.Values.Where(x => x.Id == ConfigLoader.DefaultChannelId).ToList().FirstOrDefault();
            await DefaultChannel.SendFileAsync(PizzaImage);

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Image posted successfully.", DateTime.Now);

            await VoiceConnection.SendSpeakingAsync(true);

            var ffmpeg = Process.Start(StreamProcess);
            var ffout = ffmpeg.StandardOutput.BaseStream;
            var txStream = VoiceConnection.GetTransmitStream();

            await ffout.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await VoiceConnection.WaitForPlaybackFinishAsync();

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Playback finished successfully.", DateTime.Now);

            await VoiceConnection.SendSpeakingAsync(false);
            VoiceConnection.Disconnect();

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Disconnected from channel successfully.", DateTime.Now);
        }
    }
}
