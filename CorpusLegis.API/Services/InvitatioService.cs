using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Exceptions;
using CorpusLegis.API.Validators;
using CorpusLegis.Contracts.InvitatioProcess;
using CorpusLegis.Shared.Dtos.Invitatio;
using CorpusLegis.Shared.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace CorpusLegis.API.Services;

public class InvitatioService : IInvitatioService
{

    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    private readonly CreateInvitatioDtoValidator _createValidator = new CreateInvitatioDtoValidator();

    private readonly IPublishEndpoint _publishEndpoint;

    private ILogger<InvitatioService> _logger;

    public InvitatioService(CorpusLegisContext db, ICurrentUserService currentUser, CreateInvitatioDtoValidator createValidator, IPublishEndpoint publishEndpoint, ILogger<InvitatioService> logger)
    {
        _db = db;
        _currentUser = currentUser;

        _createValidator = createValidator;

        _publishEndpoint = publishEndpoint;

        _logger = logger;
    }

    #region emisores
    public async Task<InvitatioDetailsDto> SendAsync(CreateInvitatioDto dto)
    {
        if (_currentUser.CivisId == null)
        {
            _logger.LogError("Intento de acceso a operaciones de Invitatio sin un CivisId en el contexto.");
            throw new UnauthorizedDomainException("No se pudo identificar al usuario actual.");
        }

        // ¿Esto va aquí o va mejor en el endpoint?
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(" | ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new BusinessRuleValidationException($"Errores de validación en la creación de la Rogatio: {errors}");
        }

        var inviterId = _currentUser.CivisId;
        var normalizedEmail = dto.InviteeEmail.Trim().ToLowerInvariant();

        // 1. validar permisos del emisor.
        var hasPermission = await _db.CivitasSodales.AnyAsync(s =>
                                            s.CivitasId == dto.CivitasId &&
                                            s.CivisId == inviterId &&
                                            (s.Role == Munus.Rector || s.Role == Munus.Magistratus));
        if (!hasPermission)
        {
            _logger.LogWarning("Intento de emitir invitación sin privilegios. CivisId: {CivisId}, CivitasId: {CivitasId}", inviterId, dto.CivitasId);
            throw new UnauthorizedDomainException("No posees el rango de Rector o Magistratus en esta Civitas para emitir invitaciones.");
        }

        // 2. identificar al receptor.
        var invitee = await _db.Cives.FirstOrDefaultAsync(c => c.Email.ToLower() == normalizedEmail);
        if (invitee == null)
        {
            throw new BusinessRuleValidationException("No existe ningún Civis registrado con el correo electrónico proporcionado.");
        }
        if (invitee.Id == inviterId)
        {
            throw new BusinessRuleValidationException("No puedes invitarte a ti mismo.");
        }

        // 3. validar pertenencia actual.
        var isAlreadyMember = await _db.CivitasSodales.AnyAsync(s => s.CivitasId == dto.CivitasId && s.CivisId == invitee.Id);
        if (isAlreadyMember)
        {
            throw new BusinessRuleValidationException("El usuario ya es miembro de esta Civitas.");
        }

        // 4. política de enfriamiento.
        var cooldownThreshold = DateTime.UtcNow.AddHours(-24);
        var recentInvitatioExists = await _db.Invitationes.AnyAsync(i =>
                                                i.CivitasId == dto.CivitasId &&
                                                i.InviteeId == invitee.Id &&
                                                i.IssuedAt >= cooldownThreshold);
        if (recentInvitatioExists)
        {
            throw new BusinessRuleValidationException("Se ha emitido una invitación a este usuario recientemente. Debes esperar 24 horas desde el último envío.");
        }

        // 5. generación del Token Criptográfico (url-safe)
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var secureToken = Convert.ToBase64String(tokenBytes)
                            .Replace("+", "-")
                            .Replace("/", "_")
                            .TrimEnd('=');

        // 6. creación de la entidad.
        var invitatio = new Invitatio
        {
            Id = Guid.NewGuid(),
            CivitasId = dto.CivitasId,
            InviterId = (Guid)inviterId,
            InviteeId = invitee.Id,
            Token = secureToken,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Status = InvitationisStatus.Pendens
        };

        _db.Invitationes.Add(invitatio);
        await _db.SaveChangesAsync();

        // rehidratar datos para el dto de retorno de forma eficiente
        var civitasName = await _db.Civitates.Where(c => c.Id == dto.CivitasId).Select(c => c.Name).FirstAsync();
        var inviterName = await _db.Cives.Where(c => c.Id == inviterId).Select(c => c.Name).FirstAsync();

        return new InvitatioDetailsDto(
            invitatio.Id,
            invitatio.CivitasId,
            civitasName,
            inviterName,
            invitee.Name,
            invitee.Email,
            invitatio.IssuedAt,
            invitatio.Status
        );
    }

