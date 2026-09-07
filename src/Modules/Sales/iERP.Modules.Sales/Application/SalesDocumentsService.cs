using iERP.Application.Abstractions.Metadata;
using iERP.Modules.Sales.Application.Dtos;
using iERP.Modules.Sales.Domain;
using iERP.Modules.Sales.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Sales.Application;

public interface ISalesDocumentsService
{
    Task<IReadOnlyList<SalesEnquiryDto>> ListEnquiriesAsync(CancellationToken cancellationToken);
    Task<SalesEnquiryDto> GetEnquiryAsync(Guid id, CancellationToken cancellationToken);
    Task<object> GetEnquiryExampleAsync(CancellationToken cancellationToken);
    Task<SalesEnquiryDto> CreateEnquiryAsync(UpsertSalesEnquiryRequest request, CancellationToken cancellationToken);
    Task<SalesEnquiryDto> UpdateEnquiryAsync(Guid id, UpsertSalesEnquiryRequest request, CancellationToken cancellationToken);
    Task DeleteEnquiryAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<SalesQuotationDto>> ListQuotationsAsync(CancellationToken cancellationToken);
    Task<SalesQuotationDto> GetQuotationAsync(Guid id, CancellationToken cancellationToken);
    Task<SalesQuotationDto> CreateQuotationAsync(UpsertSalesQuotationRequest request, CancellationToken cancellationToken);
    Task<SalesQuotationDto> UpdateQuotationAsync(Guid id, UpsertSalesQuotationRequest request, CancellationToken cancellationToken);
    Task DeleteQuotationAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<SalesOrderDto>> ListOrdersAsync(CancellationToken cancellationToken);
    Task<SalesOrderDto> GetOrderAsync(Guid id, CancellationToken cancellationToken);
    Task<SalesOrderDto> CreateOrderAsync(UpsertSalesOrderRequest request, CancellationToken cancellationToken);
    Task<SalesOrderDto> UpdateOrderAsync(Guid id, UpsertSalesOrderRequest request, CancellationToken cancellationToken);
    Task DeleteOrderAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<SalesInvoiceDto>> ListInvoicesAsync(CancellationToken cancellationToken);
    Task<SalesInvoiceDto> GetInvoiceAsync(Guid id, CancellationToken cancellationToken);
    Task<SalesInvoiceDto> CreateInvoiceAsync(UpsertSalesInvoiceRequest request, CancellationToken cancellationToken);
    Task<SalesInvoiceDto> UpdateInvoiceAsync(Guid id, UpsertSalesInvoiceRequest request, CancellationToken cancellationToken);
    Task DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<SalesAdjustmentNoteDto>> ListCreditNotesAsync(CancellationToken cancellationToken);
    Task<SalesAdjustmentNoteDto> GetCreditNoteAsync(Guid id, CancellationToken cancellationToken);
    Task<SalesAdjustmentNoteDto> CreateCreditNoteAsync(UpsertSalesAdjustmentNoteRequest request, CancellationToken cancellationToken);
    Task<SalesAdjustmentNoteDto> UpdateCreditNoteAsync(Guid id, UpsertSalesAdjustmentNoteRequest request, CancellationToken cancellationToken);
    Task DeleteCreditNoteAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<SalesAdjustmentNoteDto>> ListDebitNotesAsync(CancellationToken cancellationToken);
    Task<SalesAdjustmentNoteDto> GetDebitNoteAsync(Guid id, CancellationToken cancellationToken);
    Task<SalesAdjustmentNoteDto> CreateDebitNoteAsync(UpsertSalesAdjustmentNoteRequest request, CancellationToken cancellationToken);
    Task<SalesAdjustmentNoteDto> UpdateDebitNoteAsync(Guid id, UpsertSalesAdjustmentNoteRequest request, CancellationToken cancellationToken);
    Task DeleteDebitNoteAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class SalesDocumentsService : ISalesDocumentsService
{
    private readonly SalesDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly IClock _clock;

    public SalesDocumentsService(SalesDbContext db, ITenantContext tenantContext, IClock clock)
    {
        _db = db;
        _tenantContext = tenantContext;
        _clock = clock;
    }

    public async Task<IReadOnlyList<SalesEnquiryDto>> ListEnquiriesAsync(CancellationToken cancellationToken)
    {
        EnsureTenant();
        var rows = await _db.SalesEnquiries.AsNoTracking()
            .Include(x => x.Lines)
            .Include(x => x.FollowUps)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return rows.Select(MapEnquiry).ToList();
    }

    public async Task<SalesEnquiryDto> GetEnquiryAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var row = await _db.SalesEnquiries.AsNoTracking()
            .Include(x => x.Lines)
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales enquiry '{id}' was not found.");
        return MapEnquiry(row);
    }

    public Task<object> GetEnquiryExampleAsync(CancellationToken cancellationToken) =>
        Task.FromResult(SalesEnquiryExamplePayload.Document);

    public async Task<SalesEnquiryDto> CreateEnquiryAsync(UpsertSalesEnquiryRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateEnquiry(request);

        var entity = new SalesEnquiry();
        entity.SetTenantId(tenantId);
        await ApplyEnquiry(entity, request, isCreate: true);
        ApplyEnquiryLines(entity, request.Items, tenantId);

        if (request.CreateInitialFollowUp)
        {
            var followUp = new SalesEnquiryFollowUp
            {
                FollowUpDate = request.FollowUpDate,
                NextFollowUpDate = request.NextFollowUpDate,
                Status = request.FollowUpStatus ?? "Pending",
                ActivityType = request.ActivityType,
                Remarks = request.Remarks
            };
            followUp.SetTenantId(tenantId);
            entity.AddFollowUp(followUp);
        }

        _db.SalesEnquiries.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetEnquiryAsync(entity.Id, cancellationToken);
    }

