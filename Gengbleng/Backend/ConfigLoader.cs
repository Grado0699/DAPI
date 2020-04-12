using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Gengbleng.Backend
{
    public static class ConfigLoader
    {
        private const string ConfigFileName = "Config.json";
        public static string[] CommandPrefix { get; set; }
        public static ulong GuildId { get; set; }
        public static string Token { get; set; }

        /// <summary>
        ///     Loads all properties of the configuration file.
        /// </summary>
        /// <returns></returns>
        public static async Task LoadConfigurationFromFileAsync()
        {
            if (!File.Exists(ConfigFileName))
            {
                throw new FileNotFoundException($"The configuration file '{ConfigFileName}' is missing.");
            }

            using var StreamConfig = File.OpenRead(ConfigFileName);
            using var StreamReaderConfig = new StreamReader(StreamConfig, new UTF8Encoding(false));
            var ReadConfig = await StreamReaderConfig.ReadToEndAsync();
            ConfigJson Configuration = JsonConvert.DeserializeObject<ConfigJson>(ReadConfig);

            CommandPrefix = Configuration.CommandPrefix;
            GuildId = Configuration.GuildId;
            Token = Configuration.Token;
        }

        private struct ConfigJson
        {
            [JsonProperty("Guild")]
            public ulong GuildId { get; private set; }
            [JsonProperty("Prefix")]
            public string[] CommandPrefix { get; private set; }

            [JsonProperty("Token")]
            public string Token { get; private set; }
        }
    }
}
