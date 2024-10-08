﻿using Backend;
using DSharpPlus;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace Luigis_Pizza.Backend;

public class Worker : BaseCommandModule{
    private readonly IAudioStreamer _audioStreamer;
    private readonly DiscordClient _client;

    private const string SoundFile = "Resources/pizza.mp3";
    private const string PizzaImage = "Resources/pizza.jpg";

    public Worker(DiscordClient client) {
        _audioStreamer = new AudioStreamer(client);
        _client = client;
    }

    /// <summary>
    ///     Connects to the voice-channel of a random member and plays a the pizza sound-file.
    /// </summary>
    public async Task StartWorkerAsync() {
        var guild = _client.Guilds.Values.FirstOrDefault(x => x.Id == ConfigLoader.GuildId);

        if (guild == null) {
            throw new ArgumentNullException($"{nameof(guild)}", "The specified Guild was not found. Check the 'Guild' parameter in the configuration file.");
        }

        var membersWithVoiceStateUp = guild.Members.Values.Where(x => x.VoiceState != null && x.VoiceState.Channel != null && x.VoiceState.Channel.GuildId == guild.Id && !x.IsBot).ToList();

        if (membersWithVoiceStateUp == null) {
            throw new ArgumentNullException($"{nameof(membersWithVoiceStateUp)}", "List of users with voice-state 'up' is null.");
        }

        if (membersWithVoiceStateUp.Count == 0) {
            // _logger.Log("There are currently no users with voice-state 'up'.", LogLevel.Warning);
            return;
        }

        var randomNumber = new Random();
        var randomMember = membersWithVoiceStateUp[randomNumber.Next(0, membersWithVoiceStateUp.Count - 1)];

        // _logger.Log("Retrieved a random member successfully.", LogLevel.Debug);

        if (!File.Exists(SoundFile) || !File.Exists(PizzaImage)) {
            throw new FileNotFoundException("Either image or sound-file is missing.");
        }

        try {
            await _audioStreamer.PlaySoundFileAsync(SoundFile, randomMember.VoiceState.Channel, "10");
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