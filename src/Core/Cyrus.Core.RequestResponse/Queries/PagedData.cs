namespace Cyrus.Core.RequestResponse.Queries;

/// <summary>
/// Container for paged query data with basic paging metadata.
/// </summary>
/// <typeparam name="T">Type of items contained in the page.</typeparam>
public class PagedData<T>
{
    /// <summary>Items returned for the current page.</summary>
    public List<T> QueryResult { get; set; } = new();

    /// <summary>1-based page number.</summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>Items per page.</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>Total number of items available across all pages, when computed.</summary>
    public int TotalCount { get; init; }

    /// <summary>Total number of pages, derived from <see cref="TotalCount"/> and <see cref="PageSize"/>.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>Convenience factory.</summary>
    public static PagedData<T> Create(IEnumerable<T> items, int pageNumber, int pageSize, int totalCount)
        => new()
        {
            QueryResult = items?.ToList() ?? new List<T>(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
}
