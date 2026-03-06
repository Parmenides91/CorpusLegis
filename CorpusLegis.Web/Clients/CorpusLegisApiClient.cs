using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Dtos.Civitas;
using CorpusLegis.Shared.Dtos.Lex;
using CorpusLegis.Shared.Dtos.Suffragium;
using CorpusLegis.Shared.Validators;
using CorpusLegis.Web.State;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CorpusLegis.Web.Clients;

public class CorpusLegisApiClient
{

    private readonly HttpClient _httpClient;
    private readonly CivisState _civisState;

    public CorpusLegisApiClient(HttpClient httpClient, CivisState civisState)
    {
        _httpClient = httpClient;
        _civisState = civisState;

        _httpClient.DefaultRequestHeaders.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
    }


    // Aquí irán los métodos para interactuar con la API de CorpusLegis. Se pueden agregar métodos para obtener datos, enviar datos, etc.


    #region Rogatio
    // Método para obtener un rogatio por su ID.
    public async Task<RogatioDetailsDto?> GetRogatioByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/rogatio/{id}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/rogatio/{id}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para obtener la lista de rogationes.
    public async Task<List<RogatioSummaryDto>> GetRogationesAsync()
    {
        var response = await _httpClient.GetAsync("/rogatio");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<RogatioSummaryDto>>() ?? new List<RogatioSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/rogatio");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<RogatioSummaryDto>>() ?? new List<RogatioSummaryDto>();
    }

