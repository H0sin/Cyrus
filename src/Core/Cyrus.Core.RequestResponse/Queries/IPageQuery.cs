namespace Cyrus.Core.RequestResponse.Queries;

/// <summary>
/// Contract for paged query request models with sorting, filtering and search options.
/// </summary>
/// <typeparam name="TData">Type of items returned by the query.</typeparam>
public interface IPageQuery<TData> : IQuery<TData>
{
    /// <summary>1-based page number. Values less than 1 should be treated as 1.</summary>
    int PageNumber { get; set; }

    /// <summary>Page size (items per page). Implementations should clamp to a reasonable maximum.</summary>
    int PageSize { get; set; }

    /// <summary>Number of items to skip, derived from <see cref="PageNumber"/> and <see cref="PageSize"/>.</summary>
    int SkipCount { get; }

    /// <summary>Number of items to take, typically equals the normalized <see cref="PageSize"/>.</summary>
    int TakeCount { get; }

    /// <summary>Whether the total count should be computed and returned alongside data.</summary>
    bool NeedTotalCount { get; set; }

    /// <summary>Single-column sorting fallback (kept for backward compatibility).</summary>
    string SortBy { get; set; }

    /// <summary>Sorting direction for the fallback <see cref="SortBy"/> column.</summary>
    bool SortAscending { get; set; }

    /// <summary>Optional multiple sort descriptors. If empty, <see cref="SortBy"/> and <see cref="SortAscending"/> may be used.</summary>
    IReadOnlyList<SortDescriptor> Sorts { get; }

    /// <summary>Optional search term used by handlers to perform broad match queries.</summary>
    string? SearchTerm { get; set; }

    /// <summary>Optional filter conditions for structured filtering.</summary>
    IReadOnlyList<FilterCondition> Filters { get; }

    /// <summary>Optional include paths for navigation properties (repository-specific usage).</summary>
    string[] Includes { get; set; }

    /// <summary>Optional list of fields to select/project (repository-specific usage).</summary>
    string[] Fields { get; set; }

    /// <summary>Adds a sort descriptor.</summary>
    void AddSort(string property, bool ascending = true);

    /// <summary>Adds a filter condition.</summary>
    void AddFilter(FilterCondition condition);

    /// <summary>Clears all sort descriptors.</summary>
    void ClearSorting();

    /// <summary>Clears all filter conditions.</summary>
    void ClearFilters();
}
