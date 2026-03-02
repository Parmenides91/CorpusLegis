using CorpusLegis.API.Services;

namespace CorpusLegis.API.Endpoints;

public static class CivitasEndpoints
{

    public static void MapCivitasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/civitas").WithTags("Civitas");

        group.MapGet("/", GetAllCivitates);
        group.MapGet("/{id:guid}", GetCivitasById);
        group.MapGet("/me", GetCivitatesForCurrentUser);
        group.MapGet("/civis/{id:guid}", GetCivitatesForUser);
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

    private static async Task<IResult> GetCivitatesForCurrentUser(ICivitasService service)
    {
        var civitates = await service.GetCivitatesForCurrentUserAsync();
        return Results.Ok(civitates);
    }

    private static async Task<IResult> GetCivitatesForUser(Guid id, ICivitasService service)
    {
        var civitates = await service.GetCivitatesForUserAsync(id);
        return Results.Ok(civitates);
    }

}
