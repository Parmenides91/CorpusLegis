using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CorpusLegis.API.Services;

public class RogatioService : IRogatioService
{
    private readonly CorpusLegisContext _db;

    public RogatioService(CorpusLegisContext db)
    {
        _db = db;
    }



    public async Task<List<RogatioSummaryDto>> GetAllAsync()
    {
        return await _db.Rogatios
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
        //return await _db.Rogatios.FindAsync(id);

        var rogatio = await _db.Rogatios.FindAsync(id);

        if (rogatio == null)
        {
            return null;
        }

        RogatioDetailsDto dto = new(
            rogatio.Id,
            rogatio.Title,
            rogatio.Content,
            Guid.Empty,
            rogatio.CreatedAt,
            rogatio.Status
        );

        return dto;
    }

    public async Task<RogatioDetailsDto> CreateAsync(CreateRogatioDto newRogatio)
    {
        Rogatio rogatio = new Rogatio
        {
            Id = Guid.NewGuid(),
            Title = newRogatio.Title,
            Content = newRogatio.Content,
            //AuthorId = newRogatio.AuthorId,
            CreatedAt = DateTime.UtcNow,
            Status = newRogatio.Status
        };

        _db.Rogatios.Add(rogatio);
        await _db.SaveChangesAsync();

        RogatioDetailsDto dto = new(
            rogatio.Id,
            rogatio.Title,
            rogatio.Content,
            Guid.Empty,
            rogatio.CreatedAt,
            rogatio.Status
        );

        return dto;
    }

    public async Task<RogatioDetailsDto?> UpdateAsync(Guid id, UpdateRogatioDto updatedRogatio)
    {
        var existingRogatio = await _db.Rogatios.FindAsync(id);

        if (existingRogatio == null)
        {
            return null;
        }

        existingRogatio.Title = updatedRogatio.Title;
        existingRogatio.Content = updatedRogatio.Content;
        //existingRogatio.AuthorId = updatedRogatio.AuthorId;
        existingRogatio.Status = updatedRogatio.Status;

        await _db.SaveChangesAsync();

        RogatioDetailsDto dto = new(
            existingRogatio.Id,
            existingRogatio.Title,
            existingRogatio.Content,
            Guid.Empty,
            existingRogatio.CreatedAt,
            existingRogatio.Status
        );

        return dto;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var filasBorradas =  await _db.Rogatios.Where(r => r.Id == id)
                                                .ExecuteDeleteAsync();

        return filasBorradas > 0;
    }

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