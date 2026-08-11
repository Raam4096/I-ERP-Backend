#!/usr/bin/env python3
from __future__ import annotations
from pathlib import Path
from textwrap import dedent

ROOT = Path(__file__).resolve().parents[1]

def write(rel: str, content: str) -> None:
    path = ROOT / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(dedent(content).lstrip("\n").replace("\r\n", "\n"), encoding="utf-8")
    print(rel)

def aud(ns: str, name: str, body: str) -> str:
    return f"""
using iERP.SharedKernel.Primitives;

namespace {ns};

public sealed class {name} : AuditableEntity
{{
{body}
}}
"""

# ---------- ENGINES ----------
E = "src/Modules/Engines/iERP.Modules.Engines"
write(f"{E}/Workflow/Domain/WorkflowDefinition.cs", aud("iERP.Modules.Engines.Workflow.Domain", "WorkflowDefinition", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
"""))
write(f"{E}/Workflow/Domain/WorkflowStep.cs", aud("iERP.Modules.Engines.Workflow.Domain", "WorkflowStep", """
    public Guid WorkflowDefinitionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int StepOrder { get; set; }
    public string? ApproverRole { get; set; }
"""))
write(f"{E}/Workflow/Domain/WorkflowInstance.cs", aud("iERP.Modules.Engines.Workflow.Domain", "WorkflowInstance", """
    public string EntityName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public Guid WorkflowId { get; set; }
    public string CurrentStep { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public Guid StartedBy { get; set; }
    public string? RejectionReason { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
"""))
write(f"{E}/Workflow/Domain/WorkflowHistory.cs", aud("iERP.Modules.Engines.Workflow.Domain", "WorkflowHistory", """
    public Guid WorkflowInstanceId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Guid ActedBy { get; set; }
    public string? Comments { get; set; }
"""))
write(f"{E}/Rules/Domain/RuleDefinition.cs", aud("iERP.Modules.Engines.Rules.Domain", "RuleDefinition", """
    public string EntityName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string Conditions { get; set; } = "[]";
    public string Actions { get; set; } = "[]";
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
"""))
write(f"{E}/Bridge/Domain/BridgeDefinition.cs", aud("iERP.Modules.Engines.Bridge.Domain", "BridgeDefinition", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SourceEntityName { get; set; } = string.Empty;
    public string TargetEntityName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))
write(f"{E}/Bridge/Domain/BridgeMapping.cs", aud("iERP.Modules.Engines.Bridge.Domain", "BridgeMapping", """
    public Guid BridgeDefinitionId { get; set; }
    public string SourceField { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public string? TransformExpression { get; set; }
"""))
write(f"{E}/Bridge/Domain/BridgeLog.cs", aud("iERP.Modules.Engines.Bridge.Domain", "BridgeLog", """
    public Guid BridgeDefinitionId { get; set; }
    public Guid SourceRecordId { get; set; }
    public Guid? TargetRecordId { get; set; }
    public string Status { get; set; } = "pending";
    public string? ErrorMessage { get; set; }
"""))
write(f"{E}/Printing/Domain/PrintTemplate.cs", aud("iERP.Modules.Engines.Printing.Domain", "PrintTemplate", """
    public string EntityName { get; set; } = string.Empty;
    public string TemplateCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))
write(f"{E}/Printing/Domain/PrintTemplateVersion.cs", aud("iERP.Modules.Engines.Printing.Domain", "PrintTemplateVersion", """
    public Guid PrintTemplateId { get; set; }
    public int Version { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string OutputType { get; set; } = "pdf";
    public bool IsActive { get; set; } = true;
"""))

write(f"{E}/Workflow/Application/NullWorkflowEngine.cs", """
using iERP.Application.Abstractions.Engines;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Engines.Workflow.Application;

public sealed class NullWorkflowEngine : IWorkflowEngine
{
    private readonly ILogger<NullWorkflowEngine> _logger;
    public NullWorkflowEngine(ILogger<NullWorkflowEngine> logger) => _logger = logger;
    public Task StartAsync(Guid tenantId, string entityName, Guid recordId, Guid workflowId, Guid startedBy, CancellationToken cancellationToken = default)
    { _logger.LogDebug("Workflow start placeholder"); return Task.CompletedTask; }
    public Task AdvanceAsync(Guid tenantId, Guid instanceId, string action, Guid actedBy, CancellationToken cancellationToken = default)
    { return Task.CompletedTask; }
    public Task CancelAsync(Guid tenantId, Guid instanceId, Guid actedBy, string? reason = null, CancellationToken cancellationToken = default)
    { return Task.CompletedTask; }
}
""")
write(f"{E}/Rules/Application/NullRuleEngine.cs", """
using iERP.Application.Abstractions.Engines;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Engines.Rules.Application;

public sealed class NullRuleEngine : IRuleEngine
{
    private readonly ILogger<NullRuleEngine> _logger;
    public NullRuleEngine(ILogger<NullRuleEngine> logger) => _logger = logger;
    public Task EvaluateAsync(Guid tenantId, string entityName, string eventName, object context, CancellationToken cancellationToken = default)
    { _logger.LogDebug("Rule evaluate placeholder"); return Task.CompletedTask; }
}
""")
write(f"{E}/Bridge/Application/NullBridgeEngine.cs", """
using iERP.Application.Abstractions.Engines;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Engines.Bridge.Application;

public sealed class NullBridgeEngine : IBridgeEngine
{
    private readonly ILogger<NullBridgeEngine> _logger;
    public NullBridgeEngine(ILogger<NullBridgeEngine> logger) => _logger = logger;
    public Task ConvertAsync(Guid tenantId, Guid bridgeDefinitionId, Guid sourceRecordId, Guid actedBy, CancellationToken cancellationToken = default)
    { _logger.LogDebug("Bridge convert placeholder"); return Task.CompletedTask; }
}
""")
write(f"{E}/Printing/Application/NullPrintEngine.cs", """
using iERP.Application.Abstractions.Engines;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Engines.Printing.Application;

public sealed class NullPrintEngine : IPrintEngine
{
    private readonly ILogger<NullPrintEngine> _logger;
    public NullPrintEngine(ILogger<NullPrintEngine> logger) => _logger = logger;
    public Task<byte[]> RenderAsync(Guid tenantId, string entityName, Guid recordId, string templateCode, CancellationToken cancellationToken = default)
    { _logger.LogDebug("Print render placeholder"); return Task.FromResult(Array.Empty<byte>()); }
}
""")

# ---------- CRM ----------
C = "src/Modules/CRM/iERP.Modules.CRM"
write(f"{C}/Domain/Lead.cs", aud("iERP.Modules.CRM.Domain", "Lead", """
    public Guid SubsidiaryId { get; set; }
    public string LeadNumber { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Source { get; set; }
    public string Status { get; set; } = "new";
    public string? Rating { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string? CurrencyCode { get; set; }
    public Guid? OwnerUserId { get; set; }
    public Guid? ConvertedCustomerId { get; set; }
    public Guid? ConvertedContactId { get; set; }
    public Guid? ConvertedOpportunityId { get; set; }
    public DateTimeOffset? ConvertedAt { get; set; }
"""))
write(f"{C}/Domain/Opportunity.cs", aud("iERP.Modules.CRM.Domain", "Opportunity", """
    public Guid SubsidiaryId { get; set; }
    public string OpportunityNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public string Stage { get; set; } = "prospecting";
    public decimal? Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public DateOnly? ExpectedCloseDate { get; set; }
    public Guid? OwnerUserId { get; set; }
    public string Status { get; set; } = "open";
"""))
write(f"{C}/Domain/Activity.cs", aud("iERP.Modules.CRM.Domain", "Activity", """
    public Guid SubsidiaryId { get; set; }
    public string ActivityType { get; set; } = "call";
    public string Subject { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OwnerUserId { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public string Status { get; set; } = "planned";
"""))
write(f"{C}/Domain/Customer.cs", aud("iERP.Modules.CRM.Domain", "Customer", """
    public Guid SubsidiaryId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CustomerType { get; set; } = "company";
    public string? CurrencyCode { get; set; }
    public decimal? CreditLimit { get; set; }
    public Guid? PaymentTermsId { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public string? Country { get; set; }
    public Guid? DefaultPriceListId { get; set; }
    public Guid? SalespersonUserId { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; } = true;
"""))
write(f"{C}/Domain/Contact.cs", aud("iERP.Modules.CRM.Domain", "Contact", """
    public Guid? CustomerId { get; set; }
    public Guid? VendorId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
"""))
write(f"{C}/Domain/Address.cs", aud("iERP.Modules.CRM.Domain", "Address", """
    public Guid? CustomerId { get; set; }
    public Guid? VendorId { get; set; }
    public string AddressType { get; set; } = "billing";
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool IsDefault { get; set; }
"""))

# ---------- CATALOG ----------
CAT = "src/Modules/Catalog/iERP.Modules.Catalog"
write(f"{CAT}/Domain/ItemCategory.cs", aud("iERP.Modules.Catalog.Domain", "ItemCategory", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
"""))
write(f"{CAT}/Domain/UnitOfMeasure.cs", aud("iERP.Modules.Catalog.Domain", "UnitOfMeasure", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))
write(f"{CAT}/Domain/UnitOfMeasureConversion.cs", aud("iERP.Modules.Catalog.Domain", "UnitOfMeasureConversion", """
    public Guid FromUomId { get; set; }
    public Guid ToUomId { get; set; }
    public decimal Factor { get; set; }
"""))
write(f"{CAT}/Domain/Item.cs", aud("iERP.Modules.Catalog.Domain", "Item", """
    public string ItemCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ItemType { get; set; } = "product";
    public Guid? CategoryId { get; set; }
    public Guid UomId { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal? ReorderLevel { get; set; }
    public decimal? ReorderQuantity { get; set; }
    public Guid? SalesTaxCodeId { get; set; }
    public Guid? PurchaseTaxCodeId { get; set; }
    public Guid? SalesGlAccountId { get; set; }
    public Guid? PurchaseGlAccountId { get; set; }
    public Guid? InventoryGlAccountId { get; set; }
    public bool IsActive { get; set; } = true;
"""))
write(f"{CAT}/Domain/PriceList.cs", aud("iERP.Modules.Catalog.Domain", "PriceList", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "USD";
    public bool IsActive { get; set; } = true;
"""))
write(f"{CAT}/Domain/PriceListItem.cs", aud("iERP.Modules.Catalog.Domain", "PriceListItem", """
    public Guid PriceListId { get; set; }
    public Guid ItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public DateOnly? EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
"""))

# ---------- Sales document helpers ----------
def sales_header(extra=""):
    return f"""
    public Guid SubsidiaryId {{ get; set; }}
    public string DocumentNo {{ get; set; }} = string.Empty;
    public DateOnly DocumentDate {{ get; set; }}
    public Guid CustomerId {{ get; set; }}
    public string CurrencyCode {{ get; set; }} = "USD";
    public decimal ExchangeRate {{ get; set; }} = 1m;
    public decimal Subtotal {{ get; set; }}
    public decimal DiscountAmount {{ get; set; }}
    public decimal TaxAmount {{ get; set; }}
    public decimal TotalAmount {{ get; set; }}
    public string Status {{ get; set; }} = "draft";
    public string? Notes {{ get; set; }}
{extra}
"""

def sales_line(header_fk):
    return f"""
    public Guid {header_fk} {{ get; set; }}
    public int LineNo {{ get; set; }}
    public Guid ItemId {{ get; set; }}
    public string? Description {{ get; set; }}
    public decimal Quantity {{ get; set; }}
    public Guid UomId {{ get; set; }}
    public decimal UnitPrice {{ get; set; }}
    public decimal DiscountPercent {{ get; set; }}
    public decimal DiscountAmount {{ get; set; }}
    public Guid? TaxCodeId {{ get; set; }}
    public decimal TaxAmount {{ get; set; }}
    public decimal LineAmount {{ get; set; }}
"""

S = "src/Modules/Sales/iERP.Modules.Sales"
for doc, line, fk in [
    ("SalesQuotation", "SalesQuotationLine", "SalesQuotationId"),
    ("SalesOrder", "SalesOrderLine", "SalesOrderId"),
    ("SalesInvoice", "SalesInvoiceLine", "SalesInvoiceId"),
    ("CreditNote", "CreditNoteLine", "CreditNoteId"),
    ("DeliveryOrder", "DeliveryOrderLine", "DeliveryOrderId"),
]:
    write(f"{S}/Domain/{doc}.cs", aud("iERP.Modules.Sales.Domain", doc, sales_header()))
    write(f"{S}/Domain/{line}.cs", aud("iERP.Modules.Sales.Domain", line, sales_line(fk)))

# ---------- PROCUREMENT ----------
PR = "src/Modules/Procurement/iERP.Modules.Procurement"
write(f"{PR}/Domain/Vendor.cs", aud("iERP.Modules.Procurement.Domain", "Vendor", """
    public Guid SubsidiaryId { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? VendorCategory { get; set; }
    public string? CurrencyCode { get; set; }
    public Guid? PaymentTermsId { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public Guid? WithholdingTaxCodeId { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankSwiftCode { get; set; }
    public bool ApprovedVendor { get; set; }
    public string? CreditRating { get; set; }
    public string? Country { get; set; }
    public bool IsActive { get; set; } = true;
"""))

def proc_header(vendor=True):
    v = "    public Guid VendorId { get; set; }\n" if vendor else ""
    return f"""
    public Guid SubsidiaryId {{ get; set; }}
    public string DocumentNo {{ get; set; }} = string.Empty;
    public DateOnly DocumentDate {{ get; set; }}
{v}    public string CurrencyCode {{ get; set; }} = "USD";
    public decimal ExchangeRate {{ get; set; }} = 1m;
    public decimal Subtotal {{ get; set; }}
    public decimal TaxAmount {{ get; set; }}
    public decimal TotalAmount {{ get; set; }}
    public string Status {{ get; set; }} = "draft";
    public string? Notes {{ get; set; }}
"""

for doc, line, fk, has_vendor in [
    ("PurchaseRequest", "PurchaseRequestLine", "PurchaseRequestId", False),
    ("PurchaseOrder", "PurchaseOrderLine", "PurchaseOrderId", True),
    ("GoodsReceivedNote", "GoodsReceivedNoteLine", "GoodsReceivedNoteId", True),
    ("SupplierInvoice", "SupplierInvoiceLine", "SupplierInvoiceId", True),
]:
    write(f"{PR}/Domain/{doc}.cs", aud("iERP.Modules.Procurement.Domain", doc, proc_header(has_vendor)))
    write(f"{PR}/Domain/{line}.cs", aud("iERP.Modules.Procurement.Domain", line, sales_line(fk)))

# ---------- INVENTORY ----------
INV = "src/Modules/Inventory/iERP.Modules.Inventory"
write(f"{INV}/Domain/Warehouse.cs", aud("iERP.Modules.Inventory.Domain", "Warehouse", """
    public Guid SubsidiaryId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))
write(f"{INV}/Domain/BinLocation.cs", aud("iERP.Modules.Inventory.Domain", "BinLocation", """
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))
write(f"{INV}/Domain/InventoryTransaction.cs", aud("iERP.Modules.Inventory.Domain", "InventoryTransaction", """
    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public string? ReferenceEntityName { get; set; }
    public Guid? ReferenceRecordId { get; set; }
"""))
write(f"{INV}/Domain/InventoryTransactionLine.cs", aud("iERP.Modules.Inventory.Domain", "InventoryTransactionLine", """
    public Guid InventoryTransactionId { get; set; }
    public int LineNo { get; set; }
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UomId { get; set; }
    public decimal? UnitCost { get; set; }
"""))
write(f"{INV}/Domain/StockBalance.cs", aud("iERP.Modules.Inventory.Domain", "StockBalance", """
    public Guid SubsidiaryId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public Guid ItemId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
"""))
write(f"{INV}/Domain/StockReservation.cs", aud("iERP.Modules.Inventory.Domain", "StockReservation", """
    public Guid SubsidiaryId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinLocationId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }
    public string SourceEntityName { get; set; } = string.Empty;
    public Guid SourceRecordId { get; set; }
    public string Status { get; set; } = "active";
"""))
write(f"{INV}/Domain/StockTransfer.cs", aud("iERP.Modules.Inventory.Domain", "StockTransfer", """
    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly TransferDate { get; set; }
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public string Status { get; set; } = "draft";
"""))
write(f"{INV}/Domain/StockTransferLine.cs", aud("iERP.Modules.Inventory.Domain", "StockTransferLine", """
    public Guid StockTransferId { get; set; }
    public int LineNo { get; set; }
    public Guid ItemId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UomId { get; set; }
"""))

# ---------- FINANCE ----------
F = "src/Modules/Finance/iERP.Modules.Finance"
for name, body in [
    ("ChartOfAccount", """
    public Guid SubsidiaryId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public Guid? ParentAccountId { get; set; }
    public bool IsPostable { get; set; } = true;
    public bool IsActive { get; set; } = true;
"""),
    ("FiscalYear", """
    public Guid SubsidiaryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsClosed { get; set; }
"""),
    ("AccountingPeriod", """
    public Guid FiscalYearId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsClosed { get; set; }
"""),
    ("JournalEntry", """
    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly PostingDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
"""),
    ("JournalEntryLine", """
    public Guid JournalEntryId { get; set; }
    public int LineNo { get; set; }
    public Guid AccountId { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1m;
    public decimal BaseDebit { get; set; }
    public decimal BaseCredit { get; set; }
    public Guid? CostCenterId { get; set; }
    public Guid? ClassId { get; set; }
"""),
    ("TaxCode", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsActive { get; set; } = true;
"""),
    ("Currency", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DecimalPlaces { get; set; } = 2;
    public bool IsActive { get; set; } = true;
"""),
    ("ExchangeRate", """
    public string FromCurrencyCode { get; set; } = string.Empty;
    public string ToCurrencyCode { get; set; } = string.Empty;
    public DateOnly RateDate { get; set; }
    public decimal Rate { get; set; }
"""),
    ("Budget", """
    public Guid SubsidiaryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid FiscalYearId { get; set; }
    public string Status { get; set; } = "draft";
"""),
    ("BudgetLine", """
    public Guid BudgetId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CostCenterId { get; set; }
    public decimal Amount { get; set; }
"""),
    ("WithholdingTaxCode", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsActive { get; set; } = true;
"""),
    ("IntercompanyConfiguration", """
    public Guid SourceSubsidiaryId { get; set; }
    public Guid TargetSubsidiaryId { get; set; }
    public Guid? ReceivableAccountId { get; set; }
    public Guid? PayableAccountId { get; set; }
    public bool IsActive { get; set; } = true;
"""),
]:
    write(f"{F}/Domain/{name}.cs", aud("iERP.Modules.Finance.Domain", name, body))

# ---------- BANKING ----------
B = "src/Modules/Banking/iERP.Modules.Banking"
for name, body in [
    ("BankAccount", """
    public Guid SubsidiaryId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public Guid? GlAccountId { get; set; }
    public bool IsActive { get; set; } = true;
"""),
    ("PaymentMethod", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""),
    ("PaymentVoucher", """
    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly DocumentDate { get; set; }
    public Guid BankAccountId { get; set; }
    public Guid? VendorId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string Status { get; set; } = "draft";
"""),
    ("PaymentVoucherLine", """
    public Guid PaymentVoucherId { get; set; }
    public int LineNo { get; set; }
    public Guid? AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
"""),
    ("ReceiptVoucher", """
    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly DocumentDate { get; set; }
    public Guid BankAccountId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string Status { get; set; } = "draft";
"""),
    ("ReceiptVoucherLine", """
    public Guid ReceiptVoucherId { get; set; }
    public int LineNo { get; set; }
    public Guid? AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
"""),
    ("BankReconciliation", """
    public Guid BankAccountId { get; set; }
    public DateOnly StatementDate { get; set; }
    public decimal StatementBalance { get; set; }
    public string Status { get; set; } = "open";
"""),
]:
    write(f"{B}/Domain/{name}.cs", aud("iERP.Modules.Banking.Domain", name, body))

# ---------- PROJECTS ----------
PJ = "src/Modules/Projects/iERP.Modules.Projects"
write(f"{PJ}/Domain/Project.cs", aud("iERP.Modules.Projects.Domain", "Project", """
    public Guid SubsidiaryId { get; set; }
    public Guid? BranchId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public Guid? ContractId { get; set; }
    public string? ProjectType { get; set; }
    public Guid? ProjectManagerId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? ContractValue { get; set; }
    public decimal? CostBudget { get; set; }
    public Guid? CostCenterId { get; set; }
    public string Status { get; set; } = "planned";
"""))
write(f"{PJ}/Domain/Contract.cs", aud("iERP.Modules.Projects.Domain", "Contract", """
    public Guid ProjectId { get; set; }
    public Guid CustomerId { get; set; }
    public string ContractNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContractType { get; set; }
    public decimal ContractValue { get; set; }
    public decimal? RetentionPercent { get; set; }
    public decimal? RetentionCap { get; set; }
    public int? DefectsLiabilityMonths { get; set; }
    public string? BillingBasis { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
"""))
write(f"{PJ}/Domain/RetentionRule.cs", aud("iERP.Modules.Projects.Domain", "RetentionRule", """
    public Guid ContractId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Percent { get; set; }
    public decimal? CapAmount { get; set; }
"""))
write(f"{PJ}/Domain/Subcontractor.cs", aud("iERP.Modules.Projects.Domain", "Subcontractor", """
    public Guid ProjectId { get; set; }
    public Guid? VendorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Scope { get; set; }
    public decimal? ContractValue { get; set; }
    public bool IsActive { get; set; } = true;
"""))

# ---------- HR ----------
write("src/Modules/HR/iERP.Modules.HR/Domain/Employee.cs", aud("iERP.Modules.HR.Domain", "Employee", """
    public Guid SubsidiaryId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? IdentificationNumber { get; set; }
    public string? Nationality { get; set; }
    public string? Designation { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? EmploymentType { get; set; }
    public DateOnly? JoinDate { get; set; }
    public string? SalaryGrade { get; set; }
    public decimal? AnnualLeaveEntitlement { get; set; }
    public string? WorkPassNumber { get; set; }
    public DateOnly? WorkPassExpiry { get; set; }
    public Guid? LinkedUserId { get; set; }
    public bool IsActive { get; set; } = true;
"""))

# ---------- MANUFACTURING ----------
M = "src/Modules/Manufacturing/iERP.Modules.Manufacturing"
write(f"{M}/Domain/BillOfMaterials.cs", aud("iERP.Modules.Manufacturing.Domain", "BillOfMaterials", """
    public Guid ItemId { get; set; }
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;
"""))
write(f"{M}/Domain/BillOfMaterialsLine.cs", aud("iERP.Modules.Manufacturing.Domain", "BillOfMaterialsLine", """
    public Guid BillOfMaterialsId { get; set; }
    public int LineNo { get; set; }
    public Guid ComponentItemId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UomId { get; set; }
"""))
write(f"{M}/Domain/WorkCentre.cs", aud("iERP.Modules.Manufacturing.Domain", "WorkCentre", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))
write(f"{M}/Domain/WorkOrder.cs", aud("iERP.Modules.Manufacturing.Domain", "WorkOrder", """
    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public Guid? BillOfMaterialsId { get; set; }
    public decimal Quantity { get; set; }
    public string Status { get; set; } = "planned";
    public DateOnly? PlannedStartDate { get; set; }
    public DateOnly? PlannedEndDate { get; set; }
"""))
write(f"{M}/Domain/WorkOrderLine.cs", aud("iERP.Modules.Manufacturing.Domain", "WorkOrderLine", """
    public Guid WorkOrderId { get; set; }
    public int LineNo { get; set; }
    public Guid ComponentItemId { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
"""))

# ---------- ASSETS ----------
A = "src/Modules/Assets/iERP.Modules.Assets"
write(f"{A}/Domain/AssetType.cs", aud("iERP.Modules.Assets.Domain", "AssetType", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))
write(f"{A}/Domain/Asset.cs", aud("iERP.Modules.Assets.Domain", "Asset", """
    public Guid SubsidiaryId { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? AssetTypeId { get; set; }
    public string? SerialNumber { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? DepreciationMethod { get; set; }
    public int? UsefulLifeYears { get; set; }
    public string? CurrentLocation { get; set; }
    public DateOnly? MaintenanceDueDate { get; set; }
    public DateOnly? CertificateExpiry { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? GlAssetAccountId { get; set; }
    public bool IsActive { get; set; } = true;
"""))
write(f"{A}/Domain/AssetMaintenanceSchedule.cs", aud("iERP.Modules.Assets.Domain", "AssetMaintenanceSchedule", """
    public Guid AssetId { get; set; }
    public string ScheduleName { get; set; } = string.Empty;
    public string Frequency { get; set; } = "monthly";
    public DateOnly? NextDueDate { get; set; }
    public bool IsActive { get; set; } = true;
"""))

# ---------- MARINE ----------
MAR = "src/Modules/Marine/iERP.Modules.Marine"
write(f"{MAR}/Domain/PortLocation.cs", aud("iERP.Modules.Marine.Domain", "PortLocation", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Country { get; set; }
    public bool IsActive { get; set; } = true;
"""))
write(f"{MAR}/Domain/Vessel.cs", aud("iERP.Modules.Marine.Domain", "Vessel", """
    public string VesselCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImoNumber { get; set; }
    public string? VesselType { get; set; }
    public string? FlagState { get; set; }
    public decimal? GrossTonnage { get; set; }
    public int? YearBuilt { get; set; }
    public string? Owner { get; set; }
    public string? ClassificationSociety { get; set; }
    public DateOnly? ClassCertificateExpiry { get; set; }
    public Guid? CurrentPortLocationId { get; set; }
    public bool IsActive { get; set; } = true;
"""))

# ---------- REPORTING ----------
write("src/Modules/Reporting/iERP.Modules.Reporting/Domain/ReportDefinition.cs", aud("iERP.Modules.Reporting.Domain", "ReportDefinition", """
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? QueryKey { get; set; }
    public bool IsActive { get; set; } = true;
"""))

# ---------- AI ----------
AI = "src/Modules/AI/iERP.Modules.AI"
write(f"{AI}/Domain/AIToolDefinition.cs", aud("iERP.Modules.AI.Domain", "AIToolDefinition", """
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
"""))
write(f"{AI}/Domain/AIToolPermission.cs", aud("iERP.Modules.AI.Domain", "AIToolPermission", """
    public Guid AIToolDefinitionId { get; set; }
    public Guid RoleId { get; set; }
    public string AllowedExecutionMode { get; set; } = "advisory";
"""))
write(f"{AI}/Domain/AILog.cs", aud("iERP.Modules.AI.Domain", "AILog", """
    public Guid UserId { get; set; }
    public string ToolName { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string? Response { get; set; }
    public string? ActionType { get; set; }
    public string ExecutionMode { get; set; } = "advisory";
    public string Status { get; set; } = "completed";
    public string? RollbackPayload { get; set; }
"""))

write(f"{AI}/Application/AIToolRegistry.cs", """
using iERP.Application.Abstractions.AI;

namespace iERP.Modules.AI.Application;

public sealed class AIToolRegistry : IAIToolRegistry
{
    private readonly Dictionary<string, IAITool> _tools = new(StringComparer.OrdinalIgnoreCase);

    public IAITool? Resolve(string toolName) =>
        _tools.TryGetValue(toolName, out var tool) ? tool : null;

    public IReadOnlyCollection<IAITool> GetAll() => _tools.Values.ToList();

    public void Register(IAITool tool) => _tools[tool.Name] = tool;
}
""")

write(f"{AI}/Application/NullAIGovernanceService.cs", """
using iERP.Application.Abstractions.AI;

namespace iERP.Modules.AI.Application;

public sealed class NullAIGovernanceService : IAIGovernanceService
{
    public Task<AIGovernanceDecision> AuthorizeAsync(
        Guid tenantId, Guid userId, string toolName, string executionMode, CancellationToken cancellationToken = default)
        => Task.FromResult(new AIGovernanceDecision(false, "AI governance not configured."));
}
""")

write(f"{AI}/Application/NullAIOrchestrator.cs", """
using iERP.Application.Abstractions.AI;

namespace iERP.Modules.AI.Application;

public sealed class NullAIOrchestrator : IAIOrchestrator
{
    public Task<AIOrchestrationResult> ExecuteAsync(AIOrchestrationRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(new AIOrchestrationResult(false, null, "not_implemented", "AI orchestrator placeholder."));
}
""")

print("business entities done")
