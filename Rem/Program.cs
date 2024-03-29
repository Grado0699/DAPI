﻿using Backend;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using Rem.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using ILogger = Backend.ILogger;

namespace Rem {
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
            catch (FileNotFoundException fileNotFoundException) {
                Console.WriteLine($"{fileNotFoundException}\n");
                return;
            }
            catch (Exception exception) {
                Console.WriteLine($"{exception}\n");
                return;
            }

            Client = new DiscordClient(new DiscordConfiguration {
                Token = ConfigLoader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            });

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

            Logger.Log("Command - Handler initialized successfully.", LogLevel.Information);

            try {
                ComNextExt.RegisterCommands<MusicPlayer>();
                Logger.Log("Registered commands successfully.", LogLevel.Information);
            }
            catch (Exception exception) {
                Logger.Log($"An error occurred while registering the commands.\n{exception}", LogLevel.Error);
                return;
            }

            var lavaLinkEndpoint = new ConnectionEndpoint {
                Hostname = "127.0.0.1",
                Port = 2333
            };

            var lavaLinkConfig = new LavalinkConfiguration {
                Password = "12344",
                RestEndpoint = lavaLinkEndpoint,
                SocketEndpoint = lavaLinkEndpoint
            };

            var lavaLinkExtension = Client.UseLavalink();

            Client.UseVoiceNext(new VoiceNextConfiguration {
                EnableIncoming = false
            });

            Logger.Log("Voice - Handler initialized successfully.", LogLevel.Information);

            Client.UseInteractivity(new InteractivityConfiguration {
                Timeout = TimeSpan.FromSeconds(30)
            });

            Logger.Log("Interactivity - Handler initialized successfully.", LogLevel.Information);

            try {
                await Client.ConnectAsync();
                Logger.Log("Connected to the API successfully.", LogLevel.Information);
            }
            catch (Exception exception) {
                Logger.Log($"An error occurred while connecting to the API. Maybe the wrong token was provided.\n{exception}", LogLevel.Error);
                return;
            }

            try {
                await lavaLinkExtension.ConnectAsync(lavaLinkConfig);
                Logger.Log("Connected to the LavaLink player successfully.", LogLevel.Information);
            }
            catch (Exception exception) {
                Logger.Log($"An error occurred while connecting to the LavaLink player. Maybe the wrong credentials were provided.\n{exception}", LogLevel.Error);
                return;
            }

            await Task.Delay(-1);
        }
    }
}