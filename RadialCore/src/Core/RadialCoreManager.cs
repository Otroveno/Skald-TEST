using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using RadialCore.Core.Diagnostics;
using RadialCore.Core.Services;
using RadialCore.Core.Events;
using RadialCore.Core.Actions;
using RadialCore.Core.UI;
using RadialCore.Contracts;
using RadialCore.Extensions.BasicActions;
using RadialCore.Input;

namespace RadialCore.Core
{
    /// <summary>
    /// Gestionnaire central du système RadialCore.
    /// Responsable de l'initialisation, du cycle de vie, et de la coordination de tous les sous-systèmes.
    /// Architecture: PluginLoader -> ContextHub -> UI -> Input
    /// </summary>
    public class RadialCoreManager
    {
        private Game? _game;
        private IGameStarter? _gameStarter;
        
        private ModPresenceService? _modPresenceService;
        private CapabilityResolver? _capabilityResolver;
        private PluginLoader? _pluginLoader;
        private ContextHub? _contextHub;
        private DebugOverlay? _debugOverlay;
        private InputManager? _inputManager;
        
        // New systems
        private PanelHost? _panelHost;
        private NotificationService? _notificationService;
        private ActionPipeline? _actionPipeline;
        private RadialCore.UI.MenuManager? _menuManager;

        private bool _initialized = false;

        /// <summary>
        /// Initialise le RadialCoreManager.
        /// Phase 1: Services de base (ModPresence, Capabilities)
        /// Phase 2: PluginLoader + chargement des plugins
        /// Phase 3: ContextHub
        /// Phase 4: UI + Input (à implémenter)
        /// </summary>
        public void Initialize(Game game, IGameStarter gameStarter)
        {
            if (_initialized)
            {
                Logger.Warning("RadialCoreManager", "Already initialized, skipping");
                return;
            }

            _game = game;
            _gameStarter = gameStarter;

            try
            {
                Logger.Info("RadialCoreManager", "=== Initialization Phase 1: Core Services ===", phase: "Init");
                
                // Phase 1: Services de base
                _modPresenceService = new ModPresenceService();
                _modPresenceService.Initialize();
                Logger.Info("RadialCoreManager", "ModPresenceService initialized", phase: "Init");

                _capabilityResolver = new CapabilityResolver();
                Logger.Info("RadialCoreManager", "CapabilityResolver initialized", phase: "Init");

                Logger.Info("RadialCoreManager", "=== Initialization Phase 2: Plugin System ===", phase: "Init");
                
                // Phase 2: Plugin system
                _pluginLoader = new PluginLoader(_modPresenceService, _capabilityResolver);
                _pluginLoader.Initialize();
                
                // Charger le plugin built-in BasicActions
                LoadBuiltInPlugins();
                
                // Discover et load external plugins (TODO: reflection-based discovery)
                _pluginLoader.DiscoverAndLoadPlugins();
                
                Logger.Info("RadialCoreManager", $"Loaded {_pluginLoader.GetLoadedPluginCount()} plugins", phase: "Init");

                Logger.Info("RadialCoreManager", "=== Initialization Phase 3: Context System ===", phase: "Init");
                
                // Phase 3: ContextHub
                _contextHub = new ContextHub(_pluginLoader, _capabilityResolver);
                _contextHub.Initialize();
                Logger.Info("RadialCoreManager", "ContextHub initialized", phase: "Init");

                Logger.Info("RadialCoreManager", "=== Initialization Phase 4: Event Bus ===", phase: "Init");
                
                // EventBus est singleton, pas besoin de l'instancier
                Logger.Info("RadialCoreManager", "EventBus ready (singleton)", phase: "Init");

                Logger.Info("RadialCoreManager", "=== Initialization Phase 5: UI Systems ===", phase: "Init");
                
                // Phase 4: PanelHost
                _panelHost = new PanelHost(_pluginLoader);
                Logger.Info("RadialCoreManager", "PanelHost initialized", phase: "Init");
                
                // Phase 5: NotificationService
                _notificationService = new NotificationService();
                Logger.Info("RadialCoreManager", "NotificationService initialized", phase: "Init");
                
                // Phase 6: ActionPipeline
                _actionPipeline = new ActionPipeline(_pluginLoader, _panelHost, _notificationService);
                Logger.Info("RadialCoreManager", "ActionPipeline initialized", phase: "Init");
                
                // Phase 7: MenuManager
                _menuManager = new RadialCore.UI.MenuManager(_pluginLoader, _contextHub, _actionPipeline, _panelHost);
                Logger.Info("RadialCoreManager", "MenuManager initialized", phase: "Init");

                Logger.Info("RadialCoreManager", "=== Initialization Phase 8: Input System ===", phase: "Init");
                
                // Phase 8: InputManager
                _inputManager = new InputManager(_contextHub);
                // Hook menu open/close to InputManager events
                SubscribeToInputEvents();
                Logger.Info("RadialCoreManager", "InputManager initialized", phase: "Init");

                Logger.Info("RadialCoreManager", "=== Initialization Phase 9: Diagnostics ===", phase: "Init");
                
                // Phase 9: Debug Overlay
                _debugOverlay = new DebugOverlay(_pluginLoader);
                Logger.Info("RadialCoreManager", "DebugOverlay initialized", phase: "Init");

                _initialized = true;
                Logger.Info("RadialCoreManager", "RadialCore initialized successfully!", phase: "Init");
                
                // Dump état initial
                _debugOverlay.DumpFullState();
            }
            catch (Exception ex)
            {
                Logger.Error("RadialCoreManager", "Critical error during initialization", ex, phase: "Init");
                throw;
            }
        }

