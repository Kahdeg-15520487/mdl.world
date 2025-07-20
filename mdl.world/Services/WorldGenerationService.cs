using mdl.worlddata.Core;
using mdl.worlddata.Geography;
using mdl.worlddata.Characters;
using mdl.worlddata.Events;
using mdl.worlddata.Items;
using mdl.worlddata.Magic;
using mdl.worlddata.Technology;
using mdl.world.Controllers;

namespace mdl.world.Services
{
    public class WorldGenerationService : IWorldGenerationService
    {
        private readonly ILogger<WorldGenerationService> _logger;
        private readonly Random _random;

        // Fantasy-SciFi themed data
        private readonly string[] _fantasySciFiThemes = {
            "Magitech Empire", "Cybernetic Wizardry", "Quantum Spellcasting", "Stellar Kingdoms",
            "Techno-Druidism", "Mechanical Familiars", "Crystal-Powered Ships", "Dimensional Rifts",
            "Bio-Magical Synthesis", "Enchanted Circuits", "Astral Networks", "Runic Computers"
        };

        private readonly string[] _biomes = {
            "Enchanted Crystal Forests", "Cyber-Punk Cities", "Floating Sky Islands", "Underground Tech Vaults",
            "Magical Wastelands", "Quantum Beaches", "Temporal Anomaly Zones", "Bio-Mechanical Jungles",
            "Stellar Observatories", "Mystical Data Centers", "Arcane Laboratories", "Dimensional Harbors"
        };

        private readonly string[] _races = {
            "Cyber-Elves", "Techno-Dwarves", "Quantum Humans", "Magical Androids", "Stellar Gnomes",
            "Bio-Enhanced Orcs", "Crystal-Born", "Data-Sprites", "Mecha-Dragons", "Astral Beings"
        };

        public WorldGenerationService(ILogger<WorldGenerationService> logger)
        {
            _logger = logger;
            _random = new Random();
        }

        public async Task<World> GenerateWorldAsync(string worldName, string theme = "Fantasy-SciFi", int techLevel = 5, int magicLevel = 7)
        {
            _logger.LogInformation("Generating world: {WorldName} with theme: {Theme}", worldName, theme);

            var parameters = new WorldGenerationParameters
            {
                WorldName = worldName,
                Theme = theme,
                TechLevel = techLevel,
                MagicLevel = magicLevel,
                IncludeMagicTech = true,
                IncludeSpaceTravel = techLevel >= 7,
                WorldSize = 25
            };

            return await GenerateCustomWorldAsync(parameters);
        }

        public async Task<World> EnhanceWorldAsync(World world, string contentType)
        {
            _logger.LogInformation("Enhancing world: {WorldName} with content type: {ContentType}", world.Name, contentType);

            switch (contentType.ToLower())
            {
                case "places":
                    await GenerateAdditionalPlacesAsync(world);
                    break;
                case "characters":
                    await GenerateAdditionalCharactersAsync(world);
                    break;
                case "events":
                    await GenerateAdditionalEventsAsync(world);
                    break;
                case "technology":
                    await GenerateAdditionalTechnologyAsync(world);
                    break;
                case "magic":
                    await GenerateAdditionalMagicAsync(world);
                    break;
                default:
                    await GenerateAdditionalPlacesAsync(world);
                    await GenerateAdditionalCharactersAsync(world);
                    break;
            }

            return world;
        }

        public async Task<World> GenerateCustomWorldAsync(WorldGenerationParameters parameters)
        {
            _logger.LogInformation("Generating custom world: {WorldName}", parameters.WorldName);

            var world = new World
            {
                Name = parameters.WorldName,
                Description = await GenerateWorldDescriptionAsync(parameters),
                WorldInfo = GenerateWorldInfo(parameters)
            };

            // Generate core content
            await GeneratePlacesAsync(world, parameters);
            await GenerateCharactersAsync(world, parameters);
            await GenerateEventsAsync(world, parameters);
            await GenerateEquipmentAsync(world, parameters);
            await GenerateMagicAsync(world, parameters);
            await GenerateTechnologyAsync(world, parameters);

            _logger.LogInformation("Successfully generated world: {WorldName} with {PlaceCount} places, {CharacterCount} characters",
                world.Name, world.Places.Count, world.HistoricFigures.Count);

            return world;
        }

        public async Task<World> GenerateCompleteWorldAsync(WorldGenerationParameters parameters, CompleteWorldRequest request)
        {
            _logger.LogInformation("Generating complete world: {WorldName} with detailed specifications", parameters.WorldName);

            var world = new World
            {
                Name = parameters.WorldName,
                Description = await GenerateDetailedWorldDescriptionAsync(parameters, request),
                WorldInfo = GenerateEnhancedWorldInfo(parameters, request)
            };

            // Generate places with hierarchy if requested
            if (request.GenerateHierarchy)
            {
                await GenerateHierarchicalPlacesAsync(world, parameters, request);
            }
            else
            {
                await GenerateSpecificPlacesAsync(world, parameters, request);
            }

            // Generate characters with specified count
            await GenerateSpecificCharactersAsync(world, parameters, request.CharacterCount);

            // Generate events with specified count
            await GenerateSpecificEventsAsync(world, parameters, request.HistoricalEventCount);

            // Generate equipment with specified count
            await GenerateSpecificEquipmentAsync(world, parameters, request.EquipmentCount);

            // Generate magic content with specified counts
            await GenerateSpecificMagicAsync(world, parameters, request);

            // Generate technology with specified count
            await GenerateSpecificTechnologyAsync(world, parameters, request.TechnologyCount);

            // Generate connections between places if requested
            if (request.GenerateConnections)
            {
                await GeneratePlaceConnectionsAsync(world, parameters);
            }

            // Generate economic systems if requested
            if (request.GenerateEconomy)
            {
                await GenerateEconomicSystemsAsync(world, parameters);
            }

            // Generate political systems if requested
            if (request.GeneratePolitics)
            {
                await GeneratePoliticalSystemsAsync(world, parameters);
            }

            _logger.LogInformation("Successfully generated complete world: {WorldName} with {PlaceCount} places, {CharacterCount} characters, {EventCount} events",
                world.Name, world.Places.Count, world.HistoricFigures.Count, world.WorldEvents.Count);

            return world;
        }

