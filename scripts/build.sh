#!/bin/bash
set -e

echo "======================================"
echo "Building Eccommerce API"
echo "======================================"

# Configuration
BUILD_CONFIGURATION="${BUILD_CONFIGURATION:-Release}"
OUTPUT_DIR="${OUTPUT_DIR:-./publish}"

echo "Build Configuration: $BUILD_CONFIGURATION"
echo "Output Directory: $OUTPUT_DIR"
echo ""

# Clean previous builds
echo "Cleaning previous builds..."
dotnet clean --configuration $BUILD_CONFIGURATION

# Restore dependencies
echo "Restoring NuGet packages..."
dotnet restore

# Build solution
echo "Building solution..."
dotnet build --configuration $BUILD_CONFIGURATION --no-restore

# Publish application
echo "Publishing application..."
dotnet publish Eccommerce.Api/Eccommerce.Api.csproj \
  --configuration $BUILD_CONFIGURATION \
  --output $OUTPUT_DIR \
  --no-restore \
  /p:UseAppHost=false

echo ""
echo "======================================"
echo "Build completed successfully!"
echo "Output: $OUTPUT_DIR"
echo "======================================"
