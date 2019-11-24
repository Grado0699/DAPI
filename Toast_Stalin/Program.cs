using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Newtonsoft.Json;
using Toast_Stalin.Events;

namespace Toast_Stalin
{
    class Program
    {
        private static DiscordClient Client { get; set; } = null;
        private static CommandsNextExtension ComNextExt { get; set; } = null;

        private static VoiceNextExtension VoiceNextExt { get; set; } = null;
        private static void Main()
        {
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            // Load the configuration file and check if it exists
            if (!File.Exists("Config.json"))
            {
                Console.WriteLine($"The configuration file 'Config.json' is missing.\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            ConfigJson Config = new ConfigJson();

            using (var StreamConfig = File.OpenRead("Config.json"))
            {
                using var StreamReaderConfig = new StreamReader(StreamConfig, new UTF8Encoding(false));
                var ReadConfig = await StreamReaderConfig.ReadToEndAsync();
                Config = JsonConvert.DeserializeObject<ConfigJson>(ReadConfig);
            }

            // Load all client-events
            var ClientEvents = new ClientToastStalin();

            // Initialize the client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            Client.Ready += ClientEvents.Client_Ready;
            Client.GuildAvailable += ClientEvents.Client_GuildAvailable;
            Client.ClientErrored += ClientEvents.Client_ClientError;

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Client initialized successfully.", DateTime.Now);

            // Initialize the command-handler
            ComNextExt = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                UseDefaultCommandHandler = true,
                StringPrefixes = Config.CommandPrefix,
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableMentionPrefix = true,
                DmHelp = false,
                EnableDms = false
            });

            ComNextExt.CommandExecuted += ClientEvents.Commands_CommandExecuted;
            ComNextExt.CommandErrored += ClientEvents.Commands_CommandErrored;

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Command-Handler initialized successfully.", DateTime.Now);

            // Register the commands
            try
            {
                //ComNextExt.RegisterCommands<Core>();
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Registered commands successfully.", DateTime.Now);
            }
            catch
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"An error occured while registering the commands.", DateTime.Now);

                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();

                return;
            }

            // Initialize the voice-handler
            Client.UseVoiceNext(new VoiceNextConfiguration
            {
                EnableIncoming = false
            });

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Voice-Handler initialized successfully.", DateTime.Now);

            // Initialize the interactivity-handler
            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(30)
            });

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Interactivity-Handler initialized successfully.", DateTime.Now);

            // Connect to the API and wait for requests
            try
            {
                await Client.ConnectAsync();
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Connected to the API successfully.", DateTime.Now);
            }
            catch
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"An error occured while connecting to the API.", DateTime.Now);

                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();

                return;
            }

            // Wait for the connection to complete
            await Task.Delay(1000);

            // Get the guild the client is connected to
            DiscordGuild Guild = Client.Guilds.Values.Where(x => x.Name.Contains("Bunch of retards playing")).ToList().FirstOrDefault();

            if (Guild == null)
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"An error occured while retrieving guild information.", DateTime.Now);
                return;
            }

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Retrieved guild successfully.", DateTime.Now);

            // Get the voice-connection to this guild
            VoiceNextExt = Client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(Guild);

            if (VoiceConnection != null)
            {
                VoiceConnection.Disconnect();
                Client.DebugLogger.LogMessage(LogLevel.Warning, Assembly.GetExecutingAssembly().GetName().Name, $"An old connection was still up, successfully closed old one.", DateTime.Now);
            }

            // Get the default channel the bot should be in
            DiscordChannel DefaultChannel = Guild.Channels.Values.Where(x => x.Id == Config.DefaultChannel).ToList().FirstOrDefault();

            // Connect to the channel of the random selected member
            VoiceConnection = await VoiceNextExt.ConnectAsync(DefaultChannel);

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Connected to channel successfully.", DateTime.Now);

            await Task.Delay(-1);
        }

        // Data from configuration file
        private struct ConfigJson
        {
            [JsonProperty("DefaulChannel")]
            public ulong DefaultChannel { get; private set; }
            [JsonProperty("Prefix")]
            public string[] CommandPrefix { get; private set; }

            [JsonProperty("Token")]
            public string Token { get; private set; }
        }
    }
}
