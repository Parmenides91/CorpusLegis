using CorpusLegis.Shared.Dtos.Invitatio;
using CorpusLegis.Web.State;

namespace CorpusLegis.Web.Clients.Invitatio;

public class InvitatioClient : CorpusLegisApiClientBase, IInvitatioClient
{

    public InvitatioClient(ILogger<InvitatioClient> logger, HttpClient httpClient, TokenProvider tokenProvider)
        : base(logger, httpClient, tokenProvider)
    {
    }


    #region emisores
    public async Task<InvitatioDetailsDto?> SendInvitatioAsync(CreateInvitatioDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/invitatio", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<InvitatioDetailsDto>();
    }

    public async Task<bool> RevokeInvitatioAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"/invitatio/{id}/revoke", null);
        await HandleNonSuccessResponseAsync(response);
        return true;
    }

    public async Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCivitasAsync(Guid civitasId)
    {
        var response = await _httpClient.GetAsync($"/invitatio/civitas/{civitasId}/pending");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<InvitatioDetailsDto>>() ?? new List<InvitatioDetailsDto>();
    }
    #endregion



    #region receptores
    public async Task<List<InvitatioDetailsDto>> GetMyPendingInvitationesAsync()
    {
        var response = await _httpClient.GetAsync("/invitatio/me/pending");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<InvitatioDetailsDto>>() ?? new List<InvitatioDetailsDto>();
    }

    public async Task<bool> AcceptInvitatioAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"/invitatio/{id}/accept", null);
        await HandleNonSuccessResponseAsync(response);
        return true;
    }

    public async Task<bool> RejectInvitatioAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"/invitatio/{id}/reject", null);
        await HandleNonSuccessResponseAsync(response);
        return true;
    }
    #endregion
}
