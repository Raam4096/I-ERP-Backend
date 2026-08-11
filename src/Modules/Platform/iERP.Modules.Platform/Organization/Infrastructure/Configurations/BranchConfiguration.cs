using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Organization.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Organization.Infrastructure.Configurations;

public sealed class BranchConfiguration : AuditableEntityConfiguration<Branch>
{
    public override void Configure(EntityTypeBuilder<Branch> builder)
    {
        base.Configure(builder);
        builder.ToTable("branches", "organization");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Code }).IsUnique();
    }
}
