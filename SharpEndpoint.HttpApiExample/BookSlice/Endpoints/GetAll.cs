using Microsoft.AspNetCore.Mvc;
using SharpEndpoint.HttpApiExample.BookSlice.Services;
using SharpEndpoint.HttpApiExample.Utils;

namespace SharpEndpoint.HttpApiExample.BookSlice.Endpoints;

public class GetAll : SharpEndpointFragment
{
    protected override string RouteGroup() => BookRouteConstants.BaseRoute;
    protected override HttpVerb Verb() => HttpVerb.GET;

    protected override IEnumerable<Action<RouteHandlerBuilder>> ConfigureRoute()
    {
        return
        [
            ..base.ConfigureRoute(),
            e => e.WithSummary("returns all books"),
            e => e.Produces(StatusCodes.Status200OK)
        ];
    }

    protected override Delegate RequestHandler()
    {
        return async ([FromServices] IBookService bookService) =>
        {
            var result = await bookService.GetAllAsync();
            return TypedResults.Json(data: result, statusCode: StatusCodes.Status200OK);
        };
    }
}