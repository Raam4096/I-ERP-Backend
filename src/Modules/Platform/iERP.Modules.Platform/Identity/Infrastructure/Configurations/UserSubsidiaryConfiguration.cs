using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Identity.Infrastructure.Configurations;

public sealed class UserSubsidiaryConfiguration : AuditableEntityConfiguration<UserSubsidiary>
{
    public override void Configure(EntityTypeBuilder<UserSubsidiary> builder)
    {
        base.Configure(builder);
        builder.ToTable("user_subsidiaries", "identity");
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.SubsidiaryId }).IsUnique();
    }
}
