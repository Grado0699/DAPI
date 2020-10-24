using Bunta.Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Bunta.Commands
{
    public class Voice : BaseCommandModule
    {
        private readonly string[] ListOfAvailableSongs = Directory.GetFiles(@"Ressources\", "*.mp3");
        private readonly Queue<string> PlaylistQueue = new Queue<string>();
        private int CurrentSongId = -1;
        private static ProcessStartInfo StreamerProcess { get; set; } = null;

        [Command("list"), Aliases("-l"), Description("Lists all available songs.")]
        public async Task ListAllSongs(CommandContext ctx) => await ctx.RespondAsync(await SongParser.ParseSongsAsync(ListOfAvailableSongs));

        [Command("listplaylist"), Aliases("-lp"), Description("Lists all songs in the current playlist.")]
        public async Task ListAllSongsInCurrentPlaylist(CommandContext ctx) => await ctx.RespondAsync(await SongParser.ParseSongsAsync(PlaylistQueue));

        [Command("set"), Aliases("-s"), Description("Sets the next song.")]
        public async Task SetNextSong(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            await ctx.RespondAsync("Enter the number of the next song or use `r` for a random song.");
            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            try
            {
                ProcessMessageResult(Message);
            }
            catch (ArgumentException)
            {
                await ctx.RespondAsync("Your input was not valid.");
                throw;
            }
            catch (IndexOutOfRangeException)
            {
                await ctx.RespondAsync("The number you entered was out of range.");
                throw;
            }

            await ctx.RespondAsync($"The next song will be `{await SongParser.ParseSongsAsync(ListOfAvailableSongs[CurrentSongId])}`.");
        }

        [Command("addSong"), Aliases("-as"), Description("Adds a song to the playlist.")]
        public async Task AddSongToPlaylist(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            await ctx.RespondAsync("Enter the number of the next song or use `r` for a random song.");
            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            try
            {
                ProcessMessageResult(Message);
            }
            catch(ArgumentException)
            {
                await ctx.RespondAsync("Your input was not valid.");
                throw;
            }
            catch(IndexOutOfRangeException)
            {
                await ctx.RespondAsync("The number you entered was out of range.");
                throw;
            }

            PlaylistQueue.Enqueue(ListOfAvailableSongs[CurrentSongId]);
            await ctx.RespondAsync($"Song `{await SongParser.ParseSongsAsync(ListOfAvailableSongs[CurrentSongId])}` added to playlist.");
        }

        private void ProcessMessageResult(InteractivityResult<DiscordMessage> Message)
        {
            if (!int.TryParse(Message.Result.Content, out CurrentSongId))
            {
                if (!(Message.Result.Content == "r"))
                {
                    throw new ArgumentException($"The entered input '{Message.Result.Content}' was not valid.");
                }

                var Randomizer = new Random();
                CurrentSongId = Randomizer.Next(0, ListOfAvailableSongs.Length - 1);
            }

            if (!(CurrentSongId < ListOfAvailableSongs.Length))
            {
                throw new IndexOutOfRangeException($"The entered number '{Message.Result.Content}' was out of index.");
            }
        }

        [Command("removeSong"), Aliases("-rs"), Description("Removes a song from the playlist.")]
        public async Task RemoveSongFromPlaylist(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            await ListAllSongsInCurrentPlaylist(ctx);
            await ctx.RespondAsync("Enter the number of the song that should be removed.");
            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (!int.TryParse(Message.Result.Content, out int SongToRemoveId))
            {
                throw new ArgumentException($"The entered input '{Message.Result.Content}' was not valid.");
            }

            //PlaylistQueue.
        }



        [Command("play")]
        public async Task ExaSupreme(CommandContext ctx)
        {
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
                Arguments = $@"-i ""{ListOfAvailableSongs[CurrentSongId]}"" -ac 2 -f s16le -ar 48000 pipe:1 -loglevel quiet -vol 10",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            VoiceConnection = await VoiceNextExt.ConnectAsync(ctx.Member.VoiceState.Channel);

            await VoiceConnection.SendSpeakingAsync(true);

            var ffmpeg = Process.Start(StreamerProcess);
            var ffout = ffmpeg.StandardOutput.BaseStream;
            var txStream = VoiceConnection.GetTransmitStream();

            await ffout.CopyToAsync(txStream);
            await txStream.FlushAsync();
            await VoiceConnection.WaitForPlaybackFinishAsync();

            VoiceConnection.Disconnect();
        }

        //[Command("reset")]
        //public async Task Reset(CommandContext ctx)
        //{
        //    var VoiceNextExt = ctx.Client.GetVoiceNext();
        //    var VoiceConnection = VoiceNextExt.GetConnection(ctx.Guild);

        //    VoiceConnection.Disconnect();
        //}
    }
}
