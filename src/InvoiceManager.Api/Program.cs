using InvoiceManager.Infrastructure.Persistence;
using InvoiceManager.Application.Abstractions.Storage;
using InvoiceManager.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "The string connection 'DefaultConnection' was not found."
        );
}


builder.Services.AddDbContext<InvoiceManagerDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    // Los datos ficticios solamente se cargan
    // durante el desarrollo.
    if (builder.Environment.IsDevelopment())
    {
        // Inicialización síncrona.
        options.UseSeeding((context, _) =>
        {
            DatabaseSeeder.Seed(context);
        });

        // Inicialización asíncrona.
        options.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            await DatabaseSeeder.SeedAsync(
                context,
                cancellationToken
            );
        });
    }
});
builder.Services
    .AddOptions<FileStorageOptions>()
    .Bind(builder.Configuration.GetSection(
        FileStorageOptions.SectionName))
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.RootPath) &&
            Path.IsPathFullyQualified(options.RootPath),
        "The Storage Path is must be absolute")
    .ValidateOnStart();

builder.Services.AddScoped<
    IFileStorageService,
    LocalFileStorageService
>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
