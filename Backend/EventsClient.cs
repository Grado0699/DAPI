using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Backend
{
    public class EventsClient : IEventsClient
    {
        private readonly ILogger _logger;

        public EventsClient(DiscordClient discordClient)
        {
            _logger = new Logger(discordClient);
        }

        public async Task Client_Ready(DiscordClient sender, ReadyEventArgs readyEventArgs)
        {
            _logger.Log("Client is ready to proceed events.", LogLevel.Information);
            await Task.CompletedTask;
        }

        public async Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs guildCreateEventArgs)
        {
            _logger.Log($"Guild available: {guildCreateEventArgs.Guild.Name}", LogLevel.Information);
            await Task.CompletedTask;
        }

        public async Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs clientErrorEventArgs)
        {
            _logger.Log(
                $"Exception occured: {clientErrorEventArgs.Exception.GetType()}: {clientErrorEventArgs.Exception.Message}",
                LogLevel.Error);
            await Task.CompletedTask;
        }

        public async Task Commands_CommandExecuted(CommandsNextExtension commandsNextExtension,
            CommandExecutionEventArgs commandExecutionEventArgs)
        {
            _logger.Log(
                $"{commandExecutionEventArgs.Context.User.Username} successfully executed '{commandExecutionEventArgs.Command.QualifiedName}'",
                LogLevel.Information);
            await Task.CompletedTask;
        }

        public async Task Commands_CommandErrored(CommandsNextExtension commandsNextExtension,
            CommandErrorEventArgs commandErrorEventArgs)
        {
            _logger.Log(
                $"{commandErrorEventArgs.Context.User.Username} tried executing '{commandErrorEventArgs.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {commandErrorEventArgs.Exception.GetType()}: {commandErrorEventArgs.Exception.Message}",
                LogLevel.Error);
            await Task.CompletedTask;
        }
    }
}