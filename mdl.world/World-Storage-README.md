# World Storage System

This document describes the JSON-based world storage system that has been added to the Fantasy World Generation Service.

## Overview

The world storage system provides persistent storage for generated worlds using JSON files. All worlds are automatically saved when generated and can be retrieved, updated, or deleted through the API.

## Features

### 🗃️ **Automatic Storage**
- All generated worlds are automatically saved to JSON files
- No manual save operation required for generation endpoints
- Worlds persist between application restarts

### 📁 **File-Based Storage**
- Uses JSON files stored in a configurable directory
- Default location: `~/MDL_Worlds/` (user profile folder)
- Each world stored as a separate `.json` file
- Human-readable format for easy inspection and backup

### 🔍 **World Management**
- List all stored worlds with metadata
- Load specific worlds by ID
- Update existing worlds
- Copy worlds with new names
- Delete worlds
- Import/Export functionality

### 📊 **Metadata Tracking**
- Creation and modification timestamps
- File size information
- Content counts (places, characters, events, items)
- Genre and theme information

## Configuration

The storage directory can be configured in `appsettings.json`:

```json
{
  "WorldStorage": {
    "Directory": "worlds"
  }
}
```

If not specified, defaults to `~/MDL_Worlds/` in the user's profile folder.

## API Endpoints

### Core World Operations
- `GET /World` - List all stored worlds (returns metadata)
- `GET /World/{worldId}` - Get a specific world
- `PUT /World/{worldId}` - Update a world
- `DELETE /World/{worldId}` - Delete a world

### World Generation (with auto-save)
- `POST /World/generate` - Generate and save a basic world
- `POST /World/generate-custom` - Generate and save a custom world
- `POST /World/generate-complete` - Generate and save a complete world

### World Management
- `POST /World/{worldId}/copy` - Copy an existing world
- `POST /World/{worldId}/enhance` - Enhance a world (loads from storage if exists)
- `GET /World/{worldId}/export` - Export world as downloadable JSON
- `POST /World/import` - Import a world from JSON

### Wiki Generation (storage-aware)
- `GET /World/{worldId}/wiki` - Get world wiki HTML (loads from storage)
- `GET /World/{worldId}/wiki/place/{placeId}` - Get place wiki
- `GET /World/{worldId}/wiki/character/{characterId}` - Get character wiki
- `GET /World/{worldId}/wiki/item/{itemId}` - Get item wiki

## Storage Behavior

### Generation Endpoints
All world generation endpoints now automatically save the generated world:
- `POST /World/generate`
- `POST /World/generate-custom` 
- `POST /World/generate-complete`

### Enhancement Endpoint
The enhance endpoint (`POST /World/{worldId}/enhance`) now:
1. First attempts to load an existing world from storage
2. If not found, generates a new world
3. Applies enhancements
4. Saves the enhanced world back to storage

### Wiki Endpoints
All wiki endpoints now:
1. First attempt to load the world from storage
2. If not found, generate a sample world and save it
3. Render the requested wiki content

## File Structure

Worlds are stored as individual JSON files:
```
worlds/
├── world-id-1.json
├── world-id-2.json
└── world-id-3.json
```

Each file contains the complete world data structure serialized to JSON with:
- Indented formatting for readability
- Camel case property naming
- Enum values as strings
- Null values omitted

## Usage Examples

### Generate and Store a World
```http
POST /World/generate
Content-Type: application/json

{
  "worldName": "My Fantasy World",
  "theme": "Fantasy-SciFi",
  "techLevel": 6,
  "magicLevel": 8
}
```

### List All Stored Worlds
```http
GET /World
```

Returns:
```json
[
  {
    "id": "world-123",
    "name": "My Fantasy World",
    "description": "...",
    "creationDate": "2025-01-01T00:00:00Z",
    "lastModified": "2025-01-01T00:00:00Z",
    "genre": "Fantasy",
    "theme": "Fantasy-SciFi",
    "placeCount": 10,
    "characterCount": 5,
    "eventCount": 3,
    "itemCount": 15,
    "fileSizeBytes": 25600
  }
]
```

### Load a Specific World
```http
GET /World/world-123
```

### Copy a World
```http
POST /World/world-123/copy
Content-Type: application/json

{
  "newName": "My Fantasy World - Copy"
}
```

### Export a World
```http
GET /World/world-123/export
```

Downloads the world as a JSON file.

## Error Handling

The storage service includes comprehensive error handling:
- File not found errors return appropriate HTTP 404 responses
- JSON serialization errors are logged and return HTTP 500
- Invalid world IDs are sanitized for file system safety
- Storage directory is automatically created if it doesn't exist

## Implementation Details

### JsonWorldStorageService
- Implements `IWorldStorageService` interface
- Uses `System.Text.Json` for serialization
- Configurable through dependency injection
- Comprehensive logging for debugging
- Thread-safe file operations

### File Naming
- World IDs are sanitized to remove invalid file name characters
- Files use `.json` extension
- Example: `world-123-abc.json`

### JSON Configuration
- Indented formatting for readability
- Camel case property names
- Enum values serialized as strings
- Null values omitted to reduce file size

## Testing

Use the provided test file `test-world-storage.http` to test all storage functionality with VS Code REST Client extension or similar tools.
