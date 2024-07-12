using Microsoft.AspNetCore.Mvc;
using SharpEndpoint.HttpApiExample.BookSlice.Services;
using SharpEndpoint.HttpApiExample.Utils;

namespace SharpEndpoint.HttpApiExample.BookSlice.Endpoints;

public class Create : SharpEndpointFragment
{
    protected override string RouteGroup() => BookRouteConstants.BaseRoute;
    protected override HttpVerb Verb() => HttpVerb.POST;

    protected override IEnumerable<Action<RouteHandlerBuilder>> Configure()
    {
        return
        [
            ..base.Configure(),
            e => e.WithSummary("create a book"),
            e => e.Produces(StatusCodes.Status201Created),
            e => e.Produces(StatusCodes.Status400BadRequest),
            e => e.Produces(StatusCodes.Status422UnprocessableEntity),
            e => e.AddEndpointFilter<FluentValidationFilter<CreateOrUpdateBookRequest>>()
        ];
    }

    protected override Delegate RequestHandler()
    {
        return async ([FromServices] IBookService bookService, [FromBody] CreateOrUpdateBookRequest dto) =>
        {
            var result = await bookService.CreateAsync(dto);

            return result.Match<IResult>(
                entity => TypedResults.Json(data: entity, statusCode: StatusCodes.Status201Created),
                err => TypedResults.Json(data: err)
            );
        };
    }
}