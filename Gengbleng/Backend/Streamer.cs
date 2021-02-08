using Backend;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ILogger = Backend.ILogger;

namespace Gengbleng.Backend
{
    public class Streamer
    {
        private readonly IAudioStreamer _audioStreamer;
        private readonly DiscordClient _client;
        private readonly ILogger _logger;

        public Streamer(DiscordClient client)
        {
            _audioStreamer = new AudioStreamer(client);
            _client = client;
            _logger = new Logger(client);
        }

        /// <summary>
        ///     Connects to the voice-channel of a random member and plays a random sound-file.
        /// </summary>
        public async Task PlayRandomSoundFile()
        {
            var guild = _client.Guilds.Values.FirstOrDefault(x => x.Id == ConfigLoader.GuildId);

            if (guild == null)
            {
                throw new ArgumentNullException(
                    $"{nameof(guild)}",
                    "The specified Guild was not found. Check the 'Guild' parameter in the configuration file.");
            }

            var membersWithVoiceStateUp = guild.Members.Values.Where(x =>
                x.VoiceState != null && x.VoiceState.Channel != null && x.VoiceState.Channel.GuildId == guild.Id &&
                !x.IsBot && x.VoiceState.Channel.Id != x.Guild.AfkChannel.Id).ToList();

            if (membersWithVoiceStateUp == null)
            {
                throw new ArgumentNullException($"{nameof(membersWithVoiceStateUp)}",
                    "List of users with voice-state 'up' is null.");
            }

            if (membersWithVoiceStateUp.Count == 0)
            {
                _logger.Log("There are currently no users with voice-state 'up'.", LogLevel.Warning);
                return;
            }

            var randomNumber = new Random();
            var randomMember = membersWithVoiceStateUp[randomNumber.Next(0, membersWithVoiceStateUp.Count - 1)];

            _logger.Log("Retrieved a random member successfully.", LogLevel.Debug);

            var soundFiles = Directory.GetFiles(@"Resources/", "*.ogg");
            var soundFile = soundFiles[randomNumber.Next(0, soundFiles.Length - 1)];

            if (!File.Exists(soundFile))
            {
                throw new FileNotFoundException("Sound-file could not be found.");
            }

            try
            {
                await _audioStreamer.PlaySoundFileAsync(soundFile, randomMember.VoiceState.Channel, "10");
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                _logger.Log($"{fileNotFoundException}", LogLevel.Error);
            }
            catch (PlatformNotSupportedException platformNotSupportedException)
            {
                _logger.Log($"{platformNotSupportedException}", LogLevel.Error);
            }
            catch (Exception exception)
            {
                _logger.Log($"{exception}", LogLevel.Error);
            }
        }
    }
}