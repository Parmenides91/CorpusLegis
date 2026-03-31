using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Shared.Dtos.Civitas;
using CorpusLegis.Shared.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CorpusLegis.API.Services;

public class CivitasService : ICivitasService
{
    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    private readonly ILogger<CivitasService> _logger; // TODO: incluir este ILogger

    private readonly IValidator<CreateCivitasDto> _createCivitasValidator; // TODO: esto se puede quitar.

    public CivitasService(CorpusLegisContext db, ICurrentUserService currentUser, IValidator<CreateCivitasDto> createCivitasValidator, ILogger<CivitasService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _createCivitasValidator = createCivitasValidator;
        _logger = logger;
    }

    

    public async Task<List<CivitasSummaryDto>> GetAllAsync()
    {
        var currentCivisId = _currentUser.CivisId;

        // Opción 01: mediante el skip navigation de Cives en Civitas.
        return await _db.Civitates
            .AsNoTracking()
            .Where(c => c.Visibility == Visibilitas.Publica || c.Cives.Any(u => u.Id == currentCivisId))
            .Select(c => new CivitasSummaryDto(
                c.Id,
                c.Name,
                c.FoundedAt,
                c.Cives.Any(u => u.Id == currentCivisId),
                c.Sodales.Any(s => s.CivisId == currentCivisId && (s.Role == Munus.Rector || s.Role == Munus.Magistratus)),
                c.Sodales.Any(s => s.CivisId == currentCivisId && s.Role == Munus.Rector)
                ))
            .ToListAsync();

        // Opción 02: mediante la relación explícita de Civis y Civitas.
        //return await _db.Civitates
        //    .AsNoTracking()
        //    .Where(c => c.Visibility == Visibilitas.Publica || c.Sodales.Any(cs => cs.CivisId == currentCivisId))
        //    .Select(c => new CivitasSummaryDto(
        //        c.Id,
        //        c.Name,
        //        c.FoundedAt,
        //        c.Sodales.Any(cs => cs.CivisId == currentCivisId),
        //        c.Sodales.Any(s => s.CivisId == currentCivisId && (s.Role == Munus.Rector || s.Role == Munus.Magistratus)),
        //        c.Sodales.Any(s => s.CivisId == currentCivisId && s.Role == Munus.Rector)
        //        ))
        //    .ToListAsync();
    }

    public async Task<CivitasDetailsDto?> GetByIdAsync(Guid id)
    {
        var currentCivisId = _currentUser.CivisId;

        // Opción 01: mediante el skip navigation de Cives en Civitas.
        var dto = await _db.Civitates
            .AsNoTracking()
            .Where(c => c.Id == id && (c.Visibility == Visibilitas.Publica || c.Cives.Any(u => u.Id == currentCivisId)))
            .Select(c => new CivitasDetailsDto(
                c.Id,
                c.Name,
                c.Description,
                c.Visibility,
                c.FoundedAt,
                c.Sodales.Any(s => s.CivisId == currentCivisId && (s.Role == Munus.Rector || s.Role == Munus.Magistratus)), // CanEdit si es Rector o Magistratus.
                c.Sodales.Any(s => s.CivisId == currentCivisId && s.Role == Munus.Rector) // CanDelete sólo si es Rector.
                ))
            .FirstOrDefaultAsync();

        // Opción 02: mediante la relación explícita de Civis y Civitas.
        //var dto = await _db.Civitates
        //    .AsNoTracking()
        //    .Where(c => c.Id == id && (c.Visibility == Visibilitas.Publica || c.Sodales.Any(cs => cs.CivisId == currentCivisId)))
        //    .Select(c => new CivitasDetailsDto(
        //        c.Id,
        //        c.Name,
        //        c.Description,
        //        c.Visibility,
        //        c.FoundedAt
        //        ))
        //    .FirstOrDefaultAsync();

        if (dto == null)
        {
            throw new NotFoundException("Civitas", id);
        }

        return dto;
    }

    public async Task<CivitasDetailsDto> CreateAsync(CreateCivitasDto newCivitas)
    {
        var currentCivisId = _currentUser.CivisId;

        if (currentCivisId == Guid.Empty)
        {
            throw new UnauthorizedDomainException("Usuario no identificado.");
        }

        var civitas = new Civitas
        {
            Id = Guid.NewGuid(),
            Name = newCivitas.Name,
            Description = newCivitas.Description,
            Visibility = newCivitas.Visibility,
            FoundedAt = DateTime.UtcNow,
            Sodales = new List<CivitasSodalis>
            {
                new CivitasSodalis
                {
                    CivisId = currentCivisId,
                    Role = Munus.Rector,
                    JoinedAt = DateTime.UtcNow
                }
            }
        };

        _db.Civitates.Add(civitas);

        await _db.SaveChangesAsync();

        return await GetByIdAsync(civitas.Id) ?? throw new Exception("Error interno al tratar de recuperar la Civitas recién creada.");
    }

