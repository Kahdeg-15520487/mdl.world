# Fantasy World Generation Service with Sci-Fi Elements

This service provides a comprehensive world generation system that creates rich, detailed fantasy worlds enhanced with sci-fi elements. The service creates unique worlds that blend magical and technological aspects seamlessly.

## Features

### 🌍 World Generation
- **Fantasy-SciFi Integration**: Creates worlds where magic and technology coexist and interact
- **Customizable Parameters**: Control technology level (1-10), magic level (1-10), and world size
- **Multiple Themes**: Choose from various pre-built themes like "Magitech Empire", "Cyberpunk-Fantasy", "Quantum-Mystical"
- **Dynamic Content**: Generates places, characters, events, equipment, magic, and technology

### 🏰 Generated Content Types

#### Places
- **Unique Locations**: Fantasy-scifi themed places like "Astral Nexus", "Cyber Haven", "Quantum Spire"
- **Diverse Biomes**: From "Enchanted Crystal Forests" to "Cyber-Punk Cities" and "Bio-Mechanical Jungles"
- **Rich Details**: Each place includes climate, resources, dangers, government type, culture, and architecture
- **Access Requirements**: Some locations may require magical resonance, cybernetic enhancements, or quantum keys

#### Characters
- **Hybrid Races**: Cyber-Elves, Techno-Dwarves, Quantum Humans, Magical Androids, Bio-Enhanced Orcs
- **Unique Titles**: Quantum Sorcerer, Cyber-Paladin, Techno-Druid, Digital Necromancer
- **Rich Backstories**: Each character has achievements, affiliations, personality traits, and legacy items
- **Technological Integration**: Characters feature cybernetic implants, magical tattoos, and neural interfaces

#### Events
- **World-Changing Events**: Magical catastrophes, technological singularities, dimensional rifts
- **Historical Impact**: Events that shaped the balance between magic and technology
- **Interconnected**: Events reference characters and places from the same world

#### Equipment
- **Weapons**: Plasma swords, quantum axes, mana rifles, cyber staffs
- **Magical Artifacts**: Orbs, amulets, crowns with digital consciousness and magical souls
- **Advanced Materials**: Quantum steel, mithril alloy, bio-metal, crystal matrix
- **Enchantments**: Self-repair protocols, adaptive resistance, neural sync capabilities

#### Magic System
- **Spell Books**: Comprehensive guides integrating magical theory with technology
- **Runes**: Mystical symbols that bridge magical energy and digital processing
- **Alchemy**: Recipes combining magical essences with technological components
- **Languages**: Quantum Runic, Binary Mystical, Cyber-Elven, Techno-Draconic

#### Technology
- **Magitech Integration**: Technology enhanced with magical principles
- **Diverse Types**: Weapons, vehicles, computers, communication devices, medical equipment
- **Manufacturers**: Quantum Industries, Stellar Corporation, Mystic Technologies
- **Specifications**: Processing power, mana capacity, quantum coherence ratings

## API Endpoints

### GET /world/themes
Returns available world themes:
- Fantasy-SciFi
- Cyberpunk-Fantasy
- Space-Magic
- Bio-Magical
- Quantum-Mystical
- Steampunk-Arcane
- Digital-Shamanism
- Techno-Druidism

### GET /world/templates
Returns pre-configured world templates with descriptions and recommended settings.

### POST /world/generate
Generate a world with basic parameters:
```json
{
  "worldName": "Mystical Tech Realm",
  "theme": "Fantasy-SciFi",
  "techLevel": 7,
  "magicLevel": 8
}
```

### POST /world/generate-custom
Generate a world with advanced customization:
```json
{
  "worldName": "Cyber-Druidic Sanctuary",
  "theme": "Techno-Druidism",
  "techLevel": 6,
  "magicLevel": 9,
  "worldSize": 30,
  "includeMagicTech": true,
  "includeSpaceTravel": false,
  "includeAncientRuins": true,
  "preferredBiomes": ["Bio-Mechanical Jungles", "Mystical Data Centers"],
  "preferredRaces": ["Techno-Dwarves", "Cyber-Elves"],
  "difficultyLevel": "Medium"
}
```

