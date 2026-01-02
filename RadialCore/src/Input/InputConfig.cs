namespace RadialCore.Input
{
    /// <summary>
    /// Stub InputKey enum (remplace TaleWorlds.InputSystem.InputKey).
    /// TODO: Mapper vers vrai InputKey API quand disponible.
    /// </summary>
    public enum InputKey
    {
        Invalid = 0,
        V = 86,
        LeftControl = 341,
        LeftShift = 340,
        LeftAlt = 342
    }

    /// <summary>
    /// Configuration de l'input pour le menu radial.
    /// Utilisé par InputManager et MCM (futur).
    /// </summary>
    public class InputConfig
    {
        /// <summary>
        /// Touche principale (défaut: V).
        /// </summary>
        public InputKey PrimaryKey { get; set; } = InputKey.V;

        /// <summary>
        /// Touche modifier optionnelle (défaut: None).
        /// Exemples: LeftControl, LeftShift, LeftAlt
        /// </summary>
        public InputKey ModifierKey { get; set; } = InputKey.Invalid;

        /// <summary>
        /// Mode: true = Hold (maintenir touche), false = Toggle (appuyer pour ouvrir/fermer).
        /// Défaut: Toggle.
        /// </summary>
        public bool IsHoldMode { get; set; } = false;

        /// <summary>
        /// Activer/désactiver le menu radial.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Retourne une description textuelle de la hotkey.
        /// </summary>
        public string GetHotkeyDescription()
        {
            string result = "";
            
            if (ModifierKey != InputKey.Invalid)
            {
                result += ModifierKey.ToString() + " + ";
            }
            
            result += PrimaryKey.ToString();
            result += $" ({(IsHoldMode ? "Hold" : "Toggle")})";
            
            return result;
        }

        /// <summary>
        /// Config par défaut.
        /// </summary>
        public static InputConfig Default => new InputConfig
        {
            PrimaryKey = InputKey.V,
            ModifierKey = InputKey.Invalid,
            IsHoldMode = false,
            Enabled = true
        };

        /// <summary>
        /// Clone la config.
        /// </summary>
        public InputConfig Clone()
        {
            return new InputConfig
            {
                PrimaryKey = this.PrimaryKey,
                ModifierKey = this.ModifierKey,
                IsHoldMode = this.IsHoldMode,
                Enabled = this.Enabled
            };
        }
    }
}
