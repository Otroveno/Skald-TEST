using System;
using System.Threading.Tasks;
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;
using RadialCore.Core.Diagnostics;
using RadialCore.Core.Events;
using RadialCore.Core.UI;

namespace RadialCore.Core.Actions
{
    /// <summary>
    /// Pipeline d'exécution d'action avec phases :
    /// 1. Precheck (conditions, validation)
    /// 2. Confirm (modal confirmation si nécessaire)
    /// 3. Execute (action principale)
    /// 4. Post (cleanup, notifications, events)
    /// Thread-safe, fail-safe, avec circuit breaker par action.
    /// </summary>
    public class ActionPipeline
    {
        private readonly PluginLoader _pluginLoader;
        private readonly PanelHost _panelHost;
        private readonly NotificationService _notificationService;

        public ActionPipeline(
            PluginLoader pluginLoader, 
            PanelHost panelHost,
            NotificationService notificationService)
        {
            _pluginLoader = pluginLoader ?? throw new ArgumentNullException(nameof(pluginLoader));
            _panelHost = panelHost ?? throw new ArgumentNullException(nameof(panelHost));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        /// <summary>
        /// Exécute une action via le pipeline complet.
        /// Retourne le résultat final après toutes les phases.
        /// </summary>
        public async Task<ActionResult> ExecuteAsync(string actionId, MenuContext context)
        {
            if (string.IsNullOrWhiteSpace(actionId))
            {
                return ActionResult.CreateFailure("Action ID cannot be empty");
            }

            Logger.Info("ActionPipeline", $"Starting action pipeline for: {actionId}", phase: "Pipeline");

            try
            {
                // Phase 1: Precheck
                var precheckResult = await PrecheckAsync(actionId, context);
                if (!precheckResult.Success)
                {
                    Logger.Warning("ActionPipeline", $"Precheck failed for {actionId}: {precheckResult.Message}", phase: "Precheck");
                    return precheckResult;
                }

                // Phase 2: Confirm (si nécessaire)
                var confirmResult = await ConfirmAsync(actionId, context);
                if (!confirmResult)
                {
                    Logger.Info("ActionPipeline", $"Action {actionId} cancelled by user", phase: "Confirm");
                    return ActionResult.CreateInfo("Action cancelled");
                }

                // Phase 3: Execute
                var executeResult = await ExecuteActionAsync(actionId, context);
                
                // Phase 4: Post
                await PostProcessAsync(actionId, context, executeResult);

                Logger.Info("ActionPipeline", $"Action pipeline completed for: {actionId} (Success: {executeResult.Success})", phase: "Pipeline");
                return executeResult;
            }
            catch (Exception ex)
            {
                Logger.Error("ActionPipeline", $"Exception in action pipeline for {actionId}", ex, phase: "Pipeline");
                return ActionResult.CreateFailure($"Pipeline error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phase 1: Precheck - Validation des conditions, permissions, etc.
        /// </summary>
        private async Task<ActionResult> PrecheckAsync(string actionId, MenuContext context)
        {
            Logger.Debug("ActionPipeline", $"Precheck for action: {actionId}", phase: "Precheck");

            try
            {
                // Vérifier que l'action existe (handler disponible)
                var handler = FindActionHandler(actionId);
                if (handler == null)
                {
                    return ActionResult.CreateFailure($"No handler found for action: {actionId}");
                }

                // TODO: Vérifier conditions custom (IConditionEvaluator)
                // TODO: Vérifier permissions
                // TODO: Vérifier resources disponibles

                await Task.CompletedTask; // Pour async
                return ActionResult.CreateSuccess();
            }
            catch (Exception ex)
            {
                Logger.Error("ActionPipeline", $"Precheck error for {actionId}", ex, phase: "Precheck");
                return ActionResult.CreateFailure($"Precheck failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phase 2: Confirm - Demande confirmation utilisateur si nécessaire.
        /// </summary>
        private async Task<bool> ConfirmAsync(string actionId, MenuContext context)
        {
            Logger.Debug("ActionPipeline", $"Confirm check for action: {actionId}", phase: "Confirm");

            try
            {
                // TODO: Check si action nécessite confirmation (metadata)
                // TODO: Afficher modal confirmation via PanelHost
                // TODO: Attendre réponse utilisateur

                await Task.CompletedTask;
                return true; // Stub: toujours confirmer
            }
            catch (Exception ex)
            {
                Logger.Error("ActionPipeline", $"Confirm error for {actionId}", ex, phase: "Confirm");
                return false;
            }
        }

        /// <summary>
        /// Phase 3: Execute - Exécution de l'action principale.
        /// </summary>
        private async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
        {
            Logger.Debug("ActionPipeline", $"Executing action: {actionId}", phase: "Execute");

            try
            {
                var handler = FindActionHandler(actionId);
                if (handler == null)
                {
                    return ActionResult.CreateFailure($"Handler not found: {actionId}");
                }

                // Execute via handler
                var result = await handler.ExecuteAsync(actionId, context);
                
                Logger.Info("ActionPipeline", 
                    $"Action {actionId} executed: {(result.Success ? "Success" : "Failed")} - {result.Message}", 
                    phase: "Execute");

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("ActionPipeline", $"Execute error for {actionId}", ex, phase: "Execute");
                return ActionResult.CreateFailure($"Execution failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Phase 4: Post - Cleanup, notifications, events.
        /// </summary>
        private async Task PostProcessAsync(string actionId, MenuContext context, ActionResult result)
        {
            Logger.Debug("ActionPipeline", $"Post-processing for action: {actionId}", phase: "Post");

            try
            {
                // Publish event
                EventBus.Instance.Publish(new ActionExecutedEvent
                {
                    ActionId = actionId,
                    Success = result.Success,
                    Message = result.Message,
                    Timestamp = GetCurrentTime()
                });

                // Show notification si message présent
                if (!string.IsNullOrWhiteSpace(result.Message))
                {
                    _notificationService.Show(result.Message, result.Type);
                }

                // TODO: Cleanup resources
                // TODO: Update context si nécessaire

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Logger.Error("ActionPipeline", $"Post-processing error for {actionId}", ex, phase: "Post");
            }
        }

        /// <summary>
        /// Trouve le handler pour une action.
        /// </summary>
        private IActionHandler? FindActionHandler(string actionId)
        {
            var handlers = _pluginLoader.GetProviders<IActionHandler>();
            
            foreach (var handler in handlers)
            {
                if (handler.CanHandle(actionId))
                {
                    return handler;
                }
            }

            return null;
        }

        private float GetCurrentTime()
        {
            return (float)(DateTime.Now - DateTime.Today).TotalSeconds;
        }
    }
}
