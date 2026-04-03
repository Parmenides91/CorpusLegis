using CorpusLegis.Shared.Dtos.Sententia;

namespace CorpusLegis.API.Services;

public interface ISententiaService
{
    Task<List<SententiaDto>> GetAllAsync(Guid rogatioId); // Devuelve la lista de Sententiae que pertenecen a una determinada Rogatio.

    // Task<SententiaDto> GetByIdAsync(Guid id); // Devuelve una Sententia específica por su ID.

    Task<CreateSententiaDto> CreateAsync(CreateSententiaDto dto); // Crea una nueva Sententia.

    Task<UpdateSententiaDto> UpdateAsync(Guid id, UpdateSententiaDto dto); // Actualiza una Sententia existente.

    Task<bool> SoftDeleteAsync(Guid id); // Eliminación lógica de una Sententia por su ID.

    Task<bool> RestoreAsync(Guid id); // Deshace la eliminación lógica.

}
