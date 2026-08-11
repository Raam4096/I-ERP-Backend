# i-ERP Generated Foundation Plan

## Status

Repository was empty. This plan drives a greenfield modular monolith foundation.

## Project layout (pragmatic)

Avoid one project per layer × module. Prefer **one class library per module** with internal folders:

```
Domain / Application / Infrastructure / Api
```

### Solution projects

| Project | Role |
|---------|------|
| `iERP.Api` | Host, middleware, OpenAPI, module endpoint mapping |
| `iERP.Worker` | Hangfire / outbox processor host (skeleton) |
| `iERP.SharedKernel` | Entities, domain events, clock, exceptions, permissions |
| `iERP.Application.Abstractions` | Cross-cutting app contracts (cache, bus, files, AI, pagination) |
| `iERP.Infrastructure` | EF shared, Redis, Azure placeholders, Identity wiring, DI |
| `iERP.Modules.Platform` | Tenancy, Identity, Organization, Settings, Audit, Attachments, Notifications, Metadata, DynamicModules |
| `iERP.Modules.Engines` | Workflow, Rules, Bridge, Printing |
| `iERP.Modules.CRM` | Leads, Opportunities, Activities + shared Customer/Contact/Address masters |
| `iERP.Modules.Catalog` | Items, UoM, price lists |
| `iERP.Modules.Sales` | Quotations, orders, invoices, credit notes, delivery |
| `iERP.Modules.Procurement` | Vendors, PR/PO/GRN/supplier invoices |
| `iERP.Modules.Inventory` | Warehouses, stock ledger/balances/reservations/transfers |
| `iERP.Modules.Finance` | COA, journals, tax, FX, budgets |
| `iERP.Modules.Banking` | Bank accounts, vouchers, reconciliation |
| `iERP.Modules.Projects` | Projects, contracts, retention, subcontractors |
| `iERP.Modules.HR` | Employees |
| `iERP.Modules.Manufacturing` | BOM, work centres, work orders |
| `iERP.Modules.Assets` | Assets / maintenance |
| `iERP.Modules.Marine` | Vessels, ports |
| `iERP.Modules.Reporting` | Report definitions + read DB factory |
| `iERP.Modules.AI` | Orchestration contracts, tool registry, governance, AI logs |
| `iERP.Migrations` | Central EF migrations assembly |
| `iERP.ArchitectureTests` | Boundary tests |
| `iERP.UnitTests` | Sample unit tests |
| `iERP.IntegrationTests` | Integration test skeleton |

**Decision:** Customer/Contact/Address live in CRM module (operational master shared with Sales via contracts/IDs). Sales references `CustomerId` Guid without writing CRM tables.

## DbContext strategy

| Context | Schema(s) |
|---------|-----------|
| `PlatformDbContext` | platform, audit, notifications, attachments, dynamic, settings (via organization settings tables in organization schema) |
| `IdentityDbContext` | identity |
| `OrganizationDbContext` | organization |
| `MetadataDbContext` | metadata |
| `WorkflowDbContext` | workflow |
| `RulesDbContext` | rules |
| `BridgeDbContext` | bridge |
| `PrintingDbContext` | printing |
| `CrmDbContext` | crm |
| `CatalogDbContext` | catalog |
| `SalesDbContext` | sales |
| `ProcurementDbContext` | procurement |
| `InventoryDbContext` | inventory |
| `FinanceDbContext` | finance |
| `BankingDbContext` | banking |
| `ProjectsDbContext` | projects |
| `HrDbContext` | hr |
| `ManufacturingDbContext` | manufacturing |
| `AssetsDbContext` | assets |
| `MarineDbContext` | marine |
| `ReportingDbContext` | reporting |
| `AiDbContext` | ai |

All share `PrimaryDatabase` connection string. Reporting also has `ReportingDatabase` factory.

## Execution steps

1. Directory.Build.props + Directory.Packages.props + .editorconfig
2. Create solution + projects + references
3. BuildingBlocks
4. Module entities + EF configs + DbContexts + DI + endpoint maps
5. Api + Worker hosts
6. Migrations project + design-time factories
7. Docker / appsettings / .env.example
8. Tests
9. Documentation
10. `dotnet restore && build && test`
11. Optional Docker migrate verification

## Non-goals (this generation)

No business workflows, calculations, posting, AI tools, PDF, real Azure publish, or CRUD beyond health/placeholder auth routes.
