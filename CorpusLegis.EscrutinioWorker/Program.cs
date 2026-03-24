using CorpusLegis.EscrutinioWorker;
using System.Net.Sockets;
using Duende.AccessTokenManagement;


var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Gestión del token M2M
var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
var keycloakEscrutinioWorkerClientSecret = builder.Configuration["Keycloak:EscrutinioWorker:ClientSecret"];

if (string.IsNullOrWhiteSpace(keycloakAuthority) || string.IsNullOrWhiteSpace(keycloakEscrutinioWorkerClientSecret))
{
    Environment.Exit(1);
}

builder.Services.AddClientCredentialsTokenManagement()
    .AddClient(ClientCredentialsClientName.Parse("keycloak-m2m"), client =>
    {
        client.TokenEndpoint = new Uri (keycloakAuthority + "/protocol/openid-connect/token");
        client.ClientId = ClientId.Parse("corpuslegis-escrutinio-worker");
        //client.ClientSecret = ClientSecret.Parse("tN7FxsI1QloNMnIrdkQAzs6RAhIazXIO");
        client.ClientSecret = ClientSecret.Parse(keycloakEscrutinioWorkerClientSecret);
    });

//builder.Services.AddHostedService<EscrutinioBatchWorker>();

builder.Services.AddHttpClient("CorpuesLegisApiClient", client =>
{
    //client.BaseAddress = new Uri("http://corpusLegis-api");
    client.BaseAddress = new Uri("http://api-corpuslegis");
})
    .AddClientCredentialsTokenHandler(ClientCredentialsClientName.Parse("keycloak-m2m"));

builder.Services.AddHostedService<EscrutinioBatchWorker>();

var host = builder.Build();
host.Run();
