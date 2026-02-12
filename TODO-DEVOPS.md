# DevOps TODO List

This document tracks remaining tasks to achieve a fully production-ready CI/CD pipeline.

## Critical (Do These First)

- [ ] **Fix JWT Secret Security Issue**
  - Location: `Eccommerce.Api/appsettings.json:13`
  - Issue: JWT secret is hardcoded in source code
  - Action: Move to environment variables or User Secrets
  - Priority: **CRITICAL** - Security vulnerability

- [ ] **Add Health Check Endpoint**
  - Location: `Eccommerce.Api/Program.cs`
  - Issue: Docker healthchecks and K8s probes reference `/health` which doesn't exist
  - Action: Add `app.MapHealthChecks("/health");` to Program.cs
  - Priority: **HIGH** - Required for container orchestration

- [ ] **Configure Production Database**
  - Location: `Eccommerce.Api/appsettings.json:3`
  - Issue: LocalDB won't work in containers or production
  - Action: Set up SQL Server container or cloud database
  - Priority: **HIGH** - Deployment blocker

## High Priority (Do Soon)

- [ ] **Create Test Projects**
  - Missing: Unit and integration test projects
  - Action: Create test projects:
    - `Eccommerce.Api.Tests` (Integration tests)
    - `Eccommerce.Application.Tests` (Business logic tests)
    - `Eccommerce.Domain.Tests` (Domain entity tests)
  - Priority: **HIGH** - Code quality and CI/CD

- [ ] **Update GitHub Actions Registry**
  - Location: `.github/workflows/ci-cd.yml:10`
  - Issue: Docker image name needs to be updated with actual organization
  - Action: Replace `ghcr.io/${{ github.repository_owner }}/` with your registry
  - Priority: **HIGH** - CI/CD pipeline

- [ ] **Configure Deployment Targets**
  - Location: `.github/workflows/ci-cd.yml` (deploy jobs)
  - Issue: Deployment steps are placeholder commands
  - Action: Add actual deployment commands for your infrastructure
  - Options: Kubernetes, Azure App Service, AWS ECS, etc.
  - Priority: **HIGH** - Required for automated deployments

## Medium Priority

- [ ] **Add Logging Framework**
  - Recommendation: Serilog with structured logging
  - Action:
    ```bash
    dotnet add Eccommerce.Api package Serilog.AspNetCore
    dotnet add Eccommerce.Api package Serilog.Sinks.Console
    dotnet add Eccommerce.Api package Serilog.Sinks.File
    ```
  - Priority: **MEDIUM** - Production monitoring

- [ ] **Set Up Application Monitoring**
  - Options:
    - Azure Application Insights
    - AWS CloudWatch
    - Prometheus + Grafana
    - Datadog, New Relic, etc.
  - Priority: **MEDIUM** - Production observability

- [ ] **Configure CORS for Production**
  - Location: `Eccommerce.Api/Program.cs`
  - Action: Update CORS policy with actual frontend URLs
  - Priority: **MEDIUM** - Security and functionality

- [ ] **Set Up Container Registry**
  - Options:
    - GitHub Container Registry (GHCR) - Already configured
    - Azure Container Registry (ACR)
    - Amazon ECR
    - Docker Hub
  - Priority: **MEDIUM** - If not using GHCR

- [ ] **Database Backup Strategy**
  - Action: Implement automated database backups
  - Options:
    - Azure SQL automated backups
    - AWS RDS backups
    - Custom backup scripts
  - Priority: **MEDIUM** - Data protection

## Low Priority (Nice to Have)

- [ ] **Add Code Quality Tools**
  - SonarQube or SonarCloud
  - Code coverage reporting
  - Static code analysis
  - Priority: **LOW** - Code quality

- [ ] **Set Up Staging Environment**
  - Mirror production for testing
  - Automated deployments from `develop` branch
  - Priority: **LOW** - Testing

- [ ] **Implement Feature Flags**
  - Libraries: LaunchDarkly, Azure App Configuration
  - Gradual rollout capabilities
  - Priority: **LOW** - Advanced deployment

