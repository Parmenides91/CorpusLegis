using CorpusLegis.Shared.Dtos.Civitas;

namespace CorpusLegis.API.Services;

public interface ICivitasService
{
    public Task<List<CivitasSummaryDto>> GetAllAsync();

    public Task<CivitasDetailsDto?> GetByIdAsync(Guid id);

    public Task<CivitasDetailsDto> CreateAsync(CreateCivitasDto dto); // Crea una nueva Civitas y devuelve sus detalles.

    public Task<CivitasDetailsDto?> UpdateAsync(Guid id, UpdateCivitasDto dto); // Actualiza una Civitas existente y devuelve sus detalles actualizados.

    public Task<bool> DeleteAsync(Guid id); // Elimina una Civitas por su ID.


    public Task<List<CivitasSummaryDto>> GetCivitatesForCurrentCivisAsync(); // TODO: cambia user por civis.

    public Task<List<CivitasSummaryDto>> GetCivitatesForCivisAsync(Guid civisId); // TODO: cambia user por civis.


    public Task AddCurrentCivisToCivitas(Guid civitasId);

    public Task AddCivisToCivitas(Guid civitasId, Guid civisId);


    public Task RemoveCurrentCivisFromCivitas(Guid civitasId);

    public Task RemoveCivisFromCivitas(Guid civitasId, Guid civisId);
}