        /// <summary>
        /// Charge les plugins built-in (BasicActions).
        /// </summary>
        private void LoadBuiltInPlugins()
        {
            try
            {
                Logger.Info("RadialCoreManager", "Loading built-in plugins...", phase: "PluginLoad");
                
                var basicActionsPlugin = new BasicActionsPlugin();
                _pluginLoader?.LoadPlugin(basicActionsPlugin);
                
                Logger.Info("RadialCoreManager", "Built-in plugins loaded", phase: "PluginLoad");
            }
            catch (Exception ex)
            {
                Logger.Error("RadialCoreManager", "Failed to load built-in plugins", ex, phase: "PluginLoad");
            }
        }

        /// <summary>
        /// S'abonne aux événements d'entrée pour l'ouverture/fermeture des menus.
        /// </summary>
        private void SubscribeToInputEvents()
        {
            try
            {
                EventBus.Instance.Subscribe<RadialMenuOpenedEvent>(e =>
                {
                    _menuManager?.OpenMenu();
                });

                EventBus.Instance.Subscribe<RadialMenuClosedEvent>(e =>
                {
                    _menuManager?.CloseMenu();
                });

                Logger.Debug("RadialCoreManager", "Subscribed to input events");
            }
            catch (Exception ex)
            {
                Logger.Error("RadialCoreManager", "Error subscribing to input events", ex);
            }
        }

        /// <summary>
        /// Update appelé à chaque frame.
        /// Délègue aux sous-systèmes: ContextHub refresh, Input polling, UI update.
        /// </summary>
        public void OnApplicationTick(float dt)
        {
            if (!_initialized) return;

            try
            {
                // Update ContextHub (refresh context snapshot si nécessaire)
                _contextHub?.OnTick(dt);
                
                // Update Plugins (tick optionnel)
                _pluginLoader?.OnTick(dt);
                
                // Poll Input (hotkey detection)
                _inputManager?.OnTick(dt);
                
                // Update NotificationService
                _notificationService?.OnTick(dt);
            }
            catch (Exception ex)
            {
                Logger.Error("RadialCoreManager", "Exception in OnApplicationTick", ex);
            }
        }

        /// <summary>
        /// Shutdown propre: cleanup plugins, UI, services.
        /// </summary>
        public void Shutdown()
        {
            if (!_initialized) return;

            try
            {
                Logger.Info("RadialCoreManager", "Shutting down RadialCore...", phase: "Shutdown");

                // Fermer le menu si ouvert
                _menuManager?.CloseMenu();
                
                // Shutdown dans l'ordre inverse de l'initialisation
                _menuManager = null;
                _actionPipeline = null;
                _notificationService = null;
                _panelHost = null;
                _inputManager = null;
                _debugOverlay = null;
                _contextHub?.Shutdown();
                _pluginLoader?.Shutdown();
                
                // Clear EventBus
                EventBus.Instance.Clear();
                
                _capabilityResolver = null;
                _modPresenceService = null;

                _initialized = false;
                Logger.Info("RadialCoreManager", "RadialCore shutdown complete", phase: "Shutdown");
            }
            catch (Exception ex)
            {
                Logger.Error("RadialCoreManager", "Exception during shutdown", ex, phase: "Shutdown");
            }
        }

        /// <summary>
        /// Accès au PluginLoader (pour debugging/extensions).
        /// </summary>
        public PluginLoader? GetPluginLoader() => _pluginLoader;

        /// <summary>
        /// Accès au ContextHub (pour debugging/extensions).
        /// </summary>
        public ContextHub? GetContextHub() => _contextHub;

        /// <summary>
        /// Accès au DebugOverlay (pour debugging/extensions).
        /// </summary>
        public DebugOverlay? GetDebugOverlay() => _debugOverlay;

        /// <summary>
        /// Accès au InputManager (pour debugging/extensions).
        /// </summary>
        public InputManager? GetInputManager() => _inputManager;

        /// <summary>
        /// Accès au MenuManager (pour debugging/extensions).
        /// </summary>
        public RadialCore.UI.MenuManager? GetMenuManager() => _menuManager;
    }
}
