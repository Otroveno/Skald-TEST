using System;
using RadialCore.Core.Diagnostics;

namespace RadialCore.Core
{
    /// <summary>
    /// Circuit Breaker pour protéger le Core des exceptions plugins.
    /// Si un plugin throw trop d'exceptions, il est automatiquement désactivé.
    /// Pattern: fail-safe, isolation d'erreurs.
    /// </summary>
    public class CircuitBreaker
    {
        private readonly string _pluginId;
        private int _failureCount = 0;
        private const int MAX_FAILURES = 5;
        private bool _isOpen = false; // true = circuit ouvert = plugin désactivé

        public CircuitBreaker(string pluginId)
        {
            _pluginId = pluginId ?? throw new ArgumentNullException(nameof(pluginId));
        }

        /// <summary>
        /// Exécute une action protégée par circuit breaker.
        /// Si l'action throw, incrémente failure count.
        /// Si MAX_FAILURES atteint, ouvre le circuit (désactive le plugin).
        /// </summary>
        public bool Execute(Func<bool> action, string operationName)
        {
            if (_isOpen)
            {
                // Circuit ouvert: plugin désactivé, ne pas exécuter
                return false;
            }

            try
            {
                bool result = action();
                
                // Success: reset failure count
                if (result && _failureCount > 0)
                {
                    Logger.Debug("CircuitBreaker", $"Plugin {_pluginId} recovered, resetting failure count", _pluginId);
                    _failureCount = 0;
                }

                return result;
            }
            catch (Exception ex)
            {
                _failureCount++;
                
                Logger.Error("CircuitBreaker",
                    $"Exception in plugin {_pluginId} during {operationName} (failure {_failureCount}/{MAX_FAILURES})",
                    ex,
                    _pluginId,
                    operationName);

                if (_failureCount >= MAX_FAILURES)
                {
                    _isOpen = true;
                    Logger.Error("CircuitBreaker",
                        $"Circuit breaker OPEN for plugin {_pluginId} - plugin disabled after {MAX_FAILURES} failures",
                        pluginId: _pluginId);
                }

                return false;
            }
        }

        /// <summary>
        /// Vérifie si le circuit est ouvert (plugin désactivé).
        /// </summary>
        public bool IsOpen => _isOpen;

        /// <summary>
        /// Reset manuel du circuit breaker (pour debug/admin).
        /// </summary>
        public void Reset()
        {
            _failureCount = 0;
            _isOpen = false;
            Logger.Info("CircuitBreaker", $"Circuit breaker reset for plugin {_pluginId}", _pluginId);
        }
    }
}
