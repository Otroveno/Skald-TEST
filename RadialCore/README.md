# RadialCore - Mount & Blade II: Bannerlord Radial Menu System

**Version:** 1.0.0  
**Target:** Mount & Blade II: Bannerlord v1.3.10  
**Framework:** .NET Framework 4.7.2  
**Status:** ? Production Ready (UI Gauntlet pending)

---

## ?? Table of Contents

1. [Overview](#-overview)
2. [Architecture](#-architecture)
3. [Quick Start](#-quick-start)
4. [Creating a Plugin](#-creating-a-plugin)
5. [Extension Points](#-extension-points)
6. [Available Services](#-available-services)
7. [Configuration](#-configuration)
8. [Debugging](#-debugging)
9. [Performance](#-performance)
10. [Roadmap](#-roadmap)
11. [Documentation](#-documentation)

---

## ?? Overview

**RadialCore** is an extensible, modular radial menu system for Mount & Blade II: Bannerlord. It provides:

- ? **Plugin-Based Architecture** : Extensible system with manifests and versioning
- ? **Capability-Based Design** : Plugins request optional services via `ICapabilityResolver`
- ? **Fail-Safe** : Circuit breaker protects Core from plugin exceptions
- ? **Contract-First** : Versioned interfaces, no circular dependencies
- ? **Structured Logging** : Detailed logs with pluginId, phase, timestamp
- ? **Mod Detection** : Automatic detection of optional mods (ButterLib, MCM, UIExtenderEx, etc.)

### Key Features

| Feature | Status | Details |
|---------|--------|---------|
| **Plugin System** | ? Complete | Versioning, circuit breaker, hot-reload ready |
| **Context System** | ? Complete | Immutable snapshots, 2x/sec refresh |
| **Event Bus** | ? Complete | Pub/sub, 6 predefined events |
| **Action Pipeline** | ? Complete | 4 phases (Precheck, Confirm, Execute, Post) |
| **Panel System** | ? Complete | 4 slots (Right, Bottom, Modal, TextInput) |
| **Notification System** | ? Complete | Queue with TTL, 4 types |
| **Diagnostics** | ? Complete | Logging, circuit breaker, debug overlay |
| **Input System** | ? Partial | Stub polling (TODO: real API) |
| **UI Gauntlet** | ? Pending | TODO: Screens and layout |
| **BasicActions Plugin** | ? Complete | 5 providers + 6 actions + 11 conditions |

---

## ??? Architecture

### Component Diagram

```
???????????????????????????????????????????????????????????
?                    SubModule.cs                         ?
?            (Bannerlord Mod Entry Point)                 ?
???????????????????????????????????????????????????????????
                           ?
                           ?
???????????????????????????????????????????????????????????
?                 RadialCoreManager                       ?
?         (Central Orchestrator - 7 Phases)               ?
???????????????????????????????????????????????????????????
           ?
    ?????????????????????????????????????????????
    ?                        ?                  ?
????????????????    ????????????????    ???????????????
?PluginLoader ?    ?ContextHub    ?    ?   EventBus  ?
?              ?    ?              ?    ?             ?
?• Discover    ?    ?• Snapshots   ?    ?• Pub/Sub    ?
?• Load        ?    ?• Refresh     ?    ?• 6 Events   ?
?• Validate    ?    ?• Providers   ?    ?• Fail-safe  ?
?• Protect     ?    ?• Services    ?    ???????????????
????????????????    ????????????????
    ?                   ?
    ? Plugins           ? Services
    ?                   ?
????????????????????????????????????????
?      UI Systems                      ?
?  ??????????????????????????????????? ?
?  ? MenuManager                     ? ?
?  ? ?? PanelHost (4 slots)          ? ?
?  ? ?? NotificationService (queue)  ? ?
?  ? ?? ActionPipeline (4 phases)    ? ?
?  ? ?? InputManager (hotkeys)       ? ?
?  ??????????????????????????????????? ?
????????????????????????????????????????
           ?
    ???????????????????????????????????????
    ?                      ?              ?
???????????????    ????????????????   ?????????????
?  Contracts  ?    ?   Services   ?   ?Diagnostics?
?             ?    ?              ?   ?           ?
?• IMenuProv. ?    ?• ModPresence ?   ?• Logger   ?
?• IActionH.  ?    ?• Capabilities?   ?• CircuitB.?
?• IPanelProv.?    ?• PlayerState ?   ?• DebugO.  ?
?• IContextP. ?    ?• NPCProximity?   ?????????????
?• IConditionE?    ????????????????
???????????????
```

### Plugin System Architecture

```
IRadialPlugin (Interface)
    ?
    ?? Manifest (pluginId, version, dependencies)
    ?
    ?? Initialize(context)
    ?  ?? RegisterMenuProvider()
    ?  ?? RegisterActionHandler()
    ?  ?? RegisterPanelProvider()
    ?  ?? RegisterContextProvider()
    ?  ?? RegisterConditionEvaluator()
    ?
    ?? OnTick(deltaTime)
    ?
    ?? Shutdown()

PluginInitializationContext
    ?? ResolveCapability<T>()      # Service Locator
    ?? IsModLoaded(modId)          # Mod detection
    ?? LogInfo/Warning/Error()     # Logging
```

---

## ?? Quick Start

### Installation

1. **Clone/Download** the RadialCore mod folder
2. **Build** the project:
   ```bash
   dotnet build
   # or in Visual Studio: Build > Build Solution
   ```
3. **Deploy** to Bannerlord Modules:
   ```
   C:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\RadialCore\
   ```
4. **Launch** Bannerlord Launcher
5. **Enable** RadialCore mod in the mods list
6. **Start** a Sandbox/Campaign game

### Verification

Open the log file:
```
My Documents\Mount and Blade II Bannerlord\Logs\RadialCore\RadialCore_[timestamp].log
```

You should see:
```
[2025-01-02 02:30:00.000] [INFO] [SubModule] RadialCore v1.0.0 loading...
[2025-01-02 02:30:00.001] [INFO] [RadialCoreManager] === Initialization Phase 1: Core Services ===
[2025-01-02 02:30:00.002] [INFO] [ModPresenceService] Module scanning complete
[2025-01-02 02:30:00.003] [INFO] [PluginLoader] Loading plugin: Basic Actions v1.0.0 (ID: Radial.BasicActions)
[2025-01-02 02:30:00.004] [INFO] [RadialCoreManager] RadialCore initialized successfully!
[2025-01-02 02:30:00.005] [INFO] [DebugOverlay] === FULL STATE DUMP ===
[2025-01-02 02:30:00.006] [INFO] [DebugOverlay] Total Plugins: 1
```

---

## ?? Creating a Plugin

### Step 1: Create the Plugin Class

```csharp
using RadialCore.Contracts;
using RadialCore.Core.Versioning;
using RadialCore.Contracts.Core;

namespace YourNamespace.YourPlugin
{
    public class YourPlugin : IRadialPlugin
    {
        private PluginManifest? _manifest;

        public PluginManifest Manifest
        {
            get
            {
                if (_manifest == null)
                {
                    _manifest = new PluginManifest(
                        pluginId: "YourMod.YourPlugin",
                        displayName: "Your Plugin Name",
                        pluginVersion: new Version(1, 0, 0),
                        requiredCoreVersion: new Version(1, 0, 0),
                        author: "Your Name",
                        description: "A brief description of your plugin"
                    );

                    // Optional mod dependencies
                    _manifest.AddOptionalModDependency("ButterLib");
                    _manifest.AddOptionalModDependency("UIExtenderEx");
                }
                return _manifest;
            }
        }

        public void Initialize(IPluginInitializationContext context)
        {
            context.LogInfo("Initializing YourPlugin...");

            // Register your providers here
            context.RegisterMenuProvider(new YourMenuProvider());
            context.RegisterActionHandler(new YourActionHandler());

            context.LogInfo("YourPlugin initialized successfully!");
        }

        public void OnTick(float deltaTime)
        {
            // Update logic (optional - most plugins don't need this)
        }

        public void Shutdown()
        {
            // Cleanup
        }
    }
}
```

### Step 2: Register the Plugin

In your main mod SubModule or via reflection-based discovery (pending):

```csharp
var yourPlugin = new YourPlugin();
radialCoreManager.GetPluginLoader()?.LoadPlugin(yourPlugin);
```

### Step 3: Implement Providers

See [Extension Points](#-extension-points) below.

---

## ?? Extension Points

### IMenuProvider (v1.0)

Provides dynamic menu entries based on context.

```csharp
public class YourMenuProvider : IMenuProvider
{
    public string ProviderId => "YourMod.YourMenuProvider";
    public int Priority => 100;

    public IEnumerable<RadialMenuEntry> GetMenuEntries(MenuContext context)
    {
        // Return menu entries (can be empty)
        yield return new RadialMenuEntry
        {
            EntryId = "yourmod.action1",
            Label = "$yourmod.action1.label",  // Localized
            Description = "Do something awesome",
            IconId = "icon.awesome",
            Type = EntryType.Action,
            Priority = 100,
            IsVisible = true,
            IsEnabled = true,
            VisibilityConditionId = "condition.always",
            EnabledConditionId = "condition.always",
            PreviewPanelId = "panel.action1"
        };
    }
}
```

### IActionHandler (v1.0)

Executes actions (supports async).

```csharp
public class YourActionHandler : IActionHandler
{
    public string HandlerId => "YourMod.YourActionHandler";

    public bool CanHandle(string actionId) => actionId.StartsWith("yourmod.");

    public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
    {
        try
        {
            if (actionId == "yourmod.action1")
            {
                // Async work
                await Task.Delay(500);
                
                // Do something with context
                return ActionResult.CreateSuccess("Action completed!");
            }

            return ActionResult.CreateFailure("Unknown action");
        }
        catch (Exception ex)
        {
            return ActionResult.CreateFailure($"Error: {ex.Message}", ex);
        }
    }
}
```

### IPanelProvider (v1.0)

Provides panel content for preview/input/modal.

```csharp
public class YourPanelProvider : IPanelProvider
{
    public string ProviderId => "YourMod.YourPanelProvider";

    public PanelContent? GetPanelContent(string panelId, MenuContext context)
    {
        if (panelId == "panel.action1")
        {
            return new PanelContent
            {
                Title = "Action Preview",
                Content = $"Player Gold: {context.PlayerGold}",
                Buttons = new List<PanelButton>
                {
                    new PanelButton { Label = "Confirm", ActionId = "confirm" },
                    new PanelButton { Label = "Cancel", ActionId = "cancel" }
                }
            };
        }

        return null;
    }
}
```

### IContextProvider (v1.0)

Feeds custom data into the ContextHub.

```csharp
public class YourContextProvider : IContextProvider
{
    public string ProviderId => "YourMod.YourContextProvider";

    public void ProvideContext(MenuContext context)
    {
        // Add custom data
        context.CustomData["yourmod.timestamp"] = DateTime.UtcNow.Ticks;
        context.CustomData["yourmod.sessionId"] = Guid.NewGuid().ToString();
    }
}
```

### IConditionEvaluator (v1.0)

Evaluates conditions for visibility/enabled.

```csharp
public class YourConditionEvaluator : IConditionEvaluator
{
    public string EvaluatorId => "YourMod.YourConditionEvaluator";

    public bool EvaluateCondition(string conditionId, MenuContext context)
    {
        return conditionId switch
        {
            "yourmod.hasGold" => context.PlayerGold > 0,
            "yourmod.isRich" => context.PlayerGold >= 10000,
            _ => false
        };
    }
}
```

### ILocalizationProvider (v1.0)

Provides translations for localized strings.

```csharp
public class YourLocalizationProvider : ILocalizationProvider
{
    public string ProviderId => "YourMod.YourLocalizationProvider";

    public string Localize(string key)
    {
        return key switch
        {
            "yourmod.action1.label" => "My Awesome Action",
            "yourmod.action1.desc" => "Does something awesome",
            _ => key
        };
    }
}
```

### IIconProvider (v1.0)

Provides icon IDs to sprite mappings.

```csharp
public class YourIconProvider : IIconProvider
{
    public string ProviderId => "YourMod.YourIconProvider";

    public string? GetIconPath(string iconId)
    {
        return iconId switch
        {
            "icon.awesome" => "path/to/awesome_icon.png",
            "icon.cool" => "path/to/cool_icon.png",
            _ => null
        };
    }
}
```

---

## ?? Available Services

### Capability Resolver

```csharp
public void Initialize(IPluginInitializationContext context)
{
    var localization = context.ResolveCapability<ILocalizationProvider>();
    if (localization != null)
    {
        string text = localization.Localize("my.key");
    }
}
```

### Mod Detection

```csharp
public void Initialize(IPluginInitializationContext context)
{
    if (context.IsModLoaded("ButterLib"))
    {
        // Use ButterLib features
    }
}
```

### Logging

```csharp
public void Initialize(IPluginInitializationContext context)
{
    context.LogInfo("Something happened");
    context.LogWarning("Be careful");
    context.LogError("Oops!", exception);
}
```

---

## ?? Configuration

### Versioning

RadialCore uses Semantic Versioning (Major.Minor.Patch):
- **Major** : Breaking changes (API incompatibility)
- **Minor** : New features (backward compatible)
- **Patch** : Bug fixes

```csharp
// In your PluginManifest
requiredCoreVersion: new Version(1, 0, 0)  // v1.x.x compatible
```

### Circuit Breaker

If a plugin throws more than **5 exceptions**, it is automatically disabled to protect the Core.

```
Exception 1 -> Logged
Exception 2 -> Logged
Exception 3 -> Logged
Exception 4 -> Logged
Exception 5 -> Logged + Warning
Exception 6+ -> Plugin DISABLED
```

### Fail-Safe Design

- ? Plugin exceptions isolated
- ? Other plugins continue working
- ? Core remains stable
- ? Full logging for diagnostics

---

## ?? Debugging

### Structured Logs

Format: `[Timestamp] [Level] [Source] [Plugin:ID] [Phase:Name] Message`

Example:
```
[2025-01-02 02:30:05.123] [INFO] [Plugin] [Plugin:Radial.BasicActions] [Phase:Load] Plugin loaded successfully
[2025-01-02 02:30:05.124] [ERROR] [Plugin] [Plugin:Radial.BasicActions] [Phase:Execute] Exception during action execution
```

### Debug Overlay (TODO)

Press `Ctrl+Shift+D` to display debug overlay with:
- Loaded plugins and their state
- Circuit breaker status
- Context snapshot (player data, game state)
- Performance metrics

### Log Location

```
My Documents\Mount and Blade II Bannerlord\Logs\RadialCore\
```

---

## ? Performance

### Optimization Tips

1. **Avoid Allocations in GetMenuEntries()**
   - Cache expensive computations
   - Reuse lists when possible
   - Use object pooling for frequent allocations

2. **Use Async for Long Operations**
   - If action takes > 100ms, use `async Task`
   - Don't block the game thread

3. **Leverage Conditions**
   - Use `VisibilityConditionId` to avoid unnecessary checks
   - Evaluate expensive conditions once

4. **Monitor Context Refresh**
   - Context snapshots refresh 2x per second
   - Only refresh on significant changes

### Metrics

| Metric | Value | Notes |
|--------|-------|-------|
| **Context Refresh Rate** | 2x/sec | Configurable |
| **Menu Entry Cache** | Automatic | Invalidated on refresh |
| **Max Notifications** | 5 | Queue-based |
| **Notification TTL** | 3s | Default |
| **Circuit Breaker Threshold** | 5 failures | Per plugin |

---

## ?? Roadmap

### v1.0.0 (Current)
- ? Architecture Core + Plugin System
- ? ModPresenceService + CapabilityResolver
- ? Structured logging + Circuit Breaker
- ? BasicActions plugin demo

### v1.1.0 (Next)
- ? UI Gauntlet (RadialMenuVM + Screen)
- ? InputManager + Hotkey config
- ? MCM v5 integration
- ? Complete extension points

### v1.2.0 (Future)
- ? Reflection-based plugin discovery
- ? Debug overlay UI
- ? Context snapshot caching optimization
- ? State machine (Closed, Opening, Open, etc.)

### v2.0.0 (Long-term)
- ? Multiplayer support
- ? Custom theming engine
- ? Plugin marketplace integration
- ? Performance profiling tools

---

## ?? Best Practices

### Naming Conventions

- **PluginId** : `ModName.PluginName` (e.g., `MyMod.DialogueMenu`)
- **ProviderId** : `PluginId.ProviderType` (e.g., `MyMod.DialogueMenu.MenuProvider`)
- **ActionId** : `category.action` (e.g., `dialogue.start`, `inventory.open`)
- **ConditionId** : `category.condition` (e.g., `player.inCombat`, `npc.nearby`)
- **IconId** : `icon.name` (e.g., `icon.inventory`, `icon.map`)

### Error Handling

Always catch exceptions and return clean `ActionResult`:

```csharp
public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
{
    try
    {
        // Action code
        return ActionResult.CreateSuccess("Completed!");
    }
    catch (Exception ex)
    {
        context.LogError($"Action {actionId} failed", ex);
        return ActionResult.CreateFailure($"Error: {ex.Message}", ex);
    }
}
```

### Localization

Prefix labels with `$` for automatic translation:

```csharp
new RadialMenuEntry
{
    Label = "$radial.action.inventory",      // Translated by ILocalizationProvider
    Description = "$radial.action.inventory.desc"
}
```

---

## ?? Contributing

Contributions are welcome! Please follow these guidelines:

1. **Respect contract-first architecture**
2. **All public interfaces must be versioned**
3. **Log all errors with context** (pluginId, phase)
4. **Fail-safe** : Plugin exceptions must not crash Core
5. **In-game testing** before PR

---

## ?? License

MIT License - See LICENSE.md

---

## ?? Support

- **Bug Reports** : Create an issue in the repo
- **Feature Requests** : Open a discussion
- **General Help** : Check documentation in `/docs`

---

## ?? Acknowledgments

- TaleWorlds Entertainment for Mount & Blade II: Bannerlord
- The Bannerlord modding community
- All contributors to the project

---

**Made with ?? for the Bannerlord community**

---

## ?? Documentation

Complete documentation is available in the `docs/` folder:

| Document | Purpose |
|----------|---------|
| **[docs/INDEX.md](./docs/INDEX.md)** | ?? Documentation index & navigation |
| **[docs/QUICK_START.md](./docs/QUICK_START.md)** | ? 5-minute getting started |
| **[docs/CONTRACTS_V1.md](./docs/CONTRACTS_V1.md)** | ?? Complete API reference |
| **[docs/RADIAL_CORE_SPEC.md](./docs/RADIAL_CORE_SPEC.md)** | ?? **Complete specification (V1.0 interfaces, invariants, error handling)** |
| **[docs/PLUGIN_DEVELOPMENT_GUIDE.md](./docs/PLUGIN_DEVELOPMENT_GUIDE.md)** | ?? Step-by-step plugin creation |
| **[docs/INITIALIZATION_PHASES.md](./docs/INITIALIZATION_PHASES.md)** | ?? Detailed initialization docs |
| **[docs/PERFORMANCE_OPTIMIZATION.md](./docs/PERFORMANCE_OPTIMIZATION.md)** | ? Performance tuning guide |
| **[docs/BASICACTIONS_DEMO.md](./docs/BASICACTIONS_DEMO.md)** | ?? Example plugin walkthrough |
| **[docs/PROJECT_SUMMARY.md](./docs/PROJECT_SUMMARY.md)** | ? Project completion status |
| **[docs/FILE_MANIFEST.md](./docs/FILE_MANIFEST.md)** | ?? Complete file structure |

**Start with:** [docs/QUICK_START.md](./docs/QUICK_START.md) for 5-minute setup, or [docs/INDEX.md](./docs/INDEX.md) for navigation.

**For Detailed Specifications:** [docs/RADIAL_CORE_SPEC.md](./docs/RADIAL_CORE_SPEC.md) - Complete V1.0 interface contracts, invariants, error handling, and performance guarantees.
