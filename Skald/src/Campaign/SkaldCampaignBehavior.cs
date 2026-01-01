using TaleWorlds.CampaignSystem;
using Skald.Core.Interfaces;

namespace Skald.Campaign
{
    public class SkaldCampaignBehavior : CampaignBehaviorBase
    {
        private ISkaldAIAdapter _ai;

        public SkaldCampaignBehavior(ISkaldAIAdapter ai)
        {
            _ai = ai;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public override void SyncData(IDataStore dataStore) { }

        private void OnDailyTick()
        {
            _ai.OnDailyTick();
        }
    }
}