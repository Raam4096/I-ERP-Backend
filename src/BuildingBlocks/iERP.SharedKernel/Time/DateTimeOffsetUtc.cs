namespace iERP.SharedKernel.Time;

public static class DateTimeOffsetUtc
{
    /// <summary>
    /// Npgsql requires offset 0 when writing to PostgreSQL <c>timestamptz</c>.
    /// </summary>
    public static DateTimeOffset Normalize(DateTimeOffset value) => value.ToUniversalTime();

    public static DateTimeOffset? Normalize(DateTimeOffset? value) =>
        value?.ToUniversalTime();
}
