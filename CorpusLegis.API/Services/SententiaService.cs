using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Shared.Dtos.Sententia;
using CorpusLegis.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CorpusLegis.API.Services;

public class SententiaService : ISententiaService
{
    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    private readonly ILogger<SententiaService> _logger;

    public SententiaService(CorpusLegisContext db, ICurrentUserService currentUser, ILogger<SententiaService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    

    public async Task<List<SententiaDto>> GetAllAsync(Guid rogatioId)
    {
        var currentCivisId = _currentUser.CivisId;

        if (currentCivisId == Guid.Empty)
        {
            throw new UnauthorizedDomainException("No eres un Civis autentificado.");
        }

        if (rogatioId == Guid.Empty)
        {
            throw new ArgumentException("El ID de la Rogatio no puede ser vacío.", nameof(rogatioId));
        }

        // se obtiene la Civitas a la que pertenece la Rogatio.
        var civitasId = await _db.Rogationes
                            .Where(r => r.Id == rogatioId)
                            .Select(r => r.CivitasId)
                            .FirstOrDefaultAsync();

        if (civitasId == Guid.Empty)
        {
            throw new NotFoundException($"No existe la Civitas con ID {civitasId}.", civitasId);
        }

        var isMember = await _db.CivitasSodales
                                .AnyAsync(cs => cs.CivisId == currentCivisId && cs.CivitasId == civitasId);

        if (!isMember)
        {
            throw new UnauthorizedDomainException("No tienes permiso para acceder a las Sententiae de esta Rogatio.");
        }

        // ¿Será necesario el rol? De alguna manera tenemos que saber si podemos eliminar cualquier Sententia (si somos Rector) o si sólo vamos a poder borrar aquellos de los que seamos autores.
        var role = await _db.CivitasSodales
                        .Where(cs => cs.CivisId == currentCivisId && cs.CivitasId == civitasId)
                        .Select(cs => cs.Role)
                        .FirstOrDefaultAsync();
        
        bool canSoftDeleteAll = role == Munus.Rector;

        var sententiaeRaw = await _db.Sententiae
                                .AsNoTracking()
                                .Where(s => s.RogatioId == rogatioId)
                                .Select(s => new {
                                    s.Id,
                                    s.ParentId,
                                    s.CivisId,
                                    CivisName = s.Civis.Name,
                                    s.Content,
                                    s.CreatedAt,
                                    s.IsEdited,
                                    s.IsDeleted,
                                })
                                .OrderBy(s => s.CreatedAt)
                                .ToListAsync();

        var dict = new Dictionary<Guid, SententiaDto>(sententiaeRaw.Count);
        foreach (var s in sententiaeRaw)
        {
            var dto = new SententiaDto(
                Id: s.Id,
                ParentId: s.ParentId,
                CivisId: s.CivisId,
                CivisName: s.CivisName,
                Content: s.Content,
                CreatedAt: s.CreatedAt,
                IsEdited: s.IsEdited,
                IsDeleted: s.IsDeleted,
                CanDelete: canSoftDeleteAll || s.CivisId == currentCivisId,
                CanEdit: s.CivisId == currentCivisId && !s.IsDeleted,
                CanRestore: canSoftDeleteAll, // significaría que es Rector.
                Replies: new List<SententiaDto>()
                );
            dict[s.Id] = dto;
        }

        var roots = new List<SententiaDto>(dict.Count);
        foreach (var dto in dict.Values)
        {
            if (dto.ParentId.HasValue && dict.TryGetValue(dto.ParentId.Value, out var parent))
            {
                parent.Replies.Add(dto);
            }
            else
            {
                // sin padre => raíz de hilo.
                roots.Add(dto);
            }
        }

        return roots;
    }

    public async Task<CreateSententiaDto> CreateAsync(CreateSententiaDto dto)
    {
        var currentCivisId = _currentUser.CivisId;
        if (currentCivisId == Guid.Empty)
        {
            throw new UnauthorizedDomainException("No eres un Civis autentificado.");
        }

        // Validar Rogatio y estado.
        var rogatio = await _db.Rogationes
                        .Select(r => new { r.Id, r.CivitasId, r.Status })
                        .FirstOrDefaultAsync(r => r.Id == dto.RogatioId)
                        ?? throw new NotFoundException($"No se ha encontrado la Rogatio {dto.RogatioId}.", dto.RogatioId);

        if (rogatio.Status != RogatioStatus.Proposita)
        {
            //throw new DomainException("Solo se pueden añadir comentarios cuando la Rogatio está en estado Proposita.");
            throw new InvalidOperationException($"Solo se pueden añadir comentarios cuando la Rogatio está en estado Proposita. Estado actual {rogatio.Status}");
        }

        // Validar pertenencia a la Civitas.
        bool isMember = await _db.CivitasSodales.AnyAsync(cs => cs.CivisId == currentCivisId && cs.CivitasId == rogatio.CivitasId);
        if (!isMember)
        {
            throw new UnauthorizedDomainException("No perteneces a la Civitas de la Rogatio, por lo que no puedes añadir Sententiae.");
        }

        // Validar ParentId si existe.
        if (dto.ParentId.HasValue)
        {
            bool parentExists = await _db.Sententiae.AnyAsync(s => s.Id == dto.ParentId.Value && s.RogatioId == dto.RogatioId);
            if (!parentExists)
            {
                throw new NotFoundException($"La Sententia madre {dto.ParentId} no existe en la Rogatio {dto.RogatioId}.", dto.ParentId);
            }
        }

        var sententia = new Sententia
        {
            Id = Guid.NewGuid(),
            RogatioId = dto.RogatioId,
            CivisId = currentCivisId,
            ParentId = dto.ParentId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            IsEdited = false,
        };

        _db.Sententiae.Add(sententia);
        await _db.SaveChangesAsync();

        return dto;

    }

    public async Task<UpdateSententiaDto> UpdateAsync(Guid id, UpdateSententiaDto dto)
    {
        var currentCivisId = _currentUser.CivisId;

        var sententia = await _db.Sententiae.FindAsync(id) ?? throw new NotFoundException("Sententia", id);

        if (sententia.CivisId != currentCivisId)
        {
            throw new UnauthorizedDomainException("Sólo el autor puede editar su Sententia.");
        }

        if (sententia.IsDeleted)
        {
            throw new InvalidOperationException("No se puede editar una Sententia (soft)eliminada.");
        }

        sententia.Content = dto.Content;
        sententia.IsEdited = true;
        // ¿añadir un campo "EditedAt"?

        await _db.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var currentCivisId = _currentUser.CivisId;
        if (currentCivisId == Guid.Empty)
        {
            throw new UnauthorizedDomainException("No eres un Civis autentificado.");
        }

        var sententia = await _db.Sententiae
                            .Include(s => s.Rogatio)
                            .FirstOrDefaultAsync(s => s.Id == id)
                            ?? throw new NotFoundException($"Sententia {id} no encontrada.", id);

        var role = await _db.CivitasSodales
                        .Where(cs => cs.CivisId == currentCivisId && cs.CivitasId == sententia.Rogatio.CivitasId)
                        .Select(cs => cs.Role)
                        .FirstOrDefaultAsync();

        if (sententia.CivisId != currentCivisId && role != Munus.Rector)
        {
            throw new UnauthorizedDomainException($"No tienes permisos para eliminar la Sententia {sententia.Id}.");
        }

        sententia.IsDeleted = true;
        sententia.DeletedByCivisId = currentCivisId;
        sententia.DeletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id)
    {
        var currentCivisId = _currentUser.CivisId;

        var sententia = await _db.Sententiae
                            .Include(s => s.Rogatio)
                            .FirstOrDefaultAsync(s => s.Id == id) ?? throw new NotFoundException("Sententia", id);

        var role = await _db.CivitasSodales
                        .Where(cs => cs.CivisId == currentCivisId && cs.CivitasId == sententia.Rogatio.CivitasId)
                        .Select(cs => cs.Role)
                        .FirstOrDefaultAsync();

        // Sólo autor o Rector pueden restaurar.
        if (sententia.CivisId != currentCivisId && role != Munus.Rector)
        {
            throw new UnauthorizedDomainException($"No tienes permisos para restaurar la Sententia {sententia.Id}.");
        }

        sententia.IsDeleted = false;
        sententia.DeletedByCivisId = null;
        sententia.DeletedAt = null;

        await _db.SaveChangesAsync();
        return true;
    }

}
