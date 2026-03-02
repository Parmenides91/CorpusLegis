using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Dtos.Civitas;
using CorpusLegis.Shared.Dtos.Lex;
using CorpusLegis.Shared.Dtos.Suffragium;
using CorpusLegis.Shared.Validators;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CorpusLegis.Web.Clients;

public class CorpusLegisApiClient
{

    private readonly HttpClient _httpClient;

    public CorpusLegisApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    // Aquí irán los métodos para interactuar con la API de CorpusLegis. Se pueden agregar métodos para obtener datos, enviar datos, etc.


    #region Rogatio
    // Método para obtener un rogatio por su ID.
    public async Task<RogatioDetailsDto?> GetRogatioByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/rogatio/{id}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para obtener la lista de rogationes.
    public async Task<List<RogatioSummaryDto>> GetRogationesAsync()
    {
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
    #endregion


    #region Suffragium
    // Método para emitir un voto (suffragium) a un rogatio.
    public async Task<SuffragiumDetailsDto?> CreateSuffragiumAsync(Guid idRogatio, CreateSuffragiumDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"/rogatio/{idRogatio}/vote", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<SuffragiumDetailsDto>();
    }
    #endregion



    #region Lex
    // Método para obtener una lex por su ID.
    public async Task<LexDetailsDto?> GetLexByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<LexDetailsDto>($"/lex/{id}");
        return response;
    }

    // Método para obtener la lista de leges.
    public async Task<List<LexSummaryDto>> GetLegesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<LexSummaryDto>>("/lex");
        return response ?? new List<LexSummaryDto>();
    }
    #endregion



    #region Civitas
    // Método para obtener una civitas por su ID.
    public async Task<CivitasDetailsDto?> GetCivitasByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<CivitasDetailsDto>($"/civitas/{id}");

        return response;
    }

    // Método para obtener la lista de civitates.
    public async Task<List<CivitasSummaryDto>> GetCivitatesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<CivitasSummaryDto>>("/civitas");

        return response ?? new List<CivitasSummaryDto>();
    }
    #endregion



    #region Excepciones
    private async Task HandleNonSuccessResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true } );

            if (problemDetails != null && !string.IsNullOrEmpty(problemDetails.Detail))
            {
                throw new ApplicationException(problemDetails.Detail);
            }
        }
        catch (JsonException)
        {
            throw;
        }

        throw new ApplicationException($"Error HTTP {response.StatusCode}.");
    }
    #endregion


}
