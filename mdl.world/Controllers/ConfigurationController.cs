using Microsoft.AspNetCore.Mvc;
using mdl.world.Services;

namespace mdl.world.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILLMTextGenerationService _llmService;
        private readonly ILogger<ConfigurationController> _logger;

        public ConfigurationController(
            IConfiguration configuration, 
            ILLMTextGenerationService llmService,
            ILogger<ConfigurationController> logger)
        {
            _configuration = configuration;
            _llmService = llmService;
            _logger = logger;
        }

        [HttpGet("llm")]
        public IActionResult GetLLMConfig()
        {
            try
            {
                var currentConfig = _llmService.GetConfiguration();
                var config = new LLMConfigResponse
                {
                    BaseUrl = currentConfig.BaseUrl,
                    Model = currentConfig.Model
                };

                return Ok(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LLM configuration");
                return StatusCode(500, "An error occurred while retrieving LLM configuration");
            }
        }

        [HttpPost("llm")]
        public IActionResult UpdateLLMConfig([FromBody] UpdateLLMConfigRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.BaseUrl))
                {
                    return BadRequest("Base URL is required");
                }

                if (!Uri.TryCreate(request.BaseUrl, UriKind.Absolute, out _))
                {
                    return BadRequest("Invalid URL format");
                }

                // Update the LLM service configuration at runtime
                _llmService.UpdateConfiguration(request.BaseUrl, request.Model);

                // Return the updated configuration
                var config = _llmService.GetConfiguration();

                return Ok(new { message = "LLM configuration updated successfully", config });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating LLM configuration");
                return StatusCode(500, "An error occurred while updating LLM configuration");
            }
        }

        [HttpGet("llm/test")]
        public async Task<IActionResult> TestLLMConnection()
        {
            try
            {
                var health = await _llmService.GetServiceHealthAsync();
                return Ok(health);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing LLM connection");
                return StatusCode(500, "An error occurred while testing LLM connection");
            }
        }
    }

    public class LLMConfigResponse
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
    }

    public class UpdateLLMConfigRequest
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string? Model { get; set; }
    }
}
