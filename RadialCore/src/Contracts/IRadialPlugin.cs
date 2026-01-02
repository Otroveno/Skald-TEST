using System;
using RadialCore.Core.Versioning;
using RadialCore.Contracts.Core;

namespace RadialCore.Contracts
{
    /// <summary>
    /// Interface de base pour tous les plugins RadialCore.
    /// Contract-first: chaque plugin doit fournir manifest + méthodes lifecycle.
    /// Les plugins ne connaissent pas le Core, seulement les contracts.
    /// </summary>
    public interface IRadialPlugin
    {
        /// <summary>
        /// Manifest du plugin (ID, version, dépendances, capabilities requises).
        /// </summary>
        PluginManifest Manifest { get; }

        /// <summary>
        /// Initialise le plugin.
        /// Appelé une fois au chargement.
        /// Le plugin peut enregistrer ses providers (Menu, Action, Panel, etc.) ici.
        /// </summary>
        /// <param name="context">Contexte fourni par le Core (CapabilityResolver, etc.)</param>
        void Initialize(IPluginInitializationContext context);

        /// <summary>
        /// Appelé à chaque tick/frame si le plugin en a besoin.
        /// La plupart des plugins n'implémentent pas ceci (stateless).
        /// </summary>
        void OnTick(float deltaTime);

        /// <summary>
        /// Cleanup du plugin.
        /// Appelé avant shutdown ou si plugin est déchargé.
        /// </summary>
        void Shutdown();
    }

    /// <summary>
    /// Contexte d'initialisation fourni au plugin.
    /// Permet au plugin d'accéder aux services Core sans dépendance directe.
    /// </summary>
    public interface IPluginInitializationContext
    {
        /// <summary>
        /// Enregistre un provider de menus.
        /// </summary>
        void RegisterMenuProvider(IMenuProvider provider);

        /// <summary>
        /// Enregistre un handler d'actions.
        /// </summary>
        void RegisterActionHandler(IActionHandler handler);

        /// <summary>
        /// Enregistre un provider de panels.
        /// </summary>
        void RegisterPanelProvider(IPanelProvider provider);

        /// <summary>
        /// Enregistre un provider de contexte.
        /// </summary>
        void RegisterContextProvider(IContextProvider provider);

        /// <summary>
        /// Enregistre un évaluateur de conditions.
        /// </summary>
        void RegisterConditionEvaluator(IConditionEvaluator evaluator);

        /// <summary>
        /// Résout une capability optionnelle.
        /// Retourne null si capability non disponible.
        /// </summary>
        T? ResolveCapability<T>() where T : class;

        /// <summary>
        /// Vérifie si un mod est chargé.
        /// </summary>
        bool IsModLoaded(string modId);

        /// <summary>
        /// Log une info depuis le plugin.
        /// </summary>
        void LogInfo(string message);

        /// <summary>
        /// Log un warning depuis le plugin.
        /// </summary>
        void LogWarning(string message);

        /// <summary>
        /// Log une erreur depuis le plugin.
        /// </summary>
        void LogError(string message, Exception? exception = null);
    }
}
