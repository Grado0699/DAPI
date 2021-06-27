using Backend;
using Bunta.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using ILogger = Backend.ILogger;

namespace Bunta {
    internal class Program {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension ComNextExt { get; set; }
        private static ILogger Logger { get; set; }

        private static void Main() {
            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync() {
            try {
                await ConfigLoader.LoadConfigurationFromFileAsync();
            }
            catch (FileNotFoundException fileNotFoundEx) {
                Console.WriteLine($"{fileNotFoundEx}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            catch (Exception ex) {
                Console.WriteLine($"{ex}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            Client = new DiscordClient(new DiscordConfiguration {Token = ConfigLoader.Token, TokenType = TokenType.Bot, AutoReconnect = true, MinimumLogLevel = LogLevel.Debug});

            IEventsClient clientEvents = new EventsClient(Client);

            Client.Ready += clientEvents.Client_Ready;
            Client.GuildAvailable += clientEvents.Client_GuildAvailable;
            Client.ClientErrored += clientEvents.Client_ClientError;

            Logger = new Logger(Client);

            Logger.Log("Client initialized successfully.", LogLevel.Information);

            ComNextExt = Client.UseCommandsNext(new CommandsNextConfiguration {
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

            Logger.Log("Command-Handler initialized successfully.", LogLevel.Information);

            try {
                ComNextExt.RegisterCommands<Core>();
                ComNextExt.RegisterCommands<Voice>();

                Logger.Log("Registered commands successfully.", LogLevel.Information);
            }
            catch {
                Logger.Log("An error occurred while registering the commands.", LogLevel.Error);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                return;
            }

            Client.UseVoiceNext(new VoiceNextConfiguration {EnableIncoming = false});

            Logger.Log("Voice-Handler initialized successfully.", LogLevel.Information);

            Client.UseInteractivity(new InteractivityConfiguration {Timeout = TimeSpan.FromSeconds(30)});

            Logger.Log("Interactivity-Handler initialized successfully.", LogLevel.Information);

            try {
                await Client.ConnectAsync();
                Logger.Log("Connected to the API successfully.", LogLevel.Information);
            }
            catch {
                Logger.Log("An error occurred while connecting to the API. Maybe the wrong token was provided.", LogLevel.Error);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                return;
            }

            await Task.Delay(-1);
        }
    }
}