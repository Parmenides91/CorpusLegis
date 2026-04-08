using CorpusLegis.Web.Clients;
using CorpusLegis.Web.Clients.Civis;
using CorpusLegis.Web.Clients.Civitas;
using CorpusLegis.Web.Clients.Invitatio;
using CorpusLegis.Web.Clients.Lex;
using CorpusLegis.Web.Clients.Rogatio;
using CorpusLegis.Web.Clients.Sententia;
using CorpusLegis.Web.Clients.Suffragium;

namespace CorpusLegis.Web.Extensions;

public static class ApiClientServiceCollectionExtensions
{
    public static IServiceCollection AddCorpusLegisApiClients(this IServiceCollection services, string baseUri)
    {

        // services.AddTransient<AccessTokenDelegatingHandler>(); // NO, porque ya hemos visto que los tokens con delegados no progresan en las peticiones teniendo IHttpsClientFactory.

        Action<HttpClient> configureClient = client =>
        {
            client.BaseAddress = new Uri(baseUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        };


        services.AddHttpClient<IRogatioClient, RogatioClient>("rogatio-client", configureClient);
        services.AddHttpClient<ICivitasClient, CivitasClient>("civitas-client", configureClient);
        services.AddHttpClient<ISuffragiumClient, SuffragiumClient>("suffragium-client", configureClient);
        services.AddHttpClient<ILexClient, LexClient>("lex-client", configureClient);
        services.AddHttpClient<IInvitatioClient, InvitatioClient>("invitatio-client", configureClient);
        services.AddHttpClient<ISententiaClient, SententiaClient>("sententia-client", configureClient);
        services.AddHttpClient<ICivisClient, CivisClient>("civis-client", configureClient);

        return services;

    }

}
