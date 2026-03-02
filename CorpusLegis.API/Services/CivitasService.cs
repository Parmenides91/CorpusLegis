using CorpusLegis.API.Data;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Shared.Dtos.Civitas;
using Microsoft.EntityFrameworkCore;

namespace CorpusLegis.API.Services;

public class CivitasService : ICivitasService
{
    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    public CivitasService(CorpusLegisContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
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

    public async Task<List<CivitasSummaryDto>> GetCivitatesForCurrentUserAsync()
    {
        var civisId = _currentUser.CivisId;

        return await _db.Civitates
            .AsNoTracking()
            .Where(civitas => civitas.Cives.Any(civis => civis.Id == civisId))
            .Select(c => new CivitasSummaryDto (
                c.Id,
                c.Name,
                c.FoundedAt
                ))
            .ToListAsync();
    }

    public async Task<List<CivitasSummaryDto>> GetCivitatesForUserAsync(Guid id)
    {
        return await _db.Civitates
            .AsNoTracking()
            .Where(civitas => civitas.Cives.Any(civis => civis.Id == id))
            .Select(c => new CivitasSummaryDto(
                c.Id,
                c.Name,
                c.FoundedAt
                ))
            .ToListAsync();
    }


}
