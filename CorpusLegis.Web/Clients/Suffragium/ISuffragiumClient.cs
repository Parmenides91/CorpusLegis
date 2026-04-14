using CorpusLegis.Shared.Dtos.Suffragium;

namespace CorpusLegis.Web.Clients.Suffragium;

public interface ISuffragiumClient
{
    public Task<SuffragiumDetailsDto?> CreateSuffragiumAsync(Guid idRogatio, CreateSuffragiumDto dto); // Método para emitir un voto (suffragium) a un rogatio.

}
