using System;
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;

namespace RadialCore.Extensions.BasicActions.Providers
{
    /// <summary>
    /// Provider de contexte basique.
    /// Ajoute des données custom au contexte.
    /// </summary>
    public class BasicContextProvider : IContextProvider
    {
        public string ProviderId => "BasicActions.ContextProvider";
        public int Priority => 100;

        public void ProvideContext(MenuContext context)
        {
            try
            {
                // Add timestamp
                context.SetCustomData("BasicActions.Timestamp", DateTime.Now);

                // Add session data
                context.SetCustomData("BasicActions.SessionId", Guid.NewGuid().ToString());

                // Add computed data
                if (context.Player != null)
                {
                    // Player wealth category
                    string wealthCategory = GetWealthCategory(context.Player.Gold);
                    context.SetCustomData("BasicActions.Player.WealthCategory", wealthCategory);

                    // Player health status
                    string healthStatus = GetHealthStatus(context.Player.HealthPercentage);
                    context.SetCustomData("BasicActions.Player.HealthStatus", healthStatus);
                }

                // Add environment data
                context.SetCustomData("BasicActions.Environment.TimeOfDay", "Day"); // Stub
                context.SetCustomData("BasicActions.Environment.Weather", "Clear"); // Stub
            }
            catch (Exception)
            {
                // Fail gracefully
            }
        }

        private string GetWealthCategory(int gold)
        {
            if (gold < 100) return "Poor";
            if (gold < 1000) return "Middle";
            if (gold < 10000) return "Rich";
            return "Very Rich";
        }

        private string GetHealthStatus(float healthPercentage)
        {
            if (healthPercentage > 0.75f) return "Healthy";
            if (healthPercentage > 0.5f) return "Wounded";
            if (healthPercentage > 0.25f) return "Injured";
            return "Critical";
        }
    }
}
