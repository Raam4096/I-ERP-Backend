namespace iERP.Application.Abstractions.Metadata;

/// <summary>
/// Product (predefined) modules/screens matching the i-ERP UI settings catalog.
/// Seeded into metadata for every tenant. Screens without a backend set
/// <see cref="PredefinedScreenSpec.RenderMode"/> to <c>under_implementation</c>.
/// </summary>
public static class PredefinedModulesCatalog
{
    public const string UnderImplementationRenderMode = "under_implementation";
    public const string GenericRenderMode = "generic";

    public static IReadOnlyList<PredefinedModuleSpec> Modules { get; } =
    [
        new("sales-distribution", "Sales & Distribution", "Sales quotations, orders, and invoicing",
        [
            Screen("quotation-management", "Quotation Management", "/sales/quotations", "/api/v1/sales_quotations"),
            Screen("sales-orders", "Sales Orders", "/sales/orders", "/api/v1/sales_orders"),
            Screen("invoice-management", "Invoice Management", "/sales/invoices", "/api/v1/sales/invoices"),
        ]),
        new("procurement-hub", "Procurement Hub", "Purchase requests, orders, and supplier invoices",
        [
            Screen("purchase-requests", "Purchase Requests", "/procurement/requests", "/api/v1/purchase_orders/requests"),
            Screen("purchase-orders", "Purchase Orders", "/procurement/orders", "/api/v1/purchase_orders"),
            Screen("supplier-invoices", "Supplier Invoices", "/procurement/supplier-invoices", "/api/v1/purchase_orders/supplier-invoices"),
        ]),
        new("inventory-supply-chain", "Inventory & Supply Chain", "Items, warehouses, and stock movement",
        [
            Screen("item-management", "Item Management", "/inventory/items", "/api/v1/items"),
            Screen("warehouse-management", "Warehouse Management", "/inventory/warehouses", "/api/v1/inventory/warehouses"),
            Screen("stock-transfers", "Stock Transfers", "/inventory/stock-transfers", "/api/v1/inventory/stock-transfers"),
        ]),
        new("finance-treasury", "Finance & Treasury", "Ledger and payables / receivables",
        [
            Screen("general-ledger", "General Ledger", "/finance/ledger", "/api/v1/finance/ledger"),
            Screen("accounts-payable", "Accounts Payable", "/finance/ap", "/api/v1/finance/ap"),
            Screen("accounts-receivable", "Accounts Receivable", "/finance/ar", "/api/v1/finance/ar"),
        ]),
        new("crm", "CRM & Customer Engagement", "Customer relationship management",
        [
            Screen("crm-mission-control", "CRM Mission Control", "/crm/mission-control", "/api/v1/dashboard/snapshot"),
            // Implemented Hybrid screens — full field layouts seeded separately.
            new PredefinedScreenSpec(
                CrmLeadsScreenCatalog.ScreenCode,
                "Lead Management",
                CrmLeadsScreenCatalog.Route,
                CrmLeadsScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
            Screen("contact-directory", "Contact Directory", "/crm/contacts", "/api/v1/customers"),
            new PredefinedScreenSpec(
                "crm-opportunities",
                "Opportunity Pipeline",
                "/crm/opportunities",
                "/api/v1/crm/opportunities",
                GenericRenderMode,
                IsImplemented: true),
            Screen("activities-follow-ups", "Activities & Follow-Ups", "/crm/activities", "/api/v1/crm/activities"),
            Screen("campaign-manager", "Campaign Manager", "/crm/campaigns", "/api/v1/crm/campaigns"),
        ]),
        new("hr-payroll", "HR & Payroll", "Employees, leave, and payroll",
        [
            Screen("employee-management", "Employee Management", "/hr/employees", "/api/v1/hr/employees"),
            Screen("leave-management", "Leave Management", "/hr/leave", "/api/v1/hr/leave"),
            Screen("payroll-processing", "Payroll Processing", "/hr/payroll", "/api/v1/hr/payroll"),
        ]),
        new("project-management", "Project Management", "Projects, tasks, and billing",
        [
            Screen("project-portfolio", "Project Portfolio", "/projects/portfolio", "/api/v1/projects"),
            Screen("project-tasks", "Project Tasks", "/projects/tasks", "/api/v1/projects/tasks"),
            Screen("project-billing", "Project Billing", "/projects/billing", "/api/v1/projects/billing"),
        ]),
        new("manufacturing", "Manufacturing", "Planning, work orders, and quality",
        [
            Screen("production-planning", "Production Planning", "/manufacturing/planning", "/api/v1/manufacturing/planning"),
            Screen("work-orders", "Work Orders", "/manufacturing/work-orders", "/api/v1/manufacturing/work-orders"),
            Screen("quality-control", "Quality Control", "/manufacturing/quality", "/api/v1/manufacturing/quality"),
        ]),
    ];

    private static PredefinedScreenSpec Screen(string code, string name, string route, string apiBasePath) =>
        new(code, name, route, apiBasePath, UnderImplementationRenderMode, IsImplemented: false);
}

public sealed record PredefinedModuleSpec(
    string Code,
    string Name,
    string? Description,
    IReadOnlyList<PredefinedScreenSpec> Screens);

public sealed record PredefinedScreenSpec(
    string Code,
    string Name,
    string Route,
    string ApiBasePath,
    string RenderMode,
    bool IsImplemented);
