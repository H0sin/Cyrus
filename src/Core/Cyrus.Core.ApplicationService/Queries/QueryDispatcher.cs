using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cyrus.Core.RequestResponse.Queries;
using Cyrus.Core.Contracts.ApplicationServices.Queries;
using Cyrus.Utilities.Logging;

namespace Cyrus.Core.ApplicationService.Queries;

/// <summary>
/// Resolves a query handler and executes it, logging routing start and performance metrics.
/// </summary>
public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueryDispatcher> _logger;
    private readonly Stopwatch _stopwatch = new();

    public QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query) where TQuery : class, IQuery<TData>
        => Execute<TQuery, TData>(query, CancellationToken.None);

    /// <inheritdoc />
    public Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query, CancellationToken cancellationToken) where TQuery : class, IQuery<TData>
    {
        _stopwatch.Restart();
        try
        {
            _logger.LogDebug("Routing query of type {QueryType} With value {Query}  Start at {StartDateTime}", query.GetType(), query, DateTime.Now);
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TData>>();
            return handler.Handle(query, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "There is not suitable handler for {QueryType} Routing failed at {StartDateTime}.", query.GetType(), DateTime.Now);
            throw;
        }
        finally
        {
            _stopwatch.Stop();
            _logger.LogInformation(CyrusEventId.PerformanceMeasurement, "Processing the {QueryType} query tooks {Millisecconds} Millisecconds", query.GetType(), _stopwatch.ElapsedMilliseconds);
        }
    }
}