    // Método para crear un nuevo rogatio.
    public async Task<RogatioDetailsDto?> CreateRogatioAsync(CreateRogatioDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/rogatio", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Post, $"/rogatio");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //request.Content = JsonContent.Create(dto);
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para actualizar un rogatio existente.
    public async Task<RogatioDetailsDto?> UpdateRogatioAsync(Guid id, UpdateRogatioDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/rogatio/{id}", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Put, $"/rogatio/{id}"); 
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //request.Content = JsonContent.Create(dto);
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para eliminar un rogatio por su ID.
    public async Task<bool> DeleteRogatioAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/rogatio/{id}");
        await HandleNonSuccessResponseAsync(response);
        return true;

        //var request = new HttpRequestMessage(HttpMethod.Delete, $"/rogatio/{id}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return true;
    }

    // Método para cambiar el estado de un rogatio (workflow).
    public async Task<RogatioDetailsDto?> ChangeRogatioStatusAsync(Guid id, WorkflowRogatioDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/rogatio/{id}/status", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Put, $"/rogatio/{id}/status");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //request.Content = JsonContent.Create(dto);
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para evaluar una rogatio.
    public async Task<RogatioDetailsDto?> EvaluateRogatioAsync(Guid id, WorkflowRogatioDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/rogatio/{id}/evaluation", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Put, $"/rogatio/{id}/evaluation");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //request.Content = JsonContent.Create(dto);
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }
    #endregion


    #region Suffragium
    // Método para emitir un voto (suffragium) a un rogatio.
    public async Task<SuffragiumDetailsDto?> CreateSuffragiumAsync(Guid idRogatio, CreateSuffragiumDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"/rogatio/{idRogatio}/vote", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<SuffragiumDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Post, $"/rogatio/{idRogatio}/vote");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //request.Content = JsonContent.Create(dto);
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<SuffragiumDetailsDto>();
    }
    #endregion



    #region Lex
    // Método para obtener una lex por su ID.
    public async Task<LexDetailsDto?> GetLexByIdAsync(Guid id)
    {
        //var response = await _httpClient.GetFromJsonAsync<LexDetailsDto>($"/lex/{id}");
        //return response;

        var response = await _httpClient.GetAsync($"/lex/{id}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<LexDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/lex/{id}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<LexDetailsDto>();
    }

    // Método para obtener la lista de leges.
    public async Task<List<LexSummaryDto>> GetLegesAsync()
    {
        //var response = await _httpClient.GetFromJsonAsync<List<LexSummaryDto>>("/lex");
        //return response ?? new List<LexSummaryDto>();

        var response = await _httpClient.GetAsync("/lex");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/lex");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();
    }

    // Método para obtener la lista de leges a las que pertenece el Civis actual.
    public async Task<List<LexSummaryDto>> GetLegesByCurrentCivisAsync()
    {
        var response = await _httpClient.GetAsync("/lex/civis/me");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/lex/civis/me");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();
    }

    // Método para obtener las leges a las que pertenece un Civis.
    public async Task<List<LexSummaryDto>> GetLegesByCivisIdAsync(Guid civisId)
    {
        var response = await _httpClient.GetAsync($"/lex/civis/{civisId}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/lex/civis/{civisId}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<LexSummaryDto>>() ?? new List<LexSummaryDto>();
    }
    #endregion



    #region Civitas
    // Método para obtener una civitas por su ID.
    public async Task<CivitasDetailsDto?> GetCivitasByIdAsync(Guid id)
    {
        //var response = await _httpClient.GetFromJsonAsync<CivitasDetailsDto>($"/civitas/{id}");
        //return response;

        var response = await _httpClient.GetAsync($"/civitas/{id}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/{id}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();
    }

    // Método para obtener la lista de Civitates.
    public async Task<List<CivitasSummaryDto>> GetCivitatesAsync()
    {
        //var response = await _httpClient.GetFromJsonAsync<List<CivitasSummaryDto>>("/civitas");
        //return response ?? new List<CivitasSummaryDto>();

        var response = await _httpClient.GetAsync("/civitas");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para obtener la lista de Civitates a las que pertenece el Civis actual.
    public async Task<List<CivitasSummaryDto>> GetUserCivitatesAsync()
    {
        //var response = await _httpClient.GetFromJsonAsync<List<CivitasSummaryDto>>("/civitas/civis/me");
        //return response ?? new List<CivitasSummaryDto>();

        var response = await _httpClient.GetAsync("/civitas/me");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/me");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para obtener las Civitates a las que pertenece un Civis.
    public async Task<List<CivitasSummaryDto>> GetCivitatesByCivisIdAsync(Guid idCivis)
    {
        //var response = await _httpClient.GetFromJsonAsync<List<CivitasSummaryDto>>($"/civitas/civis/{idCivis}");
        //return response ?? new List<CivitasSummaryDto>();

        var response = await _httpClient.GetAsync($"/civitas/civis/{idCivis}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/civis/{idCivis}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para agregar el Civis actual al Civitas.
    public async Task JoinCurrentCivisToCivitasAsync(Guid idCivitas)
    {
        var response = await _httpClient.PostAsync($"/civitas/{idCivitas}/members/me", null);
        await HandleNonSuccessResponseAsync(response);

        //var request = new HttpRequestMessage(HttpMethod.Post, $"/civitas/{idCivitas}/members/me");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
    }

    // Método para agregar un Civis a un Civitas.
    public async Task JoinCivisToCivitasAsync(Guid idCivitas, Guid idCivis)
    {
        var response = await _httpClient.PostAsync($"/civitas/{idCivitas}/members/{idCivis}", null);
        await HandleNonSuccessResponseAsync(response);

        //var request = new HttpRequestMessage(HttpMethod.Post, $"/civitas/{idCivitas}/members/{idCivis}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
    }

    // Método para eliminar el Civis actual del Civitas.
    public async Task LeaveCurrentCivisFromCivitasAsync(Guid idCivitas)
    {
        var response = await _httpClient.DeleteAsync($"/civitas/{idCivitas}/members/me");
        await HandleNonSuccessResponseAsync(response);

        //var request = new HttpRequestMessage(HttpMethod.Delete, $"/civitas/{idCivitas}/members/me");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
    }

    // Método para eliminar un Civis de un Civitas.
    public async Task LeaveCivisFromCivitasAsync(Guid idCivitas, Guid idCivis)
    {
        var response = await _httpClient.DeleteAsync($"/civitas/{idCivitas}/members/{idCivis}");
        await HandleNonSuccessResponseAsync(response);

        //var request = new HttpRequestMessage(HttpMethod.Delete, $"/civitas/{idCivitas}/members/{idCivis}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
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
