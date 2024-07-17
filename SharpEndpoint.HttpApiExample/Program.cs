using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpEndpoint;
using SharpEndpoint.HttpApiExample.BookSlice.Services;
using SharpEndpoint.HttpApiExample.Persistence;

var builder = WebApplication.CreateBuilder(args);

const string connectionString = "DataSource=db.sqlite3;Cache=Shared;";
builder.Services.AddDbContext<BookDbContext>(opts => opts.UseSqlite(connectionString));

var optionsBuilder = new DbContextOptionsBuilder<BookDbContext>();
optionsBuilder.UseSqlite(connectionString);

await using (var dbContext = new BookDbContext(optionsBuilder.Options))
{
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    dbContext.Database.OpenConnection();
    dbContext.Database.ExecuteSqlRaw("PRAGMA journal_mode=DELETE;");
    dbContext.Database.CloseConnection();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.SupportNonNullableReferenceTypes());

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));
builder.Services.TryAddScoped<IBookService, BookService>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(
        namingPolicy: JsonNamingPolicy.CamelCase,
        allowIntegerValues: false)
    );
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOriginsForCors", x => x
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});

var app = builder.Build();

app.Lifetime.ApplicationStopping.Register(() =>
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "book_db.sqlite3");
    if (File.Exists(dbPath)) File.Delete(dbPath);
});

app.UseSwagger();
app.UseSwaggerUI(o => o.EnableTryItOutByDefault());

app.UseCors("AllowedOriginsForCors");
app.MapSharpEndpointFragmentsFromAssembly(typeof(Program).Assembly);
app.Run();