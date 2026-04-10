using CorpusLegis.Shared.Dtos.Suffragium;

namespace CorpusLegis.API.Services;

public interface ISuffragiumService
{
    public Task<SuffragiumDetailsDto> CreateAsync(CreateSuffragiumDto dto);
}
