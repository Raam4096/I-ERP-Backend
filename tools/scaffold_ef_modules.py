#!/usr/bin/env python3
from __future__ import annotations
from pathlib import Path
from textwrap import dedent

ROOT = Path(__file__).resolve().parents[1]

def write(rel: str, content: str) -> None:
    path = ROOT / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(dedent(content).lstrip("\n").replace("\r\n", "\n"), encoding="utf-8")
    print(rel)

def gen_module(
    project_rel: str,
    module_ns: str,
    context_name: str,
    schema: str,
    entities: list[tuple[str, str, str]],  # entity, table, extras
    route: str,
    endpoint_name: str,
    extra_di: str = "",
    extra_usings_di: str = "",
):
    # configs
    for entity, table, extras in entities:
        write(f"{project_rel}/Infrastructure/Configurations/{entity}Configuration.cs", f"""
using iERP.Infrastructure.Persistence;
using {module_ns}.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace {module_ns}.Infrastructure.Configurations;

public sealed class {entity}Configuration : AuditableEntityConfiguration<{entity}>
{{
    public override void Configure(EntityTypeBuilder<{entity}> builder)
    {{
        base.Configure(builder);
        builder.ToTable("{table}", "{schema}");
{extras}
    }}
}}
""")

    dbsets = []
    for entity, _, _ in entities:
        plural = entity + "s"
        for a, b in [
            ("Addresss", "Addresses"),
            ("Activitys", "Activities"),
            ("Opportunitys", "Opportunities"),
            ("Categorys", "Categories"),
            ("Historys", "Histories"),
            ("Currencys", "Currencies"),
            ("Subsidiarys", "Subsidiaries"),
            ("BillOfMaterialss", "BillOfMaterials"),
            ("BillOfMaterialsLines", "BillOfMaterialsLines"),
        ]:
            plural = plural.replace(a, b)
        dbsets.append(f"    public DbSet<{entity}> {plural} => Set<{entity}>();")

    write(f"{project_rel}/Infrastructure/{context_name}.cs", f"""
using iERP.Infrastructure.Persistence;
using {module_ns}.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace {module_ns}.Infrastructure;

public sealed class {context_name} : DbContext
{{
    private readonly ITenantContext _tenantContext;

    public {context_name}(DbContextOptions<{context_name}> options, ITenantContext tenantContext) : base(options)
    {{
        _tenantContext = tenantContext;
    }}

{chr(10).join(dbsets)}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {{
        modelBuilder.HasDefaultSchema("{schema}");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof({context_name}).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }}
}}
""")

    write(f"{project_rel}/Api/{endpoint_name}Endpoints.cs", f"""
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace {module_ns}.Api;

public static class {endpoint_name}Endpoints
{{
    public static IEndpointRouteBuilder Map{endpoint_name}Endpoints(this IEndpointRouteBuilder app)
    {{
        var group = app.MapGroup("/api/v1/{route}").WithTags("{endpoint_name}");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("{endpoint_name} module ready")))
            .WithName("{endpoint_name}Health");
        return app;
    }}
}}
""")

    write(f"{project_rel}/DependencyInjection.cs", f"""
using iERP.Infrastructure.Persistence.Interceptors;
using {module_ns}.Infrastructure;
{extra_usings_di}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace {module_ns};

public static class DependencyInjection
{{
    public static IServiceCollection Add{endpoint_name}Module(this IServiceCollection services, IConfiguration configuration)
    {{
        var connectionString = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp";

        services.AddDbContext<{context_name}>((sp, options) =>
        {{
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("iERP.Migrations"));
            options.AddInterceptors(
                sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditSaveChangesInterceptor>());
        }});
{extra_di}
        return services;
    }}
}}
""")


# ENGINES - multiple contexts in one project
E = "src/Modules/Engines/iERP.Modules.Engines"

