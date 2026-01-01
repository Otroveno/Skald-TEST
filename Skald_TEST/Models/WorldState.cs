
using System.Collections.Generic;

namespace AIInfluence.Models
{
    public class WorldState
    {
        public int Day { get; set; }
        public PlayerState Player { get; set; }
        public List<KingdomState> Kingdoms { get; set; }
        public List<WarState> Wars { get; set; }
    }

    public class PlayerState
    {
        public string Name { get; set; }
        public string Clan { get; set; }
        public string Kingdom { get; set; }
        public float Gold { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
    }

    public class KingdomState
    {
        public string Name { get; set; }
        public float Strength { get; set; }
    }

    public class WarState
    {
        public string KingdomA { get; set; }
        public string KingdomB { get; set; }
    }
}
