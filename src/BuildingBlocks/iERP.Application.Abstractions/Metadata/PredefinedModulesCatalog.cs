namespace iERP.Application.Abstractions.Metadata;

/// <summary>
/// Product (predefined) modules/screens.
/// CRM: Leads + Opportunities only.
/// Sales: Enquiry, Quotation, Orders, Debit/Credit Notes, Invoice.
/// Other modules stay stubs until implemented.
/// </summary>
public static class PredefinedModulesCatalog
{
    public const string UnderImplementationRenderMode = "under_implementation";
    public const string GenericRenderMode = "generic";

    public static IReadOnlyList<PredefinedModuleSpec> Modules { get; } =
    [
        // 1. CRM
        new("crm", "CRM", "Customer relationship management",
        [
            new PredefinedScreenSpec(
                CrmLeadsScreenCatalog.ScreenCode,
                "Lead",
                CrmLeadsScreenCatalog.Route,
                CrmLeadsScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
            new PredefinedScreenSpec(
                CrmOpportunitiesScreenCatalog.ScreenCode,
                "Opportunity",
                CrmOpportunitiesScreenCatalog.Route,
                CrmOpportunitiesScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
        ]),

        // 2. Sales
        new("sales", "Sales", "Sales enquiry, quotations, orders, invoices, credit & debit notes, and delivery orders",
        [
            new PredefinedScreenSpec(
                SalesEnquiryScreenCatalog.ScreenCode,
                "Sales Enquiry",
                SalesEnquiryScreenCatalog.Route,
                SalesEnquiryScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
            new PredefinedScreenSpec(
                SalesQuotationScreenCatalog.ScreenCode,
                "Sales Quotations",
                SalesQuotationScreenCatalog.Route,
                SalesQuotationScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
            new PredefinedScreenSpec(
                SalesOrderScreenCatalog.ScreenCode,
                "Sales Orders",
                SalesOrderScreenCatalog.Route,
                SalesOrderScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
            new PredefinedScreenSpec(
                SalesInvoiceScreenCatalog.ScreenCode,
                "Sales Invoices",
                SalesInvoiceScreenCatalog.Route,
                SalesInvoiceScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
            new PredefinedScreenSpec(
                SalesCreditDebitNoteScreenCatalog.ScreenCode,
                "Credit & Debit Notes",
                SalesCreditDebitNoteScreenCatalog.Route,
                SalesCreditDebitNoteScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
            new PredefinedScreenSpec(
                SalesDeliveryOrderScreenCatalog.ScreenCode,
                "Delivery Orders",
                SalesDeliveryOrderScreenCatalog.Route,
                SalesDeliveryOrderScreenCatalog.ApiBasePath,
                GenericRenderMode,
                IsImplemented: true),
        ]),

        // 3. HR
        new("hr", "HR", "Human resources and employee management",
        [
            new PredefinedScreenSpec(
                "hr-employees",
                "Employee",
                "/hr/employees",
                "/api/v1/hr/employees",
                UnderImplementationRenderMode,
                IsImplemented: false),
        ]),

        // 4. Masters
        new("masters", "Masters", "Core master data management",
        [
            new PredefinedScreenSpec(
                "masters-currency",
                "Currency",
                "/settings/catalog/masters/currency",
                "/api/v1/finance/currencies",
                UnderImplementationRenderMode,
                IsImplemented: false),
            new PredefinedScreenSpec(
                "masters-subsidiary",
                "Subsidiary",
                "/settings/catalog/masters/subsidiary",
                "/api/v1/organization/subsidiaries",
                UnderImplementationRenderMode,
                IsImplemented: false),
            new PredefinedScreenSpec(
                "masters-uom",
                "UOM",
                "/settings/catalog/masters/uom",
                "/api/v1/catalog/units-of-measure",
                UnderImplementationRenderMode,
                IsImplemented: false),
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
