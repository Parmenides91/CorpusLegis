using CorpusLegis.Shared.Dtos.Sententia;

namespace CorpusLegis.API.Services;

public interface ISententiaService
{
    public Task<List<SententiaDto>> GetAllAsync(Guid rogatioId); // Devuelve la lista de Sententiae que pertenecen a una determinada Rogatio.

    // Task<SententiaDto> GetByIdAsync(Guid id); // Devuelve una Sententia específica por su ID.

    public Task<CreateSententiaDto> CreateAsync(CreateSententiaDto dto); // Crea una nueva Sententia.

    public Task<UpdateSententiaDto> UpdateAsync(Guid id, UpdateSententiaDto dto); // Actualiza una Sententia existente.

    public Task<bool> SoftDeleteAsync(Guid id); // Eliminación lógica de una Sententia por su ID.

    public Task<bool> RestoreAsync(Guid id); // Deshace la eliminación lógica.

}
