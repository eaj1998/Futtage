using System;
using System.IO;

namespace Futtage.Infrastructure.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();

        public FileLogger(string logFilePath = null)
        {
            _logFilePath = logFilePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Futtage",
                "Logs",
                $"app_{DateTime.Now:yyyyMMdd}.log"
            );

            // Criar diretório se não existir
            var directory = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void LogDebug(string message)
        {
            WriteLog("DEBUG", message);
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        public void LogError(string message, Exception exception = null)
        {
            var fullMessage = exception != null
                ? $"{message}\nException: {exception}"
                : message;

            WriteLog("ERROR", fullMessage);
        }

        private void WriteLog(string level, string message)
        {
            lock (_lockObject)
            {
                try
                {
                    var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                    File.AppendAllText(_logFilePath, logEntry);
                }
                catch
                {
                    // Falha silenciosa para evitar loops de erro
                }
            }
        }
    }
}