using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using RadialCore.Core;
using RadialCore.Core.Diagnostics;

namespace RadialCore
{
    /// <summary>
    /// Point d'entrée principal du mod RadialCore.
    /// Initialise le RadialCoreManager et gère le cycle de vie du mod.
    /// </summary>
    public class SubModule : MBSubModuleBase
    {
        private RadialCoreManager? _coreManager;

        /// <summary>
        /// Appelé lors du chargement du sous-module.
        /// Initialise les systèmes de base (logging, versioning).
        /// </summary>
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                Logger.Initialize("RadialCore");
                Logger.Info("SubModule", "RadialCore v1.0.0 loading...");
                Logger.Info("SubModule", "Target: Mount & Blade II: Bannerlord v1.3.10");
                Logger.Info("SubModule", ".NET Framework 4.7.2");
            }
            catch (Exception ex)
            {
                // Fallback si logging fail
                Console.WriteLine($"[RadialCore] CRITICAL: Failed to initialize logger: {ex.Message}");
            }
        }

        /// <summary>
        /// Appelé au démarrage du jeu (Campaign ou Sandbox).
        /// Initialise le RadialCoreManager si contexte Sandbox/Campaign.
        /// </summary>
        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            try
            {
                // Ne charger que pour Campaign/Sandbox (pas custom battle, etc.)
                if (game.GameType is Campaign)
                {
                    Logger.Info("SubModule", "Sandbox/Campaign detected, initializing RadialCoreManager...");
                    
                    _coreManager = new RadialCoreManager();
                    _coreManager.Initialize(game, gameStarter);

                    Logger.Info("SubModule", "RadialCoreManager initialized successfully");
                }
                else
                {
                    Logger.Info("SubModule", $"Game type {game.GameType?.GetType().Name ?? "Unknown"} detected, RadialCore will not initialize");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SubModule", $"Failed to initialize RadialCoreManager: {ex.Message}", ex);
                Logger.Error("SubModule", "RadialCore will not be available for this session");
            }
        }

        /// <summary>
        /// Appelé à chaque frame/tick du jeu.
        /// Délègue au RadialCoreManager pour update plugins, UI, input.
        /// </summary>
        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);

            try
            {
                _coreManager?.OnApplicationTick(dt);
            }
            catch (Exception ex)
            {
                Logger.Error("SubModule", $"Exception in OnApplicationTick: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Appelé lors de la fin du jeu.
        /// Cleanup propre du RadialCoreManager et des plugins.
        /// </summary>
        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);

            try
            {
                if (_coreManager != null)
                {
                    Logger.Info("SubModule", "Game ending, cleaning up RadialCoreManager...");
                    _coreManager.Shutdown();
                    _coreManager = null;
                    Logger.Info("SubModule", "RadialCoreManager cleaned up successfully");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SubModule", $"Exception during cleanup: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Appelé lors de l'unload du module.
        /// Cleanup final et fermeture des logs.
        /// </summary>
        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

            try
            {
                Logger.Info("SubModule", "RadialCore unloading...");
                Logger.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RadialCore] Exception during unload: {ex.Message}");
            }
        }
    }
}
