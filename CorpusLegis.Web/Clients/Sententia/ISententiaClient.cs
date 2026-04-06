using CorpusLegis.Shared.Dtos.Sententia;

namespace CorpusLegis.Web.Clients.Sententia;

public interface ISententiaClient
{
    Task<List<SententiaDto>> GetAllByRogatioIdAsync(Guid rogatioId); // Lista de todas las Sententiae de una Rogatio.

    Task<CreateSententiaDto?> CreateSententiaAsync(CreateSententiaDto dto); // Crear una nueva Sententia.

    Task<UpdateSententiaDto?> UpdateSententiaAsync(Guid id, UpdateSententiaDto dto); // Actualizar una Sententia existente.

    Task<bool> SoftDeleteSententiaAsync(Guid id); // SoftDelete de una Sententia.

    Task<bool> RestoreSententiaAsync(Guid id); // Deshace el SoftDelete de una Sententia.
}
