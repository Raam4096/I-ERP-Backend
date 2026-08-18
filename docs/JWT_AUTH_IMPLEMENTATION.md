# JWT authentication — backend implementation

## Existing setup discovered

- Modular monolith (.NET 8) with Minimal APIs
- JWT **validation** already wired (`Smart` scheme → JwtBearer, Development header fallback)
- Custom Identity schema (`identity.users`, `identity.refresh_tokens`, roles/permissions)
- Auth routes existed as **501 stubs** under `/api/v1/auth/*`
- CRM Lead APIs already use `.RequireAuthorization()`
- Claims expected by runtime: `tenant_id`, `user_id`
- No ASP.NET Identity `UserManager` — custom `AppUser` + `PasswordHasher<AppUser>`

## Authentication architecture chosen

```text
POST /api/v1/auth/login
  → resolve Tenant by Code (platform.tenants)
  → set ITenantContext
  → load AppUser by email
  → PasswordHasher.VerifyHashedPassword
  → issue JWT access token + opaque refresh token
  → store SHA-256(refresh) in identity.refresh_tokens

POST /api/v1/auth/refresh
  → lookup refresh by hash (IgnoreQueryFilters)
  → rotate: revoke old, issue new pair

POST /api/v1/auth/logout
  → revoke refresh token hash
```

Access JWT claims: `sub`, `user_id`, `tenant_id`, `email`, `role`(s).

## Files created

| File | Purpose |
|------|---------|
| `Identity/Application/Auth/AuthDtos.cs` | Login/refresh/logout DTOs + token response |
| `Identity/Application/Auth/AuthValidators.cs` | FluentValidation |
| `Identity/Application/Auth/JwtTokenService.cs` | Access/refresh token create + hash |
| `Identity/Application/Auth/AuthService.cs` | Login/refresh/logout orchestration |
| `Identity/Application/Seeding/DevelopmentAuthSeeder.cs` | Dev tenant + admin user |
| `SharedKernel/Exceptions/UnauthorizedException.cs` | Maps to HTTP 401 |
| `Application.Abstractions/Options/AuthSeedOptions.cs` | Seed configuration |
| `docs/FRONTEND_AUTH_INTEGRATION.md` | UI handoff |
| `docs/JWT_AUTH_IMPLEMENTATION.md` | This file |

## Files modified

| File | Change |
|------|--------|
| `Identity/Api/AuthEndpoints.cs` | Real login/refresh/logout |
| `Platform/DependencyInjection.cs` | Register auth services + validators |
| `Platform.csproj` | FluentValidation DI extensions |
| `Infrastructure/DependencyInjection.cs` | Bind `AuthSeedOptions` |
| `Infrastructure/Exceptions/GlobalExceptionHandler.cs` | 401 mapping |
| `Application.Abstractions/Options/JwtOptions.cs` | Default access token 15 min |
| `iERP.Api/Program.cs` | Swagger Bearer; migrate Platform+Identity+CRM; run auth seeder |
| `iERP.Api/appsettings.json` | Jwt lifetimes + AuthSeed (disabled) |
| `iERP.Api/appsettings.Development.json` | AuthSeed enabled with demo password |

## NuGet packages added

- `FluentValidation.DependencyInjectionExtensions` on Platform project (central version already present)
- No new JWT packages (JwtBearer already referenced)

## Database changes

**No new migration.** Existing Identity migration already includes:

- `identity.users` (`password_hash`, etc.)
- `identity.refresh_tokens` (`token_hash`, `expires_at`, `revoked_at`, `replaced_by_token_hash`)

Startup now runs:

- `PlatformDbContext.Database.MigrateAsync()`
- `IdentityDbContext.Database.MigrateAsync()`
- `CrmDbContext.Database.MigrateAsync()`

Migration name already in repo: `20260811063953_InitialIdentity`.

## JWT configuration keys

```text
Jwt:Issuer
Jwt:Audience
Jwt:SigningKey          (≥ 32 chars; override via env / user-secrets)
Jwt:AccessTokenMinutes  (default 15)
Jwt:RefreshTokenDays    (default 14)

AuthSeed:Enabled
AuthSeed:TenantCode
AuthSeed:TenantName
AuthSeed:AdminEmail
AuthSeed:AdminPassword
AuthSeed:AdminDisplayName
AuthSeed:AdminUserName
```

### Required secrets / env (deployed)

```text
Jwt__SigningKey=<long-random-secret>
# optional seed on non-prod only:
AuthSeed__Enabled=true
AuthSeed__AdminPassword=<strong-password>
ASPNETCORE_ENVIRONMENT=Production   # disables Swagger + Development header auth
```

Local user-secrets example:

```bash
dotnet user-secrets set "Jwt:SigningKey" "your-32+-char-secret" --project src/iERP.Api
```

Do not commit production secrets.

## Login / refresh / logout

| Method | Path | Auth |
|--------|------|------|
| POST | `/api/v1/auth/login` | Anonymous |
| POST | `/api/v1/auth/refresh` | Anonymous |
| POST | `/api/v1/auth/logout` | Anonymous |

CRM leads remain `.RequireAuthorization()`.

## Authorization setup

- Default schemes: `Smart` (Bearer JWT, or Development headers when `IsDevelopment()`)
- No new role policies yet (roles are issued in JWT for future `[Authorize(Roles=...)]`)
- Health endpoints remain public

## Swagger

- Bearer security scheme added (`Authorize` button)
- UI enabled **only when `IsDevelopment()`**
- Production: Swagger off

## How to test locally

1. Ensure PostgreSQL is running and connection string points at `ierp_dev`
2. `dotnet run --project src/iERP.Api` (Development)
3. Seeder creates tenant `demo` + `admin@ierp.local` / `ChangeMe!123`
4. Open `http://localhost:5080/swagger`
5. `POST /api/v1/auth/login` with body above
6. Click **Authorize**, paste `accessToken`
7. Call `GET /api/crm/leads`
8. Exercise refresh + logout with returned `refreshToken`

## Assumptions / notes

- Login requires **`tenantCode`** because emails are unique per tenant
- Development header auth (`X-Tenant-Id` / `X-User-Id`) still works in Development when no Bearer is sent
- Prefer Bearer JWT for UI going forward
- AuthSeed password in `appsettings.Development.json` is for local demos only
- Creating production users beyond the seeder is out of scope (no admin user-management API yet)
