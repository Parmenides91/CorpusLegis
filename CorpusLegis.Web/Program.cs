using CorpusLegis.Web.Clients;
using CorpusLegis.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


// Se agrega una referencia al proyecto de CorpusLegis.Shared para poder usar sus servicios
//var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("corpuslegis-api");
// TODO: no me dice Gemini cómo hacerlo. --> se hace desde el SolutionExplorer.


// Se registra el servicio del mockeo del Civis como Scoped (para que sea uno distinto para cada usuario).
builder.Services.AddScoped<CorpusLegis.Web.State.CivisState>();
builder.Services.AddTransient<CivisHeaderHandler>(); // Los DelegatinHandler deben registrarse como Transient.

// Se le indica dónde está la API.
//builder.Services.AddHttpClient<CorpusLegisApiClient>(client =>
//    client.BaseAddress = new Uri("https+http://corpuslegis-api")); // Se agrega el handler al Client que ya usamos para que añada la cabecera con el CivisId a cada petición.

// Se le indica dónde está la API ++ y se agrega el handler al Client que ya usamos para que añada la cabecera con el CivisId a cada petición.
builder.Services.AddScoped<CorpusLegisApiClient>(sp =>
    {
        // Lo sugerido por Copilot:
        //var civisState = builder.Services.BuildServiceProvider().GetRequiredService<CorpusLegis.Web.State.CivisState>();
        //var handler = new CivisHeaderHandler(civisState);
        //var httpClient = new HttpClient(handler)
        //{
        //    BaseAddress = new Uri("https+http://corpuslegis-api")
        //};
        //return new CorpusLegisApiClient(httpClient);

        // Gemini:
        // se obtiene el estado real de la sesión Blazor actual.
        var state = sp.GetRequiredService<CorpusLegis.Web.State.CivisState>();
        //se instancia el Handler pasándole el estado actual.
        var handler = new CorpusLegis.Web.Clients.CivisHeaderHandler(state)
        {
            InnerHandler = new HttpClientHandler() // El Handler que realmente hace la petición HTTP.
        }
        ;
        // se crea el HttpClient atado a este Handler.
        var httpClient = new HttpClient(handler)
        {
            //BaseAddress = new Uri("https+http://corpuslegis-api")
            BaseAddress = new Uri("http://localhost:5208") // Para que funcione en producción, ya que el esquema "https+http" no es reconocido por los navegadores.
        };
        // se devuelve el cliente tipado
        return new CorpusLegis.Web.Clients.CorpusLegisApiClient(httpClient);
    }
    );


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
