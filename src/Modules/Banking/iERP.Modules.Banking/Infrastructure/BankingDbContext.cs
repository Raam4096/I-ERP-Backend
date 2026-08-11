using iERP.Infrastructure.Persistence;
using iERP.Modules.Banking.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Banking.Infrastructure;

public sealed class BankingDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public BankingDbContext(DbContextOptions<BankingDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<PaymentVoucher> PaymentVouchers => Set<PaymentVoucher>();
    public DbSet<PaymentVoucherLine> PaymentVoucherLines => Set<PaymentVoucherLine>();
    public DbSet<ReceiptVoucher> ReceiptVouchers => Set<ReceiptVoucher>();
    public DbSet<ReceiptVoucherLine> ReceiptVoucherLines => Set<ReceiptVoucherLine>();
    public DbSet<BankReconciliation> BankReconciliations => Set<BankReconciliation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("banking");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
