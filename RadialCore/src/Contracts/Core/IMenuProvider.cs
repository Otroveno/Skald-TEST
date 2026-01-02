using System.Collections.Generic;
using RadialCore.Contracts.Models;

namespace RadialCore.Contracts.Core
{
    /// <summary>
    /// Interface V1 pour les providers de menu.
    /// Un MenuProvider fournit des entrées de menu dynamiques basées sur le contexte.
    /// Version: 1.0.0
    /// </summary>
    public interface IMenuProvider
    {
        /// <summary>
        /// ID unique du provider (ex: "BasicActions.MainMenu").
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Priorité d'affichage (plus élevé = affiché en premier).
        /// Range: 0-1000, défaut: 100.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Retourne les entrées de menu pour le contexte actuel.
        /// Appelé lors de l'ouverture du menu ou refresh.
        /// </summary>
        /// <param name="context">Contexte actuel (joueur, position, sélection, etc.)</param>
        /// <returns>Liste d'entrées de menu (peut être vide)</returns>
        IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context);
    }
}
