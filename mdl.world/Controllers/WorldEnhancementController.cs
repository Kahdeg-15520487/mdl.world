using Microsoft.AspNetCore.Mvc;
using mdl.world.Services;
using mdl.worlddata.Core;
using System.Text.Json;

namespace mdl.world.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorldEnhancementController : ControllerBase
    {
        private readonly IWorldEnhancementService _worldEnhancementService;
        private readonly IWorldGenerationService _worldGenerationService;
        private readonly ILogger<WorldEnhancementController> _logger;

        public WorldEnhancementController(
            IWorldEnhancementService worldEnhancementService,
            IWorldGenerationService worldGenerationService,
            ILogger<WorldEnhancementController> logger)
        {
            _worldEnhancementService = worldEnhancementService;
            _worldGenerationService = worldGenerationService;
            _logger = logger;
        }

        [HttpPost("enhance")]
        public async Task<IActionResult> EnhanceWorld([FromBody] WorldEnhancementRequest request)
        {
            try
            {
                if (request.World == null)
                {
                    return BadRequest("World data is required");
                }

                if (string.IsNullOrWhiteSpace(request.UserComment))
                {
                    return BadRequest("User comment is required");
                }

                var result = await _worldEnhancementService.EnhanceWorldAsync(
                    request.World, 
                    request.UserComment, 
                    request.TargetSection);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enhancing world");
                return StatusCode(500, "An error occurred while enhancing the world");
            }
        }

        [HttpPost("regenerate-section")]
        public async Task<IActionResult> RegenerateSection([FromBody] RegenerateRequest request)
        {
            try
            {
                if (request.World == null)
                {
                    return BadRequest("World data is required");
                }

                if (string.IsNullOrWhiteSpace(request.SectionType) || string.IsNullOrWhiteSpace(request.SectionId))
                {
                    return BadRequest("Section type and ID are required");
                }

                var result = await _worldEnhancementService.RegenerateWorldSectionAsync(
                    request.World,
                    request.SectionType,
                    request.SectionId,
                    request.UserComment);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating world section");
                return StatusCode(500, "An error occurred while regenerating the world section");
            }
        }

        [HttpPost("add-content")]
        public async Task<IActionResult> AddContent([FromBody] AddContentRequest request)
        {
            try
            {
                if (request.World == null)
                {
                    return BadRequest("World data is required");
                }

                if (string.IsNullOrWhiteSpace(request.ContentType) || string.IsNullOrWhiteSpace(request.Description))
                {
                    return BadRequest("Content type and description are required");
                }

                var result = await _worldEnhancementService.AddContentToWorldAsync(
                    request.World,
                    request.ContentType,
                    request.Description);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding content to world");
                return StatusCode(500, "An error occurred while adding content to the world");
            }
        }

        [HttpPost("generate-narrative")]
        public async Task<IActionResult> GenerateNarrative([FromBody] World world)
        {
            try
            {
                if (world == null)
                {
                    return BadRequest("World data is required");
                }

                var narrative = await _worldEnhancementService.GenerateWorldNarrativeAsync(world);

                return Ok(new { narrative = narrative });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating world narrative");
                return StatusCode(500, "An error occurred while generating the world narrative");
            }
        }

        [HttpPost("update-properties")]
        public async Task<IActionResult> UpdateProperties([FromBody] UpdatePropertiesRequest request)
        {
            try
            {
                if (request.World == null)
                {
                    return BadRequest("World data is required");
                }

                if (string.IsNullOrWhiteSpace(request.UserComment))
                {
                    return BadRequest("User comment is required");
                }

                var updatedWorld = await _worldEnhancementService.UpdateWorldPropertiesAsync(
                    request.World,
                    request.UserComment);

                return Ok(updatedWorld);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating world properties");
                return StatusCode(500, "An error occurred while updating world properties");
            }
        }

        [HttpPost("create-and-enhance")]
        public async Task<IActionResult> CreateAndEnhance([FromBody] CreateAndEnhanceRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.WorldName))
                {
                    return BadRequest("World name is required");
                }

                // Step 1: Generate initial world
                var world = await _worldGenerationService.GenerateWorldAsync(
                    request.WorldName,
                    request.Theme ?? "Fantasy-SciFi",
                    request.TechLevel ?? 5,
                    request.MagicLevel ?? 7);

                // Step 2: Enhance with user comment if provided
                if (!string.IsNullOrWhiteSpace(request.UserComment))
                {
                    var enhancementResult = await _worldEnhancementService.EnhanceWorldAsync(
                        world,
                        request.UserComment,
                        request.TargetSection);

                    return Ok(enhancementResult);
                }

                // Step 3: Generate narrative for the initial world
                var narrative = await _worldEnhancementService.GenerateWorldNarrativeAsync(world);

                return Ok(new WorldEnhancementResult
                {
                    UpdatedWorld = world,
                    GeneratedNarrative = narrative,
                    ChangesApplied = new List<string> { "Generated initial world" },
                    UserComment = request.UserComment ?? "Initial world generation"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating and enhancing world");
                return StatusCode(500, "An error occurred while creating and enhancing the world");
            }
        }

        [HttpGet("export/{worldId}")]
        public async Task<IActionResult> ExportWorld(string worldId)
        {
            try
            {
                // For now, we'll generate a sample world with the given ID name
                // In a real implementation, this would retrieve from a database
                var world = await _worldGenerationService.GenerateWorldAsync(
                    worldId, 
                    "Fantasy-SciFi", 
                    5, 
                    7);
                
                // Generate a comprehensive narrative for the world
                var narrative = await _worldEnhancementService.GenerateWorldNarrativeAsync(world);
                
                // Create export package
                var exportData = new
                {
                    WorldId = worldId,
                    ExportedAt = DateTime.UtcNow,
                    WorldData = world,
                    Narrative = narrative,
                    Summary = new
                    {
                        Places = world.Places.Count,
                        Characters = world.HistoricFigures.Count,
                        Events = world.WorldEvents.Count,
                        TechnicalSpecs = world.TechnicalSpecs.Count,
                        MagicItems = world.RunesOfPower.Count + world.SpellBooks.Count + world.AlchemyRecipes.Count
                    }
                };
                
                return Ok(exportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting world");
                return StatusCode(500, "An error occurred while exporting the world");
            }
        }
    }

    public class WorldEnhancementRequest
    {
        public World World { get; set; } = new();
        public string UserComment { get; set; } = string.Empty;
        public string? TargetSection { get; set; }
    }

    public class RegenerateRequest
    {
        public World World { get; set; } = new();
        public string SectionType { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string UserComment { get; set; } = string.Empty;
    }

    public class AddContentRequest
    {
        public World World { get; set; } = new();
        public string ContentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdatePropertiesRequest
    {
        public World World { get; set; } = new();
        public string UserComment { get; set; } = string.Empty;
    }

    public class CreateAndEnhanceRequest
    {
        public string WorldName { get; set; } = string.Empty;
        public string? Theme { get; set; }
        public int? TechLevel { get; set; }
        public int? MagicLevel { get; set; }
        public string? UserComment { get; set; }
        public string? TargetSection { get; set; }
    }
}
