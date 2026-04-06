using CorpusLegis.Shared.Dtos.Sententia;
using CorpusLegis.Web.State;

namespace CorpusLegis.Web.Clients.Sententia;

public class SententiaClient : CorpusLegisApiClientBase, ISententiaClient
{

    public SententiaClient(ILogger<SententiaClient> logger, HttpClient httpClient, TokenProvider tokenProvider)
    : base(logger, httpClient, tokenProvider)
    {
        
    }


    public async Task<List<SententiaDto>> GetAllByRogatioIdAsync(Guid rogatioId)
    {
        var response = await _httpClient.GetAsync($"/sententia/rogatio/{rogatioId}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<SententiaDto>>() ?? new List<SententiaDto>();
    }


    public async Task<CreateSententiaDto?> CreateSententiaAsync(CreateSententiaDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/sententia", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<CreateSententiaDto>();
    }


    public async Task<UpdateSententiaDto?> UpdateSententiaAsync(Guid id, UpdateSententiaDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/sententia/{id}", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<UpdateSententiaDto>();
    }


    public async Task<bool> SoftDeleteSententiaAsync(Guid id)
    {
        var respose = await _httpClient.DeleteAsync($"/sententia/{id}");
        await HandleNonSuccessResponseAsync(respose);
        return true;
    }


    public async Task<bool> RestoreSententiaAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"/sententia/{id}/restore", null);
        await HandleNonSuccessResponseAsync(response);
        return true;
    }

}
