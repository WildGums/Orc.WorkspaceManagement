# Orc.WorkspaceManagement

Orc.WorkspaceManagement is a library for WildGums open-source that makes it easy to manage workspaces in .NET applications. It provides a workspace manager, serialization support, and XAML integration for saving and restoring application state.

---

## Critical Rules (Read First)

These rules are **non-negotiable**. Violating them causes broken builds, crashes, or downstream breakage.

### 1. Never Edit Generated Files

Files matching `*.generated.cs`, `*.generated.xaml` are auto-generated.

- **NEVER** manually edit these files

### 2. ABI / API Stability

This project maintains stable ABI / API. Breaking changes break downstream apps.

| Allowed | Never |
|---------|-------|
| Add new overloads | Modify existing signatures |
| Add new methods | Remove public APIs |
| Add new classes | Change return types |

### 3. Tests Are Mandatory

**Building alone is NOT sufficient.** Run tests before claiming completion (see [Commands](#commands)).

### 4. Branch Protection (COMPLIANCE REQUIRED)

**Direct commits to protected branches are a policy violation.**

| Repository | Protected Branches |
|------------|-------------------|
| Orc.WorkspaceManagement | `master` |
| Orc.WorkspaceManagement | `develop` |

**Required workflow:**

1. **Create a feature branch FIRST** — Use naming convention: `feature/issue-NNNN-description`
2. **Make all commits on the feature branch** — Never commit directly to protected branches
3. **Submit a Pull Request** — Changes must be reviewed by a human before merging

```bash
# CORRECT — Always create a feature branch first
git checkout -b feature/issue-1234-fix-description

# NEVER DO THIS — Policy violation
git checkout develop && git commit  # FORBIDDEN

# NEVER DO THIS — Policy violation
git checkout master && git commit  # FORBIDDEN
```

The repository has protected branches that must be respected.

---

## Commands

Single source of truth for all commands:

| Task | Command |
|------|---------|
| **Build** | `dotnet cake --target=build` |
| **Test** | `dotnet cake --target=test` |
| **Build and test** | `dotnet cake --target=buildandtest` |

---

## Architecture & Directories

### Solution Overview

```
Orc.WorkspaceManagement       => Core workspace management library
Orc.WorkspaceManagement.Xaml  => WPF/XAML integration (ViewModels, Views)
```

### Directory Guide

| Directory | Editable? | Notes |
|-----------|-----------|-------|
| `*.generated.cs` | No | Leave as-is |
| `*.generated.xaml` | No | Leave as-is |
| `deployment` | No | Deployment / build scripts |
| `src/Orc.WorkspaceManagement/` | Yes | Core workspace management code |
| `src/Orc.WorkspaceManagement.Xaml/` | Yes | WPF/XAML-specific code |
| `src/Orc.WorkspaceManagement.Tests/` | Yes | Tests |
| `src/Orc.WorkspaceManagement.Example/` | Yes | Example application |

---

## Writing Code

### Anti-Patterns (Never Do This)

| Anti-Pattern | Why |
|-------------|-----|
| Modifying method signatures | ABI breaking |
| Manual edits to `*.generated.cs`, `*.generated.xaml` | Overwritten on regenerate |
| Using default parameters in public APIs | ABI breaking |
| **Skipping failing tests** | **Unacceptable — tests must pass** |

---

## Testing & Debugging

### Running Tests

```bash
dotnet cake --target=test
```

### Tests MUST Pass

> **NON-NEGOTIABLE:** Tests must PASS before claiming completion.
>
> - Do NOT skip failing tests
> - Do NOT claim completion if tests fail
> - Do NOT use `SkipException` to work around failures

### Writing Tests

1. Use NUnit to write tests
2. Create a Facts class for a feature
3. Combine Pascal / Snake case for test methods (e.g. `Feature_Does_Work`)

```csharp
[Test]
public void Feature_Does_Work()
{
    var result = 47 - 5;

    Assert.That(result, Is.EqualTo(42));
}
```

**Philosophy:** Tests FAIL when wrong, never skip (except missing hardware).

### Public API Tests

This project uses snapshot-based public API tests (`PublicApiFacts`). If you add or change public APIs:

1. Run the tests once to see them fail with the diff.
2. Update the `.verified.txt` snapshots to match the new API surface:
   - `PublicApiFacts.Orc_WorkspaceManagement_HasNoBreakingChanges_Async.verified.txt`
   - `PublicApiFacts.Orc_WorkspaceManagement_Xaml_HasNoBreakingChanges_Async.verified.txt`
3. Commit the updated snapshots together with your code changes.

### Debugging Methodology

1. **Establish baseline** — What's the known-good state?
2. **One change at a time** — Verify each change before proceeding
3. **Track changes in a table** — Log what you changed and the result
4. **Platform differences are signals** — If X works and Y fails, the difference IS the answer
5. **Revert if worse** — Don't pile fixes on top of failures

---

## Further Reading

| Topic | Document |
|-------|----------|
| Contributing guidelines | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Documentation portal | http://opensource.wildgums.com |
