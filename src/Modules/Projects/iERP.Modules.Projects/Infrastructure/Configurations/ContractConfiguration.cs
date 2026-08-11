using iERP.Infrastructure.Persistence;
using iERP.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Projects.Infrastructure.Configurations;

public sealed class ContractConfiguration : AuditableEntityConfiguration<Contract>
{
    public override void Configure(EntityTypeBuilder<Contract> builder)
    {
        base.Configure(builder);
        builder.ToTable("contracts", "projects");
        builder.HasIndex(x => new { x.TenantId, x.ContractNo }).IsUnique();
    }
}
