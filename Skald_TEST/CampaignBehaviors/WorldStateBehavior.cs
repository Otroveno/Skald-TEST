
using TaleWorlds.CampaignSystem;
using AIInfluence.Services;

namespace AIInfluence.CampaignBehaviors
{
    public class WorldStateBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        private void OnDailyTick()
        {
            var state = WorldExporter.Export();
            JsonStore.Save("world_state.json", state);
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
