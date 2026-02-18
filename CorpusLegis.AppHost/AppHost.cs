var builder = DistributedApplication.CreateBuilder(args);


// Se agrega el proyecto de la API
var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("corpuslegis-api");

// Se agrega el proyecto del WebApp
builder.AddProject<Projects.CorpusLegis_Web>("corpuslegis-web")
        .WithExternalHttpEndpoints()
        .WithReference(api_corpuslegis);

builder.Build().Run();
