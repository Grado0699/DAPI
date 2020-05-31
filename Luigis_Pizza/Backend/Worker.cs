using Backend;
using DSharpPlus;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Luigis_Pizza.Backend
{
    public class Worker
    {
        private readonly AudioStreamer _audioStreamer;
        private readonly DiscordClient _client;
        private readonly Logger _logger;

        private const string SoundFile = "Ressources/pizza.mp3";
        private const string PizzaImage = "Ressources/pizza.jpg";

        public Worker(DiscordClient client)
        {
            _audioStreamer = new AudioStreamer(client);
            _client = client;
            _logger = new Logger(client);
        }

        /// <summary>
        ///     Connects to the voicechannel of a random member and plays a the pizza soundfile.
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public async Task StartWorkerAsync()
        {
            var Guild = _client.Guilds.Values.Where(x => x.Id == ConfigLoader.GuildId).ToList().FirstOrDefault();

            if (Guild == null)
            {
                throw new ArgumentNullException("The specified Guild was not found. Check the 'Guild' parameter in the configuration file.");
            }

            var MembersWithVoiceStateUp = Guild.Members.Values.Where(x => x.VoiceState != null && x.VoiceState.Channel != null && x.VoiceState.Channel.GuildId == Guild.Id && !x.IsBot).ToList();

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

            if (!File.Exists(SoundFile) || !File.Exists(PizzaImage))
            {
                throw new FileNotFoundException("Either image or soundfile is missing.");
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
