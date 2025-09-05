# Use Microsoft's official .NET image as base which includes .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy

# Avoid prompts from apt
ENV DEBIAN_FRONTEND=noninteractive

# Install minimal dependencies for Godot headless build
RUN apt-get update && apt-get install -y \
    wget \
    unzip \
    ca-certificates \
    fontconfig \
    && rm -rf /var/lib/apt/lists/*

# Set Godot version
ENV GODOT_VERSION="4.4.1"

# Download and install Godot headless
RUN wget -q https://github.com/godotengine/godot/releases/download/${GODOT_VERSION}-stable/Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64.zip

RUN unzip Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64.zip

RUN mkdir -p /opt/godot \
    && mv Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64/* /opt/godot/ \
    && ln -s /opt/godot/Godot_v${GODOT_VERSION}-stable_mono_linux.x86_64 /usr/local/bin/godot \
    && chmod +x /opt/godot/Godot_v${GODOT_VERSION}-stable_mono_linux.x86_64 \
    && rm -rf Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64 \
    && rm Godot_v${GODOT_VERSION}-stable_mono_linux_x86_64.zip

# Download export templates
RUN wget -q https://github.com/godotengine/godot/releases/download/${GODOT_VERSION}-stable/Godot_v${GODOT_VERSION}-stable_mono_export_templates.tpz

RUN mkdir -p /root/.local/share/godot/export_templates/${GODOT_VERSION}.stable.mono \
    && unzip Godot_v${GODOT_VERSION}-stable_mono_export_templates.tpz \
    && mv templates/* /root/.local/share/godot/export_templates/${GODOT_VERSION}.stable.mono/ \
    && rm -rf templates Godot_v${GODOT_VERSION}-stable_mono_export_templates.tpz

# Create working directory and artifact directory
WORKDIR /workspace
RUN mkdir -p /workspace/artifact

# Copy project files for dependency restoration (for Docker layer caching)
COPY Commander-keen.csproj Commander-keen.sln ./
RUN echo "Pre-restoring common .NET dependencies..." \
    && dotnet restore --verbosity quiet || true

# Verify installations and display versions
RUN echo "=== Build Environment Info ===" \
    && echo "Godot version:" \
    && godot --version --headless \
    && echo ".NET version:" \
    && dotnet --version \
    && echo "=========================="

# Copy build script
COPY docker-build.sh /usr/local/bin/docker-build.sh
RUN chmod +x /usr/local/bin/docker-build.sh

# Set up proper permissions for GitHub Actions (container runs as root, CI needs access)
# Create user with same UID as GitHub Actions (1001) for seamless file ownership
RUN groupadd -g 1001 builder && useradd -u 1001 -g 1001 -m builder \
    && chown -R 1001:1001 /workspace \
    && chmod -R 755 /workspace

# Use ENTRYPOINT so arguments are passed to the script properly
ENTRYPOINT ["/usr/local/bin/docker-build.sh"]