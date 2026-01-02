using System;
using System.Collections.Generic;
using RadialCore.Core.Diagnostics;

namespace RadialCore.Core.Services
{
    /// <summary>
    /// Résolveur de capabilities (services/interfaces) pour les plugins.
    /// Permet aux plugins de demander des services optionnels sans dépendances dures.
    /// Exemple: plugin demande ILocalizationProvider, le Core fournit si disponible.
    /// Pattern: Service Locator simplifié + capability-based.
    /// </summary>
    public class CapabilityResolver
    {
        private readonly Dictionary<Type, object> _capabilities = new Dictionary<Type, object>();
        private readonly object _lock = new object();

        /// <summary>
        /// Enregistre une capability (service/interface).
        /// </summary>
        public void RegisterCapability<T>(T implementation) where T : class
        {
            if (implementation == null)
                throw new ArgumentNullException(nameof(implementation));

            lock (_lock)
            {
                var type = typeof(T);
                
                if (_capabilities.ContainsKey(type))
                {
                    Logger.Warning("CapabilityResolver", $"Capability {type.Name} already registered, overwriting");
                }

                _capabilities[type] = implementation;
                Logger.Debug("CapabilityResolver", $"Registered capability: {type.Name}");
            }
        }

        /// <summary>
        /// Résout une capability. Retourne null si non disponible.
        /// Les plugins doivent gérer gracefully l'absence de capability.
        /// </summary>
        public T? ResolveCapability<T>() where T : class
        {
            lock (_lock)
            {
                var type = typeof(T);
                
                if (_capabilities.TryGetValue(type, out object? implementation))
                {
                    return implementation as T;
                }

                Logger.Debug("CapabilityResolver", $"Capability {type.Name} not available");
                return null;
            }
        }

        /// <summary>
        /// Vérifie si une capability est disponible.
        /// </summary>
        public bool HasCapability<T>() where T : class
        {
            lock (_lock)
            {
                return _capabilities.ContainsKey(typeof(T));
            }
        }

        /// <summary>
        /// Unregister une capability (cleanup).
        /// </summary>
        public void UnregisterCapability<T>() where T : class
        {
            lock (_lock)
            {
                var type = typeof(T);
                if (_capabilities.Remove(type))
                {
                    Logger.Debug("CapabilityResolver", $"Unregistered capability: {type.Name}");
                }
            }
        }

        /// <summary>
        /// Liste toutes les capabilities enregistrées (debug).
        /// </summary>
        public IEnumerable<Type> GetRegisteredCapabilities()
        {
            lock (_lock)
            {
                return new List<Type>(_capabilities.Keys);
            }
        }

        /// <summary>
        /// Clear toutes les capabilities (shutdown).
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                Logger.Info("CapabilityResolver", $"Clearing {_capabilities.Count} capabilities");
                _capabilities.Clear();
            }
        }
    }
}
