﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using System.Linq;
using System.Threading.Tasks;

namespace Rem.Commands; 

public class MusicPlayer : BaseCommandModule {
    [Command]
    public async Task Join(CommandContext ctx) {
        var lava = ctx.Client.GetLavalink();

        if (!lava.ConnectedNodes.Any()) {
            await ctx.RespondAsync("The LavaLink connection is not established");
            return;
        }

        var node = lava.ConnectedNodes.Values.First();
        var channel = ctx.Member?.VoiceState.Channel;

        if (channel?.Type != ChannelType.Voice) {
            await ctx.RespondAsync("Not a valid voice channel.");
            return;
        }

        await node.ConnectAsync(channel);
        await ctx.RespondAsync($"Joined {channel.Name}!");
    }

    [Command]
    public async Task Leave(CommandContext ctx) {
        var lava = ctx.Client.GetLavalink();

        if (!lava.ConnectedNodes.Any()) {
            await ctx.RespondAsync("The LavaLink connection is not established");
            return;
        }

        var node = lava.ConnectedNodes.Values.First();
        var channel = ctx.Member?.VoiceState.Channel;

        if (channel?.Type != ChannelType.Voice) {
            await ctx.RespondAsync("Not a valid voice channel.");
            return;
        }

        var conn = node.GetGuildConnection(channel.Guild);

        if (conn == null) {
            await ctx.RespondAsync("LavaLink is not connected.");
            return;
        }

        await conn.DisconnectAsync();
        await ctx.RespondAsync($"Left {channel.Name}!");
    }

    [Command]
    public async Task Play(CommandContext ctx, [RemainingText] string search) {
        if (ctx.Member?.VoiceState == null || ctx.Member.VoiceState.Channel == null) {
            await ctx.RespondAsync("You are not in a voice channel.");
            return;
        }

        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null) {
            await ctx.RespondAsync("LavaLink is not connected.");
            return;
        }

        var loadResult = await node.Rest.GetTracksAsync(search);

        if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches) {
            await ctx.RespondAsync($"Track search failed for {search}.");
            return;
        }

        var track = loadResult.Tracks.First();

        await conn.SetVolumeAsync(5);
        await conn.PlayAsync(track);

        await ctx.RespondAsync($"Now playing {track.Title}!");
    }

    [Command]
    public async Task Pause(CommandContext ctx) {
        if (ctx.Member?.VoiceState == null || ctx.Member.VoiceState.Channel == null) {
            await ctx.RespondAsync("You are not in a voice channel.");
            return;
        }

        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null) {
            await ctx.RespondAsync("LavaLink is not connected.");
            return;
        }

        if (conn.CurrentState.CurrentTrack == null) {
            await ctx.RespondAsync("There are no tracks loaded.");
            return;
        }

        await conn.PauseAsync();
    }
}