namespace iERP.Application.Abstractions.Options;

/// <summary>
/// Optional bootstrap user for local/dev environments. Never enable with a weak password in production.
/// </summary>
public sealed class AuthSeedOptions
{
    public const string SectionName = "AuthSeed";

    public bool Enabled { get; set; }
    public string TenantCode { get; set; } = "demo";
    public string TenantName { get; set; } = "Demo Tenant";
    public string AdminEmail { get; set; } = "admin@ierp.local";
    public string AdminPassword { get; set; } = string.Empty;
    public string AdminDisplayName { get; set; } = "Demo Admin";
    public string AdminUserName { get; set; } = "admin";
}
