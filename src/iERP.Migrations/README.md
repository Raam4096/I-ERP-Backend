# iERP Migrations

Central migrations assembly for all module DbContexts.

Each DbContext owns its PostgreSQL schema and migration history table can be shared or separated by context.

## Add migration (example)

```bash
dotnet ef migrations add InitialPlatform --project src/iERP.Migrations --startup-project src/iERP.Api --context PlatformDbContext --output-dir Migrations/Platform
dotnet ef migrations add InitialIdentity --project src/iERP.Migrations --startup-project src/iERP.Api --context IdentityDbContext --output-dir Migrations/Identity
dotnet ef migrations add InitialOrganization --project src/iERP.Migrations --startup-project src/iERP.Api --context OrganizationDbContext --output-dir Migrations/Organization
dotnet ef migrations add InitialMetadata --project src/iERP.Migrations --startup-project src/iERP.Api --context MetadataDbContext --output-dir Migrations/Metadata
dotnet ef migrations add InitialCrm --project src/iERP.Migrations --startup-project src/iERP.Api --context CrmDbContext --output-dir Migrations/Crm
```

Repeat for each context. See `docs/database-migrations.md`.
