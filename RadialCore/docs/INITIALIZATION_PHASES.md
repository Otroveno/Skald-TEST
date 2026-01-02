# ?? RadialCore Initialization Phases

This document describes the 9-phase initialization process of RadialCore and how plugins are loaded and initialized.

---

## ?? Overview

RadialCore initializes in **9 phases** to ensure:
- Dependencies are loaded in correct order
- Services are available when plugins need them
- Failures are gracefully handled with circuit breaker
- Full diagnostic logging for troubleshooting

---

## ?? Phase Sequence

### Phase 1: Core Services

**Responsibility:** Initialize basic infrastructure services.

```csharp
_modPresenceService = new ModPresenceService();
_modPresenceService.Initialize();
// Scans loaded modules, populates cache

_capabilityResolver = new CapabilityResolver();
// Service locator for optional capabilities
```

**Log Output:**
```
[INFO] [RadialCoreManager] === Initialization Phase 1: Core Services ===
[INFO] [RadialCoreManager] ModPresenceService initialized
[INFO] [RadialCoreManager] CapabilityResolver initialized
```

**What's Available:**
- Module detection system ready
- Capability service ready (empty initially)

---

### Phase 2: Plugin System

**Responsibility:** Initialize plugin loader and load built-in plugins.

```csharp
_pluginLoader = new PluginLoader(_modPresenceService, _capabilityResolver);
_pluginLoader.Initialize();

// Load built-in plugins
LoadBuiltInPlugins();  // BasicActionsPlugin

// Discover external plugins (TODO)
_pluginLoader.DiscoverAndLoadPlugins();
```

**Log Output:**
```
[INFO] [RadialCoreManager] === Initialization Phase 2: Plugin System ===
[INFO] [PluginLoader] [Plugin:Radial.BasicActions] Loading plugin: Basic Actions v1.0.0
[INFO] [Plugin] [Plugin:Radial.BasicActions] Initialized MenuProvider
[INFO] [Plugin] [Plugin:Radial.BasicActions] Initialized ActionHandler
[INFO] [PluginLoader] Plugin Radial.BasicActions loaded successfully
```

**What Happens:**
1. BasicActionsPlugin.Initialize() is called
2. All 5 providers are registered
3. Circuit breaker is set up (5 exception threshold)
4. Plugin is marked as initialized

**What's Available:**
- Menu providers
- Action handlers
- Panel providers
- Context providers
- Condition evaluators

---

### Phase 3: Context System

**Responsibility:** Initialize the ContextHub that maintains a snapshot of game state.

```csharp
_contextHub = new ContextHub(_pluginLoader, _capabilityResolver);
_contextHub.Initialize();
```

**Log Output:**
```
[INFO] [RadialCoreManager] === Initialization Phase 3: Context System ===
[INFO] [ContextHub] ContextHub initialized
[INFO] [ContextHub] Registered 3 context providers
```

**What Happens:**
1. ContextHub queries all IContextProvider implementations
2. Current game state is snapshotted:
   - Player info (Gold, Level, Health, etc.)
   - Game state (OnMap, InMission, InConversation)
   - NPC proximity (stub)
   - Custom data from plugins

**What's Available:**
- MenuContext snapshots
- Game state queries
- Player information

---

### Phase 4: Event Bus

**Responsibility:** Ensure EventBus singleton is ready.

```csharp
// EventBus is singleton, no explicit initialization needed
// But we verify it's ready
Logger.Info("RadialCoreManager", "EventBus ready (singleton)", phase: "Init");
```

**Log Output:**
```
[INFO] [RadialCoreManager] === Initialization Phase 4: Event Bus ===
[INFO] [RadialCoreManager] EventBus ready (singleton)
```

**What's Available:**
- 6 predefined events:
  - `RadialMenuOpenedEvent`
  - `RadialMenuClosedEvent`
  - `ActionExecutedEvent`
  - `ActionFailedEvent`
  - `PanelShownEvent`
  - `PanelHiddenEvent`

**Event Subscription:**
```csharp
EventBus.Instance.Subscribe<RadialMenuOpenedEvent>(e =>
{
    // Menu was opened
});
```

---

### Phase 5: Panel System

**Responsibility:** Initialize panel hosting system for UI content.

```csharp
_panelHost = new PanelHost(_pluginLoader);
```

**Log Output:**
```
[INFO] [RadialCoreManager] === Initialization Phase 5: UI Systems ===
[INFO] [PanelHost] PanelHost initialized
```

