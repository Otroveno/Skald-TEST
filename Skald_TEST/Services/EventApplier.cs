
using AIInfluence.Models;
using TaleWorlds.CampaignSystem;
using System;

namespace AIInfluence.Services
{
    public static class EventApplier
    {
        public static void Apply(WorldEvent evt)
        {
            switch (evt.Type)
            {
                case WorldEventType.ClanInfluenceChange:
                    var clan = Hero.MainHero.Clan;
                    if (clan != null)
                        clan.Influence = Math.Clamp(clan.Influence + evt.Value, -10000f, 100000f);
                    break;
            }
        }
    }
}
