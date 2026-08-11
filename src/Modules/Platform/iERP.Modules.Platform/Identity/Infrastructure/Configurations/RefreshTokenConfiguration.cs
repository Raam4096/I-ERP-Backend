using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Identity.Infrastructure.Configurations;

public sealed class RefreshTokenConfiguration : AuditableEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);
        builder.ToTable("refresh_tokens", "identity");
        builder.HasIndex(x => new { x.TenantId, x.TokenHash }).IsUnique();
    }
}
