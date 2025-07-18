using Microsoft.AspNetCore.Mvc;
using mdl.worlddata.Core;
using mdl.world.Services;
using System.Text.Json;

namespace mdl.world.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorldController : ControllerBase
    {
        private readonly ILogger<WorldController> _logger;
        private readonly IWorldGenerationService _worldGenerationService;
        private readonly IWorldStorageService _worldStorageService;

        public WorldController(ILogger<WorldController> logger, IWorldGenerationService worldGenerationService, IWorldStorageService worldStorageService)
        {
            _logger = logger;
            _worldGenerationService = worldGenerationService;
            _worldStorageService = worldStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorldMetadata>>> Get()
        {
            try
            {
                var worlds = await _worldStorageService.GetAllWorldsAsync();
                return Ok(worlds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving worlds from storage");
                return StatusCode(500, "An error occurred while retrieving worlds");
            }
        }

        [HttpGet("{worldId}")]
        public async Task<ActionResult<World>> GetWorld(string worldId)
        {
            try
            {
                var world = await _worldStorageService.LoadWorldAsync(worldId);
                if (world == null)
                {
                    return NotFound($"World with ID {worldId} not found");
                }
                return Ok(world);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving world: {WorldId}", worldId);
                return StatusCode(500, "An error occurred while retrieving the world");
            }
        }

        [HttpDelete("{worldId}")]
        public async Task<ActionResult> DeleteWorld(string worldId)
        {
            try
            {
                var deleted = await _worldStorageService.DeleteWorldAsync(worldId);
                if (!deleted)
                {
                    return NotFound($"World with ID {worldId} not found");
                }
                return Ok($"World {worldId} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting world: {WorldId}", worldId);
                return StatusCode(500, "An error occurred while deleting the world");
            }
        }

        [HttpPost("generate")]
        public async Task<ActionResult<World>> GenerateWorld([FromBody] WorldGenerationRequest request)
        {
            try
            {
                var world = await _worldGenerationService.GenerateWorldAsync(
                    request.WorldName, 
                    request.Theme, 
                    request.TechLevel, 
                    request.MagicLevel
                );
                
                // Automatically save the generated world
                var savedWorld = await _worldStorageService.SaveWorldAsync(world);
                
                return Ok(savedWorld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating world: {WorldName}", request.WorldName);
                return StatusCode(500, "An error occurred while generating the world");
            }
        }

        [HttpPost("generate-custom")]
        public async Task<ActionResult<World>> GenerateCustomWorld([FromBody] WorldGenerationParameters parameters)
        {
            try
            {
                var world = await _worldGenerationService.GenerateCustomWorldAsync(parameters);
                
                // Automatically save the generated world
                var savedWorld = await _worldStorageService.SaveWorldAsync(world);
                
                return Ok(savedWorld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating custom world: {WorldName}", parameters.WorldName);
                return StatusCode(500, "An error occurred while generating the custom world");
            }
        }

        [HttpPost("{worldId}/enhance")]
        public async Task<ActionResult<World>> EnhanceWorld(string worldId, [FromBody] EnhanceWorldRequest request)
        {
            try
            {
                // First try to load existing world from storage
                var world = await _worldStorageService.LoadWorldAsync(worldId);
                
                if (world == null)
                {
                    // If not found, generate a new world using the worldId as the name
                    world = await _worldGenerationService.GenerateWorldAsync(
                        request.WorldName ?? worldId);
                }
                
                // Use the new WorldEnhancementService to enhance the world
                var enhancementService = HttpContext.RequestServices.GetService<IWorldEnhancementService>();
                if (enhancementService != null)
                {
                    var enhancementResult = await enhancementService.EnhanceWorldAsync(
                        world, 
                        $"Enhance this world with focus on {request.ContentType}");
                    
                    // Save the enhanced world
                    var savedWorld = await _worldStorageService.SaveWorldAsync(enhancementResult.UpdatedWorld);
                    
                    return Ok(savedWorld);
                }
                
                // Fallback to basic enhancement if WorldEnhancementService not available
                var enhancedWorld = await _worldGenerationService.EnhanceWorldAsync(world, request.ContentType);
                
                // Save the enhanced world
                var savedEnhancedWorld = await _worldStorageService.SaveWorldAsync(enhancedWorld);
                
                return Ok(savedEnhancedWorld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enhancing world: {WorldId}", worldId);
                return StatusCode(500, "An error occurred while enhancing the world");
            }
        }

        [HttpGet("themes")]
        public ActionResult<IEnumerable<string>> GetAvailableThemes()
        {
            var themes = new[]
            {
                "Fantasy-SciFi",
                "Cyberpunk-Fantasy",
                "Space-Magic",
                "Bio-Magical",
                "Quantum-Mystical",
                "Steampunk-Arcane",
                "Digital-Shamanism",
                "Techno-Druidism"
            };
            
            return Ok(themes);
        }

        [HttpGet("templates")]
        public ActionResult<IEnumerable<WorldTemplate>> GetWorldTemplates()
        {
            var templates = new[]
            {
                new WorldTemplate { Name = "Magitech Empire", Theme = "Fantasy-SciFi", TechLevel = 8, MagicLevel = 7, Description = "A realm where magic and technology have merged into a unified force" },
                new WorldTemplate { Name = "Cyber-Mystical Realm", Theme = "Cyberpunk-Fantasy", TechLevel = 9, MagicLevel = 6, Description = "A world where ancient magic meets cutting-edge cybernetics" },
                new WorldTemplate { Name = "Quantum Spellcaster Society", Theme = "Quantum-Mystical", TechLevel = 10, MagicLevel = 9, Description = "A reality where quantum mechanics and magical theory intertwine" },
                new WorldTemplate { Name = "Bio-Magical Synthesis", Theme = "Bio-Magical", TechLevel = 7, MagicLevel = 8, Description = "A world where biological enhancement and magical evolution go hand in hand" },
                new WorldTemplate { Name = "Stellar Kingdoms", Theme = "Space-Magic", TechLevel = 9, MagicLevel = 7, Description = "Interstellar empires powered by both technology and cosmic magic" }
            };
            
            return Ok(templates);
        }

        [HttpPost("generate-complete")]
        public async Task<ActionResult<World>> GenerateCompleteWorld([FromBody] CompleteWorldRequest request)
        {
            try
            {
                var parameters = new WorldGenerationParameters
                {
                    WorldName = request.WorldName,
                    Theme = request.Theme,
                    TechLevel = request.TechLevel,
                    MagicLevel = request.MagicLevel,
                    WorldSize = request.TotalPlaces,
                    IncludeMagicTech = request.IncludeMagicTech,
                    IncludeSpaceTravel = request.IncludeSpaceTravel,
                    IncludeAncientRuins = request.IncludeAncientRuins,
                    PreferredBiomes = request.PreferredBiomes ?? new List<string>(),
                    PreferredRaces = request.PreferredRaces ?? new List<string>(),
                    DifficultyLevel = request.DifficultyLevel
                };

                var world = await _worldGenerationService.GenerateCompleteWorldAsync(parameters, request);
                
                // Automatically save the generated world
                var savedWorld = await _worldStorageService.SaveWorldAsync(world);
                
                return Ok(savedWorld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating complete world: {WorldName}", request.WorldName);
                return StatusCode(500, "An error occurred while generating the complete world");
            }
        }

        [HttpGet("{worldId}/wiki")]
        public async Task<ActionResult> GetWorldWiki(string worldId)
        {
            try
            {
                // Try to load from storage first
                var world = await _worldStorageService.LoadWorldAsync(worldId);
                
                // If not found, generate a sample world
                if (world == null)
                {
                    world = await _worldGenerationService.GenerateWorldAsync(worldId, "Fantasy-SciFi", 6, 8);
                    // Optionally save it for future reference
                    await _worldStorageService.SaveWorldAsync(world);
                }
                
                var htmlRenderingService = HttpContext.RequestServices.GetService<IWorldHtmlRenderingService>();
                if (htmlRenderingService == null)
                {
                    return StatusCode(500, "HTML rendering service not available");
                }

                var html = await htmlRenderingService.RenderWorldAsHtmlAsync(world);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating world wiki: {WorldId}", worldId);
                return StatusCode(500, "An error occurred while generating the world wiki");
            }
        }

        [HttpGet("{worldId}/wiki/place/{placeId}")]
        public async Task<ActionResult> GetPlaceWiki(string worldId, string placeId)
        {
            try
            {
                // Try to load from storage first
                var world = await _worldStorageService.LoadWorldAsync(worldId);
                
                // If not found, generate a sample world
                if (world == null)
                {
                    world = await _worldGenerationService.GenerateWorldAsync(worldId, "Fantasy-SciFi", 6, 8);
                    await _worldStorageService.SaveWorldAsync(world);
                }
                
                var htmlRenderingService = HttpContext.RequestServices.GetService<IWorldHtmlRenderingService>();
                if (htmlRenderingService == null)
                {
                    return StatusCode(500, "HTML rendering service not available");
                }

                var html = await htmlRenderingService.RenderPlaceAsHtmlAsync(world, placeId);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating place wiki: {WorldId}/{PlaceId}", worldId, placeId);
                return StatusCode(500, "An error occurred while generating the place wiki");
            }
        }

        [HttpGet("{worldId}/wiki/character/{characterId}")]
        public async Task<ActionResult> GetCharacterWiki(string worldId, string characterId)
        {
            try
            {
                // Try to load from storage first
                var world = await _worldStorageService.LoadWorldAsync(worldId);
                
                // If not found, generate a sample world
                if (world == null)
                {
                    world = await _worldGenerationService.GenerateWorldAsync(worldId, "Fantasy-SciFi", 6, 8);
                    await _worldStorageService.SaveWorldAsync(world);
                }
                
                var htmlRenderingService = HttpContext.RequestServices.GetService<IWorldHtmlRenderingService>();
                if (htmlRenderingService == null)
                {
                    return StatusCode(500, "HTML rendering service not available");
                }

                var html = await htmlRenderingService.RenderCharacterAsHtmlAsync(world, characterId);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating character wiki: {WorldId}/{CharacterId}", worldId, characterId);
                return StatusCode(500, "An error occurred while generating the character wiki");
            }
        }

        [HttpGet("{worldId}/wiki/item/{itemId}")]
        public async Task<ActionResult> GetItemWiki(string worldId, string itemId)
        {
            try
            {
                // Try to load from storage first
                var world = await _worldStorageService.LoadWorldAsync(worldId);
                
                // If not found, generate a sample world
                if (world == null)
                {
                    world = await _worldGenerationService.GenerateWorldAsync(worldId, "Fantasy-SciFi", 6, 8);
                    await _worldStorageService.SaveWorldAsync(world);
                }
                
                var htmlRenderingService = HttpContext.RequestServices.GetService<IWorldHtmlRenderingService>();
                if (htmlRenderingService == null)
                {
                    return StatusCode(500, "HTML rendering service not available");
                }

                var html = await htmlRenderingService.RenderItemAsHtmlAsync(world, itemId);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating item wiki: {WorldId}/{ItemId}", worldId, itemId);
                return StatusCode(500, "An error occurred while generating the item wiki");
            }
        }

        [HttpPut("{worldId}")]
        public async Task<ActionResult<World>> UpdateWorld(string worldId, [FromBody] World world)
        {
            try
            {
                // Ensure the world ID matches
                world.Id = worldId;
                
                var savedWorld = await _worldStorageService.SaveWorldAsync(world);
                return Ok(savedWorld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating world: {WorldId}", worldId);
                return StatusCode(500, "An error occurred while updating the world");
            }
        }

        [HttpPost("{worldId}/copy")]
        public async Task<ActionResult<World>> CopyWorld(string worldId, [FromBody] CopyWorldRequest request)
        {
            try
            {
                var originalWorld = await _worldStorageService.LoadWorldAsync(worldId);
                if (originalWorld == null)
                {
                    return NotFound($"World with ID {worldId} not found");
                }

                // Create a copy with a new ID and name
                var copiedWorld = new World
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = request.NewName ?? $"{originalWorld.Name} - Copy",
                    Description = originalWorld.Description,
                    CreationDate = DateTime.UtcNow,
                    WorldInfo = originalWorld.WorldInfo,
                    Places = originalWorld.Places,
                    HistoricFigures = originalWorld.HistoricFigures,
                    WorldEvents = originalWorld.WorldEvents,
                    Equipment = originalWorld.Equipment,
                    SpellBooks = originalWorld.SpellBooks,
                    RunesOfPower = originalWorld.RunesOfPower,
                    AlchemyRecipes = originalWorld.AlchemyRecipes,
                    TechnicalSpecs = originalWorld.TechnicalSpecs
                };

                var savedWorld = await _worldStorageService.SaveWorldAsync(copiedWorld);
                return Ok(savedWorld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying world: {WorldId}", worldId);
                return StatusCode(500, "An error occurred while copying the world");
            }
        }

        [HttpGet("{worldId}/export")]
        public async Task<ActionResult> ExportWorld(string worldId)
        {
            try
            {
                var world = await _worldStorageService.LoadWorldAsync(worldId);
                if (world == null)
                {
                    return NotFound($"World with ID {worldId} not found");
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(world, jsonOptions);
                var fileName = $"{world.Name.Replace(" ", "_")}_export.json";
                
                return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting world: {WorldId}", worldId);
                return StatusCode(500, "An error occurred while exporting the world");
            }
        }

        [HttpPost("import")]
        public async Task<ActionResult<World>> ImportWorld([FromBody] World world)
        {
            try
            {
                // Ensure the world has a unique ID
                if (await _worldStorageService.WorldExistsAsync(world.Id))
                {
                    world.Id = Guid.NewGuid().ToString();
                }

                var savedWorld = await _worldStorageService.SaveWorldAsync(world);
                return Ok(savedWorld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing world: {WorldName}", world.Name);
                return StatusCode(500, "An error occurred while importing the world");
            }
        }
    }

    public class WorldGenerationRequest
    {
        public string WorldName { get; set; } = string.Empty;
        public string Theme { get; set; } = "Fantasy-SciFi";
        public int TechLevel { get; set; } = 5;
        public int MagicLevel { get; set; } = 7;
    }

    public class EnhanceWorldRequest
    {
        public string? WorldName { get; set; }
        public string ContentType { get; set; } = "all";
    }

    public class CompleteWorldRequest
    {
        public string WorldName { get; set; } = string.Empty;
        public string Theme { get; set; } = "Fantasy-SciFi";
        public int TechLevel { get; set; } = 5;
        public int MagicLevel { get; set; } = 7;
        public int TotalPlaces { get; set; } = 50;
        public int ContinentCount { get; set; } = 3;
        public int CountryCount { get; set; } = 8;
        public int RegionCount { get; set; } = 15;
        public int CityCount { get; set; } = 20;
        public int TownCount { get; set; } = 25;
        public int VillageCount { get; set; } = 30;
        public int DungeonCount { get; set; } = 12;
        public int NaturalFeatureCount { get; set; } = 18;
        public int CharacterCount { get; set; } = 25;
        public int HistoricalEventCount { get; set; } = 15;
        public int EquipmentCount { get; set; } = 40;
        public int SpellBookCount { get; set; } = 8;
        public int RuneCount { get; set; } = 12;
        public int AlchemyRecipeCount { get; set; } = 10;
        public int TechnologyCount { get; set; } = 15;
        public bool IncludeMagicTech { get; set; } = true;
        public bool IncludeSpaceTravel { get; set; } = false;
        public bool IncludeAncientRuins { get; set; } = true;
        public bool GenerateHierarchy { get; set; } = true;
        public bool GenerateConnections { get; set; } = true;
        public bool GenerateEconomy { get; set; } = true;
        public bool GeneratePolitics { get; set; } = true;
        public List<string>? PreferredBiomes { get; set; }
        public List<string>? PreferredRaces { get; set; }
        public List<string>? PreferredTechnologies { get; set; }
        public List<string>? PreferredMagicSchools { get; set; }
        public string DifficultyLevel { get; set; } = "Medium";
        public string WorldScale { get; set; } = "Continental"; // Local, Regional, Continental, Global, Interplanetary
        public Dictionary<string, object>? CustomSettings { get; set; }
    }

    public class WorldTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
        public int TechLevel { get; set; }
        public int MagicLevel { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class CopyWorldRequest
    {
        public string? NewName { get; set; }
    }
}
