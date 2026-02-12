# Scripts Directory

This directory contains automation scripts for building, testing, and deploying the Eccommerce API.

## Available Scripts

### Build Scripts

#### `build.sh` / `build.ps1`
Builds the entire solution and publishes the API.

**Linux/Mac:**
```bash
./scripts/build.sh
```

**Windows PowerShell:**
```powershell
.\scripts\build.ps1
```

**Options:**
- Environment variable `BUILD_CONFIGURATION` (default: Release)
- Environment variable `OUTPUT_DIR` (default: ./publish)

**Examples:**
```bash
# Build in Debug mode
BUILD_CONFIGURATION=Debug ./scripts/build.sh

# Build to custom output directory
OUTPUT_DIR=/app/dist ./scripts/build.sh
```

```powershell
# Build in Debug mode
.\scripts\build.ps1 -Configuration Debug

# Build to custom output directory
.\scripts\build.ps1 -OutputDir "C:\app\dist"
```

### Test Scripts

#### `test.sh` / `test.ps1`
Runs all tests in the solution with code coverage.

**Linux/Mac:**
```bash
./scripts/test.sh
```

**Windows PowerShell:**
```powershell
.\scripts\test.ps1
```

**Options:**
- Environment variable `BUILD_CONFIGURATION` (default: Release)
- Environment variable `COLLECT_COVERAGE` (default: true)

**Examples:**
```bash
# Run tests without coverage
COLLECT_COVERAGE=false ./scripts/test.sh
```

```powershell
# Run tests without coverage
.\scripts\test.ps1 -CollectCoverage $false
```

**Note:** These scripts will gracefully handle the case when no test projects exist and provide guidance on creating them.

### Database Migration Scripts

#### `migrate.sh` / `migrate.ps1`
Applies Entity Framework Core database migrations.

**Linux/Mac:**
```bash
./scripts/migrate.sh
```

**Windows PowerShell:**
```powershell
.\scripts\migrate.ps1
```

**Options:**
- Environment variable `CONNECTION_STRING` (optional - uses appsettings if not provided)
- Environment variable `ASPNETCORE_ENVIRONMENT` (default: Production)

**Examples:**
```bash
# Run migrations with custom connection string
CONNECTION_STRING="Server=localhost;Database=EccommerceDb;User Id=sa;Password=Pass@123;" ./scripts/migrate.sh
```

```powershell
# Run migrations with custom connection string
.\scripts\migrate.ps1 -ConnectionString "Server=localhost;Database=EccommerceDb;User Id=sa;Password=Pass@123;"
```

## CI/CD Integration

These scripts are designed to be used in CI/CD pipelines. See [.github/workflows/ci-cd.yml](../.github/workflows/ci-cd.yml) for examples.

### GitHub Actions Example

```yaml
- name: Build application
  run: ./scripts/build.sh

- name: Run tests
  run: ./scripts/test.sh

- name: Run migrations
  run: ./scripts/migrate.sh
  env:
    CONNECTION_STRING: ${{ secrets.DB_CONNECTION_STRING }}
```

## Local Development

### First Time Setup

1. **Build the project:**
   ```bash
   ./scripts/build.sh
   ```

2. **Run migrations:**
   ```bash
   ./scripts/migrate.sh
   ```

3. **Run tests** (once test projects are created):
   ```bash
   ./scripts/test.sh
   ```

### Daily Development Workflow

```bash
# Pull latest changes
git pull

# Build the project
./scripts/build.sh

# Run migrations (if new migrations exist)
./scripts/migrate.sh

# Run tests
./scripts/test.sh
```

## Docker Development

For Docker-based development, use Docker Compose instead:

```bash
# Build and run with Docker Compose
docker-compose up --build

# Run migrations in Docker
docker-compose exec api dotnet ef database update
```

## Troubleshooting

### Permission Issues (Linux/Mac)

If you get permission denied errors:
```bash
chmod +x scripts/*.sh
```

### EF Core Tools Not Found

The migration scripts will automatically install EF Core tools if they're not found. If you need to install them manually:

```bash
dotnet tool install --global dotnet-ef
```

### Build Failures

1. Ensure you have .NET 8.0 SDK installed:
   ```bash
   dotnet --version
   ```

2. Clean and restore:
   ```bash
   dotnet clean
   dotnet restore
   ```

3. Check for package conflicts:
   ```bash
   dotnet list package --vulnerable
   ```

## Best Practices

1. **Always run tests before committing:**
   ```bash
   ./scripts/test.sh && git commit
   ```

2. **Use specific configurations:**
   - Use `Debug` for local development
   - Use `Release` for production builds

3. **Keep migrations up to date:**
   - Run migrations after pulling changes
   - Test migrations locally before pushing

4. **Monitor build output:**
   - Review warnings and errors
   - Address code quality issues

## Additional Resources

- [.NET CLI Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [Entity Framework Core Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Docker Documentation](https://docs.docker.com/)
