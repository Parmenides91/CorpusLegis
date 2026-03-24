using CorpusLegis.Shared.Dtos;

namespace CorpusLegis.Web.Clients.Rogatio;

public interface IRogatioClient
{
    Task<RogatioDetailsDto?> GetRogatioByIdAsync(Guid id); // Método para obtener un rogatio por su ID.

    Task<List<RogatioSummaryDto>> GetRogationesAsync(); // Método para obtener la lista de rogationes.

    Task<RogatioDetailsDto?> CreateRogatioAsync(CreateRogatioDto dto); // Método para crear un nuevo rogatio.

    Task<RogatioDetailsDto?> UpdateRogatioAsync(Guid id, UpdateRogatioDto dto); // Método para actualizar un rogatio existente.

    Task<bool> DeleteRogatioAsync(Guid id); // Método para eliminar un rogatio por su ID.

    Task<RogatioDetailsDto?> ChangeRogatioStatusAsync(Guid id, WorkflowRogatioDto dto); // Método para cambiar el estado de un rogatio (workflow).

    Task<RogatioDetailsDto?> EvaluateRogatioAsync(Guid id, WorkflowRogatioDto dto); // Método para evaluar una rogatio.
}
