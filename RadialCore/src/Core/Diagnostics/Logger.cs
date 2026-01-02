using System;
using System.IO;
using System.Text;

namespace RadialCore.Core.Diagnostics
{
    /// <summary>
    /// Système de logging structuré pour RadialCore.
    /// Logs formatés avec timestamp, source, niveau, pluginId, phase.
    /// Thread-safe, file-based avec rotation.
    /// </summary>
    public static class Logger
    {
        private static string? _logFilePath;
        private static readonly object _lock = new object();
        private static bool _initialized = false;
        private static string _moduleName = "RadialCore";

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        /// <summary>
        /// Initialise le logger avec le nom du module.
        /// Crée le fichier de log dans My Documents/Mount and Blade II Bannerlord/Logs/RadialCore/
        /// </summary>
        public static void Initialize(string moduleName)
        {
            lock (_lock)
            {
                if (_initialized) return;

                _moduleName = moduleName;
                
                try
                {
                    string myDocsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string logDir = Path.Combine(myDocsPath, "Mount and Blade II Bannerlord", "Logs", _moduleName);
                    Directory.CreateDirectory(logDir);

                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    _logFilePath = Path.Combine(logDir, $"{_moduleName}_{timestamp}.log");

                    _initialized = true;

                    Info("Logger", "Logger initialized successfully");
                    Info("Logger", $"Log file: {_logFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{_moduleName}] CRITICAL: Failed to initialize logger: {ex.Message}");
                }
            }
        }

        public static void Debug(string source, string message, string? pluginId = null, string? phase = null)
        {
            Log(LogLevel.Debug, source, message, pluginId, phase);
        }

        public static void Info(string source, string message, string? pluginId = null, string? phase = null)
        {
            Log(LogLevel.Info, source, message, pluginId, phase);
        }

        public static void Warning(string source, string message, string? pluginId = null, string? phase = null)
        {
            Log(LogLevel.Warning, source, message, pluginId, phase);
        }

        public static void Error(string source, string message, Exception? ex = null, string? pluginId = null, string? phase = null)
        {
            string fullMessage = message;
            if (ex != null)
            {
                fullMessage += $"\nException: {ex.GetType().Name}: {ex.Message}\nStackTrace:\n{ex.StackTrace}";
            }
            Log(LogLevel.Error, source, fullMessage, pluginId, phase);
        }

        private static void Log(LogLevel level, string source, string message, string? pluginId, string? phase)
        {
            if (!_initialized) return;

            lock (_lock)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.Append($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]");
                    sb.Append($" [{level.ToString().ToUpper()}]");
                    sb.Append($" [{source}]");
                    
                    if (!string.IsNullOrEmpty(pluginId))
                        sb.Append($" [Plugin:{pluginId}]");
                    
                    if (!string.IsNullOrEmpty(phase))
                        sb.Append($" [Phase:{phase}]");
                    
                    sb.Append($" {message}");

                    string logLine = sb.ToString();

                    // Console output
                    Console.WriteLine($"[{_moduleName}] {logLine}");

                    // File output
                    if (!string.IsNullOrEmpty(_logFilePath))
                    {
                        File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{_moduleName}] CRITICAL: Logging failed: {ex.Message}");
                }
            }
        }

        public static void Shutdown()
        {
            lock (_lock)
            {
                if (_initialized)
                {
                    Info("Logger", "Shutting down logger");
                    _initialized = false;
                }
            }
        }
    }
}
