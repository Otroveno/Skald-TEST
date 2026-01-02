# ?? Contributing to RadialCore

Thank you for your interest in contributing to RadialCore! This guide explains how to contribute effectively.

---

## ?? Table of Contents

1. [Before You Start](#-before-you-start)
2. [Setting Up Development](#-setting-up-development)
3. [Code Guidelines](#-code-guidelines)
4. [Making Changes](#-making-changes)
5. [Testing](#-testing)
6. [Documentation](#-documentation)
7. [Submitting Changes](#-submitting-changes)

---

## ?? Before You Start

### Types of Contributions

| Type | How | Example |
|------|-----|---------|
| **Bug Fix** | Create issue first | Fix exception in ActionPipeline |
| **Feature** | Discuss in issues | Add new extension point |
| **Plugin** | Create separate repository | MyAwesomePlugin for RadialCore |
| **Documentation** | Direct PR | Fix typo in guide |
| **Performance** | Create issue first | Optimize context refresh |

### Project Philosophy

RadialCore follows these principles:

1. **Contract-First** - All public APIs are interfaces
2. **Fail-Safe** - Plugins cannot crash the core
3. **Version-Stable** - Breaking changes require major version bump
4. **Performance-Conscious** - No allocations in hot paths
5. **Well-Documented** - All code should be understandable

---

## ?? Setting Up Development

### Prerequisites

- Visual Studio 2019+ or Visual Studio Code
- .NET Framework 4.7.2 SDK
- Mount & Blade II: Bannerlord v1.3.10
- Git (for version control)

### Clone Repository

```bash
git clone https://github.com/yourusername/RadialCore.git
cd RadialCore
```

### Build

```bash
# Using Visual Studio
Open RadialCore.csproj and build

# Using dotnet CLI
dotnet build
```

### Verify Installation

```bash
# Check if DLL was created and copied
dir "C:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\RadialCore\bin\Win64_Shipping_Client\"
# Should contain: RadialCore.dll
```

---

## ?? Code Guidelines

### Naming Conventions

```csharp
// Public interfaces: IPascalCase
public interface IMenuProvider { }

// Public classes: PascalCase
public class MenuManager { }

// Public methods: PascalCase
public void Initialize(IPluginInitializationContext context) { }

// Private fields: _camelCase
private string _pluginId;

// Constants: UPPER_SNAKE_CASE
private const float REFRESH_INTERVAL = 0.5f;

// Local variables: camelCase
var contextSnapshot = GetSnapshot();
```

### File Organization

```csharp
using System;  // Framework
using System.Collections.Generic;  // Framework
using TaleWorlds.CampaignSystem;  // TaleWorlds
using RadialCore.Contracts;  // Project references

namespace RadialCore.Core
{
    /// <summary>
    /// Brief description.
    /// </summary>
    public class MyClass
    {
        // Fields
        private string _field;
        
        // Properties
        public string Property { get; set; }
        
        // Constructors
        public MyClass() { }
        
        // Methods
        public void Method() { }
    }
}
```

### XML Documentation

All public APIs must have XML comments:

```csharp
/// <summary>
/// Executes an action asynchronously.
/// </summary>
/// <param name="actionId">The ID of the action to execute</param>
/// <param name="context">The current menu context</param>
/// <returns>An ActionResult indicating success or failure</returns>
public async Task<ActionResult> ExecuteActionAsync(string actionId, MenuContext context)
{
    // Implementation
}
```

### Error Handling

Always handle exceptions properly:

```csharp
// ? Good
try
{
    DoSomething();
}
catch (Exception ex)
{
    Logger.Error("Operation", $"Failed to do something: {ex.Message}", ex);
    return false;
}

// ? Bad - Silent exception
try
{
    DoSomething();
}
catch { }
```

### Logging

Log with full context:

```csharp
// ? Good
Logger.Info("PluginLoader", $"Loading plugin {manifest.PluginId}", pluginId: pluginId);

// ? Bad - No context
Logger.Info("Started");
```

---

## ?? Making Changes

### Code Structure

When adding new features:

1. **Create interface** if it's a public API
2. **Create implementation** in appropriate namespace
3. **Add error handling** for all failure cases
4. **Add logging** with context
5. **Update documentation** before submitting

### Example: Adding a New Extension Point

```csharp
// Step 1: Create interface in Contracts/Core/
public interface INewProvider
{
    string ProviderId { get; }
    void DoSomething(MenuContext context);
}

// Step 2: Create implementation in Extension or Core
public class MyNewProvider : INewProvider
{
    public string ProviderId => "MyPlugin.MyProvider";
    
    public void DoSomething(MenuContext context)
    {
        // Implementation
    }
}

// Step 3: Register in plugin
public void Initialize(IPluginInitializationContext context)
{
    context.RegisterNewProvider(new MyNewProvider());
}

// Step 4: Update IPluginInitializationContext
public interface IPluginInitializationContext
{
    void RegisterNewProvider(INewProvider provider);
}

// Step 5: Document in CONTRACTS_V1.md
```

### Versioning Interfaces

When modifying interfaces:

```csharp
// For backward-compatible additions:
// Increment Minor version
// (No change needed to interface name)

// For breaking changes:
// Increment Major version
// Create new interface: IMenuProviderV2
// Keep old interface for deprecation period

public interface IMenuProvider  // v1.0
{
    // Existing API
}

public interface IMenuProviderV2  // v2.0
{
    // New API
}
```

---

## ?? Testing

### In-Game Testing

1. **Build** the project
2. **Enable** RadialCore in launcher
3. **Start** a Sandbox game
4. **Check logs** in `My Documents\Mount and Blade II Bannerlord\Logs\RadialCore\`
5. **Press V** to test menu

### Unit Testing

While not required, consider testing:

```csharp
// Example: Test version compatibility
[TestMethod]
public void TestVersionCompatibility()
{
    var v1 = new Version(1, 0, 0);
    var v1_0_1 = new Version(1, 0, 1);
    
    Assert.IsTrue(v1.IsCompatibleWith(v1_0_1));
}
```

### Circuit Breaker Testing

Test exception handling:

```csharp
// Intentionally throw exceptions
// Verify circuit breaker stops plugin after 5 exceptions
// Check logs for proper error messages
```

---

## ?? Documentation

### Update When:

- [ ] Adding new public interface
- [ ] Adding new extension point
- [ ] Changing API
- [ ] Adding feature
- [ ] Finding a bug and fixing it

### Files to Update:

```
1. Code comments/XML docs
2. Relevant docs/ file
   - CONTRACTS_V1.md (new interfaces)
   - PLUGIN_DEVELOPMENT_GUIDE.md (new features)
   - INITIALIZATION_PHASES.md (initialization changes)
   - PERFORMANCE_OPTIMIZATION.md (perf changes)
3. This file if guidelines change
```

### Documentation Template

```markdown
## [Feature Name]

**Purpose:** Brief description

**Status:** ? Implemented / ? Planned / ? Deprecated

**Versions:** v1.0.0+

**Example:**
\`\`\`csharp
// Code example
\`\`\`

**Notes:** Any additional information
```

---

## ?? Submitting Changes

### Commit Message Format

```
[Type] Brief description

Longer description if needed.

Closes #123
```

**Types:**
- `[Feature]` - New functionality
- `[Fix]` - Bug fix
- `[Docs]` - Documentation only
- `[Refactor]` - Code restructuring
- `[Perf]` - Performance improvement
- `[Test]` - Testing improvements

### Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation

## Testing
- [ ] Tested in-game
- [ ] Logs show no errors
- [ ] No exceptions in circuit breaker

## Checklist
- [ ] Code follows guidelines
- [ ] XML documentation added
- [ ] Logs updated
- [ ] Tests pass (if applicable)
- [ ] Documentation updated
```

### Before Submitting

```
? Code compiles without warnings
? Tested in-game
? Logs show no errors
? Documentation updated
? XML comments added
? No breaking changes (or justified)
? Follows code guidelines
? Circuit breaker tested (if applicable)
```

---

## ?? Development Workflow

### Example: Adding a New Condition Type

```
1. Create issue: "Feature: Add new condition XYZ"
2. Get approval
3. Create feature branch: git checkout -b feature/new-condition
4. Implement:
   a. Add IConditionEvaluator implementation
   b. Add to BasicActionsPlugin
   c. Add XML comments
   d. Add logging
   e. Handle exceptions
5. Test:
   a. Build: dotnet build
   b. Verify logs
   c. Test in-game
6. Document:
   a. Update CONTRACTS_V1.md
   b. Update PLUGIN_DEVELOPMENT_GUIDE.md
   c. Add code comments
7. Commit: [Feature] Add XYZ condition type
8. Push: git push origin feature/new-condition
9. Create PR with template
10. Address review comments
11. Merge when approved
```

---

## ?? Reporting Bugs

### Bug Report Template

```markdown
## Description
Brief description of bug

## Steps to Reproduce
1. Step 1
2. Step 2
3. Step 3

## Expected Behavior
What should happen

## Actual Behavior
What actually happens

## Logs
[Attach log file or relevant snippet]

## Environment
- RadialCore version: v1.0.0
- Bannerlord version: v1.3.10
- Other mods: [List]
```

---

## ?? Tips for Success

### Before Coding

1. **Read** relevant documentation
2. **Check** existing issues (might already be fixed)
3. **Discuss** major changes in an issue first
4. **Ask** if unsure about approach

### While Coding

1. **Test frequently** (build after each change)
2. **Check logs** for errors/warnings
3. **Document** as you go
4. **Follow** existing patterns
5. **Keep commits** logical and small

### Before Submitting

1. **Review** your own changes
2. **Test in-game** thoroughly
3. **Update** all documentation
4. **Check** for compiler warnings
5. **Run final build**

---

## ?? Code Review Checklist

When reviewing others' code:

- [ ] Follows naming conventions
- [ ] Has XML documentation
- [ ] Includes error handling
- [ ] Has logging
- [ ] No breaking changes (justified if any)
- [ ] Tested in-game
- [ ] Documentation updated
- [ ] No compiler warnings
- [ ] Follows fail-safe principles
- [ ] Performance acceptable

---

## ?? Learning Resources

### For RadialCore Contributors

1. **[INITIALIZATION_PHASES.md](./docs/INITIALIZATION_PHASES.md)** - How system initializes
2. **[PLUGIN_DEVELOPMENT_GUIDE.md](./docs/PLUGIN_DEVELOPMENT_GUIDE.md)** - Pattern examples
3. **[CONTRACTS_V1.md](./docs/CONTRACTS_V1.md)** - API reference
4. **[PERFORMANCE_OPTIMIZATION.md](./docs/PERFORMANCE_OPTIMIZATION.md)** - Perf guidelines

### For Bannerlord Modding

1. Bannerlord modding community documentation
2. TaleWorlds Forums
3. Example mods (ButterLib, UIExtenderEx, etc.)

---

## ? What We Look For

### ? Great Contributions

- ? Solve real problems
- ? Follow guidelines
- ? Well-documented
- ? Thoroughly tested
- ? Clear commit messages
- ? Responsive to feedback

### ? Avoid

- ? Scope creep (too many changes)
- ? Breaking changes without discussion
- ? Incomplete documentation
- ? Silent exceptions
- ? Performance regressions
- ? Unrelated changes

---

## ?? Recognition

Contributors are recognized in:
- Commit history
- Project README
- Release notes
- Community acknowledgments

---

## ?? Questions?

- **How to contribute:** This file
- **Code guidelines:** "Code Guidelines" section above
- **API reference:** [CONTRACTS_V1.md](./docs/CONTRACTS_V1.md)
- **Architecture:** [INITIALIZATION_PHASES.md](./docs/INITIALIZATION_PHASES.md)

---

## ?? License

By contributing to RadialCore, you agree that your contributions are licensed under the MIT License.

---

**Thank you for contributing to RadialCore! ??**

