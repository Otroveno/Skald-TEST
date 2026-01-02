using System;
using RadialCore.Contracts;
using RadialCore.Core.Versioning;
using RadialCore.Extensions.BasicActions.Providers;
using RadialCore.Extensions.BasicActions.Conditions;

namespace RadialCore.Extensions.BasicActions
{
    /// <summary>
    /// Plugin built-in RadialCore : BasicActions.
    /// Fournit des actions basiques (Inventory, Map, Quests, Talk).
    /// Démontre l'architecture plugin complète avec tous les providers.
    /// </summary>
    public class BasicActionsPlugin : IRadialPlugin
    {
        private PluginManifest? _manifest;
        private IPluginInitializationContext? _context;

        public PluginManifest Manifest
        {
            get
            {
                if (_manifest == null)
                {
                    _manifest = new PluginManifest(
                        pluginId: "Radial.BasicActions",
                        displayName: "Basic Actions",
                        pluginVersion: new Core.Versioning.Version(1, 0, 0),
                        requiredCoreVersion: new Core.Versioning.Version(1, 0, 0),
                        author: "RadialCore Team",
                        description: "Built-in plugin providing basic radial menu actions: Inventory, Map, Quests, Talk"
                    );
                }
                return _manifest;
            }
        }

        public void Initialize(IPluginInitializationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            context.LogInfo("Initializing BasicActions plugin...");

            try
            {
                // Register Menu Provider
                var menuProvider = new BasicMenuProvider();
                context.RegisterMenuProvider(menuProvider);
                context.LogInfo("Registered BasicMenuProvider");

                // Register Action Handler
                var actionHandler = new BasicActionHandler();
                context.RegisterActionHandler(actionHandler);
                context.LogInfo("Registered BasicActionHandler");

                // Register Panel Provider
                var panelProvider = new BasicPanelProvider();
                context.RegisterPanelProvider(panelProvider);
                context.LogInfo("Registered BasicPanelProvider");

                // Register Context Provider
                var contextProvider = new BasicContextProvider();
                context.RegisterContextProvider(contextProvider);
                context.LogInfo("Registered BasicContextProvider");

                // Register Condition Evaluator
                var conditionEvaluator = new BasicConditionEvaluator();
                context.RegisterConditionEvaluator(conditionEvaluator);
                context.LogInfo("Registered BasicConditionEvaluator");

                context.LogInfo("BasicActions plugin initialized successfully");
                context.LogInfo("Available actions: inventory, map, quests, talk (conditional)");
            }
            catch (Exception ex)
            {
                context.LogError("Failed to initialize BasicActions plugin", ex);
                throw;
            }
        }

        public void OnTick(float deltaTime)
        {
            // BasicActions est stateless, pas de tick nécessaire
        }

        public void Shutdown()
        {
            _context?.LogInfo("Shutting down BasicActions plugin");
        }
    }
}
