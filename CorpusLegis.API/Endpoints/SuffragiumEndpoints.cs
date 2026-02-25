using CorpusLegis.API.Services;
using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Dtos.Suffragium;
using MiniValidation;

namespace CorpusLegis.API.Endpoints;

public static class SuffragiumEndpoints
{
    public static void MapSuffragiumEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/rogatio").WithTags("Rogatio");

        // Se enlazan las rutas a métodos locales
        group.MapPost("/{id:guid}/vote", CreateSuffragium);
    }


    private static async Task<IResult> CreateSuffragium(CreateSuffragiumDto dto, ISuffragiumService service)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors);
        }

        try
        {
            var result = await service.CreateAsync(dto);

            /* TODO: ¿Quiero devolver la DTO del voto, quiero ir a un listado de votos, quiero ir a la Rogatio y simplemente que se visualice que se ha votado? De ello depende qué devuelvo aquí*/
            return Results.Created($"/rogatio/{result.Id}", result); // si ha ido bien, será un código 201 + el objeto (DTO) creado.
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { errors = ex.Message });
        }
    }

}
