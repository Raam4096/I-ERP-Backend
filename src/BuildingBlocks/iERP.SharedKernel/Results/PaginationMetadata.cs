namespace iERP.SharedKernel.Results;

public sealed record PaginationMetadata(
    int Page,
    int PageSize,
    long TotalCount,
    int TotalPages)
{
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
