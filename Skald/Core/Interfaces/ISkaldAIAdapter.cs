namespace Skald.Core.Interfaces
{
public interface ISkaldAIAdapter
{
void OnDailyTick();
float EvaluateDecisionWeight(string decisionId);
}
}