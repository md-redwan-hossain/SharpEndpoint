using SharpEndpoint.HttpApiExample.BookSlice.Domain;
using SharpEndpoint.HttpApiExample.Utils;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace SharpEndpoint.HttpApiExample.BookSlice.Services;

public interface IBookService
{
    Task<ValueOutcome<Book, IBadOutcome>> CreateAsync(CreateOrUpdateBookRequest dto);
    Task<ValueOutcome<Book, IBadOutcome>> UpdateAsync(int id, CreateOrUpdateBookRequest dto);
    Task<PagedData<IList<Book>>> GetAllAsync(int page, int limit);
    Task<ValueOutcome<Book, IBadOutcome>> GetOneAsync(int id);
    Task<ValueOutcome<IGoodOutcome, IBadOutcome>> RemoveAsync(int id);
}