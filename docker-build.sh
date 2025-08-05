#!/bin/bash

set -e

# Configuration
PROJECT_NAME="commander-keen"
GODOT_TIMEOUT=120
IMPORT_TIMEOUT=180
BUILD_TIMEOUT=120

# Platform configurations
declare -A PLATFORMS=(
    ["windows"]="WindowsDesktop|${PROJECT_NAME}-windows.exe"
    ["linux"]="Linux/X11|${PROJECT_NAME}-linux.x86_64"
    ["macos"]="macOS|${PROJECT_NAME}-macos.zip"
)

echo "Starting Godot .NET build process..."

# Change to project directory (artifact directory already exists from Dockerfile)
cd /workspace

# Restore .NET dependencies (incremental restore of any new dependencies)
echo "Restoring .NET dependencies..."
dotnet restore --verbosity quiet

# Import project assets (after C# compilation)
echo "Importing project assets..."

echo "Running asset import with timeout..."
if ! timeout $IMPORT_TIMEOUT godot --headless --import --verbose --quit 2>&1; then
    echo "ERROR: Asset import failed!"
    echo "Contents of current directory:"
    ls -la
    exit 1
fi

# Give Godot time to complete all file operations
echo "Waiting for import completion..."
sleep 5

# Verify basic import structure
if [ ! -d ".godot" ]; then
    echo "ERROR: Import failed - .godot directory not found!"
    exit 1
fi

# Build C# project only if not skipping setup (i.e., during initial setup phase)
if [ "$SKIP_SETUP" != "true" ]; then
    echo "Building C# project..."

    # Set up version information for build
    VERSION_PROPS=""
    if [ -n "$VERSION_TAG" ]; then
        echo "Using version from tag: $VERSION_TAG"
        # Clean the version (remove 'v' prefix if present)
        CLEAN_VERSION=$(echo "$VERSION_TAG" | sed 's/^v//')
        VERSION_PROPS="-p:Version=$CLEAN_VERSION -p:InformationalVersion=$CLEAN_VERSION"
    elif [ -n "$GITHUB_REF" ] && [[ "$GITHUB_REF" == refs/tags/* ]]; then
        TAG_NAME=$(echo "$GITHUB_REF" | sed 's/refs\/tags\///')
        CLEAN_VERSION=$(echo "$TAG_NAME" | sed 's/^v//')
        echo "Using version from GitHub ref: $CLEAN_VERSION"
        VERSION_PROPS="-p:Version=$CLEAN_VERSION -p:InformationalVersion=$CLEAN_VERSION"
    else
        echo "No version tag found, using default"
    fi

    # Try manual dotnet build first (most reliable)
    if dotnet build ${PROJECT_NAME}.csproj -c Release --nologo --verbosity quiet $VERSION_PROPS; then
        echo "Manual dotnet build successful"
    elif timeout $BUILD_TIMEOUT godot --headless --build-solutions --quit 2>&1; then
        echo "Build successful with Godot strategy"
    else
        echo "ERROR: C# build failed!"
        echo "Listing project files for debugging:"
        ls -la *.csproj *.cs 2>/dev/null || echo "No project files found"
        exit 1
    fi

    echo "C# build completed"
else
    echo "Skipping C# build - using existing compiled assemblies"
fi

# Check if we're only doing setup (import + C# build)
if [ "$SETUP_ONLY" = "true" ]; then
    echo "Setup-only mode - completing import and C# build..."
    echo "Setup completed successfully!"
    
    # Fix permissions for GitHub Actions
    echo "Fixing file permissions..."
    chmod -R 755 .godot/ 2>/dev/null || true
    chown -R 1001:1001 .godot/ 2>/dev/null || true
    
    exit 0
fi

# Function to build a platform
build_platform() {
    local platform_name=$1
    local preset=$2
    local output_file=$3
    
    echo "Building for $platform_name..."
    
    if timeout $GODOT_TIMEOUT godot --headless --export-release "$preset" "$output_file" --quit 2>&1; then
        echo "$platform_name export command completed"
    else
        echo "ERROR: $platform_name build failed or timed out!"
        return 1
    fi

    # Verify build output
    if [ ! -f "$output_file" ]; then
        echo "ERROR: $platform_name executable was not created!"
        return 1
    fi
    
    echo "$platform_name build successful"
    return 0
}

# Check if we're building a specific platform (for matrix builds)
if [ -n "$BUILD_PLATFORM" ] && [ -n "$BUILD_PRESET" ] && [ -n "$BUILD_OUTPUT" ]; then
    echo "Building single platform: $BUILD_PLATFORM"
    
    # Skip setup if assets are already imported (for multi-step builds)
    if [ "$SKIP_SETUP" = "true" ] && [ -d ".godot" ]; then
        echo "Skipping setup - using existing assets and compiled assemblies..."
        
        # Verify C# assemblies exist
        if [ ! -d ".godot/mono" ]; then
            echo "ERROR: No compiled C# assemblies found! Run setup first."
            exit 1
        fi
    else
        echo "Running full setup (import + C# build)..."
    fi
    
    if ! build_platform "$BUILD_PLATFORM" "$BUILD_PRESET" "artifact/$BUILD_OUTPUT"; then
        exit 1
    fi
else
    echo "Building all platforms..."
    
    # Build all platforms using the configuration
    for platform in "${!PLATFORMS[@]}"; do
        IFS='|' read -r preset output <<< "${PLATFORMS[$platform]}"
        if ! build_platform "$platform" "$preset" "artifact/$output"; then
            exit 1
        fi
    done
fi

echo "Build completed successfully!"

# Fix permissions for GitHub Actions (container runs as root, but CI needs access)
echo "Fixing file permissions..."
chmod -R 755 artifact/
chown -R 1001:1001 artifact/ 2>/dev/null || true

echo "Built artifacts:"
ls -la artifact/