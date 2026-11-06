using InvoiceManager.Infrastructure.Persistence;
using InvoiceManager.Application.Abstractions.Storage;
using InvoiceManager.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using InvoiceManager.Application.Abstractions.Extraction;
using InvoiceManager.Infrastructure.AI;
using InvoiceManager.Application.UseCases.Invoices;
using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Infrastructure.Persistence.Repositories;

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

//Gemini Configuration
builder.Services
    .AddOptions<GeminiOptions>()
    .Bind(builder.Configuration.GetSection(
        GeminiOptions.SectionName))
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.ApiKey),
        "The API Key is required.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Model),
        "The Gemini Model is required")
    .ValidateOnStart();

//Http Client Typing for Gemini
builder.Services.AddHttpClient<
    IInvoiceExtractionService,
    GeminiInvoiceExtractionService
>(client =>
{
    client.BaseAddress = new Uri(
        "https://generativelanguage.googleapis.com/"
    );

    client.Timeout = TimeSpan.FromSeconds(90);
});
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IUnitOfWork, InvoiceManagerDbContext>();

//UseCase Inyection
builder.Services.AddScoped<ListInvoicesUseCase>();
builder.Services.AddScoped<GetInvoiceDetailsUseCase>();
builder.Services.AddScoped<UpdateInvoiceUseCase>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "InvoiceManager API v1");
        c.RoutePrefix = string.Empty; // Esto hace que Swagger abra directamente en la raíz (ej: https://localhost:xxxx/)
    });
}

app.UseHttpsRedirection();
 
app.UseAuthorization();

app.MapControllers();

app.Run();
