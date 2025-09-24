using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cyrus.Core.RequestResponse.Common;
using Cyrus.Core.RequestResponse.Queries;
using Cyrus.Core.Contracts.ApplicationServices.Queries;
using Cyrus.Core.Domain.Exceptions;
using Cyrus.Utilities.Logging;
using Cyrus.Utilities.Resources;

namespace Cyrus.Core.ApplicationService.Queries;

/// <summary>
/// Catches domain exceptions during query execution and converts them into uniform results
/// with localized messages. Non-domain exceptions are rethrown for higher-level handling.
/// </summary>
public class QueryDispatcherDomainExceptionHandlerDecorator(
    IServiceProvider serviceProvider,
    ILogger<QueryDispatcherDomainExceptionHandlerDecorator> logger,
    IQueryDispatcher inner)
    : QueryDispatcherDecorator(inner)
{
    public override int Order => 2;

    public override Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query) => Execute<TQuery, TData>(query, CancellationToken.None);

    public override async Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query, CancellationToken cancellationToken)
    {
        try
        {
            return await Inner.Execute<TQuery, TData>(query, cancellationToken);
        }
        catch (InvalidEntityStateException ex)
        {
            logger.LogError(CyrusEventId.DomainValidationException, ex, "Processing of {QueryType} With value {Query} failed at {StartDateTime} because there are domain exceptions.", query.GetType(), query, DateTime.Now);
            return DomainExceptionHandlingWithReturnValue<TData>(ex);
        }
        catch (DomainStateException ex)
        {
            logger.LogError(CyrusEventId.DomainValidationException, ex, "Processing of {QueryType} With value {Query} failed at {StartDateTime} because there are domain exceptions.", query.GetType(), query, DateTime.Now);
            return DomainExceptionHandlingWithReturnValue<TData>(ex);
        }
        catch (AggregateException ex) when (ex.InnerException is DomainStateException domainStateException)
        {
            logger.LogError(CyrusEventId.DomainValidationException, ex, "Processing of {QueryType} With value {Query} failed at {StartDateTime} because there are domain exceptions.", query.GetType(), query, DateTime.Now);
            return DomainExceptionHandlingWithReturnValue<TData>(domainStateException);
        }
        catch
        {
            throw;
        }
    }

    private QueryResult<TData> DomainExceptionHandlingWithReturnValue<TData>(DomainStateException ex)
    {
        var queryResult = new QueryResult<TData>
        {
            Status = ApplicationServiceStatus.InvalidDomainState
        };
        queryResult.AddMessage(GetExceptionText(ex));
        return queryResult;
    }

    private string GetExceptionText(DomainStateException domainStateException)
    {
        var translator = serviceProvider.GetService<ITranslator>();
        if (translator == null) return domainStateException.ToString();

        var result = (domainStateException.Parameters?.Any() == true)
            ? translator[domainStateException.Message, domainStateException.Parameters]
            : translator[domainStateException.Message];

        logger.LogInformation(CyrusEventId.DomainValidationException, "Domain Exception message is {DomainExceptionMessage}", result);
        return result;
    }
}