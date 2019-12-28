using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using Luigis_Pizza.Commands;
using Luigis_Pizza.Backend;

namespace Luigis_Pizza
{
    class Program
    {
        private static DiscordClient Client { get; set; } = null;
        private static CommandsNextExtension ComNextExt { get; set; } = null;
        private static Timer ClientTimer { get; set; } = null;

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
            catch(FileNotFoundException FileNotFoundEx)
            {
                Console.WriteLine($"{FileNotFoundEx}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
            catch(Exception Ex)
            {
                Console.WriteLine($"{Ex}\n\nPress any key to continue...");
                Console.ReadKey();
                return;
            }
   
            var ClientEvents = new EventsClient();

            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigLoader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            Client.Ready += ClientEvents.Client_Ready;
            Client.GuildAvailable += ClientEvents.Client_GuildAvailable;
            Client.ClientErrored += ClientEvents.Client_ClientError;

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Client initialized successfully.", DateTime.Now);

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

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Command-Handler initialized successfully.", DateTime.Now);
            
            try
            {
                ComNextExt.RegisterCommands<Core>();
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Registered commands successfully.", DateTime.Now);
            }
            catch
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, "An error occured while registering the commands.", DateTime.Now);

                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();

                return;
            }

            Client.UseVoiceNext(new VoiceNextConfiguration
            {
                EnableIncoming = false
            });

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Voice-Handler initialized successfully.", DateTime.Now);

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(30)
            });

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Interactivity-Handler initialized successfully.", DateTime.Now);

            try
            {
                await Client.ConnectAsync();
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Connected to the API successfully.", DateTime.Now);
            }
            catch
            {
                Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, "An error occured while connecting to the API. Maybe the wrong token was provided.", DateTime.Now);

                Console.WriteLine($"Press any key to continue...");
                Console.ReadKey();

                return;
            }

            ClientTimer = new Timer(Core.TimerSpan);
            ClientTimer.Elapsed += ClientTimer_Elapsed;

            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Timer initialized successfully.", DateTime.Now);

            ClientTimer.Start();
            await Task.Delay(-1);
        }

        private static async void ClientTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Timer-Elapsed-Event executed.", DateTime.Now);

            ClientTimer.Stop();

            if (Core.EnableTimer)
            {
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Timer is enabled: going to start the worker.", DateTime.Now);

                try
                {
                    var ClientWorker = new Worker();
                    await ClientWorker.StartWorker(Client);
                }
                catch (ArgumentNullException ArgumentNullEx)
                {
                    Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"{ArgumentNullEx}", DateTime.Now);
                }
                catch (FileNotFoundException FileNotFoundEx)
                {
                    Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"{FileNotFoundEx}", DateTime.Now);
                }
                catch (PlatformNotSupportedException PlatformNotSupportedEx)
                {
                    Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"{PlatformNotSupportedEx}", DateTime.Now);
                }
                catch (Exception Ex)
                {
                    Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"{Ex}", DateTime.Now);
                }
            }
            else
            {
                Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Timer is disabled.", DateTime.Now);
            }

            ClientTimer.Interval = Core.TimerSpan;
            ClientTimer.Start();
        }
    }
}
