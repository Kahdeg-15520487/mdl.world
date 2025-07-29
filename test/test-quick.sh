#!/bin/bash

# Fantasy World Generator API Test Runner
# Quick test script for Unix/Linux/macOS

echo "========================================"
echo "Fantasy World Generator API Test Suite"
echo "========================================"
echo

# Read port from launchSettings.json
LAUNCH_SETTINGS_PATH="../mdl.world/Properties/launchSettings.json"
if [ -f "$LAUNCH_SETTINGS_PATH" ]; then
    # Extract port using jq if available, otherwise use grep/sed
    if command -v jq >/dev/null 2>&1; then
        API_PORT=$(jq -r '.profiles.http.applicationUrl' "$LAUNCH_SETTINGS_PATH" | sed 's/.*://')
    else
        API_PORT=$(grep -o '"applicationUrl":\s*"[^"]*"' "$LAUNCH_SETTINGS_PATH" | sed 's/.*localhost://' | sed 's/".*//')
    fi
else
    echo "Warning: launchSettings.json not found, using default port 5000"
    API_PORT=5000
fi

# Fallback to default if parsing failed
if [ -z "$API_PORT" ]; then
    API_PORT=5000
fi

BASE_URL="http://localhost:$API_PORT"
echo "Using API endpoint: $BASE_URL"
echo

# Check if server is running
echo "[1/5] Checking if API server is running..."
if ! curl -s -f "$BASE_URL/world/themes" > /dev/null; then
    echo "ERROR: API server is not running on $BASE_URL"
    echo "Please start the server with: dotnet run"
    exit 1
fi
echo "✅ Server is running!"
echo

# Test 1: Get themes
echo "[2/5] Testing: Get Available Themes"
curl -X GET "$BASE_URL/world/themes" | jq '.' 2>/dev/null || curl -X GET "$BASE_URL/world/themes"
echo
echo

# Test 2: Basic world generation
echo "[3/5] Testing: Basic World Generation"
WORLD_RESPONSE=$(curl -s -X POST "$BASE_URL/world/generate" \
     -H "Content-Type: application/json" \
     -d '{"worldName":"ShellTestWorld","theme":"Fantasy-SciFi","techLevel":6,"magicLevel":7}')

echo "$WORLD_RESPONSE" | jq '.' 2>/dev/null || echo "$WORLD_RESPONSE"

# Extract world ID for further testing
WORLD_ID=$(echo "$WORLD_RESPONSE" | jq -r '.id' 2>/dev/null)
echo
echo "Generated World ID: $WORLD_ID"
echo

# Test 3: Get all worlds
echo "[4/5] Testing: Get All Worlds"
curl -X GET "$BASE_URL/world" | jq '.' 2>/dev/null || curl -X GET "$BASE_URL/world"
echo
echo

# Test 4: Get specific world (if ID was extracted)
if [ "$WORLD_ID" != "null" ] && [ -n "$WORLD_ID" ]; then
    echo "[5/5] Testing: Get Specific World ($WORLD_ID)"
    curl -X GET "$BASE_URL/world/$WORLD_ID" | jq '.name, .description' 2>/dev/null || curl -X GET "$BASE_URL/world/$WORLD_ID"
else
    echo "[5/5] Testing: Get World Templates"
    curl -X GET "$BASE_URL/world/templates" | jq '.' 2>/dev/null || curl -X GET "$BASE_URL/world/templates"
fi
echo
echo

echo "========================================"
echo "✅ Basic API tests completed!"
echo "For comprehensive testing, run:"
echo "  PowerShell: ./test-api-suite.ps1"
echo "  HTTP Tests: Use test-api-collection.http"
echo "========================================"
