using System.ComponentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SharpEndpoint;

public abstract class SharpEndpointFragment
{
    protected virtual string RouteGroup() => string.Empty;
    protected virtual string Route() => string.Empty;
    protected abstract HttpVerb Verb();

    protected virtual IEnumerable<Action<RouteGroupBuilder>> ConfigureOnGroup()
    {
        var defaultTag = string.IsNullOrEmpty(RouteGroup()) ? "Uncategorized" : RouteGroup();

        return
        [
            e => e.WithOpenApi(),
            e => e.WithTags(defaultTag)
        ];
    }

    protected virtual IEnumerable<Action<RouteHandlerBuilder>> Configure() => [];

    protected abstract Delegate RequestHandler();

    private void MapEndpoint(IEndpointRouteBuilder app)
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

        foreach (var elem in ConfigureOnGroup())
        {
            elem(group);
        }

        foreach (var elem in Configure())
        {
            elem(endpoint);
        }
    }
}