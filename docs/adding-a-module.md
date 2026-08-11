# Adding a Module

1. Create `src/Modules/<Name>/iERP.Modules.<Name>/` with folders:
   - `Domain/`
   - `Application/`
   - `Infrastructure/` (+ `Configurations/`)
   - `Api/`
2. Add project to `iERP.sln` and reference SharedKernel, Application.Abstractions, Infrastructure.
3. Create entities inheriting `AuditableEntity` (or platform exception rules).
4. Create `IEntityTypeConfiguration<T>` and a module `DbContext` with schema.
5. Register DbContext + services in `DependencyInjection.AddXxxModule`.
6. Map health/endpoints and call from `iERP.Api` `Program.cs`.
7. Add design-time factory in `iERP.Migrations`.
8. Document ownership in `module-boundaries.md` and schema in `database-architecture.md`.
