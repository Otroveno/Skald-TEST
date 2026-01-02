using System.Collections.Generic;

namespace RadialCore.Contracts.Models
{
    /// <summary>
    /// Entrée de menu radial.
    /// Représente une action ou un sous-menu dans le menu radial.
    /// </summary>
    public class RadialMenuEntry
    {
        /// <summary>
        /// ID unique de l'entrée (ex: "inventory.open", "map.show").
        /// Utilisé pour identifier l'action à exécuter.
        /// </summary>
        public string EntryId { get; set; } = "";

        /// <summary>
        /// Texte affiché (ou clé de traduction si commence par "$").
        /// Exemple: "Open Inventory" ou "$radial.inventory.open"
        /// </summary>
        public string Label { get; set; } = "";

        /// <summary>
        /// Description (tooltip).
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// ID de l'icône (résolu par IIconProvider).
        /// </summary>
        public string IconId { get; set; } = "";

        /// <summary>
        /// Type d'entrée.
        /// </summary>
        public EntryType Type { get; set; } = EntryType.Action;

        /// <summary>
        /// Priorité d'affichage (plus élevé = affiché en premier dans le segment).
        /// Range: 0-1000, défaut: 100.
        /// </summary>
        public int Priority { get; set; } = 100;

        /// <summary>
        /// Visible dans le menu radial.
        /// Peut être contrôlé par des conditions.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Enabled (cliquable).
        /// Si false, affiché grisé.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Condition ID pour visible (optionnel).
        /// Si spécifié, évalué par IConditionEvaluator.
        /// </summary>
        public string? VisibilityConditionId { get; set; }

        /// <summary>
        /// Condition ID pour enabled (optionnel).
        /// </summary>
        public string? EnabledConditionId { get; set; }

        /// <summary>
        /// ID du panel preview (optionnel).
        /// Si spécifié, affiché dans le right panel au hover.
        /// </summary>
        public string? PreviewPanelId { get; set; }

        /// <summary>
        /// Sous-entrées (si Type == Submenu).
        /// </summary>
        public List<RadialMenuEntry> SubEntries { get; set; } = new List<RadialMenuEntry>();

        /// <summary>
        /// Données custom attachées à l'entrée.
        /// Peut être utilisé par les plugins pour stocker des métadonnées.
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Type d'entrée de menu.
    /// </summary>
    public enum EntryType
    {
        /// <summary>
        /// Action simple (exécute une action au clic).
        /// </summary>
        Action,

        /// <summary>
        /// Sous-menu (ouvre un nouveau menu radial).
        /// </summary>
        Submenu,

        /// <summary>
        /// Séparateur visuel (non cliquable).
        /// </summary>
        Separator
    }
}
