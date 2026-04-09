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

    private readonly ILogger<CivitasService> _logger;

    public CivitasService(CorpusLegisContext db, ICurrentUserService currentUser, ILogger<CivitasService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    

    public async Task<List<CivitasSummaryDto>> GetAllAsync()
    {
        var currentCivisId = _currentUser.CivisId;

        // Opción 01: mediante el skip navigation de Cives en Civitas (no es necesario aquí hacerlo mediante la relación explícita de Civis y Civitas).
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
    }

    public async Task<CivitasDetailsDto?> GetByIdAsync(Guid id)
    {
        var currentCivisId = _currentUser.CivisId;

        // Opción 01: mediante el skip navigation de Cives en Civitas (no es necesario aquí hacerlo mediante la relación explícita de Civis y Civitas).
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

        return dto ?? throw new NotFoundException("Civitas", id);
    }

    public async Task<CivitasDetailsDto> CreateAsync(CreateCivitasDto dto)
    {
        var currentCivisId = _currentUser.CivisId;

        if (currentCivisId == Guid.Empty)
        {
            _logger.LogWarning("Intento de creación de Civitas fallido: usuario actual no identificado (Guid.Empty).");
            throw new UnauthorizedDomainException("Usuario no identificado.");
        }

        var civitas = new Civitas
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Visibility = dto.Visibility,
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

        _logger.LogInformation("Civitas '{CivitasName}' ({CivitasId}) creada con éxito por el usuario {UserId}.", civitas.Name, civitas.Id, currentCivisId);

        return await GetByIdAsync(civitas.Id) ?? throw new Exception("Error interno al tratar de recuperar la Civitas recién creada.");
    }

    public async Task<CivitasDetailsDto?> UpdateAsync(Guid id, UpdateCivitasDto dto)
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
            _logger.LogWarning("El usuario {UserId} intentó editar la Civitas {CivitasId} sin ser Rector ni Magistratus.", currentCivisId, id);
            throw new UnauthorizedDomainException("Sólo Rector o Magistratus de la Civitas pueden editarla.");
        }

        existingCivitas.Name = dto.Name;
        existingCivitas.Description = dto.Description;
        existingCivitas.Visibility = dto.Visibility;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Civitas {CivitasId} actualizada con éxito por el usuario {UserId}.", id, currentCivisId);

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
                _logger.LogWarning("El usuario {UserId} intentó eliminar la Civitas {CivitasId}, pero no existe.", currentCivisId, id);
                return false;
            }

            _logger.LogWarning("El usuario {UserId} intentó eliminar la Civitas {CivitasId} sin ser el Rector (Intento destructivo no autorizado).", currentCivisId, id);
            throw new BusinessRuleValidationException("No tienes permisos para eliminar esta Civitas. Sólo el Rector puede hacerlo.");
        }

        var filasBorradas = await _db.Civitates.Where(c => c.Id == id).ExecuteDeleteAsync(); // si la Civitas tiene Leges, la bbdd no permite el borrado. TODO: avisar o mostrar o controlar esto para que desde la UI se sepa que esto no se puede hacer.

        if (filasBorradas > 0)
        {
            _logger.LogInformation("Civitas {CivitasId} eliminada con éxito por el usuario {UserId}.", id, currentCivisId);
        }

        return filasBorradas > 0;
    }

    public async Task<List<CivitasSummaryDto>> GetCivitatesForCurrentCivisAsync()
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

    public async Task<List<CivitasSummaryDto>> GetCivitatesForCivisAsync(Guid civisId)
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

        // Opción 02: mediante la relación explícita de Civis y Civitas (en este caso no puedo usar el skip navigation).

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

        _logger.LogWarning("El Civis {CivisId} intentó unirse a la Civitas {CivitasId} pero ya era miembro.", currentCivisId, civitasId);

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

        _logger.LogInformation("El Civis {CivisId} se ha unido a la Civitas {CivitasId} bajo el rol Plebeius.", currentCivisId, civitasId);
    }

    public async Task AddCivisToCivitas(Guid civitasId, Guid civisId)
    {

        // Opción 02: mediante la relación explícita de Civis y Civitas (en este caso no puedo usar el skip navigation).

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
        var civis = await _db.Cives.FindAsync(civisId) ?? throw new BusinessRuleValidationException($"El Civis con ID {civisId} no existe.");

        var civitasInfo = await _db.Civitates
            .Include(c => c.Cives)
            .FirstOrDefaultAsync(c => c.Id == civitasId) ?? throw new BusinessRuleValidationException($"La Civitas con ID {civitasId} no existe.");

        if (!civitasInfo.Cives.Any(c => c.Id == civisId))
        {
            throw new BusinessRuleValidationException($"El Civis con ID {civisId} no es miembro de la Civitas con ID {civitasId}");
        }

        civitasInfo.Cives.Remove(civis);

        await _db.SaveChangesAsync();
    }

}
