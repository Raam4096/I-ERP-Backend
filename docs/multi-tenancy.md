# Multi-Tenancy

## Strategy

Shared-database, shared-schema-per-module, **tenant_id per row**.

- Tenant IDs are UUID/`Guid`.
- `Tenant` (SaaS customer) is a platform root table and has **no** `tenant_id`.
- All other operational entities implement `ITenantEntity`.

## Runtime resolution

1. JWT claim `tenant_id` (also accepts `tenantId`)
2. `ClaimTenantResolver` → `ITenantContext`
3. `TenantResolutionMiddleware` sets/clears tenant per request

## EF Core protections

- Global query filters: `tenant_id == current tenant` AND `is_deleted == false`
- `TenantSaveChangesInterceptor`:
  - stamps `TenantId` on insert when empty
  - rejects mismatched tenant
  - prevents `TenantId` mutation after insert
  - stamps audit timestamps via `IClock` (UTC)

## Future RLS

Architecture leaves room for PostgreSQL Row Level Security policies keyed by session `app.tenant_id`. Application filters remain the primary defense today.
