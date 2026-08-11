using iERP.Application.Abstractions.AI;

namespace iERP.Modules.AI.Application;

public sealed class AIToolRegistry : IAIToolRegistry
{
    private readonly Dictionary<string, IAITool> _tools = new(StringComparer.OrdinalIgnoreCase);

    public IAITool? Resolve(string toolName) =>
        _tools.TryGetValue(toolName, out var tool) ? tool : null;

    public IReadOnlyCollection<IAITool> GetAll() => _tools.Values.ToList();

    public void Register(IAITool tool) => _tools[tool.Name] = tool;
}
