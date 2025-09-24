namespace Cyrus.Core.RequestResponse.Queries;

/// <summary>
/// Default implementation of <see cref="IPageQuery{TData}"/> with clamped paging, multi-sort, and filtering support.
/// </summary>
public class PageQuery<TData> : IPageQuery<TData>
{
    /// <summary>Default page size when not specified.</summary>
    public const int DefaultPageSize = 10;

    /// <summary>Maximum allowed page size to prevent excessive queries.</summary>
    public const int MaxPageSize = 200;

    private readonly List<SortDescriptor> _sorts = new();
    private readonly List<FilterCondition> _filters = new();

    /// <summary>1-based page number. Values below 1 will be treated as 1.</summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>Requested page size, clamped to a safe maximum.</summary>
    public int PageSize { get; set; } = DefaultPageSize;

    /// <summary>Number of items to skip, derived from normalized page number and size.</summary>
    public int SkipCount => (NormalizePageNumber(PageNumber) - 1) * NormalizePageSize(PageSize);

    /// <summary>Number of items to take, equals the normalized page size.</summary>
    public int TakeCount => NormalizePageSize(PageSize);

    /// <summary>Whether to compute and return the total count along with items.</summary>
    public bool NeedTotalCount { get; set; }

    /// <summary>Fallback single-column sort property name (used when no multi-sorts are supplied).</summary>
    public string SortBy { get; set; } = "Id";

    /// <summary>Sorting direction for the <see cref="SortBy"/> fallback column.</summary>
    public bool SortAscending { get; set; } = true;

    /// <summary>Multiple sort descriptors for advanced sorting scenarios.</summary>
    public IReadOnlyList<SortDescriptor> Sorts => _sorts;

    /// <summary>Optional search term for fuzzy/broad search operations.</summary>
    public string? SearchTerm { get; set; }

    /// <summary>Structured filter conditions for building dynamic predicates.</summary>
    public IReadOnlyList<FilterCondition> Filters => _filters;

    /// <summary>Optional include paths for data source navigation/expansion.</summary>
    public string[] Includes { get; set; } = Array.Empty<string>();

    /// <summary>Optional list of fields to select/project (repository-specific).</summary>
    public string[] Fields { get; set; } = Array.Empty<string>();

    /// <summary>Adds a sorting descriptor to the end of the list.</summary>
    /// <param name="property">Property/column name to sort by.</param>
    /// <param name="ascending">True for ascending order; false for descending.</param>
    public void AddSort(string property, bool ascending = true)
    {
        if (string.IsNullOrWhiteSpace(property)) return;
        _sorts.Add(new SortDescriptor(property, ascending));
    }

    /// <summary>Adds a new filter condition.</summary>
    /// <param name="condition">Filter condition to apply.</param>
    public void AddFilter(FilterCondition? condition)
    {
        if (condition is null) return;
        _filters.Add(condition);
    }

    /// <summary>Clears all registered sorting descriptors.</summary>
    public void ClearSorting() => _sorts.Clear();

    /// <summary>Clears all registered filter conditions.</summary>
    public void ClearFilters() => _filters.Clear();

    /// <summary>
    /// Ensures page number is at least 1.
    /// </summary>
    private static int NormalizePageNumber(int pageNumber) => pageNumber < 1 ? 1 : pageNumber;

    /// <summary>
    /// Clamps page size between 1 and <see cref="MaxPageSize"/>.
    /// </summary>
    private static int NormalizePageSize(int pageSize)
    {
        if (pageSize < 1) return DefaultPageSize;
        return pageSize > MaxPageSize ? MaxPageSize : pageSize;
    }
}
