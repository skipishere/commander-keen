#!/bin/bash

set -e

echo "Starting Godot .NET build process..."

# Create artifact directory
mkdir -p artifact

# Change to project directory
cd /workspace

# Verify Godot installation
echo "Checking Godot version..."
godot --version

# Verify .NET installation
echo "Checking .NET version..."
dotnet --version

# Restore .NET dependencies
echo "Restoring .NET dependencies..."
dotnet restore

# Import project assets
echo "Importing project assets..."
timeout 60 xvfb-run -a godot --headless --import || echo "Import completed or timed out"

# Wait a moment for import to complete
sleep 5

# Build for Windows
echo "Building for Windows..."
godot --headless --export-release "WindowsDesktop" artifact/commander-keen-windows.exe

# Build for Linux
echo "Building for Linux..."
godot --headless --export-release "Linux/X11" artifact/commander-keen-linux.x86_64

# Build for macOS
echo "Building for macOS..."
godot --headless --export-release "macOS" artifact/commander-keen-macos.zip

echo "Build completed successfully!"
echo "Built artifacts:"
ls -la artifact/