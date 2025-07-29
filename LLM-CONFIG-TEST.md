# LLM Configuration Test Script

This script demonstrates how to test the new LLM configuration functionality.

## 1. Start the API Server

```powershell
cd "j:\workspace2\llm\mdl.world\mdl.world"
dotnet run
```

## 2. Test Configuration Endpoints

### Get Current LLM Configuration
```powershell
curl -X GET "http://localhost:5000/api/Configuration/llm"
```

### Update LLM Configuration
```powershell
# Update to use Ollama (common local LLM)
curl -X POST "http://localhost:5000/api/Configuration/llm" -H "Content-Type: application/json" -d '{
  "baseUrl": "http://localhost:11434",
  "model": "llama2"
}'
```

### Test LLM Connection
```powershell
curl -X GET "http://localhost:5000/api/Configuration/llm/test"
```

## 3. Frontend Testing

1. Open browser to `http://localhost:5000`
2. Go to the Settings tab (⚙️)
3. Scroll down to "LLM Configuration" section
4. You can:
   - View current LLM URL and model
   - Update the configuration
   - Test the connection
   - Use quick presets for common LLM servers

## 4. Configuration Presets Available

- **Local LLM**: `http://localhost:8080` (default)
- **Ollama**: `http://localhost:11434` 
- **LM Studio**: `http://localhost:1234`

## 5. Features Added

### Backend:
- New `ConfigurationController` with LLM config endpoints
- Runtime configuration updates in `LLMTextGenerationService`
- Health check endpoints already existed

### Frontend:
- LLM configuration section in Settings tab
- Real-time configuration loading
- Test connection functionality
- Quick preset buttons for popular LLM servers
- Visual feedback for connection status

## Usage Example

1. Start your LLM server (e.g., Ollama, LM Studio, etc.)
2. Open the app and go to Settings
3. Enter your LLM server URL in the "LLM Base URL" field
4. Click "Test LLM Connection" to verify
5. Click "Update LLM Config" to save
6. The app will now use your LLM for text generation!

## Notes

- Configuration changes are applied immediately without restart
- The original configuration file values are used as defaults
- Configuration is stored in memory and resets on app restart
- For persistent configuration, update `appsettings.json`
