using RadialCore.Contracts.Models;

namespace RadialCore.Contracts.Core
{
    /// <summary>
    /// Interface V1 pour les évaluateurs de conditions.
    /// Un ConditionEvaluator évalue des conditions pour déterminer si une entrée est visible/enabled.
    /// Version: 1.0.0
    /// </summary>
    public interface IConditionEvaluator
    {
        /// <summary>
        /// ID unique de l'évaluateur (ex: "BasicActions.ConditionEvaluator").
        /// </summary>
        string EvaluatorId { get; }

        /// <summary>
        /// Vérifie si cet évaluateur peut gérer la condition spécifiée.
        /// </summary>
        /// <param name="conditionId">ID de la condition (ex: "player.inCombat", "npc.nearby")</param>
        /// <returns>True si l'évaluateur peut évaluer cette condition</returns>
        bool CanEvaluate(string conditionId);

        /// <summary>
        /// Évalue la condition.
        /// </summary>
        /// <param name="conditionId">ID de la condition</param>
        /// <param name="context">Contexte actuel</param>
        /// <returns>True si la condition est remplie</returns>
        bool Evaluate(string conditionId, MenuContext context);
    }
}
