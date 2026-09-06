namespace iERP.Application.Abstractions.Metadata;

/// <summary>
/// Sales Enquiry screen — field keys are camelCase to match UI payload.
/// </summary>
public static class SalesEnquiryScreenCatalog
{
    public const string ScreenCode = "sales-enquiries";
    public const string ModuleCode = "sales";
    public const string ScreenName = "Sales Enquiry";
    public const string Route = "/sales/enquiries";
    public const string ApiBasePath = "/api/v1/sales/enquiries";

    public static IReadOnlyList<ScreenSectionSpec> Sections { get; } =
    [
        new("header", "Header", "Enquiry identity and customer contact.", 1,
        [
            new("enquiryCode", "Enquiry Code", "text", "input", 1, false, true),
            new("customer", "Customer", "text", "input", 2, true, false),
            new("contactPerson", "Contact Person", "text", "input", 3, false, false),
            new("opportunity", "Opportunity", "text", "input", 4, false, false),
            new("email", "Email", "text", "input", 5, false, false),
            new("phone", "Phone", "text", "input", 6, false, false),
            new("altPhone", "Alt Phone", "text", "input", 7, false, false),
            new("title", "Title", "text", "input", 8, true, false),
            new("status", "Status", "text", "select", 9, false, false),
        ]),
        new("commercial", "Commercial", "Forecast and commercial classification.", 2,
        [
            new("probability", "Probability", "number", "number", 1, false, false),
            new("expectedClose", "Expected Close", "date", "datepicker", 2, false, false),
            new("winLossReason", "Win/Loss Reason", "text", "input", 3, false, false),
            new("projectedTotal", "Projected Total", "number", "number", 4, false, false),
            new("forecastType", "Forecast Type", "text", "select", 5, false, false),
            new("weightedTotal", "Weighted Total", "number", "number", 6, false, false),
            new("range", "Range", "text", "input", 7, false, false),
            new("subsidiary", "Subsidiary", "text", "select", 8, false, false),
            new("className", "Class", "text", "select", 9, false, false),
            new("location", "Location", "text", "input", 10, false, false),
            new("department", "Department", "text", "input", 11, false, false),
            new("salesRep", "Sales Rep", "text", "select", 12, false, false),
            new("lastSalesActivity", "Last Sales Activity", "date", "datepicker", 13, false, false),
            new("currency", "Currency", "text", "select", 14, false, false),
            new("exchangeRate", "Exchange Rate", "number", "number", 15, false, false),
        ]),
        new("terms", "Terms", "Payment, price, and delivery terms.", 3,
        [
            new("payment", "Payment", "text", "textarea", 1, false, false),
            new("prices", "Prices", "text", "textarea", 2, false, false),
            new("delivery", "Delivery", "text", "textarea", 3, false, false),
        ]),
        new("follow_up", "Follow-up", "Initial / next follow-up activity.", 4,
        [
            new("createInitialFollowUp", "Create Initial Follow-Up", "boolean", "checkbox", 1, false, false),
            new("followUpDate", "Follow-Up Date", "date", "datepicker", 2, false, false),
            new("nextFollowUpDate", "Next Follow-Up Date", "date", "datepicker", 3, false, false),
            new("followUpStatus", "Follow-Up Status", "text", "select", 4, false, false),
            new("activityType", "Activity Type", "text", "select", 5, false, false),
            new("remarks", "Remarks", "text", "textarea", 6, false, false),
        ]),
        new("items", "Line Items", "Enquiry line items grid.", 5,
        [
            new("items", "Items", "json", "grid", 1, false, false),
        ]),
    ];
}
