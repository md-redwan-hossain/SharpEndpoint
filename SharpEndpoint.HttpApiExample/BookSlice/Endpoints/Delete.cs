using Microsoft.AspNetCore.Mvc;
using SharpEndpoint.HttpApiExample.BookSlice.Services;
using SharpEndpoint.HttpApiExample.Utils;

namespace SharpEndpoint.HttpApiExample.BookSlice.Endpoints;

public class Delete : SharpEndpointFragment
{
    protected override string RouteGroup() => BookRouteConstants.BaseRoute;
    protected override string Route() => BookRouteConstants.IdParam;

    protected override HttpVerb Verb() => HttpVerb.DELETE;

    protected override IEnumerable<Action<RouteHandlerBuilder>> Configure()
    {
        return
        [
            ..base.Configure(),
            e => e.WithSummary("delete a book"),
            e => e.Produces(StatusCodes.Status204NoContent),
            e => e.Produces(StatusCodes.Status400BadRequest),
            e => e.Produces(StatusCodes.Status422UnprocessableEntity)
        ];
    }

    protected override Delegate RequestHandler()
    {
        return async ([FromRoute] int id, [FromServices] IBookService bookService) =>
        {
            var result = await bookService.RemoveAsync(id);

            return result.Match<IResult>(
                entity => TypedResults.Json(data: entity, statusCode: StatusCodes.Status204NoContent),
                err => TypedResults.Json(data: err)
            );
        };
    }
}