# Architecture Decisions

## ADR-001: One project per module (not per layer)

**Decision:** Each module is a single class library with `Domain/Application/Infrastructure/Api` folders.

**Why:** Avoids dozens/hundreds of csproj files while preserving logical boundaries. Architecture tests still enforce key dependency rules.

## ADR-002: Customer master lives in CRM

**Decision:** `Customer`, `Contact`, and `Address` are owned by the CRM schema/module.

**Why:** CRM is the operational master for party data shared with Sales. Sales stores `CustomerId` only and must not write CRM tables.

## ADR-003: Grouped platform DbContexts

**Decision:** Platform uses `PlatformDbContext`, `IdentityDbContext`, `OrganizationDbContext`, `MetadataDbContext` rather than one context per tiny platform concern.

**Why:** Reduces migration/ops overhead while keeping schemas separated (`platform`, `identity`, `organization`, `metadata`, `audit`, ...).

## ADR-004: Namespace-filtered EF configurations

**Decision:** Shared module assemblies apply `IEntityTypeConfiguration` via namespace prefix, not whole-assembly scan.

**Why:** Multiple DbContexts share Platform/Engines assemblies; whole-assembly `ApplyConfigurationsFromAssembly` would cross-contaminate models.

## ADR-005: Null/dev Azure implementations

**Decision:** API starts with null providers for Service Bus, Blob, OpenAI, and optional Redis/Hangfire.

**Why:** Local development must not require Azure credentials.

## ADR-006: MediatR adopted for module CQRS (CRM first)

**Decision:** CRM Lead Management uses MediatR + FluentValidation pipeline behaviors + AutoMapper 16.

**Why:** Explicit product requirement for CQRS handlers. Scoped to module DI; other modules can adopt gradually. AutoMapper 16 used due to vulnerability advisory on 13.x/14.x.

## ADR-007: AI never touches the database

**Decision:** AI module persists only AI metadata/logs; tool execution must call ERP application services.

**Why:** Security, tenancy, permissions, and auditability.

## ADR-008: Avoid property name `Version` on domain entities

**Decision:** BOM uses `BomVersion`; print templates use `TemplateVersionNumber`.

**Why:** `AuditableEntity.Version` is the concurrency token; reusing `Version` caused CS0108 under TreatWarningsAsErrors.

## ADR-009: Development authentication fallback

**Decision:** In Development, when no Bearer token is present, authenticate via `Development` scheme using `X-Tenant-Id` / `X-User-Id` headers (with defaults).

**Why:** Lead APIs require `Authorize` before login endpoints exist; enables local Swagger testing safely outside Production.
