#!/bin/bash

set -e

echo "Starting Godot .NET build process..."

# Create artifact directory
mkdir -p artifact

# Change to project directory
cd /workspace

# Set up virtual display for headless operations
export DISPLAY=:99
Xvfb :99 -screen 0 1024x768x24 &
sleep 2

# Verify Godot installation
echo "Checking Godot version..."
godot --version --headless

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
if ! timeout 180 godot --headless --import --verbose --quit 2>&1; then
    echo "WARNING: Standard import failed or timed out..."
    # Try a more aggressive approach
    echo "Step 2: Trying import with xvfb..."
    if ! timeout 120 xvfb-run -a godot --headless --import --quit 2>&1; then
        echo "WARNING: Import with xvfb also failed..."
        echo "Step 3: Forcing basic project validation..."
        timeout 60 godot --headless --validate --quit 2>&1 || echo "Validation attempt completed"
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

# Strategy 1: Try manual dotnet build first (most reliable)
echo "Strategy 1: Manual dotnet build"
if dotnet build Commander-keen.csproj -c Release --nologo --verbosity quiet; then
    echo "Manual dotnet build successful"
elif timeout 90 godot --headless --build-solutions --quit 2>&1; then
    echo "Build successful with Godot headless strategy"
elif timeout 90 xvfb-run -a godot --headless --build-solutions --quit 2>&1; then
    echo "Build successful with xvfb strategy"
else
    echo "ERROR: All build strategies failed!"
    echo "Listing project files for debugging:"
    ls -la *.csproj *.cs 2>/dev/null || echo "No project files found"
    exit 1
fi

echo "C# build completed"

# Build for Windows
echo "Building for Windows..."
if timeout 90 godot --headless --export-release "WindowsDesktop" artifact/commander-keen-windows.exe --quit 2>&1; then
    echo "Windows export command completed"
elif timeout 90 xvfb-run -a godot --headless --export-release "WindowsDesktop" artifact/commander-keen-windows.exe --quit 2>&1; then
    echo "Windows export command completed with xvfb"
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
if timeout 90 godot --headless --export-release "Linux/X11" artifact/commander-keen-linux.x86_64 --quit 2>&1; then
    echo "Linux export command completed"
elif timeout 90 xvfb-run -a godot --headless --export-release "Linux/X11" artifact/commander-keen-linux.x86_64 --quit 2>&1; then
    echo "Linux export command completed with xvfb"
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
if timeout 90 godot --headless --export-release "macOS" artifact/commander-keen-macos.zip --quit 2>&1; then
    echo "macOS export command completed"
elif timeout 90 xvfb-run -a godot --headless --export-release "macOS" artifact/commander-keen-macos.zip --quit 2>&1; then
    echo "macOS export command completed with xvfb"
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

# Clean up background processes
echo "Cleaning up processes..."
pkill Xvfb 2>/dev/null || true

# Fix permissions for GitHub Actions (container runs as root, but CI needs access)
echo "Fixing file permissions..."
chmod -R 755 artifact/
chown -R 1001:1001 artifact/ 2>/dev/null || true

echo "Built artifacts:"
ls -la artifact/