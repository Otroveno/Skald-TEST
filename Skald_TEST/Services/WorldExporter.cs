
using System.Linq;
using TaleWorlds.CampaignSystem;
using AIInfluence.Models;

namespace AIInfluence.Services
{
    public static class WorldExporter
    {
        public static WorldState Export()
        {
            var hero = Hero.MainHero;
            var campaign = Campaign.Current;

            return new WorldState
            {
                Day = (int)campaign.Time.ElapsedDaysUntilNow,
                Player = new PlayerState
                {
                    Name = hero.Name.ToString(),
                    Clan = hero.Clan?.Name.ToString(),
                    Kingdom = hero.Clan?.Kingdom?.Name.ToString(),
                    Gold = hero.Gold,
                    PosX = hero.GetPosition().x,
                    PosY = hero.GetPosition().y
                },
                Kingdoms = campaign.Kingdoms
                    .Select(k => new KingdomState
                    {
                        Name = k.Name.ToString(),
                        Strength = k.TotalStrength
                    }).ToList(),
                Wars = campaign.Kingdoms
                    .SelectMany(k => k.Stances)
                    .Where(s => s.IsAtWar)
                    .Select(s => new WarState
                    {
                        KingdomA = s.Faction1.Name.ToString(),
                        KingdomB = s.Faction2.Name.ToString()
                    }).ToList()
            };
        }
    }
}
