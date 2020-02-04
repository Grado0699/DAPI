using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;

namespace Bunta.Commands
{
    public class Voice : BaseCommandModule
    {
        private string[] ListOfSongs = Directory.GetFiles(@"Ressources\", "*.mp3");
        private static ProcessStartInfo StreamerProcess { get; set; } = null;

        [Command("list"), Aliases("-l"), Description("Lists all available songs.")]
        public async Task ListAllSongs(CommandContext ctx)
        {
            foreach (var Song in ListOfSongs)
            {

            }
        }


















        [Command("play")]
        public async Task ExaSupreme(CommandContext ctx)
        {
            const string RessourceName = "Ressources/Nightcore - PlayXSpectre.mp3";

            var VoiceNextExt = ctx.Client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(ctx.Guild);

            if (VoiceConnection != null)
            {
                VoiceConnection.Disconnect();
            }

            if (!File.Exists("ffmpeg.exe"))
            {
                return;
            }

            StreamerProcess = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $@"-i ""{RessourceName}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            VoiceConnection = await VoiceNextExt.ConnectAsync(ctx.Member.VoiceState.Channel);

            VoiceConnection.SendSpeaking(true);

            var ffmpeg = Process.Start(StreamerProcess);
            var ffout = ffmpeg.StandardOutput.BaseStream;
            var txStream = VoiceConnection.GetTransmitStream();

            await ffout.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await VoiceConnection.WaitForPlaybackFinishAsync();

            VoiceConnection.Disconnect();
        }

        [Command("reset")]
        public async Task Reset(CommandContext ctx)
        {
            var VoiceNextExt = ctx.Client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(ctx.Guild);

            VoiceConnection.Disconnect();
        }
    }
}