        private async Task<string> GenerateWorldDescriptionAsync(WorldGenerationParameters parameters)
        {
            var description = $"The world of {parameters.WorldName} is a unique realm where ";

            if (parameters.MagicLevel > 7)
                description += "ancient magic flows through every corner, ";
            else if (parameters.MagicLevel > 3)
                description += "magic exists in harmony with technology, ";
            else
                description += "remnants of old magic still linger, ";

            if (parameters.TechLevel > 7)
                description += "and advanced technology has reached the stars. ";
            else if (parameters.TechLevel > 3)
                description += "while innovative technology shapes daily life. ";
            else
                description += "though technology remains primitive in most regions. ";

            description += $"This {parameters.Theme} world is characterized by ";
            description += _fantasySciFiThemes[_random.Next(_fantasySciFiThemes.Length)].ToLower();
            description += ", creating a unique blend of wonder and innovation.";

            return description;
        }

        private async Task<string> GenerateDetailedWorldDescriptionAsync(WorldGenerationParameters parameters, CompleteWorldRequest request)
        {
            var description = $"The vast world of {parameters.WorldName} is a {request.WorldScale.ToLower()} realm that spans ";

            if (request.WorldScale == "Interplanetary")
                description += "multiple worlds and star systems, ";
            else if (request.WorldScale == "Global")
                description += "entire continents and oceans, ";
            else if (request.WorldScale == "Continental")
                description += "vast continents and regions, ";
            else
                description += "diverse regions and territories, ";

            description += $"featuring {request.ContinentCount} major continents, {request.CountryCount} sovereign nations, ";
            description += $"and {request.RegionCount} distinct regions. ";

            if (parameters.MagicLevel > 7)
                description += "Ancient magic flows through every corner of this realm, ";
            else if (parameters.MagicLevel > 3)
                description += "Magic exists in harmony with technology, ";
            else
                description += "Remnants of old magic still linger, ";

            if (parameters.TechLevel > 7)
                description += "while advanced technology has reached the stars. ";
            else if (parameters.TechLevel > 3)
                description += "while innovative technology shapes daily life. ";
            else
                description += "though technology remains primitive in most regions. ";

            description += $"This {parameters.Theme} world is home to {request.CharacterCount} notable figures ";
            description += $"and contains {request.DungeonCount} mysterious dungeons, ";
            description += $"{request.NaturalFeatureCount} natural wonders, ";
            description += $"and {request.HistoricalEventCount} world-shaping events that have molded its history.";

            return description;
        }

        private WorldInfo GenerateWorldInfo(WorldGenerationParameters parameters)
        {
            return new WorldInfo
            {
                Genre = parameters.Theme,
                TimeEra = parameters.TechLevel > 6 ? "Future" : "Medieval-Future",
                MagicLevel = parameters.MagicLevel switch
                {
                    <= 3 => "Low",
                    <= 7 => "Medium",
                    _ => "High"
                },
                TechnologyLevel = parameters.TechLevel switch
                {
                    <= 3 => "Pre-Industrial",
                    <= 6 => "Industrial",
                    <= 8 => "Information Age",
                    _ => "Space Age"
                },
                ActiveThemes = new List<string> { parameters.Theme, "Adventure", "Exploration", "Magic-Tech Fusion" },
                Laws = new WorldLaws
                {
                    MagicExists = parameters.MagicLevel > 0,
                    DeathIsPermanent = parameters.DifficultyLevel == "Hard",
                    TimeTravel = parameters.TechLevel >= 9,
                    Multiverse = parameters.TechLevel >= 8 && parameters.MagicLevel >= 8,
                    CustomLaws = new Dictionary<string, object>
                    {
                        { "MagicTechInteraction", parameters.IncludeMagicTech },
                        { "SpaceTravel", parameters.IncludeSpaceTravel },
                        { "AncientRuins", parameters.IncludeAncientRuins }
                    }
                }
            };
        }

        private WorldInfo GenerateEnhancedWorldInfo(WorldGenerationParameters parameters, CompleteWorldRequest request)
        {
            var worldInfo = GenerateWorldInfo(parameters);

            // Add custom settings from the request
            if (request.CustomSettings != null)
            {
                foreach (var setting in request.CustomSettings)
                {
                    worldInfo.CustomSettings[setting.Key] = setting.Value.ToString() ?? "";
                }
            }

            // Add scale and structure information
            worldInfo.CustomSettings["WorldScale"] = request.WorldScale;
            worldInfo.CustomSettings["ContinentCount"] = request.ContinentCount.ToString();
            worldInfo.CustomSettings["CountryCount"] = request.CountryCount.ToString();
            worldInfo.CustomSettings["RegionCount"] = request.RegionCount.ToString();
            worldInfo.CustomSettings["HasHierarchy"] = request.GenerateHierarchy.ToString();
            worldInfo.CustomSettings["HasConnections"] = request.GenerateConnections.ToString();
            worldInfo.CustomSettings["HasEconomy"] = request.GenerateEconomy.ToString();
            worldInfo.CustomSettings["HasPolitics"] = request.GeneratePolitics.ToString();

            return worldInfo;
        }

        private async Task GenerateHierarchicalPlacesAsync(World world, WorldGenerationParameters parameters, CompleteWorldRequest request)
        {
            // Generate places with hierarchical structure using existing logic
            var totalPlaces = request.ContinentCount + request.CountryCount + request.RegionCount +
                             request.CityCount + request.TownCount + request.VillageCount +
                             request.DungeonCount + request.NaturalFeatureCount;

            // Use the existing place generation logic
            var tempParameters = new WorldGenerationParameters
            {
                WorldName = parameters.WorldName,
                Theme = parameters.Theme,
                TechLevel = parameters.TechLevel,
                MagicLevel = parameters.MagicLevel,
                IncludeMagicTech = parameters.IncludeMagicTech,
                IncludeSpaceTravel = parameters.IncludeSpaceTravel,
                WorldSize = totalPlaces
            };

            await GeneratePlacesAsync(world, tempParameters);
        }

        private async Task GenerateSpecificPlacesAsync(World world, WorldGenerationParameters parameters, CompleteWorldRequest request)
        {
            var totalPlaces = request.TotalPlaces;

            var tempParameters = new WorldGenerationParameters
            {
                WorldName = parameters.WorldName,
                Theme = parameters.Theme,
                TechLevel = parameters.TechLevel,
                MagicLevel = parameters.MagicLevel,
                IncludeMagicTech = parameters.IncludeMagicTech,
                IncludeSpaceTravel = parameters.IncludeSpaceTravel,
                WorldSize = totalPlaces
            };

            await GeneratePlacesAsync(world, tempParameters);
        }

