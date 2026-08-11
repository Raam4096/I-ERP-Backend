using iERP.Infrastructure.Persistence;
using iERP.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Projects.Infrastructure.Configurations;

public sealed class ProjectConfiguration : AuditableEntityConfiguration<Project>
{
    public override void Configure(EntityTypeBuilder<Project> builder)
    {
        base.Configure(builder);
        builder.ToTable("projects", "projects");
        builder.HasIndex(x => new { x.TenantId, x.ProjectCode }).IsUnique();
    }
}
