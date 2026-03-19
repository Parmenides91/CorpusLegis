using CorpusLegis.Web.State;

namespace CorpusLegis.Web.Clients;

public abstract class CorpusLegisApiClientBase
{
    protected readonly ILogger _logger;
    protected readonly HttpClient _httpClient;

    protected CorpusLegisApiClientBase(ILogger logger, HttpClient httpClient, TokenProvider tokenProvider)
    {
        _logger = logger;
        _httpClient = httpClient;

        if (!string.IsNullOrEmpty(tokenProvider.AccessToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
        }
    }



    #region Excepciones
    protected async Task HandleNonSuccessResponseAsync(HttpResponseMessage response)
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
