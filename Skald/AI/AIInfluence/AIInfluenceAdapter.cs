using Skald.Core.Interfaces;


namespace Skald.AI.AIInfluence
{
public class AIInfluenceAdapter : ISkaldAIAdapter
{
public void OnDailyTick()
{
// AI Influence enhanced behavior
}


public float EvaluateDecisionWeight(string decisionId)
{
return 1.5f;
}
}
}