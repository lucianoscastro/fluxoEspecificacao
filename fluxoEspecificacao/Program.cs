using System.Collections.Concurrent;
using fluxoEspecificacao;

var builder = WebApplication.CreateBuilder(args);

// Register thread-safe in-memory storage
builder.Services.AddSingleton<ConcurrentDictionary<Guid, Pet>>();

// Generate the OpenAPI document
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Serve the OpenAPI document at /openapi/v1.json and the Swagger UI at /swagger
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "fluxoEspecificacao v1"));
}

app.UseHttpsRedirection();

app.MapPost("/pets", (RegisterPetRequest request, ConcurrentDictionary<Guid, Pet> db) =>
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(request.Name))
    {
        errors.Add("Name", ["Name is required and cannot be empty."]);
    }

    if (string.IsNullOrWhiteSpace(request.Breed))
    {
        errors.Add("Breed", ["Breed is required and cannot be empty."]);
    }

    if (string.IsNullOrWhiteSpace(request.TutorName))
    {
        errors.Add("TutorName", ["Tutor name is required and cannot be empty."]);
    }

    if (request.BirthDate > DateTime.UtcNow)
    {
        errors.Add("BirthDate", ["Birth date must be in the past."]);
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var pet = new Pet(
        Guid.NewGuid(),
        request.Name!.Trim(),
        request.Breed!.Trim(),
        request.BirthDate,
        request.TutorName!.Trim()
    );

    db[pet.Id] = pet;

    return Results.Created($"/pets/{pet.Id}", pet);
});

app.Run();

namespace fluxoEspecificacao
{
    public record Pet(Guid Id, string Name, string Breed, DateTime BirthDate, string TutorName);
    public record RegisterPetRequest(string? Name, string? Breed, DateTime BirthDate, string? TutorName);
}

public partial class Program { }

