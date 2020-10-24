using DSharpPlus;
using System;
using System.Reflection;

namespace Backend
{
    public class Logger
    {
        private readonly DiscordClient _discordClient;

        public Logger(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        /// <summary>
        ///     Logs a message.
        /// </summary>
        public void Log(string logMessage, LogLevel logLevel)
        {
            _discordClient.DebugLogger.LogMessage(logLevel, Assembly.GetExecutingAssembly().GetName().Name, logMessage,
                DateTime.Now);
        }
    }
}