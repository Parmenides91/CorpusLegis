using CorpusLegis.Reputatio.Domain;

namespace CorpusLegis.Reputatio.Services;

public interface IReputatioService
{
    Task<CivisReputatio?> GetByCivisIdAsync(Guid civisId, CancellationToken cancellationToken = default);
}
