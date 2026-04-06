using CorpusLegis.API.Services;
using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Dtos.Suffragium;
using MiniValidation;

namespace CorpusLegis.API.Endpoints;

public static class SuffragiumEndpoints
{
    public static void MapSuffragiumEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rogatio")
            .WithTags("Rogatio")
            .RequireAuthorization()
            ;

        // Se enlazan las rutas a métodos locales
        group.MapPost("/{id:guid}/vote", CreateSuffragium);
    }


    private static async Task<IResult> CreateSuffragium(Guid id, CreateSuffragiumDto dto, ISuffragiumService service)
    {

        if (dto.RogatioId != id)
        {
            return Results.BadRequest("Route ID does not match DTO RogatioId");
        }

        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors);
        }

        try
        {
            var result = await service.CreateAsync(dto);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { errors = ex.Message });
        }
    }

}
