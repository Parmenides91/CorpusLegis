using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Dtos.Suffragium;

namespace CorpusLegis.API.Services;

public interface ISuffragiumService
{
    Task<SuffragiumDetailsDto> CreateAsync(CreateSuffragiumDto dto);
}
