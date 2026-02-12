# DevOps Guide - Eccommerce API

This document provides a comprehensive guide for DevOps engineers working with the Eccommerce API project.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [CI/CD Pipeline](#cicd-pipeline)
3. [Local Development](#local-development)
4. [Docker & Containerization](#docker--containerization)
5. [Kubernetes Deployment](#kubernetes-deployment)
6. [Database Management](#database-management)
7. [Security](#security)
8. [Monitoring & Logging](#monitoring--logging)
9. [Troubleshooting](#troubleshooting)

## Architecture Overview

### Project Structure

```
Eccommerce.Api/
├── .github/workflows/        # GitHub Actions CI/CD pipelines
├── k8s/                      # Kubernetes manifests
├── scripts/                  # Build, test, and deployment scripts
├── Eccommerce.Api/          # Web API (Presentation Layer)
├── Eccormmerce.Application/ # Business Logic (CQRS with MediatR)
├── Eccommerce.Domain/       # Domain Entities
└── Eccommerce.Infrastructure/ # Data Access & Infrastructure
```

### Technology Stack

- **Framework:** .NET 8.0
- **Architecture:** Clean Architecture + CQRS
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** JWT Bearer Tokens
- **Validation:** FluentValidation
- **API Documentation:** Swagger/OpenAPI
- **Containerization:** Docker
- **Orchestration:** Kubernetes

## CI/CD Pipeline

### GitHub Actions Workflow

The CI/CD pipeline is defined in [.github/workflows/ci-cd.yml](.github/workflows/ci-cd.yml).

#### Pipeline Stages

1. **Build and Test** (runs on all branches)
   - Checkout code
   - Setup .NET 8.0
   - Restore dependencies
   - Build solution
   - Run tests (when available)
   - Publish artifacts

2. **Code Analysis**
   - Security scanning for vulnerable packages
   - Static code analysis

3. **Docker Build**
   - Build multi-stage Docker image
   - Tag with branch name, SHA, and semantic version
   - Push to GitHub Container Registry
   - Scan for vulnerabilities with Trivy

4. **Deploy to Development** (develop branch only)
   - Triggered on push to `develop`
   - Deploys to development environment

5. **Deploy to Production** (main branch only)
   - Triggered on push to `main`
   - Requires manual approval
   - Deploys to production environment
   - Runs database migrations

#### Environment Variables

Configure these secrets in GitHub Actions:

```yaml
GITHUB_TOKEN          # Automatically provided by GitHub
# Add these manually:
# Azure/AWS credentials (if deploying to cloud)
# Database connection strings
# Registry credentials (if not using GHCR)
```

### Branch Strategy

- `main` - Production-ready code
- `develop` - Development integration branch
- `feature/*` - Feature branches
- Pull requests required for `main` and `develop`

## Local Development

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or Docker
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Setup

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd Eccommerce.Api
   ```

2. **Configure environment variables:**
   ```bash
   cp .env.example .env
   # Edit .env with your local settings
   ```

3. **Start dependencies with Docker Compose:**
   ```bash
   docker-compose up -d sqlserver
   ```

4. **Run database migrations:**
   ```bash
   ./scripts/migrate.sh
   # or on Windows:
   .\scripts\migrate.ps1
   ```

5. **Build and run:**
   ```bash
   ./scripts/build.sh
   dotnet run --project Eccommerce.Api/Eccommerce.Api.csproj
   ```

### Using Docker Compose for Full Stack

```bash
# Start all services
docker-compose up --build

# View logs
docker-compose logs -f api

# Stop services
docker-compose down

# Clean up volumes
docker-compose down -v
```

Access the API at: http://localhost:5000
Swagger UI: http://localhost:5000/swagger

## Docker & Containerization

### Dockerfile

The [Dockerfile](Dockerfile) uses multi-stage builds for optimization:

1. **base** - Runtime base image (ASP.NET Core 8.0)
2. **build** - Build stage with SDK
3. **test** - Test execution (optional, commented out)
4. **publish** - Publish optimized output
5. **final** - Production image with health checks

### Building Docker Image

```bash
# Build image
docker build -t eccommerce-api:latest .

# Build with specific configuration
docker build --build-arg BUILD_CONFIGURATION=Release -t eccommerce-api:v1.0.0 .

# Run container
docker run -p 8080:8080 \
  -e ConnectionStrings__Default="Server=sqlserver;Database=EccommerceDb;..." \
  -e JwtSettings__Secret="your-secret-key" \
  eccommerce-api:latest
```

### Docker Compose Services

- **sqlserver** - SQL Server 2022 database
- **api** - Eccommerce API application

## Kubernetes Deployment

### Prerequisites

- Kubernetes cluster (v1.24+)
- kubectl configured
- Container registry access
- Ingress controller

### Deployment Steps

1. **Create namespace:**
   ```bash
   kubectl apply -f k8s/namespace.yaml
   ```

2. **Configure secrets:**
   ```bash
   cp k8s/secret.yaml.example k8s/secret.yaml
   # Edit k8s/secret.yaml with base64-encoded values
   kubectl apply -f k8s/secret.yaml
   ```

3. **Apply configurations:**
   ```bash
   kubectl apply -f k8s/configmap.yaml
   ```

4. **Deploy application:**
   ```bash
   kubectl apply -f k8s/deployment.yaml
   kubectl apply -f k8s/service.yaml
   kubectl apply -f k8s/ingress.yaml
   ```

5. **Enable auto-scaling:**
   ```bash
   kubectl apply -f k8s/hpa.yaml
   ```

See [k8s/README.md](k8s/README.md) for detailed Kubernetes deployment guide.

## Database Management

### Entity Framework Core Migrations

#### Create Migration

```bash
cd Eccommerce.Api
dotnet ef migrations add MigrationName --project ../Eccommerce.Infrastructure
```

#### Apply Migrations

```bash
# Local development
./scripts/migrate.sh

# Production (with connection string)
CONNECTION_STRING="Server=..." ./scripts/migrate.sh

# Docker
docker-compose exec api dotnet ef database update

# Kubernetes
kubectl exec -it deployment/eccommerce-api -n eccommerce -- dotnet ef database update
```

#### Rollback Migration

```bash
dotnet ef database update PreviousMigrationName
```

### Database Connection Strings

**Development (LocalDB):**
```
Server=(localdb)\\MSSQLLocalDB;Database=EccommerceDb;Trusted_Connection=True;
```

**Docker:**
```
Server=sqlserver;Database=EccommerceDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

**Production:**
```
Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};Encrypt=True;TrustServerCertificate=False;
```

## Security

### Secrets Management

**CRITICAL:** Never commit secrets to version control!

#### Local Development
- Use User Secrets: `dotnet user-secrets set "JwtSettings:Secret" "your-secret"`
- Or use `.env` file (already in .gitignore)

#### CI/CD
- GitHub Secrets for GitHub Actions
- Azure Key Vault for Azure deployments
- AWS Secrets Manager for AWS deployments

#### Kubernetes
- Kubernetes Secrets (base64-encoded)
- External secrets operators
- Sealed Secrets for GitOps

### Security Best Practices

1. **JWT Secret:**
   - Minimum 64 characters
   - Use cryptographically secure random generator
   - Rotate regularly

2. **Database:**
   - Use strong passwords
   - Enable SSL/TLS encryption
   - Restrict network access
   - Regular backups

3. **Container Security:**
   - Scan images for vulnerabilities
   - Use non-root users
   - Keep base images updated
   - Use multi-stage builds

4. **API Security:**
   - Enable CORS properly
   - Rate limiting
   - Input validation
   - HTTPS only in production

### Current Security Issues to Address

⚠️ **IMPORTANT:** The following issues need immediate attention:

1. **JWT Secret in appsettings.json** - Hardcoded secret (line 13)
   - **Fix:** Move to environment variables or User Secrets
   - **Impact:** High - Compromised authentication

2. **LocalDB Connection String** - Won't work in containers
   - **Fix:** Use SQL Server container or cloud database
   - **Impact:** Medium - Deployment failure

## Monitoring & Logging

### Health Checks

**Add this to Program.cs:**

```csharp
app.MapHealthChecks("/health");
```

**Health check endpoints:**
- `/health` - Basic health check
- `/health/ready` - Readiness probe (recommended)
- `/health/live` - Liveness probe (recommended)

### Logging

The application uses built-in .NET logging with Serilog recommended for production.

**Add Serilog (recommended):**

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
```

### Application Insights (Azure)

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

### Prometheus Metrics (Kubernetes)

```bash
dotnet add package prometheus-net.AspNetCore
```

## Troubleshooting

### Common Issues

#### 1. Build Failures

**Error:** "Unable to restore packages"
```bash
# Clear NuGet cache
dotnet nuget locals all --clear
dotnet restore
```

**Error:** "Project not found"
```bash
# Verify solution structure
dotnet sln list
# Clean and rebuild
dotnet clean && dotnet build
```

#### 2. Database Connection Failures

**LocalDB not found:**
- Install SQL Server Express LocalDB
- Use Docker SQL Server instead

**Connection timeout:**
- Check SQL Server is running
- Verify connection string
- Check firewall rules

#### 3. Docker Issues

**Build fails:**
```bash
# Check Docker is running
docker info

# Clear Docker cache
docker builder prune

# Rebuild without cache
docker build --no-cache -t eccommerce-api .
```

**Container won't start:**
```bash
# Check logs
docker logs <container-id>

# Inspect container
docker inspect <container-id>
```

#### 4. Kubernetes Issues

**Pods not starting:**
```bash
# Check pod status
kubectl describe pod <pod-name> -n eccommerce

# Check logs
kubectl logs <pod-name> -n eccommerce

# Check events
kubectl get events -n eccommerce --sort-by='.lastTimestamp'
```

**Image pull failures:**
- Verify image exists in registry
- Check image pull secrets
- Verify registry credentials

### Performance Issues

1. **Slow API responses:**
   - Enable logging to identify bottlenecks
   - Check database query performance
   - Review connection pooling settings

2. **High memory usage:**
   - Monitor with `kubectl top pods`
   - Adjust resource limits in deployment.yaml
   - Check for memory leaks

3. **Database locks:**
   - Review transaction scopes
   - Optimize queries
   - Add database indexes

## Additional Resources

### Documentation
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Docker Documentation](https://docs.docker.com/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)

### Tools
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)
- [Helm](https://helm.sh/)
- [k9s](https://k9scli.io/) - Kubernetes CLI UI

### Scripts Reference
- [scripts/README.md](scripts/README.md) - Build and deployment scripts
- [k8s/README.md](k8s/README.md) - Kubernetes deployment guide

## Support

For issues and questions:
1. Check this documentation
2. Review GitHub Issues
3. Contact the DevOps team
4. Review application logs

---

**Last Updated:** 2026-02-12
**Maintained by:** DevOps Team
