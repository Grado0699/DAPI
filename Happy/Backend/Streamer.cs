using Backend;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Happy.Backend
{
    public class Streamer
    {
        private readonly AudioStreamer _audioStreamer;
        private readonly Logger _logger;

        public Streamer(DiscordClient client)
        {
            _audioStreamer = new AudioStreamer(client);
            _logger = new Logger(client);
        }

        /// <summary>
        ///     Connects to the specified voice channel and plays the default soundfile.
        /// </summary>
        public async Task PlayDefaultSoundFile(DiscordChannel discordChannel)
        {
            const string soundFile = "Ressources/happy_aye_sir.ogg";

            if (!File.Exists(soundFile))
            {
                throw new FileNotFoundException("Soundfile could not be found.");
            }

            try
            {
                await _audioStreamer.PlaySoundFileAsync(soundFile, discordChannel, "10");
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