
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
// ENDPOINTS (MINIMAL API) para Rogatio.
// ---------------------------------------------------------
// GET /rogatio --> devuelve la lista de Rogatios
app.MapGet("/rogatio", async (CorpusLegisContext db)
    => await db.Rogatios
        .Select(r => new RogatioSummaryDto (
            r.Id,
            r.Title,
            r.Content,
            r.AuthorId,
            r.CreatedAt,
            r.Status
            ))
        .AsNoTracking()
        .ToListAsync()
    );

// GET /rogatio/{id} --> devuelve un Rogatio por su id
app.MapGet("rogatio/{id:guid}", async (CorpusLegisContext db, Guid id) =>
{
    var rogatio = await db.Rogatios.FindAsync(id);

    if (rogatio == null)
    {
        return Results.NotFound();
    }

    RogatioDetailsDto dto = new(
        rogatio.Id,
        rogatio.Title,
        rogatio.Content,
        Guid.Empty,
        rogatio.CreatedAt,
        rogatio.Status
    );

    return Results.Ok(dto);
});

// POST /rogatio --> crea un nuevo Rogatio
app.MapPost("/rogatio", async (CorpusLegisContext db, CreateRogatioDto newRogatio) => 
{
    Rogatio rogatio = new Rogatio
    {
        Id = Guid.NewGuid(),
        Title = newRogatio.Title,
        Content = newRogatio.Content,
        //AuthorId = newRogatio.AuthorId,
        CreatedAt = DateTime.UtcNow,
        Status = newRogatio.Status
    };

    db.Rogatios.Add(rogatio);
    await db.SaveChangesAsync();

    RogatioDetailsDto dto = new(
        rogatio.Id,
        rogatio.Title,
        rogatio.Content,
        Guid.Empty,
        rogatio.CreatedAt,
        rogatio.Status
    );

    return Results.Created($"/rogatio/{rogatio.Id}", dto); // se devuelve un 201 con el DTO de detalles
});

// PUT /rogatio/{id} --> actualiza un Rogatio existente por su id
app.MapPut("/rogatio/{id:guid}", async (CorpusLegisContext db, Guid id, UpdateRogatioDto updatedRogatio) =>
{
    //var existingRogatio = await db.Rogatios.FindAsync(updatedRogatio.Id);
    var existingRogatio = await db.Rogatios.FindAsync(id);

    if (existingRogatio == null)
    {
        return Results.NotFound();
    }

    existingRogatio.Title = updatedRogatio.Title;
    existingRogatio.Content = updatedRogatio.Content;
    //existingRogatio.AuthorId = updatedRogatio.AuthorId;
    existingRogatio.Status = updatedRogatio.Status;

    await db.SaveChangesAsync();

    RogatioDetailsDto dto = new(
        existingRogatio.Id,
        existingRogatio.Title,
        existingRogatio.Content,
        Guid.Empty,
        existingRogatio.CreatedAt,
        existingRogatio.Status
    );

    return Results.Ok(dto); // se devuelve un 200 con el DTO de detalles actualizado
}
);

// DELETE /rogatio/{id} --> elimina un Rogatio por su id
app.MapDelete("/rogatio/{id:guid}", async (CorpusLegisContext db, Guid id) =>
{
    await db.Rogatios.Where(r => r.Id == id).ExecuteDeleteAsync();

    return Results.NoContent(); // se devuelve un 204
}
);
// ---------------------------------------------------------


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CorpusLegisContext>();
    db.Database.EnsureCreated(); // Se crea automáticamente la base de datos al iniciar la aplicación (si no existe), sólo para DEV. Evita lanzar update-database.

    if (!db.Rogatios.Any()) // Se crea un Rogatio de ejemplo si la tabla está vacía.
    {
        db.Rogatios.Add(new Rogatio
        {
            Id = Guid.NewGuid(),
            Title = "Rogatio de ejemplo",
            Content = "Contenido del rogatio de ejemplo",
            Status = "Draft",
            CreatedAt = DateTime.UtcNow
        });

        db.SaveChanges();
    }
}

app.Run();
