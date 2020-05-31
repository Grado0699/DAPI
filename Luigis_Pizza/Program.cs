using Backend;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Luigis_Pizza.Backend;
using Luigis_Pizza.Commands;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace Luigis_Pizza
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
            catch (Exception exception)
            {
                Logger.Log($"An error occured while connecting to the API. Maybe the wrong token was provided.\n{exception}", LogLevel.Error);

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
            Logger.Log("Timer-Elapsed-Event executed.", LogLevel.Info);

            ClientTimer.Stop();

            if (Core.EnableTimer)
            {
                Logger.Log("Timer-Elapsed-Event executed.", LogLevel.Info);
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Timer is enabled: going to start the worker.", DateTime.Now);

                try
                {
                    var ClientWorker = new Worker(Client);
                    await ClientWorker.StartWorkerAsync();
                }
                catch (ArgumentNullException argumentNullException)
                {
                    Logger.Log($"{argumentNullException}", LogLevel.Error);
                }
                catch (FileNotFoundException fileNotFoundException)
                {
                    Logger.Log($"{fileNotFoundException}", LogLevel.Error);
                }
                catch (PlatformNotSupportedException platformNotSupportedException)
                {
                    Logger.Log($"{platformNotSupportedException}", LogLevel.Error);
                }
                catch (Exception exception)
                {
                    Logger.Log($"{exception}", LogLevel.Error);
                }
            }
            else
            {
                Logger.Log("Timer is disabled.", LogLevel.Info);
            }

            ClientTimer.Interval = Core.TimerSpan;
            ClientTimer.Start();
        }
    }
}
