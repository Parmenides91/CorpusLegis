using CorpusLegis.PdfWorker;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Ya no uso esta clase.
//builder.Services.AddHostedService<PdfGeneratorWorker>();

// Se agrega el consumer para el evento de LexPromulgatedIntegrationEvent.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LexPromulgatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("rabbitmq-corpuslegis"); // mismo nombre que tenga en AppHost.cs o no va a funcionar.
        cfg.Host(connectionString);
        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();
host.Run();
