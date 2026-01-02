# RadialCore Contracts V1 - API Reference

**Version:** 1.0.0  
**Date:** 2026-01-02

## ?? Vue d'ensemble

Les **Contracts V1** définissent l'API publique pour les plugins RadialCore. Tous les providers et interfaces sont versionnés et stables.

## ?? Interfaces Principales

### IRadialPlugin

Interface de base pour tous les plugins.

```csharp
public interface IRadialPlugin
{
    PluginManifest Manifest { get; }
    void Initialize(IPluginInitializationContext context);
    void OnTick(float deltaTime);
    void Shutdown();
}
```

**Exemple minimal :**
```csharp
public class MyPlugin : IRadialPlugin
{
    public PluginManifest Manifest => new PluginManifest(
        pluginId: "MyMod.MyPlugin",
        displayName: "My Plugin",
        pluginVersion: new Version(1, 0, 0),
        requiredCoreVersion: new Version(1, 0, 0)
    );

    public void Initialize(IPluginInitializationContext context)
    {
        context.RegisterMenuProvider(new MyMenuProvider());
        context.RegisterActionHandler(new MyActionHandler());
    }

    public void OnTick(float deltaTime) { }
    public void Shutdown() { }
}
```

---

## ?? Providers

### 1. IMenuProvider

Fournit des entrées de menu dynamiques.

```csharp
public interface IMenuProvider
{
    string ProviderId { get; }
    int Priority { get; }
    IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context);
}
```

**Exemple :**
```csharp
public class MyMenuProvider : IMenuProvider
{
    public string ProviderId => "MyPlugin.MenuProvider";
    public int Priority => 100;

    public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
    {
        yield return new RadialMenuEntry
        {
            EntryId = "myaction.do",
            Label = "Do Something",
            Description = "Does something cool",
            IconId = "icon.star",
            Type = EntryType.Action,
            Priority = 100
        };
    }
}
```

### 2. IActionHandler

Exécute des actions (sync/async).

```csharp
public interface IActionHandler
{
    string HandlerId { get; }
    bool CanHandle(string actionId);
    Task<ActionResult> ExecuteAsync(string actionId, MenuContext context);
}
```

**Exemple :**
```csharp
public class MyActionHandler : IActionHandler
{
    public string HandlerId => "MyPlugin.ActionHandler";

    public bool CanHandle(string actionId)
    {
        return actionId.StartsWith("myaction.");
    }

    public async Task<ActionResult> ExecuteAsync(string actionId, MenuContext context)
    {
        if (actionId == "myaction.do")
        {
            // Action async (API call, loading, etc.)
            await Task.Delay(1000);
            return ActionResult.CreateSuccess("Action completed!");
        }
        
        return ActionResult.CreateFailure("Unknown action");
    }
}
```

### 3. IPanelProvider

Fournit du contenu UI pour les panels.

```csharp
public interface IPanelProvider
{
    string ProviderId { get; }
    bool CanProvide(string panelId);
    PanelContent? GetPanelContent(string panelId, MenuContext context);
}
```

**Exemple (Preview Panel) :**
```csharp
public class MyPanelProvider : IPanelProvider
{
    public string ProviderId => "MyPlugin.PanelProvider";

    public bool CanProvide(string panelId)
    {
        return panelId == "myaction.preview";
    }

    public PanelContent? GetPanelContent(string panelId, MenuContext context)
    {
        return new PanelContent
        {
            Type = PanelType.Preview,
            Title = "My Action",
            Content = "This action will do something cool!"
        };
    }
}
```

**Exemple (Modal Confirmation) :**
```csharp
public PanelContent GetConfirmModal()
{
    return new PanelContent
    {
        Type = PanelType.ConfirmModal,
        Title = "Confirm Action",
        Content = "Are you sure you want to do this?",
        Buttons = new List<PanelButton>
        {
            new PanelButton
            {
                Label = "Yes",
                ActionId = "myaction.confirm",
                Style = ButtonStyle.Primary
            },
            new PanelButton
            {
                Label = "No",
                ActionId = "myaction.cancel",
                Style = ButtonStyle.Default
            }
        }
    };
}
```

### 4. IContextProvider

Alimente le ContextHub avec des données.

```csharp
public interface IContextProvider
{
    string ProviderId { get; }
    int Priority { get; }
    void ProvideContext(MenuContext context);
}
```

**Exemple :**
```csharp
public class MyContextProvider : IContextProvider
{
    public string ProviderId => "MyPlugin.ContextProvider";
    public int Priority => 100;

    public void ProvideContext(MenuContext context)
    {
        // Ajouter des données custom au context
        context.SetCustomData("MyPlugin.customValue", 42);
        context.SetCustomData("MyPlugin.timestamp", DateTime.Now);
    }
}
```

### 5. IConditionEvaluator

Évalue des conditions pour visible/enabled.

```csharp
public interface IConditionEvaluator
{
    string EvaluatorId { get; }
    bool CanEvaluate(string conditionId);
    bool Evaluate(string conditionId, MenuContext context);
}
```

**Exemple :**
```csharp
public class MyConditionEvaluator : IConditionEvaluator
{
    public string EvaluatorId => "MyPlugin.ConditionEvaluator";

    public bool CanEvaluate(string conditionId)
    {
        return conditionId.StartsWith("my.");
    }

    public bool Evaluate(string conditionId, MenuContext context)
    {
        switch (conditionId)
        {
            case "my.player.rich":
                return context.Player?.Gold > 1000;
            
            case "my.player.healthy":
                return context.Player?.HealthPercentage > 0.5f;
            
            default:
                return false;
        }
    }
}
```

