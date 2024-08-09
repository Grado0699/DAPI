using Backend;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Gengbleng.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Gengbleng;

internal class Program {
    private static DiscordClient Client { get; set; }
    private static CommandsNextExtension ComNextExt { get; set; }

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
            MinimumLogLevel = LogLevel.Debug,
            LoggerFactory = logFactory,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
        });

        var services = new ServiceCollection().AddLogging().AddTransient<AudioStreamer>().BuildServiceProvider();

        IEventsClient clientEvents = new EventsClient();

        Client.Ready += clientEvents.Client_Ready;
        Client.GuildAvailable += clientEvents.Client_GuildAvailable;
        Client.ClientErrored += clientEvents.Client_ClientError;

        logger.LogInformation("Client initialized successfully");

        ComNextExt = Client.UseCommandsNext(new CommandsNextConfiguration {
            UseDefaultCommandHandler = true,
            StringPrefixes = ConfigLoader.CommandPrefix,
            CaseSensitive = false,
            EnableDefaultHelp = true,
            EnableMentionPrefix = true,
            DmHelp = false,
            EnableDms = false,
            Services = services
        });

        ComNextExt.CommandExecuted += clientEvents.Commands_CommandExecuted;
        ComNextExt.CommandErrored += clientEvents.Commands_CommandErrored;

        logger.LogInformation("Command-Handler initialized successfully");

        try {
            ComNextExt.RegisterCommands<Commands.Commands>();
            logger.LogInformation("Registered commands successfully");
        }
        catch (Exception exception) {
            logger.LogError($"An error occurred while registering the commands:\n{exception}\n\nPress any key to continue...");
            Console.ReadKey();

            return;
        }

        var slashCommandsExtension = Client.UseSlashCommands();
        slashCommandsExtension.RegisterCommands<SlashCommands>();
        
        Client.UseVoiceNext(new VoiceNextConfiguration {
            EnableIncoming = false
        });

        logger.LogInformation("Voice - Handler initialized successfully");

        Client.UseInteractivity(new InteractivityConfiguration {
            Timeout = TimeSpan.FromSeconds(30)
        });

        logger.LogInformation("Interactivity - Handler initialized successfully");

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