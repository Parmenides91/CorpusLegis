using CorpusLegis.EscrutinioWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<EscrutinioBatchWorker>();

builder.Services.AddHttpClient("CorpuesLegisApiClient", client =>
{
    //client.BaseAddress = new Uri("http://corpusLegis-api");
    client.BaseAddress = new Uri("http://api-corpuslegis");
});

builder.Services.AddHostedService<EscrutinioBatchWorker>();


var host = builder.Build();
host.Run();
