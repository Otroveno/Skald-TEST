namespace RadialCore.Contracts.Core
{
    /// <summary>
    /// Interface V1 pour les providers d'icônes.
    /// Un IconProvider fournit des icônes pour les entrées de menu.
    /// Version: 1.0.0
    /// </summary>
    public interface IIconProvider
    {
        /// <summary>
        /// ID unique du provider (ex: "BasicActions.IconProvider").
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Vérifie si ce provider peut fournir l'icône spécifiée.
        /// </summary>
        /// <param name="iconId">ID de l'icône (ex: "icon.inventory", "icon.map")</param>
        /// <returns>True si le provider peut fournir cette icône</returns>
        bool HasIcon(string iconId);

        /// <summary>
        /// Retourne le sprite path pour l'icône.
        /// Format Bannerlord: "SPGeneral\\MapIcons\\..."
        /// </summary>
        /// <param name="iconId">ID de l'icône</param>
        /// <returns>Path du sprite (ou null si non trouvé)</returns>
        string? GetIconSpritePath(string iconId);
    }
}
