using CorpusLegis.Shared.Dtos.Reputatio;

namespace CorpusLegis.Web.Clients.Civis;

public interface ICivisClient
{
    public Task<ReputatioDto?> GetReputatioAsync(Guid id); // Obtiene la reputación de un Civis por su ID

}