        private async Task GenerateSpecificCharactersAsync(World world, WorldGenerationParameters parameters, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var character = new HistoricFigure
                {
                    Name = GenerateCharacterName(),
                    Title = GenerateCharacterTitle(parameters),
                    Description = GenerateCharacterDescription(parameters),
                    Race = _races[_random.Next(_races.Length)],
                    Class = GenerateCharacterClass(parameters),
                    BirthDate = DateTime.UtcNow.AddYears(-_random.Next(50, 500)),
                    DeathDate = _random.Next(1, 5) == 1 ? DateTime.UtcNow.AddYears(-_random.Next(1, 50)) : null,
                    IsAlive = _random.Next(1, 5) != 1,
                    BirthPlaceId = world.Places.Count > 0 ? world.Places[_random.Next(world.Places.Count)].Id : "",
                    AssociatedPlaceIds = GenerateAssociatedPlaceIds(world),
                    Achievements = GenerateAchievements(parameters),
                    RelatedEventIds = new List<string>(),
                    Attributes = GenerateAttributes(),
                    Relationships = GenerateRelationshipIds(world)
                };

                world.HistoricFigures.Add(character);
            }
        }

        private async Task GenerateSpecificEventsAsync(World world, WorldGenerationParameters parameters, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var worldEvent = new WorldEvent
                {
                    Name = GenerateEventName(parameters),
                    Description = GenerateEventDescription(parameters),
                    Type = GenerateWorldEventType(parameters),
                    StartDate = DateTime.UtcNow.AddYears(-_random.Next(1, 1000)),
                    EndDate = _random.Next(1, 3) == 1 ? DateTime.UtcNow.AddYears(-_random.Next(1, 500)) : null,
                    Status = EventStatus.Historical,
                    ParticipantIds = GenerateEventParticipantIds(world),
                    AffectedPlaceIds = GenerateAffectedPlaceIds(world),
                    Consequences = GenerateEventConsequencesDictionary(parameters),
                    GlobalImpactLevel = _random.Next(1, 11)
                };

                world.WorldEvents.Add(worldEvent);
            }
        }

        private async Task GenerateSpecificEquipmentAsync(World world, WorldGenerationParameters parameters, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Equipment equipment;

                if (_random.Next(1, 3) == 1)
                {
                    equipment = new Weapon
                    {
                        Name = GenerateWeaponName(parameters),
                        Description = GenerateEquipmentDescription(parameters),
                        Type = EquipmentType.Weapon,
                        Rarity = (EquipmentRarity)_random.Next(0, 7),
                        Value = _random.Next(10, 10000),
                        Weight = _random.Next(1, 50),
                        Material = GenerateEquipmentMaterial(parameters),
                        Condition = "Good",
                        CreatorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : null,
                        History = GenerateEquipmentHistory(parameters),
                        Properties = GenerateEquipmentProperties(parameters),
                        WeaponType = (WeaponType)_random.Next(0, 12),
                        Damage = _random.Next(1, 20),
                        Range = _random.Next(5, 100),
                        DamageType = GenerateDamageType(parameters),
                        IsMagical = parameters.MagicLevel > 3,
                        Enchantments = GenerateEnchantments(parameters)
                    };
                }
                else
                {
                    equipment = new MagicalArtifact
                    {
                        Name = GenerateMagicalArtifactName(parameters),
                        Description = GenerateEquipmentDescription(parameters),
                        Type = EquipmentType.Artifact,
                        Rarity = (EquipmentRarity)_random.Next(0, 7),
                        Value = _random.Next(100, 50000),
                        Weight = _random.Next(1, 20),
                        Material = GenerateEquipmentMaterial(parameters),
                        Condition = "Good",
                        CreatorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : null,
                        History = GenerateEquipmentHistory(parameters),
                        Properties = GenerateEquipmentProperties(parameters),
                        MagicType = (MagicType)_random.Next(0, 9),
                        MagicPower = _random.Next(1, 20),
                        Spells = GenerateArtifactSpells(parameters),
                        Charges = _random.Next(1, 10),
                        MaxCharges = _random.Next(5, 15)
                    };
                }

                world.Equipment.Add(equipment);
            }
        }

        private async Task GenerateSpecificMagicAsync(World world, WorldGenerationParameters parameters, CompleteWorldRequest request)
        {
            // Generate SpellBooks
            for (int i = 0; i < request.SpellBookCount; i++)
            {
                var spellBook = new SpellBook
                {
                    Name = GenerateSpellBookName(parameters),
                    Description = GenerateSpellBookDescription(parameters),
                    AuthorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : "",
                    RequiredLevel = _random.Next(1, 15),
                    Language = GenerateAncientLanguage(),
                    IsComplete = _random.Next(1, 4) != 1,
                    MissingPages = new List<string>()
                };

                world.SpellBooks.Add(spellBook);
            }

            // Generate Runes
            for (int i = 0; i < request.RuneCount; i++)
            {
                var rune = new RuneOfPower
                {
                    Name = GenerateRuneName(parameters),
                    Symbol = GenerateRuneSymbol(),
                    Description = GenerateRuneDescription(parameters),
                    Type = (RuneType)_random.Next(0, 9),
                    PowerLevel = _random.Next(1, 10),
                    Element = GenerateRuneElement(),
                    Effects = GenerateRuneEffects(parameters),
                    ActivationCondition = GenerateActivationCondition(),
                    IsActive = _random.Next(1, 3) == 1,
                    Location = world.Places.Count > 0 ? world.Places[_random.Next(world.Places.Count)].Name : "Unknown",
                    CreatorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : ""
                };

                world.RunesOfPower.Add(rune);
            }

            // Generate Alchemy Recipes
            for (int i = 0; i < request.AlchemyRecipeCount; i++)
            {
                var recipe = new AlchemyRecipe
                {
                    Name = GenerateAlchemyRecipeName(parameters),
                    Description = GenerateAlchemyRecipeDescription(parameters),
                    Type = (AlchemyType)_random.Next(0, 8),
                    Difficulty = _random.Next(1, 10),
                    Ingredients = GenerateAlchemyIngredients(parameters),
                    Steps = GenerateAlchemySteps(parameters),
                    PreparationTime = TimeSpan.FromHours(_random.Next(1, 24)),
                    Effects = GenerateAlchemyEffects(parameters),
                    SideEffects = GenerateAlchemySideEffects(),
                    CreatorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : "",
                    IsSecret = _random.Next(1, 4) == 1
                };

                world.AlchemyRecipes.Add(recipe);
            }
        }

        private async Task GenerateSpecificTechnologyAsync(World world, WorldGenerationParameters parameters, int count)
        {
            var techTypes = Enum.GetValues<TechSpecType>();

            for (int i = 0; i < count; i++)
            {
                var techSpec = new TechnicalSpecification
                {
                    Name = GenerateTechName(parameters),
                    Description = GenerateTechDescription(parameters),
                    Type = techTypes[_random.Next(techTypes.Length)],
                    TechLevel = _random.Next(1, Math.Max(2, parameters.TechLevel + 1)),
                    Manufacturer = GenerateTechManufacturer(parameters),
                    ModelNumber = GenerateTechModelNumber(),
                    Specifications = GenerateTechSpecifications(parameters),
                    Requirements = GenerateTechRequirements(parameters),
                    Capabilities = GenerateTechCapabilities(parameters),
                    PowerConsumption = GeneratePowerConsumption(parameters),
                    MaintenanceSchedule = GenerateMaintenanceSchedule(),
                    IsClassified = _random.Next(1, 10) == 1
                };

                world.TechnicalSpecs.Add(techSpec);
            }
        }

        private async Task GeneratePlaceConnectionsAsync(World world, WorldGenerationParameters parameters)
        {
            // Generate trade routes and connections between places
            var places = world.Places.ToList();

            foreach (var place in places)
            {
                if (place.CustomProperties == null)
                    place.CustomProperties = new Dictionary<string, string>();

                var connections = new List<string>();
                var connectionCount = _random.Next(1, 5);

                for (int i = 0; i < connectionCount && i < places.Count - 1; i++)
                {
                    var connectedPlace = places[_random.Next(places.Count)];
                    if (connectedPlace.Id != place.Id)
                    {
                        connections.Add($"{connectedPlace.Name} ({GenerateConnectionType()})");
                    }
                }

                place.CustomProperties["Connections"] = string.Join(", ", connections);
            }
        }

        private async Task GenerateEconomicSystemsAsync(World world, WorldGenerationParameters parameters)
        {
            foreach (var place in world.Places)
            {
                if (place.CustomProperties == null)
                    place.CustomProperties = new Dictionary<string, string>();

                place.CustomProperties["EconomicSystem"] = GenerateEconomicSystem(parameters);
                place.CustomProperties["MainIndustries"] = GenerateMainIndustries(parameters);
                place.CustomProperties["TradeGoods"] = GenerateTradeGoods(parameters);
                place.CustomProperties["Currency"] = GenerateCurrency(parameters);
            }
        }

        private async Task GeneratePoliticalSystemsAsync(World world, WorldGenerationParameters parameters)
        {
            foreach (var place in world.Places)
            {
                if (place.CustomProperties == null)
                    place.CustomProperties = new Dictionary<string, string>();

                place.CustomProperties["PoliticalSystem"] = GeneratePoliticalSystem(parameters);
                place.CustomProperties["Ruler"] = GenerateRuler(parameters);
                place.CustomProperties["Laws"] = GenerateLaws(parameters);
                place.CustomProperties["Diplomacy"] = GenerateDiplomaticStance(parameters);
            }
        }

        private async Task GeneratePlacesAsync(World world, WorldGenerationParameters parameters)
        {
            var placesToGenerate = parameters.WorldSize;

            for (int i = 0; i < placesToGenerate; i++)
            {
                var place = new Place
                {
                    Name = GenerateFantasySciFiPlaceName(),
                    Description = GeneratePlaceDescription(parameters),
                    Type = GeneratePlaceType(),
                    Geography = new GeographicInfo
                    {
                        Climate = GenerateClimate(),
                        Terrain = GenerateTerrain(parameters),
                        NaturalResources = GenerateResources(parameters),
                        Coordinates = new Coordinates
                        {
                            Latitude = _random.NextDouble() * 180 - 90,
                            Longitude = _random.NextDouble() * 360 - 180,
                            Elevation = parameters.IncludeSpaceTravel ? _random.Next(-1000, 5000) : _random.Next(0, 3000)
                        },
                        Area = _random.Next(1, 1000000),
                        Borders = GenerateBorders()
                    },
                    Population = new Population
                    {
                        TotalCount = _random.Next(100, 100000),
                        RaceDistribution = GenerateRaceDistribution(),
                        GovernmentType = GenerateGovernmentType(),
                        Languages = GenerateLanguages(),
                        Religions = GenerateReligions(parameters)
                    },
                    NotableFeatures = GenerateNotableFeatures(parameters),
                    CustomProperties = GenerateCustomProperties(parameters)
                };

                world.Places.Add(place);
            }
        }

        private async Task GenerateCharactersAsync(World world, WorldGenerationParameters parameters)
        {
            var charactersToGenerate = Math.Min(parameters.WorldSize / 2, 15);

            for (int i = 0; i < charactersToGenerate; i++)
            {
                var character = new HistoricFigure
                {
                    Name = GenerateCharacterName(),
                    Title = GenerateCharacterTitle(parameters),
                    Description = GenerateCharacterDescription(parameters),
                    Race = _races[_random.Next(_races.Length)],
                    Class = GenerateCharacterClass(parameters),
                    BirthDate = DateTime.UtcNow.AddYears(-_random.Next(50, 500)),
                    DeathDate = _random.Next(1, 5) == 1 ? DateTime.UtcNow.AddYears(-_random.Next(1, 50)) : null,
                    IsAlive = _random.Next(1, 5) != 1,
                    BirthPlaceId = world.Places.Count > 0 ? world.Places[_random.Next(world.Places.Count)].Id : "",
                    AssociatedPlaceIds = GenerateAssociatedPlaceIds(world),
                    Achievements = GenerateAchievements(parameters),
                    RelatedEventIds = new List<string>(),
                    Attributes = GenerateAttributes(),
                    Relationships = GenerateRelationshipIds(world)
                };

                world.HistoricFigures.Add(character);
            }
        }

        private async Task GenerateEventsAsync(World world, WorldGenerationParameters parameters)
        {
            var eventsToGenerate = parameters.WorldSize / 3;

            for (int i = 0; i < eventsToGenerate; i++)
            {
                var worldEvent = new WorldEvent
                {
                    Name = GenerateEventName(parameters),
                    Description = GenerateEventDescription(parameters),
                    Type = GenerateWorldEventType(parameters),
                    StartDate = DateTime.UtcNow.AddYears(-_random.Next(1, 1000)),
                    EndDate = _random.Next(1, 3) == 1 ? DateTime.UtcNow.AddYears(-_random.Next(1, 500)) : null,
                    Status = EventStatus.Historical,
                    ParticipantIds = GenerateEventParticipantIds(world),
                    AffectedPlaceIds = GenerateAffectedPlaceIds(world),
                    Consequences = GenerateEventConsequencesDictionary(parameters),
                    GlobalImpactLevel = _random.Next(1, 11)
                };

                world.WorldEvents.Add(worldEvent);
            }
        }

        private async Task GenerateEquipmentAsync(World world, WorldGenerationParameters parameters)
        {
            var equipmentToGenerate = parameters.WorldSize / 2;

            for (int i = 0; i < equipmentToGenerate; i++)
            {
                Equipment equipment;

                if (_random.Next(1, 3) == 1)
                {
                    equipment = new Weapon
                    {
                        Name = GenerateWeaponName(parameters),
                        Description = GenerateEquipmentDescription(parameters),
                        Type = EquipmentType.Weapon,
                        Rarity = (EquipmentRarity)_random.Next(0, 7),
                        Value = _random.Next(10, 10000),
                        Weight = _random.Next(1, 50),
                        Material = GenerateEquipmentMaterial(parameters),
                        Condition = "Good",
                        CreatorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : null,
                        History = GenerateEquipmentHistory(parameters),
                        Properties = GenerateEquipmentProperties(parameters),
                        WeaponType = (WeaponType)_random.Next(0, 12),
                        Damage = _random.Next(1, 20),
                        Range = _random.Next(5, 100),
                        DamageType = GenerateDamageType(parameters),
                        IsMagical = parameters.MagicLevel > 3,
                        Enchantments = GenerateEnchantments(parameters)
                    };
                }
                else
                {
                    equipment = new MagicalArtifact
                    {
                        Name = GenerateMagicalArtifactName(parameters),
                        Description = GenerateEquipmentDescription(parameters),
                        Type = EquipmentType.Artifact,
                        Rarity = (EquipmentRarity)_random.Next(0, 7),
                        Value = _random.Next(100, 50000),
                        Weight = _random.Next(1, 20),
                        Material = GenerateEquipmentMaterial(parameters),
                        Condition = "Good",
                        CreatorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : null,
                        History = GenerateEquipmentHistory(parameters),
                        Properties = GenerateEquipmentProperties(parameters),
                        MagicType = (MagicType)_random.Next(0, 9),
                        MagicPower = _random.Next(1, 20),
                        Spells = GenerateArtifactSpells(parameters),
                        Charges = _random.Next(1, 10),
                        MaxCharges = _random.Next(5, 15)
                    };
                }

                world.Equipment.Add(equipment);
            }
        }

        private async Task GenerateMagicAsync(World world, WorldGenerationParameters parameters)
        {
            if (parameters.MagicLevel == 0) return;

            // Generate SpellBooks
            var spellBooksToGenerate = Math.Max(1, parameters.MagicLevel / 2);
            for (int i = 0; i < spellBooksToGenerate; i++)
            {
                var spellBook = new SpellBook
                {
                    Name = GenerateSpellBookName(parameters),
                    Description = GenerateSpellBookDescription(parameters),
                    AuthorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : "",
                    RequiredLevel = _random.Next(1, 15),
                    Language = GenerateAncientLanguage(),
                    IsComplete = _random.Next(1, 4) != 1,
                    MissingPages = new List<string>()
                };

                world.SpellBooks.Add(spellBook);
            }

            // Generate Runes
            var runesToGenerate = parameters.MagicLevel;
            for (int i = 0; i < runesToGenerate; i++)
            {
                var rune = new RuneOfPower
                {
                    Name = GenerateRuneName(parameters),
                    Symbol = GenerateRuneSymbol(),
                    Description = GenerateRuneDescription(parameters),
                    Type = (RuneType)_random.Next(0, 9),
                    PowerLevel = _random.Next(1, 10),
                    Element = GenerateRuneElement(),
                    Effects = GenerateRuneEffects(parameters),
                    ActivationCondition = GenerateActivationCondition(),
                    IsActive = _random.Next(1, 3) == 1,
                    Location = world.Places.Count > 0 ? world.Places[_random.Next(world.Places.Count)].Name : "Unknown",
                    CreatorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : ""
                };

                world.RunesOfPower.Add(rune);
            }

            // Generate Alchemy Recipes
            var alchemyToGenerate = parameters.MagicLevel / 2;
            for (int i = 0; i < alchemyToGenerate; i++)
            {
                var recipe = new AlchemyRecipe
                {
                    Name = GenerateAlchemyRecipeName(parameters),
                    Description = GenerateAlchemyRecipeDescription(parameters),
                    Type = (AlchemyType)_random.Next(0, 8),
                    Difficulty = _random.Next(1, 10),
                    Ingredients = GenerateAlchemyIngredients(parameters),
                    Steps = GenerateAlchemySteps(parameters),
                    PreparationTime = TimeSpan.FromHours(_random.Next(1, 24)),
                    Effects = GenerateAlchemyEffects(parameters),
                    SideEffects = GenerateAlchemySideEffects(),
                    CreatorId = world.HistoricFigures.Count > 0 ? world.HistoricFigures[_random.Next(world.HistoricFigures.Count)].Id : "",
                    IsSecret = _random.Next(1, 4) == 1
                };

                world.AlchemyRecipes.Add(recipe);
            }
        }

        private async Task GenerateTechnologyAsync(World world, WorldGenerationParameters parameters)
        {
            if (parameters.TechLevel == 0) return;

            var techToGenerate = parameters.TechLevel;
            var techTypes = Enum.GetValues<TechSpecType>();

            for (int i = 0; i < techToGenerate; i++)
            {
                var techSpec = new TechnicalSpecification
                {
                    Name = GenerateTechName(parameters),
                    Description = GenerateTechDescription(parameters),
                    Type = techTypes[_random.Next(techTypes.Length)],
                    TechLevel = _random.Next(1, Math.Max(2, parameters.TechLevel + 1)),
                    Manufacturer = GenerateTechManufacturer(parameters),
                    ModelNumber = GenerateTechModelNumber(),
                    Specifications = GenerateTechSpecifications(parameters),
                    Requirements = GenerateTechRequirements(parameters),
                    Capabilities = GenerateTechCapabilities(parameters),
                    PowerConsumption = GeneratePowerConsumption(parameters),
                    MaintenanceSchedule = GenerateMaintenanceSchedule(),
                    IsClassified = _random.Next(1, 10) == 1
                };

                world.TechnicalSpecs.Add(techSpec);
            }
        }

        // Helper methods for generating specific content
        private string GenerateFantasySciFiPlaceName()
        {
            var prefixes = new[] { "Neo", "Astral", "Cyber", "Quantum", "Mystic", "Stellar", "Arcane", "Tech", "Crystal", "Void" };
            var suffixes = new[] { "Haven", "Citadel", "Nexus", "Spire", "Realm", "Station", "Core", "Gate", "Sanctum", "Hub" };

            return $"{prefixes[_random.Next(prefixes.Length)]}{suffixes[_random.Next(suffixes.Length)]}";
        }

        private string GeneratePlaceDescription(WorldGenerationParameters parameters)
        {
            var descriptions = new[]
            {
                "A magnificent fusion of ancient magic and cutting-edge technology",
                "Where holographic displays blend seamlessly with enchanted crystals",
                "A bustling metropolis powered by both arcane energy and quantum processors",
                "An otherworldly location where spells are cast through neural interfaces",
                "A hidden sanctuary where magical creatures coexist with AI constructs"
            };

            return descriptions[_random.Next(descriptions.Length)];
        }

        private string GenerateClimate()
        {
            var climates = new[] { "Temperate", "Tropical", "Arctic", "Desert", "Mystical", "Artificial", "Temporal Flux", "Energy Storm" };
            return climates[_random.Next(climates.Length)];
        }

        private List<string> GenerateResources(WorldGenerationParameters parameters)
        {
            var resources = new List<string>();
            var availableResources = new[]
            {
                "Mana Crystals", "Quantum Ore", "Mythril", "Data Fragments", "Ether Gas",
                "Nano-materials", "Enchanted Metals", "Bio-fuel", "Temporal Shards", "Psionic Stones"
            };

            var resourceCount = _random.Next(1, 4);
            for (int i = 0; i < resourceCount; i++)
            {
                var resource = availableResources[_random.Next(availableResources.Length)];
                if (!resources.Contains(resource))
                    resources.Add(resource);
            }

            return resources;
        }

        private string GenerateGovernmentType()
        {
            var governments = new[]
            {
                "Techno-Monarchy", "Mage Council", "AI Democracy", "Corporate Federation",
                "Quantum Republic", "Arcane Empire", "Digital Commune", "Hybrid Oligarchy"
            };
            return governments[_random.Next(governments.Length)];
        }

        private List<string> GenerateNotableFeatures(WorldGenerationParameters parameters)
        {
            var features = new List<string> { "Magitech Spire", "Quantum Rift", "Digital Shrine", "Bio-Mechanical Grove" };
            return features;
        }

        private Dictionary<string, string> GenerateCustomProperties(WorldGenerationParameters parameters)
        {
            return new Dictionary<string, string>
            {
                { "Theme", parameters.Theme },
                { "DangerLevel", _random.Next(1, 10).ToString() }
            };
        }

        private string GenerateCharacterName()
        {
            var firstNames = new[] { "Zara", "Kai", "Nova", "Orion", "Luna", "Axel", "Vera", "Cyrus", "Aria", "Neon" };
            var lastNames = new[] { "Starweaver", "Cybermage", "Quantumborn", "Techbane", "Voidwalker", "Dataforge", "Spellcode", "Netcaster" };

            return $"{firstNames[_random.Next(firstNames.Length)]} {lastNames[_random.Next(lastNames.Length)]}";
        }

        private string GenerateCharacterTitle(WorldGenerationParameters parameters)
        {
            var titles = new[]
            {
                "Quantum Sorcerer", "Cyber-Paladin", "Techno-Druid", "Digital Necromancer",
                "Mecha-Ranger", "Data-Witch", "Nano-Cleric", "Stellar Barbarian"
            };
            return titles[_random.Next(titles.Length)];
        }

        private string GenerateCharacterDescription(WorldGenerationParameters parameters)
        {
            var descriptions = new[]
            {
                "A master of both ancient magic and cutting-edge technology",
                "One who bridges the gap between the mystical and the digital",
                "A pioneer in the fusion of arcane arts and cyber-enhancement",
                "A guardian of the balance between magic and machine",
                "An explorer of the quantum realms and magical dimensions"
            };

            return descriptions[_random.Next(descriptions.Length)];
        }

        private List<string> GenerateAchievements(WorldGenerationParameters parameters)
        {
            var achievements = new List<string>();
            var availableAchievements = new[]
            {
                "Created the first magitech interface", "Discovered quantum-magical resonance",
                "Established the Cyber-Mage Academy", "Defeated the Rogue AI Overlord",
                "Opened the first dimensional portal", "Synthesized digital consciousness with magical souls"
            };

            var achievementCount = _random.Next(1, 4);
            for (int i = 0; i < achievementCount; i++)
            {
                var achievement = availableAchievements[_random.Next(availableAchievements.Length)];
                if (!achievements.Contains(achievement))
                    achievements.Add(achievement);
            }

            return achievements;
        }

        // Additional helper methods
        private string GenerateEventName(WorldGenerationParameters parameters) =>
            $"The {_fantasySciFiThemes[_random.Next(_fantasySciFiThemes.Length)]} Incident";

        private string GenerateEventDescription(WorldGenerationParameters parameters) =>
            "A significant event that shaped the balance between magic and technology in the world.";

        private Dictionary<string, string> GenerateEventConsequencesDictionary(WorldGenerationParameters parameters) =>
            new Dictionary<string, string>
            {
                { "Primary", "Changed the fundamental understanding of magitech integration" },
                { "Secondary", "Established new trade routes between magical and technological regions" }
            };

        private List<string> GenerateEventParticipantIds(World world) =>
            world.HistoricFigures.Take(_random.Next(1, 4)).Select(f => f.Id).ToList();

        private List<string> GenerateAffectedPlaceIds(World world) =>
            world.Places.Take(_random.Next(1, 3)).Select(p => p.Id).ToList();

        private PlaceType GeneratePlaceType() =>
            new[] { PlaceType.City, PlaceType.Town, PlaceType.Village, PlaceType.NaturalFeature, PlaceType.Dungeon, PlaceType.Other }[_random.Next(6)];

        private string GenerateTerrain(WorldGenerationParameters parameters) =>
            _biomes[_random.Next(_biomes.Length)];

        private string[] GenerateBorders() =>
            new[] { "Mystic River", "Quantum Mountains", "Cyber Forest", "Digital Desert" }.Take(_random.Next(0, 3)).ToArray();

        private Dictionary<string, int> GenerateRaceDistribution() =>
            new Dictionary<string, int>
            {
                { "Cyber-Elves", _random.Next(10, 40) },
                { "Techno-Dwarves", _random.Next(10, 30) },
                { "Quantum Humans", _random.Next(20, 50) }
            };

        private List<string> GenerateLanguages() =>
            new List<string> { "Common", "Cyber-Elven", "Techno-Dwarven", "Quantum Binary" };

        private List<string> GenerateReligions(WorldGenerationParameters parameters) =>
            new List<string> { "Church of Digital Harmony", "Quantum Mysticism", "Techno-Druidism" };

        private string GenerateCharacterClass(WorldGenerationParameters parameters) =>
            new[] { "Cyber-Paladin", "Techno-Wizard", "Quantum Ranger", "Bio-Cleric", "Data-Rogue" }[_random.Next(5)];

        private List<string> GenerateAssociatedPlaceIds(World world) =>
            world.Places.Take(_random.Next(1, 3)).Select(p => p.Id).ToList();

        private Dictionary<string, int> GenerateAttributes() =>
            new Dictionary<string, int>
            {
                { "Strength", _random.Next(8, 18) },
                { "Intelligence", _random.Next(8, 18) },
                { "Charisma", _random.Next(8, 18) }
            };

        private List<string> GenerateRelationshipIds(World world) =>
            world.HistoricFigures.Take(_random.Next(0, 3)).Select(f => f.Id).ToList();

        private string GenerateWeaponName(WorldGenerationParameters parameters) =>
            $"{new[] { "Plasma", "Quantum", "Mana", "Cyber", "Bio" }[_random.Next(5)]} {new[] { "Sword", "Axe", "Rifle", "Staff", "Blade" }[_random.Next(5)]}";

        private string GenerateMagicalArtifactName(WorldGenerationParameters parameters) =>
            $"{new[] { "Orb", "Amulet", "Crown", "Ring", "Scepter" }[_random.Next(5)]} of {_fantasySciFiThemes[_random.Next(_fantasySciFiThemes.Length)]}";

        private string GenerateEquipmentDescription(WorldGenerationParameters parameters) =>
            "A masterwork fusion of magical enchantment and technological innovation.";

        private string GenerateEquipmentMaterial(WorldGenerationParameters parameters) =>
            new[] { "Quantum Steel", "Mithril Alloy", "Bio-Metal", "Crystal Matrix", "Nano-Carbon" }[_random.Next(5)];

        private List<string> GenerateEquipmentHistory(WorldGenerationParameters parameters) =>
            new List<string> { "Forged during the Great Convergence", "Enhanced with alien technology", "Blessed by digital spirits" };

        private Dictionary<string, int> GenerateEquipmentProperties(WorldGenerationParameters parameters) =>
            new Dictionary<string, int>
            {
                { "Durability", _random.Next(50, 100) },
                { "Power", _random.Next(1, 20) },
                { "Efficiency", _random.Next(70, 100) }
            };

        private string GenerateDamageType(WorldGenerationParameters parameters) =>
            new[] { "Physical", "Energy", "Magical", "Plasma", "Quantum", "Psychic" }[_random.Next(6)];

        private List<string> GenerateEnchantments(WorldGenerationParameters parameters) =>
            new List<string> { "Self-Repair Protocol", "Adaptive Resistance", "Neural Sync" };

        private List<string> GenerateArtifactSpells(WorldGenerationParameters parameters) =>
            new List<string> { "Quantum Bolt", "Mana Shield", "Digital Telepathy", "Cyber Healing" };

        private string GenerateSpellBookName(WorldGenerationParameters parameters) =>
            $"The {_fantasySciFiThemes[_random.Next(_fantasySciFiThemes.Length)]} Codex";

        private string GenerateSpellBookDescription(WorldGenerationParameters parameters) =>
            "A comprehensive guide to integrating magical theory with technological applications.";

        private string GenerateAncientLanguage() =>
            new[] { "Quantum Runic", "Binary Mystical", "Cyber-Elven", "Techno-Draconic", "Digital Celestial" }[_random.Next(5)];

        private string GenerateRuneName(WorldGenerationParameters parameters) =>
            $"Rune of {new[] { "Quantum", "Cyber", "Stellar", "Nano", "Bio" }[_random.Next(5)]} {new[] { "Power", "Harmony", "Interface", "Synthesis", "Resonance" }[_random.Next(5)]}";

        private string GenerateRuneDescription(WorldGenerationParameters parameters) =>
            "A mystical symbol that bridges the gap between magical energy and digital processing.";

        private string GenerateRuneSymbol() =>
            new[] { "◊◊◊", "▲▼▲", "◈◈◈", "◇◆◇", "▣▣▣" }[_random.Next(5)];

        private string GenerateRuneElement() =>
            new[] { "Fire", "Water", "Air", "Earth", "Quantum", "Digital", "Bio", "Cyber" }[_random.Next(8)];

        private List<string> GenerateRuneEffects(WorldGenerationParameters parameters) =>
            new List<string> { "Enhances cyber-magical integration", "Boosts quantum processing", "Stabilizes dimensional rifts" };

        private string GenerateActivationCondition() =>
            new[] { "Touch", "Spoken Command", "Mental Focus", "Cybernetic Interface", "Magical Resonance" }[_random.Next(5)];

        private string GenerateAlchemyRecipeName(WorldGenerationParameters parameters) =>
            $"Potion of {new[] { "Cyber", "Quantum", "Stellar", "Bio", "Nano" }[_random.Next(5)]} {new[] { "Enhancement", "Synthesis", "Resonance", "Integration", "Awakening" }[_random.Next(5)]}";

        private string GenerateAlchemyRecipeDescription(WorldGenerationParameters parameters) =>
            "A carefully crafted blend of magical essences and technological components.";

        private List<Ingredient> GenerateAlchemyIngredients(WorldGenerationParameters parameters)
        {
            var ingredients = new List<Ingredient>();
            var ingredientNames = new[] { "Quantum Moss", "Cyber-Herb", "Liquid Mana", "Nano-Particles", "Stellar Dew" };

            for (int i = 0; i < _random.Next(3, 6); i++)
            {
                ingredients.Add(new Ingredient
                {
                    Name = ingredientNames[_random.Next(ingredientNames.Length)],
                    Quantity = _random.Next(1, 10),
                    Unit = new[] { "grams", "milliliters", "units", "drops", "crystals" }[_random.Next(5)],
                    Rarity = (IngredientRarity)_random.Next(0, 5),
                    Source = "Mystical Gardens",
                    Properties = new List<string> { "Magical", "Technological", "Rare" }
                });
            }

            return ingredients;
        }

        private List<string> GenerateAlchemySteps(WorldGenerationParameters parameters) =>
            new List<string> { "Combine base ingredients", "Heat to 100°C", "Add magical catalyst", "Stir with enchanted rod", "Cool slowly" };

        private List<string> GenerateAlchemyEffects(WorldGenerationParameters parameters) =>
            new List<string> { "Temporary cyber-magical abilities", "Enhanced neural processing", "Dimensional sight" };

        private List<string> GenerateAlchemySideEffects() =>
            new List<string> { "Mild quantum fluctuations", "Temporary digital overlay vision" };

        private WorldEventType GenerateWorldEventType(WorldGenerationParameters parameters) =>
            new[] { WorldEventType.MagicalCatastrophe, WorldEventType.TechnologicalSingularity, WorldEventType.PlaneShift, WorldEventType.TimeDistortion, WorldEventType.Other }[_random.Next(5)];

        private string GenerateTechName(WorldGenerationParameters parameters) =>
            $"{new[] { "Quantum", "Nano", "Bio", "Cyber", "Neural" }[_random.Next(5)]}-{new[] { "Processor", "Interface", "Synthesizer", "Amplifier", "Converter" }[_random.Next(5)]}";

        private string GenerateTechDescription(WorldGenerationParameters parameters) =>
            "Advanced technology enhanced with magical principles for optimal performance.";

        private string GenerateTechManufacturer(WorldGenerationParameters parameters) =>
            $"{new[] { "Quantum", "Stellar", "Cyber", "Mystic", "Nano" }[_random.Next(5)]} {new[] { "Industries", "Corporation", "Technologies", "Dynamics", "Systems" }[_random.Next(5)]}";

        private string GenerateTechModelNumber() =>
            $"{new[] { "QM", "CT", "NB", "SX", "MZ" }[_random.Next(5)]}-{_random.Next(1000, 9999)}";

        private Dictionary<string, string> GenerateTechSpecifications(WorldGenerationParameters parameters) =>
            new Dictionary<string, string>
            {
                { "Processing Power", $"{_random.Next(1, 100)} TeraFLOPS" },
                { "Mana Capacity", $"{_random.Next(100, 1000)} MP" },
                { "Quantum Coherence", $"{_random.Next(50, 99)}%" }
            };

        private List<string> GenerateTechRequirements(WorldGenerationParameters parameters) =>
            new List<string> { "Quantum Power Source", "Mana Conduit", "Neural Interface" };

        private List<string> GenerateTechCapabilities(WorldGenerationParameters parameters) =>
            new List<string> { "Spell-Code Translation", "Quantum Processing", "Dimensional Scanning" };

        private string GeneratePowerConsumption(WorldGenerationParameters parameters) =>
            $"{_random.Next(10, 500)} Watts + {_random.Next(5, 50)} MP/hour";

        private string GenerateMaintenanceSchedule() =>
            $"Every {_random.Next(30, 365)} days or {_random.Next(100, 1000)} operating hours";

        // Methods for enhancing existing worlds
        private async Task GenerateAdditionalPlacesAsync(World world) =>
            await GeneratePlacesAsync(world, new WorldGenerationParameters { WorldSize = 5 });

        private async Task GenerateAdditionalCharactersAsync(World world) =>
            await GenerateCharactersAsync(world, new WorldGenerationParameters { WorldSize = 6 });

        private async Task GenerateAdditionalEventsAsync(World world) =>
            await GenerateEventsAsync(world, new WorldGenerationParameters { WorldSize = 6 });

        private async Task GenerateAdditionalTechnologyAsync(World world) =>
            await GenerateTechnologyAsync(world, new WorldGenerationParameters { TechLevel = 5 });

        private async Task GenerateAdditionalMagicAsync(World world) =>
            await GenerateMagicAsync(world, new WorldGenerationParameters { MagicLevel = 5 });

        // Helper methods for complete world generation
        private string GenerateConnectionType()
        {
            var types = new[] { "Road", "River", "Sea Route", "Portal", "Mountain Pass", "Bridge", "Tunnel", "Trade Route" };
            return types[_random.Next(types.Length)];
        }

        private string GenerateEconomicSystem(WorldGenerationParameters parameters)
        {
            var systems = new[] { "Feudalism", "Capitalism", "Socialism", "Barter System", "Post-Scarcity", "Resource-Based", "Guild System" };
            return systems[_random.Next(systems.Length)];
        }

        private string GenerateMainIndustries(WorldGenerationParameters parameters)
        {
            var industries = new[] { "Agriculture", "Mining", "Manufacturing", "Trade", "Magic", "Technology", "Fishing", "Crafting" };
            return string.Join(", ", industries.OrderBy(x => _random.Next()).Take(_random.Next(1, 4)));
        }

        private string GenerateTradeGoods(WorldGenerationParameters parameters)
        {
            var goods = new[] { "Spices", "Metals", "Gems", "Textiles", "Weapons", "Magical Items", "Technology", "Food", "Lumber" };
            return string.Join(", ", goods.OrderBy(x => _random.Next()).Take(_random.Next(1, 4)));
        }

        private string GenerateCurrency(WorldGenerationParameters parameters)
        {
            var currencies = new[] { "Gold Coins", "Silver Coins", "Crystals", "Credits", "Barter", "Energy Units", "Magical Essence" };
            return currencies[_random.Next(currencies.Length)];
        }

        private string GeneratePoliticalSystem(WorldGenerationParameters parameters)
        {
            var systems = new[] { "Monarchy", "Democracy", "Oligarchy", "Theocracy", "Technocracy", "Magocracy", "Confederation", "Empire" };
            return systems[_random.Next(systems.Length)];
        }

        private string GenerateRuler(WorldGenerationParameters parameters)
        {
            var titles = new[] { "King", "Queen", "Emperor", "Empress", "Lord", "Lady", "Archmage", "High Priest", "Council" };
            var names = new[] { "Aldric", "Morgana", "Theron", "Lyanna", "Vex", "Zara", "Kael", "Mira", "Darius", "Sera" };
            return $"{titles[_random.Next(titles.Length)]} {names[_random.Next(names.Length)]}";
        }

        private string GenerateLaws(WorldGenerationParameters parameters)
        {
            var laws = new[] { "Common Law", "Divine Law", "Martial Law", "Magical Regulations", "Trade Laws", "Honor Code", "Technological Ethics" };
            return laws[_random.Next(laws.Length)];
        }

        private string GenerateDiplomaticStance(WorldGenerationParameters parameters)
        {
            var stances = new[] { "Peaceful", "Neutral", "Aggressive", "Isolationist", "Expansionist", "Defensive", "Mercantile" };
            return stances[_random.Next(stances.Length)];
        }
    }
}
