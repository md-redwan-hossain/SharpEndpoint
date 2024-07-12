| Branch | Status                                                                                                             |
|--------|--------------------------------------------------------------------------------------------------------------------|
| main   | ![Dotnet 8](https://github.com/md-redwan-hossain/SharpEndpoint/actions/workflows/dotnet.yml/badge.svg?branch=main) |

- [Installation](#installation)
- [Usage Guide](#usage-guide)
- [Default Choices](#default-choices)
- [Examples](#examples)

---

### Installation

- `SharpEndpoint` is a C# library based on minimal API that offers an opinionated way to organize minimal API endpoints.
- To install, run `dotnet add package SharpEndpoint` or from [Nuget](https://www.nuget.org/packages/SharpEndpoint/)

---

### Usage Guide

- Crate a class and inherit it from `SharpEndpointFragment`, then implement the required methods.
- Override `Route` and `RouteGroup` on demand to set endpoint route.
- Override `ConfigureRoute` and `ConfigureRouteGroup` for providing configurations.
- Constructor dependency injection is not allowed by design to stick with the minimal API convention.
- To map all the endpoints, Call `MapSharpEndpointFragmentsFromAssembly` in the `Program.cs` file from
  the `WebApplication` instance. For example,

```csharp
var app = builder.Build();
app.MapSharpEndpointFragmentsFromAssembly(typeof(Program).Assembly);
```

---

### Default Choices

- The default `Route` and `RouteGroup` are `string.Empty`
- OpenAPI is enabled by default on both `Route` and `RouteGroup` by `WithOpenApi()` in `ConfigureRoute`
  and `ConfigureRouteGroup`
- To use the default configurations, use `..base.Configure()` in the return `IEnumerable`

---

### Examples

- A complete REST API with CRUD functionality example is also given to showcase the usefulness of SharpOutcome. Source
  code is available [here.](https://github.com/md-redwan-hossain/SharpEndpoint/tree/main/SharpEndpoint.HttpApiExample)

```csharp
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
            e => e.Produces(StatusCodes.Status304NotModified),
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
```
