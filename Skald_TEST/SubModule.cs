
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using AIInfluence.CampaignBehaviors;

namespace AIInfluence
{
    public class SubModule : TaleWorlds.MountAndBlade.SubModule
    {
        protected override void OnGameLoaded(Game game, object initializer)
        {
            if (game.GameType is Campaign)
            {
                var campaign = (Campaign)game.GameType;
                campaign.AddBehavior(new WorldStateBehavior());
                campaign.AddBehavior(new DialogueAIBehavior());
                campaign.AddBehavior(new WorldEventBehavior());
                campaign.AddBehavior(new UIDebugBehavior());
            }
        }
    }
}
