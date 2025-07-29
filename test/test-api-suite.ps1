# Fantasy World Generator API Test Script
# Tests all major functionality of the MDL World API

param(
    [string]$BaseUrl = "",
    [switch]$Verbose
)

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

# Set BaseUrl if not provided
if ([string]::IsNullOrEmpty($BaseUrl)) {
    $port = Get-ApiPortFromLaunchSettings
    $BaseUrl = "http://localhost:$port"
}

Write-Host "🌟 Fantasy World Generator API Test Suite" -ForegroundColor Cyan
Write-Host "Testing API at: $BaseUrl" -ForegroundColor Yellow
Write-Host "=========================================="

# Test counter
$testCount = 0
$passedTests = 0
$failedTests = 0

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Endpoint,
        [string]$Body = $null,
        [int]$ExpectedStatus = 200
    )
    
    $script:testCount++
    Write-Host "`n[$script:testCount] Testing: $Name" -ForegroundColor White
    
    try {
        $headers = @{"Content-Type" = "application/json"}
        
        if ($Body) {
            $response = Invoke-RestMethod -Uri "$BaseUrl$Endpoint" -Method $Method -Body $Body -Headers $headers -StatusCodeVariable statusCode
        } else {
            $response = Invoke-RestMethod -Uri "$BaseUrl$Endpoint" -Method $Method -Headers $headers -StatusCodeVariable statusCode
        }
        
        if ($statusCode -eq $ExpectedStatus) {
            Write-Host "   ✅ PASS - Status: $statusCode" -ForegroundColor Green
            $script:passedTests++
            
            if ($Verbose -and $response) {
                if ($response -is [array]) {
                    Write-Host "   📊 Response: Array with $($response.Count) items" -ForegroundColor Blue
                } elseif ($response.id) {
                    Write-Host "   🆔 Generated ID: $($response.id)" -ForegroundColor Blue
                } elseif ($response.name) {
                    Write-Host "   📝 Name: $($response.name)" -ForegroundColor Blue
                }
            }
            
            return $response
        } else {
            Write-Host "   ❌ FAIL - Expected: $ExpectedStatus, Got: $statusCode" -ForegroundColor Red
            $script:failedTests++
            return $null
        }
    }
    catch {
        # Handle HTTP error responses (4xx, 5xx)
        if ($_.Exception -is [Microsoft.PowerShell.Commands.HttpResponseException]) {
            $actualStatus = [int]$_.Exception.Response.StatusCode
            if ($actualStatus -eq $ExpectedStatus) {
                Write-Host "   ✅ PASS - Status: $actualStatus (Expected Error)" -ForegroundColor Green
                $script:passedTests++
                return $null
            } else {
                Write-Host "   ❌ FAIL - Expected: $ExpectedStatus, Got: $actualStatus" -ForegroundColor Red
                $script:failedTests++
                return $null
            }
        } else {
            Write-Host "   ❌ FAIL - Error: $($_.Exception.Message)" -ForegroundColor Red
            $script:failedTests++
            return $null
        }
    }
}

# Test 1: Check if server is running
Write-Host "`n🔧 INFRASTRUCTURE TESTS" -ForegroundColor Magenta
$themes = Test-Endpoint "Get Available Themes" "GET" "/world/themes"

if (-not $themes) {
    Write-Host "❌ Server appears to be down. Please start the API server first." -ForegroundColor Red
    exit 1
}

# Test 2: Get world templates
$null = Test-Endpoint "Get World Templates" "GET" "/world/templates"

# Test 3: Get existing worlds
$null = Test-Endpoint "Get All Worlds" "GET" "/world"

Write-Host "`n🌍 WORLD GENERATION TESTS" -ForegroundColor Magenta

# Test 4: Basic World Generation
$basicWorldRequest = @{
    worldName = "TestWorld-Basic-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    theme = "Fantasy-SciFi"
    techLevel = 6
    magicLevel = 7
} | ConvertTo-Json

$generatedWorld = Test-Endpoint "Basic World Generation" "POST" "/world/generate" $basicWorldRequest

# Test 5: Custom World Generation
$customWorldRequest = @{
    worldName = "TestWorld-Custom-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    theme = "Cyberpunk-Fantasy"
    techLevel = 8
    magicLevel = 5
    worldSize = 20
    includeMagicTech = $true
    includeSpaceTravel = $false
    includeAncientRuins = $true
    preferredBiomes = @("Bio-Mechanical Jungles", "Mystical Data Centers")
    preferredRaces = @("Cyber-Elves", "Techno-Dwarves")
    difficultyLevel = "Medium"
} | ConvertTo-Json -Depth 10

$null = Test-Endpoint "Custom World Generation" "POST" "/world/generate-custom" $customWorldRequest

Write-Host "`n🏰 WORLD MANAGEMENT TESTS" -ForegroundColor Magenta

# Test 6: Get specific world (if we have one)
if ($generatedWorld -and $generatedWorld.id) {
    $null = Test-Endpoint "Get Specific World" "GET" "/world/$($generatedWorld.id)"
    
    # Test 7: Enhance world with additional characters
    $enhanceRequest = @{
        worldName = "Enhanced-$($generatedWorld.name)"
        contentType = "characters"
    } | ConvertTo-Json
    
    $null = Test-Endpoint "Enhance World (Characters)" "POST" "/world/$($generatedWorld.id)/enhance" $enhanceRequest
    
    # Test 8: Enhance world with additional places
    $enhancePlacesRequest = @{
        worldName = "Enhanced-Places-$($generatedWorld.name)"
        contentType = "places"
    } | ConvertTo-Json
    
    $null = Test-Endpoint "Enhance World (Places)" "POST" "/world/$($generatedWorld.id)/enhance" $enhancePlacesRequest
}

