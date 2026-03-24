
using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Endpoints;
using CorpusLegis.API.Infrastructure;
using CorpusLegis.API.Infrastructure.Security;
using CorpusLegis.API.Infrastructure.Seeders;
using CorpusLegis.API.Services;
using CorpusLegis.API.Validators;
using CorpusLegis.Shared.Dtos;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Trace;
using System.Collections;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.IdentityModel.Tokens", LogLevel.Debug);


// Se agrega una referencia al proyecto de CorpusLegis.Shared para poder usar sus servicios
//var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("corpuslegis-api");
// TODO: no me dice Gemini cómo hacerlo. --> se hace desde el SolutionExplorer.

// Add services to the container.

//builder.Services.AddMemoryCache(); // necesario para el servicio de aprovisionamiento de usuarios (UserProvisioningService) que usa IMemoryCache.
builder.AddRedisDistributedCache("redis-corpuslegis"); // necesario para el servicio de aprovisionamiento de usuarios (UserProvisioningService) que usa DDD.
builder.Services.AddScoped<IUserProvisioningService, UserProvisioningService>(); // se registra el servicio de aprovisionamiento de Civis.

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


var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
//var keycloakAuthority = "http://localhost:8080/realms/CorpusLegis";
if (string.IsNullOrEmpty(keycloakAuthority))
{
    throw new InvalidOperationException("La variable de entorno 'Keycloak:Authority' no se ha inyectado correctamente desde el AppHost.");
}
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuers = new[]
            {
                keycloakAuthority,
                "http://localhost:8080/realms/CorpusLegis"
            },
    ValidateAudience = false, // Habrá que ajustarlo en PRO.
    ValidAudiences = new[]
    {
        "account", // para aceptar el token de acceso que Keycloak emite para su propia consola de administración, lo cual es útil para pruebas.
        "corpuslegis-api" // el nombre del cliente que hay en Keycloak para esta API.
    },
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    RequireSignedTokens = true,
    ClockSkew = TimeSpan.FromMinutes(30), // TODO: este valor debe ser menor.
    NameClaimType = "preferred_username"//, // Para que el nombre de usuario sea el que viene del token de Keycloak.
    //RoleClaimType = "realm_access.roles" // Para que los roles sean los que vienen del token de Keycloak.
};
builder.Services.AddAuthentication( options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer( options =>
    {
        //options.Authority = $"{keycloakUrl}/realms/CorpusLegis";
        //options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Authority = keycloakAuthority;
        options.RequireHttpsMetadata = false; // en PRO esto tendrá que ser true.

        options.MapInboundClaims = false; // para que no me cambie los nombres de los claims (por ejemplo, "preferred_username" a "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").
        options.IncludeErrorDetails = true;

        options.MetadataAddress = $"{keycloakAuthority}/.well-known/openid-configuration";
        options.TokenValidationParameters = tokenValidationParameters;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader))
                {
                    logger.LogWarning("[API SEGURIDAD] Cabecera Authorization ausente.");
                }
                else
                {
                    // Solo imprimimos los primeros 10 caracteres para verificar que es un JWT (debería empezar por "Bearer ey...")
                    var preview = authHeader.Length > 17 ? authHeader.Substring(0, 17) : "Inválido";
                    logger.LogInformation("[API SEGURIDAD] Cabecera Authorization recibida. Formato: {Preview}...", preview);
                    logger.LogInformation("[API SEGURIDAD] Authorization header preview: {Preview}", authHeader.Length > 50 ? authHeader.Substring(0, 50) : authHeader);

                    // Validación manual para depuración
                    //var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7) : authHeader;
                    //var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    //try
                    //{
                    //    var principal = handler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                    //    logger.LogInformation("[API DEBUG] Manual ValidateToken OK. Subject: {sub}", principal?.Identity?.Name);
                    //}
                    //catch (Exception ex)
                    //{
                    //    logger.LogError(ex, "[API DEBUG] Manual ValidateToken error: {Message}", ex.Message);
                    //}
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError("[API CRÍTICO] Fallo de validación JWT: {ExceptionMessage}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("[API DEBUG] Token JWT validado por middleware.");
                Console.WriteLine("[API DEBUG] Token JWT validado con éxito.");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError("[API CRÍTICO] Challenge emitido. Error: {Error}, Detalle: {ErrorDescription}", context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            }
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

builder.Services.AddHttpContextAccessor(); // Necesario para que CurrentUserService pueda acceder al contexto HTTP.
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>(); // Servicio para obtener el usuario actual (Civis) a partir del contexto HTTP

// Se registra el servicio de Excepciones.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Se registra el servicio de validaciones.
builder.Services.AddValidatorsFromAssemblyContaining<CreateRogatioDtoValidator>();

// se agrega la transformación de Claims.
builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>(); // para la Autorización Basada en Roles (RBAC)

var app = builder.Build();

/* Manejo de excepciones y redirección (early pipeline) */
// Manejador para Excepciones.
app.UseExceptionHandler();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
else
{
    //app.UseDeveloperExceptionPage();
    //app.UseHttpsRedirection(); // NO EN DESARROLLO (SÍ LO TENDREMOS EN CUALQUIER OTRO ENTORNO)
}

//app.Use(async (ctx, next) =>
//{
//    var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
//    logger.LogInformation("Incoming Authorization API: {Auth}", ctx.Request.Headers["Authorization"].ToString());
//    await next();
//});

/* Autentificación y autorización (middle pipeline) */
// Se habilita la autenticación y autorización.
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserProvisioningMiddleware>(); // Middleware personalizado para aprovisionar el Civis en la base de datos si el usuario autenticado no existe aún.

/* Mapeo de endpoints (late pipeline) */
app.MapDefaultEndpoints();
app.MapRogatioEndpoints(); // Se mapean los endpoints de Rogatio.
app.MapSuffragiumEndpoints(); // Se mapean los endpoints de Suffragium.
app.MapLexEndpoints(); // Se mapean los endpoints de Lex.
app.MapCivitasEndpoints(); // Se mapean los endpoints de Civitas.
//app.MapGet("/auth/token", async (HttpContext ctx) =>
//{
//    var token = await ctx.GetTokenAsync("access_token");
//    return token is null ? Results.Unauthorized() : Results.Ok(new { access_token = token });
//}).RequireAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.Use(async (ctx, next) =>
//{
//    var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
//    foreach (var h in ctx.Request.Headers)
//    {
//        logger.LogInformation("[INCOMING HEADER] {Name}: {Value}", h.Key, h.Value.ToString().Length > 200 ? h.Value.ToString().Substring(0, 200) + "..." : h.Value.ToString());
//    }
//    await next();
//});

//app.MapControllers();

/* Migraciones y seeding */
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CorpusLegisContext>();
    db.Database.Migrate();
    // cada vez que hagas una modificación del Domain debes hacer:
    // 1) situarte en el proyecto CorpusLegis.API con la Package Manager Console.
    // 2) ejecutar "dotnet ef migrations add NombreDeLaMigración".
    // 3) mirar ^^ (al lanzar la app las migraciones se irán aplicando en orden).
    DatabaseSeeder.Seed(db); // Mi clase propia con datos de inicio.
}

app.Run();
