using Backend;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using System;
using System.IO;
using System.Threading.Tasks;
using Toast_Stalin.Commands;

namespace Toast_Stalin
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension ComNextExt { get; set; }
        private static Logger Logger { get; set; }

        private static void Main()
        {
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            try
            {
                await ConfigLoader.LoadConfigurationFromFileAsync();
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Console.WriteLine($"{fileNotFoundException}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigLoader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            var clientEvents = new EventsClient(Client);

            Client.Ready += clientEvents.Client_Ready;
            Client.GuildAvailable += clientEvents.Client_GuildAvailable;
            Client.ClientErrored += clientEvents.Client_ClientError;

            Logger = new Logger(Client);
            Logger.Log("Client initialized successfully.", LogLevel.Info);

            ComNextExt = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                UseDefaultCommandHandler = true,
                StringPrefixes = ConfigLoader.CommandPrefix,
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableMentionPrefix = true,
                DmHelp = false,
                EnableDms = false
            });

            ComNextExt.CommandExecuted += clientEvents.Commands_CommandExecuted;
            ComNextExt.CommandErrored += clientEvents.Commands_CommandErrored;

            Logger.Log("Command - Handler initialized successfully.", LogLevel.Info);

            try
            {
                ComNextExt.RegisterCommands<Core>();
                ComNextExt.RegisterCommands<Voice>();
                Logger.Log("Registered commands successfully.", LogLevel.Info);
            }
            catch (Exception exception)
            {
                Logger.Log($"An error occured while registering the commands.\n{exception}", LogLevel.Error);

                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();

                return;
            }

            Client.UseVoiceNext(new VoiceNextConfiguration
            {
                EnableIncoming = false
            });

            Logger.Log("Voice - Handler initialized successfully.", LogLevel.Info);

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(30)
            });

            Logger.Log("Interactivity - Handler initialized successfully.", LogLevel.Info);

            try
            {
                await Client.ConnectAsync();
                Logger.Log("Connected to the API successfully.", LogLevel.Info);
            }
            catch (Exception exception)
            {
                Logger.Log(
                    $"An error occured while connecting to the API. Maybe the wrong token was provided.\n{exception}",
                    LogLevel.Error);

                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();

                return;
            }

            await Task.Delay(-1);
        }
    }
}