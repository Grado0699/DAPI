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
            var Guild = _client.Guilds.Values.Where(x => x.Id == ConfigLoader.GuildId).FirstOrDefault();

            if (Guild == null)
            {
                throw new ArgumentNullException("The specified Guild was not found. Check the 'Guild' parameter in the configuration file.");
            }

            var MembersWithVoiceStateUp = Guild.Members.Values.Where(x => x.VoiceState != null && x.VoiceState.Channel != null && x.VoiceState.Channel.GuildId == Guild.Id && !x.IsBot && x.VoiceState.Channel.Id != x.Guild.AfkChannel.Id).ToList();

            if (MembersWithVoiceStateUp == null)
            {
                throw new ArgumentNullException("List of users with voicestate 'up' is null.");
            }

            if (MembersWithVoiceStateUp.Count == 0)
            {
                _logger.Log("There are currently no users with voicestate 'up'.", LogLevel.Warning);
                return;
            }

            var RandomNumber = new Random();
            var RandomMember = MembersWithVoiceStateUp[RandomNumber.Next(0, MembersWithVoiceStateUp.Count - 1)];

            _logger.Log("Retrieved a random member successfully.", LogLevel.Debug);

            var SoundFiles = Directory.GetFiles(@"Ressources/", "*.ogg");
            var SoundFile = SoundFiles[RandomNumber.Next(0, SoundFiles.Length - 1)];

            if (!File.Exists(SoundFile))
            {
                throw new FileNotFoundException("Soundfile could not be found.");
            }

            try
            {
                await _audioStreamer.PlaySoundFileAsync(SoundFile, RandomMember.VoiceState.Channel, "10");
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
