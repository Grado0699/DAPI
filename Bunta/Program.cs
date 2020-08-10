using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Bunta.Backend;
using Bunta.Commands;

namespace Bunta
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension ComNextExt { get; set; }

        private static void Main()
        {
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            try
            {
                await ConfigLoader.LoadConfigurationFromFile();
            }
            catch (FileNotFoundException fileNotFoundEx)
            {
                Console.WriteLine($"{fileNotFoundEx}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            catch (Exception Ex)
            {
                Console.WriteLine($"{Ex}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            var clientEvents = new EventsClient();

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigLoader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            Client.Ready += clientEvents.Client_Ready;
            Client.GuildAvailable += clientEvents.Client_GuildAvailable;
            Client.ClientErrored += clientEvents.Client_ClientError;

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name,
                "Client initialized successfully.", DateTime.Now);

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

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name,
                "Command-Handler initialized successfully.", DateTime.Now);

            try
            {
                ComNextExt.RegisterCommands<Core>();
                ComNextExt.RegisterCommands<Voice>();

                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name,
                    "Registered commands successfully.", DateTime.Now);
            }
            catch
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name,
                    "An error occured while registering the commands.", DateTime.Now);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                return;
            }

            Client.UseVoiceNext(new VoiceNextConfiguration
            {
                EnableIncoming = false
            });

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name,
                "Voice-Handler initialized successfully.", DateTime.Now);

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(30)
            });

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name,
                "Interactivity-Handler initialized successfully.", DateTime.Now);

            try
            {
                await Client.ConnectAsync();
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name,
                    "Connected to the API successfully.", DateTime.Now);
            }
            catch
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name,
                    "An error occured while connecting to the API. Maybe the wrong token was provided.", DateTime.Now);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                return;
            }

            await Task.Delay(-1);
        }
    }
}