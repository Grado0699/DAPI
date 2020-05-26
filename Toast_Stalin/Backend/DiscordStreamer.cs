using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace Toast_Stalin.Backend
{
    public static class DiscordStreamer
    {
        private static ProcessStartInfo StreamerProcess { get; set; } = null;
        public static async Task StreamToDiscord(CommandContext ctx, string RessourceName)
        {
            var VoiceNextExt = ctx.Client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(ctx.Guild);

            if (VoiceConnection != null)
            {
                VoiceConnection.Disconnect();
                ctx.Client.DebugLogger.LogMessage(LogLevel.Warning, Assembly.GetExecutingAssembly().GetName().Name, $"An old connection was still up, successfully closed old one.", DateTime.Now);
            }

            // Check if OS is Windows
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Application is running on Windows.", DateTime.Now);

                if (!File.Exists("ffmpeg.exe"))
                {
                    ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"FFMPEG.exe is missing.", DateTime.Now);
                    return;
                }

                ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"FFMPEG.exe is present.", DateTime.Now);

                StreamerProcess = new ProcessStartInfo
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $@"-i ""{RessourceName}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Initialized streamer for Windows successfully.", DateTime.Now);
            }
            else
            {
                // Check if OS is Linux
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Application is running on Linux.", DateTime.Now);

                    StreamerProcess = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{RessourceName}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Initialized streamer for Linux successfully.", DateTime.Now);
                }
                else
                {
                    ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Application is running on an unknown OS.", DateTime.Now);
                    return;
                }
            }

            VoiceConnection = await VoiceNextExt.ConnectAsync(ctx.Member.VoiceState.Channel);

            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Connected to channel successfully.", DateTime.Now);

            await VoiceConnection.SendSpeakingAsync(true);

            var ffmpeg = Process.Start(StreamerProcess);
            var ffout = ffmpeg.StandardOutput.BaseStream;
            var txStream = VoiceConnection.GetTransmitStream();

            await ffout.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await VoiceConnection.WaitForPlaybackFinishAsync();

            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Playback finished successfully.", DateTime.Now);

            await VoiceConnection.SendSpeakingAsync(false);

            VoiceConnection.Disconnect();

            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Disconnected from channel successfully.", DateTime.Now);
        }
    }
}
