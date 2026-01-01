using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using Skald.Core.Interfaces;
using Skald.AI.Native;
using Skald.AI.AIInfluence;
using Skald.Core.Utils;


namespace Skald.Core
{
public static class SkaldBootstrap
{
public static ISkaldAIAdapter ActiveAI { get; private set; }


public static void Initialize()
{
if (ModDetector.IsLoaded("AIInfluence"))
ActiveAI = new AIInfluenceAdapter();
else
ActiveAI = new NativeAIAdapter();
}


public static void OnCampaignLoaded()
{
Campaign.Current.AddCampaignBehavior(new SkaldCampaignBehavior(ActiveAI));
}
}
}