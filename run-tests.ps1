# Fantasy World Generator API - Test Runner
# Convenience script to run tests from the root directory

param(
    [switch]$Verbose,
    [switch]$Quick,
    [string]$BaseUrl = ""
)

Write-Host "🧪 Fantasy World Generator API Test Runner" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan

if ($Quick) {
    Write-Host "Running quick tests..." -ForegroundColor Yellow
    & "test\test-simple.ps1"
} else {
    Write-Host "Running comprehensive test suite..." -ForegroundColor Yellow
    if ($Verbose) {
        if ($BaseUrl) {
            & "test\test-api-suite.ps1" -Verbose -BaseUrl $BaseUrl
        } else {
            & "test\test-api-suite.ps1" -Verbose
        }
    } else {
        if ($BaseUrl) {
            & "test\test-api-suite.ps1" -BaseUrl $BaseUrl
        } else {
            & "test\test-api-suite.ps1"
        }
    }
}
