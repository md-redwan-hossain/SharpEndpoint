using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SharpEndpoint.HttpApiExample.BookSlice.Domain;

namespace SharpEndpoint.HttpApiExample.Persistence;

public class BookDbContext(DbContextOptions<BookDbContext> options)
    : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<Book>().HasData(GenerateData());
    }

    private static List<Book> GenerateData()
    {
        List<string> genres = ["Fantasy", "Sci-Fi", "Mystery", "Romance", "Horror", "Non-fiction"];

        return new Bogus.Faker<Book>()
            .RuleFor(b => b.Id, f => f.IndexFaker + 1)
            .RuleFor(b => b.Isbn, _ => Guid.NewGuid().ToString())
            .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
            .RuleFor(b => b.Genre, f => f.PickRandom(genres))
            .RuleFor(b => b.Author, f => f.Name.FullName())
            .RuleFor(b => b.Price, f => f.Random.Int(100, 1000))
            .Generate(200);
    }
}