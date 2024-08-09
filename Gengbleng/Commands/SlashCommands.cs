using System.Threading.Tasks;
using Backend;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Gengbleng.Backend;

namespace Gengbleng.Commands;

public class SlashCommands : ApplicationCommandModule {
    [SlashCommand("testus", "A slash command made to test the DSharpPlus Slash Commands extension!")]
    public async Task TestCommand(InteractionContext ctx) {
        var streamer = new Streamer(ctx.Client);
        await streamer.PlayRandomSoundFile();

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));
    }

    [SlashCommand("barrel", "BARREL!")]
    public async Task Barrel(InteractionContext ctx) {
        const string soundFile = "Resources/E1.ogg";

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Barrel!"));
    }

    [SlashCommand("shotgunknees", "Shotgun-Kneeeeeeeeeeeees")]
    public async Task ShotgunKnees(InteractionContext ctx) {
        const string soundFile = "Resources/ShotgunKnees.ogg";

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "500");

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Barrel!"));
    }

    [SlashCommand("shanty", "Plays a shanty")]
    public async Task PlayShanty(InteractionContext ctx) {
        const string soundFile = "Resources/ShantyLeaveHerJohnny.ogg";

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var activity = new DiscordActivity();
        var discord = ctx.Client;

        activity.Name = "Leave her Johnny";
        activity.ActivityType = ActivityType.ListeningTo;

        await discord.UpdateStatusAsync(activity);

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Barrel!"));
    }
}