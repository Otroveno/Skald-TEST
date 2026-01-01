
namespace AIInfluence.Models
{
    public enum WorldEventType
    {
        RelationChange,
        TownProsperityChange,
        ClanInfluenceChange
    }

    public class WorldEvent
    {
        public WorldEventType Type { get; set; }
        public string TargetId { get; set; }
        public float Value { get; set; }
        public string Reason { get; set; }
    }
}
