using iERP.Application.Abstractions.AI;

namespace iERP.Infrastructure.AI;

public sealed class NullLLMProvider : ILLMProvider
{
    public Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default) =>
        Task.FromResult("AI provider is not configured.");
}
