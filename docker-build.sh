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

# Determine version once at startup
determine_version() {
    if [ -n "$VERSION_TAG" ]; then
        echo "Using version from VERSION_TAG: $VERSION_TAG"
        VERSION=$(echo "$VERSION_TAG" | sed 's/^v//')
    elif [ -n "$GITHUB_REF" ] && [[ "$GITHUB_REF" == refs/tags/* ]]; then
        TAG_NAME=$(echo "$GITHUB_REF" | sed 's/refs\/tags\///')
        VERSION=$(echo "$TAG_NAME" | sed 's/^v//')
        echo "Using version from GITHUB_REF: $VERSION"
    else
        echo "No version tag found, using project default"
        VERSION=""
    fi
}

# Function to setup Godot project (import assets and compile C#)
setup_project() {
    echo "=== SETUP: Importing assets and building C# ==="
    
    # Determine version for C# compilation
    determine_version
    
    cd /workspace

    # Import project assets
    echo "Importing project assets..."
    
    if timeout $IMPORT_TIMEOUT godot --headless --import --quit 2>&1; then
        echo "Asset import successful"
    else
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

    # Set up version properties if version was determined
    VERSION_PROPS=""
    if [ -n "$VERSION" ]; then
        VERSION_PROPS="-p:Version=$VERSION -p:InformationalVersion=$VERSION"
    fi

    if dotnet build Commander-keen.csproj -c Release --nologo --verbosity quiet $VERSION_PROPS; then
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
    echo "Output file: $output_file"
    
    # Run export command - ignore exit code since Godot may crash during cleanup
    timeout $GODOT_TIMEOUT godot --headless --export-release "$preset" "$output_file" --quit 2>&1
    local export_exit_code=$?
    
    # Check if build output exists (this is the real success indicator)
    if [ -f "$output_file" ]; then
        echo "$platform_name export completed successfully"
    else
        echo "ERROR: $platform_name build failed - output file not created!"
        echo "Export command exit code: $export_exit_code"
        echo "Contents of artifact directory:"
        ls -la artifact/ || echo "Artifact directory doesn't exist"
        return 1
    fi
    
    echo "$platform_name build successful"
    return 0
}

# Function to build platform(s) - all by default, single if BUILD_PLATFORM is specified
build_platforms() {
    cd /workspace
    
    # Ensure artifact directory exists
    mkdir -p artifact
    
    # Determine which platforms to build
    local platforms_to_build=()
    
    if [ -z "$BUILD_PLATFORM" ]; then
        echo "=== BUILD: All platforms (no specific platform set) ==="
        # Add all platforms to the build array
        for platform in "${!PLATFORMS[@]}"; do
            platforms_to_build+=("$platform")
        done
    else
        echo "=== BUILD: Single platform build ==="
        
        # Validate platform exists
        if [ -z "${PLATFORMS[$BUILD_PLATFORM]}" ]; then
            echo "ERROR: Unknown platform '$BUILD_PLATFORM'"
            echo "Available platforms: ${!PLATFORMS[*]}"
            exit 1
        fi
        
        # Add single platform to build array
        platforms_to_build=("$BUILD_PLATFORM")
        
        echo "Building platform: $BUILD_PLATFORM"
    fi
    
    # Build all platforms in the array
    for platform in "${platforms_to_build[@]}"; do
        IFS='|' read -r preset output <<< "${PLATFORMS[$platform]}"
        
        echo "Using preset: $preset"
        echo "Output file: $output"
        
        if ! build_platform "$platform" "$preset" "artifact/$output"; then
            exit 1
        fi
    done
    
    if [ ${#platforms_to_build[@]} -eq 1 ]; then
        echo "Platform build completed successfully!"
    else
        echo "All platforms built successfully!"
    fi
}

# Function to show built artifacts
show_artifacts() {
    echo "Built artifacts:"
    ls -la artifact/
}

# Main execution logic
main() {
    echo "Starting Godot .NET build process..."
    
    if [ "$1" = "setup" ] || [ -z "$1" ]; then
        setup_project
    fi
    
    if [ "$1" = "build" ] || [ -z "$1" ]; then
        build_platforms
        show_artifacts
    fi
}

# Execute main function with all arguments
main "$@"