using CorpusLegis.Web.Clients;
using CorpusLegis.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


// Se agrega una referencia al proyecto de CorpusLegis.Shared para poder usar sus servicios
//var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("corpuslegis-api");
// TODO: no me dice Gemini cómo hacerlo. --> se hace desde el SolutionExplorer.


// Se le indica dónde está la API.
builder.Services.AddHttpClient<CorpusLegisApiClient>(client =>
    client.BaseAddress = new Uri("https+http://corpuslegis-api"));


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
