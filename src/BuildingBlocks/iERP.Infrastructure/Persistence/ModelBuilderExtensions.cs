using System.Reflection;
using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace iERP.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies IEntityTypeConfiguration implementations whose namespace starts with the given prefix.
    /// Required when multiple DbContexts share one module assembly.
    /// </summary>
    public static void ApplyConfigurationsFromNamespace(
        this ModelBuilder modelBuilder,
        Assembly assembly,
        string namespacePrefix)
    {
        var applyMethod = typeof(ModelBuilder)
            .GetMethods()
            .Single(m => m.Name == nameof(ModelBuilder.ApplyConfiguration)
                         && m.GetParameters().Length == 1
                         && m.IsGenericMethodDefinition);

        var configurationTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: not null }
                        && t.Namespace.StartsWith(namespacePrefix, StringComparison.Ordinal));

        foreach (var type in configurationTypes)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType || iface.GetGenericTypeDefinition() != typeof(IEntityTypeConfiguration<>))
                {
                    continue;
                }

                var entityType = iface.GetGenericArguments()[0];
                var configuration = Activator.CreateInstance(type)!;
                applyMethod.MakeGenericMethod(entityType).Invoke(modelBuilder, [configuration]);
            }
        }
    }

    public static void ApplySnakeCaseNamingConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                entity.SetTableName(SnakeCaseNameConverter.ToSnakeCase(tableName));
            }

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(SnakeCaseNameConverter.ToSnakeCase(property.Name));
            }

            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrWhiteSpace(keyName))
                {
                    key.SetName(SnakeCaseNameConverter.ToSnakeCase(keyName));
                }
            }

            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrWhiteSpace(indexName))
                {
                    index.SetDatabaseName(SnakeCaseNameConverter.ToSnakeCase(indexName));
                }
            }

            foreach (var fk in entity.GetForeignKeys())
            {
                var fkName = fk.GetConstraintName();
                if (!string.IsNullOrWhiteSpace(fkName))
                {
                    fk.SetConstraintName(SnakeCaseNameConverter.ToSnakeCase(fkName));
                }
            }
        }
    }

    public static void ApplyTenantAndSoftDeleteFilters(this ModelBuilder modelBuilder, ITenantContext tenantContext)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!typeof(ITenantEntity).IsAssignableFrom(clrType) || !typeof(ISoftDeletable).IsAssignableFrom(clrType))
            {
                continue;
            }

            var method = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(SetTenantSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(clrType);

            method.Invoke(null, [modelBuilder, tenantContext]);
        }
    }

    private static void SetTenantSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder, ITenantContext tenantContext)
        where TEntity : class, ITenantEntity, ISoftDeletable
    {
        // Use TenantEfFilter (AsyncLocal) so EF re-evaluates the current tenant per query.
        // Capturing the scoped ITenantContext instance in HasQueryFilter is unsafe.
        _ = tenantContext;
        modelBuilder.Entity<TEntity>().HasQueryFilter(e =>
            !e.IsDeleted &&
            TenantEfFilter.TenantId != null &&
            e.TenantId == TenantEfFilter.TenantId);
    }

    public static void ConfigureMoneyPrecision(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(decimal) && property.ClrType != typeof(decimal?))
                {
                    continue;
                }

                var name = property.Name;
                if (name.Contains("ExchangeRate", StringComparison.OrdinalIgnoreCase))
                {
                    property.SetPrecision(19);
                    property.SetScale(8);
                }
                else if (name.Contains("Quantity", StringComparison.OrdinalIgnoreCase) ||
                         name.Contains("Qty", StringComparison.OrdinalIgnoreCase))
                {
                    property.SetPrecision(19);
                    property.SetScale(6);
                }
                else
                {
                    property.SetPrecision(19);
                    property.SetScale(4);
                }
            }
        }
    }
}
