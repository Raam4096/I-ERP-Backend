namespace iERP.SharedKernel.Security;

public static class SystemRoles
{
    public const string SuperAdmin = "Super Admin";
    public const string TenantAdmin = "Tenant Admin";
    public const string FinanceManager = "Finance Manager";
    public const string FinanceExecutive = "Finance Executive";
    public const string SalesManager = "Sales Manager";
    public const string SalesExecutive = "Sales Executive";
    public const string PurchaseManager = "Purchase Manager";
    public const string PurchaseExecutive = "Purchase Executive";
    public const string WarehouseStaff = "Warehouse Staff";
    public const string ReadOnly = "Read Only";

    public static IReadOnlyList<string> All { get; } =
    [
        SuperAdmin,
        TenantAdmin,
        FinanceManager,
        FinanceExecutive,
        SalesManager,
        SalesExecutive,
        PurchaseManager,
        PurchaseExecutive,
        WarehouseStaff,
        ReadOnly
    ];
}
