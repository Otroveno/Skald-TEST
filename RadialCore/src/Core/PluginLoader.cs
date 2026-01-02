using System;
using System.Collections.Generic;
using System.Linq;
using RadialCore.Contracts;
using RadialCore.Core.Diagnostics;
using RadialCore.Core.Services;
using VersionType = RadialCore.Core.Versioning.Version;

namespace RadialCore.Core
{
    /// <summary>
    /// Chargeur et gestionnaire de plugins.
    /// Responsable de: découverte, validation, chargement, lifecycle, circuit-breaking.
    /// </summary>
    public class PluginLoader
    {
        private readonly ModPresenceService _modPresenceService;
        private readonly CapabilityResolver _capabilityResolver;
        private readonly List<LoadedPlugin> _loadedPlugins = new List<LoadedPlugin>();
        private readonly Dictionary<string, CircuitBreaker> _circuitBreakers = new Dictionary<string, CircuitBreaker>();
        
        private readonly VersionType _coreVersion = new VersionType(1, 0, 0);

        private class LoadedPlugin
        {
            public IRadialPlugin Plugin { get; set; } = null!;
            public Versioning.PluginManifest Manifest { get; set; } = null!;
            public bool Initialized { get; set; }
            public PluginInitializationContextImpl Context { get; set; } = null!;
        }

        public PluginLoader(ModPresenceService modPresenceService, CapabilityResolver capabilityResolver)
        {
            _modPresenceService = modPresenceService ?? throw new ArgumentNullException(nameof(modPresenceService));
            _capabilityResolver = capabilityResolver ?? throw new ArgumentNullException(nameof(capabilityResolver));
        }

        public void Initialize()
        {
            Logger.Info("PluginLoader", "PluginLoader initialized");
            Logger.Info("PluginLoader", $"Core version: v{_coreVersion}");
        }

        /// <summary>
        /// Charge un plugin manuellement.
        /// Valide manifest, dépendances, version, puis initialise.
        /// </summary>
        public bool LoadPlugin(IRadialPlugin plugin)
        {
            if (plugin == null)
            {
                Logger.Error("PluginLoader", "Attempted to load null plugin");
                return false;
            }

            try
            {
                var manifest = plugin.Manifest;
                string pluginId = manifest.PluginId;

                Logger.Info("PluginLoader", $"Loading plugin: {manifest}", pluginId: pluginId, phase: "Load");

                // Check si déjà chargé
                if (_loadedPlugins.Any(p => p.Manifest.PluginId == pluginId))
                {
                    Logger.Warning("PluginLoader", $"Plugin {pluginId} already loaded", pluginId: pluginId);
                    return false;
                }

                // Validation version Core
                if (!_coreVersion.IsCompatibleWith(manifest.RequiredCoreVersion))
                {
                    Logger.Error("PluginLoader", 
                        $"Plugin {pluginId} requires Core v{manifest.RequiredCoreVersion}, but current Core is v{_coreVersion}",
                        pluginId: pluginId);
                    return false;
                }

                // Validation dépendances mod
                foreach (var dep in manifest.ModDependencies)
                {
                    bool modLoaded = _modPresenceService.IsModLoaded(dep.Key);
                    bool required = dep.Value;

                    if (required && !modLoaded)
                    {
                        Logger.Error("PluginLoader",
                            $"Plugin {pluginId} requires mod {dep.Key} which is not loaded",
                            pluginId: pluginId);
                        return false;
                    }

                    if (!modLoaded)
                    {
                        Logger.Warning("PluginLoader",
                            $"Optional mod dependency {dep.Key} not loaded for plugin {pluginId}",
                            pluginId: pluginId);
                    }
                }

                // Créer circuit breaker pour ce plugin
                var circuitBreaker = new CircuitBreaker(pluginId);
                _circuitBreakers[pluginId] = circuitBreaker;

                // Créer contexte d'initialisation
                var context = new PluginInitializationContextImpl(
                    pluginId,
                    _modPresenceService,
                    _capabilityResolver);

                // Initialiser le plugin avec circuit breaker
                bool initialized = circuitBreaker.Execute(() =>
                {
                    plugin.Initialize(context);
                    return true;
                }, "Initialize");

                if (!initialized)
                {
                    Logger.Error("PluginLoader", $"Plugin {pluginId} initialization failed (circuit breaker tripped)", pluginId: pluginId);
                    return false;
                }

                // Enregistrer le plugin
                _loadedPlugins.Add(new LoadedPlugin
                {
                    Plugin = plugin,
                    Manifest = manifest,
                    Initialized = true,
                    Context = context
                });

                Logger.Info("PluginLoader", $"Plugin {pluginId} loaded successfully", pluginId: pluginId, phase: "Load");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("PluginLoader", $"Exception while loading plugin: {ex.Message}", ex, pluginId: plugin.Manifest?.PluginId);
                return false;
            }
        }

        /// <summary>
        /// Découvre et charge les plugins externes.
        /// TODO: Reflection-based discovery dans un dossier Plugins.
        /// </summary>
        public void DiscoverAndLoadPlugins()
        {
            Logger.Info("PluginLoader", "Discovering external plugins...", phase: "Discovery");
            
            // TODO: Implement reflection-based plugin discovery
            // Scan dossier ModuleData/Plugins/*.dll
            // Chercher types implémentant IRadialPlugin
            // Instancier et LoadPlugin()

            Logger.Info("PluginLoader", "External plugin discovery not yet implemented", phase: "Discovery");
        }

        /// <summary>
        /// Update tous les plugins (OnTick).
        /// </summary>
        public void OnTick(float deltaTime)
        {
            foreach (var loadedPlugin in _loadedPlugins)
            {
                if (!loadedPlugin.Initialized) continue;

                string pluginId = loadedPlugin.Manifest.PluginId;
                var circuitBreaker = _circuitBreakers[pluginId];

                circuitBreaker.Execute(() =>
                {
                    loadedPlugin.Plugin.OnTick(deltaTime);
                    return true;
                }, "OnTick");
            }
        }

        /// <summary>
        /// Shutdown tous les plugins.
        /// </summary>
        public void Shutdown()
        {
            Logger.Info("PluginLoader", "Shutting down all plugins...", phase: "Shutdown");

            foreach (var loadedPlugin in _loadedPlugins)
            {
                try
                {
                    string pluginId = loadedPlugin.Manifest.PluginId;
                    Logger.Info("PluginLoader", $"Shutting down plugin {pluginId}", pluginId: pluginId, phase: "Shutdown");
                    loadedPlugin.Plugin.Shutdown();
                }
                catch (Exception ex)
                {
                    Logger.Error("PluginLoader", $"Exception during plugin shutdown: {ex.Message}", ex);
                }
            }

            _loadedPlugins.Clear();
            _circuitBreakers.Clear();
        }

        public int GetLoadedPluginCount() => _loadedPlugins.Count(p => p.Initialized);

        public IEnumerable<Versioning.PluginManifest> GetLoadedPluginManifests() => _loadedPlugins.Where(p => p.Initialized).Select(p => p.Manifest);

        /// <summary>
        /// Retourne tous les providers d'un type spécifique (MenuProvider, ActionHandler, etc.).
        /// </summary>
        public IEnumerable<T> GetProviders<T>() where T : class
        {
            var providers = new List<T>();
            
            foreach (var loadedPlugin in _loadedPlugins.Where(p => p.Initialized))
            {
                var pluginProviders = loadedPlugin.Context.GetProvidersOfType<T>();
                providers.AddRange(pluginProviders);
            }

            return providers;
        }
    }
}
