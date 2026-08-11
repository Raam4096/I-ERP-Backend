namespace iERP.SharedKernel.Time;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