    public async Task<SalesEnquiryDto> UpdateEnquiryAsync(Guid id, UpsertSalesEnquiryRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateEnquiry(request);

        var entity = await _db.SalesEnquiries
            .Include(x => x.Lines)
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales enquiry '{id}' was not found.");

        await ApplyEnquiry(entity, request, isCreate: false);

        foreach (var line in entity.Lines.ToList())
        {
            _db.SalesEnquiryLines.Remove(line);
        }

        ApplyEnquiryLines(entity, request.Items, tenantId);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetEnquiryAsync(id, cancellationToken);
    }

    public async Task DeleteEnquiryAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var entity = await _db.SalesEnquiries
            .Include(x => x.Lines)
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales enquiry '{id}' was not found.");

        var now = _clock.UtcNow;
        foreach (var line in entity.Lines)
        {
            line.SoftDelete(null, now);
        }

        foreach (var followUp in entity.FollowUps)
        {
            followUp.SoftDelete(null, now);
        }

        entity.SoftDelete(null, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SalesQuotationDto>> ListQuotationsAsync(CancellationToken cancellationToken)
    {
        EnsureTenant();
        var rows = await _db.SalesQuotations.AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return rows.Select(MapQuotation).ToList();
    }

    public async Task<SalesQuotationDto> GetQuotationAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var row = await _db.SalesQuotations.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales quotation '{id}' was not found.");
        return MapQuotation(row);
    }

    public async Task<SalesQuotationDto> CreateQuotationAsync(UpsertSalesQuotationRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateQuotation(request);

        var entity = new SalesQuotation();
        entity.SetTenantId(tenantId);
        await ApplyQuotation(entity, request, isCreate: true, cancellationToken);
        ApplyQuotationLines(entity, request.Items, tenantId);
        _db.SalesQuotations.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetQuotationAsync(entity.Id, cancellationToken);
    }

    public async Task<SalesQuotationDto> UpdateQuotationAsync(Guid id, UpsertSalesQuotationRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateQuotation(request);

        var entity = await _db.SalesQuotations
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales quotation '{id}' was not found.");

        await ApplyQuotation(entity, request, isCreate: false, cancellationToken);
        foreach (var line in entity.Lines.ToList())
        {
            _db.SalesQuotationLines.Remove(line);
        }

        ApplyQuotationLines(entity, request.Items, tenantId);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetQuotationAsync(id, cancellationToken);
    }

    public async Task DeleteQuotationAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var entity = await _db.SalesQuotations
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales quotation '{id}' was not found.");

        var now = _clock.UtcNow;
        foreach (var line in entity.Lines)
        {
            line.SoftDelete(null, now);
        }

        entity.SoftDelete(null, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SalesOrderDto>> ListOrdersAsync(CancellationToken cancellationToken)
    {
        EnsureTenant();
        var rows = await _db.SalesOrders.AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return rows.Select(MapOrder).ToList();
    }

    public async Task<SalesOrderDto> GetOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var row = await _db.SalesOrders.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales order '{id}' was not found.");
        return MapOrder(row);
    }

    public async Task<SalesOrderDto> CreateOrderAsync(UpsertSalesOrderRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateOrder(request);

        var entity = new SalesOrder();
        entity.SetTenantId(tenantId);
        await ApplyOrder(entity, request, isCreate: true, cancellationToken);
        ApplyOrderLines(entity, request.Items, tenantId);
        _db.SalesOrders.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetOrderAsync(entity.Id, cancellationToken);
    }

    public async Task<SalesOrderDto> UpdateOrderAsync(Guid id, UpsertSalesOrderRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateOrder(request);

        var entity = await _db.SalesOrders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales order '{id}' was not found.");

        await ApplyOrder(entity, request, isCreate: false, cancellationToken);
        foreach (var line in entity.Lines.ToList())
        {
            _db.SalesOrderLines.Remove(line);
        }

        ApplyOrderLines(entity, request.Items, tenantId);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetOrderAsync(id, cancellationToken);
    }

    public async Task DeleteOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var entity = await _db.SalesOrders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales order '{id}' was not found.");

        var now = _clock.UtcNow;
        foreach (var line in entity.Lines)
        {
            line.SoftDelete(null, now);
        }

        entity.SoftDelete(null, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SalesInvoiceDto>> ListInvoicesAsync(CancellationToken cancellationToken)
    {
        EnsureTenant();
        var rows = await _db.SalesInvoices.AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return rows.Select(MapInvoice).ToList();
    }

    public async Task<SalesInvoiceDto> GetInvoiceAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var row = await _db.SalesInvoices.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales invoice '{id}' was not found.");
        return MapInvoice(row);
    }

    public async Task<SalesInvoiceDto> CreateInvoiceAsync(UpsertSalesInvoiceRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateInvoice(request);

        var entity = new SalesInvoice();
        entity.SetTenantId(tenantId);
        await ApplyInvoice(entity, request, isCreate: true, cancellationToken);
        ApplyInvoiceLines(entity, request.Items, tenantId);
        _db.SalesInvoices.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetInvoiceAsync(entity.Id, cancellationToken);
    }

    public async Task<SalesInvoiceDto> UpdateInvoiceAsync(Guid id, UpsertSalesInvoiceRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateInvoice(request);

        var entity = await _db.SalesInvoices
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales invoice '{id}' was not found.");

        await ApplyInvoice(entity, request, isCreate: false, cancellationToken);
        foreach (var line in entity.Lines.ToList())
        {
            _db.SalesInvoiceLines.Remove(line);
        }

        ApplyInvoiceLines(entity, request.Items, tenantId);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetInvoiceAsync(id, cancellationToken);
    }

    public async Task DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var entity = await _db.SalesInvoices
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Sales invoice '{id}' was not found.");

        var now = _clock.UtcNow;
        foreach (var line in entity.Lines)
        {
            line.SoftDelete(null, now);
        }

