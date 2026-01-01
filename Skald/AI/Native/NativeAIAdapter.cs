using Skald.Core.Interfaces;


namespace Skald.AI.Native
{
public class NativeAIAdapter : ISkaldAIAdapter
{
public void OnDailyTick()
{
// Vanilla Bannerlord behavior
}


public float EvaluateDecisionWeight(string decisionId)
{
return 1.0f;
}
}
}