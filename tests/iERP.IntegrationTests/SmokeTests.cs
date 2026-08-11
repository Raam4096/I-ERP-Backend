using FluentAssertions;
using Xunit;

namespace iERP.IntegrationTests;

/// <summary>
/// Integration test skeleton. Full WebApplicationFactory tests can be added when a test database is available.
/// These tests intentionally do not require Docker during a simple build/test run.
/// </summary>
public sealed class SmokeTests
{
    [Fact]
    public void IntegrationTestProject_IsWired()
    {
        typeof(Program).Assembly.GetName().Name.Should().Be("iERP.Api");
    }
}
