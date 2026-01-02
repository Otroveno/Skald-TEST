using System;
using System.Collections.Generic;
using System.Linq;
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;
using RadialCore.Core.Diagnostics;

namespace RadialCore.UI
{
    public class MenuManager
    {
        private readonly Core.PluginLoader _pluginLoader;
        private readonly Core.ContextHub _contextHub;
        private readonly Core.Actions.ActionPipeline _actionPipeline;
        private readonly Core.UI.PanelHost _panelHost;

        private bool _isMenuOpen = false;

        public bool IsMenuOpen => _isMenuOpen;

        public MenuManager(
            Core.PluginLoader pluginLoader,
            Core.ContextHub contextHub,
            Core.Actions.ActionPipeline actionPipeline,
            Core.UI.PanelHost panelHost)
        {
            _pluginLoader = pluginLoader ?? throw new ArgumentNullException(nameof(pluginLoader));
            _contextHub = contextHub ?? throw new ArgumentNullException(nameof(contextHub));
            _actionPipeline = actionPipeline ?? throw new ArgumentNullException(nameof(actionPipeline));
            _panelHost = panelHost ?? throw new ArgumentNullException(nameof(panelHost));

            Logger.Info("MenuManager", "MenuManager initialized");
        }

        public void OpenMenu()
        {
            if (_isMenuOpen)
            {
                Logger.Warning("MenuManager", "Menu already open, ignoring");
                return;
            }

            try
            {
                Logger.Info("MenuManager", "Opening radial menu");

                _contextHub.ForceRefresh();
                var context = _contextHub.GetCurrentContext();

                var entries = CollectMenuEntries(context);

                if (entries.Count == 0)
                {
                    Logger.Warning("MenuManager", "No menu entries available");
                    return;
                }

                _isMenuOpen = true;
                Logger.Info("MenuManager", $"Menu opened with {entries.Count} entries (stub)");
            }
            catch (Exception ex)
            {
                Logger.Error("MenuManager", "Failed to open menu", ex);
            }
        }

        public void CloseMenu()
        {
            if (!_isMenuOpen)
            {
                return;
            }

            try
            {
                Logger.Info("MenuManager", "Closing radial menu");
                _panelHost.HideAllPanels();
                _isMenuOpen = false;
                Logger.Info("MenuManager", "Menu closed");
            }
            catch (Exception ex)
            {
                Logger.Error("MenuManager", "Error closing menu", ex);
            }
        }

        private List<RadialMenuEntry> CollectMenuEntries(MenuContext context)
        {
            var entries = new List<RadialMenuEntry>();

            try
            {
                var providers = _pluginLoader.GetProviders<IMenuProvider>()
                    .OrderByDescending(p => p.Priority)
                    .ToList();

                Logger.Debug("MenuManager", $"Collecting entries from {providers.Count} menu providers");

                foreach (var provider in providers)
                {
                    try
                    {
                        var providerEntries = provider.GetMenuEntries(context);
                        if (providerEntries != null)
                        {
                            entries.AddRange(providerEntries.Where(e => e.IsVisible));
                            Logger.Debug("MenuManager", $"Provider {provider.ProviderId} provided {providerEntries.Count()} entries");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("MenuManager", $"Error in MenuProvider {provider.ProviderId}", ex);
                    }
                }

                entries = entries.OrderByDescending(e => e.Priority).ToList();
                Logger.Info("MenuManager", $"Collected {entries.Count} total menu entries");
            }
            catch (Exception ex)
            {
                Logger.Error("MenuManager", "Error collecting menu entries", ex);
            }

            return entries;
        }
    }
}
