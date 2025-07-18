using System.Text;
using System.Text.Json;

namespace mdl.world.Services
{
    public class LLMTextGenerationService : ILLMTextGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LLMTextGenerationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;
        private readonly string _model;

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
                return "Unable to generate description at this time.";
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
    }
}
