namespace iERP.Application.Abstractions.AI;

public interface ILLMProvider
{
    Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default);
}
