# Adding an Entity

1. Create domain class inheriting `AuditableEntity` in the owning module.
2. Add properties (`Guid` IDs, `decimal` money, `DateTimeOffset` timestamps).
3. Add `IEntityTypeConfiguration<T>`:
   - `ToTable("snake_name", "schema")`
   - tenant indexes / unique constraints (`tenant_id`, business code)
   - JSONB columns where intentional
4. Expose `DbSet<T>` on the module DbContext.
5. Add migration for that DbContext only.
6. Do not create foreign keys into another module's tables unless unavoidable; prefer Guid references + validation.
