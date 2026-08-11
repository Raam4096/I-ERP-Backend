# Development Guide

## Local stack

```bash
docker compose up -d
dotnet restore
dotnet build
dotnet run --project src/iERP.Api
```

Swagger: http://localhost:5080/swagger

## Configuration rules

- Prefer `IOptions<T>` over direct `IConfiguration` reads in application services.
- Keep Azure dependencies disabled locally (`Enabled: false`).
- Never commit secrets; use `.env.example` as a template.

## Coding conventions

- Nullable reference types enabled
- Warnings as errors
- Async + `CancellationToken` on application interfaces
- Pagination default 20 / max 100
- No generic repository / UnitOfWork wrappers around EF Core
- Money uses `decimal`, never `float`/`double`

## Implementing the next API

1. Add command/query + handler in module Application
2. Add FluentValidation validator if needed
3. Map endpoint in module `Api/*Endpoints.cs`
4. Keep business rules in Domain/Application; Infrastructure only persists/integrates

See also: [adding-an-api.md](adding-an-api.md), [adding-an-entity.md](adding-an-entity.md).
