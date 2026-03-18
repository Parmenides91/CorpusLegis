using CorpusLegis.Web.State;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace CorpusLegis.Web.Clients;

public class AccessTokenDelegatingHandler : DelegatingHandler
{
    //private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TokenProvider _tokenProvider;

    public AccessTokenDelegatingHandler(/*IHttpContextAccessor httpContextAccessor*/ TokenProvider tokenProvider)
    {
        //_httpContextAccessor = httpContextAccessor;
        _tokenProvider = tokenProvider;
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //var context = _httpContextAccessor.HttpContext;

        //if (context != null)
        //{
        //    var token = await context.GetTokenAsync("access_token"); 

        //    Console.WriteLine("DEBUG token length: " + (token?.Length ?? 0));

        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    }
        //}

        //return await base.SendAsync(request, cancellationToken);

        Console.WriteLine("TokenProvider length: " + (_tokenProvider.AccessToken?.Length ?? 0));
        if (!string.IsNullOrEmpty(_tokenProvider.AccessToken))
        {
            Console.WriteLine("TokenProvider length: " + (_tokenProvider.AccessToken?.Length ?? 0));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

}
