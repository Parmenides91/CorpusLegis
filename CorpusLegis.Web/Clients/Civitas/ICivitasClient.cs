using CorpusLegis.Shared.Dtos.Civitas;

namespace CorpusLegis.Web.Clients.Civitas;

public interface ICivitasClient
{
    public Task<CivitasDetailsDto?> GetCivitasByIdAsync(Guid id); // Método para obtener una civitas por su ID.

    public Task<List<CivitasSummaryDto>> GetCivitatesAsync(); // Método para obtener la lista de Civitates.

    public Task<CivitasDetailsDto?> CreateCivitasAsync(CreateCivitasDto dto); // Método para crear una nueva Civitas.

    public Task<CivitasDetailsDto?> UpdateCivitasAsync(Guid id, UpdateCivitasDto dto); // Método para actualizar una Civitas existente.

    public Task<bool> DeleteCivitasAsync(Guid id); // Método para eliminar una Civitas por su ID.

    public Task<List<CivitasSummaryDto>> GetUserCivitatesAsync(); // Método para obtener la lista de Civitates a las que pertenece el Civis actual.

    public Task<List<CivitasSummaryDto>> GetCivitatesByCivisIdAsync(Guid idCivis); // Método para obtener las Civitates a las que pertenece un Civis. [NO TIENE UNA VISUALIZACIÓN EN LA WEB]

    public Task JoinCurrentCivisToCivitasAsync(Guid idCivitas); // Método para agregar el Civis actual al Civitas.

    public Task JoinCivisToCivitasAsync(Guid idCivitas, Guid idCivis); // Método para agregar un Civis a un Civitas.

    public Task LeaveCurrentCivisFromCivitasAsync(Guid idCivitas); // Método para eliminar el Civis actual del Civitas.

    public Task LeaveCivisFromCivitasAsync(Guid idCivitas, Guid idCivis); // Método para eliminar un Civis de un Civitas.

    public Task<List<CivitasMemberDto>> GetCivitasMembersAsync(Guid civitasId); // Método para traer los miembros paginados o listados
}
