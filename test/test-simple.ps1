# Simple Fantasy World Generator API Test Script

# Function to read port from launchSettings.json
function Get-ApiPortFromLaunchSettings {
    $launchSettingsPath = Join-Path $PSScriptRoot "..\mdl.world\Properties\launchSettings.json"
    
    if (Test-Path $launchSettingsPath) {
        try {
            $launchSettings = Get-Content $launchSettingsPath | ConvertFrom-Json
            $applicationUrl = $launchSettings.profiles.http.applicationUrl
            
            if ($applicationUrl -match "http://localhost:(\d+)") {
                return $Matches[1]
            }
        }
        catch {
            Write-Warning "Could not parse launchSettings.json: $($_.Exception.Message)"
        }
    }
    
    # Fallback to default port
    return "5000"
}

$port = Get-ApiPortFromLaunchSettings
$BaseUrl = "http://localhost:$port"

Write-Host "Fantasy World Generator API Test Suite" -ForegroundColor Cyan
Write-Host "Testing API at: $BaseUrl" -ForegroundColor Yellow

# Test 1: Server Health Check
Write-Host "`nTest 1: Server Health Check" -ForegroundColor White
try {
    $themes = Invoke-RestMethod -Uri "$BaseUrl/world/themes" -Method GET
    Write-Host "✅ Server is running! Found $($themes.Count) themes" -ForegroundColor Green
} catch {
    Write-Host "❌ Server is not responding" -ForegroundColor Red
    exit 1
}

# Test 2: Basic World Generation
Write-Host "`nTest 2: Basic World Generation" -ForegroundColor White
try {
    $worldRequest = @{
        worldName = "TestWorld-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
        theme = "Fantasy-SciFi"
        techLevel = 7
        magicLevel = 8
    } | ConvertTo-Json

    $world = Invoke-RestMethod -Uri "$BaseUrl/world/generate" -Method POST -Body $worldRequest -ContentType "application/json"
    Write-Host "✅ World generated successfully!" -ForegroundColor Green
    Write-Host "   World ID: $($world.id)" -ForegroundColor Blue
    Write-Host "   World Name: $($world.name)" -ForegroundColor Blue
    Write-Host "   Places: $($world.places.Count)" -ForegroundColor Blue
    Write-Host "   Characters: $($world.historicFigures.Count)" -ForegroundColor Blue
    
    # Test 3: Retrieve Generated World
    Write-Host "`nTest 3: Retrieve Generated World" -ForegroundColor White
    $retrievedWorld = Invoke-RestMethod -Uri "$BaseUrl/world/$($world.id)" -Method GET
    Write-Host "✅ World retrieved successfully!" -ForegroundColor Green
    
    # Test 4: Get All Worlds
    Write-Host "`nTest 4: Get All Worlds" -ForegroundColor White
    $allWorlds = Invoke-RestMethod -Uri "$BaseUrl/world" -Method GET
    Write-Host "✅ Found $($allWorlds.Count) worlds in storage" -ForegroundColor Green

    Write-Host "`n🎉 All tests passed!" -ForegroundColor Green
    Write-Host "Generated World Summary:" -ForegroundColor Yellow
    Write-Host "- Name: $($world.name)" -ForegroundColor White
    Write-Host "- Theme: $($world.worldInfo.genre)" -ForegroundColor White
    Write-Host "- Places: $($world.places.Count)" -ForegroundColor White
    Write-Host "- Characters: $($world.historicFigures.Count)" -ForegroundColor White
    Write-Host "- Events: $($world.worldEvents.Count)" -ForegroundColor White
    Write-Host "- Equipment: $($world.equipment.Count)" -ForegroundColor White
    
} catch {
    Write-Host "❌ Test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nTest completed at $(Get-Date)" -ForegroundColor Gray
