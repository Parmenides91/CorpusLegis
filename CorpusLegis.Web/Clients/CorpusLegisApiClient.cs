using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Dtos.Civitas;
using CorpusLegis.Shared.Dtos.Lex;
using CorpusLegis.Shared.Dtos.Suffragium;
using CorpusLegis.Shared.Validators;
using CorpusLegis.Web.State;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mime;
using System.Text.Json;

namespace CorpusLegis.Web.Clients;

public class CorpusLegisApiClient
{
    private readonly ILogger<CorpusLegisApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly TokenProvider _tokenProvider;

    public CorpusLegisApiClient(HttpClient httpClient, TokenProvider tokenProvider, ILogger<CorpusLegisApiClient> logger)
    {
        _logger = logger;
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


    #region Civitas
    // Método para obtener una civitas por su ID.
    public async Task<CivitasDetailsDto?> GetCivitasByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/{id}");
        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();
    }

    // Método para obtener la lista de Civitates.
    public async Task<List<CivitasSummaryDto>> GetCivitatesAsync()
    {
        // MÉTODO 004: MÉTODO CORRECTO - CON USUARIOS REALES (TOKEN) - CON MANEJO DE ERRORES [este es el que debemos hacer funcionar]
        var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas");
        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();


        // MÉTODO 005: MÉTODO CORRECTO - CON USUARIOS REALES (TOKEN) - CON MANEJO DE ERRORES - CON TRAZAS DE DEPURACIÓN [no sé qué le pasa a este método]
        // INICIO MÉTODO NORMAL - este método es el normal con trazas de debug
        //if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        //{
        //    throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo.");
        //}

        //// Comprobación de rigor: Un JWT debe tener tres partes separadas por puntos.
        //if (!_tokenProvider.AccessToken.Contains('.'))
        //{
        //    throw new InvalidOperationException($"[CRÍTICO] El token obtenido de Keycloak no es un JWT válido. Valor recibido: {_tokenProvider.AccessToken}");
        //}

        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas");
        //request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);

        //var verlo = request.Headers.Authorization?.ToString();
        //Console.WriteLine("[CLIENT] Authorization header preview: " + (request.Headers.Authorization?.ToString() ?? "<null>"));

        //var response = await _httpClient.SendAsync(request);

        //if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400 && response.Headers.Location != null)
        //{
        //    var location = response.Headers.Location;
        //    // Solo reenviar si es el mismo host y esquema (evita filtrar token a terceros)
        //    if (location.IsAbsoluteUri && location.Scheme == _httpClient.BaseAddress.Scheme && location.Host == _httpClient.BaseAddress.Host)
        //    {
        //        var newReq = new HttpRequestMessage(HttpMethod.Get, location);
        //        newReq.Headers.Authorization = request.Headers.Authorization;
        //        response = await _httpClient.SendAsync(newReq);
        //    }
        //    else
        //    {
        //        // Log para depuración
        //        Console.WriteLine("[CLIENT] Redirection to different host or scheme, not re-sending Authorization: " + location);
        //    }
        //}

        //await HandleNonSuccessResponseAsync(response);

        //return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
        // FIN MÉTODO NORMAL - Hasta aquí



        // MÉTODO 006: MÉTODO GUARRADA - CON USUARIOS REALES (TOKEN) [funciona pero es una guarrada]
        //Método para el debug:
        //if (string.IsNullOrEmpty(_tokenProvider.AccessToken))
        //    throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo.");

        //if (!_tokenProvider.AccessToken.Contains('.'))
        //    throw new InvalidOperationException($"[CRÍTICO] El token obtenido de Keycloak no es un JWT válido. Valor recibido: {_tokenProvider.AccessToken}");

        //var request = new HttpRequestMessage(HttpMethod.Get, "/civitas");
        //request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);

        //// Pedimos explícitamente JSON
        //request.Headers.Accept.Clear();
        //request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //Console.WriteLine("[CLIENT] Authorization header preview: " + (request.Headers.Authorization?.ToString() ?? "<null>"));

        //var response = await _httpClient.SendAsync(request);

        //// Manejo de redirecciones manual (ya lo tenías)
        //if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400 && response.Headers.Location != null)
        //{
        //    var location = response.Headers.Location;
        //    if (location.IsAbsoluteUri && location.Scheme == _httpClient.BaseAddress.Scheme && location.Host == _httpClient.BaseAddress.Host)
        //    {
        //        var newReq = new HttpRequestMessage(HttpMethod.Get, location);
        //        newReq.Headers.Authorization = request.Headers.Authorization;
        //        newReq.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //        response = await _httpClient.SendAsync(newReq);
        //    }
        //    else
        //    {
        //        Console.WriteLine("[CLIENT] Redirection to different host or scheme, not re-sending Authorization: " + location);
        //    }
        //}

        //// Loguear status y headers
        //Console.WriteLine("[CLIENT] Status: " + (int)response.StatusCode + " " + response.StatusCode);
        //Console.WriteLine("[CLIENT] Content-Type: " + response.Content.Headers.ContentType?.ToString());

        //// Leer body como string para depuración
        //var body = await response.Content.ReadAsStringAsync();
        //Console.WriteLine("[CLIENT] Body preview: " + (body?.Length > 2000 ? body.Substring(0, 2000) + "..." : body));

        //// Manejar respuestas no exitosas
        //if (!response.IsSuccessStatusCode)
        //{
        //    throw new HttpRequestException($"HTTP {(int)response.StatusCode} {response.StatusCode}. Body: {(string.IsNullOrEmpty(body) ? "<empty>" : body.Substring(0, Math.Min(body.Length, 500)))}");
        //}

        //// Intentar deserializar manualmente
        //try
        //{
        //    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    var result = System.Text.Json.JsonSerializer.Deserialize<List<CivitasSummaryDto>>(body, options);
        //    return result ?? new List<CivitasSummaryDto>();
        //}
        //catch (System.Text.Json.JsonException jex)
        //{
        //    Console.WriteLine("[CLIENT] JSON parse error: " + jex.Message);
        //    throw new HttpRequestException($"HTTP {(int)response.StatusCode} {response.StatusCode}. Body not valid JSON.", jex);
        //}

        // MÉTODO 007: COMPROBAR EL JSON:
        //if (string.IsNullOrWhiteSpace(_tokenProvider.AccessToken))
        //{
        //    throw new InvalidOperationException("[CRÍTICO] El AccessToken es nulo o está vacío en el momento de la petición HTTP.");
        //}

        //var request = new HttpRequestMessage(HttpMethod.Get, "/civitas");
        //request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);

        //// Explicitamente pedimos JSON para evitar que la API devuelva HTML/XML por defecto
        //request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //var response = await _httpClient.SendAsync(request);

        //// Si no es 200-299, procesamos el error como siempre
        //await HandleNonSuccessResponseAsync(response);

        //// AUDITORÍA ESTRICTA: Leemos el contenido como texto plano primero
        //var rawBody = await response.Content.ReadAsStringAsync();

        //// Mostramos los primeros 500 caracteres del cuerpo real que recibimos
        //Console.WriteLine($"[AUDITORÍA CLIENTE] Tipo de contenido: {response.Content.Headers.ContentType}");
        //Console.WriteLine($"[AUDITORÍA CLIENTE] Cuerpo bruto recibido (Primeros 500 chars):\n{(rawBody.Length > 500 ? rawBody.Substring(0, 500) : rawBody)}");

        //try
        //{
        //    // Intentamos la deserialización con opciones permisivas
        //    var options = new System.Text.Json.JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };
        //    var result = System.Text.Json.JsonSerializer.Deserialize<List<CivitasSummaryDto>>(rawBody, options);
        //    return result ?? new List<CivitasSummaryDto>();
        //}
        //catch (System.Text.Json.JsonException jex)
        //{
        //    Console.WriteLine($"[AUDITORÍA CLIENTE CRÍTICO] Fallo de serialización JSON. Razón: {jex.Message}");
        //    throw new HttpRequestException($"HTTP 200 OK pero el cuerpo no es JSON válido para List<CivitasSummaryDto>.", jex);
        //}

    }

