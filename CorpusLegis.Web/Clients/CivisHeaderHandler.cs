//using CorpusLegis.Web.State;

//namespace CorpusLegis.Web.Clients;

//public class CivisHeaderHandler : DelegatingHandler
//{
    //private readonly CivisState _civisState;

    //public CivisHeaderHandler(CivisState civisState)
    //{
    //    _civisState = civisState;
    //}

    //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //{
    //    request.Headers.Remove("X-Civis-Id");
    //    request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());

    //    return await base.SendAsync(request, cancellationToken);
    //}

//}
