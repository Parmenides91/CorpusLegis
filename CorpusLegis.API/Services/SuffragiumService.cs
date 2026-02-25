using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.Shared.Dtos.Suffragium;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CorpusLegis.API.Services;

public class SuffragiumService : ISuffragiumService
{
    private readonly CorpusLegisContext _db;

    public SuffragiumService(CorpusLegisContext db)
    {
        _db = db;
    }

    public async Task<SuffragiumDetailsDto> CreateAsync(CreateSuffragiumDto newSuffragium)
    {
        Guid CivisDefaultGuid = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"); // Sempronio
        Guid CivitasDefaultGuid = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"); // Solfamidas

        bool alreadyVoted = await _db.Suffragia.AnyAsync(s => s.RogatioId == newSuffragium.RogatioId && s.CivisId == CivisDefaultGuid);

        if (alreadyVoted)
        {
            throw new InvalidOperationException("El Civis ya ha emitido un voto para esta Rogatio.");
        }

        Suffragium suffragium = new Suffragium
        {
            Id = Guid.NewGuid(),
            RogatioId = newSuffragium.RogatioId,
            Rogatio = _db.Rogationes.FirstOrDefault(c => c.Id == newSuffragium.RogatioId)!,

            //CivisId = newSuffragium.CivisId,
            CivisId = CivisDefaultGuid, // Sempronio // TODO: esto está hardcodeado.

            //Civis = _db.Cives.FirstOrDefault(c => c.Id == newSuffragium.CivisId)!,
            //Civis = _db.Cives.FirstOrDefault(c => c.Id == Guid.Parse("0f8fad5b - d9cb - 469f - a165 - 70867728950e"))!,

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
