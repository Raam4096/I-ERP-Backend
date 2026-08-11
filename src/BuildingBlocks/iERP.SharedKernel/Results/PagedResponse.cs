namespace iERP.SharedKernel.Results;

public sealed record PagedResponse<T>(
    bool Success,
    IReadOnlyList<T> Data,
    PaginationMetadata Pagination,
    string? Message = null)
{
    public static PagedResponse<T> Create(
        IReadOnlyList<T> data,
        int page,
        int pageSize,
        long totalCount,
        string? message = null)
    {
        var totalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        return new(
            true,
            data,
            new PaginationMetadata(page, pageSize, totalCount, totalPages),
            message);
    }
}
