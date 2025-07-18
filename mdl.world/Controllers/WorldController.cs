using Microsoft.AspNetCore.Mvc;
using mdl.worlddata.Core;
using mdl.world.Services;

namespace mdl.world.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorldController : ControllerBase
    {
        private readonly ILogger<WorldController> _logger;
        private readonly IWorldGenerationService _worldGenerationService;

        public WorldController(ILogger<WorldController> logger, IWorldGenerationService worldGenerationService)
        {
            _logger = logger;
            _worldGenerationService = worldGenerationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<World>>> Get()
        {
            // This could be enhanced to return stored worlds
            var sampleWorld = await _worldGenerationService.GenerateWorldAsync("Sample World", "Fantasy-SciFi", 6, 8);
            return Ok(new[] { sampleWorld });
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
                
                return Ok(world);
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
                return Ok(world);
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
                // In a real implementation, you would retrieve the world from storage
                // For now, we'll generate a new world and enhance it
                var world = await _worldGenerationService.GenerateWorldAsync(request.WorldName ?? "Enhanced World");
                var enhancedWorld = await _worldGenerationService.EnhanceWorldAsync(world, request.ContentType);
                
                return Ok(enhancedWorld);
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
                return Ok(world);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating complete world: {WorldName}", request.WorldName);
                return StatusCode(500, "An error occurred while generating the complete world");
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
}
