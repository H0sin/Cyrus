using Cyrus.Core.Contracts.ApplicationServices.Commands;
using Cyrus.Core.Contracts.ApplicationServices.Events;
using Cyrus.Core.Contracts.ApplicationServices.Queries;
using Cyrus.Utilities;

namespace Cyrus.EndPoints.Web.Extensions;

public static class HttpContextExtensions
{
    public static ICommandDispatcher? CommandDispatcher(this HttpContext httpContext) =>
        (ICommandDispatcher)httpContext.RequestServices.GetService(typeof(ICommandDispatcher))!;

    public static IQueryDispatcher QueryDispatcher(this HttpContext httpContext) =>
        (IQueryDispatcher)httpContext.RequestServices.GetService(typeof(IQueryDispatcher))!;

    public static IEventDispatcher EventDispatcher(this HttpContext httpContext) =>
        (IEventDispatcher)httpContext.RequestServices.GetService(typeof(IEventDispatcher))!;

    public static CyrusService CyrusApplicationContext(this HttpContext httpContext) =>
        (CyrusService)httpContext.RequestServices.GetService(typeof(CyrusService))!;
}