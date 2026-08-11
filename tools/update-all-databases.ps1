$ErrorActionPreference = 'Stop'
$contexts = @(
  'PlatformDbContext','IdentityDbContext','OrganizationDbContext','MetadataDbContext',
  'WorkflowDbContext','RulesDbContext','BridgeDbContext','PrintingDbContext',
  'CrmDbContext','CatalogDbContext','SalesDbContext','ProcurementDbContext',
  'InventoryDbContext','FinanceDbContext','BankingDbContext','ProjectsDbContext',
  'HrDbContext','ManufacturingDbContext','AssetsDbContext','MarineDbContext',
  'ReportingDbContext','AiDbContext'
)

foreach ($ctx in $contexts) {
  Write-Host "Updating $ctx"
  dotnet ef database update `
    --project src/iERP.Migrations `
    --startup-project src/iERP.Api `
    --context $ctx
}
