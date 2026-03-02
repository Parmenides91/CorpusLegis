using CorpusLegis.API.Data;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Shared.Dtos.Civitas;
using Microsoft.EntityFrameworkCore;

namespace CorpusLegis.API.Services;

public class CivitasService : ICivitasService
{
    private readonly CorpusLegisContext _db;

    public CivitasService(CorpusLegisContext db)
    {
        _db = db;
    }

    public async Task<List<CivitasSummaryDto>> GetAllAsync()
    {
        return await _db.Civitates
            .Select(c => new CivitasSummaryDto (
                c.Id,
                c.Name,
                c.FoundedAt
                ))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CivitasDetailsDto?> GetByIdAsync(Guid id)
    {
        var dto = await _db.Civitates
            .Where(c => c.Id == id)
            .Select(c => new CivitasDetailsDto (
                c.Id,
                c.Name,
                c.FoundedAt
                ))
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (dto == null)
        {
            throw new NotFoundException("Civitas", id);
        }

        return dto;
    }
}
