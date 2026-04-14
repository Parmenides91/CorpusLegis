using CorpusLegis.Shared.Dtos.Lex;
using CorpusLegis.Web.State;

namespace CorpusLegis.Web.Clients.Lex;

public class LexClient : CorpusLegisApiClientBase, ILexClient
{

    public LexClient(ILogger<LexClient> logger, HttpClient httpClient, TokenProvider tokenProvider)
        : base(logger, httpClient, tokenProvider)
    {

    }


    #region Lex
    // Método para obtener una lex por su ID.
    public async Task<LexDetailsDto?> GetLexByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/lex/{id}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<LexDetailsDto>();
    }

    // Método para obtener la lista de leges.
    public async Task<List<LexSummaryDto>> GetLegesAsync()
    {
        var response = await _httpClient.GetAsync("/lex");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();
    }

    // Método para obtener la lista de leges a las que pertenece el Civis actual.
    public async Task<List<LexSummaryDto>> GetLegesByCurrentCivisAsync()
    {
        var response = await _httpClient.GetAsync("/lex/civis/me");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();
    }

    // Método para obtener las leges a las que pertenece un Civis.
    public async Task<List<LexSummaryDto>> GetLegesByCivisIdAsync(Guid civisId)
    {
        var response = await _httpClient.GetAsync($"/lex/civis/{civisId}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();
    }
    #endregion

}
