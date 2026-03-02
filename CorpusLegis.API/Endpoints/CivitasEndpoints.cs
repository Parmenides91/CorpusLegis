using CorpusLegis.API.Services;

namespace CorpusLegis.API.Endpoints;

public static class CivitasEndpoints
{

    public static void MapCivitasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/civitas").WithTags("Civitas");

        group.MapGet("/", GetAllCivitates);
        group.MapGet("/{id:guid}", GetCivitasById);
    }


    private static async Task<IResult> GetAllCivitates(ICivitasService service)
    {
        var civitates = await service.GetAllAsync();

        return Results.Ok(civitates);
    }

    private static async Task<IResult> GetCivitasById(Guid id, ICivitasService service)
    {
        var civitas = await service.GetByIdAsync(id);

        return civitas is not null ? Results.Ok(civitas) : Results.NotFound();
    }


}
