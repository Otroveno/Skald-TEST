using RadialCore.Contracts.Models;

namespace RadialCore.Contracts.Core
{
    /// <summary>
    /// Interface V1 pour les providers de contexte.
    /// Un ContextProvider alimente le ContextHub avec des données.
    /// Appelé périodiquement pour refresh le snapshot de contexte.
    /// Version: 1.0.0
    /// </summary>
    public interface IContextProvider
    {
        /// <summary>
        /// ID unique du provider (ex: "BasicActions.PlayerContext").
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Priorité d'exécution (plus élevé = appelé en premier).
        /// Range: 0-1000, défaut: 100.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Alimente le contexte avec des données.
        /// Le provider modifie l'objet context en place.
        /// </summary>
        /// <param name="context">Contexte à alimenter</param>
        void ProvideContext(MenuContext context);
    }
}
