using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace Backend
{
    public interface IEventsClient
    {
        Task Client_Ready(DiscordClient sender, ReadyEventArgs readyEventArgs);
        Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs guildCreateEventArgs);
        Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs clientErrorEventArgs);

        Task Commands_CommandExecuted(CommandsNextExtension commandsNextExtension,
            CommandExecutionEventArgs commandExecutionEventArgs);

        Task Commands_CommandErrored(CommandsNextExtension commandsNextExtension,
            CommandErrorEventArgs commandErrorEventArgs);
    }
}