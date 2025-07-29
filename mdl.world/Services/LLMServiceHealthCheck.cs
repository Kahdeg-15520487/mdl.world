using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace mdl.world.Services
{
    public class LLMServiceHealthCheck : IHealthCheck
    {
        private readonly ILLMTextGenerationService _llmService;
        private readonly ILogger<LLMServiceHealthCheck> _logger;

        public LLMServiceHealthCheck(ILLMTextGenerationService llmService, ILogger<LLMServiceHealthCheck> logger)
        {
            _llmService = llmService;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var serviceHealth = await _llmService.GetServiceHealthAsync();
                
                if (serviceHealth.IsAvailable)
                {
                    var data = new Dictionary<string, object>
                    {
                        { "BaseUrl", serviceHealth.BaseUrl },
                        { "Model", serviceHealth.Model },
                        { "ResponseTimeMs", serviceHealth.ResponseTimeMs },
                        { "CheckedAt", serviceHealth.CheckedAt }
                    };
                    
                    return HealthCheckResult.Healthy(serviceHealth.Status, data);
                }
                else
                {
                    var data = new Dictionary<string, object>
                    {
                        { "BaseUrl", serviceHealth.BaseUrl },
                        { "Model", serviceHealth.Model },
                        { "ResponseTimeMs", serviceHealth.ResponseTimeMs },
                        { "ErrorMessage", serviceHealth.ErrorMessage ?? "Unknown error" },
                        { "CheckedAt", serviceHealth.CheckedAt }
                    };
                    
                    return HealthCheckResult.Unhealthy(serviceHealth.Status, data: data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during LLM service health check");
                
                var data = new Dictionary<string, object>
                {
                    { "ErrorMessage", ex.Message },
                    { "CheckedAt", DateTime.UtcNow }
                };
                
                return HealthCheckResult.Unhealthy("Health check failed", ex, data);
            }
        }
    }
}
