using CorpusLegis.Web.Clients;
using CorpusLegis.Web.Clients.Civitas;
using CorpusLegis.Web.Clients.Lex;
using CorpusLegis.Web.Clients.Rogatio;
using CorpusLegis.Web.Clients.Suffragium;
using CorpusLegis.Web.Components;
using CorpusLegis.Web.Endpoints;
using CorpusLegis.Web.Extensions;
using CorpusLegis.Web.State;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
if (string.IsNullOrEmpty(keycloakAuthority))
{
    throw new InvalidOperationException("La variable 'Keycloak:Authority' no se ha inyectado correctamente desde el AppHost.");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        //options.Cookie.Name = "__Host-CorpusLegis";
        //options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = "CorpusLegis.Auth";
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = keycloakAuthority;
        options.ClientId = builder.Configuration["Keycloak:ClientId"] ?? "corpuslegis-web"; // TODO: ¿no debería ser corpuslegis-web-blazor?
        options.ClientSecret = builder.Configuration["keycloak:ClientSecret"];
        options.ResponseType = OpenIdConnectResponseType.Code;

        options.SaveTokens = true; // Fundamental para recuperar el Access Token después.
        options.RequireHttpsMetadata = false; // en PRO esto tendrá que ser true.

        // Scopes stándard de OpenId.
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");

        // TODO: implementar un mecanismo de refresco de tokens para evitar que el usuario tenga que volver a loguearse cada vez que expire el Access Token.
        options.Scope.Add("offline_access"); // para obtener refresh token.
        options.SaveTokens = true; // para guardar el refresh token también.

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            NameClaimType = "preferred_username",
            RoleClaimType = "realm_access.roles"
        };
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("OIDC RedirectUri: {uri}", context.ProtocolMessage.RedirectUri);
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();


// Se registra el handler de la autentificación.
builder.Services.AddScoped<CorpusLegis.Web.State.TokenProvider>();

//builder.Services.AddTransient<AccessTokenDelegatingHandler>(); // el que se encarga de añadir el token a las peticiones HTTP hacia la API (debe ser transient).

// Se le indica dónde está la API.
//builder.Services.AddHttpClient<CorpusLegisApiClient>(client =>
//{
//    client.BaseAddress = new Uri("http://api-corpuslegis");
//})
//    //.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
//    .AddHttpMessageHandler<AccessTokenDelegatingHandler>() // se inyecta el nuevo handler.
//    ;

// Se registran los Clients de la API para la Web.
//builder.Services.AddHttpClient<ICivitasClient, CivitasClient>("civitas-client", client =>
//{
//    client.BaseAddress = new Uri("http://api-corpuslegis");
//    client.DefaultRequestHeaders.Accept.Clear();
//    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//})
//    //.AddHttpMessageHandler<AccessTokenDelegatingHandler>() // inyección del handler para añadir el token a las peticiones HTTP hacia la API.
//    ;
//builder.Services.AddHttpClient<IRogatioClient, RogatioClient>("rogatio-client", client =>
//{
//    client.BaseAddress = new Uri("http://api-corpuslegis");
//    client.DefaultRequestHeaders.Accept.Clear();
//    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//})
//    //.AddHttpMessageHandler<AccessTokenDelegatingHandler>() // inyección del handler para añadir el token a las peticiones HTTP hacia la API.
//    ;
//builder.Services.AddHttpClient<ISuffragiumClient, SuffragiumClient>("suffragium-client", client =>
//{
//    client.BaseAddress = new Uri("http://api-corpuslegis");
//    client.DefaultRequestHeaders.Accept.Clear();
//    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//})
//    //.AddHttpMessageHandler<AccessTokenDelegatingHandler>() // inyección del handler para añadir el token a las peticiones HTTP hacia la API.
//    ;
//builder.Services.AddHttpClient<ILexClient, LexClient>("lex-client", client =>
//{
//    client.BaseAddress = new Uri("http://api-corpuslegis");
//    client.DefaultRequestHeaders.Accept.Clear();
//    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//})
//    //.AddHttpMessageHandler<AccessTokenDelegatingHandler>() // inyección del handler para añadir el token a las peticiones HTTP hacia la API.
//    ;
builder.Services.AddCorpusLegisApiClients("http://api-corpuslegis");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

//app.MapGet("/login", (string? returnUrl, HttpContext context) =>
//{
//    return TypedResults.Challenge(
//        new Microsoft.AspNetCore.Authentication.AuthenticationProperties
//        {
//            RedirectUri = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl
//        });
//});
//app.MapPost("/logout", (HttpContext context) =>
//{
//    return TypedResults.SignOut(
//        new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/" },
//        [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
//});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseRouting(); // recomendado antes de Auth.

//app.Use(async (ctx, next) =>
//{
//    var logger = ctx.RequestServices.GetRequiredService<ILogger<Program>>();
//    logger.LogInformation("Incoming Authorization WEB: {Auth}", ctx.Request.Headers["Authorization"].ToString());
//    await next();
//});

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints(); // Rutas de autentificación

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
