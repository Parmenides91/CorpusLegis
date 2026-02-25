using CorpusLegis.PdfWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<PdfGeneratorWorker>();

var host = builder.Build();
host.Run();
