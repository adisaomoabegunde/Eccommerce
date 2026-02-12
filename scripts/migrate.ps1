# PowerShell database migration script for Eccommerce API
[CmdletBinding()]
param(
    [Parameter()]
    [string]$ConnectionString = "",

    [Parameter()]
    [string]$Environment = "Production"
)

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Running Database Migrations" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan

Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host ""

try {
    # Check if EF Core tools are installed
    $efVersion = dotnet ef --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Installing EF Core tools..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-ef
        if ($LASTEXITCODE -ne 0) { throw "Failed to install EF Core tools" }

        # Refresh PATH
        $env:Path = [System.Environment]::GetEnvironmentVariable("Path", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path", "User")
    }

    Write-Host "EF Core version:" -ForegroundColor Green
    dotnet ef --version
    Write-Host ""

    # Navigate to the API project directory
    Push-Location Eccommerce.Api

    # Apply migrations
    Write-Host "Applying migrations to database..." -ForegroundColor Green
    if ($ConnectionString) {
        Write-Host "Using provided connection string" -ForegroundColor Yellow
        dotnet ef database update --connection $ConnectionString --no-build
    }
    else {
        Write-Host "Using connection string from appsettings" -ForegroundColor Yellow
        dotnet ef database update --no-build
    }

    if ($LASTEXITCODE -ne 0) { throw "Migration failed" }

    Pop-Location

    Write-Host ""
    Write-Host "======================================" -ForegroundColor Cyan
    Write-Host "Migrations applied successfully!" -ForegroundColor Green
    Write-Host "======================================" -ForegroundColor Cyan
}
catch {
    Pop-Location
    Write-Host ""
    Write-Host "======================================" -ForegroundColor Red
    Write-Host "Migration failed: $_" -ForegroundColor Red
    Write-Host "======================================" -ForegroundColor Red
    exit 1
}
