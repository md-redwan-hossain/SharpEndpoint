using System.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SharpEndpoint;

/// <summary>
/// <c>SharpEndpointFragment</c> offers a straightforward and opinionated way to organize minimal API endpoints.
/// Constructor dependency injection is not allowed by design to stick with the minimal API convention.
/// </summary>
public abstract class SharpEndpointFragment
{
    /// <summary>
    /// <c>RouteGroup</c> is the same as <c>[StringSyntax("Route")] string prefix</c> in <c>MapGroup</c> method in Minimal API.
    /// </summary>
    protected virtual string RouteGroup() => string.Empty;

    /// <summary>
    /// <c>Route</c> is the same as <c>[StringSyntax("Route")] string pattern</c> in <c>Map*</c> methods in Minimal API.
    /// </summary>
    protected virtual string Route() => string.Empty;

    /// <summary>
    /// <c>Verb</c> will map the <c>RequestHandler</c> with appropriate <c>Map*</c> method in Minimal API.
    /// </summary>
    protected abstract HttpVerb Verb();

    /// <summary>
    /// Sets conventions on the <c>RouteGroup</c>
    /// </summary>
    protected virtual IEnumerable<Action<RouteGroupBuilder>> ConfigureRouteGroup()
    {
        var defaultTag = string.IsNullOrEmpty(RouteGroup()) ? "Uncategorized" : RouteGroup();

        return
        [
            e => e.WithOpenApi(),
            e => e.WithTags(defaultTag)
        ];
    }

    /// <summary>
    /// Sets conventions on the <c>Route</c>
    /// </summary>
    protected virtual IEnumerable<Action<RouteHandlerBuilder>> ConfigureRoute()
    {
        return [e => e.WithOpenApi()];
    }

    /// <summary>
    /// <c>RequestHandler</c> will handle the request.
    /// </summary>
    protected abstract Delegate RequestHandler();

    private void MapSharpEndpointFragment__DO_NOT_OVERRIDE(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RouteGroup());

        var endpoint = Verb() switch
        {
            HttpVerb.GET => group.MapGet(Route(), RequestHandler()),
            HttpVerb.POST => group.MapPost(Route(), RequestHandler()),
            HttpVerb.PUT => group.MapPut(Route(), RequestHandler()),
            HttpVerb.DELETE => group.MapDelete(Route(), RequestHandler()),
            HttpVerb.PATCH => group.MapPatch(Route(), RequestHandler()),
            _ => throw new InvalidEnumArgumentException()
        };

        foreach (var elem in ConfigureRouteGroup())
        {
            elem(group);
        }

        foreach (var elem in ConfigureRoute())
        {
            elem(endpoint);
        }
    }
}