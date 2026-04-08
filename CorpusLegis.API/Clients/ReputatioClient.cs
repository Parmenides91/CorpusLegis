using CorpusLegis.Shared.Dtos.Reputatio;

namespace CorpusLegis.API.Clients;

public class ReputatioClient(HttpClient httpClient, ILogger<ReputatioClient> logger) : IReputatioClient
{
    public async Task<ReputatioDto?> GetReputatioAsync(Guid civisId, CancellationToken ct = default)
    {
		try
		{
			var response = await httpClient.GetAsync($"reputatio/{civisId}", ct);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ReputatioDto>(cancellationToken: ct);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            logger.LogWarning("Error consultando Reputatio: {StatusCode}", response.StatusCode);
            return null;
        }
		catch (Exception ex)
        {
            logger.LogError(ex, "Fallo de red al consultar microservicio Reputatio.");
            return null;
		}
    }


}
