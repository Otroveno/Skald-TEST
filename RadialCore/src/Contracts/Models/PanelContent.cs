using System.Collections.Generic;

namespace RadialCore.Contracts.Models
{
    /// <summary>
    /// Contenu d'un panel UI.
    /// Retourné par IPanelProvider.GetPanelContent().
    /// </summary>
    public class PanelContent
    {
        /// <summary>
        /// Type de panel.
        /// </summary>
        public PanelType Type { get; set; }

        /// <summary>
        /// Titre du panel (optionnel).
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Contenu texte du panel.
        /// Peut être formaté (HTML-like si supporté par UI).
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// Boutons du panel (pour modal/confirm).
        /// </summary>
        public List<PanelButton> Buttons { get; set; } = new List<PanelButton>();

        /// <summary>
        /// Input config (pour text input panel).
        /// </summary>
        public TextInputConfig? InputConfig { get; set; }

        /// <summary>
        /// Données custom pour rendering avancé.
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Type de panel.
    /// </summary>
    public enum PanelType
    {
        /// <summary>
        /// Panel de preview (right panel, non modal).
        /// </summary>
        Preview,

        /// <summary>
        /// Modal de confirmation (oui/non).
        /// </summary>
        ConfirmModal,

        /// <summary>
        /// Modal d'information (ok).
        /// </summary>
        InfoModal,

        /// <summary>
        /// Panel d'input texte.
        /// </summary>
        TextInput,

        /// <summary>
        /// Panel custom (à gérer par le plugin).
        /// </summary>
        Custom
    }

    /// <summary>
    /// Bouton de panel.
    /// </summary>
    public class PanelButton
    {
        /// <summary>
        /// Label du bouton (ou clé de traduction).
        /// </summary>
        public string Label { get; set; } = "";

        /// <summary>
        /// Action ID à exécuter au clic.
        /// </summary>
        public string ActionId { get; set; } = "";

        /// <summary>
        /// Style du bouton.
        /// </summary>
        public ButtonStyle Style { get; set; } = ButtonStyle.Default;

        /// <summary>
        /// Enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }

    public enum ButtonStyle
    {
        Default,
        Primary,
        Danger,
        Success
    }

    /// <summary>
    /// Configuration d'input texte.
    /// </summary>
    public class TextInputConfig
    {
        /// <summary>
        /// Placeholder text.
        /// </summary>
        public string Placeholder { get; set; } = "";

        /// <summary>
        /// Valeur initiale.
        /// </summary>
        public string InitialValue { get; set; } = "";

        /// <summary>
        /// Longueur max.
        /// </summary>
        public int MaxLength { get; set; } = 100;

        /// <summary>
        /// Validation regex (optionnel).
        /// </summary>
        public string? ValidationRegex { get; set; }

        /// <summary>
        /// Action ID à exécuter au submit (Enter).
        /// </summary>
        public string SubmitActionId { get; set; } = "";
    }
}
