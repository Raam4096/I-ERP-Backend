using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class CreditNoteLineConfiguration : AuditableEntityConfiguration<CreditNoteLine>
{
    public override void Configure(EntityTypeBuilder<CreditNoteLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("credit_note_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.Id });
    }
}
