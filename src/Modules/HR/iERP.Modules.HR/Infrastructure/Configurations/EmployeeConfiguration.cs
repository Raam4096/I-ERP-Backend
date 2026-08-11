using iERP.Infrastructure.Persistence;
using iERP.Modules.HR.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.HR.Infrastructure.Configurations;

public sealed class EmployeeConfiguration : AuditableEntityConfiguration<Employee>
{
    public override void Configure(EntityTypeBuilder<Employee> builder)
    {
        base.Configure(builder);
        builder.ToTable("employees", "hr");
        builder.HasIndex(x => new { x.TenantId, x.EmployeeCode }).IsUnique();
    }
}
