using CorpusLegis.Shared.Dtos.Civitas;

namespace CorpusLegis.Web.Clients.Civitas;

public interface ICivitasClient
{
    Task<CivitasDetailsDto?> GetCivitasByIdAsync(Guid id); // Método para obtener una civitas por su ID.

    Task<List<CivitasSummaryDto>> GetCivitatesAsync(); // Método para obtener la lista de Civitates.

    Task<CivitasDetailsDto?> CreateCivitasAsync(CreateCivitasDto dto); // Método para crear una nueva Civitas.

    Task<CivitasDetailsDto?> UpdateCivitasAsync(Guid id, UpdateCivitasDto dto); // Método para actualizar una Civitas existente.

    Task<bool> DeleteCivitasAsync(Guid id); // Método para eliminar una Civitas por su ID.

    Task<List<CivitasSummaryDto>> GetUserCivitatesAsync(); // Método para obtener la lista de Civitates a las que pertenece el Civis actual.

    Task<List<CivitasSummaryDto>> GetCivitatesByCivisIdAsync(Guid idCivis); // Método para obtener las Civitates a las que pertenece un Civis. [NO TIENE UNA VISUALIZACIÓN EN LA WEB]

    Task JoinCurrentCivisToCivitasAsync(Guid idCivitas); // Método para agregar el Civis actual al Civitas.

    Task JoinCivisToCivitasAsync(Guid idCivitas, Guid idCivis); // Método para agregar un Civis a un Civitas.

    Task LeaveCurrentCivisFromCivitasAsync(Guid idCivitas); // Método para eliminar el Civis actual del Civitas.

    Task LeaveCivisFromCivitasAsync(Guid idCivitas, Guid idCivis); // Método para eliminar un Civis de un Civitas.

    Task<List<CivitasMemberDto>> GetCivitasMembersAsync(Guid civitasId); // Método para traer los miembros paginados o listados
}
