using TaleWorlds.CampaignSystem;
using Skald.Core.Interfaces;
using Skald.AI.Native;
using Skald.AI.AIInfluence;
using Skald.Core.Utils;

namespace Skald.Core
{
    public static class SkaldBootstrap
    {
        public static ISkaldAIAdapter ActiveAI;

        public static void Initialize()
        {
            SkaldContext.Reset();
            if (ModDetector.IsLoaded("AIInfluence"))
            {
                ActiveAI = new AIInfluenceAdapter();
                SkaldContext.IsAIInfluenceEnabled = true;
                SkaldContext.ActiveAIName = "AI Influence";
            }
            else
            {
                ActiveAI = new NativeAIAdapter();
            }
        }

        public static void OnCampaignLoaded()
        {
            Campaign.Current.AddCampaignBehavior(new Skald.Campaign.SkaldCampaignBehavior(ActiveAI));
        }
    }
}