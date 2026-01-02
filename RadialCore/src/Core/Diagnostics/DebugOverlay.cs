using System.Collections.Generic;
using System.Linq;

namespace RadialCore.Core.Diagnostics
{
    /// <summary>
    /// Overlay de debug pour afficher l'état du système RadialCore.
    /// TODO: Implémenter UI Gauntlet pour affichage in-game.
    /// Pour l'instant, logs seulement.
    /// </summary>
    public class DebugOverlay
    {
        private bool _visible = false;
        private readonly PluginLoader _pluginLoader;

        public DebugOverlay(PluginLoader pluginLoader)
        {
            _pluginLoader = pluginLoader;
        }

        /// <summary>
        /// Toggle visibilité de l'overlay.
        /// </summary>
        public void Toggle()
        {
            _visible = !_visible;
            
            if (_visible)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// Affiche l'overlay (logs état actuel).
        /// </summary>
        private void Show()
        {
            Logger.Info("DebugOverlay", "=== RadialCore Debug Overlay ===");
            Logger.Info("DebugOverlay", $"Core Version: 1.0.0");
            
            // Liste des plugins
            var manifests = _pluginLoader.GetLoadedPluginManifests().ToList();
            Logger.Info("DebugOverlay", $"Loaded Plugins: {manifests.Count}");
            
            foreach (var manifest in manifests)
            {
                Logger.Info("DebugOverlay", $"  - {manifest.DisplayName} v{manifest.PluginVersion} (ID: {manifest.PluginId})");
            }

            // TODO: Afficher circuit breaker status
            // TODO: Afficher context snapshot
            // TODO: Afficher performance metrics

            Logger.Info("DebugOverlay", "=== End Debug Overlay ===");
        }

        /// <summary>
        /// Masque l'overlay.
        /// </summary>
        private void Hide()
        {
            Logger.Info("DebugOverlay", "Debug overlay hidden");
        }

        /// <summary>
        /// Dump complet de l'état pour debugging.
        /// </summary>
        public void DumpFullState()
        {
            Logger.Info("DebugOverlay", "=== FULL STATE DUMP ===");
            
            // Plugins
            var manifests = _pluginLoader.GetLoadedPluginManifests().ToList();
            Logger.Info("DebugOverlay", $"Total Plugins: {manifests.Count}");
            
            foreach (var manifest in manifests)
            {
                Logger.Info("DebugOverlay", $"Plugin: {manifest.PluginId}");
                Logger.Info("DebugOverlay", $"  Display Name: {manifest.DisplayName}");
                Logger.Info("DebugOverlay", $"  Version: {manifest.PluginVersion}");
                Logger.Info("DebugOverlay", $"  Required Core: {manifest.RequiredCoreVersion}");
                Logger.Info("DebugOverlay", $"  Author: {manifest.Author}");
                Logger.Info("DebugOverlay", $"  Description: {manifest.Description}");
                
                if (manifest.ModDependencies.Any())
                {
                    Logger.Info("DebugOverlay", "  Mod Dependencies:");
                    foreach (var dep in manifest.ModDependencies)
                    {
                        string required = dep.Value ? "required" : "optional";
                        Logger.Info("DebugOverlay", $"    - {dep.Key} ({required})");
                    }
                }
            }

            // Providers
            var menuProviders = _pluginLoader.GetProviders<Contracts.Core.IMenuProvider>().ToList();
            var actionHandlers = _pluginLoader.GetProviders<Contracts.Core.IActionHandler>().ToList();
            var panelProviders = _pluginLoader.GetProviders<Contracts.Core.IPanelProvider>().ToList();
            var contextProviders = _pluginLoader.GetProviders<Contracts.Core.IContextProvider>().ToList();
            var conditionEvaluators = _pluginLoader.GetProviders<Contracts.Core.IConditionEvaluator>().ToList();

            Logger.Info("DebugOverlay", $"Registered Providers:");
            Logger.Info("DebugOverlay", $"  MenuProviders: {menuProviders.Count}");
            Logger.Info("DebugOverlay", $"  ActionHandlers: {actionHandlers.Count}");
            Logger.Info("DebugOverlay", $"  PanelProviders: {panelProviders.Count}");
            Logger.Info("DebugOverlay", $"  ContextProviders: {contextProviders.Count}");
            Logger.Info("DebugOverlay", $"  ConditionEvaluators: {conditionEvaluators.Count}");

            Logger.Info("DebugOverlay", "=== END FULL STATE DUMP ===");
        }
    }
}
