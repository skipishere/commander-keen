# Build Release Documentation

This document explains how the automated release build system works for the Commander Keen Godot project.

## Overview

The project includes GitHub Actions workflows to automatically build the game for Windows, Linux, and macOS platforms when releases are created.

## Workflows

### 1. Main Release Workflow (`build-release.yml`)

**Trigger**: 
- When a git tag matching `v*.*.*` is pushed
- When a GitHub release is created
- Manual trigger via workflow dispatch

**What it does**:
- Sets up Godot 4.4.1 and .NET 8.0 on Ubuntu
- Restores .NET dependencies
- Imports Godot project assets
- Builds the game for all three platforms
- Creates release packages
- Uploads artifacts to GitHub

### 2. Docker-based Workflow (`build-release-docker.yml`)

**Trigger**: 
- Manual trigger only (for testing)

**What it does**:
- Uses a pre-built Docker image with Godot
- Builds the game in a containerized environment
- Good for testing the build process

## Export Presets

The project includes export presets for:
- **Windows Desktop**: Builds as `commander-keen-windows.exe`
- **Linux/X11**: Builds as `commander-keen-linux.x86_64`
- **macOS**: Builds as `commander-keen-macos.zip`

## Manual Build

To build manually:

1. Install Godot 4.4.1 and .NET 8.0 SDK
2. Run `dotnet restore` to restore dependencies
3. Open the project in Godot or use headless mode:
   ```bash
   godot --headless --import
   godot --headless --export-release "WindowsDesktop" builds/windows.exe
   godot --headless --export-release "Linux/X11" builds/linux.x86_64
   godot --headless --export-release "macOS" builds/macos.zip
   ```

## Creating a Release

1. Create a git tag: `git tag v1.0.0`
2. Push the tag: `git push origin v1.0.0`
3. The workflow will automatically build and create a GitHub release with artifacts

Alternatively, create a release through the GitHub web interface and the workflow will build the artifacts.