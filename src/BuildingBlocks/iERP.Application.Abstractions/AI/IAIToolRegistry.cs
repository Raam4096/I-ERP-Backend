namespace iERP.Application.Abstractions.AI;

public interface IAIToolRegistry
{
    IAITool? Resolve(string toolName);
    IReadOnlyCollection<IAITool> GetAll();
    void Register(IAITool tool);
}
