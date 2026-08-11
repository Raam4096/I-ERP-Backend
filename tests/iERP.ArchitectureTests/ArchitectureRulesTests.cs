using FluentAssertions;
using iERP.Modules.CRM.Domain;
using NetArchTest.Rules;
using Xunit;

namespace iERP.ArchitectureTests;

public sealed class ArchitectureRulesTests
{
    [Fact]
    public void SharedKernel_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InAssembly(typeof(iERP.SharedKernel.Primitives.Entity).Assembly)
            .ShouldNot()
            .HaveDependencyOn("iERP.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationAbstractions_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InAssembly(typeof(iERP.Application.Abstractions.Caching.ICacheService).Assembly)
            .ShouldNot()
            .HaveDependencyOn("iERP.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CrmDomain_Should_Not_Depend_On_SalesInfrastructure()
    {
        var result = Types.InAssembly(typeof(Lead).Assembly)
            .That()
            .ResideInNamespace("iERP.Modules.CRM.Domain")
            .ShouldNot()
            .HaveDependencyOn("iERP.Modules.Sales.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
