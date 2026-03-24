using CorpusLegis.API.Domain;
using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Dtos.Civitas;

namespace CorpusLegis.API.Services;

public interface ICivitasService
{
    Task<List<CivitasSummaryDto>> GetAllAsync();

    Task<CivitasDetailsDto?> GetByIdAsync(Guid id);

    Task<CivitasDetailsDto> CreateAsync(CreateCivitasDto dto); // Crea una nueva Civitas y devuelve sus detalles.

    Task<CivitasDetailsDto?> UpdateAsync(Guid id, UpdateCivitasDto dto); // Actualiza una Civitas existente y devuelve sus detalles actualizados.

    Task<bool> DeleteAsync(Guid id); // Elimina una Civitas por su ID.


    Task<List<CivitasSummaryDto>> GetCivitatesForCurrentUserAsync(); // TODO: cambia user por civis.

    Task<List<CivitasSummaryDto>> GetCivitatesForUserAsync(Guid id); // TODO: cambia user por civis.


    Task AddCurrentCivisToCivitas(Guid civitasId);

    Task AddCivisToCivitas(Guid civitasId, Guid civisId);


    Task RemoveCurrentCivisFromCivitas(Guid civitasId);

    Task RemoveCivisFromCivitas(Guid civitasId, Guid civisId);
}
