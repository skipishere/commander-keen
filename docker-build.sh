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

# Build C# project first - this is critical for Godot 4.x with C#
echo "Building C# project..."
if ! godot --headless --build-solutions; then
    echo "ERROR: Failed to build C# solutions!"
    echo "C# scripts must be compiled before assets can be imported."
    exit 1
fi

# Import project assets (after C# compilation)
echo "Importing project assets..."
if ! timeout 120 xvfb-run -a godot --headless --import --quit; then
    echo "ERROR: Failed to import project assets!"
    echo "This often happens when C# compilation failed or resources can't be loaded."
    echo "Trying alternative import method..."
    
    # Try a more thorough import process
    if ! timeout 120 xvfb-run -a godot --headless --editor --quit; then
        echo "ERROR: Alternative import method also failed!"
        exit 1
    fi
fi

# Wait a moment for import to complete
sleep 5

# Verify import was successful by checking if .godot directory was created
if [ ! -d ".godot" ]; then
    echo "ERROR: Import failed - .godot directory not found!"
    echo "For C# projects, ensure that C# compilation completed successfully before import."
    exit 1
fi

# Additional verification - check for imported assets
if [ ! -d ".godot/imported" ]; then
    echo "ERROR: Import incomplete - .godot/imported directory not found!"
    echo "This indicates that asset import did not complete successfully."
    echo "For C# projects, this often means C# scripts couldn't be loaded due to compilation issues."
    exit 1
fi

echo "Import verification successful - found $(ls .godot/imported | wc -l) imported assets"

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