namespace iERP.Modules.Platform.Identity.Application.Auth;

public sealed record LoginRequest(
    string TenantCode,
    string Email,
    string Password);

public sealed record RefreshRequest(string RefreshToken);

public sealed record LogoutRequest(string RefreshToken);

public sealed record AuthUserDto(
    Guid Id,
    Guid TenantId,
    string Email,
    string UserName,
    string? DisplayName,
    IReadOnlyList<string> Roles);

public sealed record AuthTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    string TokenType,
    AuthUserDto User);
