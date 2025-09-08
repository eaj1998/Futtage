using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Futtage.Infrastructure.Configuration;

namespace Futtage.Infrastructure.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private readonly object _lock = new object();

        public FileLogger(IConfiguration configuration = null)
        {
            _logFilePath = GetLogPath(configuration);
            EnsureLogDirectoryExists();
        }

        private string GetLogPath(IConfiguration configuration)
        {
            try
            {
                if (configuration != null)
                {
                    var configPath = configuration["LoggingPath"];
                    if (!string.IsNullOrEmpty(configPath))
                    {
                        var expandedPath = ConfigurationHelper.ExpandEnvironmentPath(configPath);

                        if (!Path.HasExtension(expandedPath))
                        {
                            expandedPath = Path.Combine(expandedPath, $"futtage_{DateTime.Now:yyyyMMdd}.log");
                        }

                        if (ConfigurationHelper.ValidatePath(expandedPath))
                        {
                            return expandedPath;
                        }
                    }
                }

                // Fallback se configuração não funcionar
                return GetFallbackPath();
            }
            catch
            {
                return GetFallbackPath();
            }
        }

        private string GetFallbackPath()
        {
            var fallbacks = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", $"futtage_{DateTime.Now:yyyyMMdd}.log"),
                Path.Combine(Path.GetTempPath(), "Futtage", $"futtage_{DateTime.Now:yyyyMMdd}.log"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"futtage_{DateTime.Now:yyyyMMdd}.log")
            };

            foreach (var path in fallbacks)
            {
                if (ConfigurationHelper.ValidatePath(path))
                    return path;
            }

            return Path.Combine(Path.GetTempPath(), $"futtage_emergency_{DateTime.Now:yyyyMMdd}.log");
        }

        private void EnsureLogDirectoryExists()
        {
            try
            {
                var directory = Path.GetDirectoryName(_logFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch
            {
                // Falha silenciosa
            }
        }

        public void LogInfo(string message) => WriteLog("INFO", message);
        public void LogWarning(string message) => WriteLog("WARNING", message);
        public void LogError(string message, Exception exception = null)
        {
            var msg = exception != null ? $"{message} | {exception}" : message;
            WriteLog("ERROR", msg);
        }

        private void WriteLog(string level, string message)
        {
            try
            {
                lock (_lock)
                {
                    var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                    File.AppendAllText(_logFilePath, entry + Environment.NewLine);
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"LOG: {message}");
            }
        }

        public void LogDebug(string message)
        {
            throw new NotImplementedException();
        }
    }
}