namespace iERP.Application.Abstractions.Jobs;

public interface IBackgroundJobService
{
    string Enqueue(ExpressionJob job);
    string Schedule(ExpressionJob job, TimeSpan delay);
}

/// <summary>
/// Lightweight job descriptor placeholder until Hangfire expressions are wired per module.
/// </summary>
public sealed record ExpressionJob(string JobName, IDictionary<string, string>? Parameters = null);
