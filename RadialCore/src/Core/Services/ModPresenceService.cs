using System;
using System.Collections.Generic;
using System.Linq;
using RadialCore.Core.Diagnostics;

namespace RadialCore.Core.Services
{
    /// <summary>
    /// Service de détection de présence de mods.
    /// Permet de vérifier si un mod (ButterLib, UIExtenderEx, MCM, AI Influence, etc.) est chargé.
    /// Évite les dépendances dures en utilisant reflection/type checking.
    /// </summary>
    public class ModPresenceService
    {
        private readonly Dictionary<string, bool> _modPresenceCache = new Dictionary<string, bool>();
        private bool _initialized = false;

        /// <summary>
        /// Initialise le service et scanne les modules chargés.
        /// </summary>
        public void Initialize()
        {
            if (_initialized) return;

            try
            {
                Logger.Info("ModPresenceService", "Scanning loaded modules...");

                // Scan basique via Type.GetType pour détecter assemblies optionnels
                ScanModule("ButterLib");
                ScanModule("UIExtenderEx");
                ScanModule("MBOptionScreen");
                ScanModule("AIInfluence");

                _initialized = true;

                Logger.Info("ModPresenceService", "Module scanning complete");
                var detectedMods = _modPresenceCache.Where(x => x.Value).Select(x => x.Key).ToList();
                if (detectedMods.Any())
                {
                    Logger.Info("ModPresenceService", $"Detected mods: {string.Join(", ", detectedMods)}");
                }
                else
                {
                    Logger.Info("ModPresenceService", "No optional mods detected");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ModPresenceService", "Failed to scan modules", ex);
            }
        }

        /// <summary>
        /// Vérifie si un mod spécifique est chargé.
        /// </summary>
        public bool IsModLoaded(string modId)
        {
            if (!_initialized)
            {
                Logger.Warning("ModPresenceService", "IsModLoaded called before initialization");
                return false;
            }

            if (_modPresenceCache.TryGetValue(modId, out bool isLoaded))
            {
                return isLoaded;
            }

            // Cache miss: scan à la demande
            bool detected = ScanModule(modId);
            return detected;
        }

        /// <summary>
        /// Scanne un module spécifique.
        /// Méthode simplifiée: check via Type.GetType pour assemblies optionnels.
        /// </summary>
        private bool ScanModule(string modId)
        {
            try
            {
                // Méthode 1: Check assembly via Type.GetType
                string? typeHint = modId switch
                {
                    "ButterLib" => "ButterLib.ButterLibSubModule, ButterLib",
                    "UIExtenderEx" => "UIExtenderEx.UIExtender, UIExtenderEx",
                    "MBOptionScreen" => "MBOptionScreen.Settings.BaseSettings, MBOptionScreen",
                    "AIInfluence" => "AIInfluence.AIInfluenceSubModule, AIInfluence",
                    _ => null
                };

                if (typeHint != null)
                {
                    var type = Type.GetType(typeHint, false);
                    if (type != null)
                    {
                        _modPresenceCache[modId] = true;
                        Logger.Debug("ModPresenceService", $"Mod detected: {modId}");
                        return true;
                    }
                }

                _modPresenceCache[modId] = false;
                return false;
            }
            catch (Exception ex)
            {
                Logger.Warning("ModPresenceService", $"Error scanning module {modId}: {ex.Message}");
                _modPresenceCache[modId] = false;
                return false;
            }
        }

        /// <summary>
        /// Reset du cache (utile pour hot-reload si supporté).
        /// </summary>
        public void ClearCache()
        {
            _modPresenceCache.Clear();
            _initialized = false;
        }
    }
}
