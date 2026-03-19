using CorpusLegis.Shared.Dtos;
using CorpusLegis.Web.State;

namespace CorpusLegis.Web.Clients.Rogatio;

public class RogatioClient : CorpusLegisApiClientBase, IRogatioClient
{
    //private readonly ILogger<RogatioClient> _logger;
    //private readonly HttpClient _httpClient;
    //private readonly TokenProvider _tokenProvider;

    public RogatioClient(ILogger<RogatioClient> logger, HttpClient httpClient, TokenProvider tokenProvider)
        : base(logger, httpClient, tokenProvider)
    {
        //_logger = logger;
        //_httpClient = httpClient;
        //_tokenProvider = tokenProvider;

        //if (!string.IsNullOrEmpty(tokenProvider.AccessToken))
        //{
        //    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
        //}
    }


    #region Rogatio
    public async Task<RogatioDetailsDto?> GetRogatioByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/rogatio/{id}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para obtener la lista de rogationes.
    public async Task<List<RogatioSummaryDto>> GetRogationesAsync()
    {
        //ESTE ES EL CORRECTO(SIN USUARIOS)
        var response = await _httpClient.GetAsync("/rogatio");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<RogatioSummaryDto>>() ?? new List<RogatioSummaryDto>();
    }

    // Método para crear un nuevo rogatio.
    public async Task<RogatioDetailsDto?> CreateRogatioAsync(CreateRogatioDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/rogatio", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para actualizar un rogatio existente.
    public async Task<RogatioDetailsDto?> UpdateRogatioAsync(Guid id, UpdateRogatioDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/rogatio/{id}", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para eliminar un rogatio por su ID.
    public async Task<bool> DeleteRogatioAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/rogatio/{id}");
        await HandleNonSuccessResponseAsync(response);
        return true;
    }

    // Método para cambiar el estado de un rogatio (workflow).
    public async Task<RogatioDetailsDto?> ChangeRogatioStatusAsync(Guid id, WorkflowRogatioDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/rogatio/{id}/status", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para evaluar una rogatio.
    public async Task<RogatioDetailsDto?> EvaluateRogatioAsync(Guid id, WorkflowRogatioDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/rogatio/{id}/evaluation", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }
#endregion
}
