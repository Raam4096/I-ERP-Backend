# Database Migrations

Migrations are centralized in `src/iERP.Migrations`.

Each module DbContext has its own migration history and schema.

## Commands

```bash
dotnet ef migrations add <Name> \
  --project src/iERP.Migrations \
  --startup-project src/iERP.Api \
  --context <DbContextName> \
  --output-dir Migrations/<Area>

dotnet ef database update \
  --project src/iERP.Migrations \
  --startup-project src/iERP.Api \
  --context <DbContextName>
```

## Contexts

`PlatformDbContext`, `IdentityDbContext`, `OrganizationDbContext`, `MetadataDbContext`,
`WorkflowDbContext`, `RulesDbContext`, `BridgeDbContext`, `PrintingDbContext`,
`CrmDbContext`, `CatalogDbContext`, `SalesDbContext`, `ProcurementDbContext`,
`InventoryDbContext`, `FinanceDbContext`, `BankingDbContext`, `ProjectsDbContext`,
`HrDbContext`, `ManufacturingDbContext`, `AssetsDbContext`, `MarineDbContext`,
`ReportingDbContext`, `AiDbContext`

## Scripts

```bash
pwsh ./tools/add-all-migrations.ps1
pwsh ./tools/update-all-databases.ps1
```

Ensure PostgreSQL is running (`docker compose up -d`) before `database update`.