    // Método para obtener la lista de Civitates a las que pertenece el Civis actual.
    public async Task<List<CivitasSummaryDto>> GetUserCivitatesAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/civis/me");
        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para obtener las Civitates a las que pertenece un Civis. [NO TIENE UNA VISUALIZACIÓN EN LA WEB]
    public async Task<List<CivitasSummaryDto>> GetCivitatesByCivisIdAsync(Guid idCivis)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/civis/{idCivis}");
        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para agregar el Civis actual al Civitas.
    public async Task JoinCurrentCivisToCivitasAsync(Guid idCivitas)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"/civitas/{idCivitas}/members/me");
        var response = await _httpClient.SendAsync(request);
        await HandleNonSuccessResponseAsync(response);
    }

    // Método para agregar un Civis a un Civitas.
    public async Task JoinCivisToCivitasAsync(Guid idCivitas, Guid idCivis)
    {
        var response = await _httpClient.PostAsync($"/civitas/{idCivitas}/members/{idCivis}", null);
        await HandleNonSuccessResponseAsync(response);
    }

    // Método para eliminar el Civis actual del Civitas.
    public async Task LeaveCurrentCivisFromCivitasAsync(Guid idCivitas)
    {
        var response = await _httpClient.DeleteAsync($"/civitas/{idCivitas}/members/me");
        await HandleNonSuccessResponseAsync(response);
    }

    // Método para eliminar un Civis de un Civitas.
    public async Task LeaveCivisFromCivitasAsync(Guid idCivitas, Guid idCivis)
    {
        var response = await _httpClient.DeleteAsync($"/civitas/{idCivitas}/members/{idCivis}");
        await HandleNonSuccessResponseAsync(response);
    }
    #endregion



    #region Excepciones
    private async Task HandleNonSuccessResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var status = (int)response.StatusCode;
        var body = response.Content == null ? null : await response.Content.ReadAsStringAsync();

        Console.WriteLine($"DEBUG: API response {status} {response.ReasonPhrase}");
        Console.WriteLine("DEBUG: Content-Type: " + (response.Content?.Headers.ContentType?.ToString() ?? "<none>"));
        Console.WriteLine("DEBUG: WWW-Authenticate: " + string.Join(";", response.Headers.WwwAuthenticate.Select(h => h.ToString())));
        Console.WriteLine("DEBUG: Body length: " + (body?.Length ?? 0));
        Console.WriteLine("DEBUG: Body preview: " + (string.IsNullOrEmpty(body) ? "<empty>" : body.Substring(0, Math.Min(400, body.Length))));

        _logger.LogDebug($"DEBUG: API response {status} {response.ReasonPhrase}");
        _logger.LogDebug("DEBUG: Content-Type: " + (response.Content?.Headers.ContentType?.ToString() ?? "<none>"));
        _logger.LogDebug("DEBUG: WWW-Authenticate: " + string.Join(";", response.Headers.WwwAuthenticate.Select(h => h.ToString())));
        _logger.LogDebug("DEBUG: Body length: " + (body?.Length ?? 0));
        _logger.LogDebug("DEBUG: Body preview: " + (string.IsNullOrEmpty(body) ? "<empty>" : body.Substring(0, Math.Min(400, body.Length))));

        if (status == 401)
        {
            throw new UnauthorizedAccessException("La sesión ha expirado o no estás autentificado. Por favor, inicia sesión de nuevo.");
        }
        if (status == 403)
        {
            throw new UnauthorizedAccessException("No tienes permisos suficientes para realizar esta acción.");
        }

        var contentType = response.Content?.Headers.ContentType?.MediaType;

        if (!string.IsNullOrWhiteSpace(body) && contentType != null && contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var problemDetails = System.Text.Json.JsonSerializer.Deserialize<Microsoft.AspNetCore.Mvc.ProblemDetails>(
                    body,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (problemDetails != null && !string.IsNullOrWhiteSpace(problemDetails.Detail))
                {
                    // Aquí lanzamos el error de negocio exacto que la API nos mandó
                    throw new HttpRequestException($"[API Error {status}] {problemDetails.Detail}");
                }
            }
            catch (System.Text.Json.JsonException)
            {
                // Ignoramos el fallo de parseo y caemos al error genérico
            }
        }

        // Fallback genérico si la API devolvió HTML (ej. un error de IIS/Kestrel) o un JSON no estándar
        throw new HttpRequestException($"HTTP {status} {response.ReasonPhrase}. {(string.IsNullOrWhiteSpace(body) ? "Sin detalles adicionales." : body)}");

    }
    #endregion


}
