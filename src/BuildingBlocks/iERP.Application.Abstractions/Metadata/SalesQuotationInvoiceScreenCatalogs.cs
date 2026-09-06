namespace iERP.Application.Abstractions.Metadata;

public static class SalesQuotationScreenCatalog
{
    public const string ScreenCode = "sales-quotations";
    public const string ModuleCode = "sales";
    public const string ScreenName = "Quotation";
    public const string Route = "/sales/quotations";
    public const string ApiBasePath = "/api/v1/sales/quotations";

    public static IReadOnlyList<ScreenSectionSpec> Sections { get; } =
    [
        new("header", "Header", "Quotation identity and customer.", 1,
        [
            new("quotationCode", "Quotation Code", "text", "input", 1, false, true),
            new("customer", "Customer", "text", "input", 2, true, false),
            new("contactPerson", "Contact Person", "text", "input", 3, false, false),
            new("email", "Email", "text", "input", 4, false, false),
            new("phone", "Phone", "text", "input", 5, false, false),
            new("title", "Title", "text", "input", 6, true, false),
            new("status", "Status", "text", "select", 7, false, false),
            new("documentDate", "Document Date", "date", "datepicker", 8, false, false),
            new("validUntil", "Valid Until", "date", "datepicker", 9, false, false),
        ]),
        new("commercial", "Commercial", "Commercial classification and currency.", 2,
        [
            new("subsidiary", "Subsidiary", "text", "select", 1, false, false),
            new("className", "Class", "text", "select", 2, false, false),
            new("location", "Location", "text", "input", 3, false, false),
            new("department", "Department", "text", "input", 4, false, false),
            new("salesRep", "Sales Rep", "text", "select", 5, false, false),
            new("currency", "Currency", "text", "select", 6, false, false),
            new("exchangeRate", "Exchange Rate", "number", "number", 7, false, false),
            new("projectedTotal", "Projected Total", "number", "number", 8, false, false),
        ]),
        new("terms", "Terms", "Payment, price, and delivery terms.", 3,
        [
            new("payment", "Payment", "text", "textarea", 1, false, false),
            new("prices", "Prices", "text", "textarea", 2, false, false),
            new("delivery", "Delivery", "text", "textarea", 3, false, false),
            new("notes", "Notes", "text", "textarea", 4, false, false),
        ]),
        new("items", "Line Items", "Quotation line items grid.", 4,
        [
            new("items", "Items", "json", "grid", 1, false, false),
        ]),
    ];
}

public static class SalesInvoiceScreenCatalog
{
    public const string ScreenCode = "sales-invoices";
    public const string ModuleCode = "sales";
    public const string ScreenName = "Invoice";
    public const string Route = "/sales/invoices";
    public const string ApiBasePath = "/api/v1/sales/invoices";

    public static IReadOnlyList<ScreenSectionSpec> Sections { get; } =
    [
        new("header", "Header", "Invoice identity and customer.", 1,
        [
            new("invoiceCode", "Invoice Code", "text", "input", 1, false, true),
            new("customer", "Customer", "text", "input", 2, true, false),
            new("contactPerson", "Contact Person", "text", "input", 3, false, false),
            new("email", "Email", "text", "input", 4, false, false),
            new("phone", "Phone", "text", "input", 5, false, false),
            new("title", "Title", "text", "input", 6, false, false),
            new("status", "Status", "text", "select", 7, false, false),
            new("documentDate", "Document Date", "date", "datepicker", 8, false, false),
            new("dueDate", "Due Date", "date", "datepicker", 9, false, false),
        ]),
        new("commercial", "Commercial", "Commercial classification and currency.", 2,
        [
            new("subsidiary", "Subsidiary", "text", "select", 1, false, false),
            new("className", "Class", "text", "select", 2, false, false),
            new("location", "Location", "text", "input", 3, false, false),
            new("department", "Department", "text", "input", 4, false, false),
            new("salesRep", "Sales Rep", "text", "select", 5, false, false),
            new("currency", "Currency", "text", "select", 6, false, false),
            new("exchangeRate", "Exchange Rate", "number", "number", 7, false, false),
            new("projectedTotal", "Invoice Total", "number", "number", 8, false, false),
        ]),
        new("terms", "Terms", "Payment and notes.", 3,
        [
            new("payment", "Payment", "text", "textarea", 1, false, false),
            new("notes", "Notes", "text", "textarea", 2, false, false),
        ]),
        new("items", "Line Items", "Invoice line items grid.", 4,
        [
            new("items", "Items", "json", "grid", 1, false, false),
        ]),
    ];
}