**Utilisation dans MenuEntry :**
```csharp
new RadialMenuEntry
{
    EntryId = "myaction.expensive",
    Label = "Expensive Action",
    VisibilityConditionId = "my.player.rich", // Visible seulement si riche
    EnabledConditionId = "my.player.healthy"  // Enabled seulement si en bonne santé
}
```

---

## ?? Modèles

### MenuContext

Snapshot immutable du contexte de jeu.

```csharp
public class MenuContext
{
    public float Timestamp { get; set; }
    public PlayerInfo? Player { get; set; }
    public SelectionInfo? Selection { get; set; }
    public GameStateInfo GameState { get; set; }
    public Dictionary<string, object> CustomData { get; set; }
    
    public T? GetCustomData<T>(string key) where T : class;
    public void SetCustomData(string key, object value);
}
```

### RadialMenuEntry

Entrée de menu radial.

```csharp
public class RadialMenuEntry
{
    public string EntryId { get; set; }              // ID unique
    public string Label { get; set; }                // Texte affiché
    public string Description { get; set; }          // Tooltip
    public string IconId { get; set; }               // ID icône
    public EntryType Type { get; set; }              // Action, Submenu, Separator
    public int Priority { get; set; }                // 0-1000
    public bool IsVisible { get; set; }              // Visible
    public bool IsEnabled { get; set; }              // Enabled (cliquable)
    public string? VisibilityConditionId { get; set; }  // Condition pour visible
    public string? EnabledConditionId { get; set; }     // Condition pour enabled
    public string? PreviewPanelId { get; set; }         // Panel preview au hover
    public List<RadialMenuEntry> SubEntries { get; set; } // Sous-entrées si Submenu
}
```

### ActionResult

Résultat d'exécution d'action.

```csharp
public class ActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public ResultType Type { get; set; }  // Info, Success, Warning, Error
    public Exception? Exception { get; set; }
    public object? Data { get; set; }
    
    // Helpers
    public static ActionResult CreateSuccess(string message = "");
    public static ActionResult CreateFailure(string message, Exception? ex = null);
    public static ActionResult CreateInfo(string message);
    public static ActionResult CreateWarning(string message);
}
```

### PanelContent

Contenu d'un panel UI.

```csharp
public class PanelContent
{
    public PanelType Type { get; set; }        // Preview, ConfirmModal, InfoModal, TextInput
    public string Title { get; set; }
    public string Content { get; set; }
    public List<PanelButton> Buttons { get; set; }
    public TextInputConfig? InputConfig { get; set; }
}
```

---

## ?? Best Practices

### 1. Naming Conventions

- **PluginId** : `ModName.PluginName` (ex: `MyMod.DialogueMenu`)
- **ProviderId** : `PluginId.ProviderType` (ex: `MyMod.DialogueMenu.MenuProvider`)
- **ActionId** : `category.action` (ex: `dialogue.start`, `inventory.open`)
- **ConditionId** : `category.condition` (ex: `player.inCombat`, `npc.nearby`)
- **IconId** : `icon.name` (ex: `icon.inventory`, `icon.map`)

### 2. Priority System

- **0-50** : Low priority (fallback actions)
- **50-100** : Normal priority (default)
- **100-500** : High priority (important actions)
- **500-1000** : Critical priority (emergency actions)

### 3. Error Handling

Toujours catch exceptions et retourner un `ActionResult` propre :

```csharp
public async Task<ActionResult> ExecuteAsync(string actionId, MenuContext context)
{
    try
    {
        // Action code
        return ActionResult.CreateSuccess("Done!");
    }
    catch (Exception ex)
    {
        return ActionResult.CreateFailure($"Failed: {ex.Message}", ex);
    }
}
```

### 4. Performance

- **Avoid allocations** dans `GetMenuEntries()` (appelé fréquemment)
- **Cache** les données lourdes dans le plugin
- **Async** pour actions longues (> 100ms)
- **Conditions** pour éviter calculs inutiles

### 5. Localization

Préfixer les labels avec `$` pour traduction automatique :

```csharp
new RadialMenuEntry
{
    Label = "$radial.action.inventory",  // Traduit par ILocalizationProvider
    Description = "$radial.action.inventory.desc"
}
```

---

## ?? Lifecycle

### Plugin Lifecycle

```
1. Constructor
2. Initialize(context)
   - Register providers
   - Setup capabilities
3. OnTick(dt) [optionnel]
4. Shutdown()
```

### Menu Lifecycle

```
1. User opens radial menu (hotkey)
2. ContextHub refresh snapshot
3. GetMenuEntries() called on all MenuProviders
4. Conditions evaluated (VisibilityConditionId, EnabledConditionId)
5. Menu displayed
6. User hovers entry ? GetPanelContent() for preview
7. User clicks entry ? ExecuteAsync() on ActionHandler
8. Menu closes
```

---

## ?? Changelog

### V1.0.0 (2026-01-02)
- Initial release
- Core interfaces: IMenuProvider, IActionHandler, IPanelProvider, IContextProvider, IConditionEvaluator
- Models: MenuContext, RadialMenuEntry, ActionResult, PanelContent
- Circuit breaker protection
- Logging utilities

---

## ?? Liens

- [README principal](../README.md)
- [Architecture](ARCHITECTURE.md)
- [Examples](EXAMPLES.md)
