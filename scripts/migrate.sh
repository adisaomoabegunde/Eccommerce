#!/bin/bash
set -e

echo "======================================"
echo "Running Database Migrations"
echo "======================================"

# Configuration
CONNECTION_STRING="${CONNECTION_STRING:-}"
ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Production}"

echo "Environment: $ENVIRONMENT"
echo ""

# Check if EF Core tools are installed
if ! dotnet ef --version > /dev/null 2>&1; then
    echo "Installing EF Core tools..."
    dotnet tool install --global dotnet-ef
    export PATH="$PATH:$HOME/.dotnet/tools"
fi

echo "EF Core version:"
dotnet ef --version
echo ""

# Navigate to the API project directory
cd Eccommerce.Api

# Apply migrations
echo "Applying migrations to database..."
if [ -n "$CONNECTION_STRING" ]; then
    echo "Using provided connection string"
    dotnet ef database update --connection "$CONNECTION_STRING" --no-build
else
    echo "Using connection string from appsettings"
    dotnet ef database update --no-build
fi

echo ""
echo "======================================"
echo "Migrations applied successfully!"
echo "======================================"
