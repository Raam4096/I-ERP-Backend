namespace iERP.Application.Abstractions.Metadata;

/// <summary>
/// Canonical CRM Leads screen layout shared by metadata seeding and form-data APIs.
/// Matches the UI Lead Management sections on new-ierp.
/// </summary>
public static class CrmLeadsScreenCatalog
{
    public const string ScreenCode = "crm-leads";
    public const string ModuleCode = "crm";
    public const string ScreenName = "Lead Management";
    public const string Route = "/crm/leads";
    public const string ApiBasePath = "/api/v1/crm/leads";

    public static IReadOnlyList<ScreenSectionSpec> Sections { get; } =
    [
        new(
            Code: "primary_information",
            Name: "Primary Information",
            Description: "Core company and contact details for the lead.",
            Order: 1,
            Fields:
            [
                new("companyName", "Company Name", "text", "input", 1, true, false),
                new("contactPerson", "Contact Person", "text", "input", 2, true, false),
                new("phone", "Phone Number", "text", "input", 3, true, false),
                new("email", "Email", "text", "input", 4, true, false),
                new("industry", "Industry", "text", "select", 5, false, false),
                new("projectType", "Project Type", "text", "select", 6, false, false),
                new("leadSource", "Lead Source", "text", "select", 7, false, false),
                new("status", "Status", "text", "select", 8, false, false),
                new("assignedTo", "Assigned To", "text", "select", 9, false, false),
                new("website", "Website", "text", "input", 10, false, false),
                new("companySize", "Company Size", "text", "select", 11, false, false),
                new("annualRevenue", "Annual Revenue", "text", "select", 12, false, false),
                new("address", "Address", "text", "textarea", 13, false, false),
            ]),
        new(
            Code: "classification",
            Name: "Classification",
            Description: "Organization / subsidiary classification.",
            Order: 2,
            Fields:
            [
                new("subsidiary", "Subsidiary", "text", "select", 1, false, false),
            ]),
        new(
            Code: "additional_information",
            Name: "Additional Information",
            Description: "Project context and free-form notes.",
            Order: 3,
            Fields:
            [
                new("projectDescription", "Project Description", "text", "textarea", 1, false, false),
                new("notes", "Notes", "text", "textarea", 2, false, false),
            ]),
        new(
            Code: "follow_ups",
            Name: "Follow-ups",
            Description: "Latest / new follow-up activity for this lead.",
            Order: 4,
            Fields:
            [
                new("followUpDate", "Follow-Up Date", "date", "datepicker", 1, false, false),
                new("newFollowUpDate", "New Follow-Up Date", "date", "datepicker", 2, false, false),
                new("followUpStatus", "Follow-Up Status", "text", "select", 3, false, false),
                new("followUpType", "Follow-Up Type", "text", "select", 4, false, false),
                new("followUpFile", "Follow-Up File", "text", "input", 5, false, false),
                new("followUpNotes", "Follow-Up Notes", "text", "textarea", 6, false, false),
            ]),
    ];
}

public sealed record ScreenSectionSpec(
    string Code,
    string Name,
    string? Description,
    int Order,
    IReadOnlyList<ScreenFieldSpec> Fields);

public sealed record ScreenFieldSpec(
    string FieldKey,
    string Label,
    string DataType,
    string ControlType,
    int Order,
    bool Required,
    bool ReadOnly);
