using FluentValidation;
using FluentValidation.Results;

namespace SharpEndpoint.HttpApiExample.Utils;

public class FluentValidationFilter<T> : IEndpointFilter
    where T : class
{
    private readonly IValidator<T> _validator;

    public FluentValidationFilter(IValidator<T> validator) => _validator = validator;


    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T)) is not T instance)
        {
            return TypedResults.UnprocessableEntity();
        }

        var validationResult = await _validator.ValidateAsync(instance, context.HttpContext.RequestAborted);
        if (validationResult.IsValid is false)
        {
            return TypedResults.Json(data: MapErrors(validationResult.Errors),
                statusCode: StatusCodes.Status400BadRequest);
        }

        return await next(context);
    }

    private static IEnumerable<Dictionary<string, object>> MapErrors(IEnumerable<ValidationFailure> errors)
    {
        return errors.Select(error =>
        {
            var errorInfo = new Dictionary<string, object>
            {
                { "propertyName", error.FormattedMessagePlaceholderValues["PropertyName"] },
                { "errorMessage", error.ErrorMessage },
                { "attemptedValue", error.AttemptedValue }
            };

            if (error.FormattedMessagePlaceholderValues.TryGetValue("CollectionIndex", out var index))
            {
                errorInfo["collectionIndex"] = index;
            }

            return errorInfo;
        });
    }
}