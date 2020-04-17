using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace Backend
{
    public class EventsClient
    {
        private readonly Logger _logger;

        public EventsClient(DiscordClient discordClient)
        {
            _logger = new Logger(discordClient);
        }

        public async Task Client_Ready(ReadyEventArgs e)
        {
            if (e is null)
            {
                throw new System.ArgumentNullException(nameof(e));
            }

            _logger.Log("Client is ready to proceed events.", LogLevel.Info);
            await Task.CompletedTask;
        }

        public async Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            _logger.Log($"Guild available: {e.Guild.Name}", LogLevel.Info);
            await Task.CompletedTask;
        }

        public async Task Client_ClientError(ClientErrorEventArgs e)
        {
            _logger.Log($"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", LogLevel.Error);
            await Task.CompletedTask;
        }

        public async Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            _logger.Log($"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", LogLevel.Info);
            await Task.CompletedTask;
        }

        public async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            _logger.Log($"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", LogLevel.Error);
            await Task.CompletedTask;
        }
    }
}