    public async Task<CivitasDetailsDto?> UpdateAsync(Guid id, UpdateCivitasDto updatedCivitas)
    {
        var currentCivisId = _currentUser.CivisId;

        var existingCivitas = await _db.Civitates.FindAsync(id);

        if (existingCivitas == null)
        {
            return null;
        }

        var hasPermissions = await _db.CivitasSodales.AnyAsync(s => s.CivitasId == id && s.CivisId == currentCivisId && (s.Role == Munus.Rector || s.Role == Munus.Magistratus));
        if (!hasPermissions) // sólo Rector o Magistratus de la Civitas pueden editarla.
        {
            throw new UnauthorizedDomainException("Sólo Rector o Magistratus de la Civitas pueden editarla.");
        }

        existingCivitas.Name = updatedCivitas.Name;
        existingCivitas.Description = updatedCivitas.Description;
        existingCivitas.Visibility = updatedCivitas.Visibility;

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var currentCivisId = _currentUser.CivisId;

        var isRector = await _db.CivitasSodales.AnyAsync(s => s.CivitasId == id && s.CivisId == currentCivisId && s.Role == Munus.Rector);
        if (!isRector) // sólo el Rector de la Civitas puede eliminarla.
        {
            // verificamos si la civitas existe para dar un 404 en vez de un 403.
            var exists = await _db.Civitates.AnyAsync(c => c.Id == id);
            if (!exists)
            {
                return false;
            }

            throw new BusinessRuleValidationException("No tienes permisos para eliminar esta Civitas. Sólo el Rector puede hacerlo.");
        }

        // TODO: ¿crear un aviso de que se va a eliminar una Civitas que tiene Civis en ella?

        var filasBorradas = await _db.Civitates.Where(c => c.Id == id).ExecuteDeleteAsync();

        return filasBorradas > 0;
    }

    public async Task<List<CivitasSummaryDto>> GetCivitatesForCurrentUserAsync()
    {
        var currentCivisId = _currentUser.CivisId;

        return await _db.Civitates
            .AsNoTracking()
            .Where(civitas => civitas.Cives.Any(civis => civis.Id == currentCivisId))
            .Select(c => new CivitasSummaryDto (
                c.Id,
                c.Name,
                c.FoundedAt,
                c.Sodales.Any(cs => cs.CivisId == currentCivisId),
                c.Sodales.Any(s => s.CivisId == currentCivisId && (s.Role == Munus.Rector || s.Role == Munus.Magistratus)),
                c.Sodales.Any(s => s.CivisId == currentCivisId && s.Role == Munus.Rector)
                ))
            .ToListAsync();
    }

    public async Task<List<CivitasSummaryDto>> GetCivitatesForUserAsync(Guid civisId)
    {
        return await _db.Civitates
            .AsNoTracking()
            .Where(civitas => civitas.Cives.Any(civis => civis.Id == civisId))
            .Select(c => new CivitasSummaryDto(
                c.Id,
                c.Name,
                c.FoundedAt,
                c.Sodales.Any(cs => cs.CivisId == civisId),
                c.Sodales.Any(s => s.CivisId == civisId && (s.Role == Munus.Rector || s.Role == Munus.Magistratus)),
                c.Sodales.Any(s => s.CivisId == civisId && s.Role == Munus.Rector)
                ))
            .ToListAsync();
    }



    public async Task AddCurrentCivisToCivitas(Guid civitasId)
    {
        // Opción 01: a través del skip navigation no defino rol explícitamente y necesito hacerlo.
        // -no me vale-

        // Opción 02: mediante la relación explícita de Civis y Civitas.
        var currentCivisId = _currentUser.CivisId;

        if (currentCivisId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("No se ha podido determinar el Civis actual.");
        }

        var civitasExists = await _db.Civitates.AnyAsync(c => c.Id == civitasId);

        if (!civitasExists)
        {
            throw new NotFoundException("Civitas", civitasId);
        }

        var isAlreadyMember = await _db.CivitasSodales.AnyAsync(cs => cs.CivitasId == civitasId && cs.CivisId == currentCivisId);

        if (isAlreadyMember)
        {
            throw new BusinessRuleValidationException($"El Civis con ID {currentCivisId} ya es miembro de la Civitas con ID {civitasId}.");
        }

        var sodalis = new CivitasSodalis
        {
            CivitasId = civitasId,
            CivisId = currentCivisId,
            Role = Munus.Plebeius,
            JoinedAt = DateTime.UtcNow
        };

        _db.CivitasSodales.Add(sodalis);

        await _db.SaveChangesAsync();
    }

    public async Task AddCivisToCivitas(Guid civitasId, Guid civisId)
    {
        // Opción 01: a través del skip navigation no defino rol explícitamente y necesito hacerlo.
        // -no me vale-

        // Opción 02: mediante la relación explícita de Civis y Civitas.
        if (civisId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("No se ha podido determinar el Civis actual.");
        }
        var civitasExists = await _db.Civitates.AnyAsync(c => c.Id == civitasId);

        if (!civitasExists)
        {
            throw new NotFoundException("Civitas", civitasId);
        }

        var isAlreadyMember = await _db.CivitasSodales.AnyAsync(cs => cs.CivitasId == civitasId && cs.CivisId == civisId);

        if (isAlreadyMember)
        {
            throw new BusinessRuleValidationException($"El Civis con ID {civisId} ya es miembro de la Civitas con ID {civitasId}.");
        }

        var sodalis = new CivitasSodalis
        {
            CivitasId = civitasId,
            CivisId = civisId,
            Role = Munus.Plebeius,
            JoinedAt = DateTime.UtcNow
        };

        _db.CivitasSodales.Add(sodalis);

        await _db.SaveChangesAsync();

    }


    public async Task RemoveCurrentCivisFromCivitas(Guid civitasId)
    {
        var currentCivisId = _currentUser.CivisId;
        if (currentCivisId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("No se ha podido determinar el Civis actual.");
        }
        await RemoveCivisFromCivitas(civitasId, currentCivisId);
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

        // TODO: añadir que si es el creador de la Civitas, él no puede abandonarla (++ una chapita de "fundador" en la UI en lugar de "miembro" (¿requerirá modificar la DTO?))

        civitasInfo.Cives.Remove(civis);

        await _db.SaveChangesAsync();
    }

}
