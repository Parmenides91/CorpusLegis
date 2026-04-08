using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Contracts.ReputatioCalculus;
using CorpusLegis.Shared.Dtos.Suffragium;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CorpusLegis.API.Services;

public class SuffragiumService : ISuffragiumService
{
    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    private readonly IPublishEndpoint _publishEndpoint;

    // private readonly ILogger<SuffragiumService> _logger; // TODO: incluir este ILogger

    public SuffragiumService(CorpusLegisContext db, ICurrentUserService currentUser, IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _currentUser = currentUser;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<SuffragiumDetailsDto> CreateAsync(CreateSuffragiumDto newSuffragium)
    {
        var currentCivisId = _currentUser.CivisId;

        var validationContext = await _db.Rogationes
            .Where(r => r.Id == newSuffragium.RogatioId)
            .Select(r => new
            {
                Exist = true,
                r.Status,
                IsMember = r.Civitas.Cives.Any(c => c.Id == currentCivisId),
                AlreadyVoted = r.Suffragia.Any(s => s.CivisId == currentCivisId)
            })
            .FirstOrDefaultAsync();

        if (validationContext == null)
        {
            throw new NotFoundException("Rogatio", newSuffragium.RogatioId);
        }

        if (validationContext.Status != Shared.Enums.RogatioStatus.InSuffragium)
        {
            throw new BusinessRuleValidationException("La Rogatio no está abierta a votación.");
        }

        if (!validationContext.IsMember)
        {
            throw new UnauthorizedDomainException("No puedes votar en una Rogatio de una Civitas a la que no perteneces.");
        }

        if (validationContext.AlreadyVoted)
        {
            throw new BusinessRuleValidationException("El Civis ya ha emitido un voto para esta Rogatio.");
        }

        Suffragium suffragium = new Suffragium
        {
            Id = Guid.NewGuid(),
            RogatioId = newSuffragium.RogatioId,
            CivisId = currentCivisId,
            Votum = newSuffragium.Votum,
            CastAt = DateTime.UtcNow
        };

        _db.Suffragia.Add(suffragium);

        var evento = new SuffragiumEmissumIntegrationEvent
        {
            CivisId = currentCivisId,
            RogatioId = newSuffragium.RogatioId,
            Timestamp = DateTime.UtcNow,
        };
        await _publishEndpoint.Publish(evento);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Conflicto al guardar el voto.");
        }

        SuffragiumDetailsDto dto = new (
            suffragium.Id,
            suffragium.RogatioId,
            suffragium.Votum
        );

        return dto;
    }
}
