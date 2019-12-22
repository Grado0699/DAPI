using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Toast_Stalin.Backend
{
    public static class ConfigLoader
    {
        public static ulong DefaultChannelId { get; set; }
        public static ulong GoulagChannelId { get; set; }
        public static ulong GuildId { get; set; }
        public static string[] CommandPrefix { get; set; }
        public static double TimeUserIsGoulaged { get; set; }
        public static string Token { get; set; }

        public static async Task LoadConfigurationFromFile()
        {
            if (!File.Exists("Config.json"))
            {
                Console.WriteLine($"The configuration file 'Config.json' is missing.\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            using var StreamConfig = File.OpenRead("Config.json");
            using var StreamReaderConfig = new StreamReader(StreamConfig, new UTF8Encoding(false));
            var ReadConfig = await StreamReaderConfig.ReadToEndAsync();
            ConfigJson Configuration = JsonConvert.DeserializeObject<ConfigJson>(ReadConfig);

            DefaultChannelId = Configuration.DefaultChannelId;
            GoulagChannelId = Configuration.GoulagChannelId;
            GuildId = Configuration.GuildId;
            CommandPrefix = Configuration.CommandPrefix;
            TimeUserIsGoulaged = Configuration.TimeUserIsGoulaged;
            Token = Configuration.Token;
        }

        private struct ConfigJson
        {
            [JsonProperty("DefaulChannel")]
            public ulong DefaultChannelId { get; private set; }

            [JsonProperty("GoulagChannel")]
            public ulong GoulagChannelId { get; private set; }

            [JsonProperty("Guild")]
            public ulong GuildId { get; private set; }

            [JsonProperty("Prefix")]
            public string[] CommandPrefix { get; private set; }

            [JsonProperty("Time_User_Is_Goulaged")]
            public double TimeUserIsGoulaged { get; private set; }

            [JsonProperty("Token")]
            public string Token { get; private set; }
        }
    }
}
