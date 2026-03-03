using CorpusLegis.API.Domain;
using CorpusLegis.Shared.Dtos.Civitas;

namespace CorpusLegis.API.Services;

public interface ICivitasService
{
    Task<List<CivitasSummaryDto>> GetAllAsync();

    Task<CivitasDetailsDto?> GetByIdAsync(Guid id);


    Task<List<CivitasSummaryDto>> GetCivitatesForCurrentUserAsync(); // TODO: cambia user por civis.

    Task<List<CivitasSummaryDto>> GetCivitatesForUserAsync(Guid id); // TODO: cambia user por civis.


    Task AddCurrentCivisToCivitas(Guid civitasId);

    Task AddCivisToCivitas(Guid civitasId, Guid civisId);
}
