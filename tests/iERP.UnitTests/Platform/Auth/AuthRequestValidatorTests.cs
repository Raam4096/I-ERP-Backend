using iERP.Modules.Platform.Identity.Application.Auth;
using iERP.UnitTests.Common;

namespace iERP.UnitTests.Platform.Auth;

public sealed class AuthRequestValidatorTests
{
    private readonly LoginRequestValidator _login = new();
    private readonly RefreshRequestValidator _refresh = new();

    [Theory]
    [InlineData("", "a@b.com", "secret", "TenantCode")]
    [InlineData("demo", "", "secret", "Email")]
    [InlineData("demo", "bad", "secret", "Email")]
    [InlineData("demo", "a@b.com", "", "Password")]
    public async Task Login_rejects_missing_or_invalid_fields(
        string tenant,
        string email,
        string password,
        string expectedProperty)
    {
        await _login.ShouldHaveValidationErrorForAsync(
            new LoginRequest(tenant, email, password),
            expectedProperty);
    }

    [Fact]
    public async Task Login_accepts_valid_request()
    {
        await _login.ShouldBeValidAsync(new LoginRequest("demo", "admin@ierp.local", "ChangeMe!123"));
    }

    [Fact]
    public async Task Refresh_rejects_empty_token()
    {
        await _refresh.ShouldHaveValidationErrorForAsync(new RefreshRequest(""), "RefreshToken");
    }
}
