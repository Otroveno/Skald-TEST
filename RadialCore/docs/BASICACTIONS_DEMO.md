# BasicActions Plugin - Guide de Démonstration

**Plugin ID:** `Radial.BasicActions`  
**Version:** 1.0.0  
**Type:** Built-in plugin (intégré au Core)

## ?? Vue d'ensemble

Le plugin BasicActions démontre **toutes les fonctionnalités** du système RadialCore :
- ? **IMenuProvider** : Entrées de menu dynamiques
- ? **IActionHandler** : Actions async avec notifications
- ? **IPanelProvider** : Previews + text input
- ? **IContextProvider** : Données contextuelles custom
- ? **IConditionEvaluator** : Conditions visible/enabled

## ?? Actions Disponibles

### 1. **Inventory** (`basicactions.inventory`)
- **Description** : Ouvre l'inventaire (simulé)
- **Icône** : `icon.inventory`
- **Preview Panel** : Affiche Gold, Level
- **Action** : Async 500ms + notification "Inventory opened"
- **Toujours visible**

### 2. **Map** (`basicactions.map`)
- **Description** : Ouvre la carte du monde (simulé)
- **Icône** : `icon.map`
- **Preview Panel** : Description de la map
- **Action** : Async 300ms + notification "Map opened"
- **Toujours visible**

### 3. **Quests** (`basicactions.quests`)
- **Description** : Affiche les quêtes actives (simulé)
- **Icône** : `icon.quests`
- **Preview Panel** : Liste de 3 quêtes stub
- **Action** : Async 400ms + notification "Quests opened - 3 active quests"
- **Toujours visible**

### 4. **Talk** (`basicactions.talk`) - **Conditionnel**
- **Description** : Parler à un NPC proche
- **Icône** : `icon.talk`
- **Preview Panel** : Nom du NPC + distance
- **Action** : Async 600ms + notification "Started conversation with [NPC]"
- **Visible** : Seulement si `context.Selection.Type == NPC`
- **Enabled** : Seulement si distance ? 5.0m
- **Priority** : 150 (high) quand disponible

### 5. **More Actions** (Submenu)
- **Sous-entrées** :
  - **Test Action 1** : Async 2s (action longue simulée)
  - **Test Action 2** : Async 1s + warning notification

## ?? Preview Panels

Tous les panels sont fournis par `BasicPanelProvider` :

### Inventory Preview
```
<b>Inventory</b>

Gold: 500
Level: 5

<i>Opens your character inventory</i>
```

### Map Preview
```
<b>World Map</b>

View the campaign map
See all settlements and parties

<i>Opens the world map screen</i>
```

### Quests Preview
```
<b>Active Quests</b>

• Quest 1: Test Quest
• Quest 2: Another Quest
• Quest 3: Final Quest

<i>View and manage your quests</i>
```

### Talk Preview (si NPC proche)
```
<b>Talk</b>

Target: John the Merchant
Distance: 3.2m

<i>Start conversation with NPC</i>
```

Si trop loin :
```
? Too far away!
```

## ?? Context Custom Data

Le `BasicContextProvider` ajoute ces données au contexte :

| Clé | Type | Description |
|-----|------|-------------|
| `BasicActions.Timestamp` | DateTime | Timestamp du contexte |
| `BasicActions.SessionId` | string (GUID) | ID de session unique |
| `BasicActions.Player.WealthCategory` | string | Poor / Middle / Rich / Very Rich |
| `BasicActions.Player.HealthStatus` | string | Healthy / Wounded / Injured / Critical |
| `BasicActions.Environment.TimeOfDay` | string | Day (stub) |
| `BasicActions.Environment.Weather` | string | Clear (stub) |

## ?? Conditions Disponibles

Le `BasicConditionEvaluator` supporte ces conditions :

### Conditions Joueur
- `basic.player.hasGold` : Gold > 0
- `basic.player.rich` : Gold ? 1000
- `basic.player.healthy` : HealthPercentage > 0.5
- `basic.player.inCombat` : IsInCombat == true
- `basic.player.onHorse` : IsOnHorse == true

### Conditions NPC
- `basic.npc.nearby` : NPC dans 10m
- `basic.npc.close` : NPC dans 5m

### Conditions État du Jeu
- `basic.gamestate.onMap` : Sur la carte campagne
- `basic.gamestate.inMission` : En mission
- `basic.gamestate.inConversation` : En conversation

## ?? Test en Jeu

