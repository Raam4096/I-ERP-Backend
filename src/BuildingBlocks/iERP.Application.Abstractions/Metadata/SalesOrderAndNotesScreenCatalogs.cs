namespace iERP.Application.Abstractions.Metadata;

public static class SalesOrderScreenCatalog
{
    public const string ScreenCode = "sales-orders";
    public const string ModuleCode = "sales";
    public const string ScreenName = "Orders";
    public const string Route = "/sales/orders";
    public const string ApiBasePath = "/api/v1/sales/orders";

    public static IReadOnlyList<ScreenSectionSpec> Sections { get; } =
    [
        new("header", "Header", "Order identity and customer. Optional source links keep the document chain open for enhancements.", 1,
        [
            new("orderCode", "Order Code", "text", "input", 1, false, true),
            new("customer", "Customer", "text", "input", 2, true, false),
            new("contactPerson", "Contact Person", "text", "input", 3, false, false),
            new("email", "Email", "text", "input", 4, false, false),
            new("phone", "Phone", "text", "input", 5, false, false),
            new("title", "Title", "text", "input", 6, false, false),
            new("status", "Status", "text", "select", 7, false, false),
            new("documentDate", "Document Date", "date", "datepicker", 8, false, false),
            new("deliveryDate", "Delivery Date", "date", "datepicker", 9, false, false),
            new("sourceQuotationCode", "Source Quotation", "text", "input", 10, false, true),
            new("sourceEnquiryCode", "Source Enquiry", "text", "input", 11, false, true),
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
            new("projectedTotal", "Order Total", "number", "number", 8, false, false),
        ]),
        new("terms", "Terms", "Payment, price, and delivery terms.", 3,
        [
            new("payment", "Payment", "text", "textarea", 1, false, false),
            new("prices", "Prices", "text", "textarea", 2, false, false),
            new("delivery", "Delivery", "text", "textarea", 3, false, false),
            new("notes", "Notes", "text", "textarea", 4, false, false),
        ]),
        new("items", "Line Items", "Order line items grid.", 4,
        [
            new("items", "Items", "json", "grid", 1, false, false),
        ]),
    ];
}

public static class SalesCreditNoteScreenCatalog
{
    public const string ScreenCode = "sales-credit-notes";
    public const string ModuleCode = "sales";
    public const string ScreenName = "Credit Notes";
    public const string Route = "/sales/credit-notes";
    public const string ApiBasePath = "/api/v1/sales/credit-notes";

    public static IReadOnlyList<ScreenSectionSpec> Sections { get; } =
    [
        new("header", "Header", "Credit note identity. Optional source invoice link for adjustments.", 1,
        [
            new("noteCode", "Credit Note Code", "text", "input", 1, false, true),
            new("customer", "Customer", "text", "input", 2, true, false),
            new("contactPerson", "Contact Person", "text", "input", 3, false, false),
            new("email", "Email", "text", "input", 4, false, false),
            new("phone", "Phone", "text", "input", 5, false, false),
            new("title", "Title", "text", "input", 6, false, false),
            new("status", "Status", "text", "select", 7, false, false),
            new("documentDate", "Document Date", "date", "datepicker", 8, false, false),
            new("dueDate", "Due Date", "date", "datepicker", 9, false, false),
            new("reason", "Reason", "text", "textarea", 10, false, false),
            new("sourceInvoiceCode", "Source Invoice", "text", "input", 11, false, true),
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
            new("projectedTotal", "Credit Total", "number", "number", 8, false, false),
        ]),
        new("terms", "Terms", "Payment and notes.", 3,
        [
            new("payment", "Payment", "text", "textarea", 1, false, false),
            new("notes", "Notes", "text", "textarea", 2, false, false),
        ]),
        new("items", "Line Items", "Credit note line items grid.", 4,
        [
            new("items", "Items", "json", "grid", 1, false, false),
        ]),
    ];
}

public static class SalesDebitNoteScreenCatalog
{
    public const string ScreenCode = "sales-debit-notes";
    public const string ModuleCode = "sales";
    public const string ScreenName = "Debit Notes";
    public const string Route = "/sales/debit-notes";
    public const string ApiBasePath = "/api/v1/sales/debit-notes";

    public static IReadOnlyList<ScreenSectionSpec> Sections { get; } =
    [
        new("header", "Header", "Debit note identity. Optional source invoice link for adjustments.", 1,
        [
            new("noteCode", "Debit Note Code", "text", "input", 1, false, true),
            new("customer", "Customer", "text", "input", 2, true, false),
            new("contactPerson", "Contact Person", "text", "input", 3, false, false),
            new("email", "Email", "text", "input", 4, false, false),
            new("phone", "Phone", "text", "input", 5, false, false),
            new("title", "Title", "text", "input", 6, false, false),
            new("status", "Status", "text", "select", 7, false, false),
            new("documentDate", "Document Date", "date", "datepicker", 8, false, false),
            new("dueDate", "Due Date", "date", "datepicker", 9, false, false),
            new("reason", "Reason", "text", "textarea", 10, false, false),
            new("sourceInvoiceCode", "Source Invoice", "text", "input", 11, false, true),
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
            new("projectedTotal", "Debit Total", "number", "number", 8, false, false),
        ]),
        new("terms", "Terms", "Payment and notes.", 3,
        [
            new("payment", "Payment", "text", "textarea", 1, false, false),
            new("notes", "Notes", "text", "textarea", 2, false, false),
        ]),
        new("items", "Line Items", "Debit note line items grid.", 4,
        [
            new("items", "Items", "json", "grid", 1, false, false),
        ]),
    ];
}

public static class SalesCreditDebitNoteScreenCatalog
{
    public const string ScreenCode = "sales-credit-debit-notes";
    public const string ModuleCode = "sales";
    public const string ScreenName = "Credit & Debit Notes";
    public const string Route = "/sales/credit-debit-notes";
    public const string ApiBasePath = "/api/v1/sales/credit-debit-notes";

    public static IReadOnlyList<ScreenSectionSpec> Sections => SalesCreditNoteScreenCatalog.Sections;
}

public static class SalesDeliveryOrderScreenCatalog
{
    public const string ScreenCode = "sales-delivery-orders";
    public const string ModuleCode = "sales";
    public const string ScreenName = "Delivery Orders";
    public const string Route = "/sales/delivery-orders";
    public const string ApiBasePath = "/api/v1/sales/delivery-orders";

    public static IReadOnlyList<ScreenSectionSpec> Sections { get; } =
    [
        new("header", "Header", "Delivery order identity and customer details.", 1,
        [
            new("deliveryOrderNumber", "Delivery Order Number", "text", "input", 1, false, true),
            new("customer", "Customer", "text", "input", 2, true, false),
            new("contactPerson", "Contact Person", "text", "input", 3, false, false),
            new("deliveryDate", "Delivery Date", "date", "datepicker", 4, false, false),
            new("status", "Status", "text", "select", 5, false, false),
            new("sourceOrderCode", "Source Sales Order", "text", "input", 6, false, true),
        ]),
        new("commercial", "Commercial", "Subsidiary and dispatch location.", 2,
        [
            new("subsidiary", "Subsidiary", "text", "select", 1, false, false),
            new("dispatchLocation", "Dispatch Location", "text", "input", 2, false, false),
            new("shippingMethod", "Shipping Method", "text", "input", 3, false, false),
        ]),
        new("items", "Line Items", "Delivered items grid.", 3,
        [
            new("items", "Items", "json", "grid", 1, false, false),
        ]),
    ];
}

