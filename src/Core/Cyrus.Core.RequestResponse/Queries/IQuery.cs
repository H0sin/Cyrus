namespace Cyrus.Core.RequestResponse.Queries;

/// <summary>
/// Marker interface for query request models that specify input parameters for fetching data.
/// </summary>
/// <typeparam name="TData">Type of the expected result payload.</typeparam>
public interface IQuery<TData>
{
}
