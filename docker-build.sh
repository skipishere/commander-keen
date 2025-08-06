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

# Function to setup Godot project (import assets and compile C#)
setup_project() {
    echo "=== SETUP: Importing assets and building C# ==="
    
    cd /workspace

    # Import project assets
    echo "Importing project assets..."
    if ! timeout $IMPORT_TIMEOUT godot --headless --import --verbose --quit 2>&1; then
        echo "ERROR: Asset import failed!"
        echo "Contents of current directory:"
        ls -la
        exit 1
    fi

    # Verify basic import structure
    if [ ! -d ".godot" ]; then
        echo "ERROR: Import failed - .godot directory not found!"
        exit 1
    fi

    # Build C# project
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

    if dotnet build ${PROJECT_NAME}.csproj -c Release --nologo --verbosity quiet $VERSION_PROPS; then
        echo "Manual dotnet build successful"
    else
        echo "ERROR: C# build failed!"
        echo "Listing project files for debugging:"
        ls -la *.csproj *.cs 2>/dev/null || echo "No project files found"
        exit 1
    fi

    echo "Setup completed successfully!"
}

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

# Function to build platform(s) - all by default, single if BUILD_PLATFORM is specified
build_platforms() {
    cd /workspace
    
    # Verify C# assemblies exist
    if [ ! -d ".godot/mono" ]; then
        echo "ERROR: No compiled C# assemblies found! Run setup first."
        exit 1
    fi
    
    if [ -z "$BUILD_PLATFORM" ]; then
        echo "=== BUILD: All platforms (no specific platform set) ==="
        build_all_platforms
        return 0
    fi
    
    echo "=== BUILD: Single platform build ==="
    
    # Look up platform configuration
    if [ -z "${PLATFORMS[$BUILD_PLATFORM]}" ]; then
        echo "ERROR: Unknown platform '$BUILD_PLATFORM'"
        echo "Available platforms: ${!PLATFORMS[*]}"
        exit 1
    fi
    
    # Parse platform configuration
    IFS='|' read -r preset output <<< "${PLATFORMS[$BUILD_PLATFORM]}"
    
    echo "Building platform: $BUILD_PLATFORM"
    echo "Using preset: $preset"
    echo "Output file: $output"
    
    if ! build_platform "$BUILD_PLATFORM" "$preset" "artifact/$output"; then
        exit 1
    fi
    
    echo "Platform build completed successfully!"
}

# Function to build all platforms
build_all_platforms() {
    echo "Building all platforms..."
    
    # Build all platforms using the configuration
    for platform in "${!PLATFORMS[@]}"; do
        IFS='|' read -r preset output <<< "${PLATFORMS[$platform]}"
        if ! build_platform "$platform" "$preset" "artifact/$output"; then
            exit 1
        fi
    done
    
    echo "All platforms built successfully!"
}

# Function to show built artifacts
show_artifacts() {
    echo "Built artifacts:"
    ls -la artifact/
}

# Main execution logic
main() {
    echo "Starting Godot .NET build process..."
    
    # Check for specific function calls
    if [ "$1" = "setup" ]; then
        setup_project
        return 0
    elif [ "$1" = "build" ]; then
        build_platforms
        show_artifacts
        return 0
    fi
    
    # Default behavior: full build (setup + build all platforms)
    echo "=== FULL BUILD: Setup + All Platforms ==="
    setup_project
    build_all_platforms
    show_artifacts
}

# Execute main function with all arguments
main "$@"