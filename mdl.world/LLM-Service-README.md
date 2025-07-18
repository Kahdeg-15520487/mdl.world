# LLM Text Generation Service

This service provides integration with a local llama-cpp server to generate descriptive text from JSON data.

## Features

- **Text Generation from JSON**: Convert any JSON data structure into descriptive narrative text
- **World Narratives**: Generate immersive descriptions of fantasy/sci-fi worlds
- **Character Descriptions**: Create detailed character backstories and descriptions
- **Location Descriptions**: Generate atmospheric descriptions of places and environments
- **Event Narratives**: Create compelling stories about historical events and occurrences

## Configuration

### appsettings.json

```json
{
  "LLM": {
    "BaseUrl": "http://localhost:8080",
    "Model": "local-model"
  }
}
```

### llama-cpp Server Setup

1. Install and run llama-cpp-python server:
   ```bash
   pip install llama-cpp-python[server]
   
   # Run the server with OpenAI API compatibility
   python -m llama_cpp.server --model path/to/your/model.gguf --port 8080 --host 0.0.0.0
   ```

2. Or use the official llama.cpp server:
   ```bash
   ./server -m path/to/your/model.gguf --port 8080 --host 0.0.0.0
   ```

## API Endpoints

### POST /api/textgeneration/generate-from-json
Generate text from custom JSON data with a custom prompt.

**Request Body:**
```json
{
  "jsonData": { /* your JSON data */ },
  "prompt": "Your custom prompt here"
}
```

### POST /api/textgeneration/generate-world-narrative
Generate a narrative description of a world by first generating the world data.

**Request Body:**
```json
"World Name Here"
```

### POST /api/textgeneration/generate-character-description
Generate a character description from character JSON data.

**Request Body:**
```json
{
  "name": "Character Name",
  "race": "Character Race",
  /* other character data */
}
```

### POST /api/textgeneration/generate-location-description
Generate a location description from place JSON data.

**Request Body:**
```json
{
  "name": "Location Name",
  "type": "Location Type",
  /* other location data */
}
```

### POST /api/textgeneration/generate-event-narrative
Generate an event narrative from event JSON data.

**Request Body:**
```json
{
  "name": "Event Name",
  "type": "Event Type",
  /* other event data */
}
```

## Usage Examples

See `test-llm-generation.http` for complete usage examples.

## Error Handling

The service includes comprehensive error handling:
- Network connection errors to the LLM server
- JSON parsing errors
- HTTP request/response errors
- Graceful fallbacks when the LLM service is unavailable

## Service Registration

The service is registered in `Program.cs`:

```csharp
// Register HTTP client for LLM service
builder.Services.AddHttpClient<ILLMTextGenerationService, LLMTextGenerationService>();

// Register LLM text generation service
builder.Services.AddScoped<ILLMTextGenerationService, LLMTextGenerationService>();
```

## Dependencies

- `HttpClient` for API communication
- `System.Text.Json` for JSON serialization
- `Microsoft.Extensions.Configuration` for configuration management
- `Microsoft.Extensions.Logging` for logging

## Architecture

The service follows the dependency injection pattern with:
- `ILLMTextGenerationService` interface for abstraction
- `LLMTextGenerationService` implementation
- OpenAI-compatible API format for maximum compatibility
- Configurable base URL and model selection
- Comprehensive logging for debugging and monitoring
