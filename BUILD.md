# Build Release Documentation

This document explains how the automated release build system works for the Commander Keen Godot project.

## Overview

The project includes a GitHub Actions workflow to automatically build the game for Windows, Linux, and macOS platforms using Docker when releases are created.

**Version Logic:**
- **CI Environment**: If `VERSION_TAG` or `GITHUB_REF` is provided, uses that version and skips git commands
- **Local Development**: If no CI version variables are present, runs `git describe` to get current version

## Workflow

### Docker-based Release Workflow (`build-release.yml`)

**Trigger**: 
- When a git tag matching `v*.*.*` is pushed
- When a GitHub release is created
- Manual trigger via workflow dispatch

**What it does**:
- Builds a custom Docker image with Godot 4.4.1 and .NET 8.0 from official sources
- Uses the secure custom image instead of third-party Docker images
- Restores .NET dependencies in the container
- Imports Godot project assets
- Builds the game for all three platforms using the `docker-build.sh` script
- Creates release packages
- Uploads artifacts to GitHub

## Export Presets

The project includes export presets for:
- **Windows Desktop**: Builds as `commander-keen-windows.exe`
- **Linux/X11**: Builds as `commander-keen-linux.x86_64`
- **macOS**: Builds as `commander-keen-macos.zip`

## Manual Build

### Using Docker (Recommended)

The project includes a complete Docker setup for consistent builds:

1. Build the Docker image:
   ```bash
   docker build -t commander-keen-builder .
   ```

2. Run the build:
   
   **On Windows (PowerShell):**
   ```powershell
   docker run --rm -v ${PWD}:/workspace -w /workspace commander-keen-builder
   ```
   
   **On Linux/macOS:**
   ```bash
   docker run --rm -v $(pwd):/workspace -w /workspace commander-keen-builder
   ```

3. To test with a specific version tag:
   ```powershell
   # Windows PowerShell
   docker run --rm -v ${PWD}:/workspace -w /workspace -e VERSION_TAG=1.2.3 commander-keen-builder
   ```
   ```bash
   # Linux/macOS
   docker run --rm -v $(pwd):/workspace -w /workspace -e VERSION_TAG=1.2.3 commander-keen-builder
   ```

This will create builds in the `artifact/` directory.

### Local Build (Alternative)

To build locally without Docker:

1. Install Godot 4.4.1 and .NET 8.0 SDK
2. Run `dotnet restore` to restore dependencies
3. Open the project in Godot or use headless mode:
   ```bash
   mkdir -p artifact
   xvfb-run -a godot --headless --import
   godot --headless --export-release "WindowsDesktop" artifact/commander-keen-windows.exe
   godot --headless --export-release "Linux/X11" artifact/commander-keen-linux.x86_64
   godot --headless --export-release "macOS" artifact/commander-keen-macos.zip
   ```

## Creating a Release

1. Create a git tag: `git tag v1.0.0`
2. Push the tag: `git push origin v1.0.0`
3. The workflow will automatically build and create a GitHub release with artifacts

Alternatively, create a release through the GitHub web interface and the workflow will build the artifacts.

## Technical Implementation

- Uses a custom Docker image built from the official Microsoft .NET SDK
- Downloads Godot 4.4.1 directly from official godotengine releases  
- Uses Godot headless mode with Xvfb for display in the container
- Restores .NET dependencies with `dotnet restore`
- Imports project assets before building with proper error handling
- Cross-platform builds from Ubuntu GitHub runners with Docker
- Proper error handling and timeout management

## Version Handling

The build system automatically extracts version information from Git tags and embeds it in the compiled game:

**Version Sources (in priority order):**
1. `VERSION_TAG` environment variable (manual override)
2. `GITHUB_REF` from CI/CD (release tags)
3. `git describe --tags --always --dirty` (local development)
4. Default fallback: `0.0.1`

**Version Format Handling:**
- **Input**: Supports complex git describe formats like `v1.2.3-alpha-7-gc2ee5a8`
- **Parsing**: Extracts clean version numbers (e.g., `1.2.3`) for .NET compatibility
- **Output**: 
  - `AssemblyVersion`: Clean format (`1.2.3`)
  - `InformationalVersion`: Full git describe output for debugging

**Testing Version Locally:**
```bash
# Create a test tag
git tag v1.2.3-beta

# Build and see version extraction
dotnet build -v normal

# Check version in game console output
```

## Recent Fixes

### Build Reliability Improvements

The build system has been improved to address common failure scenarios:

**Error Handling Enhancements:**
- Import failures now properly abort the build instead of continuing silently
- Each build export is verified to ensure output files are actually created
- Build script exits immediately on any error using proper shell error handling
- GitHub workflow only uploads artifacts when builds actually succeed

**Verification Steps:**
- Validates `.godot` directory creation after import
- Checks existence of each platform build output before packaging
- Fails fast if any expected build artifact is missing
- Removes `if: always()` conditions that masked build failures

These improvements ensure that build failures are detected early and artifacts are only created when builds genuinely succeed.
