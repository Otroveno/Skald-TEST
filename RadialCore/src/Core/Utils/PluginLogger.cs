using System;
using RadialCore.Core.Diagnostics;

namespace RadialCore.Core.Utils
{
    /// <summary>
    /// Utilitaires de logging pour les plugins.
    /// Wraps Logger.cs avec des helpers pour plugins.
    /// </summary>
    public static class PluginLogger
    {
        /// <summary>
        /// Log un message d'info pour un plugin.
        /// </summary>
        public static void Info(string pluginId, string message, string? phase = null)
        {
            Logger.Info("Plugin", message, pluginId: pluginId, phase: phase);
        }

        /// <summary>
        /// Log un warning pour un plugin.
        /// </summary>
        public static void Warning(string pluginId, string message, string? phase = null)
        {
            Logger.Warning("Plugin", message, pluginId: pluginId, phase: phase);
        }

        /// <summary>
        /// Log une erreur pour un plugin.
        /// </summary>
        public static void Error(string pluginId, string message, Exception? exception = null, string? phase = null)
        {
            Logger.Error("Plugin", message, exception, pluginId: pluginId, phase: phase);
        }

        /// <summary>
        /// Log un debug pour un plugin.
        /// </summary>
        public static void Debug(string pluginId, string message, string? phase = null)
        {
            Logger.Debug("Plugin", message, pluginId: pluginId, phase: phase);
        }

        /// <summary>
        /// Log une action plugin (execution start/end).
        /// </summary>
        public static void LogActionStart(string pluginId, string actionId)
        {
            Logger.Debug("ActionHandler", $"Executing action: {actionId}", pluginId: pluginId, phase: "Execute");
        }

        /// <summary>
        /// Log fin d'action avec résultat.
        /// </summary>
        public static void LogActionEnd(string pluginId, string actionId, bool success, string? message = null)
        {
            string resultMsg = $"Action {actionId} {(success ? "succeeded" : "failed")}";
            if (!string.IsNullOrEmpty(message))
            {
                resultMsg += $": {message}";
            }

            if (success)
            {
                Logger.Debug("ActionHandler", resultMsg, pluginId: pluginId, phase: "Execute");
            }
            else
            {
                Logger.Warning("ActionHandler", resultMsg, pluginId: pluginId, phase: "Execute");
            }
        }

        /// <summary>
        /// Log une exception plugin avec context.
        /// </summary>
        public static void LogException(string pluginId, string operation, Exception exception)
        {
            Logger.Error("Plugin", $"Exception during {operation}", exception, pluginId: pluginId, phase: operation);
        }
    }
}
