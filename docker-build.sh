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

# Import project assets (after C# compilation)
echo "Importing project assets..."

# First, try the standard import method with more time
echo "Step 1: Running standard asset import..."
if ! timeout 180 xvfb-run -a godot --headless --import; then
    echo "WARNING: Standard import failed..."
fi

# Give Godot time to complete all file operations
echo "Waiting for import completion..."
sleep 10

# Verify basic import structure
if [ ! -d ".godot" ]; then
    echo "ERROR: Import failed - .godot directory not found!"
    exit 1
fi

# Build C# project first - this is critical for Godot 4.x with C#
echo "Building C# project..."
if ! godot --headless --build-solutions; then
    echo "ERROR: Failed to build C# solutions!"
    echo "C# scripts must be compiled before assets can be imported."
    exit 1
fi

# Build for Windows
echo "Building for Windows..."
if ! godot --headless --export-release "WindowsDesktop" artifact/commander-keen-windows.exe; then
    echo "ERROR: Windows build failed!"
    exit 1
fi

# Verify Windows build output
if [ ! -f "artifact/commander-keen-windows.exe" ]; then
    echo "ERROR: Windows executable was not created!"
    exit 1
fi
echo "Windows build successful"

# Build for Linux
echo "Building for Linux..."
if ! godot --headless --export-release "Linux/X11" artifact/commander-keen-linux.x86_64; then
    echo "ERROR: Linux build failed!"
    exit 1
fi

# Verify Linux build output
if [ ! -f "artifact/commander-keen-linux.x86_64" ]; then
    echo "ERROR: Linux executable was not created!"
    exit 1
fi
echo "Linux build successful"

# Build for macOS
echo "Building for macOS..."
if ! godot --headless --export-release "macOS" artifact/commander-keen-macos.zip; then
    echo "ERROR: macOS build failed!"
    exit 1
fi

# Verify macOS build output
if [ ! -f "artifact/commander-keen-macos.zip" ]; then
    echo "ERROR: macOS build was not created!"
    exit 1
fi
echo "macOS build successful"

echo "Build completed successfully!"
echo "Built artifacts:"
ls -la artifact/