### POST /world/{worldId}/enhance
Enhance an existing world with additional content:
```json
{
  "worldName": "Enhanced Realm",
  "contentType": "places" // or "characters", "events", "technology", "magic", "all"
}
```

## World Generation Parameters

### Technology Level (1-10)
- **1-3**: Pre-Industrial - Basic tools, early machinery
- **4-6**: Industrial - Steam power, early electronics
- **7-8**: Information Age - Computers, internet, mobile devices
- **9-10**: Space Age - Quantum computing, interstellar travel, AI

### Magic Level (1-10)
- **1-3**: Low Magic - Rare, mysterious, mostly folklore
- **4-7**: Medium Magic - Present but not dominant, coexists with technology
- **8-10**: High Magic - Abundant, integrated into daily life and technology

### World Size
- **Small (10-20)**: Intimate worlds with focused content
- **Medium (25-50)**: Balanced worlds with diverse content
- **Large (50+)**: Expansive worlds with extensive content

## Sample Generated World

Here's an example of what the service generates:

**World Name**: "Quantum Harmony Realm"
**Theme**: Fantasy-SciFi
**Description**: "A magnificent realm where ancient magic flows through quantum circuits, creating a unique blend of wonder and innovation."

**Sample Location**: "Neo-Astral Nexus"
- **Type**: Cyber-Punk City with Magical Anomalies
- **Population**: 45,000 (Mixed races including Cyber-Elves and Quantum Humans)
- **Government**: AI Democracy with Mage Council oversight
- **Architecture**: Crystalline towers with holographic facades
- **Resources**: Mana Crystals, Quantum Ore, Data Fragments
- **Dangers**: Rogue AI Constructs, Magical Anomalies

**Sample Character**: "Zara Starweaver, Quantum Sorcerer"
- **Race**: Cyber-Elf
- **Description**: "A master of both ancient magic and cutting-edge technology"
- **Achievements**: "Created the first magitech interface", "Discovered quantum-magical resonance"
- **Physical**: "Enhanced with subtle cybernetic implants and glowing magical tattoos"
- **Legacy Item**: "Quantum Staff of Infinite Possibilities"

**Sample Equipment**: "Plasma Sword of Enchanted Circuits"
- **Type**: Weapon (Energy Sword)
- **Rarity**: Epic
- **Material**: Quantum Steel
- **Enchantments**: Self-Repair Protocol, Adaptive Resistance, Neural Sync
- **Damage Type**: Plasma/Magical
- **Special**: Responds to wielder's thoughts via neural interface

## Running the Service

1. Start the service:
   ```bash
   cd mdl.world
   dotnet run
   ```

2. The service will be available at `http://localhost:5000`

3. Use the provided HTTP test file (`test-world-generation.http`) to try different endpoints

## Integration with D&D Campaigns

This service is perfect for:
- **Campaign Creation**: Generate rich, detailed worlds for your D&D campaigns
- **Session Preparation**: Quickly create new locations, NPCs, and plot hooks
- **Player Exploration**: Generate content on-the-fly as players explore
- **Multi-Campaign Consistency**: Maintain consistent world-building across sessions

The generated content is designed to be D&D-compatible while adding unique sci-fi elements that create memorable and engaging experiences for players.

## Architecture

The service follows clean architecture principles:
- **Controllers**: Handle HTTP requests and responses
- **Services**: Contain business logic for world generation
- **Models**: Define data structures for world elements
- **Dependency Injection**: Ensures loose coupling and testability

The world generation uses sophisticated algorithms to ensure:
- **Consistency**: All generated content fits together thematically
- **Variety**: Each generation produces unique, interesting content
- **Balance**: Magic and technology levels are properly balanced
- **Replayability**: Same parameters can produce different but equally valid worlds
