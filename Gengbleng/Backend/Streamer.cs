using Backend;
using DSharpPlus;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gengbleng.Backend
{
    public class Streamer
    {
        private readonly AudioStreamer _audioStreamer;
        private readonly DiscordClient _client;
        private readonly Logger _logger;

        public Streamer(DiscordClient client)
        {
            _audioStreamer = new AudioStreamer(client);
            _client = client;
            _logger = new Logger(client);
        }

        /// <summary>
        ///     Connects to the voicechannel of a random member and plays a random soundfile.
        /// </summary>
        /// <returns></returns>
        public async Task PlayRandomSoundFile()
        {
            var guild = _client.Guilds.Values.FirstOrDefault(x => x.Id == ConfigLoader.GuildId);

            if (guild == null)
            {
                throw new ArgumentNullException(
                    "The specified Guild was not found. Check the 'Guild' parameter in the configuration file.");
            }

            var membersWithVoiceStateUp = guild.Members.Values.Where(x =>
                x.VoiceState != null && x.VoiceState.Channel != null && x.VoiceState.Channel.GuildId == guild.Id &&
                !x.IsBot && x.VoiceState.Channel.Id != x.Guild.AfkChannel.Id).ToList();

            if (membersWithVoiceStateUp == null)
            {
                throw new ArgumentNullException("List of users with voicestate 'up' is null.");
            }

            if (membersWithVoiceStateUp.Count == 0)
            {
                _logger.Log("There are currently no users with voicestate 'up'.", LogLevel.Warning);
                return;
            }

            var randomNumber = new Random();
            var randomMember = membersWithVoiceStateUp[randomNumber.Next(0, membersWithVoiceStateUp.Count - 1)];

            _logger.Log("Retrieved a random member successfully.", LogLevel.Debug);

            var soundFiles = Directory.GetFiles(@"Ressources/", "*.ogg");
            var soundFile = soundFiles[randomNumber.Next(0, soundFiles.Length - 1)];

            if (!File.Exists(soundFile))
            {
                throw new FileNotFoundException("Soundfile could not be found.");
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