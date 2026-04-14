using CorpusLegis.Reputatio.Configuration;
using CorpusLegis.Reputatio.Consumers;
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

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapReputatioEndpoints();

app.Run();
