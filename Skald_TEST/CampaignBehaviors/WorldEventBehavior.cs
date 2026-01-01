
using TaleWorlds.CampaignSystem;
using AIInfluence.Services;

namespace AIInfluence.CampaignBehaviors
{
    public class WorldEventBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        private void OnDailyTick()
        {
            int day = (int)Campaign.Current.Time.ElapsedDaysUntilNow;
            foreach (var evt in AIClient.RequestWorldEvents())
            {
                var key = evt.Type + ":" + evt.TargetId;
                if (EventCooldowns.CanApply(key, day, 7))
                {
                    EventApplier.Apply(evt);
                    AIDebugService.Log(evt.Reason);
                }
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            var data = EventCooldowns.Data;
            dataStore.SyncData("AIInfluence.Cooldowns", ref data);
        }
    }
}
