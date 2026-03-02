using CorpusLegis.Shared.Dtos.Civitas;

namespace CorpusLegis.API.Services;

public interface ICivitasService
{
    Task<List<CivitasSummaryDto>> GetAllAsync();

    Task<CivitasDetailsDto?> GetByIdAsync(Guid id);


    Task<List<CivitasSummaryDto>> GetCivitatesForCurrentUserAsync();

    Task<List<CivitasSummaryDto>> GetCivitatesForUserAsync(Guid id);
}
