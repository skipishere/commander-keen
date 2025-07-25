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

# First, try the standard import method with more time and better timeout
echo "Step 1: Running standard asset import with timeout..."
if ! timeout 300 xvfb-run -a godot --headless --import --verbose; then
    echo "WARNING: Standard import failed or timed out..."
    # Try a more aggressive approach
    echo "Step 2: Trying import with quit flag..."
    if ! timeout 120 xvfb-run -a godot --headless --import --quit; then
        echo "WARNING: Import with quit flag also failed..."
    fi
fi

# Give Godot time to complete all file operations
echo "Waiting for import completion..."
sleep 5

# Verify basic import structure
if [ ! -d ".godot" ]; then
    echo "ERROR: Import failed - .godot directory not found!"
    echo "Contents of current directory:"
    ls -la
    exit 1
fi

echo "Import completed successfully, .godot directory exists"

# Build C# project first - this is critical for Godot 4.x with C#
echo "Building C# project..."
echo "Attempting build with multiple strategies..."

# Strategy 1: Try normal build first
echo "Strategy 1: Normal build"
if timeout 120 godot --headless --build-solutions --verbose 2>&1; then
    echo "Build successful with normal strategy"
elif timeout 120 godot --headless --build-solutions 2>&1; then
    echo "Build successful with quiet strategy"
elif timeout 120 xvfb-run -a godot --headless --build-solutions 2>&1; then
    echo "Build successful with xvfb strategy"
else
    echo "All build strategies failed, trying manual dotnet build..."
    if dotnet build Commander-keen.csproj -c Release --nologo; then
        echo "Manual dotnet build successful"
    else
        echo "ERROR: All build strategies failed!"
        echo "Listing project files for debugging:"
        ls -la *.csproj *.cs
        exit 1
    fi
fi

echo "C# build completed"

# Build for Windows
echo "Building for Windows..."
if timeout 120 godot --headless --export-release "WindowsDesktop" artifact/commander-keen-windows.exe 2>&1 || \
   timeout 120 xvfb-run -a godot --headless --export-release "WindowsDesktop" artifact/commander-keen-windows.exe 2>&1; then
    echo "Windows export command completed"
else
    echo "ERROR: Windows build failed or timed out!"
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
if timeout 120 godot --headless --export-release "Linux/X11" artifact/commander-keen-linux.x86_64 2>&1 || \
   timeout 120 xvfb-run -a godot --headless --export-release "Linux/X11" artifact/commander-keen-linux.x86_64 2>&1; then
    echo "Linux export command completed"
else
    echo "ERROR: Linux build failed or timed out!"
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
if timeout 120 godot --headless --export-release "macOS" artifact/commander-keen-macos.zip 2>&1 || \
   timeout 120 xvfb-run -a godot --headless --export-release "macOS" artifact/commander-keen-macos.zip 2>&1; then
    echo "macOS export command completed"
else
    echo "ERROR: macOS build failed or timed out!"
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