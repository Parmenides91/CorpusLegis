using CorpusLegis.API.Services;
using CorpusLegis.Shared.Dtos.Sententia;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace CorpusLegis.API.Endpoints;

public static class SententiaEndpoints
{

    public static void MapSententiaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/sententia")
            .WithTags("Sententia")
            .RequireAuthorization()
            ;

        // Se enlazan las rutas a métodos locales
        group.MapGet("/rogatio/{rogatioId:guid}", GetAllSententiae);
        //group.MapGet("/{id:guid}", GetSententiaById); // ¿queremos ver una Sententia específica?
        group.MapPost("/", CreateSententia);
        group.MapPut("/{id:guid}", UpdateSententia);
        group.MapDelete("/{id:guid}", SoftDeleteSententia);
        group.MapPut("/{id:guid}/restore", RestoreSententia);
    }

    

    private static async Task<IResult> GetAllSententiae(Guid rogatioId, ISententiaService service)
    {
        var sententiae = await service.GetAllAsync(rogatioId);

        return Results.Ok(sententiae);
    }

    private static async Task<IResult> CreateSententia(CreateSententiaDto dto, IValidator<CreateSententiaDto> validator, ISententiaService service)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await service.CreateAsync(dto);

        try
        {
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { errors = ex.Message });
        }

        //return result is not null ? Results.Ok() : Results.InternalServerError();
    }

    private static async Task<IResult> UpdateSententia(Guid id, UpdateSententiaDto dto, ISententiaService service)
    {
        var updated = await service.UpdateAsync(id, dto);

        return updated is not null ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> SoftDeleteSententia(Guid id, ISententiaService service)
    {
        var deleted = await service.SoftDeleteAsync(id);

        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> RestoreSententia(Guid id, ISententiaService service)
    {
        var restored = await service.RestoreAsync(id);

        return restored ? Results.NoContent() : Results.NotFound();
    }

}
