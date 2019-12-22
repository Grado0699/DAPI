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
        private static ProcessStartInfo StreamProcess {get;set;} = null;

        public async Task StartWorker(DiscordClient Client)
        {
            // Get the guild the client is connected to
            DiscordGuild Guild = Client.Guilds.Values.Where(x => x.Name.Contains("Bunch of retards playing")).ToList().FirstOrDefault();

            if(Guild == null)
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"An error occured while retrieving guild information.", DateTime.Now);
                return;
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Retrieved guild successfully.", DateTime.Now);

            // Get a list of members of this guild that have an active voicestate and are connected to a channel of this guild
            List<DiscordMember> MembersVoiceStateUp = Guild.Members.Values.Where(x => x.VoiceState != null && x.VoiceState.Channel != null && x.VoiceState.Channel.GuildId == Guild.Id).ToList();

            if(MembersVoiceStateUp == null)
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"An error occured while retrieving members with the voicestate: up.", DateTime.Now);
                return;
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Retrieved members with active voicestate successfully.", DateTime.Now);

            if (MembersVoiceStateUp.Count == 0)
            {
                Client.DebugLogger.LogMessage(LogLevel.Warning, Assembly.GetExecutingAssembly().GetName().Name, $"There are currently no members with voicestate: up in this guild.", DateTime.Now);
                return;
            }

            // Select a random member of this list
            var RandomNumber = new Random();
            DiscordMember RandomMember = MembersVoiceStateUp[RandomNumber.Next(0, MembersVoiceStateUp.Count - 1)];

            if(RandomMember == null)
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"An error occured while selecting a random member.", DateTime.Now);
                return;
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Retrieved a random member successfully.", DateTime.Now);

            // Get the voice-connection to this guild
            var VoiceNextExt = Client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(Guild);

            if(VoiceConnection != null)
            {
                VoiceConnection.Disconnect();
                Client.DebugLogger.LogMessage(LogLevel.Warning, Assembly.GetExecutingAssembly().GetName().Name, $"An old connection was still up, successfully closed old one.", DateTime.Now);
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Voice connection retrieved successfully.", DateTime.Now);

            // Check if the image and soundfile are not missing
            if (!File.Exists("Ressources/luigi.mp3") || !File.Exists("Ressources/pizza.jpg"))
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Either image or soundfile is missing.", DateTime.Now);
                return;
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Image and soundfile are present.", DateTime.Now);

            // Check if OS is Windows
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Application is running on Windows.", DateTime.Now);

                if (!File.Exists("ffmpeg.exe"))
                {
                    Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"FFMPEG.exe is missing.", DateTime.Now);
                    return;
                }

                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"FFMPEG.exe is present.", DateTime.Now);

                StreamProcess = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{"Ressources/luigi.mp3"}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Initialized streamer for Windows successfully.", DateTime.Now);
            }
            else
            {
                // Check if OS is Linux
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Application is running on Linux.", DateTime.Now);

                    StreamProcess = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{"Ressources/luigi.mp3"}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                        RedirectStandardOutput = true,
                        UseShellExecute = true
                    };

                    Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Initialized streamer for Linux successfully.", DateTime.Now);
                }
                else
                {
                    Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Application is running on an unknown OS.", DateTime.Now);
                    return;
                }
            }

            // Connect to the channel of the random selected member
            VoiceConnection = await VoiceNextExt.ConnectAsync(RandomMember.VoiceState.Channel);

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Connected to channel successfully.", DateTime.Now);

            // Post the image in the default channel of the server
            await Guild.GetDefaultChannel().SendFileAsync("Ressources/pizza.jpg");

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Image posted successfully.", DateTime.Now);

            VoiceConnection.SendSpeaking(true);

            // Start the FFMPEG and stream to the server
            var ffmpeg = Process.Start(StreamProcess);
            var ffout = ffmpeg.StandardOutput.BaseStream;
            var txStream = VoiceConnection.GetTransmitStream();

            await ffout.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await VoiceConnection.WaitForPlaybackFinishAsync();

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Playback finished successfully.", DateTime.Now);

            // After playback is finished, disconnect from channel
            VoiceConnection.Disconnect();

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Disconnected from channel successfully.", DateTime.Now);
        }
    }
}
