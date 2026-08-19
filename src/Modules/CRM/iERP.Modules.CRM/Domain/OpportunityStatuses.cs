namespace iERP.Modules.CRM.Domain;

public static class OpportunityStatuses
{
    public const string New = "New";
    public const string InProgress = "InProgress";
    public const string Won = "Won";
    public const string Lost = "Lost";
    public const string Discarded = "Discarded";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        New, InProgress, Won, Lost, Discarded
    };

    public static bool IsClosed(string status) =>
        status.Equals(Won, StringComparison.OrdinalIgnoreCase) ||
        status.Equals(Lost, StringComparison.OrdinalIgnoreCase);
}
