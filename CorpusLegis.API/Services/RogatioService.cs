using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace CorpusLegis.API.Services;

public class RogatioService : IRogatioService
{
    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    //Guid CivisDefaultGuid = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"); // Sempronio
    Guid CivitasDefaultGuid = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"); // Solfamidas

    public RogatioService(CorpusLegisContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }



    public async Task<List<RogatioSummaryDto>> GetAllAsync()
    {
        return await _db.Rogationes
                .Select(r => new RogatioSummaryDto (
                    r.Id,
                    r.Title,
                    r.Content,
                    r.CivisId,
                    r.CreatedAt,
                    r.Status
                    ))
                .AsNoTracking()
                .ToListAsync();
    }

    public async Task<RogatioDetailsDto?> GetByIdAsync(Guid id)
    {
        //var currentCivisId = CivisDefaultGuid; // TODO: esto está hardcodeado, debería venir del contexto de autenticación
        var civisId = _currentUser.CivisId; // TODO: proviene del servicio de mockeo.

        var dto = await _db.Rogationes
            .Where(r => r.Id == id)
            .Select(r => new RogatioDetailsDto(
                r.Id,
                r.Title,
                r.Content,
                r.CivisId,
                r.Civis.Name,
                r.CivitasId,
                r.Civitas.Name,
                r.CreatedAt,
                r.Status,
                r.Suffragia.Count(s => s.Votum == SuffragiumValue.Pro),
                r.Suffragia.Count(s => s.Votum == SuffragiumValue.Contra),
                r.Suffragia.Count(s => s.Votum == SuffragiumValue.Abstentio),
                r.Suffragia.Any(s => s.CivisId == civisId) // TODO: proviene del servicio de mockeo.
                ))
            .AsNoTracking()
            .FirstOrDefaultAsync();


        if (dto == null)
        {
            throw new NotFoundException("rOgAtIo", id);
            //return null;
        }

        return dto;
    }

    public async Task<RogatioDetailsDto> CreateAsync(CreateRogatioDto newRogatio)
    {
        var civisId = _currentUser.CivisId;

        Rogatio rogatio = new Rogatio
        {
            Id = Guid.NewGuid(),
            Title = newRogatio.Title,
            Content = newRogatio.Content,
            CivisId = civisId, // TODO: proviene del servicio de mockeo.
            CivitasId = CivitasDefaultGuid, // TODO: esto está hardcodeado
            CreatedAt = DateTime.UtcNow,
            Status = newRogatio.Status
        };

        _db.Rogationes.Add(rogatio);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(rogatio.Id);
    }

    public async Task<RogatioDetailsDto?> UpdateAsync(Guid id, UpdateRogatioDto updatedRogatio)
    {
        var civisId = _currentUser.CivisId;

        var existingRogatio = await _db.Rogationes.FindAsync(id);

        if (existingRogatio == null)
        {
            return null;
        }

        if (existingRogatio.CivisId != civisId)
        {
            //throw new UnauthorizedAccessException("Sólo el creador de la Rogatio puede editarla.");
            throw new UnauthorizedDomainException("Sólo el creador de la Rogatio puede editarla.");
        }

        if (existingRogatio.Status != RogatioStatus.Inchoatus)
        {
            throw new InvalidOperationException($"No está permitido editar una Rogatio que no está en estado Inchoatus. Estado actual: {existingRogatio.Status}");
        }

        existingRogatio.Title = updatedRogatio.Title;
        existingRogatio.Content = updatedRogatio.Content;
        existingRogatio.CivisId = civisId; // TODO: proviene del servicio de mockeo.
        existingRogatio.CivitasId = CivitasDefaultGuid; // TODO: esto está hardcodeado
        existingRogatio.Status = updatedRogatio.Status;

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var civisId = _currentUser.CivisId;

        var rogatioMeta = await _db.Rogationes
            .Where(r => r.Id == id)
            .Select(r => new { r.CivisId, r.Status }) // me traigo sólo los campos que me interesa validar, para hacer más ligera la consulta.
            .FirstOrDefaultAsync();

        if (rogatioMeta == null)
        {
            return false;
        }

        if (rogatioMeta.CivisId != civisId)
        {
            //throw new UnauthorizedAccessException("Sólo el creador de la Rogatio puede eliminarla.");
            throw new UnauthorizedDomainException("Sólo el creador de la Rogatio puede editarla.");
        }

        if (rogatioMeta.Status != RogatioStatus.Inchoatus)
        {
            throw new InvalidOperationException($"Sólo se pueden borrar Rogatios en estado de Inchoatus. Estado actual: {rogatioMeta.Status}");
        }

        var filasBorradas =  await _db.Rogationes.Where(r => r.Id == id)
                                                .ExecuteDeleteAsync();

        return filasBorradas > 0;
    }




    //// ---------------------------------------------------------
    //// ENDPOINTS (MINIMAL API) para Rogatio.
    //// ---------------------------------------------------------
    //// GET /rogatio --> devuelve la lista de Rogatios
    //app.MapGet("/rogatio", async (CorpusLegisContext db)
    //    => await db.Rogatios
    //        .Select(r => new RogatioSummaryDto (
    //            r.Id,
    //            r.Title,
    //            r.Content,
    //            r.AuthorId,
    //            r.CreatedAt,
    //            r.Status
    //            ))
    //        .AsNoTracking()
    //        .ToListAsync()
    //    );

