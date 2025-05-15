using Backend;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Erika;

internal class Program {
    private static DiscordClient? Client { get; set; }

    private static void Main() {
        MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private static async Task MainAsync() {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        var logFactory = new LoggerFactory().AddSerilog();
        var logger = logFactory.CreateLogger<Program>();

        try {
            await ConfigLoader.LoadConfigurationFromFileAsync();
        }
        catch (FileNotFoundException fileNotFoundException) {
            logger.LogError($"{fileNotFoundException}\n\nPress any key to continue...");
            Console.ReadKey();

            return;
        }
        catch (Exception exception) {
            logger.LogError($"{exception}\n\nPress any key to continue...");
            Console.ReadKey();

            return;
        }

        Client = new DiscordClient(new DiscordConfiguration {
            Token = ConfigLoader.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.Trace,
            LoggerFactory = logFactory,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.Guilds | DiscordIntents.GuildVoiceStates | DiscordIntents.GuildMembers
        });

        // var services = new ServiceCollection().AddLogging().AddSingleton<AudioStreamer>().BuildServiceProvider();
        var services = new ServiceCollection().AddSingleton(Log.Logger).AddLogging(x => x.AddSerilog()).BuildServiceProvider();

        IEventsClient clientEvents = new EventsClient();

        Client.Ready += clientEvents.Client_Ready;
        Client.GuildAvailable += clientEvents.Client_GuildAvailable;
        Client.ClientErrored += clientEvents.Client_ClientError;

        logger.LogInformation("Command-Handler initialized successfully");

        try {
            var slashCommandsExtension = Client.UseSlashCommands(new SlashCommandsConfiguration {
                Services = services
            });
            slashCommandsExtension.RegisterCommands<Commands.Commands>();

            logger.LogInformation("Registered commands successfully");
        }
        catch (Exception exception) {
            logger.LogError($"An error occurred while registering the commands:\n{exception}\n\nPress any key to continue...");
            Console.ReadKey();

            return;
        }

        Client.UseVoiceNext(new VoiceNextConfiguration {
            EnableIncoming = false
        });

        logger.LogInformation("Voice - Handler initialized successfully");

        try {
            await Client.ConnectAsync();
            logger.LogInformation("Connected to the API successfully");
        }
        catch (Exception exception) {
            logger.LogError($"An error occurred while connecting to the API. Maybe the wrong token was provided?\n{exception}\n\nPress any key to continue...");
            Console.ReadKey();

            return;
        }

        await Task.Delay(-1);
    }
}