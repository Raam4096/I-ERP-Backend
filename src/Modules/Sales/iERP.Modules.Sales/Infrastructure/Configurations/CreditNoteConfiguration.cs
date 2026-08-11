using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class CreditNoteConfiguration : AuditableEntityConfiguration<CreditNote>
{
    public override void Configure(EntityTypeBuilder<CreditNote> builder)
    {
        base.Configure(builder);
        builder.ToTable("credit_notes", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
