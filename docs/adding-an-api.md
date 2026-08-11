# Adding an API

Prefer Minimal API groups per module.

```csharp
public static class LeadEndpoints
{
    public static IEndpointRouteBuilder MapLeadEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/leads").WithTags("Leads");
        group.MapGet("/", async (/* handler deps */, CancellationToken ct) => { /* ... */ });
        return app;
    }
}
```

## Conventions

- Prefix: `/api/v1/`
- Return `ApiResponse<T>` / `PagedResponse<T>` / `ApiErrorResponse`
- Use permission checks (`crm.lead.read`, etc.)
- Do not put business logic in endpoint methods beyond orchestration
- Register mapping in module endpoint aggregator and `Program.cs`
