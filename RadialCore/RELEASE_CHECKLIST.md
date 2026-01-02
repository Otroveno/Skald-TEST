# ?? RadialCore Release Checklist

Use this checklist when preparing a release of RadialCore.

---

## ?? Pre-Release (1 Week Before)

### Code Verification
- [ ] All tests pass
- [ ] No compiler warnings
- [ ] No critical bugs reported
- [ ] Circuit breaker working
- [ ] Exception handling complete
- [ ] Memory leaks checked

### Documentation
- [ ] README.md updated
- [ ] CONTRACTS_V1.md current
- [ ] PLUGIN_DEVELOPMENT_GUIDE.md complete
- [ ] All new features documented
- [ ] CHANGELOG.md created/updated
- [ ] Breaking changes noted (if any)

### Testing
- [ ] Build succeeds on clean checkout
- [ ] In-game testing completed
- [ ] All plugins load correctly
- [ ] BasicActions plugin works
- [ ] Log files generated correctly
- [ ] No crashes observed

---

## ?? Release Day

### Version Update
- [ ] Increment version in RadialCore.csproj
- [ ] Update Version.cs if needed
- [ ] Update SubModule.xml version
- [ ] Update version in docs (README, CONTRACTS_V1, etc.)

### Final Build
- [ ] Clean build: `dotnet clean && dotnet build`
- [ ] No warnings or errors
- [ ] DLL created successfully
- [ ] DLL copied to Modules folder
- [ ] SubModule.xml copied

### Verification
- [ ] Test build with fresh Bannerlord install
- [ ] Verify mod appears in launcher
- [ ] Test all functionality
- [ ] Check logs for errors
- [ ] Verify plugin loading
- [ ] Test with no other mods
- [ ] Test with common mods (ButterLib, MCM, etc.)

### Documentation Final Check
- [ ] No broken links
- [ ] All code examples work
- [ ] Screenshots/diagrams present (if any)
- [ ] Quick start guide tested
- [ ] All docs formats correct

---

## ?? Package & Release

### Create Release Package

```
RadialCore_v1.x.x/
??? RadialCore/
?   ??? bin/Win64_Shipping_Client/
?   ?   ??? RadialCore.dll
?   ??? SubModule.xml
?   ??? (other required files)
??? README.md
??? LICENSE.md
??? docs/
?   ??? QUICK_START.md
?   ??? CONTRACTS_V1.md
?   ??? (all documentation)
??? CHANGELOG.md
```

### Create Changelog

```markdown
# RadialCore v1.x.x - Release Notes

## New Features
- Feature 1
- Feature 2

## Bug Fixes
- Fix 1
- Fix 2

## Performance
- Optimization 1

## Breaking Changes
(If any)

## Credits
Contributors for this release
```

### Git Release

```bash
# Create tag
git tag -a v1.x.x -m "RadialCore v1.x.x Release"

# Push tag
git push origin v1.x.x

# Create GitHub Release from tag
# Copy CHANGELOG to release notes
```

---

## ?? Distribution Channels

### Nexus Mods
- [ ] Create mod page (if first release)
- [ ] Upload mod file
- [ ] Add screenshots
- [ ] Write description
- [ ] Set requirements
- [ ] Test download & install
- [ ] Publish

### GitHub
- [ ] Create release on GitHub
- [ ] Attach release package
- [ ] Add CHANGELOG to notes
- [ ] Mark as latest release (if stable)

### Other Platforms
- [ ] Discord community
- [ ] Bannerlord forums
- [ ] Modding communities

---

## ?? Announcement

### Prepare Announcement

```markdown
# RadialCore v1.x.x Released!

## What's New
- Summary of major changes

## Download
- Nexus Mods link
- GitHub release link

## Installation
- Quick steps from QUICK_START.md

## Known Issues (if any)
- Issue 1: Workaround
- Issue 2: Will be fixed in vX.x.x

## Thanks
- Credits to contributors
```

### Post Announcement
- [ ] Post on Nexus Mods
- [ ] Post on GitHub releases
- [ ] Post on Discord
- [ ] Post on forums (if applicable)
- [ ] Update community channels

---

## ? Post-Release

### Monitoring
- [ ] Monitor for bug reports
- [ ] Check logs in community feedback
- [ ] Watch GitHub issues
- [ ] Respond to user questions
- [ ] Fix critical bugs quickly (v1.x.x-hotfix)

### Feedback Collection
- [ ] Ask for feedback on features
- [ ] Ask for plugin ideas
- [ ] Gather performance reports
- [ ] Note feature requests

### Next Steps
- [ ] Plan v1.x.x (next minor)
- [ ] Plan v2.0.0 (if needed)
- [ ] Document lessons learned
- [ ] Update roadmap

---

## ?? Hotfix Release Process

If critical bug found after release:

1. **Create branch:** `git checkout -b hotfix/vX.X.X`
2. **Fix bug:** Make minimal change
3. **Test:** Verify fix in-game
4. **Version:** Update to vX.X.X-hotfix or vX.X.Z
5. **Release:** Follow quick release process
6. **Announce:** Note it's a hotfix

---

## ?? Release Checklist Summary

### Critical ?
- [ ] Code builds without warnings
- [ ] All plugins load
- [ ] No crashes
- [ ] BasicActions works
- [ ] Documentation current

### Important ?
- [ ] Version numbers updated
- [ ] Changelog created
- [ ] All docs proofread
- [ ] In-game tested
- [ ] Logs clean

### Nice to Have ?
- [ ] Screenshots/GIFs
- [ ] Tutorial video
- [ ] Community feedback incorporated
- [ ] Roadmap updated

---

## ?? Version Numbering

Follow Semantic Versioning (Major.Minor.Patch):

```
v1.0.0    Initial release
v1.0.1    Patch: bug fix
v1.1.0    Minor: new features, backward compatible
v1.1.1    Patch: bug fix
v2.0.0    Major: breaking changes
```

---

## ?? Release Metrics

Track these for each release:

| Metric | Target | Notes |
|--------|--------|-------|
| Build Time | < 30s | From clean |
| Install Size | < 500KB | DLL + docs |
| Init Time | < 100ms | Game startup |
| Memory Usage | < 10MB | Runtime |
| Bug Reports | 0 critical | After 1 week |
| User Satisfaction | > 90% | Community feedback |

---

## ?? Release Success Criteria

Release is successful when:

? Build succeeds without warnings  
? All in-game tests pass  
? Documentation complete  
? No critical bugs found (first week)  
? Users report positive feedback  
? No compatibility issues with common mods  
? Performance meets targets  

---

## ?? Support During Release

Have ready:
- [ ] Support email or contact
- [ ] Common troubleshooting steps
- [ ] Known issues list
- [ ] Installation verification steps
- [ ] Debug guide for issues

---

## ?? Security Checklist

- [ ] No hardcoded credentials
- [ ] No unsafe code in hot paths
- [ ] No external dependencies vulnerabilities
- [ ] File access restricted properly
- [ ] No SQL injection (if applicable)
- [ ] Input validation complete

---

## ? Quality Gates

Release must have:

| Item | Status |
|------|--------|
| Compiler Warnings | 0 |
| Runtime Errors | 0 |
| Security Issues | 0 |
| Critical Bugs | 0 |
| Code Coverage | Tested |
| Documentation | Complete |
| Performance | Target Met |

---

**When all items are checked ?, release is ready!**

---

*Last Updated: 2025-01-02 for RadialCore v1.0.0*

