using iERP.Infrastructure.Persistence;
using iERP.Modules.Banking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Banking.Infrastructure.Configurations;

public sealed class PaymentMethodConfiguration : AuditableEntityConfiguration<PaymentMethod>
{
    public override void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        base.Configure(builder);
        builder.ToTable("payment_methods", "banking");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
