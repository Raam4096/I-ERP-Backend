$ErrorActionPreference = 'Stop'
$contexts = @(
  @{ Name = 'PlatformDbContext'; Dir = 'Platform' },
  @{ Name = 'IdentityDbContext'; Dir = 'Identity' },
  @{ Name = 'OrganizationDbContext'; Dir = 'Organization' },
  @{ Name = 'MetadataDbContext'; Dir = 'Metadata' },
  @{ Name = 'WorkflowDbContext'; Dir = 'Workflow' },
  @{ Name = 'RulesDbContext'; Dir = 'Rules' },
  @{ Name = 'BridgeDbContext'; Dir = 'Bridge' },
  @{ Name = 'PrintingDbContext'; Dir = 'Printing' },
  @{ Name = 'CrmDbContext'; Dir = 'Crm' },
  @{ Name = 'CatalogDbContext'; Dir = 'Catalog' },
  @{ Name = 'SalesDbContext'; Dir = 'Sales' },
  @{ Name = 'ProcurementDbContext'; Dir = 'Procurement' },
  @{ Name = 'InventoryDbContext'; Dir = 'Inventory' },
  @{ Name = 'FinanceDbContext'; Dir = 'Finance' },
  @{ Name = 'BankingDbContext'; Dir = 'Banking' },
  @{ Name = 'ProjectsDbContext'; Dir = 'Projects' },
  @{ Name = 'HrDbContext'; Dir = 'Hr' },
  @{ Name = 'ManufacturingDbContext'; Dir = 'Manufacturing' },
  @{ Name = 'AssetsDbContext'; Dir = 'Assets' },
  @{ Name = 'MarineDbContext'; Dir = 'Marine' },
  @{ Name = 'ReportingDbContext'; Dir = 'Reporting' },
  @{ Name = 'AiDbContext'; Dir = 'Ai' }
)

foreach ($ctx in $contexts) {
  Write-Host "Adding Initial$($ctx.Dir) for $($ctx.Name)"
  dotnet ef migrations add "Initial$($ctx.Dir)" `
    --project src/iERP.Migrations `
    --startup-project src/iERP.Api `
    --context $ctx.Name `
    --output-dir "Migrations/$($ctx.Dir)"
}
