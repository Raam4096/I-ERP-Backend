namespace iERP.Application.Abstractions.Metadata;

/// <summary>
/// Canonical UI example for the CRM Leads screen (section → snake_case fields).
/// Served by GET /api/v1/crm/leads/example for UI layout/demo; not persisted unless seeded separately.
/// </summary>
public static class CrmLeadsExamplePayload
{
    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, object?>> ValuesBySection { get; } =
        new Dictionary<string, IReadOnlyDictionary<string, object?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["primary_information"] = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["company_name"] = "Nexus Innovations Pvt Ltd",
                ["contact_person"] = "Rajesh Kumar",
                ["phone_number"] = "+91 98765 43210",
                ["email"] = "rajesh.kumar@nexusinnovations.com",
                ["industry"] = "Information Technology",
                ["project_type"] = "Web Application",
                ["lead_source"] = "Website",
                ["status"] = "New",
                ["assigned_to"] = "John Doe",
                ["website"] = "https://www.nexusinnovations.com",
                ["company_size"] = "50-200 Employees",
                ["annual_revenue"] = "$1M - $5M",
                ["address"] = "Plot No. 42, IT Park, Madhurawada, Visakhapatnam, Andhra Pradesh - 530048",
            },
            ["classification"] = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["subsidiary"] = "Nexus Global Tech Solutions",
            },
            ["additional_information"] = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["project_description"] =
                    "Looking to develop a comprehensive enterprise resource planning (ERP) portal with automated invoicing and inventory management capabilities using a modern stack.",
                ["notes"] =
                    "Client prefers an initial prototype demonstration within two weeks; budget approval is already cleared from their technical head.",
            },
            ["follow_ups"] = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["follow_up_date"] = "08/28/2026",
                ["new_follow_up_date"] = "09/04/2026",
                ["follow_up_status"] = "Scheduled",
                ["follow_up_type"] = "Video Call Demo",
                ["follow_up_file"] = "requirements_v1.pdf",
                ["follow_up_notes"] =
                    "Discussed preliminary scope and technical feasibility; scheduled a formal walkthrough of the proposed architecture next week.",
            },
        };
}
