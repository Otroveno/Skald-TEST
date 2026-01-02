using System;
using System.Collections.Generic;

namespace RadialCore.Core.Versioning
{
    /// <summary>
    /// Manifest d'un plugin RadialCore.
    /// Contient metadata: ID, version, dépendances, capabilities requises.
    /// Utilisé pour validation de compatibilité et chargement.
    /// </summary>
    public class PluginManifest
    {
        /// <summary>
        /// ID unique du plugin (ex: "Radial.BasicActions", "Radial.DialogueMenu").
        /// </summary>
        public string PluginId { get; }

        /// <summary>
        /// Nom display du plugin.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Version du plugin.
        /// </summary>
        public Version PluginVersion { get; }

        /// <summary>
        /// Version minimale du Core requise.
        /// </summary>
        public Version RequiredCoreVersion { get; }

        /// <summary>
        /// Auteur du plugin.
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Description courte du plugin.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Dépendances optionnelles (autres mods).
        /// Key: ModId, Value: si requis ou optionnel.
        /// </summary>
        public Dictionary<string, bool> ModDependencies { get; }

        /// <summary>
        /// Capabilities requises (interfaces Core).
        /// Si capability manquante, le plugin peut fail-safe ou se désactiver.
        /// </summary>
        public List<string> RequiredCapabilities { get; }

        public PluginManifest(
            string pluginId,
            string displayName,
            Version pluginVersion,
            Version requiredCoreVersion,
            string author = "",
            string description = "")
        {
            if (string.IsNullOrWhiteSpace(pluginId))
                throw new ArgumentException("PluginId cannot be empty", nameof(pluginId));

            PluginId = pluginId;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? pluginId : displayName;
            PluginVersion = pluginVersion;
            RequiredCoreVersion = requiredCoreVersion;
            Author = author;
            Description = description;
            ModDependencies = new Dictionary<string, bool>();
            RequiredCapabilities = new List<string>();
        }

        /// <summary>
        /// Ajoute une dépendance mod optionnelle.
        /// </summary>
        public void AddOptionalModDependency(string modId)
        {
            ModDependencies[modId] = false;
        }

        /// <summary>
        /// Ajoute une dépendance mod requise.
        /// </summary>
        public void AddRequiredModDependency(string modId)
        {
            ModDependencies[modId] = true;
        }

        /// <summary>
        /// Ajoute une capability requise.
        /// </summary>
        public void AddRequiredCapability(string capabilityName)
        {
            if (!RequiredCapabilities.Contains(capabilityName))
            {
                RequiredCapabilities.Add(capabilityName);
            }
        }

        public override string ToString()
        {
            return $"{DisplayName} v{PluginVersion} (ID: {PluginId})";
        }
    }
}
