namespace iERP.Application.Abstractions.Metadata;

/// <summary>
/// Canonical CRM Leads screen layout. Field keys are snake_case to match the UI payload.
/// CRM has exactly two screens: Leads and Opportunities.
/// </summary>
public static class CrmLeadsScreenCatalog
{
    public const string ScreenCode = "crm-leads";
    public const string ModuleCode = "crm";
    public const string ScreenName = "Leads";
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
                new("company_name", "Company Name", "text", "input", 1, true, false),
                new("contact_person", "Contact Person", "text", "input", 2, true, false),
                new("phone_number", "Phone Number", "text", "input", 3, true, false),
                new("email", "Email", "text", "input", 4, true, false),
                new("industry", "Industry", "text", "select", 5, false, false),
                new("project_type", "Project Type", "text", "select", 6, false, false),
                new("lead_source", "Lead Source", "text", "select", 7, false, false),
                new("status", "Status", "text", "select", 8, false, false),
                new("assigned_to", "Assigned To", "text", "select", 9, false, false),
                new("website", "Website", "text", "input", 10, false, false),
                new("company_size", "Company Size", "text", "select", 11, false, false),
                new("annual_revenue", "Annual Revenue", "text", "select", 12, false, false),
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
                new("project_description", "Project Description", "text", "textarea", 1, false, false),
                new("notes", "Notes", "text", "textarea", 2, false, false),
            ]),
        new(
            Code: "follow_ups",
            Name: "Follow-ups",
            Description: "Latest / new follow-up activity for this lead.",
            Order: 4,
            Fields:
            [
                new("follow_up_date", "Follow-Up Date", "date", "datepicker", 1, false, false),
                new("new_follow_up_date", "New Follow-Up Date", "date", "datepicker", 2, false, false),
                new("follow_up_status", "Follow-Up Status", "text", "select", 3, false, false),
                new("follow_up_type", "Follow-Up Type", "text", "select", 4, false, false),
                new("follow_up_file", "Follow-Up File", "text", "input", 5, false, false),
                new("follow_up_notes", "Follow-Up Notes", "text", "textarea", 6, false, false),
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
