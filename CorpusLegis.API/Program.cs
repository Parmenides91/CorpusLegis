
using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Endpoints;
using CorpusLegis.API.Services;
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

// Se registra el servicio de Rogatio.
builder.Services.AddScoped<IRogatioService, RogatioService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Se mapean los endpoints de Rogatio.
app.MapRogatioEndpoints();


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
