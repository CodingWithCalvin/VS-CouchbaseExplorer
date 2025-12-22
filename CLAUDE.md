# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Critical Rules

**These rules override all other instructions:**

1. **NEVER commit directly to main** - Always create a feature branch and submit a pull request
2. **Conventional commits** - Format: `type(scope): description`
3. **GitHub Issues for TODOs** - Use `gh` CLI to manage issues, no local TODO files. Use conventional commit format for issue titles
4. **Pull Request titles** - Use conventional commit format (same as commits)
5. **Branch naming** - Use format: `type/scope/short-description` (e.g., `feat/ui/settings-dialog`)
6. **Working an issue** - Always create a new branch from an updated main branch
7. **Check branch status before pushing** - Verify the remote tracking branch still exists. If a PR was merged/deleted, create a new branch from main instead
8. **WPF for all UI** - All UI must be implemented using WPF (XAML/C#). No web-based technologies (HTML, JavaScript, WebView)

---

### GitHub CLI Commands

```bash
gh issue list                    # List open issues
gh issue view <number>           # View details
gh issue create --title "type(scope): description" --body "..."
gh issue close <number>
```

### Conventional Commit Types

| Type | Description |
|------|-------------|
| `feat` | New feature |
| `fix` | Bug fix |
| `docs` | Documentation only |
| `refactor` | Code change that neither fixes a bug nor adds a feature |
| `test` | Adding or updating tests |
| `chore` | Maintenance tasks |

---

## Project Overview

VS-CouchbaseExplorer is a Visual Studio 2022 extension for browsing and querying Couchbase Server and Capella databases. The extension is currently in preview/beta status.

## Build Commands

```bash
# Restore NuGet packages
nuget restore src/CodingWithCalvin.CouchbaseExplorer.sln

# Build (Release, x64)
msbuild src/CodingWithCalvin.CouchbaseExplorer/CodingWithCalvin.CouchbaseExplorer.csproj /p:configuration=Release /p:platform=x64 /p:DeployExtension=False
```

The `/p:DeployExtension=False` flag prevents auto-deployment during CI builds. Remove it for local development to auto-deploy to the VS experimental instance.

## Architecture

### VS Extension Pattern

The extension follows the standard Visual Studio SDK pattern:

1. **CouchbaseExplorerPackage** (AsyncPackage) - Entry point that registers with VS and initializes the extension
2. **CouchbaseExplorerWindowCommand** - Command handler that responds to menu clicks and shows the tool window
3. **CouchbaseExplorerWindow** (ToolWindowPane) - Dockable tool window container
4. **CouchbaseExplorerWindowControl** (WPF UserControl) - XAML-based UI with TreeView for server/bucket hierarchy

### Key Files

- `VSCommandTable.vsct` - Defines menu commands, groups, and placement in VS menus
- `source.extension.vsixmanifest` - VSIX package metadata (version, dependencies, assets)
- `CouchbaseExplorerPackage.cs` - Package initialization and service registration

### Data Layer

Uses **CouchbaseNetClient 3.5.2** for cluster connections, authentication, and N1QL queries.

## Technology Stack

- .NET Framework 4.8
- Visual Studio SDK 17.9 (VS 2022)
- WPF for UI
- MSBuild + NuGet for builds
- VSIX packaging format

## CI/CD

GitHub Actions workflows in `.github/workflows/`:
- `release_build_and_deploy.yml` - Builds on PR and main branch push, creates VSIX artifact
- `publish.yml` - Manual trigger to publish to VS Marketplace

## Development Prerequisites

- Visual Studio 2022 with "Visual Studio extension development" workload
- Extensibility Essentials 2022 extension (for template support)
- A Couchbase Server or Capella instance for testing
