using CorpusLegis.API.Services;

namespace CorpusLegis.API.Endpoints;

public static class CivitasEndpoints
{

    public static void MapCivitasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/civitas")
            .WithTags("Civitas")
            //.RequireAuthorization()
            ;

        group.MapGet("/", GetAllCivitates);
        group.MapGet("/{id:guid}", GetCivitasById);

        group.MapGet("/me", GetCivitatesForCurrentUser); // TODO: cambia user por civis.
        group.MapGet("/civis/{id:guid}", GetCivitatesForUser); // TODO: cambia user por civis.

        group.MapPost("/{civitasId:guid}/members/me", JoinCurrentCivisToCivitas);
        group.MapPost("/{civitasId:guid}/members/{civisId:guid}", JoinCivisToCivitas);

        group.MapDelete("/{civitasId:guid}/members/me", LeaveCurrentCivisFromCivitas);
        group.MapDelete("/{civitasId:guid}/members/{civisId:guid}", LeaveCivisFromCivitas);
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



    private static async Task JoinCurrentCivisToCivitas(Guid civitasId, ICivitasService service)
    {
        await service.AddCurrentCivisToCivitas(civitasId);
    }

    private static async Task JoinCivisToCivitas(Guid civitasId, Guid civisId, ICivitasService service)
    {
        await service.AddCivisToCivitas(civitasId, civisId);
    }



    private static async Task LeaveCurrentCivisFromCivitas(Guid civitasId, ICivitasService service)
    {
        await service.RemoveCurrentCivisFromCivitas(civitasId);
    }

    private static async Task LeaveCivisFromCivitas(Guid civitasId, Guid civisId, ICivitasService service)
    {
        await service.RemoveCivisFromCivitas(civitasId, civisId);
    }

}
