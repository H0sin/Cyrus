using Cyrus.Core.Contracts.Data.Commands;
using Cyrus.Core.Contracts.Data.Queries;

namespace Cyrus.EndPoints.Web.Extensions.DependencyInjection;

public static class AddDataAccessExtensions
{
    public static IServiceCollection AddZaminDataAccess(
        this IServiceCollection services,
        IEnumerable<Assembly> assembliesForSearch) =>
        services.AddRepositories(assembliesForSearch).AddUnitOfWorks(assembliesForSearch);

    public static IServiceCollection AddRepositories(this IServiceCollection services,
        IEnumerable<Assembly> assembliesForSearch) =>
        services.AddWithTransientLifetime(assembliesForSearch, typeof(ICommandRepository<,>), typeof(IQueryRepository));

    public static IServiceCollection AddUnitOfWorks(this IServiceCollection services,
        IEnumerable<Assembly> assembliesForSearch) =>
        services.AddWithTransientLifetime(assembliesForSearch, typeof(IUnitOfWork));
}