        entity.SoftDelete(null, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SalesAdjustmentNoteDto>> ListCreditNotesAsync(CancellationToken cancellationToken)
    {
        EnsureTenant();
        var rows = await _db.CreditNotes.AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return rows.Select(MapCreditNote).ToList();
    }

    public async Task<SalesAdjustmentNoteDto> GetCreditNoteAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var row = await _db.CreditNotes.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Credit note '{id}' was not found.");
        return MapCreditNote(row);
    }

    public async Task<SalesAdjustmentNoteDto> CreateCreditNoteAsync(UpsertSalesAdjustmentNoteRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateAdjustmentNote(request);

        var entity = new CreditNote();
        entity.SetTenantId(tenantId);
        await ApplyCreditNote(entity, request, isCreate: true, cancellationToken);
        ApplyCreditNoteLines(entity, request.Items, tenantId);
        _db.CreditNotes.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetCreditNoteAsync(entity.Id, cancellationToken);
    }

    public async Task<SalesAdjustmentNoteDto> UpdateCreditNoteAsync(Guid id, UpsertSalesAdjustmentNoteRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateAdjustmentNote(request);

        var entity = await _db.CreditNotes
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Credit note '{id}' was not found.");

        await ApplyCreditNote(entity, request, isCreate: false, cancellationToken);
        foreach (var line in entity.Lines.ToList())
        {
            _db.CreditNoteLines.Remove(line);
        }

        ApplyCreditNoteLines(entity, request.Items, tenantId);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetCreditNoteAsync(id, cancellationToken);
    }

    public async Task DeleteCreditNoteAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var entity = await _db.CreditNotes
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Credit note '{id}' was not found.");

        var now = _clock.UtcNow;
        foreach (var line in entity.Lines)
        {
            line.SoftDelete(null, now);
        }

        entity.SoftDelete(null, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SalesAdjustmentNoteDto>> ListDebitNotesAsync(CancellationToken cancellationToken)
    {
        EnsureTenant();
        var rows = await _db.DebitNotes.AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return rows.Select(MapDebitNote).ToList();
    }

    public async Task<SalesAdjustmentNoteDto> GetDebitNoteAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var row = await _db.DebitNotes.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Debit note '{id}' was not found.");
        return MapDebitNote(row);
    }

    public async Task<SalesAdjustmentNoteDto> CreateDebitNoteAsync(UpsertSalesAdjustmentNoteRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateAdjustmentNote(request);

        var entity = new DebitNote();
        entity.SetTenantId(tenantId);
        await ApplyDebitNote(entity, request, isCreate: true, cancellationToken);
        ApplyDebitNoteLines(entity, request.Items, tenantId);
        _db.DebitNotes.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetDebitNoteAsync(entity.Id, cancellationToken);
    }

    public async Task<SalesAdjustmentNoteDto> UpdateDebitNoteAsync(Guid id, UpsertSalesAdjustmentNoteRequest request, CancellationToken cancellationToken)
    {
        var tenantId = EnsureTenant();
        ValidateAdjustmentNote(request);

        var entity = await _db.DebitNotes
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Debit note '{id}' was not found.");

        await ApplyDebitNote(entity, request, isCreate: false, cancellationToken);
        foreach (var line in entity.Lines.ToList())
        {
            _db.DebitNoteLines.Remove(line);
        }

        ApplyDebitNoteLines(entity, request.Items, tenantId);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetDebitNoteAsync(id, cancellationToken);
    }

