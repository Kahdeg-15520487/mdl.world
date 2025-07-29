using System.Text.Json;

namespace mdl.world.Services
{
    public interface ILLMTextGenerationService
    {
        /// <summary>
        /// Generates descriptive text from JSON data using a local LLM server
        /// </summary>
        /// <param name="jsonData">The JSON data to generate text from</param>
        /// <param name="prompt">Custom prompt to guide text generation</param>
        /// <returns>Generated text description</returns>
        Task<string> GenerateTextFromJsonAsync(object jsonData, string prompt);

        /// <summary>
        /// Generates a narrative description of a world from its JSON representation
        /// </summary>
        /// <param name="worldJson">The world data in JSON format</param>
        /// <returns>A narrative description of the world</returns>
        Task<string> GenerateWorldNarrativeAsync(object worldJson);

        /// <summary>
        /// Generates character descriptions from character JSON data
        /// </summary>
        /// <param name="characterJson">The character data in JSON format</param>
        /// <returns>A descriptive text about the character</returns>
        Task<string> GenerateCharacterDescriptionAsync(object characterJson);

        /// <summary>
        /// Generates location descriptions from place JSON data
        /// </summary>
        /// <param name="placeJson">The place data in JSON format</param>
        /// <returns>A descriptive text about the location</returns>
        Task<string> GenerateLocationDescriptionAsync(object placeJson);

        /// <summary>
        /// Generates event narratives from event JSON data
        /// </summary>
        /// <param name="eventJson">The event data in JSON format</param>
        /// <returns>A narrative description of the event</returns>
        Task<string> GenerateEventNarrativeAsync(object eventJson);

        /// <summary>
        /// Checks if the LLM service is available and responding
        /// </summary>
        /// <returns>True if the service is available, false otherwise</returns>
        Task<bool> IsServiceAvailableAsync();

        /// <summary>
        /// Gets detailed health information about the LLM service
        /// </summary>
        /// <returns>Health status information</returns>
        Task<LLMServiceHealth> GetServiceHealthAsync();

        /// <summary>
        /// Updates the LLM service configuration
        /// </summary>
        /// <param name="baseUrl">New base URL for the LLM service</param>
        /// <param name="model">New model name (optional)</param>
        void UpdateConfiguration(string baseUrl, string? model = null);

        /// <summary>
        /// Gets the current LLM service configuration
        /// </summary>
        /// <returns>Current configuration</returns>
        LLMServiceConfig GetConfiguration();
    }

    public class LLMRequest
    {
        public string Model { get; set; } = "local-model";
        public List<LLMMessage> Messages { get; set; } = new();
        public float Temperature { get; set; } = 0.7f;
        public int MaxTokens { get; set; } = 1000;
        public bool Stream { get; set; } = false;
    }

    public class LLMMessage
    {
        public string Role { get; set; } = "user";
        public string Content { get; set; } = "";
    }

    public class LLMResponse
    {
        public List<LLMChoice> Choices { get; set; } = new();
        public LLMUsage Usage { get; set; } = new();
    }

    public class LLMChoice
    {
        public int Index { get; set; }
        public LLMMessage Message { get; set; } = new();
        public string FinishReason { get; set; } = "";
    }

    public class LLMUsage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
    }

    public class LLMServiceHealth
    {
        public bool IsAvailable { get; set; }
        public string Status { get; set; } = "";
        public string BaseUrl { get; set; } = "";
        public string Model { get; set; } = "";
        public int ResponseTimeMs { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }

    public class LLMServiceConfig
    {
        public string BaseUrl { get; set; } = "";
        public string Model { get; set; } = "local-model";
    }
}
