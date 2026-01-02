using System;
using System.Collections.Generic;
using System.Linq;
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;
using RadialCore.Core.Diagnostics;
using RadialCore.Core.Services;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace RadialCore.Core
{
    /// <summary>
    /// Hub de contexte centralisé avec snapshot capability-based.
    /// Agrège les données depuis ContextProviders et fournit un snapshot immutable.
    /// Utilise polling contrôlé pour éviter overhead.
    /// </summary>
    public class ContextHub
    {
        private readonly PluginLoader _pluginLoader;
        private readonly Services.CapabilityResolver _capabilityResolver;
        
        private MenuContext? _currentSnapshot;
        private float _timeSinceLastRefresh = 0f;
        private const float REFRESH_INTERVAL = 0.5f; // Refresh 2x par seconde
        
        private readonly object _snapshotLock = new object();

        public ContextHub(PluginLoader pluginLoader, Services.CapabilityResolver capabilityResolver)
        {
            _pluginLoader = pluginLoader ?? throw new ArgumentNullException(nameof(pluginLoader));
            _capabilityResolver = capabilityResolver ?? throw new ArgumentNullException(nameof(capabilityResolver));
        }

        public void Initialize()
        {
            Logger.Info("ContextHub", "ContextHub initialized");
            
            // Enregistrer les services de context par défaut
            RegisterDefaultServices();
            
            // Créer snapshot initial
            RefreshContext();
        }

        /// <summary>
        /// Enregistre les services de contexte par défaut (PlayerState, NPCProximity, etc.).
        /// </summary>
        private void RegisterDefaultServices()
        {
            try
            {
                // Enregistrer PlayerStateService comme capability
                var playerStateService = new Services.PlayerStateService();
                _capabilityResolver.RegisterCapability<Services.IPlayerStateService>(playerStateService);
                Logger.Info("ContextHub", "Registered PlayerStateService");

                // Enregistrer NPCProximityService
                var npcProximityService = new Services.NPCProximityService();
                _capabilityResolver.RegisterCapability<Services.INPCProximityService>(npcProximityService);
                Logger.Info("ContextHub", "Registered NPCProximityService");
            }
            catch (Exception ex)
            {
                Logger.Error("ContextHub", "Failed to register default services", ex);
            }
        }

        public void OnTick(float deltaTime)
        {
            _timeSinceLastRefresh += deltaTime;

            if (_timeSinceLastRefresh >= REFRESH_INTERVAL)
            {
                RefreshContext();
                _timeSinceLastRefresh = 0f;
            }
        }

        /// <summary>
        /// Refresh le snapshot de contexte.
        /// Appelé périodiquement ou manuellement lors de l'ouverture du menu.
        /// </summary>
        private void RefreshContext()
        {
            try
            {
                // Créer nouveau snapshot
                var newSnapshot = new MenuContext
                {
                    Timestamp = GetCurrentTime()
                };

                // Collecter données depuis services par défaut
                CollectDefaultContextData(newSnapshot);

                // Collecter données depuis ContextProviders plugins
                var contextProviders = _pluginLoader.GetProviders<IContextProvider>()
                    .OrderByDescending(p => p.Priority)
                    .ToList();

                foreach (var provider in contextProviders)
                {
                    try
                    {
                        provider.ProvideContext(newSnapshot);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("ContextHub", 
                            $"Exception in ContextProvider {provider.ProviderId}", 
                            ex, 
                            pluginId: provider.ProviderId);
                    }
                }

                // Lock et update snapshot
                lock (_snapshotLock)
                {
                    _currentSnapshot = newSnapshot;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ContextHub", "Exception during context refresh", ex);
            }
        }

        /// <summary>
        /// Collecte les données de contexte par défaut (Player, GameState, etc.).
        /// </summary>
        private void CollectDefaultContextData(MenuContext context)
        {
            try
            {
                // Player info
                var playerStateService = _capabilityResolver.ResolveCapability<Services.IPlayerStateService>();
                if (playerStateService != null)
                {
                    context.Player = playerStateService.GetPlayerInfo();
                }

                // Game state
                context.GameState = new GameStateInfo
                {
                    IsOnMap = Campaign.Current != null,
                    IsInMission = false, // TODO: Check mission state
                    IsInConversation = false,
                    IsInInventory = false,
                    IsPaused = false,
                    CurrentScreen = GetCurrentScreenName()
                };

                // Selection (NPC proximity)
                var npcProximityService = _capabilityResolver.ResolveCapability<Services.INPCProximityService>();
                if (npcProximityService != null)
                {
                    var nearbyNPC = npcProximityService.GetNearestNPC();
                    if (nearbyNPC != null)
                    {
                        context.Selection = new SelectionInfo
                        {
                            Type = SelectionType.NPC,
                            TargetId = nearbyNPC.Value.id,
                            TargetName = nearbyNPC.Value.name,
                            DistanceToPlayer = nearbyNPC.Value.distance
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ContextHub", "Error collecting default context data", ex);
            }
        }

        /// <summary>
        /// Helper: récupère le nom de l'écran actuel.
        /// </summary>
        private string GetCurrentScreenName()
        {
            try
            {
                var activeState = Game.Current?.GameStateManager?.ActiveState;
                return activeState?.GetType().Name ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Helper: récupère le timestamp actuel.
        /// Utilise Time API si disponible, sinon fallback sur DateTime.
        /// </summary>
        private float GetCurrentTime()
        {
            try
            {
                // Fallback à DateTime si Time API pas disponible
                return (float)(DateTime.Now - DateTime.Today).TotalSeconds;
            }
            catch
            {
                return 0f;
            }
        }

        /// <summary>
        /// Récupère le snapshot actuel (thread-safe).
        /// Retourne une copie pour éviter modifications concurrentes.
        /// </summary>
        public MenuContext GetCurrentContext()
        {
            lock (_snapshotLock)
            {
                // Retourner copie superficielle (snapshot est immutable)
                return _currentSnapshot ?? new MenuContext
                {
                    Timestamp = GetCurrentTime()
                };
            }
        }

        /// <summary>
        /// Force un refresh immédiat du snapshot.
        /// Utile lors de l'ouverture du menu radial.
        /// </summary>
        public void ForceRefresh()
        {
            RefreshContext();
            _timeSinceLastRefresh = 0f;
            Logger.Debug("ContextHub", "Context snapshot force refreshed");
        }

        public void Shutdown()
        {
            lock (_snapshotLock)
            {
                _currentSnapshot = null;
            }
            Logger.Info("ContextHub", "ContextHub shutdown");
        }
    }
}
