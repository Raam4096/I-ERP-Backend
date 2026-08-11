using iERP.Infrastructure.Persistence;
using iERP.Modules.Banking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Banking.Infrastructure.Configurations;

public sealed class ReceiptVoucherConfiguration : AuditableEntityConfiguration<ReceiptVoucher>
{
    public override void Configure(EntityTypeBuilder<ReceiptVoucher> builder)
    {
        base.Configure(builder);
        builder.ToTable("receipt_vouchers", "banking");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
