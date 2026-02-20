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
                    r.AuthorId,
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
