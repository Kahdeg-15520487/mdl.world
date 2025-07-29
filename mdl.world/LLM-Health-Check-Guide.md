# LLM Service Health Check - Testing Guide

## Overview
The application now includes comprehensive health checking for the LLM service to ensure better user experience and error handling.

## New Features Added

### 1. LLM Service Health Check Interface
- `IsServiceAvailableAsync()` - Quick availability check
- `GetServiceHealthAsync()` - Detailed health information

### 2. Health Check Endpoints
- `/health` - Overall application health (includes LLM service)
- `/api/TextGeneration/available` - LLM service availability only
- `/api/TextGeneration/health` - LLM service detailed health

### 3. Frontend Status Indicator
- Real-time status indicator in the header
- Automatic periodic health checks (every 30 seconds)
- Visual feedback for service availability
- Clickable indicator shows detailed health modal

### 4. Enhanced Error Handling
- Pre-checks before LLM operations
- Better error messages when service is unavailable
- Graceful degradation when LLM is offline

## Testing the Health Check

### 1. Start the Application
```bash
cd mdl.world
dotnet run
```

### 2. Test Health Endpoints
```bash
# Overall health
curl http://localhost:5100/health

# LLM availability
curl http://localhost:5100/api/TextGeneration/available

# LLM detailed health
curl http://localhost:5100/api/TextGeneration/health
```

### 3. Frontend Testing
1. Open http://localhost:5100 in a browser
2. Check the status indicator in the top-right corner
3. Red dot = LLM service unavailable
4. Green dot = LLM service available
5. Click the indicator to see detailed health information

### 4. Test Enhancement Features
1. Try to enhance a world when LLM service is unavailable
2. Should show error message preventing the operation
3. Enhance button should be disabled when service is down

## Expected Behavior

### When LLM Service is Unavailable
- Status indicator shows red dot
- Health endpoints return `isAvailable: false`
- Enhance operations are blocked with user-friendly messages
- Error details include connection information

### When LLM Service is Available
- Status indicator shows green dot
- Health endpoints return `isAvailable: true`
- All LLM-dependent features work normally
- Response times are reported

## Configuration

LLM service configuration is in `appsettings.json`:
```json
{
  "LLM": {
    "BaseUrl": "http://localhost:8080",
    "Model": "local-model"
  }
}
```

## Monitoring

The frontend automatically checks service status every 30 seconds and updates the UI accordingly. This ensures users always have current information about service availability.
