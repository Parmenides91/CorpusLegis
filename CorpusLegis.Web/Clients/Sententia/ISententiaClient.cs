using CorpusLegis.Shared.Dtos.Sententia;

namespace CorpusLegis.Web.Clients.Sententia;

public interface ISententiaClient
{
    public Task<List<SententiaDto>> GetAllByRogatioIdAsync(Guid rogatioId); // Lista de todas las Sententiae de una Rogatio.

    public Task<CreateSententiaDto?> CreateSententiaAsync(CreateSententiaDto dto); // Crear una nueva Sententia.

    public Task<UpdateSententiaDto?> UpdateSententiaAsync(Guid id, UpdateSententiaDto dto); // Actualizar una Sententia existente.

    public Task<bool> SoftDeleteSententiaAsync(Guid id); // SoftDelete de una Sententia.

    public Task<bool> RestoreSententiaAsync(Guid id); // Deshace el SoftDelete de una Sententia.
}