Write-Host "`n🤖 LLM INTEGRATION TESTS" -ForegroundColor Magenta

# Test 9: Text generation from JSON (if world exists)
if ($generatedWorld) {
    $textGenRequest = @{
        jsonData = $generatedWorld
        prompt = "Generate a dramatic description of this fantasy world"
    } | ConvertTo-Json -Depth 10
    
    Test-Endpoint "Generate Text from World JSON" "POST" "/api/textgeneration/generate-from-json" $textGenRequest
}

# Test 10: World narrative generation
$narrativeRequest = "TestNarrative-$(Get-Date -Format 'yyyyMMdd-HHmmss')" | ConvertTo-Json

Test-Endpoint "Generate World Narrative" "POST" "/api/textgeneration/generate-world-narrative" $narrativeRequest

Write-Host "`n🔍 WORLD ENHANCEMENT SERVICE TESTS" -ForegroundColor Magenta

# Test 11: World enhancement service (if we have a world)
if ($generatedWorld) {
    $enhancementRequest = @{
        world = $generatedWorld
        userComment = "Add more magical academies and cyber-enhancement facilities"
        targetSection = "places"
    } | ConvertTo-Json -Depth 10
    
    Test-Endpoint "World Enhancement Service" "POST" "/api/worldenhancement/enhance" $enhancementRequest
}

Write-Host "`n🧪 EDGE CASE TESTS" -ForegroundColor Magenta

# Test 12: Invalid world ID
Test-Endpoint "Get Invalid World" "GET" "/world/invalid-id-12345" $null 404

# Test 13: Empty world generation request (API generates with defaults)
$emptyRequest = @{} | ConvertTo-Json
Test-Endpoint "Empty Generation Request (Default Values)" "POST" "/world/generate" $emptyRequest 200

# Test 14: Invalid theme
$invalidThemeRequest = @{
    worldName = "InvalidThemeTest"
    theme = "NonExistentTheme"
    techLevel = 5
    magicLevel = 5
} | ConvertTo-Json

Test-Endpoint "Invalid Theme Request" "POST" "/world/generate" $invalidThemeRequest

Write-Host "`n📊 CONTENT VALIDATION TESTS" -ForegroundColor Magenta

# Test 15: Validate generated world structure
if ($generatedWorld) {
    Write-Host "`n[15] Validating Generated World Structure" -ForegroundColor White
    
    $validationPassed = $true
    $validationErrors = @()
    
    # Check required fields
    $requiredFields = @("id", "name", "description", "creationDate", "worldInfo", "places", "historicFigures", "worldEvents", "equipment")
    foreach ($field in $requiredFields) {
        if (-not $generatedWorld.$field) {
            $validationErrors += "Missing required field: $field"
            $validationPassed = $false
        }
    }
    
    # Check content counts
    if ($generatedWorld.places.Count -eq 0) {
        $validationErrors += "No places generated"
        $validationPassed = $false
    }
    
    if ($generatedWorld.historicFigures.Count -eq 0) {
        $validationErrors += "No historic figures generated"
        $validationPassed = $false
    }
    
    if ($validationPassed) {
        Write-Host "   ✅ PASS - World structure is valid" -ForegroundColor Green
        Write-Host "   📊 Places: $($generatedWorld.places.Count)" -ForegroundColor Blue
        Write-Host "   👥 Characters: $($generatedWorld.historicFigures.Count)" -ForegroundColor Blue
        Write-Host "   📚 Events: $($generatedWorld.worldEvents.Count)" -ForegroundColor Blue
        Write-Host "   ⚔️ Equipment: $($generatedWorld.equipment.Count)" -ForegroundColor Blue
        $script:passedTests++
    } else {
        Write-Host "   ❌ FAIL - Validation errors:" -ForegroundColor Red
        foreach ($validationError in $validationErrors) {
            Write-Host "     • $validationError" -ForegroundColor Red
        }
        $script:failedTests++
    }
    $script:testCount++
}

# Cleanup Test: Delete test worlds (optional)
Write-Host "`n🧹 CLEANUP (Optional)" -ForegroundColor Magenta
if ($generatedWorld -and $generatedWorld.id) {
    $confirm = Read-Host "Delete test world '$($generatedWorld.name)' (ID: $($generatedWorld.id))? [y/N]"
    if ($confirm -eq 'y' -or $confirm -eq 'Y') {
        Test-Endpoint "Delete Test World" "DELETE" "/world/$($generatedWorld.id)"
    }
}

# Final Results
Write-Host "`n" + "=" * 50
Write-Host "🏁 TEST RESULTS SUMMARY" -ForegroundColor Cyan
Write-Host "Total Tests: $testCount" -ForegroundColor White
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Failed: $failedTests" -ForegroundColor Red

$successRate = [math]::Round(($passedTests / $testCount) * 100, 1)
Write-Host "Success Rate: $successRate%" -ForegroundColor $(if ($successRate -ge 80) { "Green" } elseif ($successRate -ge 60) { "Yellow" } else { "Red" })

if ($failedTests -eq 0) {
    Write-Host "`n🎉 ALL TESTS PASSED! The Fantasy World Generator API is working perfectly!" -ForegroundColor Green
} elseif ($successRate -ge 80) {
    Write-Host "`n✅ Most tests passed! API is functional with minor issues." -ForegroundColor Yellow
} else {
    Write-Host "`n⚠️  Multiple test failures detected. Please check the API implementation." -ForegroundColor Red
}

Write-Host "`n📝 Test completed at $(Get-Date)" -ForegroundColor Gray
