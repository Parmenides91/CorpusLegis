using CorpusLegis.Shared.Dtos;
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


    // Método para obtener un rogatio por su ID.
    public async Task<RogatioDetailsDto?> GetRogatioByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<RogatioDetailsDto>($"/rogatio/{id}");

        return response;
    }

    // Método para obtener la lista de rogationes.
    public async Task<List<RogatioSummaryDto>> GetRogationesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<RogatioSummaryDto>>("/rogatio");

        return response ?? new List<RogatioSummaryDto>();
    }

    // Método para crear un nuevo rogatio.
    public async Task<RogatioDetailsDto?> CreateRogatioAsync(CreateRogatioDto dto)
    {
        // Hace un POST a la ruta /rogatio enviado el DTO como JSON.
        var response = await _httpClient.PostAsJsonAsync("/rogatio", dto);

        // Se lanzará una excepción si él código HTTP no es exitosa (fuera de 200-299).
        response.EnsureSuccessStatusCode();

        await HandleNonSuccessResponseAsync(response);

        // Lee la respuesta JSON y la convierte al DTO correspondiente.
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para actualizar un rogatio existente.
    public async Task<RogatioDetailsDto?> UpdateRogatioAsync(Guid id, UpdateRogatioDto dto)
    {
        // Hace un PUT a la ruta /rogatio/{id} enviado el DTO como JSON.
        var response = await _httpClient.PutAsJsonAsync($"/rogatio/{id}", dto);


        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

            if (problemDetails?.Errors != null)
            {
                throw new ApiValidationException(problemDetails.Errors);
            }
        }

        // Se lanzará una excepción si él código HTTP no es exitosa (fuera de 200-299).
        response.EnsureSuccessStatusCode();
        // Lee la respuesta JSON y la convierte al DTO correspondiente.
        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }

    // Método para eliminar un rogatio por su ID.
    public async Task<bool> DeleteRogatioAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/rogatio/{id}");
        // Se lanzará una excepción si él código HTTP no es exitosa (fuera de 200-299).
        response.EnsureSuccessStatusCode();

        return true;
    }


    // Método para emitir un voto (suffragium) a un rogatio.
    public async Task<SuffragiumDetailsDto?> CreateSuffragiumAsync(Guid idRogatio, CreateSuffragiumDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"/rogatio/{idRogatio}/vote", dto);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SuffragiumDetailsDto>();
    }


    // Método para cambiar el estado de un rogatio (workflow).
    public async Task<RogatioDetailsDto?> ChangeRogatioStatusAsync(Guid id, WorkflowRogatioDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"/rogatio/{id}/status", dto);
        //if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        //{
        //    var problemDetails = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        //    if (problemDetails?.Errors != null)
        //    {
        //        throw new ApiValidationException(problemDetails.Errors);
        //    }
        //}

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RogatioDetailsDto>();
    }


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


}
