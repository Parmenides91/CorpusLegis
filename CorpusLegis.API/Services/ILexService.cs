using CorpusLegis.Shared.Dtos.Lex;

namespace CorpusLegis.API.Services;

public interface ILexService
{
    Task<List<LexSummaryDto>> GetAllAsync();

    Task<LexDetailsDto?> GetByIdAsync(Guid id);
}
