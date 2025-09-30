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
- When a GitHub release is created
- Manual trigger via workflow dispatch

**What it does**:
- Builds a custom Docker image with Godot 4.5 and .NET 9.0 from official sources
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

### Using Docker

The project includes a complete Docker setup for consistent builds:

1. **Build the Docker image:**
   ```bash
   docker build -t commander-keen-builder .
   ```

2. **Run the build:**

   **Basic Commands:**
   PowerShell
   ```powershell
   docker run --rm -v ${PWD}:/workspace -w /workspace commander-keen-builder
   ```
   
   Bash
   ```bash
   docker run --rm -v $(pwd):/workspace -w /workspace commander-keen-builder
   ```

   **Environment Variables:**
   - **`BUILD_PLATFORM`**: Build specific platform (`windows`, `linux`, `macos`) instead of all
   - **`VERSION_TAG`**: Override version number (format: `1.2.3` without 'v' prefix)

   **Examples:**
   ```powershell
   docker run --rm -v ${PWD}:/workspace -w /workspace -e BUILD_PLATFORM=windows -e VERSION_TAG=1.2.3 commander-keen-builder
   ```

   **Platform Outputs:**
   - `windows` → `commander-keen-windows.exe`
   - `linux` → `commander-keen-linux.x86_64`
   - `macos` → `commander-keen-macos.zip`

   Builds are created in the `artifact/` directory.

## Creating a Release

Create a release through the GitHub web interface and the workflow will build the artifacts.

## Version Handling

The build system automatically extracts version information from Git tags:

**Version Sources (in priority order):**
1. `VERSION_TAG` environment variable (manual override)
2. `GITHUB_REF` from CI/CD (release tags)  
3. `git describe --tags --always --dirty` (local development)
4. Default fallback: `0.0.1`

**Format:** Supports complex git describe formats like `v1.2.3-alpha-7-gc2ee5a8` and extracts clean version numbers for .NET compatibility.
