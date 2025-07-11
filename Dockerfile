# Use Microsoft's official .NET image as base which includes .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy

# Avoid prompts from apt
ENV DEBIAN_FRONTEND=noninteractive

# Install dependencies for Godot
RUN apt-get update && apt-get install -y \
    wget \
    unzip \
    curl \
    ca-certificates \
    xvfb \
    libc6-dev \
    libgcc-s1 \
    libasound2-dev \
    libpulse-dev \
    libudev-dev \
    libxi6 \
    libxrandr2 \
    libxinerama1 \
    libxcursor1 \
    libgl1-mesa-dev \
    libglu1-mesa-dev \
    && rm -rf /var/lib/apt/lists/*

# Set Godot version - using 4.2.2 (latest stable available)
ENV GODOT_VERSION="4.2.2"

# Download and install Godot headless
RUN wget -q https://github.com/godotengine/godot/releases/download/4.2.2-stable/Godot_v4.2.2-stable_linux.x86_64.zip

RUN unzip Godot_v4.2.2-stable_linux.x86_64.zip

RUN mv Godot_v4.2.2-stable_linux.x86_64 /usr/local/bin/godot \
    && chmod +x /usr/local/bin/godot \
    && rm Godot_v4.2.2-stable_linux.x86_64.zip

# Download export templates
RUN wget -q https://github.com/godotengine/godot/releases/download/4.2.2-stable/Godot_v4.2.2-stable_export_templates.tpz

RUN mkdir -p /root/.local/share/godot/export_templates/4.2.2.stable \
    && unzip Godot_v4.2.2-stable_export_templates.tpz \
    && mv templates/* /root/.local/share/godot/export_templates/4.2.2.stable/ \
    && rm -rf templates Godot_v4.2.2-stable_export_templates.tpz

# Create working directory
WORKDIR /workspace

# Copy build script
COPY docker-build.sh /usr/local/bin/docker-build.sh
RUN chmod +x /usr/local/bin/docker-build.sh

# Default command
CMD ["/usr/local/bin/docker-build.sh"]