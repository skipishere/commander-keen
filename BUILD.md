# Build Release Documentation

This document explains how the automated release build system works for the Commander Keen Godot project.

## Overview

The project includes a GitHub Actions workflow to automatically build the game for Windows, Linux, and macOS platforms using Docker when releases are created.

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
   ```bash
   docker run --rm -v $(pwd):/workspace -w /workspace commander-keen-builder
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
- Imports project assets before building
- Cross-platform builds from Ubuntu GitHub runners with Docker
- Proper error handling and timeout management