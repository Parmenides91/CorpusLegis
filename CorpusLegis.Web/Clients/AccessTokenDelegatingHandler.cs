//using CorpusLegis.Web.State;
//using Microsoft.AspNetCore.Authentication;
//using System.Net.Http.Headers;

//namespace CorpusLegis.Web.Clients;

//public class AccessTokenDelegatingHandler : DelegatingHandler
//{
//    private readonly TokenProvider _tokenProvider;

//    public AccessTokenDelegatingHandler(TokenProvider tokenProvider)
//    {
//        _tokenProvider = tokenProvider;
//    }


//    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//    {
//        Console.WriteLine("TokenProvider length: " + (_tokenProvider.AccessToken?.Length ?? 0));

//        if (!string.IsNullOrEmpty(_tokenProvider.AccessToken))
//        {
//            Console.WriteLine("TokenProvider length: " + (_tokenProvider.AccessToken?.Length ?? 0));
//            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
//            //request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); // Esto ha ido al Program.cs
//        }

//        return await base.SendAsync(request, cancellationToken);
//    }

//}
