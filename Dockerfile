# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Eccommerce.Api/Eccommerce.Api.csproj", "Eccommerce.Api/"]
COPY ["Eccommerce.Infrastructure/Eccommerce.Infrastructure.csproj", "Eccommerce.Infrastructure/"]
COPY ["Eccormmerce.Application/Eccormmerce.Application.csproj", "Eccormmerce.Application/"]
COPY ["Eccommerce.Domain/Eccommerce.Domain.csproj", "Eccommerce.Domain/"]
RUN dotnet restore "./Eccommerce.Api/Eccommerce.Api.csproj"
COPY . .
WORKDIR "/src/Eccommerce.Api"
RUN dotnet build "./Eccommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage runs tests (optional - uncomment when test projects are added)
# FROM build AS test
# WORKDIR /src
# RUN dotnet test --configuration $BUILD_CONFIGURATION --no-build --verbosity normal

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Eccommerce.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install curl for health checks
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER app

# Health check configuration
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Eccommerce.Api.dll"]