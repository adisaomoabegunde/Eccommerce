# PowerShell build script for Eccommerce API
[CmdletBinding()]
param(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [Parameter()]
    [string]$OutputDir = './publish'
)

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Building Eccommerce API" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan

Write-Host "Build Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Output Directory: $OutputDir" -ForegroundColor Yellow
Write-Host ""

try {
    # Clean previous builds
    Write-Host "Cleaning previous builds..." -ForegroundColor Green
    dotnet clean --configuration $Configuration
    if ($LASTEXITCODE -ne 0) { throw "Clean failed" }

    # Restore dependencies
    Write-Host "Restoring NuGet packages..." -ForegroundColor Green
    dotnet restore
    if ($LASTEXITCODE -ne 0) { throw "Restore failed" }

    # Build solution
    Write-Host "Building solution..." -ForegroundColor Green
    dotnet build --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }

    # Publish application
    Write-Host "Publishing application..." -ForegroundColor Green
    dotnet publish Eccommerce.Api/Eccommerce.Api.csproj `
        --configuration $Configuration `
        --output $OutputDir `
        --no-restore `
        /p:UseAppHost=false
    if ($LASTEXITCODE -ne 0) { throw "Publish failed" }

    Write-Host ""
    Write-Host "======================================" -ForegroundColor Cyan
    Write-Host "Build completed successfully!" -ForegroundColor Green
    Write-Host "Output: $OutputDir" -ForegroundColor Yellow
    Write-Host "======================================" -ForegroundColor Cyan
}
catch {
    Write-Host ""
    Write-Host "======================================" -ForegroundColor Red
    Write-Host "Build failed: $_" -ForegroundColor Red
    Write-Host "======================================" -ForegroundColor Red
    exit 1
}
