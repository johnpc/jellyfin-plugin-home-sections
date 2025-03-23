# Jellyfin Home Sections Plugin

A Jellyfin plugin that allows users to customize their home screen with configurable sections.

## Features

- Configure custom home sections through an admin configuration page
- Default section types included:
  - Library Tiles - displays all available libraries
  - Recently Added - shows recently added media items
  - Pinned Collection - displays items from a specific collection
- Add, remove, and reorder sections as desired
- REST API endpoint to retrieve section data for client-side rendering

## Installation

1. Download the latest release from the releases page
2. In the Jellyfin dashboard, go to Dashboard → Plugins → Catalog
3. Click on "..." and select "Install from file"
4. Select the downloaded .dll file
5. Restart Jellyfin when prompted

## Configuration

1. After installation, go to Dashboard → Plugins
2. Find "Home Sections" in the plugins list and click on it
3. Configure your desired home sections
4. Save your configuration

## Development

This plugin is built using .NET and follows the Jellyfin plugin architecture.

### Building

```bash
dotnet build
```

### Prerequisites

- .NET SDK 6.0 or later
- Jellyfin server (for testing)

## License

This project is licensed under the MIT License - see the LICENSE file for details.
