using DSharpPlus;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace Backend {
    public class Logger : ILogger {
        private readonly DiscordClient _discordClient;

        public Logger(DiscordClient discordClient) {
            _discordClient = discordClient;
        }

        /// <summary>
        ///     Logs a message.
        /// </summary>
        public void Log(string logMessage, LogLevel logLevel) {
            _discordClient.Logger.Log(logLevel, Assembly.GetExecutingAssembly().GetName().Name, logMessage, DateTime.Now);
        }
    }
}