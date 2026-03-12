using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Dtos.Civitas;
using CorpusLegis.Shared.Dtos.Lex;
using CorpusLegis.Shared.Dtos.Suffragium;
using CorpusLegis.Shared.Validators;
using CorpusLegis.Web.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace CorpusLegis.Web.Clients;

public class CorpusLegisApiClient
{

    private readonly HttpClient _httpClient;
    private readonly TokenProvider _tokenProvider;

    public CorpusLegisApiClient(HttpClient httpClient, TokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;

        if (!string.IsNullOrEmpty(tokenProvider.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
        }

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
        //ESTE ES EL CORRECTO(SIN USUARIOS)
        var response = await _httpClient.GetAsync("/rogatio");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<RogatioSummaryDto>>() ?? new List<RogatioSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/rogatio");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<RogatioSummaryDto>>() ?? new List<RogatioSummaryDto>();

        //if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        //{
        //    throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo o está vacío en el momento de la petición HTTP. El ciclo de vida de Blazor no está persistiendo el estado o la sesión ha expirado.");
        //}
        //var request = new HttpRequestMessage(HttpMethod.Get, $"/rogatio");
        //request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
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

        // ESTE ES EL CORRECTO (SIN USUARIOS)
        //var response = await _httpClient.GetAsync($"/civitas/{id}");
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/{id}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();

        if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        {
            throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo o está vacío en el momento de la petición HTTP. El ciclo de vida de Blazor no está persistiendo el estado o la sesión ha expirado.");
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/{id}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();
    }

    // Método para obtener la lista de Civitates.
    public async Task<List<CivitasSummaryDto>> GetCivitatesAsync()
    {
        //var response = await _httpClient.GetFromJsonAsync<List<CivitasSummaryDto>>("/civitas");
        //return response ?? new List<CivitasSummaryDto>();

        // ESTE ES EL CORRECTO (SIN USUARIOS)
        //var response = await _httpClient.GetAsync("/civitas");
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        // este es el que debe funcionar con usuarios
        //if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        //{
        //    throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo o está vacío en el momento de la petición HTTP. El ciclo de vida de Blazor no está persistiendo el estado o la sesión ha expirado.");
        //}
        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas");
        //request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        {
            throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo.");
        }

        // Comprobación de rigor: Un JWT debe tener tres partes separadas por puntos.
        if (!_tokenProvider.AccessToken.Contains('.'))
        {
            throw new InvalidOperationException($"[CRÍTICO] El token obtenido de Keycloak no es un JWT válido. Valor recibido: {_tokenProvider.AccessToken}");
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);

        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);

        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

    }

    // Método para obtener la lista de Civitates a las que pertenece el Civis actual.
    public async Task<List<CivitasSummaryDto>> GetUserCivitatesAsync()
    {
        //var response = await _httpClient.GetFromJsonAsync<List<CivitasSummaryDto>>("/civitas/civis/me");
        //return response ?? new List<CivitasSummaryDto>();

        // ESTE ES EL CORRECTO (SIN USUARIOS)
        //var response = await _httpClient.GetAsync("/civitas/me");
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/me");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        // este es el que debe funcionar con usuarios
        //if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        //{
        //    throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo o está vacío en el momento de la petición HTTP. El ciclo de vida de Blazor no está persistiendo el estado o la sesión ha expirado.");
        //}
        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/civis/me");
        //request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        {
            throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo.");
        }

