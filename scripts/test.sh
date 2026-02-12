#!/bin/bash
set -e

echo "======================================"
echo "Running Eccommerce API Tests"
echo "======================================"

# Configuration
BUILD_CONFIGURATION="${BUILD_CONFIGURATION:-Release}"
COLLECT_COVERAGE="${COLLECT_COVERAGE:-true}"

echo "Build Configuration: $BUILD_CONFIGURATION"
echo "Collect Coverage: $COLLECT_COVERAGE"
echo ""

# Check if test projects exist
if ! find . -name "*.Tests.csproj" -o -name "*Tests.csproj" | grep -q .; then
    echo "WARNING: No test projects found in the solution!"
    echo "Please create test projects (e.g., Eccommerce.Api.Tests, Eccommerce.Application.Tests)"
    echo ""
    echo "Example test project structure:"
    echo "  - Eccommerce.Api.Tests (Integration/API tests)"
    echo "  - Eccommerce.Application.Tests (Unit tests for business logic)"
    echo "  - Eccommerce.Domain.Tests (Unit tests for domain entities)"
    echo ""
    exit 0
fi

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Build test projects
echo "Building test projects..."
dotnet build --configuration $BUILD_CONFIGURATION --no-restore

# Run tests
echo "Running tests..."
if [ "$COLLECT_COVERAGE" = "true" ]; then
    echo "Collecting code coverage..."
    dotnet test \
        --configuration $BUILD_CONFIGURATION \
        --no-build \
        --verbosity normal \
        --collect:"XPlat Code Coverage" \
        --results-directory ./TestResults \
        --logger "trx;LogFileName=test-results.trx"
else
    dotnet test \
        --configuration $BUILD_CONFIGURATION \
        --no-build \
        --verbosity normal \
        --logger "trx;LogFileName=test-results.trx"
fi

echo ""
echo "======================================"
echo "Tests completed successfully!"
echo "Test results: ./TestResults"
echo "======================================"
