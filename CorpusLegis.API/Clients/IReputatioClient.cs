using CorpusLegis.Shared.Dtos.Reputatio;

namespace CorpusLegis.API.Clients;

public interface IReputatioClient
{
    Task<ReputatioDto?> GetReputatioAsync(Guid civisId, CancellationToken ct = default);
}
