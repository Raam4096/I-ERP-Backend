using iERP.Modules.Sales.Application;
using iERP.Modules.Sales.Application.Dtos;
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Sales.Api;

public static class SalesEndpoints
{
    public static IEndpointRouteBuilder MapSalesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/health", () => Results.Ok(ApiResponse<string>.Ok("Sales module ready")))
            .WithName("SalesHealth")
            .WithTags("Sales")
            .AllowAnonymous();

        // Keep legacy health path used by old metadata stubs.
        app.MapGet("/api/v1/sales_quotations/health", () => Results.Ok(ApiResponse<string>.Ok("Sales module ready")))
            .WithName("SalesQuotationsHealthLegacy")
            .WithTags("Sales")
            .AllowAnonymous();

        MapEnquiries(app);
        MapQuotations(app);
        MapInvoices(app);
        return app;
    }

    private static void MapEnquiries(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/sales/enquiries")
            .WithTags("Sales Enquiries")
            .RequireAuthorization();

        group.MapGet("/", async (ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<IReadOnlyList<SalesEnquiryDto>>.Ok(await svc.ListEnquiriesAsync(ct))));

        group.MapGet("/example", async (ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<object>.Ok(await svc.GetEnquiryExampleAsync(ct))));

        group.MapGet("/{id:guid}", async (Guid id, ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<SalesEnquiryDto>.Ok(await svc.GetEnquiryAsync(id, ct))));

        group.MapPost("/", async ([FromBody] UpsertSalesEnquiryRequest request, ISalesDocumentsService svc, CancellationToken ct) =>
        {
            var created = await svc.CreateEnquiryAsync(request, ct);
            return Results.Created($"/api/v1/sales/enquiries/{created.Id}", ApiResponse<SalesEnquiryDto>.Ok(created, "Enquiry created."));
        });

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpsertSalesEnquiryRequest request, ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<SalesEnquiryDto>.Ok(await svc.UpdateEnquiryAsync(id, request, ct), "Enquiry updated.")));

        group.MapDelete("/{id:guid}", async (Guid id, ISalesDocumentsService svc, CancellationToken ct) =>
        {
            await svc.DeleteEnquiryAsync(id, ct);
            return Results.NoContent();
        });
    }

    private static void MapQuotations(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/sales/quotations")
            .WithTags("Sales Quotations")
            .RequireAuthorization();

        group.MapGet("/", async (ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<IReadOnlyList<SalesQuotationDto>>.Ok(await svc.ListQuotationsAsync(ct))));

        group.MapGet("/{id:guid}", async (Guid id, ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<SalesQuotationDto>.Ok(await svc.GetQuotationAsync(id, ct))));

        group.MapPost("/", async ([FromBody] UpsertSalesQuotationRequest request, ISalesDocumentsService svc, CancellationToken ct) =>
        {
            var created = await svc.CreateQuotationAsync(request, ct);
            return Results.Created($"/api/v1/sales/quotations/{created.Id}", ApiResponse<SalesQuotationDto>.Ok(created, "Quotation created."));
        });

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpsertSalesQuotationRequest request, ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<SalesQuotationDto>.Ok(await svc.UpdateQuotationAsync(id, request, ct), "Quotation updated.")));

        group.MapDelete("/{id:guid}", async (Guid id, ISalesDocumentsService svc, CancellationToken ct) =>
        {
            await svc.DeleteQuotationAsync(id, ct);
            return Results.NoContent();
        });
    }

    private static void MapInvoices(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/sales/invoices")
            .WithTags("Sales Invoices")
            .RequireAuthorization();

        group.MapGet("/", async (ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<IReadOnlyList<SalesInvoiceDto>>.Ok(await svc.ListInvoicesAsync(ct))));

        group.MapGet("/{id:guid}", async (Guid id, ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<SalesInvoiceDto>.Ok(await svc.GetInvoiceAsync(id, ct))));

        group.MapPost("/", async ([FromBody] UpsertSalesInvoiceRequest request, ISalesDocumentsService svc, CancellationToken ct) =>
        {
            var created = await svc.CreateInvoiceAsync(request, ct);
            return Results.Created($"/api/v1/sales/invoices/{created.Id}", ApiResponse<SalesInvoiceDto>.Ok(created, "Invoice created."));
        });

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpsertSalesInvoiceRequest request, ISalesDocumentsService svc, CancellationToken ct) =>
            Results.Ok(ApiResponse<SalesInvoiceDto>.Ok(await svc.UpdateInvoiceAsync(id, request, ct), "Invoice updated.")));

        group.MapDelete("/{id:guid}", async (Guid id, ISalesDocumentsService svc, CancellationToken ct) =>
        {
            await svc.DeleteInvoiceAsync(id, ct);
            return Results.NoContent();
        });
    }
}
