using Microsoft.AspNetCore.Mvc;
using mdl.world.Services;
using mdl.worlddata.Core;
using System.Text.Json;

namespace mdl.world.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TextGenerationController : ControllerBase
    {
        private readonly ILLMTextGenerationService _llmService;
        private readonly IWorldGenerationService _worldGenerationService;
        private readonly ILogger<TextGenerationController> _logger;

        public TextGenerationController(
            ILLMTextGenerationService llmService, 
            IWorldGenerationService worldGenerationService,
            ILogger<TextGenerationController> logger)
        {
            _llmService = llmService;
            _worldGenerationService = worldGenerationService;
            _logger = logger;
        }

        [HttpPost("generate-from-json")]
        public async Task<IActionResult> GenerateFromJson([FromBody] GenerateTextRequest request)
        {
            try
            {
                var result = await _llmService.GenerateTextFromJsonAsync(request.JsonData, request.Prompt);
                return Ok(new { generatedText = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating text from JSON");
                return StatusCode(500, "An error occurred while generating text");
            }
        }

        [HttpPost("generate-world-narrative")]
        public async Task<IActionResult> GenerateWorldNarrative([FromBody] string worldName)
        {
            try
            {
                // Generate a world first
                var world = await _worldGenerationService.GenerateWorldAsync(worldName);
                
                // Generate narrative from the world data
                var narrative = await _llmService.GenerateWorldNarrativeAsync(world);
                
                return Ok(new { world = world, narrative = narrative });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating world narrative");
                return StatusCode(500, "An error occurred while generating world narrative");
            }
        }

        [HttpPost("generate-character-description")]
        public async Task<IActionResult> GenerateCharacterDescription([FromBody] object characterData)
        {
            try
            {
                var description = await _llmService.GenerateCharacterDescriptionAsync(characterData);
                return Ok(new { description = description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating character description");
                return StatusCode(500, "An error occurred while generating character description");
            }
        }

        [HttpPost("generate-location-description")]
        public async Task<IActionResult> GenerateLocationDescription([FromBody] object locationData)
        {
            try
            {
                var description = await _llmService.GenerateLocationDescriptionAsync(locationData);
                return Ok(new { description = description });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating location description");
                return StatusCode(500, "An error occurred while generating location description");
            }
        }

        [HttpPost("generate-event-narrative")]
        public async Task<IActionResult> GenerateEventNarrative([FromBody] object eventData)
        {
            try
            {
                var narrative = await _llmService.GenerateEventNarrativeAsync(eventData);
                return Ok(new { narrative = narrative });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating event narrative");
                return StatusCode(500, "An error occurred while generating event narrative");
            }
        }
    }

    public class GenerateTextRequest
    {
        public object JsonData { get; set; } = new();
        public string Prompt { get; set; } = "";
    }
}
