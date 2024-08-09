using Backend;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace Happy.Backend;

public class Streamer : BaseCommandModule{
    private readonly IAudioStreamer _audioStreamer;

    public Streamer(DiscordClient client) {
        _audioStreamer = new AudioStreamer(client);
    }

    /// <summary>
    ///     Connects to the specified voice channel and plays the default sound-file.
    /// </summary>
    public async Task PlayDefaultSoundFile(DiscordChannel discordChannel) {
        const string soundFile = "Resources/happy_aye_sir.ogg";

        if (!File.Exists(soundFile)) {
            throw new FileNotFoundException("Sound-file could not be found.");
        }

        try {
            await _audioStreamer.PlaySoundFileAsync(soundFile, discordChannel, "10");
        }
        catch (FileNotFoundException fileNotFoundException) {
            // _logger.Log($"{fileNotFoundException}", LogLevel.Error);
        }
        catch (PlatformNotSupportedException platformNotSupportedException) {
            // _logger.Log($"{platformNotSupportedException}", LogLevel.Error);
        }
        catch (Exception exception) {
            // _logger.Log($"{exception}", LogLevel.Error);
        }
    }
}