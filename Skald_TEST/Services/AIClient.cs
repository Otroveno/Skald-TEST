
using System.Collections.Generic;
using AIInfluence.Models;
using TaleWorlds.CampaignSystem;

namespace AIInfluence.Services
{
    public static class AIClient
    {
        public static AIResponse RequestDialogue(Hero hero)
        {
            var kingdom = hero.Clan?.Kingdom?.Name.ToString() ?? "aucun royaume";
            return new AIResponse
            {
                DialogueText = $"Le monde change, {Hero.MainHero.Name}. Les vents de guerre soufflent sur {kingdom}.",
                Tags = new() { "lore" },
                IsImportant = false
            };
        }

        public static List<WorldEvent> RequestWorldEvents()
        {
            var events = new List<WorldEvent>();
            if (Hero.MainHero.Clan?.Kingdom != null &&
                Hero.MainHero.Clan.Kingdom.IsAtWar())
            {
                events.Add(new WorldEvent
                {
                    Type = WorldEventType.ClanInfluenceChange,
                    TargetId = Hero.MainHero.Clan.StringId,
                    Value = -1f,
                    Reason = "Pressions de la guerre."
                });
            }
            return events;
        }
    }
}
