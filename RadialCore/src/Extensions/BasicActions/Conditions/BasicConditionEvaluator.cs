using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;

namespace RadialCore.Extensions.BasicActions.Conditions
{
    /// <summary>
    /// Évaluateur de conditions basiques.
    /// Évalue des conditions simples basées sur le contexte.
    /// </summary>
    public class BasicConditionEvaluator : IConditionEvaluator
    {
        public string EvaluatorId => "BasicActions.ConditionEvaluator";

        public bool CanEvaluate(string conditionId)
        {
            return conditionId.StartsWith("basic.");
        }

        public bool Evaluate(string conditionId, MenuContext context)
        {
            switch (conditionId)
            {
                // Player conditions
                case "basic.player.hasGold":
                    return context.Player != null && context.Player.Gold > 0;

                case "basic.player.rich":
                    return context.Player != null && context.Player.Gold >= 1000;

                case "basic.player.healthy":
                    return context.Player != null && context.Player.HealthPercentage > 0.5f;

                case "basic.player.inCombat":
                    return context.Player != null && context.Player.IsInCombat;

                case "basic.player.onHorse":
                    return context.Player != null && context.Player.IsOnHorse;

                // NPC conditions
                case "basic.npc.nearby":
                    return context.Selection != null && 
                           context.Selection.Type == SelectionType.NPC &&
                           context.Selection.DistanceToPlayer <= 10.0f;

                case "basic.npc.close":
                    return context.Selection != null && 
                           context.Selection.Type == SelectionType.NPC &&
                           context.Selection.DistanceToPlayer <= 5.0f;

                // Game state conditions
                case "basic.gamestate.onMap":
                    return context.GameState.IsOnMap;

                case "basic.gamestate.inMission":
                    return context.GameState.IsInMission;

                case "basic.gamestate.inConversation":
                    return context.GameState.IsInConversation;

                default:
                    return false;
            }
        }
    }
}
