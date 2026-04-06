using CorpusLegis.Shared.Dtos.Suffragium;
using CorpusLegis.Web.State;

namespace CorpusLegis.Web.Clients.Suffragium;

public class SuffragiumClient : CorpusLegisApiClientBase, ISuffragiumClient
{


    public SuffragiumClient(ILogger<SuffragiumClient> logger, HttpClient httpClient, TokenProvider tokenProvider)
        : base(logger, httpClient, tokenProvider)
    {

    }


    #region Suffragium
    // Método para emitir un voto (suffragium) a un rogatio.
    public async Task<SuffragiumDetailsDto?> CreateSuffragiumAsync(Guid idRogatio, CreateSuffragiumDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"/rogatio/{idRogatio}/vote", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<SuffragiumDetailsDto>();
    }
    #endregion

}
