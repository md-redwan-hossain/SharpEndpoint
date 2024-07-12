using Mapster;
using Microsoft.EntityFrameworkCore;
using SharpEndpoint.HttpApiExample.BookSlice.Domain;
using SharpEndpoint.HttpApiExample.Persistence;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace SharpEndpoint.HttpApiExample.BookSlice.Services;

public class BookService : IBookService
{
    private readonly BookDbContext _bookDbContext;
    public BookService(BookDbContext bookDbContext) => _bookDbContext = bookDbContext;

    public async Task<ValueOutcome<Book, IBadOutcome>> CreateAsync(CreateOrUpdateBookRequest dto)
    {
        try
        {
            var duplicateIsbn = await _bookDbContext.Books
                .Where(x => x.Isbn == dto.Isbn)
                .FirstOrDefaultAsync();

            if (duplicateIsbn is not null)
            {
                return new BadOutcome(BadOutcomeTag.Conflict, $"Duplicate isbn: {dto.Isbn}");
            }

            var entity = await dto.BuildAdapter().AdaptToTypeAsync<Book>();
            await _bookDbContext.Books.AddAsync(entity);
            await _bookDbContext.SaveChangesAsync();
            return entity;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BadOutcome(BadOutcomeTag.Unexpected);
        }
    }

    public async Task<ValueOutcome<Book, IBadOutcome>> UpdateAsync(int id, CreateOrUpdateBookRequest dto)
    {
        try
        {
            var entityToUpdate = await _bookDbContext.Books.FindAsync(id);
            if (entityToUpdate is null) return new BadOutcome(BadOutcomeTag.NotFound);

            await dto.BuildAdapter().AdaptToAsync(entityToUpdate);
            _bookDbContext.Books.Attach(entityToUpdate);
            _bookDbContext.Entry(entityToUpdate).State = EntityState.Modified;
            await _bookDbContext.SaveChangesAsync();
            return entityToUpdate;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BadOutcome(BadOutcomeTag.Unexpected);
        }
    }

    public async Task<IList<Book>> GetAllAsync()
    {
        return await _bookDbContext.Books.ToListAsync();
    }

    public async Task<ValueOutcome<Book, IBadOutcome>> GetOneAsync(int id)
    {
        var entity = await _bookDbContext.Books.FindAsync(id);
        if (entity is null) return new BadOutcome(BadOutcomeTag.NotFound);
        return entity;
    }

    public async Task<ValueOutcome<IGoodOutcome, IBadOutcome>> RemoveAsync(int id)
    {
        try
        {
            var entityToDelete = await _bookDbContext.Books.FindAsync(id);
            if (entityToDelete is null) return new BadOutcome(BadOutcomeTag.NotFound);

            if (_bookDbContext.Entry(entityToDelete).State is EntityState.Detached)
            {
                _bookDbContext.Books.Attach(entityToDelete);
            }

            _bookDbContext.Books.Remove(entityToDelete);
            await _bookDbContext.SaveChangesAsync();

            return new GoodOutcome(GoodOutcomeTag.Deleted);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new BadOutcome(BadOutcomeTag.Unexpected);
        }
    }
}