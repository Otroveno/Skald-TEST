# ?? RadialCore Quick Start (5 Minutes)

**TL;DR:** Install the mod, press V, enjoy!

---

## ?? 30 Second Version

1. **Build** ? `dotnet build` or Visual Studio
2. **Enable** ? Add RadialCore to Bannerlord mods
3. **Play** ? Start Sandbox game
4. **Press V** ? Menu opens!

---

## ?? 5 Minute Detailed Version

### Step 1: Build (1 minute)

```bash
# Clone or download RadialCore
cd RadialCore

# Build
dotnet build
# OR use Visual Studio: Build > Build Solution
```

**Result:** `RadialCore.dll` created and copied to Modules folder

### Step 2: Enable in Launcher (1 minute)

1. Open **Bannerlord Launcher**
2. Click **Mods**
3. Find **RadialCore** in the list
4. **Check** the checkbox to enable
5. Make sure it's **above** other mod-dependent mods

### Step 3: Start Game (2 minutes)

1. Click **Play** in launcher
2. Select **Sandbox** (or any campaign)
3. Start a new game
4. Wait for game to load

### Step 4: Try the Menu (1 minute)

1. Once in-game, press **V** (default hotkey)
2. You should see a radial menu (stub UI - see logs for now)
3. Check logs for verification:
   ```
   My Documents\Mount and Blade II Bannerlord\Logs\RadialCore\
   ```

---

## ? Verification Checklist

### Installation Works If:
- [ ] Build succeeds without errors
- [ ] Mod appears in Bannerlord launcher
- [ ] Game launches with RadialCore enabled
- [ ] No crash on startup

### Plugin Loaded If:
- [ ] Logs show initialization complete
- [ ] Logs show "Radial.BasicActions" loaded
- [ ] No "circuit breaker" warnings

### Menu Works If:
- [ ] Pressing V opens menu (check logs)
- [ ] Menu entries are collected (check logs)
- [ ] Can execute actions (check logs/notifications)

---

## ?? Check the Logs

**Location:**
```
My Documents\Mount and Blade II Bannerlord\Logs\RadialCore\RadialCore_[date].log
```

**Look for:**
```
[INFO] RadialCore v1.0.0 loading...
[INFO] ModPresenceService initialized
[INFO] Loading plugin: Basic Actions v1.0.0
[INFO] RadialCore initialized successfully!
```

---

## ?? Configuration (Optional)

Edit `docs/CONFIG.md` (or in-game MCM when available) to customize:
- **Hotkey** - Default: V
- **Mode** - Hold or Toggle
- **Debug Overlay** - Show/hide diagnostics

*(For v1.0.0, configuration is manual. MCM integration coming in v1.1)*

---

## ?? Next Steps

### Just Using the Mod?
? Explore the menu, click entries, enjoy!

### Want to Create a Plugin?
? Read [PLUGIN_DEVELOPMENT_GUIDE.md](./docs/PLUGIN_DEVELOPMENT_GUIDE.md)

### Want Full Documentation?
? Read [README.md](./README.md)

### Something Broken?
? Check logs, see [Troubleshooting](#-troubleshooting) below

---

## ?? Troubleshooting

### Build Fails

**Check:**
1. Do you have .NET Framework 4.7.2 SDK?
2. Are TaleWorlds paths correct in .csproj?
3. Try: `dotnet clean && dotnet build`

**Fix:**
```xml
<!-- In RadialCore.csproj, update BannerlordGamePath: -->
<BannerlordGamePath>C:\SteamLibrary\steamapps\common\Mount &amp; Blade II Bannerlord</BannerlordGamePath>
```

---

### Mod Not Appearing in Launcher

**Check:**
1. Is `SubModule.xml` in the correct location?
2. Is `RadialCore.dll` in Modules\RadialCore\bin\Win64_Shipping_Client\?

**Fix:**
Build again to auto-copy files to correct location.

---

### Game Crashes on Startup

**Check logs:**
1. Look for errors in `RadialCore_[date].log`
2. Plugin causing crash?
3. Missing dependency?

**Most common:**
```
[ERROR] RadialCore initialization failed
```

**Solution:**
- Check logs for specific error
- Disable other mods to isolate issue
- Report issue with log file

---

### Menu Doesn't Appear

**This is expected in v1.0.0** (UI Gauntlet pending for v1.1)

**Check:**
1. Press V (default hotkey)
2. Check logs for:
   ```
   [INFO] MenuManager collected X entries
   ```
3. Entries should be in logs, not yet visible in UI

**For now:**
- Functionality is complete
- UI visualization coming soon

---

## ?? What's Included

### Built-in Plugin
**BasicActions** - Demonstrates all features:
- 5 menu entries (Inventory, Map, Quests, Talk, More)
- Async action execution
- Notifications
- Panel content
- Conditions

### Core Systems
- ? Plugin loader
- ? Context system
- ? Event bus
- ? Action pipeline
- ? Panel system
- ? Notifications
- ? Diagnostics

### Not Yet
- ? UI Gauntlet screens (v1.1)
- ? MCM configuration (v1.1)
- ? Real input polling (v1.1)

---

## ?? Tips

### Check Your Work
```bash
# After building, verify files exist:
dir "C:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\RadialCore\bin\Win64_Shipping_Client\"
# Should see: RadialCore.dll

dir "C:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\RadialCore\"
# Should see: SubModule.xml
```

### Monitor Logs in Real-Time
```bash
# PowerShell: Follow logs
Get-Content (Get-ChildItem "My Documents\Mount and Blade II Bannerlord\Logs\RadialCore\" | Sort-Object LastWriteTime -Desc | Select-Object -First 1).FullName -Tail 50 -Wait
```

### Disable Other Mods to Test
If game crashes, disable other mods to isolate RadialCore.

---

## ?? Quick Documentation Links

| Need | Link |
|------|------|
| **Full Documentation** | [README.md](./README.md) |
| **Create a Plugin** | [PLUGIN_DEVELOPMENT_GUIDE.md](./docs/PLUGIN_DEVELOPMENT_GUIDE.md) |
| **API Reference** | [CONTRACTS_V1.md](./docs/CONTRACTS_V1.md) |
| **All Docs** | [INDEX.md](./docs/INDEX.md) |

---

## ? That's It!

You're ready to use RadialCore. Check the logs, explore the system, and enjoy!

---

**Questions?** ? Check [docs/INDEX.md](./docs/INDEX.md) for full documentation

**Found a bug?** ? Check logs and report with error message

**Want to contribute?** ? Read [PLUGIN_DEVELOPMENT_GUIDE.md](./docs/PLUGIN_DEVELOPMENT_GUIDE.md)

---

Made with ?? for the Bannerlord community

