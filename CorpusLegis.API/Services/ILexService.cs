using CorpusLegis.Shared.Dtos.Lex;

namespace CorpusLegis.API.Services;

public interface ILexService
{
    public Task<List<LexSummaryDto>> GetAllAsync();

    public Task<LexDetailsDto?> GetByIdAsync(Guid id);


    public Task<List<LexSummaryDto>> GetAllByCurrentCivisAsync();

    public Task<List<LexSummaryDto>> GetAllByCivisAsync(Guid civisId);
}
