using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;

namespace RadialCore.Extensions.BasicActions.Providers
{
    /// <summary>
    /// Provider de panels basiques.
    /// Fournit des previews pour chaque action + démo text input.
    /// </summary>
    public class BasicPanelProvider : IPanelProvider
    {
        public string ProviderId => "BasicActions.PanelProvider";

        public bool CanProvide(string panelId)
        {
            return panelId.StartsWith("basicactions.preview.") || 
                   panelId.StartsWith("basicactions.input.");
        }

        public PanelContent? GetPanelContent(string panelId, MenuContext context)
        {
            switch (panelId)
            {
                case "basicactions.preview.inventory":
                    return GetInventoryPreview(context);

                case "basicactions.preview.map":
                    return GetMapPreview(context);

                case "basicactions.preview.quests":
                    return GetQuestsPreview(context);

                case "basicactions.preview.talk":
                    return GetTalkPreview(context);

                case "basicactions.input.rename":
                    return GetRenameInputPanel(context);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Preview pour Inventory.
        /// </summary>
        private PanelContent GetInventoryPreview(MenuContext context)
        {
            var player = context.Player;
            
            string content = "<b>Inventory</b>\n\n";
            
            if (player != null)
            {
                content += $"Gold: {player.Gold}\n";
                content += $"Level: {player.Level}\n";
                content += "\n<i>Opens your character inventory</i>";
            }
            else
            {
                content += "<i>Player data not available</i>";
            }

            return new PanelContent
            {
                Type = PanelType.Preview,
                Title = "Inventory",
                Content = content
            };
        }

        /// <summary>
        /// Preview pour Map.
        /// </summary>
        private PanelContent GetMapPreview(MenuContext context)
        {
            string content = "<b>World Map</b>\n\n";
            content += "View the campaign map\n";
            content += "See all settlements and parties\n";
            content += "\n<i>Opens the world map screen</i>";

            return new PanelContent
            {
                Type = PanelType.Preview,
                Title = "Map",
                Content = content
            };
        }

        /// <summary>
        /// Preview pour Quests.
        /// </summary>
        private PanelContent GetQuestsPreview(MenuContext context)
        {
            string content = "<b>Active Quests</b>\n\n";
            content += "• Quest 1: Test Quest\n";
            content += "• Quest 2: Another Quest\n";
            content += "• Quest 3: Final Quest\n";
            content += "\n<i>View and manage your quests</i>";

            return new PanelContent
            {
                Type = PanelType.Preview,
                Title = "Quests",
                Content = content
            };
        }

        /// <summary>
        /// Preview pour Talk.
        /// </summary>
        private PanelContent GetTalkPreview(MenuContext context)
        {
            var selection = context.Selection;
            
            string content = "<b>Talk</b>\n\n";
            
            if (selection != null && selection.Type == SelectionType.NPC)
            {
                content += $"Target: {selection.TargetName}\n";
                content += $"Distance: {selection.DistanceToPlayer:F1}m\n";
                content += "\n<i>Start conversation with NPC</i>";
                
                if (selection.DistanceToPlayer > 5.0f)
                {
                    content += "\n\n<color=red>? Too far away!</color>";
                }
            }
            else
            {
                content += "<i>No NPC nearby</i>";
            }

            return new PanelContent
            {
                Type = PanelType.Preview,
                Title = "Talk",
                Content = content
            };
        }

        /// <summary>
        /// Exemple de text input panel.
        /// </summary>
        private PanelContent GetRenameInputPanel(MenuContext context)
        {
            return new PanelContent
            {
                Type = PanelType.TextInput,
                Title = "Rename Item",
                Content = "Enter a new name for your item:",
                InputConfig = new TextInputConfig
                {
                    Placeholder = "Item name...",
                    InitialValue = "",
                    MaxLength = 50,
                    ValidationRegex = @"^[a-zA-Z0-9\s]+$",
                    SubmitActionId = "basicactions.rename.submit"
                }
            };
        }
    }
}
