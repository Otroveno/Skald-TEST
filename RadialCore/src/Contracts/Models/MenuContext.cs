using System.Collections.Generic;

namespace RadialCore.Contracts.Models
{
    /// <summary>
    /// Contexte immutable du menu radial.
    /// Snapshot des données de jeu pertinentes pour les plugins.
    /// Refreshé périodiquement par le ContextHub.
    /// </summary>
    public class MenuContext
    {
        /// <summary>
        /// Timestamp du snapshot (en secondes depuis game start).
        /// </summary>
        public float Timestamp { get; set; }

        /// <summary>
        /// Informations sur le joueur.
        /// </summary>
        public PlayerInfo? Player { get; set; }

        /// <summary>
        /// Informations sur la sélection courante (NPC, settlement, etc.).
        /// </summary>
        public SelectionInfo? Selection { get; set; }

        /// <summary>
        /// État du jeu (in combat, in conversation, on map, etc.).
        /// </summary>
        public GameStateInfo GameState { get; set; } = new GameStateInfo();

        /// <summary>
        /// Données custom ajoutées par les ContextProviders.
        /// Key: providerId.key, Value: données sérialisables.
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Helper: récupère une donnée custom typée.
        /// </summary>
        public T? GetCustomData<T>(string key) where T : class
        {
            if (CustomData.TryGetValue(key, out object? value))
            {
                return value as T;
            }
            return null;
        }

        /// <summary>
        /// Helper: set une donnée custom.
        /// </summary>
        public void SetCustomData(string key, object value)
        {
            CustomData[key] = value;
        }
    }

    /// <summary>
    /// Informations sur le joueur.
    /// </summary>
    public class PlayerInfo
    {
        public string Name { get; set; } = "";
        public int Gold { get; set; }
        public int Level { get; set; }
        public bool IsInCombat { get; set; }
        public bool IsOnHorse { get; set; }
        public float Health { get; set; }
        public float HealthPercentage { get; set; }
    }

    /// <summary>
    /// Informations sur la sélection courante.
    /// </summary>
    public class SelectionInfo
    {
        public SelectionType Type { get; set; }
        public string? TargetId { get; set; }
        public string? TargetName { get; set; }
        public float DistanceToPlayer { get; set; }
    }

    public enum SelectionType
    {
        None,
        NPC,
        Settlement,
        Party,
        Troop,
        Item
    }

    /// <summary>
    /// État du jeu.
    /// </summary>
    public class GameStateInfo
    {
        public bool IsOnMap { get; set; }
        public bool IsInMission { get; set; }
        public bool IsInConversation { get; set; }
        public bool IsInInventory { get; set; }
        public bool IsPaused { get; set; }
        public string CurrentScreen { get; set; } = "";
    }
}
