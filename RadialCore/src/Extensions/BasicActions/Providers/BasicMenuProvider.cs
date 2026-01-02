using System.Collections.Generic;
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;

namespace RadialCore.Extensions.BasicActions.Providers
{
    /// <summary>
    /// Provider de menu basique pour RadialCore.
    /// Fournit des entrées standards : Inventory, Map, Quests, Talk.
    /// </summary>
    public class BasicMenuProvider : IMenuProvider
    {
        public string ProviderId => "BasicActions.MenuProvider";
        public int Priority => 100; // Normal priority

        public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
        {
            var entries = new List<RadialMenuEntry>();

            // Inventory entry
            entries.Add(new RadialMenuEntry
            {
                EntryId = "basicactions.inventory",
                Label = "Inventory",
                Description = "Open your inventory",
                IconId = "icon.inventory",
                Type = EntryType.Action,
                Priority = 100,
                IsVisible = true,
                IsEnabled = true,
                PreviewPanelId = "basicactions.preview.inventory"
            });

            // Map entry
            entries.Add(new RadialMenuEntry
            {
                EntryId = "basicactions.map",
                Label = "Map",
                Description = "Open the world map",
                IconId = "icon.map",
                Type = EntryType.Action,
                Priority = 90,
                IsVisible = true,
                IsEnabled = true,
                PreviewPanelId = "basicactions.preview.map"
            });

            // Quests entry
            entries.Add(new RadialMenuEntry
            {
                EntryId = "basicactions.quests",
                Label = "Quests",
                Description = "View your active quests",
                IconId = "icon.quests",
                Type = EntryType.Action,
                Priority = 80,
                IsVisible = true,
                IsEnabled = true,
                PreviewPanelId = "basicactions.preview.quests"
            });

            // Talk entry (conditional - only if NPC nearby)
            bool hasNearbyNPC = context.Selection != null && 
                                context.Selection.Type == SelectionType.NPC;

            if (hasNearbyNPC)
            {
                entries.Add(new RadialMenuEntry
                {
                    EntryId = "basicactions.talk",
                    Label = $"Talk to {context.Selection!.TargetName}",
                    Description = $"Start conversation with {context.Selection.TargetName}",
                    IconId = "icon.talk",
                    Type = EntryType.Action,
                    Priority = 150, // Higher priority when available
                    IsVisible = true,
                    IsEnabled = context.Selection.DistanceToPlayer <= 5.0f, // Enabled only if close enough
                    PreviewPanelId = "basicactions.preview.talk"
                });
            }

            // Test submenu
            entries.Add(new RadialMenuEntry
            {
                EntryId = "basicactions.submenu.test",
                Label = "More Actions",
                Description = "Additional actions submenu",
                IconId = "icon.more",
                Type = EntryType.Submenu,
                Priority = 50,
                IsVisible = true,
                IsEnabled = true,
                SubEntries = new List<RadialMenuEntry>
                {
                    new RadialMenuEntry
                    {
                        EntryId = "basicactions.test.action1",
                        Label = "Test Action 1",
                        Description = "Simulated async action",
                        Type = EntryType.Action,
                        IsVisible = true,
                        IsEnabled = true
                    },
                    new RadialMenuEntry
                    {
                        EntryId = "basicactions.test.action2",
                        Label = "Test Action 2",
                        Description = "Another test action",
                        Type = EntryType.Action,
                        IsVisible = true,
                        IsEnabled = true
                    }
                }
            });

            return entries;
        }
    }
}