**What Happens:**
1. Panel slots are created:
   - Right panel (preview)
   - Bottom panel (details)
   - Modal panel (confirmation)
   - TextInput panel (input)

**What's Available:**
- Panel display/hide functionality
- Provider resolution for panel content

---

### Phase 6: Notification Service

**Responsibility:** Initialize notification queue and display system.

```csharp
_notificationService = new NotificationService();
```

**Log Output:**
```
[INFO] [NotificationService] NotificationService initialized
```

**What Happens:**
1. Notification queue is created
2. TTL system is ready (default 3 seconds)
3. Max 5 simultaneous notifications

**What's Available:**
- Notification display:
  - Info (blue)
  - Success (green)
  - Warning (yellow)
  - Error (red)

---

### Phase 7: Action Pipeline

**Responsibility:** Initialize the action execution pipeline.

```csharp
_actionPipeline = new ActionPipeline(_pluginLoader, _panelHost, _notificationService);
```

**Log Output:**
```
[INFO] [ActionPipeline] ActionPipeline initialized
```

**What Happens:**
1. Pipeline stages are set up:
   - Stage 1: Precheck (find handler, validate)
   - Stage 2: Confirm (show confirmation if needed)
   - Stage 3: Execute (run handler async)
   - Stage 4: Post (notifications + events)

**What's Available:**
- Async action execution
- Automatic notification display on completion
- Event publishing

---

### Phase 8: Menu Manager

**Responsibility:** Initialize the UI orchestrator.

```csharp
_menuManager = new RadialCore.UI.MenuManager(_pluginLoader, _contextHub, _actionPipeline, _panelHost);
```

**Log Output:**
```
[INFO] [MenuManager] MenuManager initialized
```

**What Happens:**
1. Menu state machine is ready
2. Entry collection system is ready
3. Condition evaluation system is ready

**What's Available:**
- Menu open/close
- Entry collection from all MenuProviders
- Condition evaluation (visibility/enabled)

---

### Phase 9: Input System

**Responsibility:** Initialize hotkey polling and input handling.

```csharp
_inputManager = new InputManager(_contextHub);
SubscribeToInputEvents();
```

**Log Output:**
```
[INFO] [RadialCoreManager] === Initialization Phase 8: Input System ===
[INFO] [InputManager] InputManager initialized
[INFO] [RadialCoreManager] Subscribed to input events
```

**What Happens:**
1. Input manager starts polling
2. Hotkey (default: V) is monitored
3. MenuOpenedEvent is published when pressed

**What's Available:**
- Hotkey detection
- Menu open/close triggering

---

### Phase 10: Diagnostics

**Responsibility:** Initialize debug systems and output initial state.

```csharp
_debugOverlay = new DebugOverlay(_pluginLoader);
_debugOverlay.DumpFullState();
```

**Log Output:**
```
[INFO] [RadialCoreManager] === Initialization Phase 9: Diagnostics ===
[INFO] [DebugOverlay] DebugOverlay initialized
[INFO] [DebugOverlay] === FULL STATE DUMP ===
[INFO] [DebugOverlay] Total Plugins: 1
[INFO] [DebugOverlay] Plugin: Radial.BasicActions
[INFO] [DebugOverlay]   Display Name: Basic Actions
[INFO] [DebugOverlay]   Version: 1.0.0
[INFO] [DebugOverlay] Registered Providers:
[INFO] [DebugOverlay]   MenuProviders: 1
[INFO] [DebugOverlay]   ActionHandlers: 1
[INFO] [DebugOverlay]   PanelProviders: 1
[INFO] [DebugOverlay]   ContextProviders: 1
[INFO] [DebugOverlay]   ConditionEvaluators: 1
```

**What's Available:**
- Full system state visibility
- Plugin validation
- Circuit breaker status

---

### Final: Success Log

```
[INFO] [RadialCoreManager] RadialCore initialized successfully!
```

---

## ?? Plugin Initialization Flow

When a plugin is loaded, these steps occur:

### 1. Manifest Validation
```csharp
// Check Core version compatibility
if (!_coreVersion.IsCompatibleWith(manifest.RequiredCoreVersion))
{
    // Fail: incompatible version
}

// Check mod dependencies
foreach (var dep in manifest.ModDependencies)
{
    if (required && !isLoaded)
    {
        // Fail: required mod not loaded
    }
}
```

