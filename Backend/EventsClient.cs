using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Backend;

public class EventsClient : IEventsClient {
    private readonly ILogger _logger = new LoggerFactory().AddSerilog().CreateLogger<EventsClient>();

    public async Task Client_Ready(DiscordClient sender, ReadyEventArgs readyEventArgs) {
        _logger.LogInformation("Client is ready to proceed events.");
        await Task.CompletedTask;
    }

    public async Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs guildCreateEventArgs) {
        _logger.LogInformation($"Guild available: {guildCreateEventArgs.Guild.Name}");
        await Task.CompletedTask;
    }

    public async Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs clientErrorEventArgs) {
        _logger.LogError($"Exception occurred: {clientErrorEventArgs.Exception.GetType()}: {clientErrorEventArgs.Exception.Message}");
        await Task.CompletedTask;
    }

    public async Task Commands_CommandExecuted(CommandsNextExtension commandsNextExtension, CommandExecutionEventArgs commandExecutionEventArgs) {
        _logger.LogInformation($"{commandExecutionEventArgs.Context.User.Username} successfully executed '{commandExecutionEventArgs.Command.QualifiedName}'");
        await Task.CompletedTask;
    }

    public async Task Commands_CommandErrored(CommandsNextExtension commandsNextExtension, CommandErrorEventArgs commandErrorEventArgs) {
        _logger.LogError(
            $"{commandErrorEventArgs.Context.User.Username} tried executing '{commandErrorEventArgs.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {commandErrorEventArgs.Exception.GetType()}: {commandErrorEventArgs.Exception.Message}");
        await Task.CompletedTask;
    }
}