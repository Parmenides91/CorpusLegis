using CorpusLegis.API.Data;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Shared.Dtos.Lex;
using Microsoft.EntityFrameworkCore;

namespace CorpusLegis.API.Services;

public class LexService : ILexService
{
    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    public LexService(CorpusLegisContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
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



    // Estos dos métodos hacen casi lo mismo, por lo que podemos hacer algo para que sea un copy-paste uno del otro.
    //public async Task<List<LexSummaryDto>> GetAllByCurrentCivisAsync()
    //{
    //    var civisId = _currentUser.CivisId;

    //    var civitasIdsQuery = _db.Cives
    //        .Where(c => c.Id == civisId)
    //        .SelectMany(c => c.Civitates.Select(ct => ct.Id));

    //    return await _db.Leges
    //        .AsNoTracking()
    //        .Where(l => civitasIdsQuery.Contains(l.CivitasId))
    //        .Select(l => new LexSummaryDto(
    //            l.Id,
    //            l.Title,
    //            l.CivitasId,
    //            l.OriginRogatioId,
    //            l.PromulgatedAt))
    //        .ToListAsync();

    //}
    //public async Task<List<LexSummaryDto>> GetAllByCivisAsync(Guid civisId)
    //{
    //    var civitasIdsQuery = _db.Cives
    //        .Where(c => c.Id == civisId)
    //        .SelectMany(c => c.Civitates.Select(ct => ct.Id));

    //    return await _db.Leges
    //        .AsNoTracking()
    //        .Where(l => civitasIdsQuery.Contains(l.CivitasId))
    //        .Select(l => new LexSummaryDto(
    //            l.Id,
    //            l.Title,
    //            l.CivitasId,
    //            l.OriginRogatioId,
    //            l.PromulgatedAt))
    //        .ToListAsync();
    //}

    private IQueryable<LexSummaryDto> BuildLegesForCivisQuery(Guid civisId)
    {
        //var civitasIdsQuery = _db.Cives
        //    .Where(c => c.Id == civisId)
        //    .SelectMany(c => c.Civitates.Select(ct => ct.Id));

        //return _db.Leges
        //    .AsNoTracking()
        //    .Where(l => civitasIdsQuery.Contains(l.CivitasId))
        //    .Select(l => new LexSummaryDto(
        //        l.Id,
        //        l.Title,
        //        l.CivitasId,
        //        l.OriginRogatioId,
        //        l.PromulgatedAt));

        return _db.Leges
            .AsNoTracking()
            .Where(l => l.Civitas.Cives.Any(c => c.Id == civisId))
            .Select(l => new LexSummaryDto(
                l.Id,
                l.Title,
                l.CivitasId,
                l.OriginRogatioId,
                l.PromulgatedAt));
    }

    public async Task<List<LexSummaryDto>> GetAllByCurrentCivisAsync()
    {
        var civisId = _currentUser.CivisId;
        return await BuildLegesForCivisQuery(civisId).ToListAsync();
    }

    public async Task<List<LexSummaryDto>> GetAllByCivisAsync(Guid civisId)
    {
        return await BuildLegesForCivisQuery(civisId).ToListAsync();
    }


}
