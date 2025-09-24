using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Cyrus.Core.RequestResponse.Common;
using Cyrus.Core.RequestResponse.Queries;
using Cyrus.Core.Contracts.ApplicationServices.Queries;
using Cyrus.Utilities.Logging;

namespace Cyrus.Core.ApplicationService.Queries;

/// <summary>
/// Validates queries using FluentValidation before dispatching to the next dispatcher in the chain.
/// Logs start, finish, and any validation failures using CyrusEventId.QueryValidation.
/// </summary>
public class QueryDispatcherValidationDecorator(
    IServiceProvider serviceProvider,
    ILogger<QueryDispatcherValidationDecorator> logger,
    IQueryDispatcher inner)
    : QueryDispatcherDecorator(inner)
{
    public override int Order => 1;

    public override Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query) => Execute<TQuery, TData>(query, CancellationToken.None);

    public override async Task<QueryResult<TData>> Execute<TQuery, TData>(TQuery query, CancellationToken cancellationToken)
    {
        logger.LogDebug(CyrusEventId.QueryValidation, "Validating query of type {QueryType} With value {Query}  start at :{StartDateTime}", query.GetType(), query, DateTime.Now);

        var validationResult = Validate<TQuery, QueryResult<TData>>(query);
        if (validationResult != null)
        {
            logger.LogInformation(CyrusEventId.QueryValidation, "Validating query of type {QueryType} With value {Query}  failed. Validation errors are: {ValidationErrors}", query.GetType(), query, validationResult.Messages);
            return validationResult;
        }

        logger.LogDebug(CyrusEventId.QueryValidation, "Validating query of type {QueryType} With value {Query}  finished at :{EndDateTime}", query.GetType(), query, DateTime.Now);
        return await Inner.Execute<TQuery, TData>(query, cancellationToken);
    }

    private TValidationResult? Validate<TQuery, TValidationResult>(TQuery query) where TValidationResult : ApplicationServiceResult, new()
    {
        var validator = serviceProvider.GetService<IValidator<TQuery>>();
        TValidationResult? res = null;

        if (validator != null)
        {
            var validation = validator.Validate(query);
            if (!validation.IsValid)
            {
                res = new() { Status = ApplicationServiceStatus.ValidationError };
                foreach (var item in validation.Errors)
                {
                    res.AddMessage(item.ErrorMessage);
                }
            }
        }
        else
        {
            logger.LogInformation(CyrusEventId.CommandValidation, "There is not any validator for {QueryType}", query.GetType());
        }
        return res;
    }
}