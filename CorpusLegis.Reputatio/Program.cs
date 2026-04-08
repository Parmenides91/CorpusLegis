using CorpusLegis.Reputatio.Configuration;
using CorpusLegis.Reputatio.Consumers;
using CorpusLegis.Reputatio.Domain;
using CorpusLegis.Reputatio.Endpoints;
using CorpusLegis.Reputatio.Services;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Se registra MongoDB a partir de la cadena de conexión que proporciona Aspire.
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)); // Registrar GuidSerializer globalmente antes de configurar cualquier conexión
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("mongo-db-corpuslegis");
    return new MongoClient(connectionString);
});

// Se registra MassTransit para consumir eventos.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RogatioCreatedConsumer>();
    x.AddConsumer<SuffragiumEmissumConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConnectionString = builder.Configuration.GetConnectionString("rabbitmq-corpuslegis");
        cfg.Host(rabbitMqConnectionString);
        cfg.ConfigureEndpoints(context);
    });
});

// Se registra el servicio de configuración de reglas de Reputatio.
builder.Services.Configure<ReputatioRulesOptions>(builder.Configuration.GetSection("ReputatioRules"));

// Se registra el servicio de Reputatio.
builder.Services.AddScoped<IReputatioService, ReputatioService>();

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();


// Endpoints de Reputatio:
//app.MapGet("/reputatio/{civisId:guid}", async (Guid civisId, IMongoClient clientMongo) =>
//{
//    var db = clientMongo.GetDatabase("mongo-db-corpuslegis");
//    var collection = db.GetCollection<CivisReputatio>("civis_reputatio"); // ¿colecciones en MongoDB? --> snake_case.
//    var reputatio = await collection.Find(x => x.CivisId == civisId).FirstOrDefaultAsync();
//    return reputatio is not null ? Results.Ok(reputatio) : Results.NotFound();
//});
app.MapReputatioEndpoints();

app.Run();
