using iERP.Infrastructure.Persistence;
using iERP.Modules.Banking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Banking.Infrastructure.Configurations;

public sealed class PaymentVoucherLineConfiguration : AuditableEntityConfiguration<PaymentVoucherLine>
{
    public override void Configure(EntityTypeBuilder<PaymentVoucherLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("payment_voucher_lines", "banking");

    }
}
