using CorpusLegis.Shared.Dtos.Reputatio;
using CorpusLegis.Web.State;

namespace CorpusLegis.Web.Clients.Civis;

public class CivisClient : CorpusLegisApiClientBase, ICivisClient
{

    public CivisClient(ILogger<CivisClient> logger, HttpClient httpClient, TokenProvider tokenProvider)
        : base(logger, httpClient, tokenProvider)
    {

    }


    public async Task<ReputatioDto?> GetReputatioAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"civis/{id}/reputatio");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<ReputatioDto>();
    }

}
