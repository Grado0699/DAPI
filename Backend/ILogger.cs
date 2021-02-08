using Microsoft.Extensions.Logging;

namespace Backend
{
    public interface ILogger
    {
        void Log(string logMessage, LogLevel logLevel);
    }
}