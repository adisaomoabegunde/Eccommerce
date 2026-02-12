# Deploying Eccommerce API to Render

This guide walks you through deploying the Eccommerce API to Render.com.

## Prerequisites

- [ ] Render account ([Sign up for free](https://render.com))
- [ ] GitHub repository connected to Render
- [ ] External SQL Server database (Azure SQL, AWS RDS, or other)

## Important: Database Consideration

**Render does not natively support SQL Server.**

### Option 1: Use External SQL Server (Recommended for your setup)
- Azure SQL Database (recommended)
- AWS RDS SQL Server
- Any external SQL Server instance

### Option 2: Switch to PostgreSQL
- Render provides managed PostgreSQL
- Requires code changes to use Npgsql instead of SQL Server
- See "Migration to PostgreSQL" section below

This guide covers **Option 1** (External SQL Server).

---

## Step-by-Step Deployment

### Step 1: Set Up External SQL Server Database

#### Option A: Azure SQL Database

1. **Go to Azure Portal** → SQL databases → Create

2. **Configure database:**
   ```
   Resource group: Create new or use existing
   Database name: eccommerce-db
   Server: Create new
   Compute + storage: Basic (5 DTUs) for testing
   ```

3. **Configure firewall:**
   - Go to server → Networking
   - Add rule to allow Azure services: ✅ Allow Azure services
   - For Render access, you may need to allow all IPs (0.0.0.0 - 255.255.255.255) or use Render's outbound IPs

4. **Get connection string:**
   ```
   Server=tcp:your-server.database.windows.net,1433;
   Database=eccommerce-db;
   User Id=yourusername;
   Password=yourpassword;
   Encrypt=True;
   TrustServerCertificate=False;
   Connection Timeout=30;
   ```

#### Option B: AWS RDS SQL Server

1. **Go to AWS Console** → RDS → Create database

2. **Configure:**
   ```
   Engine: Microsoft SQL Server
   Edition: Express (free tier eligible)
   Template: Free tier
   DB instance: eccommerce-db
   Username: admin
   Password: <strong-password>
   ```

3. **Configure security group:**
   - Allow inbound traffic on port 1433 from anywhere (or Render's IPs)

4. **Get endpoint:**
   ```
   Server=your-instance.region.rds.amazonaws.com,1433;
   Database=eccommerce;
   User Id=admin;
   Password=yourpassword;
   ```

### Step 2: Run Database Migrations

**Before deploying**, apply migrations to your external database:

```bash
# Set connection string as environment variable
export CONNECTION_STRING="Server=tcp:your-server.database.windows.net,1433;Database=eccommerce-db;User Id=yourusername;Password=yourpassword;Encrypt=True;"

# Run migrations
./scripts/migrate.sh

# Or on Windows:
$env:CONNECTION_STRING="Server=tcp:your-server.database.windows.net,1433;Database=eccommerce-db;User Id=yourusername;Password=yourpassword;Encrypt=True;"
.\scripts\migrate.ps1
```

### Step 3: Add Health Check Endpoint (CRITICAL)

The Render deployment expects a `/health` endpoint. Add this to your `Program.cs`:

**File:** `Eccommerce.Api/Program.cs`

Add before `app.Run();`:

```csharp
// Health check endpoint for Render
app.MapHealthChecks("/health");
```

**Commit this change:**
```bash
git add Eccommerce.Api/Program.cs
git commit -m "feat: add health check endpoint for deployment"
git push origin feature/setup-cicd
```

### Step 4: Connect GitHub Repository to Render

1. **Log in to Render:** https://dashboard.render.com

2. **Click "New +" → Blueprint**

3. **Connect your GitHub repository:**
   - Click "Connect account" if not already connected
   - Select your repository: `adisaomoabegunde/Eccommerce`
   - Click "Connect"

4. **Render will detect** the `render.yaml` file

### Step 5: Configure Environment Variables

After connecting the repository:

1. **Go to your service** → Environment

2. **Add these environment variables:**

| Key | Value | Secret? |
|-----|-------|---------|
| `ConnectionStrings__Default` | Your SQL Server connection string | ✅ Yes |
| `JwtSettings__Secret` | Generate a strong 64+ char random string | ✅ Yes |
| `JwtSettings__Issuer` | EccommerceApi | ❌ No |
| `JwtSettings__Audience` | EccommerceClient | ❌ No |
| `JwtSettings__ExpiryMinutes` | 60 | ❌ No |
| `ASPNETCORE_ENVIRONMENT` | Production | ❌ No |
| `ASPNETCORE_URLS` | http://+:8080 | ❌ No |

**To generate JWT Secret:**
```bash
# Linux/Mac
openssl rand -base64 64

# PowerShell
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | % {[char]$_})

# Or use: https://generate-secret.vercel.app/64
```

### Step 6: Deploy

1. **Click "Apply"** in Render Dashboard

2. **Render will:**
   - Pull your code from GitHub
   - Build the Docker image
   - Deploy the container
   - Assign a URL: `https://eccommerce-api.onrender.com`

3. **Wait for deployment** (5-10 minutes first time)

4. **Check deployment logs** in Render Dashboard

### Step 7: Verify Deployment

1. **Health Check:**
   ```bash
   curl https://your-app.onrender.com/health
   # Should return: Healthy
   ```

2. **Swagger UI:**
   ```
   https://your-app.onrender.com/swagger
   ```

3. **Test Authentication Endpoint:**
   ```bash
   curl -X POST https://your-app.onrender.com/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"email":"test@example.com","password":"password"}'
   ```

---

## Automatic Deployments

Once set up, Render will automatically deploy when you push to the `main` branch:

```bash
# Merge your feature branch to main
git checkout main
git merge feature/setup-cicd
git push origin main

# Render will automatically deploy!
```

---

## Scaling & Performance

### Free Tier (Starter Plan)
- ✅ 512 MB RAM
- ✅ 0.1 CPU
- ⚠️ Spins down after 15 min of inactivity (cold starts)
- ✅ Free for 90 days, then $7/month

### Paid Plans
- **Standard:** $25/month - 2 GB RAM, always on
- **Pro:** $85/month - 4 GB RAM, auto-scaling

### Upgrade to Paid Plan:
```bash
# In render.yaml, change:
plan: standard  # or pro
```

---

## Troubleshooting

### Issue: Health Check Failing

**Symptom:** Deployment fails with "Health check timeout"

**Solution:**
1. Ensure `/health` endpoint is added to `Program.cs`
2. Check logs for startup errors
3. Verify database connection string is correct

### Issue: Database Connection Fails

**Symptom:** Logs show "Cannot connect to database"

**Solution:**
1. Verify connection string in environment variables
2. Check database firewall allows Render's IPs
3. Test connection string locally:
   ```bash
   CONNECTION_STRING="your-connection-string" ./scripts/migrate.sh
   ```

### Issue: Build Fails

**Symptom:** Docker build fails

**Solution:**
1. Check Dockerfile is in repository root
2. Verify all project files are committed
3. Review build logs in Render Dashboard

### Issue: Cold Starts (Free Tier)

**Symptom:** First request takes 30+ seconds

**Solution:**
- This is normal on free tier (app spins down after 15 min)
- Upgrade to Standard plan ($25/mo) for always-on service
- Or use a service like [UptimeRobot](https://uptimerobot.com) to ping your app every 5 minutes

---

## Render vs Docker Compose Differences

| Feature | Docker Compose (Local) | Render (Production) |
|---------|------------------------|---------------------|
| Database | SQL Server container | External SQL Server required |
| Environment | `.env` file | Render Dashboard env vars |
| Port | localhost:5000 | https://your-app.onrender.com |
| Restart | Manual | Automatic (always running) |
| Scaling | Single instance | Can scale horizontally |
| HTTPS | No | Yes (automatic SSL) |

---

## Alternative: PostgreSQL on Render (Option 2)

If you want to use Render's managed PostgreSQL instead:

### Benefits:
- ✅ Free PostgreSQL database (256 MB)
- ✅ Automatic backups
- ✅ No external database needed
- ✅ Lower latency (same region)

### Drawbacks:
- ❌ Requires code changes
- ❌ Migration of existing data

### Steps to Migrate:

1. **Update NuGet packages:**
   ```bash
   # Remove SQL Server packages
   dotnet remove Eccommerce.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer

   # Add PostgreSQL packages
   dotnet add Eccommerce.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
   ```

2. **Update DbContext configuration:**
   ```csharp
   // In DependencyInjection.cs
   services.AddDbContext<AppDbContext>(options =>
       options.UseNpgsql(configuration.GetConnectionString("Default")));
   ```

3. **Update connection string format:**
   ```
   Host=your-postgres.render.com;Database=eccommerce;Username=user;Password=pass;SSL Mode=Require;Trust Server Certificate=true;
   ```

4. **Create new migrations:**
   ```bash
   # Remove old migrations
   rm -rf Eccommerce.Infrastructure/Migrations/*

   # Create new migrations for PostgreSQL
   dotnet ef migrations add InitialCreate --project Eccommerce.Infrastructure
   ```

5. **Update render.yaml:**
   ```yaml
   databases:
     - name: eccommerce-db
       plan: starter # Free tier
       postgresMajorVersion: 16

   services:
     - type: web
       name: eccommerce-api
       # ... existing config ...
       envVars:
         - key: ConnectionStrings__Default
           fromDatabase:
             name: eccommerce-db
             property: connectionString
   ```

---

## Cost Estimation

### Option 1: External SQL Server

| Service | Tier | Cost |
|---------|------|------|
| Render Web Service | Starter (Free 90 days) | $0 → $7/month |
| Azure SQL Database | Basic (5 DTU) | ~$5/month |
| AWS RDS SQL Server | db.t3.micro | ~$13/month |
| **Total** | | **$5-20/month** |

### Option 2: PostgreSQL on Render

| Service | Tier | Cost |
|---------|------|------|
| Render Web Service | Starter | $0 → $7/month |
| Render PostgreSQL | Starter (256MB) | $0 → $7/month |
| **Total** | | **$0-14/month** |

---

## Monitoring & Logs

### View Logs:
1. Go to Render Dashboard → Your Service → Logs
2. Real-time log streaming
3. Filter by log level

### Set Up Alerts:
- Render → Settings → Notifications
- Configure email/Slack notifications for:
  - Deploy failures
  - Health check failures
  - High resource usage

---

## Custom Domain

To use your own domain:

1. **Go to** Render Dashboard → Your Service → Settings

2. **Add Custom Domain:** e.g., `api.yourdomain.com`

3. **Add DNS Records** at your domain registrar:
   ```
   Type: CNAME
   Name: api
   Value: your-app.onrender.com
   ```

4. **Render automatically provisions SSL** (via Let's Encrypt)

---

## CI/CD Integration

Your GitHub Actions pipeline and Render work together:

1. **GitHub Actions:**
   - Builds and tests on every push
   - Builds Docker image
   - Runs security scans

2. **Render:**
   - Automatically deploys when you push to `main`
   - Pulls latest code
   - Rebuilds Docker image
   - Deploys with zero downtime

---

## Next Steps

1. ✅ Add health check endpoint to Program.cs
2. ✅ Set up external SQL Server database
3. ✅ Run database migrations
4. ✅ Connect GitHub repository to Render
5. ✅ Configure environment variables
6. ✅ Deploy and verify

**Need help?** Check [Render Documentation](https://render.com/docs) or let me know!

---

## Useful Links

- [Render Dashboard](https://dashboard.render.com)
- [Render Docker Deployment](https://render.com/docs/docker)
- [Render Environment Variables](https://render.com/docs/environment-variables)
- [Azure SQL Database](https://azure.microsoft.com/en-us/products/azure-sql/database)
- [AWS RDS SQL Server](https://aws.amazon.com/rds/sqlserver/)

---

**Last Updated:** 2026-02-12
