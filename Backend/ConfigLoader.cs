using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Backend; 

public static class ConfigLoader {
    private const string ConfigFileName = "Config.json";
    public static string[] CommandPrefix { get; set; }
    public static ulong DefaultChannelId { get; set; }
    public static ulong GulagChannelId { get; set; }
    public static ulong GuildId { get; set; }
    public static string Token { get; set; }

    /// <summary>
    ///     Loads all properties of the configuration file.
    /// </summary>
    public static async Task LoadConfigurationFromFileAsync() {
        if (!File.Exists(ConfigFileName)) {
            throw new FileNotFoundException($"The configuration file '{ConfigFileName}' is missing.");
        }

        await using var streamConfig = File.OpenRead(ConfigFileName);
        using var streamReaderConfig = new StreamReader(streamConfig, new UTF8Encoding(false));
        var readConfig = await streamReaderConfig.ReadToEndAsync();
        var configuration = JsonConvert.DeserializeObject<ConfigJson>(readConfig);

        CommandPrefix = configuration.CommandPrefix;
        DefaultChannelId = configuration.DefaultChannelId;
        GulagChannelId = configuration.GulagChannelId;
        GuildId = configuration.GuildId;
        Token = configuration.Token;
    }

    private struct ConfigJson {
        [JsonProperty("Prefix")] public string[] CommandPrefix { get; private set; }
        [JsonProperty("DefaultChannel")] public ulong DefaultChannelId { get; private set; }
        [JsonProperty("GoulagChannel")] public ulong GulagChannelId { get; private set; }
        [JsonProperty("Guild")] public ulong GuildId { get; private set; }
        [JsonProperty("Token")] public string Token { get; private set; }
    }
}