### Scénario 1 : Menu de Base
1. Appuyer sur `V` (hotkey par défaut)
2. Le menu radial s'ouvre (stub logs)
3. **4 entrées visibles** : Inventory, Map, Quests, More Actions

### Scénario 2 : Talk Conditionnel
1. Se rapprocher d'un NPC (distance ? 10m)
2. Ouvrir le menu radial
3. **5 entrées visibles** : + Talk to [NPC]
4. Si distance > 5m : Talk est grisé (disabled)
5. Si distance ? 5m : Talk est cliquable

### Scénario 3 : Action Async
1. Cliquer sur "Inventory"
2. **ActionPipeline** :
   - Phase 1: Precheck ?
   - Phase 2: Confirm ?
   - Phase 3: Execute (500ms delay)
   - Phase 4: Post (notification)
3. **Notification** : "Inventory opened (simulated)" (3s TTL)
4. Menu se ferme

### Scénario 4 : Action Longue
1. More Actions > Test Action 1
2. Execute avec 2s delay
3. Notification : "Test Action 1 completed after 2 seconds!"

## ?? Logs Attendus

```
[INFO] [Plugin:Radial.BasicActions] Initializing BasicActions plugin...
[INFO] [Plugin:Radial.BasicActions] Registered BasicMenuProvider
[INFO] [Plugin:Radial.BasicActions] Registered BasicActionHandler
[INFO] [Plugin:Radial.BasicActions] Registered BasicPanelProvider
[INFO] [Plugin:Radial.BasicActions] Registered BasicContextProvider
[INFO] [Plugin:Radial.BasicActions] Registered BasicConditionEvaluator
[INFO] [Plugin:Radial.BasicActions] BasicActions plugin initialized successfully
[INFO] [Plugin:Radial.BasicActions] Available actions: inventory, map, quests, talk (conditional)

[DEBUG] [MenuManager] Collecting entries from 1 menu providers
[DEBUG] [MenuManager] Provider BasicActions.MenuProvider provided 5 entries
[INFO] [MenuManager] Collected 5 total menu entries

[INFO] [ActionPipeline] [Phase:Pipeline] Starting action pipeline for: basicactions.inventory
[DEBUG] [ActionPipeline] [Phase:Precheck] Precheck for action: basicactions.inventory
[DEBUG] [ActionPipeline] [Phase:Confirm] Confirm check for action: basicactions.inventory
[DEBUG] [ActionPipeline] [Phase:Execute] Executing action: basicactions.inventory
[INFO] [ActionPipeline] [Phase:Execute] Action basicactions.inventory executed: Success - Inventory opened (simulated)
[DEBUG] [ActionPipeline] [Phase:Post] Post-processing for action: basicactions.inventory
[INFO] [ActionPipeline] [Phase:Pipeline] Action pipeline completed for: basicactions.inventory (Success: True)

[DEBUG] [NotificationService] Notification added: [Success] Inventory opened (simulated)
```

## ?? Extensibilité

Ce plugin démontre comment créer vos propres plugins :

### Créer un MenuProvider
```csharp
public class MyMenuProvider : IMenuProvider
{
    public string ProviderId => "MyPlugin.MenuProvider";
    public int Priority => 100;

    public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
    {
        yield return new RadialMenuEntry
        {
            EntryId = "myplugin.action1",
            Label = "My Action",
            IconId = "icon.custom",
            Type = EntryType.Action
        };
    }
}
```

### Créer un ActionHandler
```csharp
public class MyActionHandler : IActionHandler
{
    public string HandlerId => "MyPlugin.ActionHandler";

    public bool CanHandle(string actionId) 
        => actionId.StartsWith("myplugin.");

    public async Task<ActionResult> ExecuteAsync(string actionId, MenuContext context)
    {
        await Task.Delay(1000);
        return ActionResult.CreateSuccess("Done!");
    }
}
```

## ?? Prochaines Étapes

Pour rendre BasicActions **vraiment fonctionnel** :

1. ? **FAIT** : Providers complets
2. **TODO** : Vraie intégration APIs Bannerlord :
   - `ExecuteInventoryAsync()` ? Ouvrir vrai screen inventory
   - `ExecuteMapAsync()` ? Ouvrir vrai screen map
   - `ExecuteTalkAsync()` ? Démarrer vraie conversation
3. **TODO** : Vrai NPC proximity detection (Mission.Current.Agents)
4. **TODO** : Vraies icônes (IIconProvider)
5. **TODO** : Vraie UI Gauntlet pour affichage visuel

Le système est **architecturalement complet** et prêt pour extension ! ??