    public async Task<bool> RevokeAsync(Guid invitatioId)
    {
        var civisId = _currentUser.CivisId;

        var invitatio = await _db.Invitationes
                            .Include(i => i.Civitas)
                            .ThenInclude(c => c.Sodales)
                            .FirstOrDefaultAsync(i => i.Id == invitatioId);

        if (invitatio == null)
        {
            _logger.LogWarning("Invitatio {InvitatioId} no encontrada o no pertenece al usuario actual", invitatioId);
            return false;
        }

        var hasPermission = invitatio.Civitas.Sodales.Any(s => s.CivisId == civisId && (s.Role == Munus.Rector || s.Role == Munus.Magistratus));

        if (!hasPermission)
        {
            _logger.LogWarning("El Civis {CivisId} ha intentado revocar la Invitatio {InvitatioId} sin permisos.", civisId, invitatioId);
            throw new UnauthorizedDomainException("No tienes permisos para revocar invitaciones en esta Civitas.");
        }

        invitatio.Status = InvitationisStatus.Revocata;
        invitatio.RespondedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<List<InvitatioDetailsDto>> GetPendingForCivitasAsync(Guid civitasId)
    {
        return await _db.Invitationes
            .AsNoTracking()
            .Where(i => i.CivitasId == civitasId && i.Status == InvitationisStatus.Pendens && i.ExpiresAt > DateTime.UtcNow)
            .Select(i => new InvitatioDetailsDto(
                i.Id,
                i.CivitasId,
                i.Civitas.Name,
                i.Inviter.Name,
                i.Invitee.Name,
                i.Invitee.Email,
                i.IssuedAt,
                i.Status)
            )
            .ToListAsync();
    }
    #endregion


    #region receptores
    public async Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCivisAsync(Guid civisId)
    {
        return await _db.Invitationes
            .AsNoTracking()
            .Where(i => i.InviteeId == civisId && i.Status == InvitationisStatus.Pendens && i.ExpiresAt > DateTime.UtcNow)
            .Select(i => new InvitatioDetailsDto(
                i.Id,
                i.CivitasId,
                i.Civitas.Name,
                i.Inviter.Name,
                i.Invitee.Name,
                i.Invitee.Email,
                i.IssuedAt,
                i.Status)
            )
            .ToListAsync();
    }

    public async Task<List<InvitatioDetailsDto>> GetPendingInvitationesForCurrentCivisAsync()
    {
        var civisId = _currentUser.CivisId;

        return await _db.Invitationes
            .AsNoTracking()
            .Where(i => i.InviteeId == civisId && i.Status == InvitationisStatus.Pendens && i.ExpiresAt > DateTime.UtcNow)
            .Select(i => new InvitatioDetailsDto(
                i.Id,
                i.CivitasId,
                i.Civitas.Name,
                i.Inviter.Name,
                i.Invitee.Name,
                i.Invitee.Email,
                i.IssuedAt,
                i.Status)
            )
            .ToListAsync();
    }

    public async Task<bool> AcceptAsync(Guid invitatioId)
    {
        var civisId = _currentUser.CivisId;

        var invitatio = await _db.Invitationes.FirstOrDefaultAsync(i => i.Id == invitatioId && i.InviteeId == civisId);

        if (invitatio == null) { 
            _logger.LogWarning("Invitatio {InvitatioId} no encontrada o no pertenece al usuario actual", invitatioId);
            return false;
        }

        if (invitatio.Status != InvitationisStatus.Pendens || invitatio.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Intento de aceptar Invitatio {InvitatioId} inválida o expirada.", invitatioId);
            return false;
        }

        invitatio.Status = InvitationisStatus.Accepta;
        invitatio.RespondedAt = DateTime.UtcNow;
        
        _db.CivitasSodales.Add(new CivitasSodalis
        {
            CivitasId = invitatio.CivitasId,
            CivisId = (Guid)civisId,
            Role = Munus.Plebeius,
            JoinedAt = DateTime.UtcNow
        });

        await _publishEndpoint.Publish(new InvitatioAcceptedIntegrationEvent
        {
            InvitatioId = invitatioId,
            CivitasId = invitatio.CivitasId,
            CivisId = (Guid)civisId,
            ProcessedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return true;

    }

    public async Task<bool> RejectAsync(Guid invitatioId)
    {
        var civisId = _currentUser.CivisId;

        var invitatio = await _db.Invitationes.FirstOrDefaultAsync(i => i.Id == invitatioId && i.InviteeId == civisId);

        if (invitatio == null)
        {
            _logger.LogWarning("Invitatio {InvitatioId} no encontrada o no pertenece al usuario actual", invitatioId);
            return false;
        }

        if (invitatio.Status != InvitationisStatus.Pendens || invitatio.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        invitatio.Status = InvitationisStatus.Repudiata;
        invitatio.RespondedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return true;
    }
    #endregion

}
