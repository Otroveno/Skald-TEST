namespace RadialCore.Contracts.Core
{
    /// <summary>
    /// Interface V1 pour les providers de localisation.
    /// Un LocalizationProvider traduit des clés de texte.
    /// Version: 1.0.0
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// ID unique du provider (ex: "BasicActions.Localization").
        /// </summary>
        string ProviderId { get; }

        /// <summary>
        /// Langue courante (ex: "en", "fr", "de").
        /// </summary>
        string CurrentLanguage { get; }

        /// <summary>
        /// Vérifie si une clé de traduction existe.
        /// </summary>
        /// <param name="key">Clé de traduction (ex: "radial.action.inventory")</param>
        /// <returns>True si la clé existe</returns>
        bool HasTranslation(string key);

        /// <summary>
        /// Traduit une clé.
        /// Retourne la clé elle-même si traduction non trouvée.
        /// </summary>
        /// <param name="key">Clé de traduction</param>
        /// <returns>Texte traduit</returns>
        string Translate(string key);

        /// <summary>
        /// Traduit une clé avec paramètres.
        /// Exemple: Translate("radial.greet", "John") -> "Hello, John!"
        /// </summary>
        string Translate(string key, params object[] args);
    }
}
