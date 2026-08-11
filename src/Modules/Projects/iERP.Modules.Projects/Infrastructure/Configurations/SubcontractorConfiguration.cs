using iERP.Infrastructure.Persistence;
using iERP.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Projects.Infrastructure.Configurations;

public sealed class SubcontractorConfiguration : AuditableEntityConfiguration<Subcontractor>
{
    public override void Configure(EntityTypeBuilder<Subcontractor> builder)
    {
        base.Configure(builder);
        builder.ToTable("subcontractors", "projects");

    }
}
