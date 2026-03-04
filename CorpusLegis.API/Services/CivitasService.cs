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
        var currentCivisId = _currentUser.CivisId; // TODO: esto proviene del servicio de mockeo de lectura de cabeceras.

        return await _db.Civitates
            .AsNoTracking()
            .Select(c => new CivitasSummaryDto (
                c.Id,
                c.Name,
                c.FoundedAt,
                c.Cives.Any(u => u.Id == currentCivisId)
                ))
            .ToListAsync();
    }

    public async Task<CivitasDetailsDto?> GetByIdAsync(Guid id)
    {
        var dto = await _db.Civitates
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CivitasDetailsDto (
                c.Id,
                c.Name,
                c.FoundedAt
                ))
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



    public async Task AddCurrentCivisToCivitas(Guid civitasId)
    {
        var civisId = _currentUser.CivisId;

        if (civisId == null)
        {
            throw new BusinessRuleValidationException("No se ha podido determinar el Civis actual.");
        }

        var civis = await _db.Cives.FindAsync(civisId);

        if (civis == null)
        {
            throw new BusinessRuleValidationException($"El Civis con ID {civisId} no existe.");
        }

        var civitasInfo = await _db.Civitates
            .Include(c => c.Cives)
            .FirstOrDefaultAsync(c => c.Id == civitasId);

        if (civitasInfo == null)
        {
            throw new BusinessRuleValidationException($"La Civitas con ID {civitasId} no existe.");
        }

        if (civitasInfo.Cives.Any(c => c.Id == civisId))
        {
            throw new BusinessRuleValidationException($"El Civis con ID {civisId} ya es miembro de la Civitas con ID {civitasId}.");
        }

        civitasInfo.Cives.Add(civis);

        await _db.SaveChangesAsync();
    }

    public async Task AddCivisToCivitas(Guid civitasId, Guid civisId)
    {
        var civis = await _db.Cives.FindAsync(civisId);

        if (civis == null)
        {
            throw new BusinessRuleValidationException($"El Civis con ID {civisId} no existe.");
        }

        var civitasInfo = await _db.Civitates
            .Include(c => c.Cives)
            .FirstOrDefaultAsync(c => c.Id == civitasId);

        if (civitasInfo == null)
        {
            throw new BusinessRuleValidationException($"La Civitas con ID {civitasId} no existe.");
        }

        if(civitasInfo.Cives.Any(c => c.Id == civisId))
        {
            throw new BusinessRuleValidationException($"El Civis con ID {civisId} ya es miembro de la Civitas con ID {civitasId}.");
        }

        civitasInfo.Cives.Add(civis);

        await _db.SaveChangesAsync();

    }



    //public async Task RemoveCurrentCivisFromCivitas(Guid civitasId)
    //{
    //    var civisId = _currentUser.CivisId;

    //    if (civisId == null)
    //    {
    //        throw new BusinessRuleValidationException("No se ha podido determinar el Civis actual.");
    //    }

    //    var civis = await _db.Cives.FindAsync(civisId);

    //    if (civis == null)
    //    {
    //        throw new BusinessRuleValidationException($"El Civis con ID {civisId} no existe.");
    //    }

    //    var civitasInfo = await _db.Civitates
    //        .Include(c => c.Cives)
    //        .FirstOrDefaultAsync(c => c.Id == civitasId);

    //    if (civitasInfo == null)
    //    {
    //        throw new BusinessRuleValidationException($"La Civitas con ID {civitasId} no existe.");
    //    }

    //    if (!civitasInfo.Cives.Any(c => c.Id == civisId))
    //    {
    //        throw new BusinessRuleValidationException($"El Civis con ID {civisId} no es miembro de la Civitas con ID {civitasId}");
    //    }

    //    civitasInfo.Cives.Remove(civis);

    //    await _db.SaveChangesAsync();
    //}

    //public async Task RemoveCivisFromCivitas(Guid civitasId, Guid civisId)
    //{
    //    if (civisId == null)
    //    {
    //        throw new BusinessRuleValidationException("No se ha podido determinar el Civis actual.");
    //    }

    //    var civis = await _db.Cives.FindAsync(civisId);

    //    if (civis == null)
    //    {
    //        throw new BusinessRuleValidationException($"El Civis con ID {civisId} no existe.");
    //    }

    //    var civitasInfo = await _db.Civitates
    //        .Include(c => c.Cives)
    //        .FirstOrDefaultAsync(c => c.Id == civitasId);

    //    if (civitasInfo == null)
    //    {
    //        throw new BusinessRuleValidationException($"La Civitas con ID {civitasId} no existe.");
    //    }

    //    if (!civitasInfo.Cives.Any(c => c.Id == civisId))
    //    {
    //        throw new BusinessRuleValidationException($"El Civis con ID {civisId} no es miembro de la Civitas con ID {civitasId}");
    //    }

    //    civitasInfo.Cives.Remove(civis);

    //    await _db.SaveChangesAsync();
    //}

    public async Task RemoveCurrentCivisFromCivitas(Guid civitasId)
    {
        var civisId = _currentUser.CivisId;
        if (civisId == null)
        {
            throw new BusinessRuleValidationException("No se ha podido determinar el Civis actual.");
        }
        await RemoveCivisFromCivitas(civitasId, civisId);
    }

    public async Task RemoveCivisFromCivitas(Guid civitasId, Guid civisId)
    {
        var civis = await _db.Cives.FindAsync(civisId);

        if (civis == null)
        {
            throw new BusinessRuleValidationException($"El Civis con ID {civisId} no existe.");
        }

        var civitasInfo = await _db.Civitates
            .Include(c => c.Cives)
            .FirstOrDefaultAsync(c => c.Id == civitasId);

        if (civitasInfo == null)
        {
            throw new BusinessRuleValidationException($"La Civitas con ID {civitasId} no existe.");
        }

        if (!civitasInfo.Cives.Any(c => c.Id == civisId))
        {
            throw new BusinessRuleValidationException($"El Civis con ID {civisId} no es miembro de la Civitas con ID {civitasId}");
        }

        civitasInfo.Cives.Remove(civis);

        await _db.SaveChangesAsync();
    }

}
