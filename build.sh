#!/bin/bash

# Build the plugin
dotnet build Jellyfin.Plugin.HomeSections -c Debug

# Create output directory
mkdir -p output

# Copy the built DLL to the output directory
cp Jellyfin.Plugin.HomeSections/bin/Debug/net6.0/Jellyfin.Plugin.HomeSections.dll output/
scp output/Jellyfin.Plugin.HomeSections.dll umbrel@jpc-home.local:/home/umbrel/umbrel/app-data/jellyfin/data/config/data/plugins/HomeSections_0.0.0.1/Jellyfin.Plugin.HomeSections/bin/Debug/net6.0
echo "moneyprintergobrrr" | ssh umbrel@jpc-home.local "echo 'moneyprintergobrrr' | sudo -S ~/umbrel/scripts/app restart jellyfin"
echo "Build complete. Plugin DLL is in the output directory."