    //// GET /rogatio/{id} --> devuelve un Rogatio por su id
    //app.MapGet("rogatio/{id:guid}", async (CorpusLegisContext db, Guid id) =>
    //{
    //    var rogatio = await db.Rogatios.FindAsync(id);

    //    if (rogatio == null)
    //    {
    //        return Results.NotFound();
    //    }

    //    RogatioDetailsDto dto = new(
    //        rogatio.Id,
    //        rogatio.Title,
    //        rogatio.Content,
    //        Guid.Empty,
    //        rogatio.CreatedAt,
    //        rogatio.Status
    //    );

    //    return Results.Ok(dto);
    //});

    //// POST /rogatio --> crea un nuevo Rogatio
    //app.MapPost("/rogatio", async (CorpusLegisContext db, CreateRogatioDto newRogatio) => 
    //{
    //    Rogatio rogatio = new Rogatio
    //    {
    //        Id = Guid.NewGuid(),
    //        Title = newRogatio.Title,
    //        Content = newRogatio.Content,
    //        //AuthorId = newRogatio.AuthorId,
    //        CreatedAt = DateTime.UtcNow,
    //        Status = newRogatio.Status
    //    };

    //    db.Rogatios.Add(rogatio);
    //    await db.SaveChangesAsync();

    //    RogatioDetailsDto dto = new(
    //        rogatio.Id,
    //        rogatio.Title,
    //        rogatio.Content,
    //        Guid.Empty,
    //        rogatio.CreatedAt,
    //        rogatio.Status
    //    );

    //    return Results.Created($"/rogatio/{rogatio.Id}", dto); // se devuelve un 201 con el DTO de detalles
    //});

    //// PUT /rogatio/{id} --> actualiza un Rogatio existente por su id
    //app.MapPut("/rogatio/{id:guid}", async (CorpusLegisContext db, Guid id, UpdateRogatioDto updatedRogatio) =>
    //{
    //    //var existingRogatio = await db.Rogatios.FindAsync(updatedRogatio.Id);
    //    var existingRogatio = await db.Rogatios.FindAsync(id);

    //    if (existingRogatio == null)
    //    {
    //        return Results.NotFound();
    //    }

    //    existingRogatio.Title = updatedRogatio.Title;
    //    existingRogatio.Content = updatedRogatio.Content;
    //    //existingRogatio.AuthorId = updatedRogatio.AuthorId;
    //    existingRogatio.Status = updatedRogatio.Status;

    //    await db.SaveChangesAsync();

    //    RogatioDetailsDto dto = new(
    //        existingRogatio.Id,
    //        existingRogatio.Title,
    //        existingRogatio.Content,
    //        Guid.Empty,
    //        existingRogatio.CreatedAt,
    //        existingRogatio.Status
    //    );

    //    return Results.Ok(dto); // se devuelve un 200 con el DTO de detalles actualizado
    //}
    //);

    //// DELETE /rogatio/{id} --> elimina un Rogatio por su id
    //app.MapDelete("/rogatio/{id:guid}", async (CorpusLegisContext db, Guid id) =>
    //{
    //    await db.Rogatios.Where(r => r.Id == id).ExecuteDeleteAsync();

    //    return Results.NoContent(); // se devuelve un 204
    //}
    //);
    //// ---------------------------------------------------------
    ///



    public async Task<RogatioDetailsDto?> ChangeStatusAsync(Guid id, WorkflowRogatioDto workflowRogatio)
    {
        var civisId = _currentUser.CivisId;

        var rogatio = await _db.Rogationes.FindAsync(id);

        if (rogatio == null)
        {
            return null;
        }

        var estadoActual = rogatio.Status;
        RogatioStatus nuevoEstado = workflowRogatio.Status;

        if (rogatio.CivisId != civisId)
        {
            //throw new UnauthorizedAccessException("Sólo el creador de la Rogatio puede progresarla.");
            throw new UnauthorizedDomainException("Sólo el creador de la Rogatio puede progresarla.");
        }

        bool transicionValida = (estadoActual, nuevoEstado) switch
        {
            (RogatioStatus.Inchoatus, RogatioStatus.Proposita) => true,
            (RogatioStatus.Proposita, RogatioStatus.InSuffragium) => true,
            (RogatioStatus.InSuffragium, RogatioStatus.Approbata) => true,
            (RogatioStatus.InSuffragium, RogatioStatus.Reprobata) => true,
            _ => false
        };


        if (!transicionValida) {
            // TODO: aquí habría que lanzar una DomainException personalizada, indicando que la transición no es válida.
            throw new Exception($"Transición no válida: {estadoActual} -> {nuevoEstado}.");
        }

        rogatio.Status = nuevoEstado;

        if (nuevoEstado == RogatioStatus.Approbata)
        {
            var nuevaLex = new Lex
            {
                Id = Guid.NewGuid(),
                CivitasId = rogatio.CivitasId,
                OriginRogatioId = rogatio.Id,
                Title = rogatio.Title,
                Content = rogatio.Content,
                PromulgatedAt = DateTime.UtcNow
            };
            _db.Leges.Add(nuevaLex);
        }

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);

    }

}