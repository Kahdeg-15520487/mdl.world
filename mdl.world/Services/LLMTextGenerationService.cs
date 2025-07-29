using System.Text;
using System.Text.Json;

namespace mdl.world.Services
{
    public class LLMTextGenerationService : ILLMTextGenerationService, IDisposable
    {
        private HttpClient _httpClient;
        private readonly ILogger<LLMTextGenerationService> _logger;
        private readonly IConfiguration _configuration;
        private string _baseUrl;
        private string _model;

        public LLMTextGenerationService(HttpClient httpClient, ILogger<LLMTextGenerationService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _baseUrl = _configuration["LLM:BaseUrl"] ?? "http://localhost:8080";
            _model = _configuration["LLM:Model"] ?? "local-model";

            // Configure HttpClient
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "mdl.world");
        }

        public async Task<string> GenerateTextFromJsonAsync(object jsonData, string prompt)
        {
            try
            {
                // Check if service is available before attempting generation
                if (!await IsServiceAvailableAsync())
                {
                    _logger.LogWarning("LLM service is not available. Returning fallback message.");
                    return "LLM service is currently unavailable. Please check the service connection and try again.";
                }

                var jsonString = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });

                var fullPrompt = $"{prompt}\n\nJSON Data:\n{jsonString}\n\nPlease generate a descriptive narrative based on this data:";

                var request = new LLMRequest
                {
                    Model = _model,
                    Messages = new List<LLMMessage>
                    {
                        new LLMMessage { Role = "system", Content = "You are a creative writer specializing in fantasy and sci-fi world building. Generate vivid, engaging descriptions based on the provided JSON data." },
                        new LLMMessage { Role = "user", Content = fullPrompt }
                    },
                    Temperature = 0.8f,
                    MaxTokens = 1000
                };

                return await SendLLMRequestAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating text from JSON");
                return "Unable to generate description at this time. Please check the LLM service connection.";
            }
        }

        public async Task<string> GenerateWorldNarrativeAsync(object worldJson)
        {
            var prompt = @"Create a compelling narrative description of this fantasy/sci-fi world. 
Include details about the setting, atmosphere, key locations, and what makes this world unique. 
Write in an engaging, immersive style that would draw readers into this world.";

            return await GenerateTextFromJsonAsync(worldJson, prompt);
        }

        public async Task<string> GenerateCharacterDescriptionAsync(object characterJson)
        {
            var prompt = @"Create a detailed character description based on the provided data. 
Include their physical appearance, personality traits, background, and what makes them memorable. 
Write in a narrative style that brings this character to life.";

            return await GenerateTextFromJsonAsync(characterJson, prompt);
        }

        public async Task<string> GenerateLocationDescriptionAsync(object placeJson)
        {
            var prompt = @"Create an immersive description of this location. 
Include sensory details about what someone would see, hear, smell, and feel when visiting this place. 
Describe the atmosphere, architecture, inhabitants, and any unique features that make this location special.";

            return await GenerateTextFromJsonAsync(placeJson, prompt);
        }

        public async Task<string> GenerateEventNarrativeAsync(object eventJson)
        {
            var prompt = @"Create a compelling narrative description of this event. 
Tell the story of what happened, who was involved, and the impact it had. 
Write in an engaging storytelling style that captures the drama and significance of the event.";

            return await GenerateTextFromJsonAsync(eventJson, prompt);
        }

        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                var health = await GetServiceHealthAsync();
                return health.IsAvailable;
            }
            catch
            {
                return false;
            }
        }

        public async Task<LLMServiceHealth> GetServiceHealthAsync()
        {
            var health = new LLMServiceHealth
            {
                BaseUrl = _baseUrl,
                Model = _model,
                CheckedAt = DateTime.UtcNow
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Create a simple health check request
                var healthRequest = new LLMRequest
                {
                    Model = _model,
                    Messages = new List<LLMMessage>
                    {
                        new LLMMessage { Role = "user", Content = "ping" }
                    },
                    Temperature = 0.1f,
                    MaxTokens = 10
                };

                var jsonContent = JsonSerializer.Serialize(healthRequest, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Set a shorter timeout for health checks
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _httpClient.PostAsync("/v1/chat/completions", httpContent, cts.Token);

                stopwatch.Stop();
                health.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;

                if (response.IsSuccessStatusCode)
                {
                    health.IsAvailable = true;
                    health.Status = "Healthy";
                }
                else
                {
                    health.IsAvailable = false;
                    health.Status = $"Unhealthy - HTTP {response.StatusCode}";
                    health.ErrorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                stopwatch.Stop();
                health.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                health.IsAvailable = false;
                health.Status = "Timeout";
                health.ErrorMessage = "Service did not respond within timeout period";
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                health.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                health.IsAvailable = false;
                health.Status = "Connection Error";
                health.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                health.ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds;
                health.IsAvailable = false;
                health.Status = "Error";
                health.ErrorMessage = ex.Message;
            }

            return health;
        }

        private async Task<string> SendLLMRequestAsync(LLMRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });

                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending request to LLM server at {BaseUrl}", _baseUrl);

                var response = await _httpClient.PostAsync("/v1/chat/completions", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var llmResponse = JsonSerializer.Deserialize<LLMResponse>(responseContent, new JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                    });

                    if (llmResponse?.Choices?.Count > 0)
                    {
                        return llmResponse.Choices[0].Message.Content;
                    }
                }
                else
                {
                    _logger.LogError("LLM server returned error: {StatusCode} - {Content}", 
                        response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error connecting to LLM server");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing LLM response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling LLM service");
            }

            return "Unable to generate text at this time. Please check the LLM server connection.";
        }

        public void UpdateConfiguration(string baseUrl, string? model = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));
                }

                if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
                {
                    throw new ArgumentException("Invalid URL format", nameof(baseUrl));
                }

                _baseUrl = baseUrl;
                if (!string.IsNullOrWhiteSpace(model))
                {
                    _model = model;
                }

                // Create a new HttpClient instance with updated configuration
                _httpClient?.Dispose();
                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri(_baseUrl);
                _httpClient.Timeout = TimeSpan.FromSeconds(30);

                _logger.LogInformation("LLM service configuration updated - BaseUrl: {BaseUrl}, Model: {Model}", _baseUrl, _model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating LLM service configuration");
                throw;
            }
        }

        public LLMServiceConfig GetConfiguration()
        {
            return new LLMServiceConfig
            {
                BaseUrl = _baseUrl,
                Model = _model
            };
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
