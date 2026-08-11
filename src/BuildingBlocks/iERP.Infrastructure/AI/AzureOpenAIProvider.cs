using iERP.Application.Abstractions.AI;
using iERP.Application.Abstractions.Options;
using Microsoft.Extensions.Options;

namespace iERP.Infrastructure.AI;

/// <summary>
/// Placeholder Azure OpenAI provider. Real Semantic Kernel wiring lives in the AI module.
/// </summary>
public sealed class AzureOpenAIProvider : ILLMProvider
{
    private readonly AzureOpenAIOptions _options;

    public AzureOpenAIProvider(IOptions<AzureOpenAIOptions> options)
    {
        _options = options.Value;
    }

    public Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.Endpoint))
        {
            return Task.FromResult("Azure OpenAI is not enabled.");
        }

        // Intentionally not calling Azure here so local startup never requires credentials.
        return Task.FromResult($"[AzureOpenAI:{_options.DeploymentName}] prompt received ({prompt.Length} chars)");
    }
}
