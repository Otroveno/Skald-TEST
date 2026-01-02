using System;
using System.Collections.Generic;
using RadialCore.Contracts;
using RadialCore.Contracts.Core;
using RadialCore.Core.Diagnostics;
using RadialCore.Core.Services;

namespace RadialCore.Core
{
    /// <summary>
    /// Implémentation du contexte d'initialisation de plugin.
    /// Fourni à chaque plugin lors de Initialize().
    /// </summary>
    internal class PluginInitializationContextImpl : IPluginInitializationContext
    {
        private readonly string _pluginId;
        private readonly ModPresenceService _modPresenceService;
        private readonly CapabilityResolver _capabilityResolver;

        private readonly List<object> _registeredProviders = new List<object>();

        public PluginInitializationContextImpl(
            string pluginId,
            ModPresenceService modPresenceService,
            CapabilityResolver capabilityResolver)
        {
            _pluginId = pluginId;
            _modPresenceService = modPresenceService;
            _capabilityResolver = capabilityResolver;
        }

        public void RegisterMenuProvider(IMenuProvider provider)
        {
            _registeredProviders.Add(provider);
            LogInfo($"Registered MenuProvider: {provider.GetType().Name} (ID: {provider.ProviderId})");
        }

        public void RegisterActionHandler(IActionHandler handler)
        {
            _registeredProviders.Add(handler);
            LogInfo($"Registered ActionHandler: {handler.GetType().Name} (ID: {handler.HandlerId})");
        }

        public void RegisterPanelProvider(IPanelProvider provider)
        {
            _registeredProviders.Add(provider);
            LogInfo($"Registered PanelProvider: {provider.GetType().Name} (ID: {provider.ProviderId})");
        }

        public void RegisterContextProvider(IContextProvider provider)
        {
            _registeredProviders.Add(provider);
            LogInfo($"Registered ContextProvider: {provider.GetType().Name} (ID: {provider.ProviderId})");
        }

        public void RegisterConditionEvaluator(IConditionEvaluator evaluator)
        {
            _registeredProviders.Add(evaluator);
            LogInfo($"Registered ConditionEvaluator: {evaluator.GetType().Name} (ID: {evaluator.EvaluatorId})");
        }

        public T? ResolveCapability<T>() where T : class
        {
            return _capabilityResolver.ResolveCapability<T>();
        }

        public bool IsModLoaded(string modId)
        {
            return _modPresenceService.IsModLoaded(modId);
        }

        public void LogInfo(string message)
        {
            Logger.Info("Plugin", message, pluginId: _pluginId);
        }

        public void LogWarning(string message)
        {
            Logger.Warning("Plugin", message, pluginId: _pluginId);
        }

        public void LogError(string message, Exception? exception = null)
        {
            Logger.Error("Plugin", message, exception, pluginId: _pluginId);
        }

        /// <summary>
        /// Interne: récupère tous les providers d'un type spécifique.
        /// </summary>
        internal IEnumerable<T> GetProvidersOfType<T>() where T : class
        {
            foreach (var provider in _registeredProviders)
            {
                if (provider is T typedProvider)
                {
                    yield return typedProvider;
                }
            }
        }
    }
}
