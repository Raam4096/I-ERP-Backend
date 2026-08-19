# i-ERP

Multi-tenant ERP SaaS platform foundation (modular monolith) built on .NET 8, PostgreSQL, and EF Core.

This repository currently contains the **architectural foundation only**: modules, entities, DbContexts, abstractions, DI, middleware, migration infrastructure, and documentation. Business workflows are intentionally not implemented.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL / Redis)
- EF Core tools (optional, for migrations):

```bash
dotnet tool install --global dotnet-ef
```

## Quick start

### 1. Start infrastructure

```bash
docker compose up -d
```

This starts PostgreSQL (`localhost:5432`, db: `ierp_dev`, user/password: `ierp`) and Redis (`localhost:6379`).

Optional pgAdmin:

```bash
docker compose --profile tools up -d
```

pgAdmin: http://localhost:5050 (`admin@ierp.local` / `admin`)

### 2. Restore and build

```bash
dotnet restore
dotnet build
```

### 3. Apply database migrations

Create and apply migrations per DbContext (see [docs/database-migrations.md](docs/database-migrations.md)). Example for platform:

```bash
dotnet ef migrations add InitialPlatform --project src/iERP.Migrations --startup-project src/iERP.Api --context PlatformDbContext --output-dir Migrations/Platform
dotnet ef database update --project src/iERP.Migrations --startup-project src/iERP.Api --context PlatformDbContext
```

Repeat for other contexts (`IdentityDbContext`, `CrmDbContext`, `SalesDbContext`, ...).

A helper script is available:

```bash
pwsh ./tools/add-all-migrations.ps1
pwsh ./tools/update-all-databases.ps1
```

### 4. Run API

```bash
dotnet run --project src/iERP.Api
```

- API: http://localhost:5080
- Swagger UI: http://localhost:5080/swagger
- Health: http://localhost:5080/health
- Live: http://localhost:5080/health/live
- Ready: http://localhost:5080/health/ready

Azure OpenAI, Service Bus, Blob Storage, Hangfire, and Redis are **optional** and disabled by default for local development.

### 5. Run tests

```bash
dotnet test
```

## Solution layout

```
src/
  iERP.Api/                  # HTTP host
  iERP.Worker/               # Outbox / background host skeleton
  iERP.Migrations/           # Central EF migrations
  BuildingBlocks/            # SharedKernel, Application.Abstractions, Infrastructure
  Modules/                   # Platform, Engines, CRM, Sales, ...
tests/
docs/
```

## Documentation

- [Architecture](docs/architecture.md)
- [Database architecture](docs/database-architecture.md)
- [Module boundaries](docs/module-boundaries.md)
- [Multi-tenancy](docs/multi-tenancy.md)
- [Development guide](docs/development-guide.md)
- [Adding a module](docs/adding-a-module.md)
- [Adding an API](docs/adding-an-api.md)
- [Adding an entity](docs/adding-an-entity.md)
- [Database migrations](docs/database-migrations.md)
- [Architecture decisions](docs/architecture-decisions.md)
- [UI API integration](docs/ui-api-integration.md)
- [Frontend auth integration](docs/FRONTEND_AUTH_INTEGRATION.md)
- [JWT auth implementation](docs/JWT_AUTH_IMPLEMENTATION.md)
- [CRM lead management](docs/crm-lead-management.md)
- [Generated plan](docs/generated-plan.md)

## Configuration

Copy `.env.example` for environment variable overrides. Strongly typed options:

- `ConnectionStrings:PrimaryDatabase`
- `ConnectionStrings:ReportingDatabase`
- `Jwt`
- `Redis`
- `Hangfire`
- `AzureServiceBus`
- `AzureOpenAI`
- `AzureBlobStorage`
