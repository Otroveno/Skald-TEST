using System;
using System.Threading.Tasks;
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;

namespace RadialCore.Extensions.BasicActions.Providers
{
    /// <summary>
    /// Handler d'actions basiques.
    /// Simule des actions async avec Task.Delay.
    /// </summary>
    public class BasicActionHandler : IActionHandler
    {
        public string HandlerId => "BasicActions.ActionHandler";

        public bool CanHandle(string actionId)
        {
            return actionId.StartsWith("basicactions.");
        }

        public async Task<ActionResult> ExecuteAsync(string actionId, MenuContext context)
        {
            try
            {
                switch (actionId)
                {
                    case "basicactions.inventory":
                        return await ExecuteInventoryAsync(context);

                    case "basicactions.map":
                        return await ExecuteMapAsync(context);

                    case "basicactions.quests":
                        return await ExecuteQuestsAsync(context);

                    case "basicactions.talk":
                        return await ExecuteTalkAsync(context);

                    case "basicactions.test.action1":
                        return await ExecuteTestAction1Async(context);

                    case "basicactions.test.action2":
                        return await ExecuteTestAction2Async(context);

                    default:
                        return ActionResult.CreateFailure($"Unknown action: {actionId}");
                }
            }
            catch (Exception ex)
            {
                return ActionResult.CreateFailure($"Action failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Simule l'ouverture de l'inventaire.
        /// </summary>
        private async Task<ActionResult> ExecuteInventoryAsync(MenuContext context)
        {
            // Simulate async operation (API call, loading, etc.)
            await Task.Delay(500);

            // TODO: Actually open inventory screen
            // For now, just return success

            return ActionResult.CreateSuccess("Inventory opened (simulated)");
        }

        /// <summary>
        /// Simule l'ouverture de la map.
        /// </summary>
        private async Task<ActionResult> ExecuteMapAsync(MenuContext context)
        {
            await Task.Delay(300);

            // TODO: Actually open map screen
            
            return ActionResult.CreateSuccess("Map opened (simulated)");
        }

        /// <summary>
        /// Simule l'ouverture des quests.
        /// </summary>
        private async Task<ActionResult> ExecuteQuestsAsync(MenuContext context)
        {
            await Task.Delay(400);

            // TODO: Actually open quests screen
            
            var questCount = 3; // Stub
            return ActionResult.CreateSuccess($"Quests opened - {questCount} active quests");
        }

        /// <summary>
        /// Simule une conversation avec un NPC.
        /// </summary>
        private async Task<ActionResult> ExecuteTalkAsync(MenuContext context)
        {
            if (context.Selection == null || context.Selection.Type != SelectionType.NPC)
            {
                return ActionResult.CreateFailure("No NPC selected");
            }

            await Task.Delay(600);

            // TODO: Actually start conversation
            
            string npcName = context.Selection.TargetName ?? "Unknown";
            return ActionResult.CreateSuccess($"Started conversation with {npcName} (simulated)");
        }

        /// <summary>
        /// Test action 1 - Simule une action longue.
        /// </summary>
        private async Task<ActionResult> ExecuteTestAction1Async(MenuContext context)
        {
            // Simulate long operation
            await Task.Delay(2000);

            return ActionResult.CreateSuccess("Test Action 1 completed after 2 seconds!");
        }

        /// <summary>
        /// Test action 2 - Simule un warning.
        /// </summary>
        private async Task<ActionResult> ExecuteTestAction2Async(MenuContext context)
        {
            await Task.Delay(1000);

            return ActionResult.CreateWarning("Test Action 2 completed with warning");
        }
    }
}
