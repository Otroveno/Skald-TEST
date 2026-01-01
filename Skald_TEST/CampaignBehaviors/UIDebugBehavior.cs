
using TaleWorlds.CampaignSystem;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;
using AIInfluence.UI;
using AIInfluence.Services;

namespace AIInfluence.CampaignBehaviors
{
    public class UIDebugBehavior : CampaignBehaviorBase
    {
        private AIDebugScreen _screen;

        public override void RegisterEvents()
        {
            CampaignEvents.TickEvent.AddNonSerializedListener(this, OnTick);
        }

        private void OnTick(float dt)
        {
            if (Input.IsKeyPressed(InputKey.F8))
            {
                if (_screen == null)
                {
                    _screen = new AIDebugScreen();
                    ScreenManager.PushScreen(_screen);
                }
                else
                {
                    ScreenManager.PopScreen();
                    _screen = null;
                }
            }

            if (_screen != null)
            {
                AIDebugService.Flush(msg => _screen.ViewModel.AddEvent(msg));
            }
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
