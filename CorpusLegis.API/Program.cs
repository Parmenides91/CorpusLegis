
using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.Shared.Dtos;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Se agrega una referencia al proyecto de CorpusLegis.Shared para poder usar sus servicios
//var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("corpuslegis-api");
// TODO: no me dice Gemini cómo hacerlo. --> se hace desde el SolutionExplorer.

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Se agrega el contexto de la base de datos (a la manera Aspire)
builder.AddSqlServerDbContext<CorpusLegisContext>("corpuslegis-db");

var app = builder.Build();

app.MapDefaultEndpoints();


// ---------------------------------------------------------
// ENDPOINTS (MINIMAL API)
// ---------------------------------------------------------
// Endpoint para crear un Rogatio
app.MapPost("/rogatio", async (CorpusLegisContext db, RogatioDto dto) =>
{
    var entity = new Rogatio
    {
        Id = Guid.NewGuid(),
        Title = dto.Title,
        Content = dto.Content,
        Status = dto.Status.ToString(),
        CreatedAt = DateTime.UtcNow
    };

    db.Rogatios.Add(entity);
    await db.SaveChangesAsync();

    return Results.Created($"/rogatio/{entity.Id}", entity); // se devuelve un 201
}
);

// Endpoint para listar los Rogatio
app.MapGet("/rogatio", async (CorpusLegisContext db) =>
    await db.Rogatios.ToListAsync());
// ---------------------------------------------------------


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Se crea automáticamente la base de datos al iniciar la aplicación (si no existe), sólo para DEV. Evita lanzar update-database.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CorpusLegisContext>();
    db.Database.EnsureCreated();
}

app.Run();
