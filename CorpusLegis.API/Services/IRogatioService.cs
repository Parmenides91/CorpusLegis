using CorpusLegis.API.Domain;
using CorpusLegis.Shared.Dtos;

namespace CorpusLegis.API.Services;

public interface IRogatioService // Sólo se definen las firmas.
{

    Task<List<RogatioSummaryDto>> GetAllAsync(); // Devuelve una lista de Rogationes.

    Task<RogatioDetailsDto?> GetByIdAsync(Guid id); // Devuelve los detalles de una Rogato específica.

    Task<RogatioDetailsDto> CreateAsync(CreateRogatioDto dto); // Crea una nueva Rogatio y devuelve sus detalles.

    Task<RogatioDetailsDto?> UpdateAsync(Guid id, UpdateRogatioDto dto); // Actualiza una Rogatio existente y devuelve sus detalles actualizados.

    Task<bool> DeleteAsync(Guid id); // Elimina una Rogatio por su ID.

    Task<RogatioDetailsDto?> ChangeStatusAsync(Guid id, WorkflowRogatioDto dto); // Modificación de los estados de edición, presentación y votación.

    Task<RogatioDetailsDto?> EvalueAsync(Guid id, WorkflowRogatioDto dto); // Decide si se aprueba o se rechaza una Rogatio.

    Task<int> EvaluatePendingAsync(); //Devuelve el número de Rogationes pendientes de deficir si se aprueban o se rechazan.

}
