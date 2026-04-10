using CorpusLegis.Shared.Dtos.Civitas;
using CorpusLegis.Web.State;

namespace CorpusLegis.Web.Clients.Civitas;

public class CivitasClient : CorpusLegisApiClientBase, ICivitasClient
{
    //private readonly ILogger<CivitasClient> _logger;
    //private readonly HttpClient _httpClient;
    //private readonly TokenProvider _tokenProvider;

    public CivitasClient(ILogger<CivitasClient> logger, HttpClient httpClient, TokenProvider tokenProvider)
        : base(logger, httpClient, tokenProvider)
    {
        //_logger = logger;
        //_httpClient = httpClient;
        //_tokenProvider = tokenProvider;

        //if (!string.IsNullOrEmpty(tokenProvider.AccessToken))
        //{
        //    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
        //}
    }



    #region Civitas
    // Método para obtener una civitas por su ID.
    public async Task<CivitasDetailsDto?> GetCivitasByIdAsync(Guid id)
    {
        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/{id}");
        //var response = await _httpClient.SendAsync(request);
        var response = await _httpClient.GetAsync($"/civitas/{id}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();
    }

    // Método para obtener la lista de Civitates.
    public async Task<List<CivitasSummaryDto>> GetCivitatesAsync()
    {
        // MÉTODO 004: MÉTODO CORRECTO - CON USUARIOS REALES (TOKEN) - CON MANEJO DE ERRORES [este es el que debemos hacer funcionar]
        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas");
        //var response = await _httpClient.SendAsync(request);
        var response = await _httpClient.GetAsync("/civitas");
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

    // Método para crear una nueva Civitas.
    public async Task<CivitasDetailsDto?> CreateCivitasAsync(CreateCivitasDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/civitas", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();
    }

    // Método para editar una Civitas.
    public async Task<CivitasDetailsDto?> UpdateCivitasAsync(Guid id, UpdateCivitasDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/civitas/{id}", dto);
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<CivitasDetailsDto>();
    }

    // Método para eliminar una Civitas.
    public async Task<bool> DeleteCivitasAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/civitas/{id}");
        await HandleNonSuccessResponseAsync(response);
        return true;
    }

    // Método para obtener la lista de Civitates a las que pertenece el Civis actual.
    public async Task<List<CivitasSummaryDto>> GetUserCivitatesAsync()
    {
        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/civis/me");
        //var response = await _httpClient.SendAsync(request);
        var response = await _httpClient.GetAsync("/civitas/civis/me");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para obtener las Civitates a las que pertenece un Civis. [NO TIENE UNA VISUALIZACIÓN EN LA WEB]
    public async Task<List<CivitasSummaryDto>> GetCivitatesByCivisIdAsync(Guid idCivis)
    {
        //var request = new HttpRequestMessage(HttpMethod.Get, $"/civitas/civis/{idCivis}");
        //var response = await _httpClient.SendAsync(request);
        var response = await _httpClient.GetAsync($"/civitas/civis/{idCivis}");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasSummaryDto>>() ?? new List<CivitasSummaryDto>();
    }

    // Método para agregar el Civis actual al Civitas.
    public async Task JoinCurrentCivisToCivitasAsync(Guid idCivitas)
    {
        //var request = new HttpRequestMessage(HttpMethod.Post, $"/civitas/{idCivitas}/members/me");
        //var response = await _httpClient.SendAsync(request);
        var response = await _httpClient.PostAsync($"/civitas/{idCivitas}/members/me", null);
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

    public async Task<List<CivitasMemberDto>> GetCivitasMembersAsync(Guid civitasId)
    {
        var response = await _httpClient.GetAsync($"/civitas/{civitasId}/members");
        await HandleNonSuccessResponseAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CivitasMemberDto>>() ?? new List<CivitasMemberDto>();
    }
    #endregion



}
