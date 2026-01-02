using System.Threading.Tasks;
using RadialCore.Contracts.Models;

namespace RadialCore.Contracts.Core
{
    /// <summary>
    /// Interface V1 pour les handlers d'actions.
    /// Un ActionHandler exécute une action identifiée par actionId.
    /// Support async pour actions longues (API calls, loading, etc.).
    /// Version: 1.0.0
    /// </summary>
    public interface IActionHandler
    {
        /// <summary>
        /// ID unique du handler (ex: "BasicActions.Handler").
        /// </summary>
        string HandlerId { get; }

        /// <summary>
        /// Vérifie si ce handler peut gérer l'action spécifiée.
        /// </summary>
        /// <param name="actionId">ID de l'action (ex: "inventory.open")</param>
        /// <returns>True si le handler peut exécuter cette action</returns>
        bool CanHandle(string actionId);

        /// <summary>
        /// Exécute l'action de manière asynchrone.
        /// Le Core attend le résultat avant de continuer.
        /// </summary>
        /// <param name="actionId">ID de l'action à exécuter</param>
        /// <param name="context">Contexte actuel</param>
        /// <returns>Résultat de l'action (succès/échec + message)</returns>
        Task<ActionResult> ExecuteAsync(string actionId, MenuContext context);
    }
}
