using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Shared.Dtos.Suffragium;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CorpusLegis.API.Services;

public class SuffragiumService : ISuffragiumService
{
    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    // private readonly ILogger<SuffragiumService> _logger; // TODO: incluir este ILogger

    public SuffragiumService(CorpusLegisContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<SuffragiumDetailsDto> CreateAsync(CreateSuffragiumDto newSuffragium)
    {
        //Guid CivisDefaultGuid = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"); // Sempronio
        Guid CivitasDefaultGuid = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"); // Roma

        var civisId = _currentUser.CivisId;

        //bool alreadyVoted = await _db.Suffragia.AnyAsync(s => s.RogatioId == newSuffragium.RogatioId && s.CivisId == civisId);
        //if (alreadyVoted)
        //{
        //    throw new InvalidOperationException("El Civis ya ha emitido un voto para esta Rogatio.");
        //}
        var validationContext = await _db.Rogationes
            .Where(r => r.Id == newSuffragium.RogatioId)
            .Select(r => new
            {
                Exist = true,
                r.Status,
                IsMember = r.Civitas.Cives.Any(c => c.Id == civisId),
                AlreadyVoted = r.Suffragia.Any(s => s.CivisId == civisId)
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
            CivisId = civisId,
            Votum = newSuffragium.Votum,
            CastAt = DateTime.UtcNow
        };

        _db.Suffragia.Add(suffragium);

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
