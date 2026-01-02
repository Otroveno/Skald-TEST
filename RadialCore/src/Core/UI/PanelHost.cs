using System;
using System.Collections.Generic;
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;
using RadialCore.Core.Diagnostics;

namespace RadialCore.Core.UI
{
    /// <summary>
    /// Host pour les panels UI avec slots prédéfinis.
    /// Gère l'affichage des panels (Right Preview, Bottom Info, Modal, Text Input).
    /// Thread-safe, fail-safe.
    /// </summary>
    public class PanelHost
    {
        private readonly PluginLoader _pluginLoader;
        private readonly object _lock = new object();

        // Slots de panels
        private PanelContent? _rightPanelContent;
        private PanelContent? _bottomPanelContent;
        private PanelContent? _modalPanelContent;
        private PanelContent? _textInputPanelContent;

        // Events pour UI updates
        public event Action<PanelSlot, PanelContent?>? OnPanelChanged;

        public PanelHost(PluginLoader pluginLoader)
        {
            _pluginLoader = pluginLoader ?? throw new ArgumentNullException(nameof(pluginLoader));
            Logger.Info("PanelHost", "PanelHost initialized");
        }

        /// <summary>
        /// Affiche un panel dans un slot spécifique.
        /// </summary>
        public void ShowPanel(PanelSlot slot, string panelId, MenuContext context)
        {
            try
            {
                Logger.Debug("PanelHost", $"Showing panel: {panelId} in slot: {slot}");

                var content = GetPanelContent(panelId, context);
                
                if (content == null)
                {
                    Logger.Warning("PanelHost", $"No content found for panel: {panelId}");
                    return;
                }

                SetPanelContent(slot, content);
            }
            catch (Exception ex)
            {
                Logger.Error("PanelHost", $"Error showing panel {panelId}", ex);
            }
        }

        /// <summary>
        /// Masque un panel d'un slot.
        /// </summary>
        public void HidePanel(PanelSlot slot)
        {
            try
            {
                Logger.Debug("PanelHost", $"Hiding panel in slot: {slot}");
                SetPanelContent(slot, null);
            }
            catch (Exception ex)
            {
                Logger.Error("PanelHost", $"Error hiding panel in slot {slot}", ex);
            }
        }

        /// <summary>
        /// Masque tous les panels.
        /// </summary>
        public void HideAllPanels()
        {
            lock (_lock)
            {
                Logger.Debug("PanelHost", "Hiding all panels");
                
                SetPanelContent(PanelSlot.Right, null);
                SetPanelContent(PanelSlot.Bottom, null);
                SetPanelContent(PanelSlot.Modal, null);
                SetPanelContent(PanelSlot.TextInput, null);
            }
        }

        /// <summary>
        /// Récupère le contenu d'un panel depuis les providers.
        /// </summary>
        private PanelContent? GetPanelContent(string panelId, MenuContext context)
        {
            var providers = _pluginLoader.GetProviders<IPanelProvider>();

            foreach (var provider in providers)
            {
                try
                {
                    if (provider.CanProvide(panelId))
                    {
                        var content = provider.GetPanelContent(panelId, context);
                        if (content != null)
                        {
                            Logger.Debug("PanelHost", $"Panel content provided by: {provider.ProviderId}");
                            return content;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("PanelHost", $"Error in PanelProvider {provider.ProviderId}", ex);
                }
            }

            return null;
        }

        /// <summary>
        /// Set le contenu d'un slot et notifie l'UI.
        /// </summary>
        private void SetPanelContent(PanelSlot slot, PanelContent? content)
        {
            lock (_lock)
            {
                switch (slot)
                {
                    case PanelSlot.Right:
                        _rightPanelContent = content;
                        break;
                    case PanelSlot.Bottom:
                        _bottomPanelContent = content;
                        break;
                    case PanelSlot.Modal:
                        _modalPanelContent = content;
                        break;
                    case PanelSlot.TextInput:
                        _textInputPanelContent = content;
                        break;
                }

                // Notify UI
                OnPanelChanged?.Invoke(slot, content);
            }
        }

        /// <summary>
        /// Récupère le contenu actuel d'un slot (thread-safe).
        /// </summary>
        public PanelContent? GetCurrentContent(PanelSlot slot)
        {
            lock (_lock)
            {
                return slot switch
                {
                    PanelSlot.Right => _rightPanelContent,
                    PanelSlot.Bottom => _bottomPanelContent,
                    PanelSlot.Modal => _modalPanelContent,
                    PanelSlot.TextInput => _textInputPanelContent,
                    _ => null
                };
            }
        }

        /// <summary>
        /// Vérifie si un slot a du contenu.
        /// </summary>
        public bool HasContent(PanelSlot slot)
        {
            return GetCurrentContent(slot) != null;
        }
    }

    /// <summary>
    /// Slots de panels UI.
    /// </summary>
    public enum PanelSlot
    {
        /// <summary>
        /// Panel de preview à droite (hover entry).
        /// </summary>
        Right,

        /// <summary>
        /// Panel d'info en bas (status, hints).
        /// </summary>
        Bottom,

        /// <summary>
        /// Modal (confirmation, info).
        /// </summary>
        Modal,

        /// <summary>
        /// Text input panel.
        /// </summary>
        TextInput
    }
}