### 2. Circuit Breaker Setup
```csharp
var circuitBreaker = new CircuitBreaker(pluginId);
_circuitBreakers[pluginId] = circuitBreaker;
```

### 3. Context Creation
```csharp
var context = new PluginInitializationContextImpl(
    pluginId,
    _modPresenceService,
    _capabilityResolver);
```

### 4. Plugin.Initialize() Call
```csharp
circuitBreaker.Execute(() =>
{
    plugin.Initialize(context);
    return true;
}, "Initialize");
```

**Inside Initialize(), plugin can:**
- Register providers
- Check mod presence
- Resolve capabilities
- Log messages

### 5. Provider Registration
```csharp
context.RegisterMenuProvider(new MyMenuProvider());
context.RegisterActionHandler(new MyActionHandler());
// ... etc
```

### 6. Completion
```csharp
_loadedPlugins.Add(new LoadedPlugin
{
    Plugin = plugin,
    Manifest = manifest,
    Initialized = true,
    Context = context
});
```

---

## ?? Error Handling During Initialization

### Circuit Breaker Protection

If Initialize() throws an exception:

```
Exception 1 ? Logged, circuit opens to "half-open"
Exception 2 ? Logged, circuit opens further
Exception 3-5 ? Each logged
Exception 6 ? PLUGIN DISABLED
```

**Log Output:**
```
[ERROR] [PluginLoader] Exception during plugin initialization
[ERROR] [CircuitBreaker] [Plugin:MyPlugin] Circuit breaker opened (threshold: 5)
[WARNING] [PluginLoader] Plugin MyPlugin initialization failed (circuit breaker tripped)
```

### Phase Failure Handling

If a phase fails, Core **attempts recovery**:

```csharp
try
{
    // Phase X
}
catch (Exception ex)
{
    Logger.Error("RadialCoreManager", "Critical error during initialization", ex, phase: "Init");
    throw;  // Re-throw to prevent partial initialization
}
```

If initialization fails, the mod is **not available** but game continues.

---

## ?? Initialization Timeline

```
0ms    ?? SubModule.OnGameStart()
       ?
5ms    ?? RadialCoreManager.Initialize()
       ?
       ?? Phase 1: Core Services (5ms)
       ?  ?? ModPresenceService + CapabilityResolver
       ?
       ?? Phase 2: Plugin System (20ms)
       ?  ?? PluginLoader + BasicActionsPlugin.Initialize()
       ?     ?? 5 providers registered
       ?
       ?? Phase 3: Context System (10ms)
       ?  ?? ContextHub + 3 context providers
       ?
       ?? Phase 4: Event Bus (1ms)
       ?  ?? Singleton ready
       ?
       ?? Phase 5: Panel System (2ms)
       ?  ?? PanelHost + 4 slots
       ?
       ?? Phase 6: Notification Service (1ms)
       ?  ?? Queue + TTL
       ?
       ?? Phase 7: Action Pipeline (2ms)
       ?  ?? 4 stages ready
       ?
       ?? Phase 8: Menu Manager (3ms)
       ?  ?? Menu orchestration ready
       ?
       ?? Phase 9: Input System (2ms)
       ?  ?? Hotkey polling
       ?
       ?? Phase 10: Diagnostics (5ms)
       ?  ?? Full state dump
       ?
65ms   ?? Initialization complete!

Total: ~65-70ms for full initialization
(negligible on game startup)
```

---

## ?? Next Steps

After initialization:

1. **On Each Tick:**
   - ContextHub refreshes state (2x/sec)
   - InputManager polls hotkey
   - PluginLoader calls OnTick() on plugins

2. **On Menu Open:**
   - MenuManager collects entries from all MenuProviders
   - Conditions are evaluated
   - Context snapshot is injected

3. **On Action Execute:**
   - ActionPipeline executes 4 phases
   - ActionHandler is called
   - Notifications are shown

---

## ?? Checklist for Custom Initialization

If you're adding new systems to RadialCore:

- [ ] Add new phase in RadialCoreManager.Initialize()
- [ ] Log phase begin/end
- [ ] Handle exceptions with circuit breaker if applicable
- [ ] Register shutdown cleanup in RadialCoreManager.Shutdown()
- [ ] Update documentation in this file
- [ ] Test initialization in game

---

## ?? Related Documentation

- [README.md](../README.md) - Main documentation
- [CONTRACTS_V1.md](./CONTRACTS_V1.md) - API contracts
- [BASICACTIONS_DEMO.md](./BASICACTIONS_DEMO.md) - Example plugin

