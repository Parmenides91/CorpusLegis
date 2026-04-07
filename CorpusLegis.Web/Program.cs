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

// Se le indica dónde está la API.
builder.Services.AddCorpusLegisApiClients("http://api-corpuslegis");

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

app.UseRouting(); // recomendado antes de Auth.

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints(); // Rutas de autentificación

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<CorpusLegis.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
