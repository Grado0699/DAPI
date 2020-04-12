﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;


namespace Gengbleng.Backend
{
    public class EventsClient
    {
        public async Task Client_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Client is ready to proceed events.", DateTime.Now);
            await Task.CompletedTask;
        }

        public async Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Guild available: {e.Guild.Name}", DateTime.Now);
            await Task.CompletedTask;
        }

        public async Task Client_ClientError(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            await Task.CompletedTask;
        }

        public async Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);
            await Task.CompletedTask;
        }

        public async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);
            await Task.CompletedTask;
        }
    }
}