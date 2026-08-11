using iERP.Infrastructure.Persistence;
using iERP.Modules.Banking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Banking.Infrastructure.Configurations;

public sealed class ReceiptVoucherLineConfiguration : AuditableEntityConfiguration<ReceiptVoucherLine>
{
    public override void Configure(EntityTypeBuilder<ReceiptVoucherLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("receipt_voucher_lines", "banking");

    }
}
