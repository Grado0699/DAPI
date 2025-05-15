using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Backend;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;

namespace Erika.Commands;

public partial class Commands : ApplicationCommandModule {
    private readonly ILogger<Commands> _logger;
    private const string StreamUrl = "http://player.ffn.de/radiobollerwagen.mp3";

    public Commands(ILogger<Commands> logger) {
        _logger = logger;
    }

    [SlashCommand("reminder", "Max should make his motorcycle license")]
    public async Task Reminder(InteractionContext ctx) {
        var channel = ctx.Guild.GetChannel(645990465686077460);
        var member = await ctx.Guild.GetMemberAsync(292444852732559360);

        var reply = $"{member.Mention}....... mach mal dein Führerschein";

        _ = new DiscordMessageBuilder().WithContent(reply).WithAllowedMention(new UserMention(member)).SendAsync(channel);

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));
    }

    [SlashCommand("clear", "Clear commands")]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task ClearCommands(InteractionContext ctx) {
        await ctx.Guild.BulkOverwriteApplicationCommandsAsync([]);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));
    }

    [SlashCommand("play", "Just Erika")]
    // [SlashRequirePermissions(Permissions.Speak)]
    public async Task Play(InteractionContext ctx) {
        const string soundFile = "Resources/Erika.ogg";

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member.VoiceState.Channel, "10");

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Success!"));
    }

    [SlashCommand("leave", "Leave the current voice channel")]
    [SlashRequirePermissions(Permissions.Speak)]
    public async Task LeaveChannel(InteractionContext ctx) {
        var voiceNextExt = ctx.Client.GetVoiceNext();
        var voiceConnection = voiceNextExt.GetConnection(ctx.Guild);

        voiceConnection?.Disconnect();

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));
    }

    [SlashCommand("bollerwagen", "Radio Bollerwagen")]
    public async Task PlayRadio(InteractionContext ctx) {
        _logger.LogInformation("/radio command invoked by {User}", $"{ctx.User.Username}#{ctx.User.Discriminator}");

        await ctx.DeferAsync();

        var vnext = ctx.Client.GetVoiceNext();
        var vnc = vnext.GetConnection(ctx.Guild);

        if (vnc == null) {
            var voiceState = ctx.Member?.VoiceState;
            if (voiceState?.Channel == null) {
                _logger.LogWarning("User {User} is not in a voice channel.", ctx.User.Username);

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You need to be in a voice channel."));
                return;
            }

            _logger.LogInformation("Joining voice channel: {Channel}", voiceState.Channel.Name);
            vnc = await vnext.ConnectAsync(voiceState.Channel);
        } else {
            _logger.LogInformation("Already connected to a voice channel.");
        }

        var transmit = vnc.GetTransmitSink();

        _logger.LogInformation("Starting FFmpeg audio stream...");
        var ffmpegAudio = Process.Start(new ProcessStartInfo {
            FileName = "ffmpeg",
            Arguments = $"-hide_banner -loglevel panic -i {StreamUrl} -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });

        var ffmpegOut = ffmpegAudio?.StandardOutput.BaseStream;

        // Start streaming audio
        _ = Task.Run(async () => {
            try {
                await ffmpegOut.CopyToAsync(transmit);
                _logger.LogInformation("Finished streaming (normal end).");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Streaming error.");
            }
            finally {
                ffmpegOut.Dispose();
            }
        });

        // Rich Presence for Discord Bot
        _ = Task.Run(async () => {
            string? lastTitle = null;
            while (ffmpegAudio is { HasExited: false }) {
                try {
                    var title = await GetCurrentIcyTitleAsync("http://player.ffn.de/radiobollerwagen.mp3");

                    if (!string.IsNullOrWhiteSpace(title) && title != lastTitle) {
                        lastTitle = title;

                        // Truncate long titles
                        if (title.Length > 100)
                            title = title[..100] + "…";

                        var fancyStatus = $"🎶 Now playing: {title}";

                        _logger.LogInformation("Updating bot status to: {Status}", fancyStatus);
                        await ctx.Client.UpdateStatusAsync(new DiscordActivity(fancyStatus, ActivityType.ListeningTo));
                    }
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Failed to fetch ICY metadata.");
                }

                await Task.Delay(15000); // Poll every 15s
            }

            _logger.LogInformation("Radio stream ended, stopping metadata updates.");
        });

        _logger.LogInformation("Radio streaming started successfully.");
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("🎶 Streaming Radio Bollerwagen..."));

        _logger.LogInformation("Radio streaming started successfully.");
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("🎶 Streaming Radio Bollerwagen..."));
    }

    private async Task<string?> GetCurrentIcyTitleAsync(string url) {
        try {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Icy-MetaData", "1");

            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
                return null;

            var stream = await response.Content.ReadAsStreamAsync();
            var metaInt = int.Parse(response.Headers.GetValues("icy-metaint").First());

            var buffer = new byte[metaInt];
            await stream.ReadExactlyAsync(buffer);

            var metaLength = stream.ReadByte();
            if (metaLength <= 0)
                return null;

            var metadataSize = metaLength * 16;
            var metaBuffer = new byte[metadataSize];
            await stream.ReadExactlyAsync(metaBuffer, 0, metadataSize);
            var metadata = Encoding.UTF8.GetString(metaBuffer);

            _logger.LogInformation($"Extracted metadata: {metadata}");

            var match = MyRegex().Match(metadata);
            return match.Success ? match.Groups[1].Value : null;
        }
        catch (Exception ex) {
            Console.WriteLine($"[MetadataError] {ex.Message}");
            return null;
        }
    }

    [GeneratedRegex(@"StreamTitle='([^']+)';")]
    private static partial Regex MyRegex();
}