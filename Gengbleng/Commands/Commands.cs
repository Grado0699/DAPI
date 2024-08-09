using Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;

namespace Gengbleng.Commands;

public class Commands : BaseCommandModule {
    private static double _timerSpan = 1800000;
    private static bool _enableTimer;

    [Command("settimer"), Aliases("t"), Description("Enable/disable the timer.")]
    public async Task SetTimer(CommandContext ctx) {
        var interActExt = ctx.Client.GetInteractivity();

        if (_enableTimer) {
            await ctx.RespondAsync($"Current status of timer is `{_enableTimer}`. Deactivate timer? [y/n]");
        } else {
            await ctx.RespondAsync($"Current status of timer is `{_enableTimer}`. Activate timer? [y/n]");
        }

        var message = await interActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

        if (message.Result.Content.ToLower().Contains('y')) {
            _enableTimer = !_enableTimer;
            await ctx.RespondAsync($"Timer set to `{_enableTimer}`.");
        } else if (message.Result.Content.ToLower().Contains('n')) {
            await ctx.RespondAsync("Do nothing.");
        } else {
            await ctx.RespondAsync("Invalid input.");
        }
    }

    [Command("interval"), Aliases("i"), Description("Set new timer interval.")]
    public async Task SetInterval(CommandContext ctx) {
        var interActExt = ctx.Client.GetInteractivity();

        await ctx.RespondAsync($"Current interval is `{_timerSpan / 1000d}`. Enter new interval in `x seconds`:");

        var message = await interActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

        if (double.TryParse(message.Result.Content, out _timerSpan)) {
            _timerSpan *= 1000d;

            await ctx.RespondAsync($"Interval set to: `{_timerSpan / 1000d}` seconds.");
        } else {
            _timerSpan = 1800000;
            await ctx.RespondAsync($"Invalid input. Set interval to default value: `{_timerSpan / 1000d}` seconds.");
        }
    }

    [Command("barrel"), Aliases("b"), Description("BARREL!")]
    public async Task Barrel(CommandContext ctx) {
        const string soundFile = "Resources/E1.ogg";

        IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel, "10");
    }

    [Command("shotgunknees"), Aliases("s"), Description("Plays the 'Shotgun-Knees' sound.")]
    public async Task ShotgunKnees(CommandContext ctx) {
        const string soundFile = "Resources/ShotgunKnees.ogg";

        IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel, "500");
    }

    [Command("random"), Aliases("r"), Description("Plays a random soundfile.")]
    public async Task PlayRandomSoundFile(CommandContext ctx) {
        var random = new Random();

        var allSoundFiles = Directory.GetFiles(@"Resources/", "*.ogg");
        var randomSoundFile = allSoundFiles[random.Next(0, allSoundFiles.Length - 1)];

        if (!File.Exists(randomSoundFile)) {
            throw new FileNotFoundException("Either image or soundfile is missing.");
        }

        IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(randomSoundFile, ctx.Member?.VoiceState.Channel, "10");
    }

    [Command("shanty"), Aliases("sh"), Description("Plays a random shanty.")]
    public async Task PlayShanty(CommandContext ctx) {
        const string soundFile = "Resources/ShantyLeaveHerJohnny.ogg";

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel, "10");
    }

    [Command("yooooooooooo"), Aliases("y"), Description("Plays yooooooooooo.")]
    public async Task PlayYooooo(CommandContext ctx) {
        const string soundFile = "Resources/Yooooooooooo.ogg";

        IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel, "10");
    }

    [Command("shit"), Aliases("arrr"), Description("Plays Shit Boat from Alestorm.")]
    public async Task ShitBoat(CommandContext ctx) {
        const string soundFile = "Resources/ShitBoat.ogg";

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel, "10");
    }

    [Command("fuck"), Aliases("f"), Description("Plays Fucked With An Anchor from Alestorm.")]
    public async Task FuckedWithAnAnchor(CommandContext ctx) {
        const string soundFile = "Resources/FuckedWithAnAnchor.ogg";

        IAudioStreamer audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel, "10");
    }

    [Command("whatdoesapirateneed"), Aliases(""), Description("Tells you want a pirate needs.")]
    public async Task WhatDoesAPiratNeed(CommandContext ctx) {
        const string soundFile = "Resources/Alestorm.ogg";

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel, "10");
    }

    [Command("leave"), Aliases("l"), Description("Leaves the current voice channel.")]
    public async Task LeaveChannel(CommandContext ctx) {
        var voiceNextExt = ctx.Client.GetVoiceNext();
        var voiceConnection = voiceNextExt.GetConnection(ctx.Guild);

        voiceConnection?.Disconnect();

        await Task.CompletedTask;
    }
}