- [ ] **Add API Versioning**
  - Package: `Microsoft.AspNetCore.Mvc.Versioning`
  - URL or header-based versioning
  - Priority: **LOW** - API evolution

- [ ] **Set Up Distributed Tracing**
  - OpenTelemetry
  - Jaeger or Zipkin
  - Priority: **LOW** - Advanced observability

- [ ] **Configure SSL/TLS Certificates**
  - Kubernetes: cert-manager with Let's Encrypt
  - Cloud: Azure Key Vault, AWS Certificate Manager
  - Priority: **LOW** - Handled by infrastructure usually

- [ ] **Add Rate Limiting**
  - Package: `AspNetCoreRateLimit`
  - Protect against abuse
  - Priority: **LOW** - Security enhancement

## Infrastructure Setup Tasks

### GitHub Actions Setup
- [ ] Configure GitHub Secrets:
  - Database connection strings
  - Cloud provider credentials (if applicable)
  - Container registry credentials (if not using GHCR)

### Kubernetes Setup (if using)
- [ ] Set up Kubernetes cluster
- [ ] Install NGINX Ingress Controller
- [ ] Configure TLS certificates (cert-manager)
- [ ] Set up monitoring (Prometheus + Grafana)
- [ ] Configure log aggregation (ELK, Loki, etc.)

### Azure Setup (if using)
- [ ] Create Azure SQL Database
- [ ] Set up Azure Container Registry
- [ ] Configure Azure App Service or AKS
- [ ] Set up Application Insights
- [ ] Configure Azure Key Vault for secrets

### AWS Setup (if using)
- [ ] Create RDS instance
- [ ] Set up ECR repository
- [ ] Configure ECS or EKS
- [ ] Set up CloudWatch
- [ ] Configure Secrets Manager

## Code Changes Required

### 1. Add Health Check Endpoint

**File:** `Eccommerce.Api/Program.cs`

Add before `app.Run();`:

```csharp
// Health checks
app.MapHealthChecks("/health");
```

For advanced health checks with database check:

```bash
dotnet add Eccommerce.Api package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
```

```csharp
// In Program.cs - before builder.Build()
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// After app is built
app.MapHealthChecks("/health");
```

### 2. Move JWT Secret to Environment Variables

**File:** `Eccommerce.Api/appsettings.json`

Change:
```json
"JwtSettings": {
  "Secret": "${JWT_SECRET}",
  "Issuer": "EccommerceApi",
  "Audience": "EccommerceClient",
  "ExpiryMinutes": 60
}
```

**File:** `Eccommerce.Api/appsettings.Development.json`

```json
{
  "JwtSettings": {
    "Secret": "DEV_ONLY_SECRET_KEY_NOT_FOR_PRODUCTION_1234567890"
  }
}
```

For production, use environment variables or User Secrets.

### 3. Update Connection String for Containers

**File:** `Eccommerce.Api/appsettings.Production.json`

Already created with environment variable placeholders.

## Verification Checklist

After completing critical tasks, verify:

- [ ] Application builds successfully
- [ ] Docker image builds without errors
- [ ] Health endpoint returns 200 OK
- [ ] Database migrations run successfully
- [ ] GitHub Actions pipeline completes successfully
- [ ] Application runs in Docker container
- [ ] Secrets are not in source code
- [ ] Tests pass (when created)

## Timeline Suggestion

**Week 1:**
- Fix JWT secret security issue
- Add health check endpoint
- Configure production database
- Update GitHub Actions registry

**Week 2:**
- Create test projects
- Set up deployment targets
- Add logging framework
- Configure monitoring

**Week 3:**
- Database backup strategy
- CORS configuration
- Infrastructure setup

**Week 4:**
- Code quality tools
- Advanced features (as needed)

## Notes

- Always test changes in a non-production environment first
- Document any infrastructure-specific configurations
- Keep this TODO list updated as tasks are completed
- Mark completed tasks with [x]

---

**Last Updated:** 2026-02-12
