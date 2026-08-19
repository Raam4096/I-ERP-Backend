using iERP.Modules.CRM.Domain;

namespace iERP.UnitTests.CRM;

internal static class CrmDomainFactory
{
    public static Lead CreateLead(
        string status = "Qualified",
        string companyName = "Acme",
        string phone = "+6591111111",
        string email = "lead@acme.test") =>
        Lead.Create(
            tenantId: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            leadNumber: "LEAD-2026-000099",
            companyName: companyName,
            phone: phone,
            email: email,
            contactPerson: "Jane",
            industry: null,
            address: null,
            annualRevenue: null,
            assignedToUserId: null,
            companySize: null,
            leadSource: null,
            projectDescription: null,
            projectType: null,
            status: status,
            subsidiary: null,
            subsidiaryId: null,
            website: null,
            notes: null);

    public static Opportunity CreateOpportunityFrom(Lead lead, string number = "OPP-2026-000099") =>
        Opportunity.CreateFromLead(
            tenantId: lead.TenantId,
            opportunityNumber: number,
            lead: lead,
            opportunityValue: 1000,
            probability: 50,
            status: OpportunityStatuses.New,
            computations: null,
            notes: null,
            closedReason: null,
            currencyCode: "USD",
            expectedCloseDate: null,
            ownerUserId: null);
}
