# ?? RadialCore Plugin Development Guide

A complete guide to creating plugins for RadialCore, from basic setup to advanced features.

---

## ?? Table of Contents

1. [Plugin Anatomy](#-plugin-anatomy)
2. [Getting Started](#-getting-started)
3. [Provider Types](#-provider-types)
4. [Advanced Features](#-advanced-features)
5. [Testing & Debugging](#-testing--debugging)
6. [Best Practices](#-best-practices)
7. [Common Patterns](#-common-patterns)
8. [Troubleshooting](#-troubleshooting)

---

## ?? Plugin Anatomy

A RadialCore plugin consists of:

```
YourPlugin/
??? YourPlugin.cs              # Main plugin class
??? Providers/
?   ??? YourMenuProvider.cs
?   ??? YourActionHandler.cs
?   ??? YourPanelProvider.cs
?   ??? YourContextProvider.cs
?   ??? YourConditionEvaluator.cs
??? Models/
    ??? YourCustomData.cs
    ??? Constants.cs
```

### Essential Files

| File | Purpose |
|------|---------|
| `YourPlugin.cs` | Main plugin class implementing `IRadialPlugin` |
| `YourMenuProvider.cs` | Provides menu entries |
| `YourActionHandler.cs` | Handles action execution |

---

## ?? Getting Started

### Step 1: Create Plugin Class

```csharp
using RadialCore.Contracts;
using RadialCore.Core.Versioning;
using RadialCore.Contracts.Core;
using System;

namespace MyMod.MyPlugin
{
    public class MyPlugin : IRadialPlugin
    {
        private PluginManifest? _manifest;
        private MyMenuProvider? _menuProvider;
        private MyActionHandler? _actionHandler;

        public PluginManifest Manifest
        {
            get
            {
                if (_manifest == null)
                {
                    _manifest = new PluginManifest(
                        pluginId: "MyMod.MyPlugin",
                        displayName: "My Custom Plugin",
                        pluginVersion: new Version(1, 0, 0),
                        requiredCoreVersion: new Version(1, 0, 0),
                        author: "Your Name",
                        description: "A custom plugin for RadialCore"
                    );

                    // Optional dependencies
                    _manifest.AddOptionalModDependency("ButterLib");
                }
                return _manifest;
            }
        }

        public void Initialize(IPluginInitializationContext context)
        {
            context.LogInfo("Initializing MyPlugin...");

            try
            {
                // Create providers
                _menuProvider = new MyMenuProvider();
                _actionHandler = new MyActionHandler(context);

                // Register providers
                context.RegisterMenuProvider(_menuProvider);
                context.RegisterActionHandler(_actionHandler);

                context.LogInfo("MyPlugin initialized successfully!");
            }
            catch (Exception ex)
            {
                context.LogError("Failed to initialize MyPlugin", ex);
                throw;
            }
        }

        public void OnTick(float deltaTime)
        {
            // Called every frame if needed
            // Most plugins don't need this
        }

        public void Shutdown()
        {
            // Cleanup resources
        }
    }
}
```

### Step 2: Create Menu Provider

```csharp
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;
using System.Collections.Generic;

namespace MyMod.MyPlugin.Providers
{
    public class MyMenuProvider : IMenuProvider
    {
        public string ProviderId => "MyMod.MyPlugin.MenuProvider";
        public int Priority => 100;

        public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
        {
            // Entry 1: Simple action
            yield return new RadialMenuEntry
            {
                EntryId = "myplugin.action1",
                Label = "$myplugin.action1.label",
                Description = "Do something",
                IconId = "icon.action1",
                Type = EntryType.Action,
                IsVisible = true,
                IsEnabled = true
            };

            // Entry 2: Conditional entry
            if (context.PlayerGold > 1000)
            {
                yield return new RadialMenuEntry
                {
                    EntryId = "myplugin.richaction",
                    Label = "$myplugin.richaction.label",
                    Type = EntryType.Action,
                    IsVisible = true,
                    IsEnabled = true
                };
            }

            // Entry 3: Submenu
            yield return new RadialMenuEntry
            {
                EntryId = "myplugin.submenu",
                Label = "$myplugin.submenu.label",
                Type = EntryType.Submenu,
                SubEntries = new List<RadialMenuEntry>
                {
                    new RadialMenuEntry
                    {
                        EntryId = "myplugin.submenu.option1",
                        Label = "$myplugin.submenu.option1"
                    },
                    new RadialMenuEntry
                    {
                        EntryId = "myplugin.submenu.option2",
                        Label = "$myplugin.submenu.option2"
                    }
                }
            };
        }
    }
}
```

### Step 3: Create Action Handler

```csharp
using RadialCore.Contracts.Core;
using RadialCore.Contracts.Models;
using System;
using System.Threading.Tasks;

namespace MyMod.MyPlugin.Providers
{
    public class MyActionHandler : IActionHandler
    {
        public string HandlerId => "MyMod.MyPlugin.ActionHandler";

        private readonly IPluginInitializationContext _context;

        public MyActionHandler(IPluginInitializationContext context)
        {
            _context = context;
        }

        public bool CanHandle(string actionId)
        {
            return actionId.StartsWith("myplugin.");
        }

        public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
        {
            try
            {
                return actionId switch
                {
                    "myplugin.action1" => await HandleAction1(context),
                    "myplugin.richaction" => await HandleRichAction(context),
                    "myplugin.submenu.option1" => await HandleOption1(context),
                    "myplugin.submenu.option2" => await HandleOption2(context),
                    _ => ActionResult.CreateFailure("Unknown action")
                };
            }
            catch (Exception ex)
            {
                _context.LogError($"Error executing {actionId}", ex);
                return ActionResult.CreateFailure($"Error: {ex.Message}", ex);
            }
        }

        private async Task<ActionResult> HandleAction1(MenuContext context)
        {
            _context.LogInfo("Executing Action 1...");
            await Task.Delay(500);  // Simulate work
            return ActionResult.CreateSuccess("Action 1 completed!");
        }

        private async Task<ActionResult> HandleRichAction(MenuContext context)
        {
            _context.LogInfo($"Executing Rich Action (Player has {context.PlayerGold} gold)");
            await Task.Delay(800);
            return ActionResult.CreateSuccess($"Rich action completed! Gold: {context.PlayerGold}");
        }

        private async Task<ActionResult> HandleOption1(MenuContext context)
        {
            await Task.Delay(300);
            return ActionResult.CreateSuccess("Option 1 selected");
        }

        private async Task<ActionResult> HandleOption2(MenuContext context)
        {
            await Task.Delay(300);
            return ActionResult.CreateSuccess("Option 2 selected");
        }
    }
}
```

### Step 4: Register Plugin

In your main mod's SubModule or bootstrapper:

```csharp
// In RadialCoreManager or your SubModule
var myPlugin = new MyPlugin();
radialCoreManager.GetPluginLoader()?.LoadPlugin(myPlugin);
```

---

## ?? Provider Types

### 1. IMenuProvider

Provides dynamic menu entries.

**Key Methods:**
- `GetMenuEntries(MenuContext)` - Returns IEnumerable<RadialMenuEntry>

**Best Practices:**
- Don't allocate lists unnecessarily
- Cache expensive computations
- Return empty enumerable if no entries available
- Use conditions in RadialMenuEntry instead of complex logic

**Example:**
```csharp
public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
{
    // Efficient: Cache list creation
    foreach (var action in _cachedActions)
    {
        if (ShouldInclude(action, context))
        {
            yield return CreateEntry(action);
        }
    }
}
```

---

### 2. IActionHandler

Executes actions (supports async).

**Key Methods:**
- `CanHandle(actionId)` - Returns true if handler can process this action
- `ExecuteActionAsync(actionId, context)` - Returns Task<ActionResult>

**Best Practices:**
- Use async/await for long operations
- Always catch exceptions
- Return meaningful ActionResult messages
- Log important operations

**Example:**
```csharp
public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
{
    try
    {
        switch (actionId)
        {
            case "myaction":
                await DoSomethingAsync();
                return ActionResult.CreateSuccess("Done!");
            
            default:
                return ActionResult.CreateFailure("Unknown action");
        }
    }
    catch (Exception ex)
    {
        _context.LogError($"Action failed: {actionId}", ex);
        return ActionResult.CreateFailure($"Error: {ex.Message}", ex);
    }
}
```

---

### 3. IPanelProvider

Provides panel content for preview, input, or modal.

**Key Methods:**
- `GetPanelContent(panelId, context)` - Returns PanelContent or null

**Panel Types:**
- **Right Panel** - Preview of selection
- **Bottom Panel** - Details
- **Modal Panel** - Confirmation
- **TextInput Panel** - Input field

**Example:**
```csharp
public PanelContent? GetPanelContent(string panelId, MenuContext context)
{
    if (panelId == "preview.myaction")
    {
        return new PanelContent
        {
            Title = "My Action Preview",
            Content = $"Gold: {context.PlayerGold}\nLevel: {context.PlayerLevel}",
            Buttons = new List<PanelButton>
            {
                new PanelButton { Label = "Execute", ActionId = "execute" },
                new PanelButton { Label = "Cancel", ActionId = "cancel" }
            }
        };
    }

    return null;
}
```

---

### 4. IContextProvider

Feeds custom data into the context snapshot.

**Key Methods:**
- `ProvideContext(MenuContext)` - Populate context.CustomData

**Example:**
```csharp
public void ProvideContext(MenuContext context)
{
    // Add custom data
    context.CustomData["myplugin.lastaction"] = _lastActionId;
    context.CustomData["myplugin.actioncount"] = _actionCount;
    context.CustomData["myplugin.timestamp"] = DateTime.UtcNow.Ticks;
}
```

---

### 5. IConditionEvaluator

Evaluates conditions for visibility/enabled state.

**Key Methods:**
- `EvaluateCondition(conditionId, context)` - Returns bool

**Example:**
```csharp
public bool EvaluateCondition(string conditionId, MenuContext context)
{
    return conditionId switch
    {
        "myplugin.hasweapon" => context.PlayerHasWeapon,
        "myplugin.incamp" => context.PlayerIsInCamp,
        "myplugin.near_npc" => context.NearbyNPC != null,
        _ => false
    };
}
```

---

## ?? Advanced Features

### 1. Capability Resolution

Access optional services:

```csharp
public void Initialize(IPluginInitializationContext context)
{
    var localization = context.ResolveCapability<ILocalizationProvider>();
    if (localization != null)
    {
        string text = localization.Localize("my.key");
    }

    var icons = context.ResolveCapability<IIconProvider>();
    if (icons != null)
    {
        string iconPath = icons.GetIconPath("icon.myicon") ?? "default.png";
    }
}
```

---

### 2. Mod Detection

Check if optional mods are loaded:

```csharp
public void Initialize(IPluginInitializationContext context)
{
    bool hasButterLib = context.IsModLoaded("ButterLib");
    if (hasButterLib)
    {
        // Use ButterLib features
        SetupWithButterLib();
    }

    bool hasMCM = context.IsModLoaded("MBOptionScreen");
    if (hasMCM)
    {
        // Register MCM settings
        RegisterMCMSettings();
    }
}
```

---

### 3. Async Operations

Handle long-running operations properly:

```csharp
public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
{
    try
    {
        // Simulate long operation
        await Task.Delay(2000);
        
        // Validate state
        if (!IsValidState(context))
        {
            return ActionResult.CreateFailure("Invalid state for action");
        }

        // Perform action
        var result = await PerformAsync();
        
        return ActionResult.CreateSuccess($"Success: {result}");
    }
    catch (OperationCanceledException)
    {
        return ActionResult.CreateFailure("Operation cancelled");
    }
    catch (Exception ex)
    {
        _context.LogError("Operation failed", ex);
        return ActionResult.CreateFailure($"Error: {ex.Message}", ex);
    }
}

private async Task<string> PerformAsync()
{
    // Actually do something
    await Task.Delay(1000);
    return "Result";
}
```

---

### 4. Hierarchical Menus

Create nested/submenu structures:

```csharp
public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
{
    var mainMenu = new RadialMenuEntry
    {
        EntryId = "myplugin.main",
        Label = "$myplugin.main",
        Type = EntryType.Submenu,
        SubEntries = new List<RadialMenuEntry>
        {
            new RadialMenuEntry
            {
                EntryId = "myplugin.main.sub1",
                Label = "$myplugin.sub1",
                Type = EntryType.Submenu,
                SubEntries = new List<RadialMenuEntry>
                {
                    new RadialMenuEntry
                    {
                        EntryId = "myplugin.main.sub1.action",
                        Label = "$myplugin.action",
                        Type = EntryType.Action
                    }
                }
            },
            new RadialMenuEntry
            {
                EntryId = "myplugin.main.sub2",
                Label = "$myplugin.sub2",
                Type = EntryType.Action
            }
        }
    };

    yield return mainMenu;
}
```

---

## ?? Testing & Debugging

### 1. Logging

Use structured logging:

```csharp
public void Initialize(IPluginInitializationContext context)
{
    context.LogInfo("Starting initialization");
    context.LogWarning("Optional dependency missing");
    context.LogError("Initialization failed", exception);
}
```

**Log File Location:**
```
My Documents\Mount and Blade II Bannerlord\Logs\RadialCore\RadialCore_[timestamp].log
```

---

### 2. In-Game Testing

Test your plugin by:
1. Starting a Sandbox game
2. Pressing hotkey (default: V)
3. Checking menu entries appear
4. Clicking entries and watching logs
5. Checking notifications appear

---

### 3. Circuit Breaker Monitoring

Monitor circuit breaker status in logs:

```
[INFO] [PluginLoader] Plugin MyPlugin loaded successfully
[ERROR] [Plugin] Exception in Initialize
[ERROR] [Plugin] Exception in Initialize
[ERROR] [Plugin] Exception in Initialize
[ERROR] [Plugin] Exception in Initialize
[ERROR] [Plugin] Exception in Initialize
[WARNING] [PluginLoader] Plugin MyPlugin initialization failed (circuit breaker tripped)
```

If you see "circuit breaker tripped", your plugin threw 5+ exceptions.

---

## ?? Best Practices

### 1. Error Handling

```csharp
// ? Good
try
{
    // logic
}
catch (Exception ex)
{
    _context.LogError("Operation failed", ex);
    return ActionResult.CreateFailure($"Error: {ex.Message}", ex);
}

// ? Bad
try
{
    // logic
}
catch { }  // Silent exception!
```

---

### 2. Performance

```csharp
// ? Bad: Allocates list every call
public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
{
    var list = new List<RadialMenuEntry>();
    // ...
    return list;
}

// ? Good: Uses yield, no allocation
public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
{
    yield return new RadialMenuEntry { ... };
}
```

---

### 3. Async/Await

```csharp
// ? Good: Async operation
public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
{
    await Task.Delay(1000);
    return ActionResult.CreateSuccess("Done!");
}

// ? Bad: Blocks game thread
public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
{
    Thread.Sleep(1000);  // BLOCKS!
    return ActionResult.CreateSuccess("Done!");
}
```

---

### 4. Localization

```csharp
// ? Good: Localized
Label = "$myplugin.action.label"

// ? Bad: Hard-coded
Label = "My Action"
```

---

## ?? Common Patterns

### Pattern 1: Conditional Entries

```csharp
public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
{
    // Always available
    yield return new RadialMenuEntry
    {
        EntryId = "myplugin.always",
        Label = "$myplugin.always",
        Type = EntryType.Action
    };

    // Only if player has gold
    if (context.PlayerGold > 0)
    {
        yield return new RadialMenuEntry
        {
            EntryId = "myplugin.expensive",
            Label = "$myplugin.expensive",
            Type = EntryType.Action,
            IsEnabled = context.PlayerGold >= 1000
        };
    }

    // Only if NPC nearby
    if (context.NearbyNPC != null)
    {
        yield return new RadialMenuEntry
        {
            EntryId = "myplugin.talk",
            Label = "$myplugin.talk",
            Type = EntryType.Action
        };
    }
}
```

---

### Pattern 2: Multiple Conditions

```csharp
public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
{
    yield return new RadialMenuEntry
    {
        EntryId = "myplugin.complex",
        Label = "$myplugin.complex",
        Type = EntryType.Action,
        VisibilityConditionId = "myplugin.canSee",
        EnabledConditionId = "myplugin.canExecute"
    };
}

// In IConditionEvaluator
public bool EvaluateCondition(string conditionId, MenuContext context)
{
    return conditionId switch
    {
        "myplugin.canSee" => !context.PlayerInConversation && context.OnMap,
        "myplugin.canExecute" => context.PlayerGold >= 100 && context.PlayerHealth > 0,
        _ => false
    };
}
```

---

### Pattern 3: Async with Validation

```csharp
public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
{
    // Validate first
    if (!ValidateContext(context))
    {
        return ActionResult.CreateFailure("Invalid context");
    }

    try
    {
        // Long operation
        var result = await FetchDataAsync();
        
        // Validate result
        if (result == null)
        {
            return ActionResult.CreateFailure("No data returned");
        }

        return ActionResult.CreateSuccess($"Success: {result}");
    }
    catch (TimeoutException)
    {
        return ActionResult.CreateFailure("Operation timed out");
    }
    catch (Exception ex)
    {
        _context.LogError("Error", ex);
        return ActionResult.CreateFailure($"Error: {ex.Message}");
    }
}
```

---

## ?? Troubleshooting

### Plugin Not Loading

**Check:**
1. Does log show "Loading plugin..."?
2. Does manifest version match Core version?
3. Are required mods loaded?

**Log Example (Error):**
```
[ERROR] [PluginLoader] Plugin MyPlugin requires Core v2.0.0, but current Core is v1.0.0
```

---

### Entries Not Appearing

**Check:**
1. Is MenuProvider registered?
2. Does GetMenuEntries() return entries?
3. Are visibility conditions evaluated correctly?

**Debug:**
```csharp
// Add logging
public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
{
    _context.LogInfo("GetMenuEntries called");
    yield return new RadialMenuEntry { ... };
}
```

---

### Action Not Executing

**Check:**
1. Is ActionHandler registered?
2. Does CanHandle() return true?
3. Is ExecuteAsync() throwing exception?

**Check Logs:**
```
[ERROR] [ActionPipeline] Exception during action execution
```

---

### Performance Issues

**Check:**
1. Are you allocating in GetMenuEntries()?
2. Are conditions expensive?
3. Are long operations blocking?

**Optimize:**
```csharp
// Cache results
private List<RadialMenuEntry> _cachedEntries;

public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
{
    if (_cachedEntries == null)
    {
        _cachedEntries = BuildEntries();
    }
    return _cachedEntries;
}
```

---

## ?? Related Documentation

- [README.md](../README.md) - Main documentation
- [CONTRACTS_V1.md](./CONTRACTS_V1.md) - API reference
- [INITIALIZATION_PHASES.md](./INITIALIZATION_PHASES.md) - Init details
- [BASICACTIONS_DEMO.md](./BASICACTIONS_DEMO.md) - Example plugin

