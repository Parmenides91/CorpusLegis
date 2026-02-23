
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

    // Se crea automáticamente la base de datos al iniciar la aplicación (si no existe), sólo para DEV. Evita lanzar update-database.
    //db.Database.EnsureCreated();

    // Al pasar a migraciones de EF ya no podemos tener ese EnsureCreated.
    db.Database.Migrate();
    // cada vez que hagas una modificación del Domain debes hacer:
    // 1) situarte en el proyecto CorpusLegis.API con la Package Manager Console.
    // 2) ejecutar "dotnet ef migrations add NombreDeLaMigración".
    // 3) mirar ^^ (al lanzar la app las migraciones se irán aplicando en orden).

    if (!db.Cives.Any()) // Se crea un Civis de ejemplo si la tabla está vacía.
    {
        Guid CivisDefaultGuid = Guid.Parse("0f8fad5b - d9cb - 469f - a165 - 70867728950e");

        db.Cives.Add(new Civis
        {
            Id = CivisDefaultGuid,
            Name = "Sempronio"
        });
    }

    if (!db.Civitates.Any()) // Se crea una Civitas de ejemplo si la tabla está vacía.
    {
        Guid CivitasDefaultGuid = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");

        db.Civitates.Add(new Civitas
        {
            Id = CivitasDefaultGuid,
            Name = "Solfamidas"
        });
    }

    if (!db.Rogationes.Any()) // Se crea un Rogatio de ejemplo si la tabla está vacía.
    {
        db.Rogationes.Add(new Rogatio
        {
            Id = Guid.NewGuid(),
            Title = "Rogatio de ejemplo",
            Content = "Contenido del rogatio de ejemplo",
            CreatedAt = DateTime.UtcNow,
            CivisId = Guid.Parse("0f8fad5b - d9cb - 469f - a165 - 70867728950e"), // Sempronio
            Civis = db.Cives.FirstOrDefault(c => c.Id == Guid.Parse("0f8fad5b - d9cb - 469f - a165 - 70867728950e"))!, // Sempronio
            CivitasId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), // Solfamidas
            Civitas = db.Civitates.FirstOrDefault(c => c.Id == Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"))!, // Solfamidas
        });

        db.SaveChanges();
    }
}

app.Run();
