using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class SalesEnquiryConfiguration : AuditableEntityConfiguration<SalesEnquiry>
{
    public override void Configure(EntityTypeBuilder<SalesEnquiry> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_enquiries", "sales");
        builder.HasIndex(x => new { x.TenantId, x.EnquiryCode }).IsUnique();
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesEnquiryId);
        builder.HasMany(x => x.FollowUps).WithOne().HasForeignKey(x => x.SalesEnquiryId);
        builder.Navigation(x => x.Lines).HasField("_lines");
        builder.Navigation(x => x.FollowUps).HasField("_followUps");
    }
}

public sealed class SalesEnquiryLineConfiguration : AuditableEntityConfiguration<SalesEnquiryLine>
{
    public override void Configure(EntityTypeBuilder<SalesEnquiryLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_enquiry_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SalesEnquiryId, x.LineNo });
    }
}

public sealed class SalesEnquiryFollowUpConfiguration : AuditableEntityConfiguration<SalesEnquiryFollowUp>
{
    public override void Configure(EntityTypeBuilder<SalesEnquiryFollowUp> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_enquiry_follow_ups", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SalesEnquiryId });
    }
}

public sealed class SalesQuotationConfiguration : AuditableEntityConfiguration<SalesQuotation>
{
    public override void Configure(EntityTypeBuilder<SalesQuotation> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_quotations", "sales");
        builder.HasIndex(x => new { x.TenantId, x.QuotationCode }).IsUnique();
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesQuotationId);
        builder.Navigation(x => x.Lines).HasField("_lines");
    }
}

public sealed class SalesQuotationLineConfiguration : AuditableEntityConfiguration<SalesQuotationLine>
{
    public override void Configure(EntityTypeBuilder<SalesQuotationLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_quotation_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SalesQuotationId, x.LineNo });
    }
}

public sealed class SalesInvoiceConfiguration : AuditableEntityConfiguration<SalesInvoice>
{
    public override void Configure(EntityTypeBuilder<SalesInvoice> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_invoices", "sales");
        builder.HasIndex(x => new { x.TenantId, x.InvoiceCode }).IsUnique();
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesInvoiceId);
        builder.Navigation(x => x.Lines).HasField("_lines");
    }
}

public sealed class SalesInvoiceLineConfiguration : AuditableEntityConfiguration<SalesInvoiceLine>
{
    public override void Configure(EntityTypeBuilder<SalesInvoiceLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_invoice_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SalesInvoiceId, x.LineNo });
    }
}
