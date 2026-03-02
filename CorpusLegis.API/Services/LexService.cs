using CorpusLegis.API.Data;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Shared.Dtos.Lex;
using Microsoft.EntityFrameworkCore;

namespace CorpusLegis.API.Services;

public class LexService : ILexService
{
    private readonly CorpusLegisContext _db;

    public LexService(CorpusLegisContext db)
    {
        _db = db;
    }

    public async Task<List<LexSummaryDto>> GetAllAsync()
    {
        return await _db.Leges
            .Select(l => new LexSummaryDto (
                l.Id,
                l.Title,
                l.CivitasId,
                l.OriginRogatioId,
                l.PromulgatedAt
                ))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<LexDetailsDto?> GetByIdAsync(Guid id)
    {
        var dto = await _db.Leges
            .Where(l => l.Id == id)
            .Select(l => new LexDetailsDto (
                l.Id,
                l.Title,
                l.Content,
                l.CivitasId,
                l.OriginRogatioId,
                l.PromulgatedAt
                ))
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (dto == null)
        {
            throw new NotFoundException("Lex no encontrada", id);
        }

        return dto;
    }
}
