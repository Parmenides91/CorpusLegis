
using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Endpoints;
using CorpusLegis.API.Infrastructure;
using CorpusLegis.API.Infrastructure.Seeders;
using CorpusLegis.API.Services;
using CorpusLegis.API.Validators;
using CorpusLegis.Shared.Dtos;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Se agrega una referencia al proyecto de CorpusLegis.Shared para poder usar sus servicios
//var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("corpuslegis-api");
// TODO: no me dice Gemini cómo hacerlo. --> se hace desde el SolutionExplorer.

// Add services to the container.

// Se configura MassTransit para usar RabbitMQ como Message Broker.
builder.Services.AddMassTransit(x =>
{
    // Outbox para garantizar la entrega de mensajes si algo falla cuando se hacen transacciones.
    x.AddEntityFrameworkOutbox<CorpusLegisContext>(outbox =>
    {
        outbox.QueryDelay = TimeSpan.FromSeconds(5); // Intervalo de tiempo para consultar la tabla de Outbox.
        outbox.DuplicateDetectionWindow = TimeSpan.FromMinutes(5); // Ventana de tiempo para detectar mensajes duplicados.
        outbox.UseSqlServer(); // Configura el Outbox para usar SQL Server.
        outbox.UseBusOutbox(); // Configura el Outbox para enviar los mensajes a través del bus de MassTransit.
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("rabbitmq-corpuslegis"); // mismo nombre que tenga en AppHost.cs o no va a funcionar.
        cfg.Host(connectionString);
        cfg.ConfigureEndpoints(context);
    });
});

// Se configura AspNetCore Authentication JwtBearer.
var keycloakUrl = builder.Configuration.GetConnectionString("keycloak-corpuslegis");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer( options =>
    {
        options.Authority = $"{keycloakUrl}/realms/CorpusLegis";
        options.RequireHttpsMetadata = false; // en PRO esto tendrá que ser true.

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false, // Habrá que ajustarlo en PRO.
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "preferred_username",
            RoleClaimType = "realm_access.roles"
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Se agrega el contexto de la base de datos (a la manera Aspire)
builder.AddSqlServerDbContext<CorpusLegisContext>("sqlserver-db-corpuslegis");

// Se registra el servicio de Rogatio.
builder.Services.AddScoped<IRogatioService, RogatioService>();

// Se registra el servicio de Suffragium.
builder.Services.AddScoped<ISuffragiumService, SuffragiumService>();

// Se registra el servicio de Lex.
builder.Services.AddScoped<ILexService, LexService>();

// Se registra el servicio de Civitas.
builder.Services.AddScoped<ICivitasService, CivitasService>();

// Se registra el servicio del mockeo de usuarios que estamos haciendo.
builder.Services.AddHttpContextAccessor(); // Necesario para que CurrentUserService pueda acceder al contexto HTTP.
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Se registra el servicio de Excepciones.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Se registra el servicio de validaciones.
builder.Services.AddValidatorsFromAssemblyContaining<CreateRogatioDtoValidator>();


var app = builder.Build();

// Manejador para Excepciones.
app.UseExceptionHandler();

// Se habilita la autenticación y autorización.
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

// Se mapean los endpoints de Rogatio.
app.MapRogatioEndpoints();

// Se mapean los endpoints de Suffragium.
app.MapSuffragiumEndpoints();

// Se mapean los endpoints de Lex.
app.MapLexEndpoints();

// Se mapean los endpoints de Civitas.
app.MapCivitasEndpoints();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CorpusLegisContext>();

    // Se crea automáticamente la base de datos al iniciar la aplicación (si no existe), sólo para DEV. Evita lanzar update-database.
    //db.Database.EnsureCreated();

    // Al pasar a migraciones de EF ya no podemos tener ese EnsureCreated.
    db.Database.Migrate(); //esto sólo funciona si siempre elimino las migrations y empiezo de cero. Hay que corregir esto en algún momento.
    // cada vez que hagas una modificación del Domain debes hacer:
    // 1) situarte en el proyecto CorpusLegis.API con la Package Manager Console.
    // 2) ejecutar "dotnet ef migrations add NombreDeLaMigración".
    // 3) mirar ^^ (al lanzar la app las migraciones se irán aplicando en orden).

    //Guid CivisDefaultGuid = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"); // Sempronio
    //Guid Civis002Guid = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Tulio

    //Guid CivitasDefaultGuid = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"); // Solfamidas

    //if (!db.Cives.Any()) // Se crea un Civis de ejemplo si la tabla está vacía.
    //{   
    //    db.Cives.Add(new Civis
    //    {
    //        Id = CivisDefaultGuid,
    //        Name = "Sempronio"
    //    });

    //    db.Cives.Add(new Civis
    //    {
    //        Id = Civis002Guid,
    //        Name = "Tulio"
    //    });
    //}

    //if (!db.Civitates.Any()) // Se crea una Civitas de ejemplo si la tabla está vacía.
    //{
    //    db.Civitates.Add(new Civitas
    //    {
    //        Id = CivitasDefaultGuid,
    //        Name = "Solfamidas"
    //    });
    //}

    //if (!db.Rogationes.Any()) // Se crea un Rogatio de ejemplo si la tabla está vacía.
    //{
    //    db.Rogationes.Add(new Rogatio
    //    {
    //        Id = Guid.NewGuid(),
    //        Title = "Rogatio primigenia",
    //        Content = "Contenido fundacional de la Civitas.",
    //        CreatedAt = DateTime.UtcNow,
    //        CivisId = CivisDefaultGuid, // Sempronio
    //        //Civis = db.Cives.FirstOrDefault(c => c.Id == Guid.Parse("0f8fad5b - d9cb - 469f - a165 - 70867728950e"))!, // Sempronio
    //        CivitasId = CivitasDefaultGuid, // Solfamidas
    //        //Civitas = db.Civitates.FirstOrDefault(c => c.Id == Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"))!, // Solfamidas
    //    });

    //    db.SaveChanges();
    //}

    DatabaseSeeder.Seed(db);

}

app.Run();
