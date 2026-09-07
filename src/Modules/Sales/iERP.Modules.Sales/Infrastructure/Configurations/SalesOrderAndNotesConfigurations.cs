using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class SalesOrderConfiguration : AuditableEntityConfiguration<SalesOrder>
{
    public override void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_orders", "sales");
        builder.HasIndex(x => new { x.TenantId, x.OrderCode }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.SourceQuotationId });
        builder.HasIndex(x => new { x.TenantId, x.SourceEnquiryId });
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesOrderId);
        builder.Navigation(x => x.Lines).HasField("_lines");
    }
}

public sealed class SalesOrderLineConfiguration : AuditableEntityConfiguration<SalesOrderLine>
{
    public override void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_order_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SalesOrderId, x.LineNo });
    }
}

public sealed class CreditNoteConfiguration : AuditableEntityConfiguration<CreditNote>
{
    public override void Configure(EntityTypeBuilder<CreditNote> builder)
    {
        base.Configure(builder);
        builder.ToTable("credit_notes", "sales");
        builder.HasIndex(x => new { x.TenantId, x.NoteCode }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.SourceInvoiceId });
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.CreditNoteId);
        builder.Navigation(x => x.Lines).HasField("_lines");
    }
}

public sealed class CreditNoteLineConfiguration : AuditableEntityConfiguration<CreditNoteLine>
{
    public override void Configure(EntityTypeBuilder<CreditNoteLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("credit_note_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.CreditNoteId, x.LineNo });
    }
}

public sealed class DebitNoteConfiguration : AuditableEntityConfiguration<DebitNote>
{
    public override void Configure(EntityTypeBuilder<DebitNote> builder)
    {
        base.Configure(builder);
        builder.ToTable("debit_notes", "sales");
        builder.HasIndex(x => new { x.TenantId, x.NoteCode }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.SourceInvoiceId });
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.DebitNoteId);
        builder.Navigation(x => x.Lines).HasField("_lines");
    }
}

public sealed class DebitNoteLineConfiguration : AuditableEntityConfiguration<DebitNoteLine>
{
    public override void Configure(EntityTypeBuilder<DebitNoteLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("debit_note_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.DebitNoteId, x.LineNo });
    }
}
