using Microsoft.AspNetCore.Mvc;
using SharpEndpoint.HttpApiExample.BookSlice.Services;
using SharpEndpoint.HttpApiExample.Utils;

namespace SharpEndpoint.HttpApiExample.BookSlice.Endpoints;

public class Update : SharpEndpointFragment
{
    protected override string RouteGroup() => BookRouteConstants.BaseRoute;
    protected override string Route() => BookRouteConstants.IdParam;
    protected override HttpVerb Verb() => HttpVerb.PUT;

    protected override IEnumerable<Action<RouteHandlerBuilder>> ConfigureRoute()
    {
        return
        [
            ..base.ConfigureRoute(),
            e => e.WithSummary("update a book"),
            e => e.Produces(StatusCodes.Status200OK),
            e => e.Produces(StatusCodes.Status404NotFound),
            e => e.AddEndpointFilter<FluentValidationFilter<CreateOrUpdateBookRequest>>()
        ];
    }

    protected override Delegate RequestHandler()
    {
        return async ([FromRoute] int id, [FromServices] IBookService bookService,
            [FromBody] CreateOrUpdateBookRequest dto) =>
        {
            var result = await bookService.UpdateAsync(id, dto);

            return result.Match<IResult>(
                entity => TypedResults.Json(data: entity, statusCode: StatusCodes.Status200OK),
                err => TypedResults.Json(data: err, statusCode: StatusCodes.Status304NotModified)
            );
        };
    }
}