    public async Task DeleteDebitNoteAsync(Guid id, CancellationToken cancellationToken)
    {
        EnsureTenant();
        var entity = await _db.DebitNotes
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Debit note '{id}' was not found.");

        var now = _clock.UtcNow;
        foreach (var line in entity.Lines)
        {
            line.SoftDelete(null, now);
        }

        entity.SoftDelete(null, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private Guid EnsureTenant()
    {
        if (!_tenantContext.HasTenant || _tenantContext.TenantId is not Guid tenantId)
        {
            throw new ForbiddenException("Tenant context is required.", ErrorCodes.TenantNotFound);
        }

        return tenantId;
    }

    private static void ValidateEnquiry(UpsertSalesEnquiryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
        {
            throw new ValidationException("Customer is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Title is required.");
        }
    }

    private static void ValidateQuotation(UpsertSalesQuotationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
        {
            throw new ValidationException("Customer is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Title is required.");
        }
    }

    private static void ValidateInvoice(UpsertSalesInvoiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
        {
            throw new ValidationException("Customer is required.");
        }
    }

    private static void ValidateOrder(UpsertSalesOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
        {
            throw new ValidationException("Customer is required.");
        }
    }

    private static void ValidateAdjustmentNote(UpsertSalesAdjustmentNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
        {
            throw new ValidationException("Customer is required.");
        }
    }

    private async Task ApplyEnquiry(SalesEnquiry entity, UpsertSalesEnquiryRequest request, bool isCreate)
    {
        if (isCreate || !string.IsNullOrWhiteSpace(request.EnquiryCode))
        {
            entity.EnquiryCode = string.IsNullOrWhiteSpace(request.EnquiryCode)
                ? await NextCodeAsync("TEA-ENQ")
                : request.EnquiryCode.Trim();
        }

        entity.Customer = request.Customer.Trim();
        entity.ContactPerson = Trim(request.ContactPerson);
        entity.Opportunity = Trim(request.Opportunity);
        entity.Email = Trim(request.Email);
        entity.Phone = Trim(request.Phone);
        entity.AltPhone = Trim(request.AltPhone);
        entity.Probability = request.Probability;
        entity.Title = request.Title.Trim();
        entity.ExpectedClose = request.ExpectedClose;
        entity.Status = string.IsNullOrWhiteSpace(request.Status) ? "Open" : request.Status.Trim();
        entity.WinLossReason = Trim(request.WinLossReason);
        entity.ProjectedTotal = request.ProjectedTotal;
        entity.ForecastType = Trim(request.ForecastType);
        entity.WeightedTotal = request.WeightedTotal;
        entity.Range = Trim(request.Range);
        entity.Subsidiary = Trim(request.Subsidiary);
        entity.ClassName = Trim(request.ClassName);
        entity.Location = Trim(request.Location);
        entity.Department = Trim(request.Department);
        entity.SalesRep = Trim(request.SalesRep);
        entity.LastSalesActivity = request.LastSalesActivity;
        entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? "INR - RUPEE" : request.Currency.Trim();
        entity.ExchangeRate = request.ExchangeRate ?? 1m;
        entity.Payment = Trim(request.Payment);
        entity.Prices = Trim(request.Prices);
        entity.Delivery = Trim(request.Delivery);
    }

    private void ApplyEnquiryLines(SalesEnquiry entity, IReadOnlyList<SalesLineItemDto>? items, Guid tenantId)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        var lineNo = 1;
        var lines = new List<SalesEnquiryLine>();
        foreach (var item in items)
        {
            var line = new SalesEnquiryLine
            {
                LineNo = lineNo++,
                Category = Trim(item.Category),
                Item = Trim(item.Item),
                Description = Trim(item.Description),
                Quantity = item.Quantity,
                Uom = Trim(item.Uom),
                PriceLevel = Trim(item.PriceLevel),
                Rate = item.Rate,
                Discount = item.Discount,
                Amount = item.Amount,
                TaxCode = Trim(item.TaxCode),
                GrossAmount = item.GrossAmount,
                ClassName = Trim(item.ClassName),
                CountryOfOrigin = Trim(item.CountryOfOrigin),
                HsCode = Trim(item.HsCode)
            };
            line.SetTenantId(tenantId);
            lines.Add(line);
        }

        entity.ReplaceLines(lines);
    }

    private async Task ApplyQuotation(SalesQuotation entity, UpsertSalesQuotationRequest request, bool isCreate, CancellationToken cancellationToken)
    {
        if (isCreate || !string.IsNullOrWhiteSpace(request.QuotationCode))
        {
            entity.QuotationCode = string.IsNullOrWhiteSpace(request.QuotationCode)
                ? await NextCodeAsync("TEA-QTN")
                : request.QuotationCode.Trim();
        }

        entity.Customer = request.Customer.Trim();
        entity.ContactPerson = Trim(request.ContactPerson);
        entity.Email = Trim(request.Email);
        entity.Phone = Trim(request.Phone);
        entity.Title = request.Title.Trim();
        entity.Status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim();
        entity.DocumentDate = request.DocumentDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        entity.ValidUntil = request.ValidUntil;
        entity.Subsidiary = Trim(request.Subsidiary);
        entity.ClassName = Trim(request.ClassName);
        entity.Location = Trim(request.Location);
        entity.Department = Trim(request.Department);
        entity.SalesRep = Trim(request.SalesRep);
        entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? "INR - RUPEE" : request.Currency.Trim();
        entity.ExchangeRate = request.ExchangeRate ?? 1m;
        entity.ProjectedTotal = request.ProjectedTotal;
        entity.Payment = Trim(request.Payment);
        entity.Prices = Trim(request.Prices);
        entity.Delivery = Trim(request.Delivery);
        entity.Notes = Trim(request.Notes);
        entity.TotalAmount = request.ProjectedTotal ?? 0m;

        if (request.SourceEnquiryId.HasValue)
        {
            var enquiry = await _db.SalesEnquiries.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SourceEnquiryId.Value, cancellationToken)
                ?? throw new ValidationException($"Source enquiry '{request.SourceEnquiryId}' was not found.");
            entity.SourceEnquiryId = enquiry.Id;
            entity.SourceEnquiryCode = enquiry.EnquiryCode;
        }
        else
        {
            entity.SourceEnquiryId = null;
            entity.SourceEnquiryCode = Trim(request.SourceEnquiryCode);
        }
    }

    private void ApplyQuotationLines(SalesQuotation entity, IReadOnlyList<SalesLineItemDto>? items, Guid tenantId)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        var lineNo = 1;
        var lines = new List<SalesQuotationLine>();
        foreach (var item in items)
        {
            var line = MapLineToQuotation(item, lineNo++, tenantId);
            lines.Add(line);
        }