        // Comprobación de rigor: Un JWT debe tener tres partes separadas por puntos.
        if (!_tokenProvider.AccessToken.Contains('.'))
        {
            throw new InvalidOperationException($"[CRÍTICO] El token obtenido de Keycloak no es un JWT válido. Valor recibido: {_tokenProvider.AccessToken}");
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);

        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);

        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para obtener las Civitates a las que pertenece un Civis.
    public async Task<List<CivitasSummaryDto>> GetCivitatesByCivisIdAsync(Guid idCivis)
    {
        //var response = await _httpClient.GetFromJsonAsync<List<CivitasSummaryDto>>($"/civitas/civis/{idCivis}");
        //return response ?? new List<CivitasSummaryDto>();

        // ESTE ES EL CORRECTO (SIN USUARIOS)
        //var response = await _httpClient.GetAsync($"/civitas/civis/{idCivis}");
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/civis/{idCivis}");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);
        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();

        if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        {
            throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo o está vacío en el momento de la petición HTTP. El ciclo de vida de Blazor no está persistiendo el estado o la sesión ha expirado.");
        }
        var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/civis/{idCivis}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para agregar el Civis actual al Civitas.
    public async Task JoinCurrentCivisToCivitasAsync(Guid idCivitas)
    {
        // ESTE ES EL CORRECTO (SIN USUARIOS)
        //var response = await _httpClient.PostAsync($"/civitas/{idCivitas}/members/me", null);
        //await HandleNonSuccessResponseAsync(response);

        //var request = new HttpRequestMessage(HttpMethod.Post, $"/civitas/{idCivitas}/members/me");
        //request.Headers.Add("X-Civis-Id", _civisState.CurrentCivisId.ToString());
        //var response = await _httpClient.SendAsync(request);
        //await HandleNonSuccessResponseAsync(response);

        if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        {
            throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo o está vacío en el momento de la petición HTTP. El ciclo de vida de Blazor no está persistiendo el estado o la sesión ha expirado.");
        }
        var request = new HttpRequestMessage(HttpMethod.Post, $"/civitas/{idCivitas}/members/me");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);
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
        //if (response.IsSuccessStatusCode)
        //{
        //    return;
        //}

        //var content = await response.Content.ReadAsStringAsync();

        //// TODO: ¿estos casos no pueden ir dentro de mis Excepciones?
        //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //{
        //    throw new UnauthorizedAccessException("El acceso a este recurso requiere estar autentificado.");
        //}

        //if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        //{
        //    throw new UnauthorizedAccessException("No tienes permisos suficientes para realizar esta acción.");
        //}


        //try
        //{
        //    var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true } );

        //    if (problemDetails != null && !string.IsNullOrEmpty(problemDetails.Detail))
        //    {
        //        throw new ApplicationException(problemDetails.Detail);
        //    }
        //}
        //catch (JsonException)
        //{
        //    throw;
        //}

        //throw new ApplicationException($"Error HTTP {response.StatusCode}.");

        var status = (int)response.StatusCode;
        var body = response.Content == null ? null : await response.Content.ReadAsStringAsync();

        Console.WriteLine($"DEBUG: API response {status} {response.ReasonPhrase}");
        Console.WriteLine("DEBUG: Content-Type: " + (response.Content?.Headers.ContentType?.ToString() ?? "<none>"));
        Console.WriteLine("DEBUG: WWW-Authenticate: " + string.Join(";", response.Headers.WwwAuthenticate.Select(h => h.ToString())));
        Console.WriteLine("DEBUG: Body length: " + (body?.Length ?? 0));
        Console.WriteLine("DEBUG: Body preview: " + (string.IsNullOrEmpty(body) ? "<empty>" : body.Substring(0, Math.Min(400, body.Length))));

        if (status == 401)
            throw new UnauthorizedAccessException("El acceso a este recurso requiere estar autenticado.");

        if (string.IsNullOrWhiteSpace(body))
            throw new HttpRequestException($"HTTP {status} {response.ReasonPhrase} (empty body)");

        var contentType = response.Content?.Headers.ContentType?.MediaType;
        if (contentType != null && contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var pd = JsonSerializer.Deserialize<ProblemDetails>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (pd?.Detail != null) throw new HttpRequestException(pd.Detail);
            }
            catch (JsonException je)
            {
                Console.WriteLine("DEBUG: JSON parse failed: " + je.Message);
                throw new HttpRequestException($"HTTP {status} {response.ReasonPhrase}. Body not valid JSON.");
            }
        }

        throw new HttpRequestException($"HTTP {status} {response.ReasonPhrase}. Body: {body}");

    }
    #endregion


}
