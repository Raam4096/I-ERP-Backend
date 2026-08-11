namespace iERP.SharedKernel.Security;

/// <summary>
/// Permission constants use module.resource.action naming.
/// </summary>
public static class Permissions
{
    public static class Crm
    {
        public const string LeadRead = "crm.lead.read";
        public const string LeadCreate = "crm.lead.create";
        public const string LeadUpdate = "crm.lead.update";
        public const string OpportunityRead = "crm.opportunity.read";
        public const string OpportunityCreate = "crm.opportunity.create";
        public const string CustomerRead = "crm.customer.read";
        public const string CustomerCreate = "crm.customer.create";
        public const string CustomerUpdate = "crm.customer.update";
    }

    public static class Sales
    {
        public const string QuotationRead = "sales.quotation.read";
        public const string QuotationCreate = "sales.quotation.create";
        public const string QuotationApprove = "sales.quotation.approve";
        public const string OrderCreate = "sales.order.create";
        public const string OrderRead = "sales.order.read";
        public const string InvoiceRead = "sales.invoice.read";
        public const string InvoiceCreate = "sales.invoice.create";
    }

    public static class Finance
    {
        public const string InvoiceApprove = "finance.invoice.approve";
        public const string GlPost = "finance.gl.post";
        public const string JournalRead = "finance.journal.read";
        public const string JournalCreate = "finance.journal.create";
    }

    public static class Procurement
    {
        public const string PurchaseOrderRead = "procurement.purchase_order.read";
        public const string PurchaseOrderCreate = "procurement.purchase_order.create";
        public const string VendorRead = "procurement.vendor.read";
        public const string VendorCreate = "procurement.vendor.create";
    }

    public static class Inventory
    {
        public const string StockRead = "inventory.stock.read";
        public const string StockAdjust = "inventory.stock.adjust";
        public const string TransferCreate = "inventory.transfer.create";
    }

    public static class Catalog
    {
        public const string ItemRead = "catalog.item.read";
        public const string ItemCreate = "catalog.item.create";
        public const string ItemUpdate = "catalog.item.update";
    }

    public static class Ai
    {
        public const string ExecuteAdvisory = "ai.tool.execute.advisory";
        public const string ExecuteSemiAutonomous = "ai.tool.execute.semi_autonomous";
        public const string ExecuteAutonomous = "ai.tool.execute.autonomous";
    }

    public static class Platform
    {
        public const string TenantManage = "platform.tenant.manage";
        public const string UserManage = "platform.user.manage";
        public const string RoleManage = "platform.role.manage";
        public const string MetadataManage = "platform.metadata.manage";
    }
}
