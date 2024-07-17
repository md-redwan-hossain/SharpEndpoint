using Microsoft.AspNetCore.Mvc;
using SharpEndpoint.HttpApiExample.BookSlice.Domain;
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
            e => e.Produces<PagedData<ICollection<Book>>>()
        ];
    }

    protected override Delegate RequestHandler()
    {
        return async ([FromServices] IBookService bookService, [FromQuery] int page = 1, [FromQuery] int limit = 15) =>
        {
            var result = await bookService.GetAllAsync(page, limit);
            var res = new PagedData<ICollection<Book>>(result.Data, result.TotalDataCount);
            return TypedResults.Json(data: res, statusCode: StatusCodes.Status200OK);
        };
    }
}