using FluentAssertions;
using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Results;
using Xunit;

namespace iERP.UnitTests.SharedKernel;

public sealed class EntityTests
{
    [Fact]
    public void AuditableEntity_SoftDelete_SetsFlags()
    {
        var entity = new SampleEntity(Guid.NewGuid());
        var deletedAt = DateTimeOffset.UtcNow;
        var deletedBy = Guid.NewGuid();

        entity.SoftDelete(deletedBy, deletedAt);

        entity.IsDeleted.Should().BeTrue();
        entity.DeletedBy.Should().Be(deletedBy);
        entity.DeletedAt.Should().Be(deletedAt);
    }

    [Fact]
    public void PaginationDefaults_AreSensible()
    {
        PaginationDefaults.DefaultPageSize.Should().Be(20);
        PaginationDefaults.MaxPageSize.Should().Be(100);
    }

    private sealed class SampleEntity : AuditableEntity
    {
        public SampleEntity(Guid tenantId) : base(tenantId)
        {
        }
    }
}
