# PowerShell test script for Eccommerce API
[CmdletBinding()]
param(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [Parameter()]
    [bool]$CollectCoverage = $true
)

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Running Eccommerce API Tests" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan

Write-Host "Build Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Collect Coverage: $CollectCoverage" -ForegroundColor Yellow
Write-Host ""

# Check if test projects exist
$testProjects = Get-ChildItem -Recurse -Filter "*.Tests.csproj"
if ($testProjects.Count -eq 0) {
    Write-Host "WARNING: No test projects found in the solution!" -ForegroundColor Yellow
    Write-Host "Please create test projects (e.g., Eccommerce.Api.Tests, Eccommerce.Application.Tests)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Example test project structure:" -ForegroundColor Cyan
    Write-Host "  - Eccommerce.Api.Tests (Integration/API tests)" -ForegroundColor White
    Write-Host "  - Eccommerce.Application.Tests (Unit tests for business logic)" -ForegroundColor White
    Write-Host "  - Eccommerce.Domain.Tests (Unit tests for domain entities)" -ForegroundColor White
    Write-Host ""
    exit 0
}

try {
    # Restore dependencies
    Write-Host "Restoring dependencies..." -ForegroundColor Green
    dotnet restore
    if ($LASTEXITCODE -ne 0) { throw "Restore failed" }

    # Build test projects
    Write-Host "Building test projects..." -ForegroundColor Green
    dotnet build --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }

    # Run tests
    Write-Host "Running tests..." -ForegroundColor Green
    if ($CollectCoverage) {
        Write-Host "Collecting code coverage..." -ForegroundColor Yellow
        dotnet test `
            --configuration $Configuration `
            --no-build `
            --verbosity normal `
            --collect:"XPlat Code Coverage" `
            --results-directory ./TestResults `
            --logger "trx;LogFileName=test-results.trx"
    }
    else {
        dotnet test `
            --configuration $Configuration `
            --no-build `
            --verbosity normal `
            --logger "trx;LogFileName=test-results.trx"
    }

    if ($LASTEXITCODE -ne 0) { throw "Tests failed" }

    Write-Host ""
    Write-Host "======================================" -ForegroundColor Cyan
    Write-Host "Tests completed successfully!" -ForegroundColor Green
    Write-Host "Test results: ./TestResults" -ForegroundColor Yellow
    Write-Host "======================================" -ForegroundColor Cyan
}
catch {
    Write-Host ""
    Write-Host "======================================" -ForegroundColor Red
    Write-Host "Tests failed: $_" -ForegroundColor Red
    Write-Host "======================================" -ForegroundColor Red
    exit 1
}
