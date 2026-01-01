
using TaleWorlds.CampaignSystem;
using AIInfluence.Services;

namespace AIInfluence.CampaignBehaviors
{
    public class DialogueAIBehavior : CampaignBehaviorBase
    {
        private string _line;

        public override void RegisterEvents()
        {
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
        }

        private void OnGameLoaded(CampaignGameStarter starter)
        {
            starter.AddDialogLine(
                "ai_dialogue",
                "hero_main_options",
                "ai_dialogue_response",
                "{=!}{AI_LINE}",
                CanShow,
                null
            );
        }

        private bool CanShow()
        {
            var hero = Hero.OneToOneConversationHero;
            if (hero == null) return false;
            _line = AIClient.RequestDialogue(hero).DialogueText;
            AI_LINE = _line;
            return !string.IsNullOrEmpty(_line);
        }

        public override void SyncData(IDataStore dataStore) { }
    }
}