        entity.ReplaceLines(lines);
    }

    private async Task ApplyInvoice(SalesInvoice entity, UpsertSalesInvoiceRequest request, bool isCreate, CancellationToken cancellationToken)
    {
        if (isCreate || !string.IsNullOrWhiteSpace(request.InvoiceCode))
        {
            entity.InvoiceCode = string.IsNullOrWhiteSpace(request.InvoiceCode)
                ? await NextCodeAsync("TEA-INV")
                : request.InvoiceCode.Trim();
        }

        entity.Customer = request.Customer.Trim();
        entity.ContactPerson = Trim(request.ContactPerson);
        entity.Email = Trim(request.Email);
        entity.Phone = Trim(request.Phone);
        entity.Title = Trim(request.Title);
        entity.Status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim();
        entity.DocumentDate = request.DocumentDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        entity.DueDate = request.DueDate;
        entity.Subsidiary = Trim(request.Subsidiary);
        entity.ClassName = Trim(request.ClassName);
        entity.Location = Trim(request.Location);
        entity.Department = Trim(request.Department);
        entity.SalesRep = Trim(request.SalesRep);
        entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? "INR - RUPEE" : request.Currency.Trim();
        entity.ExchangeRate = request.ExchangeRate ?? 1m;
        entity.ProjectedTotal = request.ProjectedTotal;
        entity.Payment = Trim(request.Payment);
        entity.Notes = Trim(request.Notes);
        entity.TotalAmount = request.ProjectedTotal ?? 0m;

        if (request.SourceOrderId.HasValue)
        {
            var order = await _db.SalesOrders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SourceOrderId.Value, cancellationToken)
                ?? throw new ValidationException($"Source order '{request.SourceOrderId}' was not found.");
            entity.SourceOrderId = order.Id;
            entity.SourceOrderCode = order.OrderCode;
        }
        else
        {
            entity.SourceOrderId = null;
            entity.SourceOrderCode = Trim(request.SourceOrderCode);
        }

        if (request.SourceQuotationId.HasValue)
        {
            var quotation = await _db.SalesQuotations.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SourceQuotationId.Value, cancellationToken)
                ?? throw new ValidationException($"Source quotation '{request.SourceQuotationId}' was not found.");
            entity.SourceQuotationId = quotation.Id;
            entity.SourceQuotationCode = quotation.QuotationCode;
        }
        else
        {
            entity.SourceQuotationId = null;
            entity.SourceQuotationCode = Trim(request.SourceQuotationCode);
        }
    }

    private async Task ApplyOrder(SalesOrder entity, UpsertSalesOrderRequest request, bool isCreate, CancellationToken cancellationToken)
    {
        if (isCreate || !string.IsNullOrWhiteSpace(request.OrderCode))
        {
            entity.OrderCode = string.IsNullOrWhiteSpace(request.OrderCode)
                ? await NextCodeAsync("TEA-SO")
                : request.OrderCode.Trim();
        }

        entity.Customer = request.Customer.Trim();
        entity.ContactPerson = Trim(request.ContactPerson);
        entity.Email = Trim(request.Email);
        entity.Phone = Trim(request.Phone);
        entity.Title = Trim(request.Title);
        entity.Status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim();
        entity.DocumentDate = request.DocumentDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        entity.DeliveryDate = request.DeliveryDate;
        entity.Subsidiary = Trim(request.Subsidiary);
        entity.ClassName = Trim(request.ClassName);
        entity.Location = Trim(request.Location);
        entity.Department = Trim(request.Department);
        entity.SalesRep = Trim(request.SalesRep);
        entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? "INR - RUPEE" : request.Currency.Trim();
        entity.ExchangeRate = request.ExchangeRate ?? 1m;
        entity.ProjectedTotal = request.ProjectedTotal;
        entity.Payment = Trim(request.Payment);
        entity.Prices = Trim(request.Prices);
        entity.Delivery = Trim(request.Delivery);
        entity.Notes = Trim(request.Notes);
        entity.TotalAmount = request.ProjectedTotal ?? 0m;

        if (request.SourceEnquiryId.HasValue)
        {
            var enquiry = await _db.SalesEnquiries.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SourceEnquiryId.Value, cancellationToken)
                ?? throw new ValidationException($"Source enquiry '{request.SourceEnquiryId}' was not found.");
            entity.SourceEnquiryId = enquiry.Id;
            entity.SourceEnquiryCode = enquiry.EnquiryCode;
        }
        else
        {
            entity.SourceEnquiryId = null;
            entity.SourceEnquiryCode = Trim(request.SourceEnquiryCode);
        }

        if (request.SourceQuotationId.HasValue)
        {
            var quotation = await _db.SalesQuotations.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SourceQuotationId.Value, cancellationToken)
                ?? throw new ValidationException($"Source quotation '{request.SourceQuotationId}' was not found.");
            entity.SourceQuotationId = quotation.Id;
            entity.SourceQuotationCode = quotation.QuotationCode;
        }
        else
        {
            entity.SourceQuotationId = null;
            entity.SourceQuotationCode = Trim(request.SourceQuotationCode);
        }
    }

    private void ApplyOrderLines(SalesOrder entity, IReadOnlyList<SalesLineItemDto>? items, Guid tenantId)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        var lineNo = 1;
        var lines = new List<SalesOrderLine>();
        foreach (var item in items)
        {
            lines.Add(MapLineToOrder(item, lineNo++, tenantId));
        }

        entity.ReplaceLines(lines);
    }

    private async Task ApplyCreditNote(CreditNote entity, UpsertSalesAdjustmentNoteRequest request, bool isCreate, CancellationToken cancellationToken)
    {
        if (isCreate || !string.IsNullOrWhiteSpace(request.NoteCode))
        {
            entity.NoteCode = string.IsNullOrWhiteSpace(request.NoteCode)
                ? await NextCodeAsync("TEA-CN")
                : request.NoteCode.Trim();
        }

        entity.Customer = request.Customer.Trim();
        entity.ContactPerson = Trim(request.ContactPerson);
        entity.Email = Trim(request.Email);
        entity.Phone = Trim(request.Phone);
        entity.Title = Trim(request.Title);
        entity.Status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim();
        entity.DocumentDate = request.DocumentDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        entity.DueDate = request.DueDate;
        entity.Reason = Trim(request.Reason);
        entity.Subsidiary = Trim(request.Subsidiary);
        entity.ClassName = Trim(request.ClassName);
        entity.Location = Trim(request.Location);
        entity.Department = Trim(request.Department);
        entity.SalesRep = Trim(request.SalesRep);
        entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? "INR - RUPEE" : request.Currency.Trim();
        entity.ExchangeRate = request.ExchangeRate ?? 1m;
        entity.ProjectedTotal = request.ProjectedTotal;
        entity.Payment = Trim(request.Payment);
        entity.Notes = Trim(request.Notes);
        entity.TotalAmount = request.ProjectedTotal ?? 0m;

        if (request.SourceInvoiceId.HasValue)
        {
            var invoice = await _db.SalesInvoices.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SourceInvoiceId.Value, cancellationToken)
                ?? throw new ValidationException($"Source invoice '{request.SourceInvoiceId}' was not found.");
            entity.SourceInvoiceId = invoice.Id;
            entity.SourceInvoiceCode = invoice.InvoiceCode;
        }
        else
        {
            entity.SourceInvoiceId = null;
            entity.SourceInvoiceCode = Trim(request.SourceInvoiceCode);
        }
    }

    private async Task ApplyDebitNote(DebitNote entity, UpsertSalesAdjustmentNoteRequest request, bool isCreate, CancellationToken cancellationToken)
    {
        if (isCreate || !string.IsNullOrWhiteSpace(request.NoteCode))
        {
            entity.NoteCode = string.IsNullOrWhiteSpace(request.NoteCode)
                ? await NextCodeAsync("TEA-DN")
                : request.NoteCode.Trim();
        }

        entity.Customer = request.Customer.Trim();
        entity.ContactPerson = Trim(request.ContactPerson);
        entity.Email = Trim(request.Email);
        entity.Phone = Trim(request.Phone);
        entity.Title = Trim(request.Title);
        entity.Status = string.IsNullOrWhiteSpace(request.Status) ? "Draft" : request.Status.Trim();
        entity.DocumentDate = request.DocumentDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        entity.DueDate = request.DueDate;
        entity.Reason = Trim(request.Reason);
        entity.Subsidiary = Trim(request.Subsidiary);
        entity.ClassName = Trim(request.ClassName);
        entity.Location = Trim(request.Location);
        entity.Department = Trim(request.Department);
        entity.SalesRep = Trim(request.SalesRep);
        entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? "INR - RUPEE" : request.Currency.Trim();
        entity.ExchangeRate = request.ExchangeRate ?? 1m;
        entity.ProjectedTotal = request.ProjectedTotal;
        entity.Payment = Trim(request.Payment);
        entity.Notes = Trim(request.Notes);
        entity.TotalAmount = request.ProjectedTotal ?? 0m;

        if (request.SourceInvoiceId.HasValue)
        {
            var invoice = await _db.SalesInvoices.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SourceInvoiceId.Value, cancellationToken)
                ?? throw new ValidationException($"Source invoice '{request.SourceInvoiceId}' was not found.");
            entity.SourceInvoiceId = invoice.Id;
            entity.SourceInvoiceCode = invoice.InvoiceCode;
        }
        else
        {
            entity.SourceInvoiceId = null;
            entity.SourceInvoiceCode = Trim(request.SourceInvoiceCode);
        }
    }

    private void ApplyCreditNoteLines(CreditNote entity, IReadOnlyList<SalesLineItemDto>? items, Guid tenantId)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        var lineNo = 1;
        var lines = new List<CreditNoteLine>();
        foreach (var item in items)
        {
            lines.Add(MapLineToCreditNote(item, lineNo++, tenantId));
        }

        entity.ReplaceLines(lines);
    }

    private void ApplyDebitNoteLines(DebitNote entity, IReadOnlyList<SalesLineItemDto>? items, Guid tenantId)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        var lineNo = 1;
        var lines = new List<DebitNoteLine>();
        foreach (var item in items)
        {
            lines.Add(MapLineToDebitNote(item, lineNo++, tenantId));
        }

        entity.ReplaceLines(lines);
    }

    private void ApplyInvoiceLines(SalesInvoice entity, IReadOnlyList<SalesLineItemDto>? items, Guid tenantId)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        var lineNo = 1;
        var lines = new List<SalesInvoiceLine>();
        foreach (var item in items)
        {
            lines.Add(MapLineToInvoice(item, lineNo++, tenantId));
        }

        entity.ReplaceLines(lines);
    }

    private async Task<string> NextCodeAsync(string prefix)
    {
        var stamp = DateTime.UtcNow.ToString("yyMM");
        var seq = Random.Shared.Next(1, 99999);
        await Task.CompletedTask;
        return $"{prefix}-{stamp}-{seq:D5}";
    }

    private static SalesQuotationLine MapLineToQuotation(SalesLineItemDto item, int lineNo, Guid tenantId)
    {
        var line = new SalesQuotationLine
        {
            LineNo = lineNo,
            Category = Trim(item.Category),
            Item = Trim(item.Item),
            Description = Trim(item.Description),
            Quantity = item.Quantity,
            Uom = Trim(item.Uom),
            PriceLevel = Trim(item.PriceLevel),
            Rate = item.Rate,
            Discount = item.Discount,
            Amount = item.Amount,
            TaxCode = Trim(item.TaxCode),
            GrossAmount = item.GrossAmount,
            ClassName = Trim(item.ClassName),
            CountryOfOrigin = Trim(item.CountryOfOrigin),
            HsCode = Trim(item.HsCode),
            UnitPrice = item.Rate,
            DiscountPercent = item.Discount,
            LineAmount = item.Amount
        };
        line.SetTenantId(tenantId);
        return line;
    }

    private static SalesInvoiceLine MapLineToInvoice(SalesLineItemDto item, int lineNo, Guid tenantId)
    {
        var line = new SalesInvoiceLine
        {
            LineNo = lineNo,
            Category = Trim(item.Category),
            Item = Trim(item.Item),
            Description = Trim(item.Description),
            Quantity = item.Quantity,
            Uom = Trim(item.Uom),
            PriceLevel = Trim(item.PriceLevel),
            Rate = item.Rate,
            Discount = item.Discount,
            Amount = item.Amount,
            TaxCode = Trim(item.TaxCode),
            GrossAmount = item.GrossAmount,
            ClassName = Trim(item.ClassName),
            CountryOfOrigin = Trim(item.CountryOfOrigin),
            HsCode = Trim(item.HsCode),
            UnitPrice = item.Rate,
            DiscountPercent = item.Discount,
            LineAmount = item.Amount
        };
        line.SetTenantId(tenantId);
        return line;
    }

    private static SalesOrderLine MapLineToOrder(SalesLineItemDto item, int lineNo, Guid tenantId)
    {
        var line = new SalesOrderLine
        {
            LineNo = lineNo,
            Category = Trim(item.Category),
            Item = Trim(item.Item),
            Description = Trim(item.Description),
            Quantity = item.Quantity,
            Uom = Trim(item.Uom),
            PriceLevel = Trim(item.PriceLevel),
            Rate = item.Rate,
            Discount = item.Discount,
            Amount = item.Amount,
            TaxCode = Trim(item.TaxCode),
            GrossAmount = item.GrossAmount,
            ClassName = Trim(item.ClassName),
            CountryOfOrigin = Trim(item.CountryOfOrigin),
            HsCode = Trim(item.HsCode),
            UnitPrice = item.Rate,
            DiscountPercent = item.Discount,
            LineAmount = item.Amount
        };
        line.SetTenantId(tenantId);
        return line;
    }

    private static CreditNoteLine MapLineToCreditNote(SalesLineItemDto item, int lineNo, Guid tenantId)
    {
        var line = new CreditNoteLine
        {
            LineNo = lineNo,
            Category = Trim(item.Category),
            Item = Trim(item.Item),
            Description = Trim(item.Description),
            Quantity = item.Quantity,
            Uom = Trim(item.Uom),
            PriceLevel = Trim(item.PriceLevel),
            Rate = item.Rate,
            Discount = item.Discount,
            Amount = item.Amount,
            TaxCode = Trim(item.TaxCode),
            GrossAmount = item.GrossAmount,
            ClassName = Trim(item.ClassName),
            CountryOfOrigin = Trim(item.CountryOfOrigin),
            HsCode = Trim(item.HsCode),
            UnitPrice = item.Rate,
            DiscountPercent = item.Discount,
            LineAmount = item.Amount
        };
        line.SetTenantId(tenantId);
        return line;
    }

    private static DebitNoteLine MapLineToDebitNote(SalesLineItemDto item, int lineNo, Guid tenantId)
    {
        var line = new DebitNoteLine
        {
            LineNo = lineNo,
            Category = Trim(item.Category),
            Item = Trim(item.Item),
            Description = Trim(item.Description),
            Quantity = item.Quantity,
            Uom = Trim(item.Uom),
            PriceLevel = Trim(item.PriceLevel),
            Rate = item.Rate,
            Discount = item.Discount,
            Amount = item.Amount,
            TaxCode = Trim(item.TaxCode),
            GrossAmount = item.GrossAmount,
            ClassName = Trim(item.ClassName),
            CountryOfOrigin = Trim(item.CountryOfOrigin),
            HsCode = Trim(item.HsCode),
            UnitPrice = item.Rate,
            DiscountPercent = item.Discount,
            LineAmount = item.Amount
        };
        line.SetTenantId(tenantId);
        return line;
    }

    private static SalesEnquiryDto MapEnquiry(SalesEnquiry entity)
    {
        var latestFollowUp = entity.FollowUps
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();

        return new SalesEnquiryDto
        {
            Id = entity.Id,
            EnquiryCode = entity.EnquiryCode,
            Customer = entity.Customer,
            ContactPerson = entity.ContactPerson,
            Opportunity = entity.Opportunity,
            Email = entity.Email,
            Phone = entity.Phone,
            AltPhone = entity.AltPhone,
            Probability = entity.Probability,
            Title = entity.Title,
            ExpectedClose = entity.ExpectedClose,
            Status = entity.Status,
            WinLossReason = entity.WinLossReason,
            ProjectedTotal = entity.ProjectedTotal,
            ForecastType = entity.ForecastType,
            WeightedTotal = entity.WeightedTotal,
            Range = entity.Range,
            Subsidiary = entity.Subsidiary,
            ClassName = entity.ClassName,
            Location = entity.Location,
            Department = entity.Department,
            SalesRep = entity.SalesRep,
            LastSalesActivity = entity.LastSalesActivity,
            Currency = entity.Currency,
            ExchangeRate = entity.ExchangeRate,
            Payment = entity.Payment,
            Prices = entity.Prices,
            Delivery = entity.Delivery,
            CreateInitialFollowUp = latestFollowUp is not null,
            FollowUpDate = latestFollowUp?.FollowUpDate,
            NextFollowUpDate = latestFollowUp?.NextFollowUpDate,
            FollowUpStatus = latestFollowUp?.Status,
            ActivityType = latestFollowUp?.ActivityType,
            Remarks = latestFollowUp?.Remarks,
            Items = entity.Lines.OrderBy(x => x.LineNo).Select(MapLine).ToList(),
            FollowUps = entity.FollowUps.Select(f => new SalesFollowUpDto(
                f.Id, f.FollowUpDate, f.NextFollowUpDate, f.Status, f.ActivityType, f.Remarks)).ToList()
        };
    }

    private static SalesQuotationDto MapQuotation(SalesQuotation entity) => new()
    {
        Id = entity.Id,
        QuotationCode = entity.QuotationCode,
        Customer = entity.Customer,
        ContactPerson = entity.ContactPerson,
        Email = entity.Email,
        Phone = entity.Phone,
        Title = entity.Title,
        Status = entity.Status,
        DocumentDate = entity.DocumentDate,
        ValidUntil = entity.ValidUntil,
        Subsidiary = entity.Subsidiary,
        ClassName = entity.ClassName,
        Location = entity.Location,
        Department = entity.Department,
        SalesRep = entity.SalesRep,
        Currency = entity.Currency,
        ExchangeRate = entity.ExchangeRate,
        ProjectedTotal = entity.ProjectedTotal,
        Payment = entity.Payment,
        Prices = entity.Prices,
        Delivery = entity.Delivery,
        Notes = entity.Notes,
        SourceEnquiryId = entity.SourceEnquiryId,
        SourceEnquiryCode = entity.SourceEnquiryCode,
        Items = entity.Lines.OrderBy(x => x.LineNo).Select(l => new SalesLineItemDto(
            l.Category, l.Item, l.Description, l.Quantity, l.Uom, l.PriceLevel, l.Rate, l.Discount,
            l.Amount, l.TaxCode, l.GrossAmount, l.ClassName, l.CountryOfOrigin, l.HsCode)).ToList()
    };

    private static SalesOrderDto MapOrder(SalesOrder entity) => new()
    {
        Id = entity.Id,
        OrderCode = entity.OrderCode,
        Customer = entity.Customer,
        ContactPerson = entity.ContactPerson,
        Email = entity.Email,
        Phone = entity.Phone,
        Title = entity.Title,
        Status = entity.Status,
        DocumentDate = entity.DocumentDate,
        DeliveryDate = entity.DeliveryDate,
        Subsidiary = entity.Subsidiary,
        ClassName = entity.ClassName,
        Location = entity.Location,
        Department = entity.Department,
        SalesRep = entity.SalesRep,
        Currency = entity.Currency,
        ExchangeRate = entity.ExchangeRate,
        ProjectedTotal = entity.ProjectedTotal,
        Payment = entity.Payment,
        Prices = entity.Prices,
        Delivery = entity.Delivery,
        Notes = entity.Notes,
        SourceEnquiryId = entity.SourceEnquiryId,
        SourceEnquiryCode = entity.SourceEnquiryCode,
        SourceQuotationId = entity.SourceQuotationId,
        SourceQuotationCode = entity.SourceQuotationCode,
        Items = entity.Lines.OrderBy(x => x.LineNo).Select(l => new SalesLineItemDto(
            l.Category, l.Item, l.Description, l.Quantity, l.Uom, l.PriceLevel, l.Rate, l.Discount,
            l.Amount, l.TaxCode, l.GrossAmount, l.ClassName, l.CountryOfOrigin, l.HsCode)).ToList()
    };

    private static SalesInvoiceDto MapInvoice(SalesInvoice entity) => new()
    {
        Id = entity.Id,
        InvoiceCode = entity.InvoiceCode,
        Customer = entity.Customer,
        ContactPerson = entity.ContactPerson,
        Email = entity.Email,
        Phone = entity.Phone,
        Title = entity.Title,
        Status = entity.Status,
        DocumentDate = entity.DocumentDate,
        DueDate = entity.DueDate,
        Subsidiary = entity.Subsidiary,
        ClassName = entity.ClassName,
        Location = entity.Location,
        Department = entity.Department,
        SalesRep = entity.SalesRep,
        Currency = entity.Currency,
        ExchangeRate = entity.ExchangeRate,
        ProjectedTotal = entity.ProjectedTotal,
        Payment = entity.Payment,
        Notes = entity.Notes,
        SourceOrderId = entity.SourceOrderId,
        SourceOrderCode = entity.SourceOrderCode,
        SourceQuotationId = entity.SourceQuotationId,
        SourceQuotationCode = entity.SourceQuotationCode,
        Items = entity.Lines.OrderBy(x => x.LineNo).Select(l => new SalesLineItemDto(
            l.Category, l.Item, l.Description, l.Quantity, l.Uom, l.PriceLevel, l.Rate, l.Discount,
            l.Amount, l.TaxCode, l.GrossAmount, l.ClassName, l.CountryOfOrigin, l.HsCode)).ToList()
    };

    private static SalesAdjustmentNoteDto MapCreditNote(CreditNote entity) => new()
    {
        Id = entity.Id,
        NoteCode = entity.NoteCode,
        Customer = entity.Customer,
        ContactPerson = entity.ContactPerson,
        Email = entity.Email,
        Phone = entity.Phone,
        Title = entity.Title,
        Status = entity.Status,
        DocumentDate = entity.DocumentDate,
        DueDate = entity.DueDate,
        Reason = entity.Reason,
        Subsidiary = entity.Subsidiary,
        ClassName = entity.ClassName,
        Location = entity.Location,
        Department = entity.Department,
        SalesRep = entity.SalesRep,
        Currency = entity.Currency,
        ExchangeRate = entity.ExchangeRate,
        ProjectedTotal = entity.ProjectedTotal,
        Payment = entity.Payment,
        Notes = entity.Notes,
        SourceInvoiceId = entity.SourceInvoiceId,
        SourceInvoiceCode = entity.SourceInvoiceCode,
        Items = entity.Lines.OrderBy(x => x.LineNo).Select(l => new SalesLineItemDto(
            l.Category, l.Item, l.Description, l.Quantity, l.Uom, l.PriceLevel, l.Rate, l.Discount,
            l.Amount, l.TaxCode, l.GrossAmount, l.ClassName, l.CountryOfOrigin, l.HsCode)).ToList()
    };

    private static SalesAdjustmentNoteDto MapDebitNote(DebitNote entity) => new()
    {
        Id = entity.Id,
        NoteCode = entity.NoteCode,
        Customer = entity.Customer,
        ContactPerson = entity.ContactPerson,
        Email = entity.Email,
        Phone = entity.Phone,
        Title = entity.Title,
        Status = entity.Status,
        DocumentDate = entity.DocumentDate,
        DueDate = entity.DueDate,
        Reason = entity.Reason,
        Subsidiary = entity.Subsidiary,
        ClassName = entity.ClassName,
        Location = entity.Location,
        Department = entity.Department,
        SalesRep = entity.SalesRep,
        Currency = entity.Currency,
        ExchangeRate = entity.ExchangeRate,
        ProjectedTotal = entity.ProjectedTotal,
        Payment = entity.Payment,
        Notes = entity.Notes,
        SourceInvoiceId = entity.SourceInvoiceId,
        SourceInvoiceCode = entity.SourceInvoiceCode,
        Items = entity.Lines.OrderBy(x => x.LineNo).Select(l => new SalesLineItemDto(
            l.Category, l.Item, l.Description, l.Quantity, l.Uom, l.PriceLevel, l.Rate, l.Discount,
            l.Amount, l.TaxCode, l.GrossAmount, l.ClassName, l.CountryOfOrigin, l.HsCode)).ToList()
    };

    private static SalesLineItemDto MapLine(SalesEnquiryLine l) => new(
        l.Category, l.Item, l.Description, l.Quantity, l.Uom, l.PriceLevel, l.Rate, l.Discount,
        l.Amount, l.TaxCode, l.GrossAmount, l.ClassName, l.CountryOfOrigin, l.HsCode);

    private static string? Trim(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
