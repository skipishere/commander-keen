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

# First, try the standard import method with more time
echo "Step 1: Running standard asset import..."
if ! timeout 180 xvfb-run -a godot --headless --import; then
    echo "WARNING: Standard import failed, trying editor mode..."
    
    # Try opening in editor mode briefly to trigger full import
    echo "Step 2: Running editor import..."
    if ! timeout 180 xvfb-run -a godot --headless --editor --quit; then
        echo "ERROR: Both import methods failed!"
        exit 1
    fi
fi

# Give Godot time to complete all file operations
echo "Waiting for import completion..."
sleep 10

# Verify basic import structure
if [ ! -d ".godot" ]; then
    echo "ERROR: Import failed - .godot directory not found!"
    exit 1
fi

if [ ! -d ".godot/imported" ]; then
    echo "ERROR: Import incomplete - .godot/imported directory not found!"
    exit 1
fi

# Verify specific critical assets that cause build failures
echo "Verifying critical asset imports..."
CRITICAL_ASSETS=(
    ".godot/imported/border.png-0be7b0e67e5060aa14b097bb4fddc453.ctex"
    ".godot/imported/PressStart2P-Regular.ttf-c7d83f2c4bd295d4c960a93a703ae2b2.fontdata"
)

MISSING_ASSETS=()
for asset in "${CRITICAL_ASSETS[@]}"; do
    if [ ! -f "$asset" ]; then
        MISSING_ASSETS+=("$asset")
    fi
done

if [ ${#MISSING_ASSETS[@]} -gt 0 ]; then
    echo "ERROR: Critical assets failed to import:"
    for asset in "${MISSING_ASSETS[@]}"; do
        echo "  - Missing: $asset"
    done
    
    echo "Attempting forced reimport..."
    # Try a more aggressive import that doesn't quit immediately
    timeout 300 xvfb-run -a godot --headless --import --verbose || {
        echo "ERROR: Forced reimport also failed!"
        echo "Available imported files:"
        ls -la .godot/imported/ | head -20
        exit 1
    }
    
    # Wait longer for completion
    sleep 15
    
    # Check again
    STILL_MISSING=()
    for asset in "${MISSING_ASSETS[@]}"; do
        if [ ! -f "$asset" ]; then
            STILL_MISSING+=("$asset")
        fi
    done
    
    if [ ${#STILL_MISSING[@]} -gt 0 ]; then
        echo "ERROR: Critical assets still missing after forced reimport:"
        for asset in "${STILL_MISSING[@]}"; do
            echo "  - Still missing: $asset"
        done
        echo "This indicates a fundamental issue with asset import."
        echo "Available imported files:"
        ls -la .godot/imported/ | head -20
        exit 1
    fi
fi

echo "Import verification successful - found $(ls .godot/imported | wc -l) imported assets"
echo "All critical assets verified successfully"

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