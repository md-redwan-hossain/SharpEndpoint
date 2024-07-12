using FluentValidation;

namespace SharpEndpoint.HttpApiExample.BookSlice;

public record CreateOrUpdateBookRequest(string Title, string Isbn, string Genre, string Author, int Price);

public class CreateOrUpdateBookRequestValidator : AbstractValidator<CreateOrUpdateBookRequest>
{
    public CreateOrUpdateBookRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Price).NotNull().GreaterThan(0);
        RuleFor(x => x.Author).NotEmpty();
        RuleFor(x => x.Genre).NotEmpty();
        RuleFor(x => x.Isbn).NotEmpty();
    }
}