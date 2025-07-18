# Quick Start Guide: Create and View Your World

## Step 1: Generate a World

Use the world generation API to create your fantasy/sci-fi world:

```http
POST http://localhost:5000/World/generate
Content-Type: application/json

{
  "worldName": "My Epic World",
  "theme": "Fantasy-SciFi",
  "techLevel": 7,
  "magicLevel": 8
}
```

**Response**: You'll get a complete world with ID, places, characters, items, and more. The world is automatically saved!

## Step 2: View Your World Wiki

Once created, view your world as a beautiful HTML wiki:

```http
GET http://localhost:5000/World/{worldId}/wiki
```

Replace `{worldId}` with the ID from step 1 (e.g., `abc123-def456-ghi789`).

## What You Get

### 🌍 **Complete World Wiki**
- World overview with theme and description
- All locations with rich details
- Character profiles and backstories  
- Magic items and technology specs
- Historical events and timeline

### 🏰 **Detailed Sub-Wikis**
- **Places**: `GET /World/{worldId}/wiki/place/{placeId}`
- **Characters**: `GET /World/{worldId}/wiki/character/{characterId}`
- **Items**: `GET /World/{worldId}/wiki/item/{itemId}`

## Quick Example

1. **Generate world**:
   ```bash
   curl -X POST http://localhost:5000/World/generate \
     -H "Content-Type: application/json" \
     -d '{"worldName":"Aetheria","theme":"Fantasy-SciFi","techLevel":6,"magicLevel":8}'
   ```

2. **Get the world ID** from response (e.g., `world-123`)

3. **View wiki**:
   ```bash
   curl http://localhost:5000/World/world-123/wiki
   ```

## Available Themes
- `Fantasy-SciFi` - Magic meets technology
- `Cyberpunk-Fantasy` - Neon-lit magical realms
- `Space-Magic` - Interstellar magical empires
- `Bio-Magical` - Living magical technology
- `Quantum-Mystical` - Reality-bending magic

## Pro Tips

### 🚀 **Generate More Content**
Use the complete world generator for richer content:
```http
POST http://localhost:5000/World/generate-complete
Content-Type: application/json

{
  "worldName": "Epic Realm",
  "theme": "Fantasy-SciFi",
  "techLevel": 7,
  "magicLevel": 8,
  "totalPlaces": 20,
  "characterCount": 15,
  "equipmentCount": 25
}
```

### 📋 **List Your Worlds**
See all your created worlds:
```http
GET http://localhost:5000/World
```

### 🔧 **Enhance Existing Worlds**
Add more content to any world:
```http
POST http://localhost:5000/World/{worldId}/enhance
Content-Type: application/json

{
  "contentType": "characters"
}
```

### 💾 **Export/Import**
- **Export**: `GET /World/{worldId}/export` - Downloads JSON file
- **Import**: `POST /World/import` - Upload JSON to create new world

## That's It!

In just 2 API calls, you've created a persistent fantasy world and viewed it as a rich HTML wiki. Your world is automatically saved and ready for enhancement, sharing, or campaign use.

**Next**: Explore specific places, characters, and items using the detailed wiki endpoints!
