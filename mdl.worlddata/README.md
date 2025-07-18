# World Data Model

This library provides a comprehensive data structure for modeling worlds in text adventure games.

## File Structure

The project is organized into multiple files for better maintainability:

```
mdl.worlddata/
├── Core/
│   └── World.cs                    # Core world classes
├── Geography/
│   └── Place.cs                    # Geographic locations and hierarchy
├── Characters/
│   └── HistoricFigure.cs          # NPCs and historic figures
├── Events/
│   └── Event.cs                   # Event system (World, Regional, Minor)
├── Items/
│   └── Equipment.cs               # Equipment, weapons, and artifacts
├── Magic/
│   ├── Spell.cs                   # Spells and spell books
│   ├── Rune.cs                    # Runes of power
│   └── Alchemy.cs                 # Alchemy recipes and ingredients
├── Technology/
│   └── TechnicalSpecification.cs  # Sci-fi technology specs
└── README.md
```

## Namespace Organization

### `mdl.worlddata.Core` → `Core/World.cs`
- **World**: The main container for all world data
- **WorldInfo**: Metadata about the world (genre, magic level, technology level)
- **WorldLaws**: Fundamental rules that govern the world

### `mdl.worlddata.Geography` → `Geography/Place.cs`
- **Place**: Geographic locations with hierarchical structure
- **PlaceType**: Types of places (World → Continent → Country → City → Building → Room)
- **GeographicInfo**: Climate, terrain, resources, coordinates
- **Population**: Demographics, government, languages, religions
- **Coordinates**: Latitude, longitude, elevation

### `mdl.worlddata.Characters` → `Characters/HistoricFigure.cs`
- **HistoricFigure**: Important NPCs with attributes, achievements, and relationships

### `mdl.worlddata.Events` → `Events/Event.cs`
- **BaseEvent**: Abstract base class for all events
- **WorldEvent**: Global events affecting the entire world
- **RegionalEvent**: Events affecting countries or regions
- **MinorEvent**: Local events affecting people or groups
- **EventStatus**: Status of events (Planned, Ongoing, Historical, Legendary, Mythical)

### `mdl.worlddata.Items` → `Items/Equipment.cs`
- **Equipment**: Base class for all items
- **Weapon**: Combat equipment with damage, range, enchantments
- **MagicalArtifact**: Fantasy items with spells, charges, attunement
- **SciFiArtifact**: Technology items with power sources and functions
- **EquipmentType**: Categories of equipment
- **EquipmentRarity**: Rarity levels from Common to Unique

### `mdl.worlddata.Magic` → `Magic/` (Multiple files)
- **SpellBook** & **Spell** (`Spell.cs`): Collections of spells with requirements
- **RuneOfPower** (`Rune.cs`): Magical runes with symbols and activation conditions
- **AlchemyRecipe** & **Ingredient** (`Alchemy.cs`): Recipes with ingredients, steps, and effects

### `mdl.worlddata.Technology` → `Technology/TechnicalSpecification.cs`
- **TechnicalSpecification**: Detailed specs for sci-fi technology
- **TechSpecType**: Types of technology (Weapon, Vehicle, Computer, etc.)

## Key Features

- **Hierarchical Geography**: Places can contain other places
- **Rich Population Data**: Race, class, age distributions
- **Historical Tracking**: Events linked to figures and places
- **Comprehensive Equipment**: Fantasy and sci-fi items
- **Magic System**: Spells, runes, and artifacts
- **Alchemy System**: Recipes and ingredients
- **Technology Specs**: For sci-fi elements
- **Flexible Design**: Custom properties and extensible enums

## Usage

```csharp
using mdl.worlddata.Core;
using mdl.worlddata.Geography;
using mdl.worlddata.Characters;

// Create a new world
var world = new World
{
    Name = "Aetheria",
    Description = "A magical realm where technology and magic coexist",
    WorldInfo = new WorldInfo
    {
        Genre = "Fantasy",
        MagicLevel = "High",
        TechnologyLevel = "Medieval"
    }
};

// Add a place
world.Places.Add(new Place
{
    Name = "Capital City",
    Type = PlaceType.City,
    Description = "The bustling heart of the kingdom"
});

// Add a historic figure
world.HistoricFigures.Add(new HistoricFigure
{
    Name = "Archmage Eldrin",
    Title = "Court Wizard",
    Race = "Elf",
    Class = "Wizard"
});
```

The structure uses GUIDs for unique identification and supports relationships between all entities through ID references.
