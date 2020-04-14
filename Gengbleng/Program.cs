﻿using Backend;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Gengbleng.Backend;
using Gengbleng.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace Gengbleng
{
    class Program
    {
        private static DiscordClient Client { get; set; } = null;
        private static CommandsNextExtension ComNextExt { get; set; } = null;
        private static Timer ClientTimer { get; set; } = null;
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
            catch (FileNotFoundException FileNotFoundEx)
            {
                Console.WriteLine($"{FileNotFoundEx}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            catch (Exception Ex)
            {
                Console.WriteLine($"{Ex}\n\nPress any key to continue...");
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

            var ClientEvents = new EventsClient(Client);

            Client.Ready += ClientEvents.Client_Ready;
            Client.GuildAvailable += ClientEvents.Client_GuildAvailable;
            Client.ClientErrored += ClientEvents.Client_ClientError;

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

            ComNextExt.CommandExecuted += ClientEvents.Commands_CommandExecuted;
            ComNextExt.CommandErrored += ClientEvents.Commands_CommandErrored;

            Logger.Log("Command-Handler initialized successfully.", LogLevel.Info);

            try
            {
                ComNextExt.RegisterCommands<Core>();
                Logger.Log("Registered commands successfully.", LogLevel.Info);
            }
            catch (Exception Ex)
            {
                Logger.Log($"An error occured while registering the commands.\n{Ex}", LogLevel.Error);

                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();

                return;
            }

            Client.UseVoiceNext(new VoiceNextConfiguration
            {
                EnableIncoming = false
            });

            Logger.Log("Voice-Handler initialized successfully.", LogLevel.Info);

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
            catch (Exception Ex)
            {
                Logger.Log($"An error occured while connecting to the API. Maybe the wrong token was provided.\n{Ex}", LogLevel.Error);

                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();

                return;
            }

            ClientTimer = new Timer(Core.TimerSpan);
            ClientTimer.Elapsed += ClientTimer_Elapsed;

            Logger.Log("Timer initialized successfully.", LogLevel.Info);

            ClientTimer.Start();
            await Task.Delay(-1);
        }

        private static async void ClientTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Log("Timer-Elapsed-Event executed.", LogLevel.Debug);

            ClientTimer.Stop();

            if (Core.EnableTimer)
            {
                Logger.Log("Timer is enabled: going to start the worker.", LogLevel.Info);

                try
                {
                    var Streamer = new Streamer(Client);
                    await Streamer.PlayRandomSoundFile();
                }
                catch (ArgumentNullException ArgumentNullEx)
                {
                    Logger.Log($"{ArgumentNullEx}", LogLevel.Error);
                }
                catch (FileNotFoundException FileNotFoundEx)
                {
                    Logger.Log($"{FileNotFoundEx}", LogLevel.Error);
                }
                catch (PlatformNotSupportedException PlatformNotSupportedEx)
                {
                    Logger.Log($"{PlatformNotSupportedEx}", LogLevel.Error);
                }
                catch (Exception Ex)
                {
                    Logger.Log($"{Ex}", LogLevel.Error);
                }
            }
            else
            {
                Logger.Log("Timer is disabled.", LogLevel.Warning);
            }

            ClientTimer.Interval = Core.TimerSpan;
            ClientTimer.Start();
        }
    }
}
