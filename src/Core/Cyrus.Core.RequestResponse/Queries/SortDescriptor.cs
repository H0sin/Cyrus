namespace Cyrus.Core.RequestResponse.Queries;

/// <summary>
/// Describes a single sorting instruction for a query.
/// </summary>
public sealed record SortDescriptor(string Property, bool Ascending = true);

