using System;

namespace RadialCore.Contracts.Models
{
    /// <summary>
    /// Résultat de l'exécution d'une action.
    /// Retourné par IActionHandler.ExecuteAsync().
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Succès ou échec de l'action.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message de résultat (affiché en notification).
        /// Optionnel, peut être vide.
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Type de résultat (pour affichage notification).
        /// </summary>
        public ResultType Type { get; set; } = ResultType.Info;

        /// <summary>
        /// Exception si échec (optionnel, pour logging).
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Données custom retournées par l'action.
        /// Peut être utilisé pour passer des données au Core ou autres plugins.
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Helper: crée un résultat de succès.
        /// </summary>
        public static ActionResult CreateSuccess(string message = "")
        {
            return new ActionResult
            {
                Success = true,
                Message = message,
                Type = ResultType.Success
            };
        }

        /// <summary>
        /// Helper: crée un résultat d'échec.
        /// </summary>
        public static ActionResult CreateFailure(string message, Exception? exception = null)
        {
            return new ActionResult
            {
                Success = false,
                Message = message,
                Type = ResultType.Error,
                Exception = exception
            };
        }

        /// <summary>
        /// Helper: crée un résultat d'info.
        /// </summary>
        public static ActionResult CreateInfo(string message)
        {
            return new ActionResult
            {
                Success = true,
                Message = message,
                Type = ResultType.Info
            };
        }

        /// <summary>
        /// Helper: crée un résultat de warning.
        /// </summary>
        public static ActionResult CreateWarning(string message)
        {
            return new ActionResult
            {
                Success = true,
                Message = message,
                Type = ResultType.Warning
            };
        }
    }

    /// <summary>
    /// Type de résultat (pour styling notification).
    /// </summary>
    public enum ResultType
    {
        Info,
        Success,
        Warning,
        Error
    }
}
