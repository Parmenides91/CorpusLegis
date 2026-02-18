var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CorpusLegis_API>("corpuslegis-api");

builder.AddProject<Projects.CorpusLegis_Web>("corpuslegis-web");

builder.Build().Run();
