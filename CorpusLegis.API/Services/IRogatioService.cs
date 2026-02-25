using CorpusLegis.API.Domain;
using CorpusLegis.Shared.Dtos;

namespace CorpusLegis.API.Services;

public interface IRogatioService // Sólo se definen las firmas.
{

    Task<List<RogatioSummaryDto>> GetAllAsync();

    Task<RogatioDetailsDto?> GetByIdAsync(Guid id);

    Task<RogatioDetailsDto> CreateAsync(CreateRogatioDto dto);

    Task<RogatioDetailsDto?> UpdateAsync(Guid id, UpdateRogatioDto dto);

    Task<bool> DeleteAsync(Guid id);


    Task<RogatioDetailsDto?> ChangeStatusAsync(Guid id, WorkflowRogatioDto dto);

}