engine_entities = {
    "Workflow": [
        ("WorkflowDefinition", "workflow_definitions", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
        ("WorkflowStep", "workflow_steps", '        builder.HasIndex(x => new { x.TenantId, x.WorkflowDefinitionId, x.Code }).IsUnique();'),
        ("WorkflowInstance", "workflow_instances", '        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.RecordId });'),
        ("WorkflowHistory", "workflow_histories", '        builder.HasIndex(x => new { x.TenantId, x.WorkflowInstanceId });'),
    ],
    "Rules": [
        ("RuleDefinition", "rule_definitions", '        builder.Property(x => x.Conditions).HasColumnType("jsonb");\n        builder.Property(x => x.Actions).HasColumnType("jsonb");\n        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.EventName, x.Priority });'),
    ],
    "Bridge": [
        ("BridgeDefinition", "bridge_definitions", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
        ("BridgeMapping", "bridge_mappings", '        builder.HasIndex(x => new { x.TenantId, x.BridgeDefinitionId, x.SourceField });'),
        ("BridgeLog", "bridge_logs", '        builder.HasIndex(x => new { x.TenantId, x.BridgeDefinitionId, x.SourceRecordId });'),
    ],
    "Printing": [
        ("PrintTemplate", "print_templates", '        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.TemplateCode }).IsUnique();'),
        ("PrintTemplateVersion", "print_template_versions", '        builder.HasIndex(x => new { x.TenantId, x.PrintTemplateId, x.Version }).IsUnique();'),
    ],
}

for area, ents in engine_entities.items():
    schema = area.lower()
    for entity, table, extras in ents:
        write(f"{E}/{area}/Infrastructure/Configurations/{entity}Configuration.cs", f"""
using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.{area}.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.{area}.Infrastructure.Configurations;

public sealed class {entity}Configuration : AuditableEntityConfiguration<{entity}>
{{
    public override void Configure(EntityTypeBuilder<{entity}> builder)
    {{
        base.Configure(builder);
        builder.ToTable("{table}", "{schema}");
{extras}
    }}
}}
""")

write(f"{E}/Workflow/Infrastructure/WorkflowDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Workflow.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Engines.Workflow.Infrastructure;

public sealed class WorkflowDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options, ITenantContext tenantContext) : base(options) => _tenantContext = tenantContext;
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowHistory> WorkflowHistories => Set<WorkflowHistory>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("workflow");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write(f"{E}/Rules/Infrastructure/RulesDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Rules.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Engines.Rules.Infrastructure;

public sealed class RulesDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    public RulesDbContext(DbContextOptions<RulesDbContext> options, ITenantContext tenantContext) : base(options) => _tenantContext = tenantContext;
    public DbSet<RuleDefinition> RuleDefinitions => Set<RuleDefinition>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("rules");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RulesDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write(f"{E}/Bridge/Infrastructure/BridgeDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Bridge.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Engines.Bridge.Infrastructure;

public sealed class BridgeDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    public BridgeDbContext(DbContextOptions<BridgeDbContext> options, ITenantContext tenantContext) : base(options) => _tenantContext = tenantContext;
    public DbSet<BridgeDefinition> BridgeDefinitions => Set<BridgeDefinition>();
    public DbSet<BridgeMapping> BridgeMappings => Set<BridgeMapping>();
    public DbSet<BridgeLog> BridgeLogs => Set<BridgeLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("bridge");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BridgeDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write(f"{E}/Printing/Infrastructure/PrintingDbContext.cs", """
using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Printing.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Engines.Printing.Infrastructure;

public sealed class PrintingDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    public PrintingDbContext(DbContextOptions<PrintingDbContext> options, ITenantContext tenantContext) : base(options) => _tenantContext = tenantContext;
    public DbSet<PrintTemplate> PrintTemplates => Set<PrintTemplate>();
    public DbSet<PrintTemplateVersion> PrintTemplateVersions => Set<PrintTemplateVersion>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("printing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrintingDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
""")

write(f"{E}/Api/EnginesEndpoints.cs", """
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Engines.Api;

public static class EnginesEndpoints
{
    public static IEndpointRouteBuilder MapEnginesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/v1/workflows").WithTags("Workflows")
            .MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Workflow module ready")));
        app.MapGroup("/api/v1/rules").WithTags("Rules")
            .MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Rules module ready")));
        app.MapGroup("/api/v1/bridges").WithTags("Bridges")
            .MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Bridge module ready")));
        app.MapGroup("/api/v1/printing").WithTags("Printing")
            .MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Printing module ready")));
        return app;
    }
}
""")

write(f"{E}/DependencyInjection.cs", """
using iERP.Application.Abstractions.Engines;
using iERP.Infrastructure.Persistence.Interceptors;
using iERP.Modules.Engines.Bridge.Application;
using iERP.Modules.Engines.Bridge.Infrastructure;
using iERP.Modules.Engines.Printing.Application;
using iERP.Modules.Engines.Printing.Infrastructure;
using iERP.Modules.Engines.Rules.Application;
using iERP.Modules.Engines.Rules.Infrastructure;
using iERP.Modules.Engines.Workflow.Application;
using iERP.Modules.Engines.Workflow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iERP.Modules.Engines;

public static class DependencyInjection
{
    public static IServiceCollection AddEnginesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("PrimaryDatabase")
            ?? "Host=localhost;Port=5432;Database=ierp;Username=ierp;Password=ierp";

        void AddCtx<TContext>() where TContext : DbContext =>
            services.AddDbContext<TContext>((sp, options) =>
            {
                options.UseNpgsql(cs, b => b.MigrationsAssembly("iERP.Migrations"));
                options.AddInterceptors(
                    sp.GetRequiredService<TenantSaveChangesInterceptor>(),
                    sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });

        AddCtx<WorkflowDbContext>();
        AddCtx<RulesDbContext>();
        AddCtx<BridgeDbContext>();
        AddCtx<PrintingDbContext>();

        services.AddScoped<IWorkflowEngine, NullWorkflowEngine>();
        services.AddScoped<IRuleEngine, NullRuleEngine>();
        services.AddScoped<IBridgeEngine, NullBridgeEngine>();
        services.AddScoped<IPrintEngine, NullPrintEngine>();
        return services;
    }
}
""")

# Business modules
gen_module(
    "src/Modules/CRM/iERP.Modules.CRM",
    "iERP.Modules.CRM",
    "CrmDbContext",
    "crm",
    [
        ("Lead", "leads", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.LeadNumber }).IsUnique();'),
        ("Opportunity", "opportunities", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.OpportunityNumber }).IsUnique();'),
        ("Activity", "activities", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId });'),
        ("Customer", "customers", '        builder.HasIndex(x => new { x.TenantId, x.CustomerCode }).IsUnique();'),
        ("Contact", "contacts", '        builder.HasIndex(x => new { x.TenantId, x.CustomerId, x.Email });'),
        ("Address", "addresses", '        builder.HasIndex(x => new { x.TenantId, x.CustomerId, x.AddressType });'),
    ],
    "leads",
    "Crm",
)

# Also map customers route
write("src/Modules/CRM/iERP.Modules.CRM/Api/CustomerEndpoints.cs", """
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.CRM.Api;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/customers").WithTags("Customers");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Customers ready")))
            .WithName("CustomersHealth");
        return app;
    }
}
""")

gen_module(
    "src/Modules/Catalog/iERP.Modules.Catalog",
    "iERP.Modules.Catalog",
    "CatalogDbContext",
    "catalog",
    [
        ("ItemCategory", "item_categories", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
        ("UnitOfMeasure", "units_of_measure", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
        ("UnitOfMeasureConversion", "unit_of_measure_conversions", '        builder.HasIndex(x => new { x.TenantId, x.FromUomId, x.ToUomId }).IsUnique();'),
        ("Item", "items", '        builder.HasIndex(x => new { x.TenantId, x.ItemCode }).IsUnique();'),
        ("PriceList", "price_lists", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
        ("PriceListItem", "price_list_items", '        builder.HasIndex(x => new { x.TenantId, x.PriceListId, x.ItemId }).IsUnique();'),
    ],
    "items",
    "Catalog",
)

sales_docs = [
    ("SalesQuotation", "sales_quotations"),
    ("SalesQuotationLine", "sales_quotation_lines"),
    ("SalesOrder", "sales_orders"),
    ("SalesOrderLine", "sales_order_lines"),
    ("SalesInvoice", "sales_invoices"),
    ("SalesInvoiceLine", "sales_invoice_lines"),
    ("CreditNote", "credit_notes"),
    ("CreditNoteLine", "credit_note_lines"),
    ("DeliveryOrder", "delivery_orders"),
    ("DeliveryOrderLine", "delivery_order_lines"),
]
sales_entities = []
for e, t in sales_docs:
    if e.endswith("Line"):
        extras = '        builder.HasIndex(x => new { x.TenantId, x.Id });'
    else:
        extras = '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'
    sales_entities.append((e, t, extras))

gen_module(
    "src/Modules/Sales/iERP.Modules.Sales",
    "iERP.Modules.Sales",
    "SalesDbContext",
    "sales",
    sales_entities,
    "sales_quotations",
    "Sales",
)

write("src/Modules/Sales/iERP.Modules.Sales/Api/SalesExtraEndpoints.cs", """
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Sales.Api;

public static class SalesExtraEndpoints
{
    public static IEndpointRouteBuilder MapSalesExtraEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/v1/sales_orders").WithTags("SalesOrders")
            .MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Sales orders ready")));
        return app;
    }
}
""")

proc_docs = [
    ("Vendor", "vendors", '        builder.HasIndex(x => new { x.TenantId, x.VendorCode }).IsUnique();'),
    ("PurchaseRequest", "purchase_requests", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("PurchaseRequestLine", "purchase_request_lines", ""),
    ("PurchaseOrder", "purchase_orders", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("PurchaseOrderLine", "purchase_order_lines", ""),
    ("GoodsReceivedNote", "goods_received_notes", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("GoodsReceivedNoteLine", "goods_received_note_lines", ""),
    ("SupplierInvoice", "supplier_invoices", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("SupplierInvoiceLine", "supplier_invoice_lines", ""),
]
gen_module(
    "src/Modules/Procurement/iERP.Modules.Procurement",
    "iERP.Modules.Procurement",
    "ProcurementDbContext",
    "procurement",
    proc_docs,
    "purchase_orders",
    "Procurement",
)

inv = [
    ("Warehouse", "warehouses", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Code }).IsUnique();'),
    ("BinLocation", "bin_locations", '        builder.HasIndex(x => new { x.TenantId, x.WarehouseId, x.Code }).IsUnique();'),
    ("InventoryTransaction", "inventory_transactions", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("InventoryTransactionLine", "inventory_transaction_lines", ""),
    ("StockBalance", "stock_balances", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.WarehouseId, x.BinLocationId, x.ItemId }).IsUnique();'),
    ("StockReservation", "stock_reservations", '        builder.HasIndex(x => new { x.TenantId, x.SourceEntityName, x.SourceRecordId });'),
    ("StockTransfer", "stock_transfers", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("StockTransferLine", "stock_transfer_lines", ""),
]
gen_module("src/Modules/Inventory/iERP.Modules.Inventory", "iERP.Modules.Inventory", "InventoryDbContext", "inventory", inv, "inventory", "Inventory")

fin = [
    ("ChartOfAccount", "chart_of_accounts", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.AccountCode }).IsUnique();'),
    ("FiscalYear", "fiscal_years", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Name }).IsUnique();'),
    ("AccountingPeriod", "accounting_periods", ""),
    ("JournalEntry", "journal_entries", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("JournalEntryLine", "journal_entry_lines", ""),
    ("TaxCode", "tax_codes", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("Currency", "currencies", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("ExchangeRate", "exchange_rates", '        builder.HasIndex(x => new { x.TenantId, x.FromCurrencyCode, x.ToCurrencyCode, x.RateDate }).IsUnique();'),
    ("Budget", "budgets", ""),
    ("BudgetLine", "budget_lines", ""),
    ("WithholdingTaxCode", "withholding_tax_codes", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("IntercompanyConfiguration", "intercompany_configurations", '        builder.HasIndex(x => new { x.TenantId, x.SourceSubsidiaryId, x.TargetSubsidiaryId }).IsUnique();'),
]
gen_module("src/Modules/Finance/iERP.Modules.Finance", "iERP.Modules.Finance", "FinanceDbContext", "finance", fin, "finance", "Finance")

bank = [
    ("BankAccount", "bank_accounts", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.AccountCode }).IsUnique();'),
    ("PaymentMethod", "payment_methods", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("PaymentVoucher", "payment_vouchers", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("PaymentVoucherLine", "payment_voucher_lines", ""),
    ("ReceiptVoucher", "receipt_vouchers", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("ReceiptVoucherLine", "receipt_voucher_lines", ""),
    ("BankReconciliation", "bank_reconciliations", ""),
]
gen_module("src/Modules/Banking/iERP.Modules.Banking", "iERP.Modules.Banking", "BankingDbContext", "banking", bank, "banking", "Banking")

proj = [
    ("Project", "projects", '        builder.HasIndex(x => new { x.TenantId, x.ProjectCode }).IsUnique();'),
    ("Contract", "contracts", '        builder.HasIndex(x => new { x.TenantId, x.ContractNo }).IsUnique();'),
    ("RetentionRule", "retention_rules", ""),
    ("Subcontractor", "subcontractors", ""),
]
gen_module("src/Modules/Projects/iERP.Modules.Projects", "iERP.Modules.Projects", "ProjectsDbContext", "projects", proj, "projects", "Projects")

gen_module(
    "src/Modules/HR/iERP.Modules.HR",
    "iERP.Modules.HR",
    "HrDbContext",
    "hr",
    [("Employee", "employees", '        builder.HasIndex(x => new { x.TenantId, x.EmployeeCode }).IsUnique();')],
    "hr",
    "Hr",
)

mfg = [
    ("BillOfMaterials", "bills_of_materials", '        builder.HasIndex(x => new { x.TenantId, x.ItemId, x.Version }).IsUnique();'),
    ("BillOfMaterialsLine", "bill_of_materials_lines", ""),
    ("WorkCentre", "work_centres", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("WorkOrder", "work_orders", '        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();'),
    ("WorkOrderLine", "work_order_lines", ""),
]
gen_module("src/Modules/Manufacturing/iERP.Modules.Manufacturing", "iERP.Modules.Manufacturing", "ManufacturingDbContext", "manufacturing", mfg, "manufacturing", "Manufacturing")

assets = [
    ("AssetType", "asset_types", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("Asset", "assets", '        builder.HasIndex(x => new { x.TenantId, x.AssetCode }).IsUnique();'),
    ("AssetMaintenanceSchedule", "asset_maintenance_schedules", ""),
]
gen_module("src/Modules/Assets/iERP.Modules.Assets", "iERP.Modules.Assets", "AssetsDbContext", "assets", assets, "assets", "Assets")

marine = [
    ("PortLocation", "port_locations", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();'),
    ("Vessel", "vessels", '        builder.HasIndex(x => new { x.TenantId, x.VesselCode }).IsUnique();'),
]
gen_module("src/Modules/Marine/iERP.Modules.Marine", "iERP.Modules.Marine", "MarineDbContext", "marine", marine, "marine", "Marine")

gen_module(
    "src/Modules/Reporting/iERP.Modules.Reporting",
    "iERP.Modules.Reporting",
    "ReportingDbContext",
    "reporting",
    [("ReportDefinition", "report_definitions", '        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();')],
    "reports",
    "Reporting",
)

gen_module(
    "src/Modules/AI/iERP.Modules.AI",
    "iERP.Modules.AI",
    "AiDbContext",
    "ai",
    [
        ("AIToolDefinition", "ai_tool_definitions", '        builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();\n        builder.Property(x => x.Name).HasMaxLength(128);'),
        ("AIToolPermission", "ai_tool_permissions", '        builder.HasIndex(x => new { x.TenantId, x.AIToolDefinitionId, x.RoleId, x.AllowedExecutionMode }).IsUnique();'),
        ("AILog", "ai_logs", '        builder.Property(x => x.RollbackPayload).HasColumnType("jsonb");\n        builder.HasIndex(x => new { x.TenantId, x.UserId, x.CreatedAt });'),
    ],
    "ai",
    "Ai",
    extra_di="""
        services.AddSingleton<iERP.Application.Abstractions.AI.IAIToolRegistry, Application.AIToolRegistry>();
        services.AddScoped<iERP.Application.Abstractions.AI.IAIGovernanceService, Application.NullAIGovernanceService>();
        services.AddScoped<iERP.Application.Abstractions.AI.IAIOrchestrator, Application.NullAIOrchestrator>();
""",
)

print("ef modules done")
