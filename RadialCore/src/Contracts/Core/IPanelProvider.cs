using RadialCore.Contracts.Models;

namespace RadialCore.Contracts.Core
{
    /// <summary>
    /// Interface V1 pour les providers de panels UI.
    /// Un PanelProvider fournit du contenu UI pour les panels (right preview, modal, input, etc.).
    /// Version: 1.0.0
    /// </summary>
    public interface IPanelProvider
    {
        /// <summary>
        /// ID unique du provider (ex: "BasicActions.PreviewPanel").
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Vérifie si ce provider peut fournir du contenu pour le panel spécifié.
        /// </summary>
        /// <param name="panelId">ID du panel (ex: "rightPreview", "modal.confirm")</param>
        /// <returns>True si le provider peut fournir du contenu</returns>
        bool CanProvide(string panelId);

        /// <summary>
        /// Retourne le contenu du panel.
        /// Appelé quand le panel doit être affiché.
        /// </summary>
        /// <param name="panelId">ID du panel</param>
        /// <param name="context">Contexte actuel</param>
        /// <returns>Contenu du panel (ou null si pas de contenu)</returns>
        PanelContent? GetPanelContent(string panelId, MenuContext context);
    }
}
