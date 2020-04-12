using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gengbleng.Backend
{
    public static class ConfigLoader
    {
        private const string ConfigFileName = "Config.json";
        public static ulong DefaultChannelId { get; set; }
        public static string[] CommandPrefix { get; set; }
        public static ulong GuildId { get; set; }
        public static string Token { get; set; }

        public static async Task LoadConfigurationFromFile()
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
            DefaultChannelId = Configuration.DefaultChannelId;
            GuildId = Configuration.GuildId;
            Token = Configuration.Token;
        }

        private struct ConfigJson
        {
            [JsonProperty("DefaultChannel")]
            public ulong DefaultChannelId { get; private set; }

            [JsonProperty("Guild")]
            public ulong GuildId { get; private set; }
            [JsonProperty("Prefix")]
            public string[] CommandPrefix { get; private set; }

            [JsonProperty("Token")]
            public string Token { get; private set; }
        }
    }
}
